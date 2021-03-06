﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public enum PktType : int
    {
        AUTH = 0,
        PASSENGER,
        ORDER,
        ONCE,
        ERROR,
        STRING,
        IMAGE,
        ENROLL,
        RECOG
    }

    [Serializable]
    public class DataPacket
    {
        static public int PKT_ACK = 48;
        static public int PKT_NACK = 49;

        public struct Packet
        {
            public PktType type;
            public int userId;
            public string carId;
            public int response;
            public int dataLen;
            public byte[] data;
            public int psgCnt;
            public string guid;
            public int accessId;
            public Bitmap fingerPrint;
            public string errMsg;
            public OrderInfo order;

            public override string ToString()
            {
                StringBuilder pktStr = new StringBuilder("Packet: ");
                pktStr.Append("TYPE[").Append(type).Append("]");
                pktStr.Append("USER_ID[").Append(userId).Append("]");
                pktStr.Append("CAR_ID[").Append(carId).Append("]");
                pktStr.Append("RESPONSE[").Append(response).Append("]");
                pktStr.Append("DATA_LEN[").Append(dataLen).Append("]");
                if(response == PKT_NACK)
                {
                    pktStr.Append("ERROR_MSG[").Append(errMsg).Append("]");
                }
                if(dataLen > 10 && data != null)
                {
                    pktStr.Append("DATA[").Append(BBDataConverter.ByteToHexString(data.Take(10).ToArray())).Append("...]");
                }
                return pktStr.ToString();
            }
        }

        //public static Packet ByteToStruct(byte[] buffer)
        //{
        //    MemoryStream ms = new MemoryStream(buffer, false);
        //    return ByteToStruct(ms);
        //}

        //public static Packet ByteToStruct(MemoryStream ms)
        //{
        //    BinaryReader br = new BinaryReader(ms);

        //    try
        //    {
        //        Packet pkt = new Packet();
        //        pkt.type = (PktType)br.ReadInt32();
        //        pkt.userId = br.ReadInt32();
        //        pkt.carId = StringUtil.ExtendedTrim(Encoding.UTF8.GetString(br.ReadBytes(16)));
        //        pkt.response = br.ReadInt32();
        //        pkt.dataLen = br.ReadInt32();
        //        switch (pkt.type)
        //        {
        //            case PktType.AUTH:
        //                pkt.data = br.ReadBytes(pkt.dataLen);
        //                //pkt.fingerPrint = Image.FromStream(new MemoryStream(pkt.data));
        //                //pkt.fingerPrint = Image.FromStream(new MemoryStream(BBDataConverter.ImageToByte(BBDataConverter.GrayRawToBitmap(pkt.data, 160, 120))));
        //                pkt.fingerPrint = BBDataConverter.BytesToImage(pkt.data);
        //                break;
        //            case PktType.PASSENGER:
        //                pkt.guid = BBDataConverter.ByteToString(br.ReadBytes(pkt.dataLen - 4));
        //                pkt.psgCnt = BitConverter.ToInt32(br.ReadBytes(4), 0);
        //                break;
        //            case PktType.ORDER:
        //                pkt.guid = BBDataConverter.ByteToString(br.ReadBytes(pkt.dataLen - 4));
        //                pkt.accessId = BitConverter.ToInt32(br.ReadBytes(4), 0);
        //                break;
        //        }

        //        br.Close();
        //        ms.Close();
        //        return pkt;
        //    } 
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public static Packet DataParser(MemoryStream ms, Packet pkt)
        {
            BinaryReader br = new BinaryReader(ms);
            try
            {
                pkt.data = br.ReadBytes(pkt.dataLen);
                switch (pkt.type)
                {
                    case PktType.AUTH:
                        pkt.fingerPrint = BBDataConverter.GrayRawToBitmap(FingerSensor.ScaleImage320x240(pkt.data), FingerSensorPacket.SIZE_FP_WIDTH, FingerSensorPacket.SIZE_FP_HEIGHT);
                        break;
                    case PktType.PASSENGER:
                        pkt.guid = BBDataConverter.ByteToString(pkt.data.Take(pkt.dataLen - 4).ToArray());
                        //pkt.guid = BBDataConverter.ByteToString(br.ReadBytes(pkt.dataLen - 4));
                        //pkt.psgCnt = BitConverter.ToInt32(br.ReadBytes(4), 0);
                        pkt.psgCnt = BBDataConverter.BytesToInt32(pkt.data.Skip(pkt.dataLen - 4).ToArray());
                        break;
                    case PktType.ORDER:
                        pkt.guid = BBDataConverter.ByteToString(pkt.data.Take(pkt.dataLen - 4).ToArray());
                        //pkt.guid = BBDataConverter.ByteToString(br.ReadBytes(pkt.dataLen - 4));
                        //pkt.accessId = BitConverter.ToInt32(br.ReadBytes(4), 0);
                        pkt.accessId = BBDataConverter.BytesToInt32(pkt.data.Skip(pkt.dataLen - 4).ToArray());
                        break;
                    case PktType.ONCE:
                        pkt.psgCnt = BBDataConverter.BytesToInt32(pkt.data.Take(19200).ToArray());
                        pkt.fingerPrint = BBDataConverter.GrayRawToBitmap(FingerSensor.ScaleImage320x240(pkt.data.Skip(4).ToArray()), FingerSensorPacket.SIZE_FP_WIDTH, FingerSensorPacket.SIZE_FP_HEIGHT);
                        break;
                }

                br.Close();
                ms.Close();
                return pkt;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


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
            bw.Write(pkt.response);
            switch (pkt.type)
            {
                case PktType.AUTH:
                    if (pkt.response == DataPacket.PKT_ACK)
                    {
                        byte[] bGuid = BBDataConverter.StringToByte(pkt.guid);
                        bw.Write(bGuid.Length);
                        bw.Write(bGuid);
                    }
                    break;
                case PktType.PASSENGER:
                    if (pkt.response == DataPacket.PKT_ACK)
                    {
                        byte[] accesId = BBDataConverter.Int32ToByte(pkt.accessId);
                        bw.Write(accesId.Length);
                        bw.Write(accesId);
                    }
                    break;
                case PktType.ORDER | PktType.ONCE:
                    if (pkt.response == DataPacket.PKT_ACK)
                    {
                        byte[] id = BBDataConverter.StringToByte(pkt.order.orderId);
                        //byte[] wdt = BBDataConverter.DateTimeToByte(pkt.order.work_dt);
                        // length
                        bw.Write(id.Length);
                        //bw.Write(id.Length + wdt.Length);
                        // data
                        bw.Write(id);
                        //bw.Write(wdt);
                    }
                    break;
            }
            if(pkt.response == PKT_NACK)
            {
                pkt.data = BBDataConverter.StringToByte(pkt.errMsg);
                bw.Write(pkt.data.Length);
                bw.Write(pkt.data);
            }
            bw.Close();
            byte[] buffer = ms.ToArray();
            ms.Dispose();

            return buffer;
        }
    }
}
