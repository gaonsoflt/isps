using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketClient
{
    class StringUtil
    {

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
