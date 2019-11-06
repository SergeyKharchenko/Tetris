namespace Tetris.GUI
{
    public class MovingFigureResult
    {

        public bool Changed
        {
            get => FigureIsDead || Backing;
            set => Backing = value;
        }
        private bool Backing;

        public bool FigureIsDead { get; set; }
    }
}