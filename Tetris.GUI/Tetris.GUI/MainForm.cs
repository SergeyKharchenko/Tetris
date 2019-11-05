using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris.GUI
{
    public partial class MainForm : Form
    {
        private readonly Subject<int> Subject;
        private readonly Subject<IObservable<long>> Timer;
        private TimeSpan Interval = TimeSpan.FromSeconds(1);
        private readonly GameEngine GameEngine = new GameEngine();

        public MainForm()
        {
            InitializeComponent();


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
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GameEngine.Dispose();
        }
    }
}
