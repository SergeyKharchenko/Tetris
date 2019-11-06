using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public interface IMovingValidator
    {
        Task<bool> ValidateAsync(GameCell[,] area, Figure figure, Point delta);
    }
}