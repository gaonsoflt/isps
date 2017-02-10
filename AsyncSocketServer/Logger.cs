using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocketServer
{
    class Logger
    {
        private string clsName;
        private string TAG = "LOG";

        public static Logger createLogger(string clsName)
        {
            return new Logger(clsName);
        }

        private Logger(string clsName)
        {
            this.clsName = clsName;
        }

        public void setTAG(string tag)
        {
            this.TAG = tag;
        }
        
        public void log(string format)
        {
            Console.WriteLine("[" + TAG + "]" + clsName + " - " + DateTime.Now + "\t" + format);
        }

        public void log(string format, params object[] arg)
        {
            Console.WriteLine("[" + TAG + "]" + clsName + "-" + DateTime.Now + "\t" + format, arg);
        }

        public void log(string format, object arg0)
        {
            Console.WriteLine("[" + TAG + "]" + clsName + "-" + DateTime.Now + "\t" + format, arg0);
        }

        public void log(string format, object arg0, object arg1)
        {
            Console.WriteLine("[" + TAG + "]" + clsName + "-" + DateTime.Now + "\t" + format, arg0, arg1);
        }       
    }
}
