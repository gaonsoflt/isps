using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    class DataPacket
    {
        [MarshalAs(UnmanagedType.I4)]
        public Commands type;
        [MarshalAs(UnmanagedType.I4)]
        public int userId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string carId;
        [MarshalAs(UnmanagedType.I4)]
        public int response;
        [MarshalAs(UnmanagedType.I4)]
        public int dataLen;
        public byte[] data;
    }
}
