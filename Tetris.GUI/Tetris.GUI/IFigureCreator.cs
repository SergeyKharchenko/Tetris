using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public interface IFigureCreator
    {
        Task<Figure> CreateFigureAsync(int width);
    }

    public class FigureCreator : IFigureCreator
    {
        private readonly TetrominoGenerator Generator = new TetrominoGenerator();
        private readonly Random Random;

        public FigureCreator()
        {
            Random = new Random((int) DateTime.Now.Ticks);
        }

        public Task<Figure> CreateFigureAsync(int width)
        {
            int figureSize = Random.Next(1, Constants.FigureMaxWidth);
            bool[,] shape = Generator.CreateTetromino(figureSize,
                                                      Random.Next(
                                                          1,
                                                          Math.Min(figureSize * 2,
                                                                   figureSize * figureSize)));

            return Task.FromResult(new Figure(new Point(width / 2 - figureSize / 2, 0), new FigurePosition(shape)));
        }
    }
}