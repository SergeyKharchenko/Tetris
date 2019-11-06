using System.Drawing;

namespace Tetris.GUI {
    public class GameState
    {
        public GameCell[,] Cells;
    }

    public class GameCell
    {
        public Point Location { get; set; }

        public Color Color { get; set; } = Constants.BackgroundColor;

        public bool IsFigure { get; set; }

        public bool IsOccupied
        {
            get => IsOccupiedBacking || IsFigure;
            set => IsOccupiedBacking = value;
        }
        private bool IsOccupiedBacking;

    }
}