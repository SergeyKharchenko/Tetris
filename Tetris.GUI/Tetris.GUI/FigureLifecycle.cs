namespace Tetris.GUI
{
    public class FigureLifecycle
    {
        public Figure Figure { get; set; }
        public FigureLifecycleTypes FigureLifecycleTypes { get; set; }
    }

    public enum FigureLifecycleTypes
    {
        None = 0,
        Init = 1,
        Moved = 2,
        Dead = 3
    }
}