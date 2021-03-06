﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static AsyncSocketServer.DataPacket;
using AsyncSocketServer;
using System.Drawing;

namespace AsyncSocketClient
{
    #region ReceiveBuffer define
    struct ReceiveBuffer
    {
        public const int BUFFER_SIZE = 1024;
        public byte[] Buffer;
        public int ToReceive;
        public MemoryStream BufStream;
        public Packet pkt;

        public ReceiveBuffer(byte[] bytes)
        {
            Buffer = new byte[BUFFER_SIZE];
            pkt = DataPacket.ByteToStructHeader(bytes);
            ToReceive = pkt.dataLen;
            BufStream = new MemoryStream(pkt.dataLen);
        }

        //public const int BUFFER_SIZE = 1024;
        //public byte[] Buffer;
        //public int ToReceive;
        //public MemoryStream BufStream;

        //public ReceiveBuffer(int toRec)
        //{
        //    Buffer = new byte[BUFFER_SIZE];
        //    ToReceive = toRec;
        //    BufStream = new MemoryStream(toRec);
        //}

        public void Dispose()
        {
            pkt = new Packet();
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
        public delegate void OnConnectEventHandler(Client sender, bool connected);
        public event OnConnectEventHandler OnConnect;

        public delegate void OnSendEventHandler(Client sender, int sent);
        public event OnSendEventHandler OnSend;

        public delegate void OnDisconnectEventHandler(Client sender);
        public event OnDisconnectEventHandler OnDisconnect;

        public delegate void DataReceivedEventHandler(Client sender, ReceiveBuffer e);
        public event DataReceivedEventHandler DataReceived;

        public delegate void OnReceiveEventHandler(Client client, string text);
        public event OnReceiveEventHandler OnReceive;

        public delegate void OnDisconnectByServerEventHandler(Client sender);
        public event OnDisconnectByServerEventHandler OnDisconnectByServer;

        Socket socket;
        string responseText = string.Empty;
        byte[] headBuf;
        ReceiveBuffer buffer;

        public bool Connected
        {
            get
            {
                if (socket != null)
                {
                    return socket.Connected;
                }

                return false;
            }
        }

        public Client()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            headBuf = new byte[32];
        }

        public void Connect(string ipAddress, int port)
        {
            try
            {
                if (socket == null)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    headBuf = new byte[4];
                }

                socket.Connect(IPAddress.Parse(ipAddress), port);

                if (OnConnect != null)
                {
                    OnConnect(this, Connected);
                }

                ReceiveAsync();
                //Receive();
                // callback 함수를 이용한 비동기 호출로 connect 연결함
                //socket.BeginConnect(ipAddress, port, connectCallBack, null);
            }
            catch (SocketException se)
            {
                throw se;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void connectCallBack(IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);
        
                if(OnConnect != null)
                {
                    OnConnect(this, Connected);
                }
            }
            catch (SocketException se) 
            {
                throw se;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DataParser(Packet pkt)
        {
            switch(pkt.type)
            {
                case PktType.AUTH:
                    break;
                case PktType.PASSENGER:
                    break;
                case PktType.ORDER:
                    break;
            }
        }

        void Send(byte[] data, int index, int length)
        {
            //socket.BeginSend(BitConverter.GetBytes(length), 0, 4, SocketFlags.None, sendCallBack, null);
            //System.Threading.Thread.Sleep(500);
            socket.BeginSend(data, index, length, SocketFlags.None, sendCallBack, null);
        }

        private void SendResponse(Packet pkt)
        {
            byte[] buffer = DataPacket.StructToByte(pkt);
            Send(buffer, 0, buffer.Length);
        }

        //public void SendAuthUserByFingerPrint(int userId, string carId, String path)
        //{
        //    SendAuthUserByFingerPrint(userId, carId, File.ReadAllBytes(path));
        //}

        //public void SendAuthUserByFingerPrint(int userId, string carId, Image image)
        public void SendAuthUserByFingerPrint(int userId, string carId, Bitmap image)
        {
            Packet pkt = new Packet();
            pkt.type = PktType.AUTH;
            pkt.userId = userId;
            pkt.carId = carId;
            pkt.response = 0;
            if(image != null) pkt.fingerPrint = image;
            SendResponse(pkt);
        }

        public void SendPassengerCount(int userId, string carId, string guid, int psgCnt)
        {
            Packet pkt = new Packet();
            pkt.type = PktType.PASSENGER;
            pkt.userId = userId;
            pkt.carId = carId;
            pkt.response = 0;
            pkt.guid = guid;
            pkt.psgCnt = psgCnt;
            SendResponse(pkt);
        }

        public void SendRequestOrder(int userId, string carId, string guid, int accessId)
        {
            Packet pkt = new Packet();
            pkt.type = PktType.ORDER;
            pkt.userId = userId;
            pkt.carId = carId;
            pkt.response = 0;
            pkt.guid = guid;
            pkt.accessId = accessId;
            SendResponse(pkt);
        }

        public void SendOnce(int userId, string carId, byte[] image, int psgCnt)
        {
            Packet pkt = new Packet();
            pkt.type = PktType.ONCE;
            pkt.userId = userId;
            pkt.carId = carId;
            pkt.response = 0;
            pkt.psgCnt = psgCnt;
            //if (image != null) pkt.fingerPrint = image;
            pkt.data = image;
            SendResponse(pkt);
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
            catch(Exception ex) 
            {
                Trace.WriteLine(string.Format("SEND ERROR\n{0}", ex.Message));
            }
        }

        public void Disconnect()
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Close();
                    socket = null;
                    if (OnDisconnect != null)
                    {
                        OnDisconnect(this);
                    }
                    buffer.Dispose();
                    socket = null;
                    headBuf = null;
                    //OnDisconnect = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void ReceiveAsync()
        {
            socket.BeginReceive(headBuf, 0, headBuf.Length, SocketFlags.None, receiveCallBack, null);
        }

        public void receiveCallBack(IAsyncResult ar)
        {
            try
            {
                int rec = socket.EndReceive(ar);

                if (rec == 0)
                {
                    if (OnDisconnect != null)
                    {
                        OnDisconnect(this);
                        return;
                    }
                }

                if (rec != headBuf.Length)
                {
                    throw new Exception();
                }
            }
            catch (SocketException se)
            {
                switch (se.SocketErrorCode)
                {
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionReset:
                        if (OnDisconnect != null)
                        {
                            OnDisconnect(this);
                            return;
                        }
                        break;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (NullReferenceException)
            {
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            buffer = new ReceiveBuffer(headBuf);
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
    }
    #endregion

}
