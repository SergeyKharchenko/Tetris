using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class GameEngine : IDisposable
    {
        private readonly RegistryFigureCreator FigureCreator = new RegistryFigureCreator();
        private readonly PlayingArea PlayingArea = new PlayingArea(new MovingValidator());
        private readonly List<IDisposable> Subscriptions = new List<IDisposable>();

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

            await PlayingArea.RestartAsync(10, 20);
            IDisposable figureLifecycleSubscribe =
                PlayingArea.FigureLifecycle.ObserveOn(Scheduler.Default).Subscribe(OnNextFigureLifecycle);
            Subscriptions.Add(figureLifecycleSubscribe);
        }

        private async void OnNextFigureLifecycle(FigureLifecycle figureLifecycle)
        {
            switch (figureLifecycle.FigureLifecycleTypes)
            {
                case FigureLifecycleTypes.Init:
                {
                    Figure figure = await FigureCreator.CreateFigureAsync(new Point(PlayingArea.Width / 2, 0));
                    PlayingArea.SetCurrentFigure(figure);
                    break;
                }
            }
        }
    }
}