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

namespace AsyncSocketServer
{
    class DBManager
    {
        private string oradb = "Data Source=192.168.205.163:1521/URYH; User ID=uruser; Password=uruser001";
        private string postdb = "Host=192.168.205.152;Username=isps;Password=GaonIsps@0805!*;Database=ISPS";

        OracleConnection oraConn;
        NpgsqlConnection postConn;

        public DBManager()
        {
            oraConn = new OracleConnection(oradb);
            postConn = new NpgsqlConnection(postdb);
        }

        public OracleConnection GetOracleConnection()
        {
            if (oraConn == null)
            {
                oraConn = new OracleConnection(oradb);
            }
            return oraConn;
        }

        public NpgsqlConnection GetPostConnection()
        {
            if (postConn == null)
            {
                postConn = new NpgsqlConnection(postdb);
            }
            return postConn;
        }
    }

    public class AccessInfoDB
    {
        DBManager db = null;

        public AccessInfoDB()
       {
            db = new DBManager();
        }

        /*
         * 
         * 
         * insert into isps_access_info(
  access_info_sq,
  user_id,
  psg_cnt, -- 동승자수
  allow_start_dt,
  allow_end_dt
) values (
  nextval('sq_isps_access_info'),
  3,
  1,
  to_timestamp('2017-01-19 12:00:00', 'yyyy-mm-dd hh24:mi:ss'),
  to_timestamp('2017-01-19 12:00:00', 'yyyy-mm-dd hh24:mi:ss')
);


         * 
         */

        public int InsertAccessInfo(AccessInfo info)
        {
            string sql_insert = "INSERT INTO isps_access_info(access_info_sq, user_id, psg_cnt, allow_start_dt, allow_end_dt, is_access) "
                + "VALUES (nextval('sq_isps_access_info'), :USER_ID, :PSG_CNT, :ALLOW_START_DT, :ALLOW_END_DT, :IS_ACCESS)";
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
                    NpgsqlParameter p_isAccess = new NpgsqlParameter(":IS_ACCESS", NpgsqlDbType.Integer);
                    p_isAccess.Value = 0;
                    cmd.Parameters.Add(p_isAccess);
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
                return executeCnt;
            }
        }

        public int UpdateAccessInfo(AccessInfo info)
        {
            string sql_insert = "UPDATE isps_access_info SET psg_cnt = :PSG_CNT, allow_start_dt = :ALLOW_START_DT, allow_end_dt = :ALLOW_END_DT "
                + "WHERE access_info_sq = :ACCESS_INFO_SQ";
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

        public int SelectAccessPsgCnt(string guid)
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
                    cmd.Parameters.Add(new NpgsqlParameter(":USER_GUID", guid)); // use sequence

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

        public AccessInfo SelectAccessInfo(int seq)
        {
            string sql_selete = "SELECT a.access_info_sq, a.user_id, u.user_nm, a.psg_cnt, a.allow_start_dt, a.allow_end_dt, a.is_access, a.access_dt"
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
                            accessInfo.isAccess = (Int32.Parse(reader["is_access"].ToString()) > 0) ? true : false;
                            accessInfo.access_dt = DateTime.Parse(reader["access_dt"].ToString());
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
    }

    public class UserDB
    {
        DBManager db = null;

        public UserDB()
        {
            db = new DBManager();
        }

        public int InsertISPSUser(UserManager.MyPerson user)
        {
            string sql_insert = "INSERT INTO isps_user (user_id, user_guid, user_nm, user_idnum, phone, fp_data) "
                + "VALUES (nextval('sq_isps_user'), :USER_GUID, :USER_NM, :USER_IDNUM, :PHONE, :FP_DATA)";
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
                    byte[] fp = BBImageConverter.ImageToByte(user.Fingerprints[0].AsBitmap);
                    NpgsqlParameter op = new NpgsqlParameter(":FP_DATA", NpgsqlDbType.Bytea);
                    op.Value = fp;
                    //NpgsqlParameter op = new NpgsqlParameter();
                    //op.ParameterName = ":FP_DATA";
                    //op.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bytea;
                    //op.Direction = ParameterDirection.Input;
                    //op.Size = fp.Length;
                    //op.Value = fp;
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
            string sql_select = "SELECT user_id, user_guid, user_nm, user_idnum, phone, fp_data FROM isps_user";
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

                            Console.WriteLine(user.Id + "/" + user.Guid + "/" + user.Name);

                            byte[] binDate = (byte[])reader["fp_data"];
                            UserManager.MyFingerprint fp = new UserManager.MyFingerprint();
                            BitmapImage image = BBImageConverter.byteToBitmapImage(binDate);
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
            string sql_selete = "SELECT user_id, user_guid, user_nm, user_idnum, phone, fp_data FROM isps_user WHERE user_id = :USER_ID";

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

                            Console.WriteLine(user.Id + "/" + user.Guid + "/" + user.Name);

                            byte[] binDate = (byte[])reader["fp_data"];
                            UserManager.MyFingerprint fp = new UserManager.MyFingerprint();
                            BitmapImage image = BBImageConverter.byteToBitmapImage(binDate);
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

        internal int UpdateISPSUser(UserManager.MyPerson user)
        {
            string sql_update = "UPDATE isps_user SET user_nm = :USER_NM, user_idnum = :USER_IDNUM, phone = :PHONE, fp_data = :FP_DATA WHERE user_id = :USER_ID";
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
                    byte[] fp = BBImageConverter.ImageToByte(user.Fingerprints[0].AsBitmap);
                    NpgsqlParameter op = new NpgsqlParameter(":FP_DATA", NpgsqlDbType.Bytea);
                    op.Value = fp;
                    cmd.Parameters.Add(op);
                    //byte[] fp = BBImageConverter.ImageToByte(user.Fingerprints[0].AsBitmap);
                    //NpgsqlParameter op = new NpgsqlParameter();
                    //op.ParameterName = ":FP_DATA";
                    //op.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bytea;
                    //op.Direction = ParameterDirection.Input;
                    //op.Size = fp.Length;
                    //op.Value = fp;

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

        public DataTable GetDBTable(string sql)
        {
            DataTable dt = new DataTable();
            NpgsqlConnection conn = db.GetPostConnection();
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

        public DataTable GetUserDBTable(string keyword)
        {
            string sql = "SELECT user_id, user_guid, user_nm, user_idnum, phone, fp_data FROM isps_user";
            if (keyword != null && keyword != String.Empty)
            {
                sql += " WHERE user_nm LIKE '%" + keyword + "%'";
            }
            return GetDBTable(sql);
        }

        public DataTable GetAccessDBTable(int userId)
        {
            string sql = "SELECT access_info_sq, psg_cnt, allow_start_dt, allow_end_dt, is_access, access_dt "
                + "FROM isps_access_info WHERE user_id = " + userId + " ORDER BY allow_start_dt DESC";
            return GetDBTable(sql);
        }
    }

}
