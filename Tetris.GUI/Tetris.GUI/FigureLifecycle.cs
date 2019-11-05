namespace Tetris.GUI
{
    public class FigureLifecycle
    {
        public Figure Figure { get; set; }
        public FigureLifecycleTypes FigureLifecycleTypes { get; set; }
    }

    public enum FigureLifecycleTypes
    {
        Init = 0,
        Moved = 1,
        Dead = 2
    }
}