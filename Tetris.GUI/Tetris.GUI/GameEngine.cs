using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Tetris.GUI {
    public class GameEngine : IDisposable {
        private readonly IFigureCreator _figureCreator = new FigureCreator();
        private readonly GameArea _playingArea = new GameArea(new MovingValidator());
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
        private readonly IObservable<Point> _navigationObservable;
        private readonly IObservable<bool> _rotationObservable;
        private TransformBlock<ChangeInfo, FigureLifecycleTypes> _changeBlock;
        private TransformBlock<FigureLifecycleTypes, FigureLifecycleTypes> _deadBlock;
        private readonly AsyncManualResetEvent _figureCreatedSignal = new AsyncManualResetEvent();

        public IObservable<GameState> GameState => GameStateSubject?.AsObservable();
        private BehaviorSubject<GameState> GameStateSubject;

        public GameEngine(IObservable<Point> navigationObservable, IObservable<bool> rotationObservable) {
            _navigationObservable = navigationObservable;
            _rotationObservable = rotationObservable;
        }

        public void Dispose() {
            Stop();
            _subscriptions.Dispose();
        }

        private void Stop() {
            _subscriptions.Clear();
            _changeBlock?.Complete();
        }

        public async Task StartAsync() {
            Stop();
            BuildDataflow();

            await _playingArea.RestartAsync(Constants.GameAreaWidth, Constants.GameAreaHeight);

            GameStateSubject = new BehaviorSubject<GameState>(await BuildGameStateAsync());

            IDisposable navigationSubscription =
                _navigationObservable.ObserveOn(Scheduler.Default)
                                           .Subscribe(offset => _changeBlock.Post(ChangeInfo.Move(offset)));
            _subscriptions.Add(navigationSubscription);

            IDisposable rotationSubscription =
                _rotationObservable.ObserveOn(Scheduler.Default)
                                           .Subscribe(_ => _changeBlock.Post(ChangeInfo.Rotate()));
            _subscriptions.Add(rotationSubscription);

            IDisposable intervalSubscription =
                Observable.Interval(Constants.Speed).ObserveOn(Scheduler.Default)
                          .Subscribe(_ => _changeBlock.Post(ChangeInfo.Move(new Point(0, 1))));
            _subscriptions.Add(intervalSubscription);

            _deadBlock.Post(FigureLifecycleTypes.Dead);
        }

        private void BuildDataflow() {
            _changeBlock = new TransformBlock<ChangeInfo, FigureLifecycleTypes>(async info => {
                await _figureCreatedSignal.WaitAsync();
                if (info.IsRotate) {
                    return await _playingArea.RotateFigureAsync();
                }
                return await _playingArea.MoveFigureAsync(info.Offset);
            });
            _deadBlock = new TransformBlock<FigureLifecycleTypes, FigureLifecycleTypes>(async figureLifecycleType => {
                _figureCreatedSignal.Reset();
                Figure figure = await _figureCreator.CreateFigureAsync(Constants.GameAreaWidth);
                await _playingArea.SetCurrentFigureAsync(figure);
                _figureCreatedSignal.Set();
                return figureLifecycleType;
            });
            var collectStateBlock = new TransformBlock<FigureLifecycleTypes, GameState>(async _ => await BuildGameStateAsync());
            var sendStateBlock = new ActionBlock<GameState>(state => {
                GameStateSubject.OnNext(state);
            });

            _changeBlock.LinkTo(_deadBlock, new DataflowLinkOptions {
                PropagateCompletion = true
            }, figureLifecycleType => figureLifecycleType == FigureLifecycleTypes.Dead);
            _changeBlock.LinkTo(collectStateBlock, new DataflowLinkOptions {
                PropagateCompletion = true
            }, figureLifecycleType => figureLifecycleType != FigureLifecycleTypes.None);
            _changeBlock.LinkTo(DataflowBlock.NullTarget<FigureLifecycleTypes>());

            _deadBlock.LinkTo(collectStateBlock);

            collectStateBlock.LinkTo(sendStateBlock, new DataflowLinkOptions { PropagateCompletion = true });
        }

        private async Task<GameState> BuildGameStateAsync() {
            return new GameState {
                Area = await _playingArea.GetGameCellsAsync()
            };
        }

        private class ChangeInfo {
            public bool IsRotate { get; set; }

            public Point Offset { get; set; }

            public static ChangeInfo Move(Point offset) => new ChangeInfo {
                Offset = offset
            };

            public static ChangeInfo Rotate() => new ChangeInfo {
                IsRotate = true
            };
        }
    }
}