using System;
using System.Drawing;

namespace Tetris.GUI
{
    public static class Constants
    {
        public static readonly Color BackgroundColor = Color.Black;

        public static readonly TimeSpan Speed = TimeSpan.FromSeconds(1);

        public static readonly int GameAreaWidth = 20;
        public static readonly int GameAreaHeight = 50;

        public static readonly int FigureMaxWidth = 5;
    }
}