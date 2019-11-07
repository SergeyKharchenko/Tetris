using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tetris.GUI.Tetromino
{
    public class TetrominoGenerator
    {
        private int _size = 5;

        public bool[,] CreateTetromino(int blocksAmount)
        {
            var tetromino = new bool[_size, _size];
            Point startPoint = GetStartPoint(_size);
            tetromino[startPoint.X, startPoint.Y] = true;

            GenerateTetromino(tetromino, blocksAmount);
            tetromino = GetFilledArea(tetromino);

            return tetromino;
        }

        private bool[,] GetFilledArea(bool[,] tetromino)
        {
            var projector = new TetraminoProjector();
            (int left, int top, int right, int bottom) bounds = projector.GetBounds(tetromino);

            int height = Math.Max(1, (bounds.bottom - bounds.top) + 1);
            int width = Math.Max(1, bounds.right - bounds.left + 1);
            var newTetramino = new bool[width, height];

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (i >= bounds.left && i <= bounds.right && j >= bounds.top && j <= bounds.bottom)
                    {
                        int x = i - bounds.left;
                        int y = j - bounds.top;
                        newTetramino[x, y] = tetromino[i, j];
                    }
                }
            }

            return newTetramino;
        }

        private Point GetStartPoint(int size)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var x = random.Next(0, size - 1);
            var y = random.Next(0, size - 1);
            return new Point(x, y);
        }

        private void GenerateTetromino(bool[,] tetromino, int blockAmount)
        {
            for (int i = 1; i <= blockAmount - 1; i++)
            {
                FillTetromino(tetromino);
            }
        }

        private List<Point> GetFilledPoints(bool[,] tetromino)
        {
            var result = new List<Point>();
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (tetromino[i, j] == true)
                    {
                        result.Add(new Point(i, j));
                    }
                }
            }
            return result;
        }

        private void FillTetromino(bool[,] tetromino)
        {
            var filledPoints = GetFilledPoints(tetromino);
            var hashSet = new HashSet<Point>();
            foreach (Point filledPoint in filledPoints)
            {
                List<Point> sidePoints = GenerateSidePoints(tetromino, filledPoint.X, filledPoint.Y);
                sidePoints.ForEach(value => hashSet.Add(value));
            }

            var random = new Random((int)DateTime.Now.Ticks);
            var nextPoint = hashSet.ElementAt(random.Next(0, hashSet.Count));
            tetromino[nextPoint.X, nextPoint.Y] = true;
        }

        private List<Point> GenerateSidePoints(bool[,] tetromino, int x, int y)
        {
            var sidePoints = new List<Point>();
            foreach (var value in new[] { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) })
            {
                int newX = value.Item1;
                int newY = value.Item2;

                if (newX == x && newY == y) { continue; }
                if (newX < 0 || newY < 0) { continue; }
                if (newX >= _size || newY >= _size) { continue; }
                if (tetromino[newX, newY] == true) { continue; }
                sidePoints.Add(new Point(newX, newY));

            }
            return sidePoints;
        }

    }
}
