using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class CommonConfig
    {
        static public CommonConfig Config;
        //static public Message Msg;

        static public void NewInstance()
        {
            if (CommonConfig.Config == null)
            {
                CommonConfig.Config = new CommonConfig();
            }
        }

        private CommonConfig() {
            //if (Msg == null)
            //{
            //    Msg = new Message();
            //}
        }

        static public class Message
        {
            static private Dictionary<Code, string> MsgDic;
            
            public enum Code : int
            {
                CD_SUCCESS = 1,
                CD_INVALID_USER,
                CD_NOT_FND_AUTH_INFO,
                CD_NOT_FND_ACCESS_INFO,
                CD_NOT_FND_ORDER_INFO,
                CD_NOT_MATCH_AUTH,
                CD_NOT_MATCH_PASSENGER_CNT,
                CD_ERR
            }

            static Message()
            {
                InitMsgDic();
            }

            static private void InitMsgDic()
            {
                MsgDic = new Dictionary<Code, string>();
                MsgDic.Add(Code.CD_NOT_FND_AUTH_INFO, "aaaaa");
                MsgDic.Add(Code.CD_NOT_MATCH_AUTH, "bbbbb");
            }

            static public string GetMessage(Code code)
            {
                if(MsgDic.ContainsKey(code))
                {
                    return MsgDic[code];
                }
                return "error";
            }
        }
    }
}
