using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public class FigureCreator : IFigureCreator
    {
        private readonly TetrominoGenerator Generator = new TetrominoGenerator();
        private readonly Random Random;

        private static readonly Color[] Colors = {
            Color.Yellow,
            Color.Aqua,
            Color.Coral,
            Color.Aquamarine,
            Color.LawnGreen,
            Color.DarkOrchid,
            Color.LightPink,
            Color.RoyalBlue,
            Color.Magenta,
            Color.Wheat
        };

        public FigureCreator()
        {
            Random = new Random((int) DateTime.Now.Ticks);
        }

        public Task<Figure> CreateFigureAsync(int width)
        {
            int figureSize = Random.Next(1, Constants.FigureMaxWidth + 1);
            bool[,] shape = Generator.CreateTetromino(figureSize,
                                                      Random.Next(
                                                          1,
                                                          Math.Min(figureSize * 2,
                                                                   figureSize * figureSize)));

            var location = new Point(width / 2 - figureSize / 2, 0);
            Color color = Colors[Random.Next(0, Colors.Length)];
            return Task.FromResult(new Figure(location, new FigurePosition(shape), color));
        }
    }
}