using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class PlayingArea
    {
        private bool[,] Area;
        private readonly IMovingValidator MovingValidator;

        public int Width => Area.GetUpperBound(0);
        public int Height => Area.GetUpperBound(1);

        private Figure CurrentFigure;

        public BehaviorSubject<FigureLifecycle> FigureLifecycle { get; private set; }

        public PlayingArea(IMovingValidator movingValidator)
        {
            MovingValidator = movingValidator;
        }

        public Task RestartAsync(int width, int height)
        {
            Area = new bool[width, height];
            FigureLifecycle = new BehaviorSubject<FigureLifecycle>(new FigureLifecycle
            {
                FigureLifecycleTypes = FigureLifecycleTypes.Init
            });
            return Task.CompletedTask;
        }

        public void SetCurrentFigure(Figure figure)
        {
            CurrentFigure = figure;
        }

        public async Task MoveFigureAsync(Point delta)
        {
            if (await MovingValidator.ValidateAsync(Area, CurrentFigure, delta))
            {
                await CurrentFigure.MoveAsync(delta);
            }
        }
    }
}