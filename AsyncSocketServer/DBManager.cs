using SourceAFIS.Simple;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AsyncSocketServer
{
    class DBManager
    {
        string oradb = "Data Source=192.168.205.163:1521/URYH; User ID=uruser; Password=uruser001";
        OracleConnection conn;

        public DBManager()
        {
            conn = new OracleConnection(oradb);
        }

        public OracleConnection GetOracleConnection()
        {
            if (conn == null)
            {
                conn = new OracleConnection(oradb);
            }
            return conn;
        }
    }

    public class AccessInfoDB
    {
        DBManager db = null;

        public AccessInfoDB()
        {
            db = new DBManager();
        }

        public int SelectISPSAccessInfo(string guid)
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql_selete = "SELECT ACCESS_INFO_SQ, USER_ID, PSG_CNT, TO_CHAR(ALLOW_START_DT,'yyyy-mm-dd HH24:MI:SS') AS ALLOW_START_DT, TO_CHAR(ALLOW_END_DT,'yyyy-mm-dd HH24:MI:SS') AS ALLOW_END_DT" 
                + " FROM ISPS_ACCESS_INFO"
                + " WHERE USER_ID = (SELECT USER_ID FROM ISPS_USER WHERE USER_GUID = :USER_GUID)"
                + " AND TO_DATE('" + nowTime + "', 'yyyy-mm-dd HH24:MI:SS') >= ALLOW_START_DT"
                + " AND TO_DATE('" + nowTime + "', 'yyyy-mm-dd HH24:MI:SS') < ALLOW_END_DT";
            int passengerCnt = -1;
            OracleConnection conn = db.GetOracleConnection();
            try
            {
                // 커넥션 오픈
                conn.Open();
                // 커맨드 생성
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql_selete;
                    cmd.Parameters.Add(new OracleParameter(":USER_GUID", guid)); // use sequence

                    // select 문 쿼리
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(guid + " / " + reader["PSG_CNT"].ToString());
                            passengerCnt = Int32.Parse(reader["PSG_CNT"].ToString());
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
    }

    public class UserDB
    {
        DBManager db = null;
        //string oradb = "Data Source=192.168.205.163:1521/URYH; User ID=uruser; Password=uruser001";
        //OracleConnection conn;

        //public UserDBManager()
        //{
        //    conn = new OracleConnection(oradb);
        //}

        //OracleConnection GetOracleConnection()
        //{
        //    if (conn == null)
        //    {
        //        conn = new OracleConnection(oradb);
        //    }
        //    return conn;
        //}

        public UserDB()
        {
            db = new DBManager();
        }

        public int InsertISPSUser(UserManager.MyPerson user)
        {
            string sql_insert = "INSERT INTO ISPS_USER (USER_ID, USER_GUID, USER_NM, FP_DATA) VALUES (SQ_ISPS_USER.nextval, :USER_GUID, :USER_NM, :FP_DATA)";
            int executeCnt = 0;
            OracleConnection conn = db.GetOracleConnection();
            // 커넥션 오픈
            conn.Open();

            using (OracleCommand cmd = new OracleCommand())
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
                    cmd.Parameters.Add(new OracleParameter(":USER_GUID", user.Guid));
                    cmd.Parameters.Add(new OracleParameter(":USER_NM", user.Name));
                    byte[] fp = BBImageConverter.ImageToByte(user.Fingerprints[0].AsBitmap);
                    OracleParameter op = new OracleParameter();
                    op.ParameterName = ":FP_DATA";
                    op.OracleType = OracleType.Blob;
                    op.Direction = ParameterDirection.Input;
                    op.Size = fp.Length;
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
            string sql_insert = "DELETE FROM ISPS_USER WHERE USER_ID = :USER_ID";
            int executeCnt = 0;
            OracleConnection conn = db.GetOracleConnection();
            // 커넥션 오픈
            conn.Open();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_insert;

                    // set parameters
                    cmd.Parameters.Add(new OracleParameter(":USER_ID", userId));

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
            string sql_select = "SELECT USER_ID, USER_GUID, USER_NM, FP_DATA FROM ISPS_USER";
            List<UserManager.MyPerson> personList = new List<UserManager.MyPerson>();
            AfisEngine afis = new AfisEngine();
            OracleConnection conn = db.GetOracleConnection();
            try
            {
                // 커넥션 오픈
                db.GetOracleConnection().Open();
                // 커맨드 생성
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql_select;

                    // select 문 쿼리
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["USER_ID"].ToString() + "/" + reader["USER_GUID"].ToString() + "/" + reader["USER_NM"].ToString());

                            UserManager.MyPerson user = new UserManager.MyPerson();
                            user.Id = Int32.Parse(reader["USER_ID"].ToString());
                            user.Guid = reader["USER_GUID"].ToString();
                            user.Name = reader["USER_NM"].ToString();

                            byte[] binDate = (byte[])reader["FP_DATA"];
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
            string sql_selete = "SELECT USER_ID, USER_GUID, USER_NM, FP_DATA FROM ISPS_USER WHERE USER_ID = :USER_ID";
            UserManager.MyPerson user = new UserManager.MyPerson();
            OracleConnection conn = db.GetOracleConnection();
            try
            {
                // 커넥션 오픈
                conn.Open();
                // 커맨드 생성
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    //cmd.CommandText = "SELECT USER_ID, USER_GUID, USER_NM, FP_DATA FROM ISPS_USER WHERE USER_ID = :USER_ID";
                    cmd.CommandText = sql_selete;
                    cmd.Parameters.Add(new OracleParameter(":USER_ID", userid)); // use sequence

                    // select 문 쿼리
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["USER_ID"].ToString() + "/" + reader["USER_GUID"].ToString() + "/" + reader["USER_NM"].ToString());

                            user.Id = Int32.Parse(reader["USER_ID"].ToString());
                            user.Guid = reader["USER_GUID"].ToString();
                            user.Name = reader["USER_NM"].ToString();

                            byte[] binDate = (byte[])reader["FP_DATA"];
                            UserManager.MyFingerprint fp = new UserManager.MyFingerprint();
                            BitmapImage image = BBImageConverter.byteToBitmapImage(binDate);
                            fp.AsBitmapSource = image;

                            user.Fingerprints.Add(fp);
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
            string sql_update = "UPDATE ISPS_USER SET USER_NM = :USER_NM, FP_DATA = :FP_DATA WHERE USER_ID = :USER_ID";
            int executeCnt = 0;
            // 커넥션 오픈
            OracleConnection conn = db.GetOracleConnection();
            conn.Open();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    // 커맨드에 커넥션, 트랜잭선 추가
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    // set query
                    cmd.CommandText = sql_update;

                    // set parameters
                    cmd.Parameters.Add(new OracleParameter(":USER_ID", user.Id));
                    //cmd.Parameters.Add(new OracleParameter(":USER_GUID", user.Guid));
                    cmd.Parameters.Add(new OracleParameter(":USER_NM", user.Name));
                    byte[] fp = BBImageConverter.ImageToByte(user.Fingerprints[0].AsBitmap);
                    OracleParameter op = new OracleParameter();
                    op.ParameterName = ":FP_DATA";
                    op.OracleType = OracleType.Blob;
                    op.Direction = ParameterDirection.Input;
                    op.Size = fp.Length;
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

        public DataTable GetDBTable(string sql)
        {
            DataTable dt = new DataTable();
            OracleConnection conn = db.GetOracleConnection();
            try
            {
                conn.Open();
                OracleDataAdapter adapter = new OracleDataAdapter(sql, conn);
                OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
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

}
