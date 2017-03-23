using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class AverageInfoManager
    {
        DBManager db;

        public AverageInfoManager()
        {
            db = new DBManager();
        }

        public List<Dictionary<string, object>> SelectAccessTotalInfo(DateTime dt)
        {
            string sql = "SELECT * FROM "
                + "(SELECT COUNT(*) as req_total FROM isps_access_info WHERE (:DT_TARGET < allow_end_dt::DATE + INTERVAL '1 days' AND :DT_TARGET >= allow_start_dt::DATE)) A, "
                + "(SELECT COUNT(*) as access_cnt FROM isps_access_info WHERE (:DT_TARGET < allow_end_dt::DATE + INTERVAL '1 days' AND :DT_TARGET >= allow_start_dt::DATE) AND access_dt IS NOT NULL) B, "
                + "(SELECT COUNT(*) as not_access_cnt FROM isps_access_info WHERE (:DT_TARGET < allow_end_dt::DATE + INTERVAL '1 days' AND :DT_TARGET >= allow_start_dt::DATE) AND access_dt IS NULL) C";
            NpgsqlParameter[] param =
            {
                new NpgsqlParameter(":DT_TARGET", dt.Date)
            };

            try
            {
                List<Dictionary<string, object>> result = db.ExecuteQuery(sql, param);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SelectAccessHistoryInfo(DateTime dt)
        {
            string DT_TARGET = dt.Date.ToString("yyyy-MM-dd");
            string sql = "WITH tmp AS (SELECT a.access_dt, u.user_nm, a.car_id"
                + " FROM isps_access_info a, isps_user u"
                + " WHERE a.user_id = u.user_id"
                + " AND ('" + DT_TARGET + "' < allow_end_dt::DATE + INTERVAL '1 days' AND '" + DT_TARGET + "' >= allow_start_dt::DATE)"
                + " AND a.access_dt IS NOT NULL"
                + ") SELECT * FROM tmp ORDER BY access_dt";

            return db.GetDBTable(sql);
        }

        public List<Dictionary<string, object>> SelectAccessMonthlyTotal(DateTime dt)
        {
            string sql = "SELECT to_char(dd, 'yyyy-MM') AS mon, COUNT(access_info_sq)"
                + " FROM (SELECT date_trunc('month', (to_date(:DT_TARGET, 'yyyy-MM-dd') - offs)) AS dd FROM generate_series(0, 365, 31) as offs) AS a"
                + " LEFT OUTER JOIN"
                + " isps_access_info AS b"
                + " ON to_char(a.dd, 'yyyy-MM') = to_char(b.allow_start_dt, 'yyyy-MM')"
                + " GROUP BY mon ORDER BY mon";
            NpgsqlParameter[] param =
            {
                new NpgsqlParameter(":DT_TARGET", dt.Date.ToString("yyyy-MM-dd"))
            };

            try
            {
                List<Dictionary<string, object>> result = db.ExecuteQuery(sql, param);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Dictionary<string, object>> SelectAccessDailyTimeTotal(DateTime dt)
        {
            string sql = "SELECT to_char(hh, 'HH24:MI') AS hm, COUNT(access_info_sq)"
                + " FROM (SELECT generate_series AS hh FROM generate_series("
                + "'" + dt.ToString("yyyy-MM-dd") + " 00:00:00'::timestamp, '" + dt.ToString("yyyy-MM-dd") + " 23:59:59', '1 hours')) AS a"
                //+ ":DT_FROM::timestamp, :DT_TO, '1 hours')) AS a"
                + " LEFT OUTER JOIN"
                + " isps_access_info AS b"
                + " ON to_char(a.hh, 'yyyy-MM-dd HH24') = to_char(b.access_dt, 'yyyy-MM-dd HH24')"
                + " GROUP BY hm ORDER BY hm";
            //NpgsqlParameter[] param =
            //{
            //    new NpgsqlParameter(":DT_FROM", dt.Date),
            //    new NpgsqlParameter(":DT_TO", dt.Date.AddHours(23).AddMinutes(59).AddSeconds(59))
            //};

            try
            {
                List<Dictionary<string, object>> result = db.ExecuteQuery(sql, null);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
