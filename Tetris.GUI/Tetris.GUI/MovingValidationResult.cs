namespace Tetris.GUI
{
    public class MovingValidationResult
    {
        public bool Allow
        {
            get => !Dead && AllowBacking;
            set => AllowBacking = value;
        }
        private bool AllowBacking;

        public bool Dead { get; set; }
    }
}