using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class AccessInfoManager
    {
        public enum DIALOG_MODE : int
        {
            SAVE = 0,
            LOAD,
            MODIFY,
            DELETE
        }

        AccessInfoDB db;

        public class AccessInfo
        {
            public int seq;
            public UserManager.MyPerson user = new UserManager.MyPerson();
            public int psgCnt;
            public DateTime allowStartDt;
            public DateTime allowEndDt;
            public DateTime access_dt;
            public string carId;
            public string purpose;
            public DateTime reg_dt;
            public DateTime mod_dt;
            public OrderInfo order;
        }

        public AccessInfoManager()
        {
            db = new AccessInfoDB();
        }

        public int SaveAccessInfo(AccessInfo info)
        {
            Console.WriteLine("Save AccessInfo database...");
            try
            {
                int executeCnt = db.UpdateAccessInfo(info);
                if (executeCnt <= 0)
                {
                    executeCnt = db.InsertAccessInfo(info);
                }
                return executeCnt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public AccessInfo SelectNowAccessibleInfo(string guid, string carId)
        {
            return db.SelectNowAccessibleInfo(guid, carId);
        }

        public AccessInfo SelectAccessInfoWithOrder(int seq)
        {
            AccessInfo info = db.SelectAccessInfo(seq);
            info.order = new OrderInfoDB().SelectOrderInfo(seq);
            return info;
        }

        public int SaveAccessInfoWithOrder(AccessInfo info)
        {
            Console.WriteLine("Save AccessInfoWithOrder database...");
            try
            {
                int executeCnt = db.UpdateAccessInfo(info);
                if (executeCnt <= 0)
                {
                    executeCnt = db.InsertAccessInfo(info);
                }
                // 귀차니즘~~ 나중에 바꿔야지~ ㅠㅠ
                int executeCnt1 = 0;
                if (info.order != null)
                {
                    OrderInfoDB orderDB = new OrderInfoDB();
                    info.order.accessId = info.seq;
                    executeCnt1 = orderDB.UpdateOrderInfo(info.order);
                    if (executeCnt1 <= 0)
                    {
                        executeCnt1 = orderDB.InsertOrderInfo(info.order);
                    }
                }
                return executeCnt + executeCnt1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}
