using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConsoleApp1 {
    class Program {
        public static void Main(string[] args) {
            Task.Run(async () => {

                IPropagatorBlock<int, int> block = CreateMyCustomBlock();

                block.LinkTo(new ActionBlock<int>(_ => { }));

                block.Post(10);
            });


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
}
