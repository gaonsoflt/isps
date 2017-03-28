using AsyncSocketServer;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AsyncSocketServer
{
    public class FingerSensor
    {
        ushort CMD_NONE = 0x00;
        ushort CMD_OPEN = 0x01;
        ushort CMD_CLOSE = 0x02;
        ushort CMD_USB_INTERNAL_CHECK = 0x03;
        ushort CMD_CHANGE_BAUDRATE = 0x04;
        ushort CMD_CMOS_LED = 0x12;
        ushort CMD_ENROLL_COUNT = 0x20;
        ushort CMD_CHECK_ENROLLED = 0x21;
        ushort CMD_ENROLL_START = 0x22;
        ushort CMD_ENROLL1 = 0x23;
        ushort CMD_ENROLL2 = 0x24;
        ushort CMD_ENROLL3 = 0x25;
        ushort CMD_IS_PRESS_FINGER = 0x26;
        ushort CMD_DELETE = 0x40;
        ushort CMD_DELETE_ALL = 0x41;
        ushort CMD_VERIFY = 0x50;
        ushort CMD_IDENTIFY = 0x51;
        ushort CMD_VERIFY_TEMPLATE = 0x52;
        ushort CMD_IDENTIFY_TEMPLATE = 0x53;
        ushort CMD_CAPTURE = 0x60;
        ushort CMD_GET_IMAGE = 0x62;
        ushort CMD_GET_RAWIMAGE = 0x63;
        ushort CMD_GET_TEMPLATE = 0x70;
        ushort CMD_ADD_TEMPLATE = 0x71;
        ushort CMD_GET_DATABASE_START = 0x72;
        ushort CMD_GET_DATABASE_END = 0x73;
        ushort CMD_FW_UPDATE = 0x80;
        ushort CMD_ISO_UPDATE = 0x81;
        ushort ACK_OK = 0x30;
        ushort NACK_INFO = 0x31;

        ushort NACK_NONE = 0x1000;
        ushort NACK_TIMEOUT = 0x1001;
        ushort NACK_INVALID_BAUDRATE = 0x1002;
        ushort NACK_INVALID_POS = 0x1003;
        ushort NACK_IS_NOT_USED = 0x1004;
        ushort NACK_IS_ALREADY_USED = 0x1005;
        ushort NACK_COMM_ERR = 0x1006;
        ushort NACK_VERIFY_FAILED = 0x1007;
        ushort NACK_IDENTIFY_FAILED = 0x1008;
        ushort NACK_DB_IS_FULL = 0x1009;
        ushort NACK_DB_IS_EMPTY = 0x1010;
        ushort NACK_TURN_ERR = 0x1011;
        ushort NACK_BAD_FINGER = 0x1012;
        ushort NACK_ENROLL_FAILED = 0x1013;
        ushort NACK_IS_NOT_SUPPORTED = 0x1014;
        ushort NACK_DEV_ERR = 0x1015;
        ushort NACK_CAPTURE_CANCELED = 0x1016;
        ushort NACK_INVALID_PARAM = 0x1017;
        ushort NACK_FINGER_IS_NOT_PRESSED = 0x1018;

        int OEM_NONE = -2000;
        int OEM_COMM_ERR = -1999;

        int PKT_ERR_START =	-500;
        int PKT_COMM_ERR = -499;
        int PKT_HDR_ERR	= -498;
        int PKT_DEV_ID_ERR = -497;
        int PKT_CHK_SUM_ERR	= -496;
        int PKT_PARAM_ERR = -495;

        int SB_OEM_PKT_SIZE = 12;

        ushort devID = 0x0001;

        // Header Of Cmd and Ack Packets
        ushort STX1 = 0x55;	//Header1 
        ushort STX2 = 0xAA;	//Header2

        // Header Of Data Packet
        ushort STX3 = 0x5A;	//Header1 
        ushort STX4 = 0xA5;	//Header2


        private static FingerSensor fingerSensor;
        public SerialPort sPort;
        bool isRecv = false;

        ushort lastAck = 0;

        private FingerSensor()
        {
        }

        public static FingerSensor GetFingerSensorInstance()
        {
            if (fingerSensor == null)
            {
                fingerSensor = new FingerSensor();
            }
            return fingerSensor;
        }

        private int RunCmd(ushort cmd, int param)
        {
            if (SendCmdOrAck(cmd, param) < 0)
                return OEM_COMM_ERR;
            if (SerialResponseReceiver() < 0)
                return OEM_COMM_ERR;
            return 0;
        }

        private int SendCmdOrAck(ushort cmd, int param)
        {
            FingerSensorPacket.SB_OEM_PKT pkt = new FingerSensorPacket.SB_OEM_PKT();
            pkt.head1 = (byte)STX1;
            pkt.head2 = (byte)STX2;
            pkt.devId = devID;
            pkt.cmd = cmd;
            pkt.param = param;
            pkt.chkSum = FingerSensorPacket.CalcChkSumOfCmdAckPkt(pkt);

            SendCmd(pkt, SB_OEM_PKT_SIZE);
            return 0;
        }

        public int CmdChangeBaudrate(int rate)
        {
            return RunCmd(CMD_CHANGE_BAUDRATE, rate);
        }

        public int CmdCaptureFinger()
        {
            int tryCnt = 0;
            while (true)
            {
                if (RunCmd(CMD_CAPTURE, 1) < 0)
                {
                    return -1;
                }
                if (lastAck == ACK_OK)
                {
                    lastAck = 0;
                    return 0;
                }
                if(tryCnt < 5)
                {
                    tryCnt++;
                    Console.WriteLine("try capture finger: " + tryCnt);
                    Delay(1000);
                } else
                {
                    return -1;
                }
            }
        }


        public int CmdGetImage()
        {
            if(RunCmd(CMD_GET_IMAGE, 0) < 0)
            {
                return OEM_COMM_ERR;
            }

            // headerSize + DeviceIdSize + checkSumSize(2 + 2 + 2)
            if (SerialDataReceiver(gbyImg256_tmp.Length + 2 + 2 + 2) < 0)
            {
                return OEM_COMM_ERR;
            }

            // image rotate
            for (int i = 0; i < 202; i++)
            {
                for (int j = 0; j < 258; j++)
                {
                    gbyImg256_2[i * 258 + j] = gbyImg256_tmp[j * 202 + i];
                }
            }

            //memset(gbyImg8bit, 161, sizeof(gbyImg8bit));
            for (int i = 0; i < gbyImg8bit.Length; i++)
            {
                gbyImg8bit[i] = 161;
            }

            for (int i = 0; i < 202; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    //memcpy(&gbyImg8bit[256 * (27 + i) + 0], &gbyImg256_2[i * 258 + 1], 256);
                    gbyImg8bit[256 * (27 + i) + 0 + j] = gbyImg256_2[i * 258 + 1 + j];
                }
            }
            return 0;
        }

        public int CmdGetRawImage()
        {

            if (RunCmd(CMD_GET_RAWIMAGE, 0) < 0)
            {
                return OEM_COMM_ERR;
            }
            if (SerialDataReceiver(gbyImgRaw_tmp.Length + 2 + 2 + 2) < 0)
            {
                return OEM_COMM_ERR;
            }

            //for (int i = 0; i < gbyImgRaw.Length; i++)
            //{
            //    gbyImgRaw[i] = 66;
            //}
            //for (int i = 0; i < 120; i++)
            //{
            //    for (int j = 0; j < 160; j++)
            //    {
            //        gbyImgRaw[320 * (2 * i + 0) + (2 * j + 0)] = gbyImgRaw_tmp[i * 160 + j];
            //        gbyImgRaw[320 * (2 * i + 0) + (2 * j + 1)] = gbyImgRaw_tmp[i * 160 + j];
            //        gbyImgRaw[320 * (2 * i + 1) + (2 * j + 0)] = gbyImgRaw_tmp[i * 160 + j];
            //        gbyImgRaw[320 * (2 * i + 1) + (2 * j + 1)] = gbyImgRaw_tmp[i * 160 + j];
            //    }
            //}
            gbyImgRaw = ScaleImage320x240(gbyImgRaw_tmp);
            return 0;
        }

        public static byte[] ScaleImage320x240(byte[] bytes)
        {
            if (bytes.Length <= (160 * 120))
            {
                int size = 320 * 240;
                byte[] scaledImg = Enumerable.Repeat<byte>(0x42, size).ToArray();
                for (int i = 0; i < 120; i++)
                {
                    for (int j = 0; j < 160; j++)
                    {
                        scaledImg[320 * (2 * i + 0) + (2 * j + 0)] = bytes[i * 160 + j];
                        scaledImg[320 * (2 * i + 0) + (2 * j + 1)] = bytes[i * 160 + j];
                        scaledImg[320 * (2 * i + 1) + (2 * j + 0)] = bytes[i * 160 + j];
                        scaledImg[320 * (2 * i + 1) + (2 * j + 1)] = bytes[i * 160 + j];
                    }
                }
                return scaledImg;
            }
            return bytes;
        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                FileStream _FileStream = new FileStream(_FileName, FileMode.Create, FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        public int CmdCmosLed(bool enable)
        {
            return RunCmd(CMD_CMOS_LED, enable ? 1 : 0);
        }

        public int CmdOpen()
        {
            if (RunCmd(CMD_OPEN, 1) < 0)
            {
                return OEM_COMM_ERR;
            }

            // device info packet size = 30
            if (SerialDataReceiver(buffDevInfo.Length + 6) < 0)
            {
                return OEM_COMM_ERR;
            }
            return 0;
        }

        public int CmdClose()
        {
            return RunCmd(CMD_CLOSE, 0);
        }

        public byte[] getCaptureImage()
        {
            return this.gbyImg8bit;
        }

        public byte[] getRawImage()
        {
            return this.gbyImgRaw;
        }

        public byte[] getRawImage160x120()
        {
            return this.gbyImgRaw_tmp;
        }

        private void SendCmd(FingerSensorPacket.SB_OEM_PKT pkt, int pktSize)
        {
            byte[] _pkt = FingerSensorPacket.StructureToByte(pkt);
            Console.WriteLine("SEND: " + FingerSensorPacket.ByteToHexString(_pkt));
            isRecv = true;
            sPort.Write(_pkt, 0, pktSize);
            //SerialDataReceived();
        }

        public void OpenSerialPort(String portName, int baudRate)
        {
            if (null == sPort)
            {
                sPort = new SerialPort();
            }
            //sPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceived);
            sPort.PortName = portName;
            sPort.BaudRate = baudRate;
            sPort.DataBits = (int)8;
            sPort.Parity = Parity.None;
            sPort.StopBits = StopBits.One;
            sPort.ReadTimeout = (int)500;
            sPort.WriteTimeout = (int)500;
            sPort.Open();
            Console.WriteLine("Opened SerialPort: " + portName + "/" + baudRate);
        }

        byte[] buffDevInfo = new byte[24]; 
        byte[] gbyImg8bit = new byte[256 * 256];
        byte[] gbyImg256_2 = new byte[202 * 258];
        byte[] gbyImg256_tmp = new byte[258 * 202];
        byte[] gbyImgRaw = new byte[320 * 240];
        byte[] gbyImgRaw_tmp = new byte[240 * 320 / 4]; // 19200
        //byte[] gbyImg2
        private int SerialResponseReceiver()
        {
            if (sPort.IsOpen)
            {
                Thread.Sleep(1000);
                string strRecData;

                if (sPort.BytesToRead > 0)
                {
                    strRecData = "";
                    byte[] buff = new byte[SB_OEM_PKT_SIZE];
                    sPort.Read(buff, 0, SB_OEM_PKT_SIZE);
                    strRecData = FingerSensorPacket.ByteToHexString(buff);
                    Console.WriteLine("RECE: " + strRecData);
                    //55 AA 01 00 00 00 00 00 30 00 30 01 
                    if (buff[0] != STX1 || buff[1] != STX2)
                    {
                        return PKT_HDR_ERR;
                    }
                    if (BitConverter.ToUInt16(buff, 2) != devID)
                    {
                        return PKT_DEV_ID_ERR;
                    }
                    lastAck = BitConverter.ToUInt16(buff, 8);
                } else
                {
                    return PKT_COMM_ERR;
                }
            }
            return 0;
        }

        private int SerialDataReceiver(int buffSize)
        {
            if (sPort.IsOpen)
            {
                string strRecData;
                Console.WriteLine("buff size: " + buffSize);
                while (sPort.BytesToRead < buffSize) ;
                if (sPort.BytesToRead > 0)
                {
                    strRecData = "";
                    byte[] comBuff = new byte[buffSize];
                    //comBuff = Encoding.UTF8.GetBytes(sPort.ReadExisting());
                    int rSize = sPort.Read(comBuff, 0, buffSize);
                    Console.WriteLine("serial read size: " + rSize);
                    strRecData = FingerSensorPacket.ByteToHexString(comBuff);
                    Console.WriteLine("RECE: " + strRecData);
                    /*
                     * [0]      5A          : Data start code1
                     * [1]      A5          : Data start code2
                     * [2-3]    DeviceID    : Device ID
                     * [n..]    Data        : n byte data
                     * [4+n]    checkSum    : checksum  
                     */
                    if (comBuff[0] != STX3 || comBuff[1] != STX4)
                    {
                        return PKT_HDR_ERR;
                    }
                    if (BitConverter.ToUInt16(comBuff, 2) != devID)
                    {
                        return PKT_DEV_ID_ERR;
                    }
                    switch(buffSize - 6)
                    {
                        case 52116: // Image
                            Array.Copy(comBuff, 4, gbyImg256_tmp, 0, buffSize - 6);
                            break;
                        case 30: // open device info
                            Array.Copy(comBuff, 4, buffDevInfo, 0, buffSize - 6);
                            break;
                        case 19200: // rawImage
                            Array.Copy(comBuff, 4, gbyImgRaw_tmp, 0, buffSize - 6);
                            break;
                    }
                }
                else
                {
                    return PKT_COMM_ERR;
                }
            }
            return 0;
        }

        public void CloseSerialPort()
        {
            if (null != sPort)
            {
                if (sPort.IsOpen)
                {
                    sPort.Close();
                    sPort.Dispose();
                    Console.WriteLine("Closed SerialPort: " + sPort.PortName + "/" + sPort.BaudRate);
                    //sPort = null;
                }
            }
        }

        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);
            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }
            return DateTime.Now;
        }

    }
}
