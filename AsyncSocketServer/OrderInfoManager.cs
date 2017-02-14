using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class OrderInfo
    {
        public int accessId;
        public string orderId;
        public DateTime work_dt;
        public DateTime reg_dt;
        public DateTime mod_dt;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Order infor: ");
            sb.Append("ACCESS_ID[").Append(accessId).Append("]");
            sb.Append("ORDER_ID[").Append(orderId).Append("]");
            sb.Append("WORK_DT[").Append(work_dt.ToString()).Append("]");
            sb.Append("REG_DT[").Append(reg_dt.ToString()).Append("]");
            sb.Append("MOD_DT[").Append(mod_dt.ToString()).Append("]");
            return sb.ToString();
        }
    }

    public class OrderInfoManager
    {
        OrderInfoDB db;

        public OrderInfoManager()
        {
            db = new OrderInfoDB();
        }

        public int SaveOrderInfo(OrderInfo info)
        {
            Console.WriteLine("Save OrderInfo to database...");
            try
            {
                int executeCnt = db.UpdateOrderInfo(info);
                if(executeCnt <= 0)
                {
                    executeCnt = db.InsertOrderInfo(info);
                }
                return executeCnt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public OrderInfo FindOrderInfoByAccessId(int accessId)
        {
            Console.WriteLine("Findng database...");
            try
            {
                return db.SelectOrderInfo(accessId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
