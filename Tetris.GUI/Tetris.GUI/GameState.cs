using System;
using System.Drawing;

namespace Tetris.GUI {
    public class GameState {
        public GameGrid Area;
    }

    public class GameCell : ICloneable {
        public Point Location { get; set; }

        public Color Color { get; set; } = Constants.BackgroundColor;

        public bool IsFigure {
            get => IsFigureBacking;
            set {
                IsFigureBacking = value;
                if (!IsOccupied) {
                    Color = Constants.BackgroundColor;
                }
            }
        }
        private bool IsFigureBacking;

        public bool IsOccupied {
            get => IsOccupiedBacking || IsFigure;
            set {
                IsOccupiedBacking = value;
                if (!IsOccupied) {
                    Color = Constants.BackgroundColor;
                }
            }
        }
        private bool IsOccupiedBacking;

        public bool IsOccupiedWithNotFigure {
            get => IsOccupiedBacking && !IsFigure;
        }

        public void Assign(GameCell cell) {
            Color = cell.Color;
            IsFigureBacking = cell.IsFigureBacking;
            IsOccupiedBacking = cell.IsOccupiedBacking;
        }

        public void Clear() {
            Color = Constants.BackgroundColor;
            IsFigureBacking = false;
            IsOccupiedBacking = false;
        }

        public object Clone() {
            return new GameCell {
                Location = Location,
                Color = Color,
                IsFigureBacking = IsFigureBacking,
                IsOccupiedBacking = IsOccupiedBacking
            };
        }
    }
}