using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.GUI
{
    public class Figure
    {
        private readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

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
            await Mutex.WaitAsync();
            try
            {
                Position = await Position.RotateAsync();
            }
            finally
            {
                Mutex.Release();
            }
        }

        public async Task MoveAsync(Point offset)
        {
            await Mutex.WaitAsync();
            try
            {
                Point location = Location;
                location.Offset(offset);
                Location = location;
            }
            finally
            {
                Mutex.Release();
            }
        }

        public override string ToString()
        {
            return $"{nameof(Location)}: {Location}{Environment.NewLine}{nameof(Position)}:{Environment.NewLine}{Position}";
        }
    }
}