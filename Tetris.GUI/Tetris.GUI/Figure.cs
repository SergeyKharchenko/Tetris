using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class Figure
    {
        public Point Location { get; private set; }

        public FigurePosition Position { get; private set; }

        public Color Color { get; private set; }

        public Figure(Point location, FigurePosition position, Color color)
        {
            Location = location;
            Position = position;
            Color = color;
        }

        public Point[] GetPositionWithOffset()
        {
            return Position.GetOffset(Location);
        }

        public async Task RotateAsync()
        {
            Position = await Position.RotateAsync();
        }

        public Task MoveAsync(Point offset)
        {
            Point location = Location;
            location.Offset(offset);
            Location = location;
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return $"{nameof(Location)}: {Location}{Environment.NewLine}{nameof(Position)}:{Environment.NewLine}{Position}";
        }
    }
}