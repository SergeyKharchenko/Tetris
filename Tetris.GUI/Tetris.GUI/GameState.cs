using System;
using System.Drawing;

namespace Tetris.GUI {
    public class GameState
    {
        public GameCell[,] Cells;
    }

    public class GameCell : ICloneable
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

        public object Clone()
        {
            return new GameCell
            {
                Location = Location, Color = Color, IsFigure = IsFigure, IsOccupied = IsOccupiedBacking
            };
        }
    }
}