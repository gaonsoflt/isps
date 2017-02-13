using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
            static private Dictionary<string, string> MsgDic2;

            public enum Code : int
            {
                SUCCESS = 1,
                INVALID_USER,
                NOT_FND_AUTH_INFO,
                NOT_FND_ACCESS_INFO,
                NOT_FND_ORDER_INFO,
                NOT_MATCH_AUTH,
                NOT_MATCH_PASSENGER_CNT,
                ERROR
            }

            static Message()
            {
                InitMsgDic();
            }

            static private void InitMsgDic()
            {
                MsgDic = new Dictionary<Code, string>();
                MsgDic.Add(Code.NOT_FND_AUTH_INFO, "aaaaa");
                MsgDic.Add(Code.NOT_MATCH_AUTH, "bbbbb");

                string sql = "SELECT code, value FROM isps_comm_code WHERE code_type = 'RESPONSE'";
                DataTable dt = new DBManager().GetDBTable(sql);
                MsgDic2 = dt.AsEnumerable()
                    .ToDictionary<DataRow, string, string>
                    (row => row.Field<string>(0), row => row.Field<string>(1));
            }

            static public string GetMessage(Code code)
            {
                if(MsgDic.ContainsKey(code))
                {
                    return MsgDic[code];
                }
                return "unknown error";
            }

            static public string GetMessage2(string code)
            {
                if (MsgDic2.ContainsKey(code))
                {
                    return MsgDic2[code];
                }
                return "unknown error";
            }
        }
    }
}
