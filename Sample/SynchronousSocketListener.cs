using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Sample
{
    public class SynchronousSocketListener
    {
        Logger logger = Logger.createLogger("SynchronousSocketListener");


        NetworkStream NS = null;
        StreamReader SR = null;
        StreamWriter SW = null;
        TcpClient client;
        public void startClient(TcpClient clientSocket)
        {
            client = clientSocket;
            Thread job_thread = new Thread(run);
            job_thread.Start();
        }

        public void run()
        {
            NS = client.GetStream(); // 소켓에서 메시지를 가져오는 스트림
            SR = new StreamReader(NS, Encoding.UTF8); // Get message
            SW = new StreamWriter(NS, Encoding.UTF8); // Send message

            string message = string.Empty;
            try
            {
                // 클라이언트 접속 시
                while (client.Connected == true) //클라이언트 메시지받기
                {
                    // 클라이언트에서 메시지가 있을 경우
                    if((message = SR.ReadLine()) != null)
                    {
                        action(message);
                    }
                    //SW.WriteLine("Server: {0} [{1}]", message, DateTime.Now); // 메시지 보내기
                    //SW.Flush();
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message.ToString());
            }
            finally
            {
                logger.log("disconnect client" + client);
                SW.Close();
                SR.Close();
                client.Close();
                NS.Close();
            }
        }

        private string[] parser(string msg)
        {
            //char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            char[] delimiterChars = { ',' };
            string[] arrMsg = msg.Split(delimiterChars);

            return arrMsg;
        }

        private void action(string msg)
        {
            logger.log(msg);
            if(msg != null && msg != "")
            {
                string[] arr = parser(msg);
                if(arr.Length < 1)
                {
                    return;
                }

                int DATA_TYPE = Int32.Parse(arr[0]);
                switch (DATA_TYPE)
                {
                    case 1:
                        string USERNAME = arr[1];
                        logger.log(USERNAME);
                        SW.WriteLine("FINGERPRINT OK"); // 메시지 보내기
                        SW.Flush();
                        break;
                    case 2:
                        SW.WriteLine("MSG OK"); // 메시지 보내기
                        SW.Flush();
                        break;
                }
            }
        }
    }

    class MessageManager
    {

    }

    class FileManager
    {

    }
}