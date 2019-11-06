using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        private GameCell[,] Area;
        private readonly IMovingValidator MovingValidator;

        public int Width => Area.GetUpperBound(0) + 1;
        public int Height => Area.GetUpperBound(1) + 1;

        private Figure CurrentFigure;

        public IObservable<FigureLifecycle> FigureLifecycle => FigureLifecycleSubject?.AsObservable();
        private BehaviorSubject<FigureLifecycle> FigureLifecycleSubject;

        public PlayingArea(IMovingValidator movingValidator)
        {
            MovingValidator = movingValidator;
        }

        public async Task RestartAsync(int width, int height)
        {
            await Mutex.WaitAsync();

            try
            {
                Area = new GameCell[width, height];
                for (var x = 0; x < Width; ++x) {
                    for (var y = 0; y < Height; ++y) {
                        Area[x, y] = new GameCell();
                    }
                }
            
                FigureLifecycleSubject = new BehaviorSubject<FigureLifecycle>(new FigureLifecycle
                {
                    FigureLifecycleTypes = FigureLifecycleTypes.Init
                });
            }
            finally
            {
                Mutex.Release();
            }
        }

        public async Task SetCurrentFigureAsync(Figure figure)
        {
            await Mutex.WaitAsync();
            try
            {
                CurrentFigure = figure;
                await ProjectFigureAsync();
            }
            finally
            {
                Mutex.Release();
            }
        }

        private Task ProjectFigureAsync()
        {
            foreach (GameCell cell in Area)
            {
                cell.IsFigure = false;
            }

            Point[] offset = CurrentFigure.GetLocationWithOffset();
            foreach (Point point in offset)
            {
                Area[point.X, point.Y].IsFigure = true;
            }

            return Task.CompletedTask;
        }

        public async Task MoveFigureAsync(Point offset)
        {
            await Mutex.WaitAsync();
            try
            {
                if (await MovingValidator.ValidateAsync(Area, CurrentFigure, offset))
                {
                    await CurrentFigure.MoveAsync(offset);
                    await ProjectFigureAsync();
                }
            }
            finally
            {
                Mutex.Release();
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
                    GameCell cell = Area[x, y];

                    cells[x, y] = new GameCell
                    {
                        Location = new Point(x, y),
                        Color = cell.IsFigure
                            ? Color.DeepSkyBlue
                            : cell.IsOccupied
                                ? Color.Red
                                : Constants.BackgroundColor
                    };
                }
            }
            return await Task.FromResult(cells);
        }
    }
}