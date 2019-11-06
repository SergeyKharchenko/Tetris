using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public interface IMovingValidator
    {
        Task<MovingValidationResult> ValidateAsync(GameCell[,] area, Figure figure, Point offset);
    }
}