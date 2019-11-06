using System.Drawing;

namespace Tetris.GUI {
    public class GameState
    {
        public GameCell[,] Cells;
    }

    public class GameCell
    {
        public Point Location { get; set; }

        public Color Color { get; set; }
    }
}