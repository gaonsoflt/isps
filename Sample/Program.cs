using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SourceAFIS.Simple; // import namespace SourceAFIS.Simple
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sample
{
    public class Program
    {

        // Initialize path to images
        //static readonly string ImagePath = Path.Combine(Path.Combine("..", ".."), "images");

        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerForm());

            /*
            TcpListener Listener = null;
            TcpClient client = null;

            int port = 5555;

            Console.WriteLine("Start socket...");
            try
            {
                //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                //Listener = new TcpListener(ipAddress, port);
                Listener = new TcpListener(port);
                Listener.Start(); // Listener 동작 시작

                while (true)
                {
                    client = Listener.AcceptTcpClient();
                    SynchronousSocketListener server = new SynchronousSocketListener();
                    server.startClient(client);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            finally
            {
            }       
            */
            /*
            // Test if input arguments were supplied:
            if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter a argument.");
                System.Console.WriteLine("Usage:");
                System.Console.WriteLine("      Sample [save] filename username");
                System.Console.WriteLine("      Sample [recog] filename username");
                System.Console.WriteLine("      Sample [show]");
                return 1;
            }

            if (args[0].ToUpper().Equals("SAVE"))
            {
                //new FingerPrintManager().savePerson(Path.Combine(ImagePath, args[1]), args[2]);
                new FingerPrintManager().savePerson(Path.Combine("images", args[1]), args[2]);
            }
            else if (args[0].ToUpper().Equals("RECOG"))
            {
                //new FingerPrintManager().recognition(Path.Combine(ImagePath, args[1]), args[2]);
                new FingerPrintManager().recognition(Path.Combine("images", args[1]), args[2]);
            }
            else if (args[0].ToUpper().Equals("SHOW"))
            {
                new FingerPrintManager().showDatabase();
            }

            FingerPrintManager.MyPerson person = new FingerPrintManager.MyPerson(); 
            */
            return 0;
        }
    }
}
