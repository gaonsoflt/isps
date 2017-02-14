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
            static private Dictionary<string, string> MsgDic;

            public enum Code : int
            {
                ERROR = 1,
                SUCCESS_AUTH,
                SUCCESS_PASSENGER,
                SUCCESS_ORDER,
                INVALID_USER,
                NOT_FND_ACCESS_INFO,
                NOT_FND_FINGERPRINT,
                NOT_FND_ORDER_INFO,
                NOT_MATCH_LOGIN_FP,
                NOT_MATCH_PASSENGER_CNT
            }

            static Message()
            {
                InitMsgDic();
            }

            static private void InitMsgDic()
            {
                string sql = "SELECT code, value FROM isps_comm_code WHERE code_type = 'RESPONSE'";
                DataTable dt = new DBManager().GetDBTable(sql);
                MsgDic = dt.AsEnumerable()
                    .ToDictionary<DataRow, string, string>
                    (row => row.Field<string>(0), row => row.Field<string>(1));
            }

            static public string GetMessage(string code)
            {
                if (MsgDic.ContainsKey(code))
                {
                    return MsgDic[code];
                }
                return "unknown error";
            }
        }
    }
}
