using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class PlayingArea
    {
        private readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        private bool[,] Area;
        private readonly IMovingValidator MovingValidator;

        public int Width => Area.GetUpperBound(0);
        public int Height => Area.GetUpperBound(1);

        private Figure CurrentFigure;

        public IObservable<FigureLifecycle> FigureLifecycle => FigureLifecycleSubject?.AsObservable();
        private BehaviorSubject<FigureLifecycle> FigureLifecycleSubject;

        public PlayingArea(IMovingValidator movingValidator)
        {
            MovingValidator = movingValidator;
        }

        public Task RestartAsync(int width, int height)
        {
            Area = new bool[width, height];
            FigureLifecycleSubject = new BehaviorSubject<FigureLifecycle>(new FigureLifecycle
            {
                FigureLifecycleTypes = FigureLifecycleTypes.Init
            });
            return Task.CompletedTask;
        }

        public async Task SetCurrentFigureAsync(Figure figure)
        {
            CurrentFigure = figure;
            await ProjectFigureAsync();
        }

        private async Task ProjectFigureAsync()
        {
            await Mutex.WaitAsync();
            try
            {
                Point[] offset = CurrentFigure.GetLocationWithOffset();
                foreach (Point point in offset)
                {
                    Area[point.X, point.Y] = true;
                }
            }
            finally
            {
                Mutex.Release();
            }
        }

        public async Task MoveFigureAsync(Point offset)
        {
            if (await MovingValidator.ValidateAsync(Area, CurrentFigure, offset))
            {
                await CurrentFigure.MoveAsync(offset);
                await ProjectFigureAsync();
            }
        }

        public async Task<GameCell[,]> GetGameCellsAsync()
        {
            int width = Width;
            int height = Height;
            var cells = new GameCell[Width, Height];

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y) {
                    bool value = Area[x, y];

                    cells[x, y] = new GameCell
                    {
                        Location = new Point(x, y),
                        Color = value ? Color.DeepSkyBlue : Color.Beige
                    };
                }
            }
            return await Task.FromResult(cells);
        }
    }
}