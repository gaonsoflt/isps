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

        // 문자열 뒤쪽에 위치한 null 을 제거한 후에 공백문자를 제거한다.
        static public string ExtendedTrim(string source)
        {
            string dest = source;
            int index = dest.IndexOf('\0');
            if (index > -1)
            {
                dest = source.Substring(0, index + 1);
            }

            return dest.TrimEnd('\0').Trim();
        }
    }
}
