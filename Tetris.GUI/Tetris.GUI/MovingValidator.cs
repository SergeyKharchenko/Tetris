using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class MovingValidator : IMovingValidator
    {
        public Task<bool> ValidateAsync(GameCell[,] area, Figure figure, Point delta)
        {
            return Task.FromResult(true);
        }
    }
}