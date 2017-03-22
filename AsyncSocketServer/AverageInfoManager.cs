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

            List<Dictionary<string, object>> result = db.ExecuteQuery(sql, param);
            return result;
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
            string sql = "SELECT * FROM "
                + "(SELECT COUNT(*) as req_total FROM isps_access_info WHERE (:DT_TARGET < allow_end_dt::DATE + INTERVAL '1 days' AND :DT_TARGET >= allow_start_dt::DATE)) A, "
                + "(SELECT COUNT(*) as access_cnt FROM isps_access_info WHERE (:DT_TARGET < allow_end_dt::DATE + INTERVAL '1 days' AND :DT_TARGET >= allow_start_dt::DATE) AND access_dt IS NOT NULL) B, "
                + "(SELECT COUNT(*) as not_access_cnt FROM isps_access_info WHERE (:DT_TARGET < allow_end_dt::DATE + INTERVAL '1 days' AND :DT_TARGET >= allow_start_dt::DATE) AND access_dt IS NULL) C";
            NpgsqlParameter[] param =
            {
                new NpgsqlParameter(":DT_TARGET", dt.Date)
            };

            List<Dictionary<string, object>> result = db.ExecuteQuery(sql, param);
            return result;
        }
    }
}
