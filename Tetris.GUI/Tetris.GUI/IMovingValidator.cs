using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public interface IMovingValidator
    {
        Task<bool> ValidateAsync(bool[,] area, Figure figure, Point delta);
    }
}