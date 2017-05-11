using AsyncSocketServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static AsyncSocketServer.DataPacket;

namespace AsyncSocketClient
{
    class DataPacket
    {
        public static Packet ByteToStructHeader(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer, false);
            return ByteToStructHeader(ms);
        }

        public static Packet ByteToStructHeader(MemoryStream ms)
        {
            BinaryReader br = new BinaryReader(ms);

            try
            {
                Packet pkt = new Packet();
                pkt.type = (PktType)br.ReadInt32();
                pkt.userId = br.ReadInt32();
                pkt.carId = StringUtil.ExtendedTrim(Encoding.UTF8.GetString(br.ReadBytes(16)));
                pkt.response = br.ReadInt32();
                pkt.dataLen = br.ReadInt32();

                br.Close();
                ms.Close();
                return pkt;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Packet DataParser(MemoryStream ms, Packet pkt)
        {
            BinaryReader br = new BinaryReader(ms);
            if (pkt.response == PKT_NACK)
            {
                pkt.errMsg = BBDataConverter.ByteToString(br.ReadBytes(pkt.dataLen));
            }
            else
            {
                pkt.data = br.ReadBytes(pkt.dataLen);
                switch (pkt.type)
                {
                    case PktType.AUTH:
                        pkt.guid = BBDataConverter.ByteToString(pkt.data);
                        break;
                    case PktType.PASSENGER:
                        pkt.accessId = BBDataConverter.BytesToInt32(pkt.data);
                        break;
                    case PktType.ORDER:
                        if (pkt.order == null)
                        {
                            pkt.order = new OrderInfo();
                        }
                        pkt.order.orderId = BBDataConverter.ByteToString(pkt.data);
                        break;
                    case PktType.ONCE:
                        if (pkt.order == null)
                        {
                            pkt.order = new OrderInfo();
                        }
                        pkt.order.orderId = BBDataConverter.ByteToString(pkt.data);
                        break;
                }
            }

            br.Close();
            ms.Close();

            return pkt;
        }

        public static byte[] StructToByte(Packet pkt)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((int)pkt.type);
            bw.Write(pkt.userId);
            try
            {
                byte[] carId = new byte[16];
                Encoding.UTF8.GetBytes(pkt.carId, 0, pkt.carId.Length, carId, 0);
                bw.Write(carId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : {0}", ex.Message.ToString());
            }
            bw.Write((int)0);
            byte[] guid;
            switch (pkt.type)
            {
                case PktType.AUTH:
                    if (pkt.fingerPrint != null)
                    {
                        byte[] fp = BBDataConverter.ImageToByte(pkt.fingerPrint);
                        bw.Write(fp.Length);
                        bw.Write(fp);
                    } else
                    {
                        bw.Write(0);
                    }
                    break;
                case PktType.PASSENGER:
                    guid = BBDataConverter.StringToByte(pkt.guid);
                    byte[] psgCnt = BBDataConverter.Int32ToByte(pkt.psgCnt);
                    bw.Write(guid.Length + psgCnt.Length);
                    bw.Write(guid);
                    bw.Write(psgCnt);
                    break;
                case PktType.ORDER:
                    guid = BBDataConverter.StringToByte(pkt.guid);
                    byte[] accessId = BBDataConverter.Int32ToByte(pkt.accessId);
                    bw.Write(guid.Length + accessId.Length);
                    bw.Write(guid);
                    bw.Write(accessId);
                    break;
                case PktType.ONCE:
                    byte[] psgCnt1 = BBDataConverter.Int32ToByte(pkt.psgCnt);
                    bw.Write(psgCnt1.Length + pkt.data.Length);
                    bw.Write(psgCnt1);
                    bw.Write(pkt.data);
                    break;
            }
            bw.Close();
            byte[] buffer = ms.ToArray();
            ms.Dispose();

            return buffer;
        }
    }
}
