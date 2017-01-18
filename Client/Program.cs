using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Please enter a argument.");
                Console.WriteLine("Usage: Client [ip] [port]");
                return;
            }

            int port = 0;
            try
            {
                port = Int32.Parse(args[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Please enter a argument.");
                Console.WriteLine("Usage: Client [ip] [port]");
                return;
            }

            SynchronousSockerClient client = new SynchronousSockerClient();
            client.StartConnect(args[0], port, args[2]);
        }
    }
}
