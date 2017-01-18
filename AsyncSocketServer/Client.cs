using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using static AsyncSocketServer.UserManager;

namespace AsyncSocketServer
{
    enum Commands : int
    {
        AUTH = 0,
        PASSENGER,
        ORDER,
        ERROR,
        STRING,
        IMAGE,
        ENROLL,
        RECOG
    }

    #region ReceiveBuffer define
    struct ReceiveBuffer
    {
        public const int BUFFER_SIZE = 1024;
        public byte[] Buffer;
        public int ToReceive;
        public MemoryStream BufStream;

        public ReceiveBuffer(int toRec)
        {
            Buffer = new byte[BUFFER_SIZE];
            ToReceive = toRec;
            BufStream = new MemoryStream(toRec);
        }

        public void Dispose()
        {
            Buffer = null;
            ToReceive = 0;
            Close();
            if (BufStream != null)
                BufStream.Dispose();
        }

        public void Close()
        {
            if (BufStream != null && BufStream.CanWrite)
                BufStream.Close();
        }
    }
    #endregion

    #region Client define
    class Client
    {
        static public int PKT_ACK = 48;
        static public int PKT_NACK = 49;

        byte[] lenBuffer;
        ReceiveBuffer buffer;
        Socket socket;
        MyPerson loginUser;

        public IPEndPoint EndPoint
        {
            get
            {
                if (socket != null && socket.Connected)
                    return (IPEndPoint)socket.RemoteEndPoint;

                return new IPEndPoint(IPAddress.None, 0);
            }
        }

        public delegate void DisconnectedEventHandler(Client sender);
        public event DisconnectedEventHandler Disconnected;
        public delegate void DataReceivedEventHandler(Client sender, ReceiveBuffer e);
        public event DataReceivedEventHandler DataReceived;
        public delegate void OnSendEventHandler(Client sender, int sent);
        public event OnSendEventHandler OnSend;

        public Client(Socket s)
        {
            socket = s;
            lenBuffer = new byte[4];
        }

        public void setLoginUser(MyPerson user)
        {
            this.loginUser = user;
        }

        public MyPerson getLoginUser()
        {
            return this.loginUser;
        }
        
        public void Close()
        {
            if (socket != null)
            {
                socket.Disconnect(false);
                socket.Close();
            }

            buffer.Dispose();
            socket = null;
            lenBuffer = null;
            Disconnected = null;
            DataReceived = null;
        }

        public void ReceiveAsync()
        {
            socket.BeginReceive(lenBuffer, 0, lenBuffer.Length, SocketFlags.None, receiveCallBack, null);
        }

        public void receiveCallBack(IAsyncResult ar)
        {
            try
            {
                int rec = socket.EndReceive(ar);

                if (rec == 0)
                {
                    if (Disconnected != null)
                    {
                        Disconnected(this);
                        return;
                    }
                }

                if (rec != 4)
                {
                    throw new Exception();
                }
            }
            catch(SocketException se)
            {
                switch(se.SocketErrorCode)
                {
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionReset:
                        if (Disconnected != null)
                        {
                            Disconnected(this);
                            return;
                        }
                        break;
                }
            }
            catch(ObjectDisposedException)
            {
                return;
            }
            catch(NullReferenceException)
            {
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            buffer = new ReceiveBuffer(BitConverter.ToInt32(lenBuffer, 0));

            socket.BeginReceive(buffer.Buffer, 0, buffer.Buffer.Length, SocketFlags.None, receivePacketCallBack, null);
        }

        public void receivePacketCallBack(IAsyncResult ar)
        {
            int rec = socket.EndReceive(ar);

            if (rec <= 0)
            {
                return;
            }

            buffer.BufStream.Write(buffer.Buffer, 0, rec);

            buffer.ToReceive -= rec;

            if (buffer.ToReceive > 0)
            {
                Array.Clear(buffer.Buffer, 0, buffer.Buffer.Length);

                socket.BeginReceive(buffer.Buffer, 0, buffer.Buffer.Length, SocketFlags.None, receivePacketCallBack, null);
                return;
            }

            if (DataReceived != null)
            {
                buffer.BufStream.Position = 0;
                DataReceived(this, buffer);
            }

            buffer.Dispose();

            ReceiveAsync();
        }

        void Send(byte[] data, int index, int length)
        {
            Console.WriteLine("send byte: " + FingerSensorPacket.ByteToHexString(data));
            socket.BeginSend(BitConverter.GetBytes(length), 0, 4, SocketFlags.None, sendCallBack, null);
            System.Threading.Thread.Sleep(500);
            socket.BeginSend(data, index, length, SocketFlags.None, sendCallBack, null);
        }

        public void sendCallBack(IAsyncResult ar)
        {
            try
            {
                int sent = socket.EndSend(ar);

                if (OnSend != null)
                {
                    OnSend(this, sent);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("SEND ERROR\n{0}", ex.Message));
            }
        }

        public void SendText(string text)
        {
            BinaryWriter bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)Commands.STRING);
            bw.Write(text);
            byte[] data = ((MemoryStream)bw.BaseStream).ToArray();
            bw.BaseStream.Dispose();
            Send(data, 0, data.Length);
        }

        public void SendResponseAuthUserByFingerPrint(int userId, int ack, String guid)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            byte[] b = StringUtil.StringToByte(guid);
            bw.Write((int)Commands.AUTH);
            bw.Write((int)userId);
            bw.Write((int)ack);
            bw.Write((int)b.Length);
            bw.Write(b);
            bw.Close();
            b = ms.ToArray();
            ms.Dispose();
            Send(b, 0, b.Length);
        }

        public void SendResponsePassengerCount(int userId, int ack)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            byte[] b = new byte[0];
            bw.Write((int)Commands.PASSENGER);
            bw.Write((int)userId);
            bw.Write((int)ack);
            bw.Write((int)b.Length);
            bw.Write(b);
            bw.Close();
            b = ms.ToArray();
            ms.Dispose();
            Send(b, 0, b.Length);
        }

        public void SendResponseOrder(int userId, int ack, byte[] order)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((int)Commands.ORDER);
            bw.Write((int)userId);
            bw.Write((int)ack);
            bw.Write((int)order.Length);
            bw.Write(order);
            bw.Close();
            byte[] b = ms.ToArray();
            ms.Dispose();
            Send(b, 0, b.Length);
        }

        public void SendError(int userId, byte[] error)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((int)Commands.ERROR);
            bw.Write((int)0);
            bw.Write((int)PKT_NACK);
            bw.Write((int)error.Length);
            bw.Write(error);
            bw.Close();
            byte[] b = ms.ToArray();
            ms.Dispose();
            Send(b, 0, b.Length);
        }
    }
    #endregion

}
