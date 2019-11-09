using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public interface IMovingValidator
    {
        Task<MovingValidationResult> ValidateMoveAsync(GameGrid area, Figure figure, Point offset);

        Task<bool> ValidateRotationAsync(GameGrid area, Figure figure);
    }
}