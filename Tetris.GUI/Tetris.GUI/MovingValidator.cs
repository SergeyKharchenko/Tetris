using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class MovingValidator : IMovingValidator
    {
        public Task<MovingValidationResult> ValidateAsync(GameCell[,] area, Figure figure, Point offset)
        {
            switch(offset)
            {
                case Point p when p.X == 1: return CanMoveRightAsync(area, figure);
                case Point p when p.X == -1: return CanMoveLeftAsync(area, figure);
                case Point p when p.Y == 1: return CanMoveDownAsync(area, figure);
                default:
                    return Task.FromResult(new MovingValidationResult
                    {
                        Allow = true
                    });
            }
        }

        private static Task<MovingValidationResult> CanMoveRightAsync(GameCell[,] area, Figure figure)
        {
            Point[] position = figure.GetPositionWithOffset();
            List<Point> rightPoints = position
                                      .GroupBy(point => point.Y, point => point.X, (y, xs) => new Point(xs.Max(), y))
                                      .ToList();

            List<GameCell> cells = area.Cast<GameCell>().ToList();

            foreach (Point rightPoint in rightPoints)
            {
                int rightBorder = cells.Where(cell => cell.Location.Y == rightPoint.Y && cell.IsOccupied)
                                       .Where(cell => cell.Location.X > rightPoint.X).Select(cell => cell.Location.X)
                                       .DefaultIfEmpty(area.GetUpperBound(0) + 1).Min();
                if (rightBorder == rightPoint.X + 1)
                {
                    return Task.FromResult(new MovingValidationResult
                    {
                        Allow = false
                    });
                }
            }

            return Task.FromResult(new MovingValidationResult
            {
                Allow = true
            });
        }

        private Task<MovingValidationResult> CanMoveLeftAsync(GameCell[,] area, Figure figure)
        {
            Point[] position = figure.GetPositionWithOffset();
            List<Point> leftPoints = position
                                      .GroupBy(point => point.Y, point => point.X, (y, xs) => new Point(xs.Min(), y))
                                      .ToList();

            List<GameCell> cells = area.Cast<GameCell>().ToList();

            foreach (Point leftPoint in leftPoints)
            {
                int rightBorder = cells
                                   .Where(cell => cell.Location.Y == leftPoint.Y && cell.IsOccupied)
                                   .Where(cell => cell.Location.X < leftPoint.X).Select(cell => cell.Location.X)
                                       .DefaultIfEmpty(-1).Max();

                if (rightBorder == leftPoint.X - 1)
                {
                    return Task.FromResult(new MovingValidationResult
                    {
                        Allow = false
                    });
                }
            }

            return Task.FromResult(new MovingValidationResult
            {
                Allow = true
            });
        }

        private Task<MovingValidationResult> CanMoveDownAsync(GameCell[,] area, Figure figure)
        {
            Point[] position = figure.GetPositionWithOffset();
            List<Point> bottomPoints = position
                                      .GroupBy(point => point.X, point => point.Y, (x, ys) => new Point(x, ys.Max()))
                                      .ToList();

            List<GameCell> cells = area.Cast<GameCell>().ToList();

            foreach (Point bottomPoint in bottomPoints)
            {
                int bottomBorder = cells
                                   .Where(cell => cell.Location.X == bottomPoint.X && cell.IsOccupied)
                                   .Where(cell => cell.Location.Y > bottomPoint.Y).Select(cell => cell.Location.Y)
                                       .DefaultIfEmpty(area.GetUpperBound(1) + 1).Min();
                if (bottomBorder == bottomPoint.Y + 1)
                {
                    return Task.FromResult(new MovingValidationResult
                    {
                        Dead = true
                    });
                }
            }

            return Task.FromResult(new MovingValidationResult
            {
                Allow = true
            });
        }
    }
}