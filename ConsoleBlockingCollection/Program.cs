using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleBlockingCollection
{
    internal class Program
    {
        #region Fields

        private static BlockingCollection<int> data = new BlockingCollection<int>(20);

        #endregion

        #region Methods

        internal static void Main(string[] args)
        {

            var producer = Task.Factory.StartNew(() => Producer());

            var consumer = Task.Factory.StartNew(() => Consumer());

            Console.Read();
        }

        private static void Consumer()
        {

            foreach (var item in data.GetConsumingEnumerable())

            {
                Thread.Sleep(1200);
                Console.WriteLine(item + "============================Count:" + data.Count);

            }
        }

        private static void Producer()
        {

            for (int ctr = 0; ctr < 10000; ctr++)

            {
                Thread.Sleep(1000);
                //Console.WriteLine("============================ctr:" + ctr);
                if (data.Count == data.BoundedCapacity)
                    continue;
                data.Add(ctr);

                

            }
        }

        #endregion
    }
}
