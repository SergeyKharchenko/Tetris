using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public class FigurePosition : ICloneable {
        private readonly bool[,] Position;
        private int Height => Position.GetUpperBound(0) + 1;
        private int Width => Position.GetUpperBound(1) + 1;

        public FigurePosition(bool[,] position) {
            Position = (bool[,])position.Clone();
        }

        public Point[] GetOffset(Point location) {
            var offset = new List<Point>();

            for (var x = 0; x < Height; ++x) {
                for (var y = 0; y < Width; ++y) {
                    bool value = Position[x, y];
                    if (value) {
                        offset.Add(new Point(location.X + x, location.Y + y));
                    }
                }
            }
            return offset.ToArray();
        }

        public Task<Point> GetOffsetAsync(Point point, Point location) {
            point.Offset(location);
            return Task.FromResult(point);
        }

        public async Task<FigurePosition> RotateAsync() {
            return await Task.Run(() => {
                var position = new bool[Width, Height];
                for (var x = 0; x < Width; ++x) {
                    for (var y = 0; y < Height; ++y) {
                        position[x, y] = Position[Height - y - 1, x];
                    }
                }
                return new FigurePosition(position);
            });
        }

        public object Clone() {
            return new FigurePosition(Position);
        }

        public override string ToString() {
            var str = "";

            for (var x = 0; x < Height; x++) {
                for (var y = 0; y < Width; y++) {
                    bool value = Position[x, y];
                    str += value ? "1" : "0";
                    if (y != Position.Length - 1) {
                        str += " ";
                    }
                }
                if (x != Position.Length - 1) {
                    str += Environment.NewLine;
                }
            }
            return str;
        }
    }
}