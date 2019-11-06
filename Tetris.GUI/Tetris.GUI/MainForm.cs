using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Tetris.GUI
{
    public partial class MainForm : Form
    {
        private readonly GameEngine GameEngine;
        private IDisposable GameStateSubscription;
        private GameState CurrentGameState;

        private float CellWidth = -1;
        private float CellHeight = -1;

        public MainForm()
        {
            InitializeComponent();

            GameEngine = new GameEngine(CreateNavigationObservable(), CreateRotationObservable());
            //Subject = new Subject<int>();

            //Console.WriteLine($"UI: {Thread.CurrentThread.ManagedThreadId}");

            //Timer = new Subject<IObservable<long>>();
            //Timer.Switch().Subscribe(Console.WriteLine);
            //Timer.OnNext(Observable.Interval(TimeSpan.FromSeconds(0.5)));

            //Subject.AsObservable()
            //       .ObserveOn(Scheduler.Default)
            //       .Do(_ => Console.WriteLine(Thread.CurrentThread.ManagedThreadId))
            //       .Select(_ => 67)
            //       .ObserveOn(SynchronizationContext.Current)
            //       .Subscribe(_ =>
            //       {
            //           Console.WriteLine($"Subscribe: {Thread.CurrentThread.ManagedThreadId}");

            //       });

            //IDisposable subscription = null;
            //subscription = Observable.Interval(TimeSpan.FromSeconds(3))
            //                         .Take(5)
            //                         .Subscribe(_ =>
            //                                    {
            //                                        Interval = new TimeSpan(Interval.Ticks / 2);
            //                                        Timer.OnNext(Observable.Interval(Interval));
            //                                    },
            //                                    () => subscription?.Dispose());

        }

        private async void OnFormShown(object sender, EventArgs e)
        {
            await GameEngine.StartAsync();
            GameStateSubscription = GameEngine.GameState.ObserveOn(SynchronizationContext.Current).Subscribe(
                gameState =>
                {
                    CurrentGameState = gameState;
                    Invalidate();
                });
        }

        private void OnFormPaint(object sender, PaintEventArgs e)
        {
            if (CurrentGameState == null)
            {
                return;
            }

            if (Math.Abs(CellWidth - -1) < float.Epsilon || Math.Abs(CellHeight - -1) < float.Epsilon)
            {
                CalculateCellSettings();
            }

            foreach (GameCell cell in CurrentGameState.Cells)
            {
                using (var brush = new SolidBrush(cell.Color))
                {
                    e.Graphics.FillRectangle(brush,
                        cell.Location.X * CellWidth,
                        cell.Location.Y * CellHeight,
                        CellWidth,
                        CellHeight);
                }

                using (var pen = new Pen(Color.Black))
                {
                    e.Graphics.DrawRectangle(pen,
                        cell.Location.X * CellWidth,
                        cell.Location.Y * CellHeight,
                        CellWidth,
                        CellHeight);
                }
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GameStateSubscription.Dispose();
            GameEngine.Dispose();
        }

        private void OnFormResize(object sender, EventArgs e)
        {
            CalculateCellSettings();
            Invalidate();
        }

        private void CalculateCellSettings()
        {
            if (CurrentGameState == null)
            {
                return;
            }

            int cellWidth = CurrentGameState.Cells.GetUpperBound(0) + 1;
            int cellHeight = CurrentGameState.Cells.GetUpperBound(1) + 1;

            CellWidth = ClientSize.Width / (float)cellWidth;
            CellHeight = ClientSize.Height / (float)cellHeight;
        }

        private IObservable<Point> CreateNavigationObservable()
        {
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(
                    handler => (s, args) => handler(s, args),
                    handler => KeyDown += handler,
                    handler => KeyDown -= handler)
                .Where(pattern => pattern.EventArgs.KeyCode == Keys.Left ||
                                  pattern.EventArgs.KeyCode == Keys.Right)
                .Select(pattern => new Point(pattern.EventArgs.KeyCode == Keys.Left ? -1 : 1, 0));
        }

        private IObservable<bool> CreateRotationObservable()
        {
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(
                    handler => (s, args) => handler(s, args),
                    handler => KeyDown += handler,
                    handler => KeyDown -= handler)
                .Where(pattern => pattern.EventArgs.KeyCode == Keys.Space)
                .Select(_ => true);
        }
    }
}
