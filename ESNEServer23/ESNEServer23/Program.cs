using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESNEServer23
{
    internal class Program
    {
        static Pool connectionsPool = new Pool(5);

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to ESNE Server 2023");
            Console.WriteLine("Listening for new conections...");

            Task task = new Task(() => { new Listener(connectionsPool, 666); });
            task.Start();

            while (true)
            {
                connectionsPool.Process();
            }

            Console.WriteLine("Exit!");
            Console.ReadLine();
        }
    }
}
