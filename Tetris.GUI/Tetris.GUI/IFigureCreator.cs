using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public interface IFigureCreator
    {
        Task<Figure> CreateFigureAsync(Point initialLocation);
    }

    public class RegistryFigureCreator : IFigureCreator
    {
        private readonly Random Random;

        public RegistryFigureCreator()
        {
            Random = new Random((int) DateTime.Now.Ticks);
        }

        public Task<Figure> CreateFigureAsync(Point initialLocation)
        {
            int index = Random.Next(0, FigureRegistry.Registry.Length - 1);
            int[,] shape = FigureRegistry.Registry[index];
            int size = shape.GetUpperBound(0) + 1;
            bool[,] shape2 = new bool[size, size];

            for (var i = 0; i < size; ++i) {
                for (var j = 0; j < size; ++j) {
                    shape2[i, j] = Convert.ToBoolean(shape[i, j]);
                }
            }

            return Task.FromResult(new Figure(initialLocation, new FigurePosition(shape2)));
        }
    }
}