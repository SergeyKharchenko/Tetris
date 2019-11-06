using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public interface IMovingValidator
    {
        Task<MovingValidationResult> ValidateAsync(GameGrid area, Figure figure, Point offset);
    }
}