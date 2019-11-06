using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class Figure
    {
        public Point Location { get; set; }

        public FigurePosition Position { get; set; }

        public Figure(Point location, FigurePosition position)
        {
            Location = location;
            Position = position;
        }

        public Point[] GetLocationWithOffset()
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