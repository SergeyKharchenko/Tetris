using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Tetris.GUI {
    public class Figure : ICloneable {
        public Point Location { get; private set; }

        public FigurePosition Position { get; private set; }

        public Color Color { get; private set; }

        public Figure(Point location, FigurePosition position, Color color) {
            Location = location;
            Position = (FigurePosition)position.Clone();
            Color = color;
        }

        public async Task<Point[]> GetPositionWithOffsetAsync() {
            await Task.Yield();
            return Position.GetOffset(Location);
        }

        public async Task RotateAsync() {
            Position = await Position.RotateAsync();
        }

        public Task MoveAsync(Point offset) {
            Point location = Location;
            location.Offset(offset);
            Location = location;
            return Task.CompletedTask;
        }

        public object Clone() {
            return new Figure(Location, Position, Color);
        }

        public override string ToString() {
            return $"{nameof(Location)}: {Location}{Environment.NewLine}{nameof(Position)}:{Environment.NewLine}{Position}";
        }
    }
}