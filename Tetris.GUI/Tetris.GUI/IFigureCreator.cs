using System.Threading.Tasks;

namespace Tetris.GUI
{
    public interface IFigureCreator
    {
        Task<Figure> CreateFigureAsync(int width);
    }
}