using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConsoleApp1 {
    class Program {
        public static void Main(string[] args) {

            Task.Run(async () => {
                //await new[] { 1, 2, 3, 4, 5, 6, 7 }.AsParallel().AnyAsync(async i => {
                //    Console.WriteLine($"START:   i {i} thread {Environment.CurrentManagedThreadId}");
                //    await Task.Delay(i * 1000);
                //    Console.WriteLine($"i {i} thread {Environment.CurrentManagedThreadId}");
                //    return i == 6;
                //});
            });

            Enumerable.Range(1, 100).Select(async i => {
                await Task.Yield();
                return i;
            }).AsParallel().Any(i => {
                Console.WriteLine($"i {i.Result} thread {Environment.CurrentManagedThreadId}");
                return i.Result == 3;
            });

            //Parallel.ForEach(Enumerable.Range(1, 100), (i, state) =>
            //{
            //    //Console.WriteLine($"START:   i {i} thread {Environment.CurrentManagedThreadId}");
            //    //await Task.Delay(i * 1000);
            //    Console.WriteLine($"i {i} thread {Environment.CurrentManagedThreadId}");
            //    if (i == 3) {
            //        state.Stop();
            //    }
            //});

            Console.ReadKey();
        }

        static IPropagatorBlock<int, int> CreateMyCustomBlock() {
            var multiplyBlock = new TransformBlock<int, int>(item => {
                Console.WriteLine(Environment.CurrentManagedThreadId);
                return item * 2;
            });
            var addBlock = new TransformBlock<int, int>(item => {
                Console.WriteLine(Environment.CurrentManagedThreadId);
                return item + 2;
            });
            var divideBlock = new TransformBlock<int, int>(item => {
                Console.WriteLine(Environment.CurrentManagedThreadId);
                return item / 2;
            });

            var flowCompletion = new DataflowLinkOptions { PropagateCompletion = true };
            multiplyBlock.LinkTo(addBlock, flowCompletion);
            addBlock.LinkTo(divideBlock, flowCompletion);

            return DataflowBlock.Encapsulate(multiplyBlock, divideBlock);
        }
    }

    public static class AsyncExtensions {
        public static async Task<bool> AnyAsync<T>(
            this IEnumerable<T> source, Func<T, Task<bool>> func) {
            foreach (var element in source) {
                if (await func(element))
                    return true;
            }
            return false;
        }
    }
}
