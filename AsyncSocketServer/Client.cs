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
using static AsyncSocketServer.DataPacket;
using static AsyncSocketServer.CommonConfig.Message;
using static AsyncSocketServer.AccessInfoManager;
using static AsyncSocketServer.OrderInfoManager;

namespace AsyncSocketServer
{
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
        public delegate void SetLogHandler(string msg);
        public static event SetLogHandler UpdateLogMsg;

        public delegate void SetMatchedUserHandler(MyPerson match);
        public static event SetMatchedUserHandler UpdateMatchedUser;

        public string name;
        byte[] lenBuffer;
        ReceiveBuffer buffer;
        Socket socket;
        MyPerson loginUser;

        AccessHisDB hisDB;

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
            name = this.EndPoint.ToString();
            hisDB = new AccessHisDB();
        }

        private void SetLoginUser(MyPerson user)
        {
            this.loginUser = user;
        }

        public MyPerson GetLoginUser()
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
            Console.WriteLine("send byte size: " + FingerSensorPacket.ByteToHexString(data));
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

        public void RunAuth(Packet pkt)
        {
            UserManager fpm = new UserManager();
            Code code;
            try
            {
                MyPerson match = fpm.recognition(fpm.Enroll(pkt.data, "guest"));
                if (match != null)
                {
                    Console.WriteLine("Found matched fingerprint.");
                    bool isMatch = CheckLoginUser(match.Id, pkt.userId);
                    if (isMatch)
                    {
                        SetLoginUser(match);
                        UpdateLogMsgWithName("Matched person(" + match.Name.ToString() + ")");
                        UpdateMatchedUser(match);
                        pkt.guid = GetLoginUser().Guid;
                        code = Code.SUCCESS_AUTH;
                    }
                    else
                    {
                        UpdateLogMsgWithName("Not Matched person(" + match.Name.ToString() + ")");
                        code = Code.NOT_MATCH_LOGIN_FP;
                    }
                }
                else
                {
                    Console.WriteLine("Not found matched fingerprint.");
                    code = Code.NOT_FND_FINGERPRINT;
                }
            }
            catch (Exception e)
            {
                code = Code.ERROR;
                pkt.errMsg = e.Message;
            }
            SendResponse(pkt, code);
        }

        public void RunPassenger(Packet pkt)
        {
            Code code;
            try
            {
                if (CheckAuthorizedUser(pkt.guid))
                {
                    AccessInfo info = new AccessInfoDB().SelectNowAccessibleInfo(pkt.guid, pkt.carId);
                    if (info != null)
                    {
                        if (pkt.psgCnt == info.psgCnt)
                        {
                            UpdateLogMsgWithName("Accessed passenger count: " + info.psgCnt);
                            pkt.accessId = info.seq;
                            code = Code.SUCCESS_PASSENGER;
                        }
                        else
                        {
                            code = Code.NOT_MATCH_PASSENGER_CNT;
                        }
                    }
                    else
                    {
                        code = Code.NOT_FND_ACCESS_INFO;
                    }
                }
                else
                {
                    code = Code.INVALID_USER;
                }
            }
            catch (Exception e)
            {
                code = Code.ERROR;
                pkt.errMsg = e.Message;
            }
            SendResponse(pkt, code);
        }

        public void RunOrder(Packet pkt)
        {
            Code code;
            try
            {
                if (CheckAuthorizedUser(pkt.guid))
                {
                    // accessId 를 사용하여 order 를 찾고 그정보를 전송한다.
                    OrderInfo info = new OrderInfoManager().FindOrderInfoByAccessId(pkt.accessId);
                    if (info != null)
                    {
                        code = Code.SUCCESS_ORDER;
                        pkt.order = info;
                    }
                    else
                    {
                        code = Code.NOT_FND_ORDER_INFO;
                    }
                }
                else
                {
                    code = Code.INVALID_USER;
                }
            }
            catch (Exception e)
            {
                code = Code.ERROR;
                pkt.errMsg = e.Message;
            }
            SendResponse(pkt, code);
        }

        private void SendResponse(Packet pkt, Code code)
        {
            if (code == Code.SUCCESS_AUTH || code == Code.SUCCESS_PASSENGER || code == Code.SUCCESS_ORDER)
            {
                pkt.response = PKT_ACK;
            }
            else
            {
                pkt.response = PKT_NACK;
                if (code != Code.ERROR)
                {
                    pkt.errMsg = GetMessage(code.ToString());
                }
            }
            byte[] buffer = DataPacket.StructToByte(pkt);
            Send(buffer, 0, buffer.Length);
            // insert history
            hisDB.InsertAccessHis(pkt.userId, name, code.ToString(), pkt.errMsg);
            UpdateLogMsgWithName("Sent data: " + pkt.ToString());
        }

        public bool CheckLoginUser(int matchedUserId, int userId)
        {
            if (userId == matchedUserId)
            {
                return true;
            }
            return false;
        }

        public bool CheckAuthorizedUser(string guid)
        {
            if (guid == GetLoginUser().Guid)
            {
                return true;
            }
            return false;
        }

        public void UpdateLogMsgWithName(string msg)
        {
            UpdateLogMsg(name + " - " + msg);
        }
    }
    #endregion

}
