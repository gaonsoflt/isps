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
            public bool isAccess;
            public DateTime access_dt;
            public string carId;
            public string purpose;
            public DateTime reg_dt;
            public DateTime mod_dt;
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

        public int UpdateAccessInfo(AccessInfo info)
        {
            Console.Write("Updating database...[");
            try
            {
                int executeCnt = new AccessInfoDB().UpdateAccessInfo(info);
                Console.WriteLine(executeCnt + "]");
                return executeCnt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "]");
                return 0;
            }
        }
    }
}
