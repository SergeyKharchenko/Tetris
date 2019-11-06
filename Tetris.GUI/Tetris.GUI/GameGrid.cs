using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tetris.GUI {
    public class GameGrid : IEnumerable<GameCell>, ICloneable {
        public GameCell[,] Cells;

        public int Width => Cells.GetUpperBound(0) + 1;
        public int Height => Cells.GetUpperBound(1) + 1;

        public GameCell this[int x, int y] {
            get {
                return Cells[x, y];
            }
            set {
                Cells[x, y] = value;
            }
        }

        public GameCell this[Point p] {
            get {
                return Cells[p.X, p.Y];
            }
            set {
                Cells[p.X, p.Y] = value;
            }
        }

        public GameCell[] this[int y] {
            get {
                List<GameCell> line = new List<GameCell>(Width);
                for (int x = 0; x < Width; x++) {
                    line.Add(Cells[x, y]);
                }
                return line.ToArray();
            }
        }

        public GameGrid(int width, int height) {
            Cells = new GameCell[width, height];
            for (var x = 0; x < Width; ++x) {
                for (var y = 0; y < Height; ++y) {
                    Cells[x, y] = new GameCell {
                        Location = new Point(x, y)
                    };
                }
            }
        }

        private GameGrid(GameCell[,] cells) {
            var width = cells.GetUpperBound(0) + 1;
            var height = cells.GetUpperBound(1) + 1;
            Cells = new GameCell[width, height];
            for (var x = 0; x < width; ++x) {
                for (var y = 0; y < height; ++y) {
                    Cells[x, y] = (GameCell)cells[x, y].Clone();
                }
            }
        }

        public IEnumerator<GameCell> GetEnumerator() {
            return Cells.Cast<GameCell>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public object Clone() {
            return new GameGrid(Cells);
        }
    }
}
