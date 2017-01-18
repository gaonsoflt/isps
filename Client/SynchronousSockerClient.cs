using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class SynchronousSockerClient
    {
        // Data buffer from incomming data
        byte[] bytes = new byte[1024];
        static readonly string ImagePath = Path.Combine("images", "");

        public SynchronousSockerClient()
        {
        }

        public void StartConnect(string ipAddress, int port, string action)
        {
            NetworkStream NS = null;
            StreamReader SR = null;
            StreamWriter SW = null;
            TcpClient client = null;

            try
            {
                client = new TcpClient(ipAddress, port); //client 연결
                Console.WriteLine("{0}:{1}에 접속하였습니다.", ipAddress, port);
                NS = client.GetStream(); // 소켓에서 메시지를 가져오는 스트림
                SR = new StreamReader(NS, Encoding.UTF8); // Get message
                SW = new StreamWriter(NS, Encoding.UTF8); // Send message

                string SendMessage = string.Empty;
                string GetMessage = string.Empty;
                
                if (action == "fp")
                {
                    SendMessage = "1,user1,test";
                    /*
                    Stream fileStream = File.OpenRead(tbFilename.Text);
                    // Alocate memory space for the file
                    byte[] fileBuffer = new byte[fileStream.Length];
                    fileStream.Read(fileBuffer, 0, (int)fileStream.Length);
                    // Open a TCP/IP Connection and send the data
                    TcpClient clientSocket = new TcpClient(tbServer.Text, 8080);
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Write(fileBuffer, 0, fileBuffer.GetLength(0));
                    networkStream.Close();
                    */
                }
                else if(action == "msg")
                {
                    SendMessage = "2,dksfjskdjfksjdfklsdjf,3333";
                    SW.WriteLine(SendMessage); // 메시지 보내기
                    SW.Flush();
                }
                GetMessage = SR.ReadLine();
                Console.WriteLine(GetMessage);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (SW != null) SW.Close();
                if (SR != null) SR.Close();
                if (client != null) client.Close();
            }
        }
    }
}
