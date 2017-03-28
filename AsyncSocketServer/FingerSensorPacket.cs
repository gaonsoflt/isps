using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    [Serializable]
    public class FingerSensorPacket
    {
        // Structure Of Cmd and Ack Packets 
        //typedef struct {
        //    BYTE Head1;
        //    BYTE Head2;
        //    WORD wDevId;
        //    int nParam;
        //    WORD wCmd;// or nAck
        //    WORD wChkSum;
        //} SB_OEM_PKT;		
        public static int SIZE_FP_WIDTH = 320;
        public static int SIZE_FP_HEIGHT = 240;

        public struct SB_OEM_PKT
        {
            public byte head1 { get; set; }
            public byte head2 { get; set; }
            public ushort devId { get; set; }
            public int param { get; set; }
            public ushort cmd { get; set; }
            public ushort chkSum { get; set; }
            //MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)] 
            //public byte[] Data;//byte배열 길이 1024
        }

        public static ushort CalcChkSumOfCmdAckPkt(SB_OEM_PKT pkt)
        {
            ushort chkSum = 0;

            byte[] _pkt = StructureToByte(pkt);
            //Console.Write("_pkt length: " + _pkt.Length);

            for (int i = 0; i < _pkt.Length; i++)
                chkSum += _pkt[i];
                        
            return chkSum;
        }

        // 구조체를 byte 배열로
        public static byte[] StructureToByte(object obj)
        {
            int datasize = Marshal.SizeOf(obj);//((PACKET_DATA)obj).TotalBytes; // 구조체에 할당된 메모리의 크기를 구한다.
            IntPtr buff = Marshal.AllocHGlobal(datasize); // 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당한다.
            Marshal.StructureToPtr(obj, buff, false); // 할당된 구조체 객체의 주소를 구한다.
            byte[] data = new byte[datasize]; // 구조체가 복사될 배열
            Marshal.Copy(buff, data, 0, datasize); // 구조체 객체를 배열에 복사
            Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함
            return data; // 배열을 리턴
        }

        //byte 배열을 구조체로
        public static object ByteToStructure(byte[] data, Type type)
        {
            IntPtr buff = Marshal.AllocHGlobal(data.Length); // 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
            Marshal.Copy(data, 0, buff, data.Length); // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
            object obj = Marshal.PtrToStructure(buff, type); // 복사된 데이터를 구조체 객체로 변환한다.
            Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함

            if (Marshal.SizeOf(obj) != data.Length)// (((PACKET_DATA)obj).TotalBytes != data.Length) // 구조체와 원래의 데이터의 크기 비교
            {
                return null; // 크기가 다르면 null 리턴
            }
            return obj; // 구조체 리턴
        }

        public static string ByteToHexString(byte[] buff)
        {
            string rtn = "";
            for (int i = 0; i < buff.Length; i++)
            {
                rtn += buff[i].ToString("X2") + " ";
            }
            return rtn;
        }
    }
}
