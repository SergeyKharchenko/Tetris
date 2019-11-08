using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public class GameArea {
        private GameGrid Grid;
        private readonly IMovingValidator MovingValidator;

        public int Width => Grid.Width;
        public int Height => Grid.Height;

        private Figure CurrentFigure;

        public GameArea(IMovingValidator movingValidator) {
            MovingValidator = movingValidator;
        }

        public async Task RestartAsync(int width, int height) {
            await Task.Yield();
            Grid = new GameGrid(width, height);
        }

        public async Task SetCurrentFigureAsync(Figure figure) {
            CurrentFigure = figure;
            await PositionCurrentFigureOnAreaAsync();
        }

        private async Task PositionCurrentFigureOnAreaAsync() {
            await Task.Yield();

            foreach (GameCell cell in Grid.Cast<GameCell>().Where(cell => cell.IsFigure)) {
                cell.IsFigure = false;
            }

            Point[] offset = CurrentFigure.GetPositionWithOffset();
            foreach (Point point in offset) {
                Grid[point].IsFigure = true;
                Grid[point].Color = CurrentFigure.Color;
            }
        }

        public async Task<FigureLifecycleTypes> MoveFigureAsync(Point offset) {
            await Task.Yield();

            if (CurrentFigure == null) {
                return FigureLifecycleTypes.None;
            }

            var lifecycleType = FigureLifecycleTypes.None;
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
            return lifecycleType;
        }

        private async Task CheckLinesOnExplosionAsync() {
            await Task.Yield();
            for (int y = 0; y < Grid.Height; y++) {
                GameCell[] line = Grid[y];
                if (line.All(cell => cell.IsOccupied)) {
                    await MoveAllUpperCellsDownAsync(y);
                }
            }
        }

        private async Task MoveAllUpperCellsDownAsync(int y) {
            await Task.Yield();
            for (int yu = y - 1; yu >= 0; yu--) {
                GameCell[] line = Grid[yu];
                GameCell[] bottomLine = Grid[yu + 1];
                for (int x = 0; x < line.Length; x++) {
                    GameCell cell = line[x];
                    bottomLine[x].Assign(cell);
                    line[x].Clear();
                }
            }
        }

        public async Task<FigureLifecycleTypes> RotateFigureAsync() {
            await Task.Yield();
            if (CurrentFigure == null) {
                return FigureLifecycleTypes.None;
            }
            await CurrentFigure.RotateAsync();
            await PositionCurrentFigureOnAreaAsync();
            return FigureLifecycleTypes.Rotated;
        }

        private async Task KillCurrentFigureAsync() {
            await Task.Yield();
            foreach (GameCell cell in Grid.Cast<GameCell>().Where(cell => cell.IsOccupied)) {
                cell.IsOccupied = true;
                cell.IsFigure = false;
            }
            CurrentFigure = null;
        }

        public async Task<GameGrid> GetGameCellsAsync() {
            await Task.Yield();
            return (GameGrid)Grid.Clone();
        }
    }
}