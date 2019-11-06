using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class FigurePosition
    {
        private readonly bool[,] Position;
        private int Size => Position.GetUpperBound(0) + 1;

        public FigurePosition(bool[,] position)
        {
            Position = (bool[,]) position.Clone();
        }

        public Point[] GetOffset(Point location)
        {
            var offset = new List<Point>();
            int size = Size;
            for (var x = 0; x < size; ++x) {
                for (var y = 0; y < size; ++y) {
                    bool value = Position[x, y];
                    if (value)
                    {
                        offset.Add(new Point(location.X + x, location.Y + y));
                    }
                }
            }
            return offset.ToArray();
        }

        public async Task<FigurePosition> RotateAsync()
        {
            return await Task.Run(() =>
            {
                int size = Size;
                var position = new bool[size, size];
                for (var x = 0; x < size; ++x) {
                    for (var y = 0; y < size; ++y) {
                        position[x, y] = Position[size - y - 1, x];
                    }
                }
                return new FigurePosition( position);
            });
        }

        public override string ToString()
        {
            var str = "";
            int size = Size;
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    bool value = Position[x, y];
                    str += value ? "1" : "0";
                    if (y != Position.Length - 1)
                    {
                        str += " ";
                    }
                }
                if (x != Position.Length - 1)
                {
                    str += Environment.NewLine;
                }
            }
            return str;
        }
    }
}