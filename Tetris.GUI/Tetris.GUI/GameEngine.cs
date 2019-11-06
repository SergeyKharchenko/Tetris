using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class GameEngine : IDisposable
    {
        private readonly IFigureCreator FigureCreator = new FigureCreator();
        private readonly PlayingArea PlayingArea = new PlayingArea(new MovingValidator());
        private readonly List<IDisposable> Subscriptions = new List<IDisposable>();
        private readonly IObservable<Point> KeyboardListeningObservable;

        public IObservable<GameState> GameState => GameStateSubject?.AsObservable();
        private BehaviorSubject<GameState> GameStateSubject;

        public GameEngine(IObservable<Point> keyboardListeningObservable)
        {
            KeyboardListeningObservable = keyboardListeningObservable;
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            Subscriptions.ForEach(subscription => subscription.Dispose());
            Subscriptions.Clear();
        }

        public async Task StartAsync()
        {
            Unsubscribe();

            await PlayingArea.RestartAsync(Constants.GameAreaWidth, Constants.GameAreaHeight);
            GameStateSubject = new BehaviorSubject<GameState>(await BuildGameState());
            IDisposable figureLifecycleSubscribe =
                PlayingArea.FigureLifecycle.ObserveOn(Scheduler.Default).Subscribe(OnNextFigureLifecycle);
            Subscriptions.Add(figureLifecycleSubscribe);

            IDisposable keyboardListeningSubscription =
                KeyboardListeningObservable.ObserveOn(Scheduler.Default)
                                           .Subscribe(async offset => { await MoveFigureAsync(offset); });
            Subscriptions.Add(keyboardListeningSubscription);

            IDisposable intervalSubscription =
                Observable.Interval(Constants.Speed).ObserveOn(Scheduler.Default)
                          .Subscribe(async _ => await MoveFigureAsync(new Point(0, 1)));
            Subscriptions.Add(intervalSubscription);
        }

        private async void OnNextFigureLifecycle(FigureLifecycle figureLifecycle)
        {
            switch (figureLifecycle.FigureLifecycleTypes)
            {
                case FigureLifecycleTypes.Init:
                {
                    Figure figure = await FigureCreator.CreateFigureAsync(Constants.GameAreaWidth);
                    await PlayingArea.SetCurrentFigureAsync(figure);
                    break;
                }
            }
            GameStateSubject.OnNext(await BuildGameState());
        }

        private async Task MoveFigureAsync(Point offset)
        {
            await PlayingArea.MoveFigureAsync(offset);
            GameStateSubject.OnNext(await BuildGameState());
        }

        private async Task<GameState> BuildGameState()
        {
            return new GameState
            {
                Cells = await PlayingArea.GetGameCellsAsync()
            };
        }
    }
}