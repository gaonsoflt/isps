using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class CarInfoManager
    {
        public enum DIALOG_MODE : int
        {
            SAVE = 0,
            LOAD,
            MODIFY,
            DELETE
        }

        public class CarInfo
        {
            public string id;
            public string owner;
            public DateTime reg_dt;
            public DateTime mod_dt;
        }

        CarInfoDB db;

        public CarInfoManager()
        {
            db = new CarInfoDB();
        }

        public int InsertCarInfo(CarInfo car)
        {
            Console.Write("Saving database...[");
            try
            {
                int executeCnt = db.InsertCarInfo(car);
                Console.WriteLine(executeCnt + "]");
                return executeCnt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "]");
                return 0;
            }
        }

        public int UpdateCarInfo(CarInfo car)
        {
            Console.Write("Updating database...[");
            try
            {
                int executeCnt = db.UpdateCarInfo(car);
                Console.WriteLine(executeCnt + "]");
                return executeCnt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "]");
                return 0;
            }
        }

        public int SaveCarInfo(CarInfo car)
        {
            int executeCnt = 0;

            executeCnt = UpdateCarInfo(car);
            if (executeCnt <= 0)
            {
                executeCnt = InsertCarInfo(car);
            }

            return executeCnt;
        }

        public int DeleteCarInfo(string carId)
        {
            Console.Write("Deleting database...[");
            try
            {
                int executeCnt = db.DeleteCarInfo(carId);
                Console.WriteLine(executeCnt + "]");
                return executeCnt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "]");
                return 0;
            }
        }

        public bool CheckExistCarId(string carId)
        {
            CarInfo info = db.SelectCarInfo(carId);
            if(info.id.Equals(carId))
            {
                return true;
            }
            return false;
        }

        public string[] SuggestStrings(string carId)
        {
            return db.GetCarIds(carId);
        }

        public DataTable GetCarIdDBTable()
        {
            return db.GetCarIdDBTable();
        }
    }
}
