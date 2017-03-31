using Npgsql;
using NpgsqlTypes;
using SourceAFIS.Simple;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static AsyncSocketServer.AccessInfoManager;
using static AsyncSocketServer.CarInfoManager;
using static AsyncSocketServer.OrderInfoManager;

namespace AsyncSocketServer
{
    class DBManager
    {
        //private string oradb = "Data Source=192.168.205.163:1521/URYH; User ID=uruser; Password=uruser001";
        private string postdb = "Host=192.168.205.152;Username=isps;Password=GaonIsps@0805!*;Database=ISPS";
        //private string postdb = "Host=121.146.68.152;Username=isps;Password=GaonIsps@0805!*;Database=ISPS";

        //OracleConnection oraConn;
        NpgsqlConnection postConn;

        public DBManager()
        {
            //oraConn = new OracleConnection(oradb);
            postConn = new NpgsqlConnection(postdb);
        }

        //public OracleConnection GetOracleConnection()
        //{
        //    if (oraConn == null)
        //    {
        //        oraConn = new OracleConnection(oradb);
        //    }
        //    return oraConn;
        //}

        public NpgsqlConnection GetPostConnection()
        {
            if (postConn == null)
            {
                postConn = new NpgsqlConnection(postdb);
            }
            return postConn;
        }

        // select
        public List<Dictionary<string, object>> ExecuteQuery(string sql, NpgsqlParameter[] param)
        {
            NpgsqlConnection con = GetPostConnection();
            try
            {
                con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = sql;
                    if (param != null)
                    {
                        foreach (NpgsqlParameter op in param)
                        {
                            cmd.Parameters.Add(op);
                        }
                    }

                    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> obj = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                obj.Add(reader.GetName(i), reader[reader.GetName(i)]);
                            }
                            result.Add(obj);
                        }
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        // update, insert, delete
        public int ExecuteNonQuery(string sql, NpgsqlParameterCollection param)
        {
            NpgsqlConnection con = GetPostConnection();
            int executeCnt = 0;
            try
            {
                con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    try
                    {
                        cmd.Connection = con;
                        cmd.Transaction = con.BeginTransaction();
                        cmd.CommandText = sql;
                        if (param != null)
                        {
                            foreach (NpgsqlParameter op in param)
                            {
                                cmd.Parameters.Add(new NpgsqlParameter(op.ParameterName, op.Value));
                            }
                        }

                        executeCnt = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        cmd.Transaction.Commit();
                    }
                    catch
                    {
                        cmd.Transaction.Rollback();
                    }
                }
                return executeCnt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable GetDBTable(string sql)
        {
            DataTable dt = new DataTable();
            NpgsqlConnection conn = GetPostConnection();
            //OracleConnection conn = db.GetOracleConnection();
            try
            {
                conn.Open();
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sql, conn);
                NpgsqlCommandBuilder builder = new NpgsqlCommandBuilder(adapter);
                //OracleDataAdapter adapter = new OracleDataAdapter(sql, conn);
                //OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                adapter.Fill(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }
    }

    //***************************************************
    // AccessInfo DB
    //***************************************************
    public class AccessInfoDB
    {
        DBManager db = null;

        public AccessInfoDB()
        {
            db = new DBManager();
        }

        public int InsertAccessInfo(AccessInfo info)
        {
            Console.Write("Insert AccessInfo: ");

            string sql_insert = "INSERT INTO isps_access_info(access_info_sq, user_id, psg_cnt, allow_start_dt, allow_end_dt, car_id, purpose, reg_dt)"
                + " VALUES (nextval('sq_isps_access_info'), :USER_ID, :PSG_CNT, :ALLOW_START_DT, :ALLOW_END_DT, :CAR_ID, :PURPOSE, now())";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();
            
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_ID", info.user.Id));
                    cmd.Parameters.Add(new NpgsqlParameter(":PSG_CNT", info.psgCnt));
                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_ID", info.carId));
                    cmd.Parameters.Add(new NpgsqlParameter(":PURPOSE", info.purpose));
                    cmd.Parameters.Add(new NpgsqlParameter(":ALLOW_START_DT", info.allowStartDt));
                    cmd.Parameters.Add(new NpgsqlParameter(":ALLOW_END_DT", info.allowEndDt));

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    Console.WriteLine(ee.Message);
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                Console.WriteLine(executeCnt);
                return executeCnt;
            }
        }

        public int UpdateAccessInfo(AccessInfo info)
        {
            Console.Write("Update AccessInfo: ");
            string sql_insert = "UPDATE isps_access_info SET psg_cnt = :PSG_CNT, allow_start_dt = :ALLOW_START_DT, allow_end_dt = :ALLOW_END_DT, purpose = :PURPOSE, car_id = :CAR_ID, mod_dt = now()"
                + " WHERE access_info_sq = :ACCESS_INFO_SQ";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":ACCESS_INFO_SQ", info.seq));
                    cmd.Parameters.Add(new NpgsqlParameter(":PSG_CNT", info.psgCnt));
                    cmd.Parameters.Add(new NpgsqlParameter(":ALLOW_START_DT", info.allowStartDt));
                    cmd.Parameters.Add(new NpgsqlParameter(":ALLOW_END_DT", info.allowEndDt));
                    cmd.Parameters.Add(new NpgsqlParameter(":PURPOSE", info.purpose));
                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_ID", info.carId));

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    Console.WriteLine(ee.Message);
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                Console.WriteLine(executeCnt);
                return executeCnt;
            }
        }

        public int UpdateAccessDate(int seq)
        {
            Console.Write("Update UpdateAccessDate: ");
            string sql_insert = "UPDATE isps_access_info SET access_dt = now()"
                + " WHERE access_info_sq = :ACCESS_INFO_SQ";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":ACCESS_INFO_SQ", seq));

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    Console.WriteLine(ee.Message);
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                Console.WriteLine(executeCnt);
                return executeCnt;
            }
        }

        public int SelectAccessPsgCnt(string guid, string carId)
        {
            //string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //string sql_selete_ora = "SELECT ACCESS_INFO_SQ, USER_ID, PSG_CNT, TO_CHAR(ALLOW_START_DT,'yyyy-mm-dd HH24:MI:SS') AS ALLOW_START_DT, TO_CHAR(ALLOW_END_DT,'yyyy-mm-dd HH24:MI:SS') AS ALLOW_END_DT" 
            //    + " FROM ISPS_ACCESS_INFO"
            //    + " WHERE USER_ID = (SELECT USER_ID FROM ISPS_USER WHERE USER_GUID = :USER_GUID)"
            //    + " AND TO_DATE('" + nowTime + "', 'yyyy-mm-dd HH24:MI:SS') >= ALLOW_START_DT"
            //    + " AND TO_DATE('" + nowTime + "', 'yyyy-mm-dd HH24:MI:SS') < ALLOW_END_DT";
            string sql_selete = "SELECT access_info_sq, user_id, psg_cnt, allow_start_dt, allow_end_dt"
                + " FROM isps_access_info"
                + " WHERE user_id = (SELECT user_id FROM isps_user WHERE user_guid = :USER_GUID)"
                + " AND car_id = :CAR_ID"
                + " AND now() >= ALLOW_START_DT"
                + " AND now() < ALLOW_END_DT";
            int passengerCnt = -1;
            NpgsqlConnection conn = db.GetPostConnection();
            try
            {
                // 커넥션 오픈
                conn.Open();
                // 커맨드 생성
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql_selete;
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_GUID", guid));
                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_ID", carId));

                    // select 문 쿼리
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            passengerCnt = Int32.Parse(reader["psg_cnt"].ToString());
                            Console.WriteLine(guid + " / " + passengerCnt);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            finally
            {
                conn.Close();
            }
            return passengerCnt;
        }

        public AccessInfo SelectNowAccessibleInfo(string guid, string carId)
        {
            //string sql_selete_ora = "SELECT ACCESS_INFO_SQ, USER_ID, PSG_CNT, TO_CHAR(ALLOW_START_DT,'yyyy-mm-dd HH24:MI:SS') AS ALLOW_START_DT, TO_CHAR(ALLOW_END_DT,'yyyy-mm-dd HH24:MI:SS') AS ALLOW_END_DT" 
            //    + " FROM ISPS_ACCESS_INFO"
            //    + " WHERE USER_ID = (SELECT USER_ID FROM ISPS_USER WHERE USER_GUID = :USER_GUID)"
            //    + " AND TO_DATE('" + nowTime + "', 'yyyy-mm-dd HH24:MI:SS') >= ALLOW_START_DT"
            //    + " AND TO_DATE('" + nowTime + "', 'yyyy-mm-dd HH24:MI:SS') < ALLOW_END_DT";
            string sql_selete = "SELECT access_info_sq, user_id, car_id, psg_cnt, allow_start_dt, allow_end_dt, access_dt"
                + " FROM isps_access_info"
                + " WHERE user_id = (SELECT user_id FROM isps_user WHERE user_guid = :USER_GUID)"
                + " AND car_id = :CAR_ID"
                + " AND now() >= allow_start_dt"
                + " AND now() < allow_end_dt";
            AccessInfo info = null;
            NpgsqlConnection conn = db.GetPostConnection();
            try
            {
                // 커넥션 오픈
                conn.Open();
                // 커맨드 생성
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql_selete;
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_GUID", guid));
                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_ID", carId));

                    // select 문 쿼리
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            info = new AccessInfo();
                            info.seq = Int32.Parse(reader["access_info_sq"].ToString());
                            info.user.Id = Int32.Parse(reader["user_id"].ToString());
                            info.carId = reader["car_id"].ToString();
                            info.psgCnt = Int32.Parse(reader["psg_cnt"].ToString());
                            info.allowStartDt = DateTime.Parse(reader["allow_start_dt"].ToString());
                            info.allowEndDt = DateTime.Parse(reader["allow_end_dt"].ToString());
                            if (ColumnExists(reader, "access_dt"))
                            {
                                info.access_dt = DateTime.Parse(reader["access_dt"].ToString());
                            }
                            Console.WriteLine(info.ToString());
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            finally
            {
                conn.Close();
            }
            return info;
        }

        public AccessInfo SelectAccessInfo(int seq)
        {
            string sql_selete = "SELECT a.access_info_sq, a.user_id, u.user_nm, a.psg_cnt, a.allow_start_dt, a.allow_end_dt, a.is_access, a.access_dt, a.purpose, a.car_id"
                + " FROM isps_access_info a, isps_user u"
                + " WHERE access_info_sq = " + seq
                + " AND a.user_id = u.user_id";
            AccessInfo accessInfo = new AccessInfo();
            NpgsqlConnection conn = db.GetPostConnection();
            try
            {
                // 커넥션 오픈
                conn.Open();
                // 커맨드 생성
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql_selete;

                    // select 문 쿼리
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accessInfo.seq = seq;
                            accessInfo.user = new UserManager.MyPerson();
                            accessInfo.user.Id = Int32.Parse(reader["user_id"].ToString());
                            accessInfo.user.Name = reader["user_nm"].ToString();
                            accessInfo.psgCnt = Int32.Parse(reader["psg_cnt"].ToString());
                            accessInfo.allowStartDt = DateTime.Parse(reader["allow_start_dt"].ToString());
                            accessInfo.allowEndDt = DateTime.Parse(reader["allow_end_dt"].ToString());
                            accessInfo.carId = reader["car_id"].ToString();
                            accessInfo.purpose = reader["purpose"].ToString();
                            if (ColumnExists(reader, "access_dt"))
                            {
                                accessInfo.access_dt = DateTime.Parse(reader["access_dt"].ToString());
                            }
                            Console.WriteLine(accessInfo.ToString());
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            finally
            {
                conn.Close();
            }
            return accessInfo;
        }

        public DataTable GetAccessInfoDBTable(int keyword)
        {
            return GetAccessInfoDBTable(keyword, 0, 0);
        }

        public DataTable GetAccessInfoDBTable(int keyword, int currentPage, int count)
        {
            string sql = "WITH tmp AS (SELECT a.access_info_sq, a.user_id, psg_cnt, allow_start_dt, allow_end_dt, purpose, access_dt, car_id, order_id, COUNT(*) OVER (RANGE UNBOUNDED PRECEDING)"
                + " FROM isps_access_info a LEFT OUTER JOIN isps_order_info b ON a.access_info_sq = b.access_info_sq) SELECT * FROM tmp WHERE user_id = " + keyword + " ORDER BY allow_start_dt DESC";
            //string sql = "WITH tmp AS (SELECT access_info_sq, psg_cnt, allow_start_dt, allow_end_dt, purpose, access_dt, car_id, COUNT(*) OVER (RANGE UNBOUNDED PRECEDING)"
            //    + " FROM isps_access_info WHERE user_id = " + keyword + ") SELECT * FROM tmp ORDER BY allow_start_dt DESC";

            if (count > 0)
            {
                sql += " LIMIT " + count + " OFFSET(" + currentPage + " - 1) * " + count;
            }
            return db.GetDBTable(sql);
        }

        public bool ColumnExists(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == columnName)
                {
                    return true;
                }
            }
            return false;
        }
    }

    //***************************************************
    // User DB
    //***************************************************
    public class UserDB
    {
        DBManager db = null;

        public UserDB()
        {
            db = new DBManager();
        }

        public int InsertISPSUser(UserManager.MyPerson user)
        {
            string sql_insert = "INSERT INTO isps_user (user_id, user_guid, user_nm, user_idnum, phone, fp_data, email, reg_dt) "
                + "VALUES (nextval('sq_isps_user'), :USER_GUID, :USER_NM, :USER_IDNUM, :PHONE, :FP_DATA, :EMAIL, now())";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    //cmd.Parameters.Add(new OracleParameter(":USER_ID", person.Id)); // use sequence
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_GUID", user.Guid));
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_NM", user.Name));
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_IDNUM", user.IdNum));
                    cmd.Parameters.Add(new NpgsqlParameter(":PHONE", user.Phone));
                    cmd.Parameters.Add(new NpgsqlParameter(":EMAIL", user.Email));
                    byte[] fp = BBDataConverter.ImageToByte(user.Fingerprints[0].AsBitmap);
                    NpgsqlParameter op = new NpgsqlParameter(":FP_DATA", NpgsqlDbType.Bytea);
                    op.Value = fp;
                    cmd.Parameters.Add(op);

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                return executeCnt;
            }
        }

        public int DeleteISPSUser(int userId)
        {
            string sql_insert = "DELETE FROM isps_user WHERE user_id = :USER_ID";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_ID", userId));

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                return executeCnt;
            }
        }

        public List<UserManager.MyPerson> SelectISPSUsers()
        {
            //string sql_select = "SELECT USER_ID, USER_GUID, USER_NM, FP_DATA FROM ISPS_USER";
            string sql_select = "SELECT user_id, user_guid, user_nm, user_idnum, phone, email, fp_data FROM isps_user";
            List<UserManager.MyPerson> personList = new List<UserManager.MyPerson>();
            AfisEngine afis = new AfisEngine();
            NpgsqlConnection conn = db.GetPostConnection();
            try
            {
                // 커넥션 오픈
                conn.Open();
                // 커맨드 생성
                using (NpgsqlCommand cmd = new NpgsqlCommand())//new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql_select;

                    // select 문 쿼리
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserManager.MyPerson user = new UserManager.MyPerson();
                            user.Id = Int32.Parse(reader["user_id"].ToString());
                            user.Guid = reader["user_guid"].ToString();
                            user.Name = reader["user_nm"].ToString();
                            user.IdNum = reader["user_idnum"].ToString();
                            user.Phone = reader["phone"].ToString();
                            user.Email = reader["email"].ToString();

                            Console.WriteLine(user.Id + "/" + user.Guid + "/" + user.Name);

                            byte[] binDate = (byte[])reader["fp_data"];
                            UserManager.MyFingerprint fp = new UserManager.MyFingerprint();
                            BitmapImage image = BBDataConverter.ByteToBitmapImage(binDate);
                            fp.AsBitmapSource = image;

                            user.Fingerprints.Add(fp);
                            afis.Extract(user);
                            personList.Add(user);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            finally
            {
                conn.Close();
            }
            return personList;
        }

        public UserManager.MyPerson SelectISPSUser(int userid)
        {
            //string sql_selete = "SELECT USER_ID, USER_GUID, USER_NM, FP_DATA FROM ISPS_USER WHERE USER_ID = :USER_ID";
            string sql_selete = "SELECT user_id, user_guid, user_nm, user_idnum, phone, email, fp_data FROM isps_user WHERE user_id = :USER_ID";

            AfisEngine afis = new AfisEngine();
            UserManager.MyPerson user = new UserManager.MyPerson();
            NpgsqlConnection conn = db.GetPostConnection();
            try
            {
                // 커넥션 오픈
                conn.Open();
                // 커맨드 생성
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    //cmd.CommandText = "SELECT USER_ID, USER_GUID, USER_NM, FP_DATA FROM ISPS_USER WHERE USER_ID = :USER_ID";
                    cmd.CommandText = sql_selete;
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_ID", userid)); // use sequence

                    // select 문 쿼리
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Id = Int32.Parse(reader["user_id"].ToString());
                            user.Guid = reader["user_guid"].ToString();
                            user.Name = reader["user_nm"].ToString();
                            user.IdNum = reader["user_idnum"].ToString();
                            user.Phone = reader["phone"].ToString();
                            user.Email = reader["email"].ToString();

                            Console.WriteLine(user.Id + "/" + user.Guid + "/" + user.Name);

                            byte[] binDate = (byte[])reader["fp_data"];
                            UserManager.MyFingerprint fp = new UserManager.MyFingerprint();
                            BitmapImage image = BBDataConverter.ByteToBitmapImage(binDate);
                            fp.AsBitmapSource = image;
                            user.Fingerprints.Add(fp);
                            afis.Extract(user);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            finally
            {
                conn.Close();
            }
            return user;
        }

        public int UpdateISPSUser(UserManager.MyPerson user)
        {
            string sql_update = "UPDATE isps_user SET user_nm = :USER_NM, user_idnum = :USER_IDNUM, phone = :PHONE, fp_data = :FP_DATA, email = :EMAIL, mod_dt = now()"
                + " WHERE user_id = :USER_ID";
            int executeCnt = 0;
            // 커넥션 오픈
            NpgsqlConnection conn = db.GetPostConnection();
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_update;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_ID", user.Id));
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_NM", user.Name));
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_IDNUM", user.IdNum));
                    cmd.Parameters.Add(new NpgsqlParameter(":PHONE", user.Phone));
                    cmd.Parameters.Add(new NpgsqlParameter(":EMAIL", user.Email));
                    byte[] fp = BBDataConverter.ImageToByte(user.Fingerprints[0].AsBitmap);
                    NpgsqlParameter op = new NpgsqlParameter(":FP_DATA", NpgsqlDbType.Bytea);
                    op.Value = fp;
                    cmd.Parameters.Add(op);

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                return executeCnt;
            }
        }

        public DataTable GetUserDBTable(string keyword)
        {
            return GetUserDBTable(keyword, 0, 0);
        }

        public DataTable GetUserDBTable(string keyword, int currentPage, int count)
        {
            string sql = "WITH tmp AS (SELECT user_id, user_guid, user_nm, user_idnum, phone, email, fp_data, COUNT(*) OVER (RANGE UNBOUNDED PRECEDING)"
                + " FROM isps_user";

            if (keyword != null && keyword != string.Empty)
            {
                sql += " WHERE user_nm LIKE '%" + keyword + "%'";
            }
            sql += ") SELECT * FROM tmp ORDER BY user_nm";

            if (count > 0)
            {
                sql += " LIMIT " + count + " OFFSET(" + currentPage + " - 1) * " + count;
            }
            return db.GetDBTable(sql);
        }
    }

    //***************************************************
    // CarInfo DB
    //***************************************************
    public class CarInfoDB
    {
        DBManager db = null;

        public CarInfoDB()
        {
            db = new DBManager();
        }

        public CarInfo SelectCarInfo(string carId)
        {
            string sql_selete = "SELECT car_id, car_owner, reg_dt, mod_dt"
                + " FROM isps_car"
                + " WHERE car_id = :CAR_ID";
            CarInfo carInfo = new CarInfo();
            NpgsqlConnection conn = db.GetPostConnection();
            try
            {
                // 커넥션 오픈
                conn.Open();
                // 커맨드 생성
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql_selete;

                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_ID", carId));

                    // select 문 쿼리
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            carInfo.id = carId;
                            carInfo.owner = reader["car_owner"].ToString();
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            finally
            {
                conn.Close();
            }
            return carInfo;
        }

        public int InsertCarInfo(CarInfo car)
        {
            string sql_insert = "INSERT INTO isps_car(car_id, car_owner, reg_dt)"
                + " VALUES (:CAR_ID, :CAR_OWNER, now())";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_ID", car.id));
                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_OWNER", car.owner));
                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    Console.WriteLine(ee.Message);
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                return executeCnt;
            }
        }

        public int UpdateCarInfo(CarInfo car)
        {
            string sql_insert = "UPDATE isps_car SET car_owner = :CAR_OWNER, mod_dt = now()"
                + " WHERE car_id = :CAR_ID";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_ID", car.id));
                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_OWNER", car.owner));

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    Console.WriteLine(ee.Message);
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                return executeCnt;
            }
        }

        public int DeleteCarInfo(string carId)
        {
            string sql_insert = "DELETE FROM isps_car WHERE car_id = :CAR_ID";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":CAR_ID", carId));

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                return executeCnt;
            }
        }

        public DataTable GetCarInfoDBTable(string keyword)
        {
            return GetCarInfoDBTable(keyword, 0, 0);
        }

        public DataTable GetCarInfoDBTable(string keyword, int currentPage, int count)
        {
            string sql = "WITH tmp AS (SELECT car_id, car_owner, reg_dt, COUNT(*) OVER (RANGE UNBOUNDED PRECEDING)"
                + " FROM isps_car";

            if (keyword != null && keyword != string.Empty)
            {
                sql += " WHERE car_id LIKE '%" + keyword + "%'";
            }
            sql += ") SELECT * FROM tmp ORDER BY car_id";

            if (count > 0)
            {
                sql += " LIMIT " + count + " OFFSET(" + currentPage + " - 1) * " + count;
            }
            return db.GetDBTable(sql);
        }

        public DataTable GetCarIdDBTable()
        {
            string sql = "SELECT car_id FROM isps_car ORDER BY car_id";
            return db.GetDBTable(sql);
        }

        public string[] GetCarIds(string keyword)
        {
            string sql = "SELECT car_id FROM isps_car";

            if (keyword != null && keyword != String.Empty)
            {
                sql += " WHERE car_id LIKE '%" + keyword + "%' ORDER BY car_id";
            }
            return db.GetDBTable(sql)
                .AsEnumerable()
                .Select(row => row.Field<string>("car_id"))
                .ToArray();
        }
    }

    //***************************************************
    // OrderInfo DB
    //***************************************************
    public class OrderInfoDB
    {
        DBManager db = null;

        public OrderInfoDB()
        {
            db = new DBManager();
        }

        public OrderInfo SelectOrderInfo(int accessId)
        {
            Console.Write("Select OrderInfo: ");

            string sql_selete = "SELECT access_info_sq, order_id, work_dt, reg_dt, mod_dt"
                + " FROM isps_order_info"
                + " WHERE access_info_sq = :ACCESS_INFO_SQ";
            OrderInfo info = null;
            NpgsqlConnection conn = db.GetPostConnection();
            try
            {
                // 커넥션 오픈
                conn.Open();
                // 커맨드 생성
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql_selete;

                    cmd.Parameters.Add(new NpgsqlParameter(":ACCESS_INFO_SQ", accessId));

                    // select 문 쿼리
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        //while (reader.Read())
                        if (reader.Read())
                        {
                            info = new OrderInfo();
                            info.accessId = Int32.Parse(reader["access_info_sq"].ToString());
                            info.orderId = reader["order_id"].ToString();
                            info.work_dt = DateTime.Parse(reader["work_dt"].ToString());
                            Console.WriteLine(info.ToString());
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            finally
            {
                conn.Close();
            }
            return info;
        }

        public int InsertOrderInfo(OrderInfo info)
        {
            Console.Write("Insert OrderInfo: ");

            string sql_insert = "INSERT INTO isps_order_info(access_info_sq, order_id, work_dt, reg_dt, mod_dt)"
                + " VALUES (:ACCESS_INFO_SQ, :ORDER_ID, :WORK_DT, now(), now())";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":ACCESS_INFO_SQ", info.accessId));
                    cmd.Parameters.Add(new NpgsqlParameter(":ORDER_ID", info.orderId));
                    cmd.Parameters.Add(new NpgsqlParameter(":WORK_DT", info.work_dt));

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    Console.WriteLine(ee.Message);
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                Console.WriteLine(executeCnt.ToString());
                return executeCnt;
            }
        }

        public int UpdateOrderInfo(OrderInfo info)
        {
            Console.Write("Update OrderInfo: ");
            string sql_insert = "UPDATE isps_order_info SET order_id = :ORDER_ID, work_dt = :WORK_DT, mod_dt = now()"
                + " WHERE access_info_sq = :ACCESS_INFO_SQ";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":ACCESS_INFO_SQ", info.accessId));
                    cmd.Parameters.Add(new NpgsqlParameter(":ORDER_ID", info.orderId));
                    cmd.Parameters.Add(new NpgsqlParameter(":WORK_DT", info.work_dt));

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    Console.WriteLine(ee.Message);
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                Console.WriteLine(executeCnt.ToString());
                return executeCnt;
            }
        }
    }

    //***************************************************
    // OrderInfo DB
    //***************************************************
    public class AccessHisDB
    {
        DBManager db = null;

        public AccessHisDB()
        {
            db = new DBManager();
        }

        public DataTable GetAccessHisDBTable(string keyword)
        {
            return GetAccessHisDBTable(keyword, 0, 0);
        }


        public DataTable GetAccessHisDBTable(string keyword, int currentPage, int count)
        {
            string sql = "WITH tmp AS (SELECT a.reg_dt, a.rt_code, a.user_id, u.user_nm, a.ip, COUNT(*) OVER (RANGE UNBOUNDED PRECEDING)"
                + " FROM isps_access_his a, isps_user u, isps_comm_code c "
                + " WHERE a.user_id = u.user_id AND a.rt_code = c.code";

            if (keyword != null && keyword != string.Empty)
            {
                sql += " AND u.user_nm LIKE '%" + keyword + "%'";
            }
            sql += ") SELECT * FROM tmp ORDER BY reg_dt DESC";

            if (count > 0)
            {
                sql += " LIMIT " + count + " OFFSET(" + currentPage + " - 1) * " + count;
            }
            return db.GetDBTable(sql);
        }

        public int InsertAccessHis(int userId, string ip, string rtCode, string errMsg)
        {
            Console.Write("Insert AccessHis: ");

            string sql_insert = "INSERT INTO isps_access_his(user_id, ip, rt_code, err_message)"
                + " VALUES (:USER_ID, :IP, :RT_CODE, :ERR_MESSAGE)";
            int executeCnt = 0;
            NpgsqlConnection conn = db.GetPostConnection();
            // 커넥션 오픈
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_ID", userId));
                    cmd.Parameters.Add(new NpgsqlParameter(":IP", ip));
                    cmd.Parameters.Add(new NpgsqlParameter(":RT_CODE", rtCode));
                    cmd.Parameters.Add(new NpgsqlParameter(":ERR_MESSAGE", (errMsg != null) ? errMsg : ""));

                    // execute query
                    executeCnt = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Transaction.Commit();
                }
                catch (Exception ee)
                {
                    cmd.Transaction.Rollback();
                    Console.WriteLine(ee.Message);
                    throw ee;
                }
                finally
                {
                    conn.Close();
                }
                Console.WriteLine(executeCnt.ToString());
                return executeCnt;
            }
        }
    }
}
