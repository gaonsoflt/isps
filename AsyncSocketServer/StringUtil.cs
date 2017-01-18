using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    class StringUtil
    {
        // 바이트 배열을 String으로 변환 
        static public string ByteToString(byte[] strByte)
        {
            string str = Encoding.Default.GetString(strByte);
            return str;
        }
        // String을 바이트 배열로 변환 
        static public byte[] StringToByte(string str)
        {
            byte[] StrByte = null;
            if (str != null)
            {
                StrByte = Encoding.UTF8.GetBytes(str);
            } else
            {
                StrByte = Encoding.UTF8.GetBytes("");
            }
            return StrByte;
        }
    }
}
