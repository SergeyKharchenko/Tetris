using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public class FigureCreator : IFigureCreator
    {
        private readonly TetrominoGenerator Generator = new TetrominoGenerator();
        private readonly Random Random;

        private static Color[] Colors = {
            Color.Yellow
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

            return Task.FromResult(new Figure(new Point(width / 2 - figureSize / 2, 0), new FigurePosition(shape)));
        }
    }
}