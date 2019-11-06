using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public class GameArea {
        private readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        private GameGrid Grid;
        private readonly IMovingValidator MovingValidator;

        public int Width => Grid.Width;
        public int Height => Grid.Height;

        private Figure CurrentFigure;

        public IObservable<FigureLifecycle> FigureLifecycle => FigureLifecycleSubject?.AsObservable();
        private BehaviorSubject<FigureLifecycle> FigureLifecycleSubject;

        public GameArea(IMovingValidator movingValidator) {
            MovingValidator = movingValidator;
        }

        public async Task RestartAsync(int width, int height) {
            await Mutex.WaitAsync();

            try {
                Grid = new GameGrid(width, height);
                FigureLifecycleSubject = new BehaviorSubject<FigureLifecycle>(new FigureLifecycle {
                    FigureLifecycleTypes = FigureLifecycleTypes.Init
                });
            } finally {
                Mutex.Release();
            }
        }

        public async Task SetCurrentFigureAsync(Figure figure) {
            await Mutex.WaitAsync();
            try {
                CurrentFigure = figure;
                await PositionCurrentFigureOnAreaAsync();
            } finally {
                Mutex.Release();
            }
        }

        private Task PositionCurrentFigureOnAreaAsync() {
            foreach (GameCell cell in Grid.Cast<GameCell>().Where(cell => cell.IsFigure)) {
                cell.IsFigure = false;
            }

            Point[] offset = CurrentFigure.GetPositionWithOffset();
            foreach (Point point in offset) {
                Grid[point].IsFigure = true;
                Grid[point].Color = CurrentFigure.Color;
            }

            return Task.CompletedTask;
        }

        public async Task MoveFigureAsync(Point offset) {
            if (CurrentFigure == null) {
                return;
            }

            var lifecycleType = FigureLifecycleTypes.None;

            await Mutex.WaitAsync();
            try {
                MovingValidationResult result = await MovingValidator.ValidateAsync(Grid, CurrentFigure, offset);
                if (result.Allow) {
                    await CurrentFigure.MoveAsync(offset);
                    await PositionCurrentFigureOnAreaAsync();
                    lifecycleType = FigureLifecycleTypes.Moved;
                } else if (result.Dead) {
                    await KillCurrentFigureAsync();
                    await CheckLinesOnExplosionAsync();
                    lifecycleType = FigureLifecycleTypes.Dead;

                }
            } finally {
                Mutex.Release();
            }
            FigureLifecycleSubject.OnNext(new FigureLifecycle {
                FigureLifecycleTypes = lifecycleType
            });
        }

        private async Task CheckLinesOnExplosionAsync() {
            for (int y = 0; y < Grid.Height; y++) {
                GameCell[] line = Grid[y];
                if (line.All(cell => cell.IsOccupied)) {
                    //foreach (var cell in line) {
                    //    cell.IsOccupied = false;
                    //}
                    await MoveAllUpperCellsDownAsync(y);
                }
            }
        }

        private Task MoveAllUpperCellsDownAsync(int y) {
            for (int yu = y - 1; yu >= 0; yu--) {
                GameCell[] line = Grid[yu];
                GameCell[] bottomLine = Grid[yu + 1];
                for (int x = 0; x < line.Length; x++) {
                    GameCell cell = line[x];
                    bottomLine[x].Assign(cell);
                    line[x].Clear();
                }
            }
            return Task.CompletedTask;
        }

        public async Task RotateFigureAsync() {
            if (CurrentFigure == null) {
                return;
            }

            var lifecycleType = FigureLifecycleTypes.None;

            await Mutex.WaitAsync();
            try {
                await CurrentFigure.RotateAsync();
                await PositionCurrentFigureOnAreaAsync();
                lifecycleType = FigureLifecycleTypes.Rotated;
            } finally {
                Mutex.Release();
            }
            FigureLifecycleSubject.OnNext(new FigureLifecycle {
                FigureLifecycleTypes = lifecycleType
            });
        }

        private Task KillCurrentFigureAsync() {
            foreach (GameCell cell in Grid.Cast<GameCell>().Where(cell => cell.IsOccupied)) {
                cell.IsOccupied = true;
                cell.IsFigure = false;
            }

            CurrentFigure = null;
            return Task.CompletedTask;
        }

        public async Task<GameGrid> GetGameCellsAsync() {
            return await Task.FromResult((GameGrid)Grid.Clone());
        }
    }
}