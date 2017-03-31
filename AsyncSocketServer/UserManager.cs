using System;
using System.Windows.Media.Imaging;
using System.IO;
using SourceAFIS.Simple; // import namespace SourceAFIS.Simple
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Drawing;
using System.Data.OracleClient;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace AsyncSocketServer
{
    public class UserManager
    {
        public enum MODE : int
        {
            SAVE = 0,
            LOAD,
            MODIFY,
            DELETE,
            RECOG
        }
        //public static int SAVE = 0;
        //public static int LOAD = 1;
        //public static int MODIFY = 2;
        //public static int DELETE = 3;
        //public static int RECOG = 4;

        // Shared AfisEngine instance (cannot be shared between different threads though)
        private AfisEngine afis;
        private static List<MyPerson> database = new List<MyPerson>();
        private static string DATABASE = "database.dat";

        private UserDB db;

        public UserManager()
        {
            // Look up the probe using Threshold = 10
            afis = new AfisEngine();
            afis.Threshold = 25;

            db = new UserDB();
        }

        // Inherit from Fingerprint in order to add Filename field
        [Serializable]
        public class MyFingerprint : Fingerprint
        {
            public string Filename;
            internal SourceAFIS.Templates.Template Decoded;
        }

        // Inherit from Person in order to add Name field
        [Serializable]
        public class MyPerson : Person
        {
            public string Name;
            public string Guid;
            public string IdNum;
            public string Phone;
            public string Email;
        }


        public void recognition(string filename, string username)
        {
            MyPerson probeUser = Enroll(filename, username);
            recognition(probeUser);
        }

        public MyPerson recognition(MyPerson probeUser)
        {
            MyPerson matchUser = null;
            try
            {
                //matchUser = UserManager.afis.Identify(probeUser, loadUsers()).FirstOrDefault() as MyPerson;

                matchUser = (from candidate in loadUsers()
                             let score = afis.Verify(probeUser, candidate)
                             where score >= afis.Threshold
                             orderby score descending
                             select candidate).FirstOrDefault();
                if (matchUser == null)
                {
                    Console.WriteLine("No matching person found.");
                    Console.Out.WriteLine();
                    return matchUser;
                }
                // Print out any non-null result
                //Console.WriteLine("Probe {0} matches registered person {1}", probeUser.Name, matchUser.Name);

                // Compute similarity score
                //float scorea = UserManager.afis.Verify(probeUser, matchUser);
                //Console.WriteLine("Similarity score between {0} and {1} = {2:F3}", probeUser.Name, matchUser.Name, scorea);
                Console.Out.WriteLine();
                return matchUser;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Out.WriteLine();
            }
            return matchUser;
        }

        public static float VerifyUserMatchRate(MyPerson tarUser, MyPerson refUser)
        {
            return new AfisEngine().Verify(tarUser, refUser);
        }


        // Take fingerprint image file and create Person object from the image
        public MyPerson Enroll(string filename, string name)
        {
            try
            {
                // Load image from the file
                Console.WriteLine(" Loading image from {0}...", filename);
                BitmapImage image = new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute));
                return Enroll(image, 0, name, "", "", "");
            } catch (Exception fnfe)
            {
                throw fnfe;
            }
        }

        public MyPerson Enroll(byte[] source, string name)
        {
            return Enroll(BBDataConverter.ByteToBitmapImage(source), 0, name, "", "", "");
        }

        public MyPerson Enroll(byte[] source, string name, string idnum, string phone, string email)
        {
            return Enroll(BBDataConverter.ByteToBitmapImage(source), 0, name, idnum, phone, email);
        }

        public MyPerson Enroll(byte[] source, int id, string name, string idnum, string phone, string email)
        {
            return Enroll(BBDataConverter.ByteToBitmapImage(source), id, name, idnum, phone, email);
        }

        public MyPerson Enroll(BitmapImage source, string name)
        {
            return Enroll(source, 0, name, "", "", "");
        }

        private MyPerson Enroll(BitmapImage source, int id, string name, string idnum, string phone, string email)
        {
            Console.WriteLine("Enrolling {0}...", name);
            MyPerson person = new MyPerson();

            // Initialize empty fingerprint object and set properties
            MyFingerprint fp = new MyFingerprint();
            try
            {
                // Load image from the file
                Console.WriteLine(" Loading image");
                fp.AsBitmapSource = source;
                // Above update of fp.AsBitmapSource initialized also raw image in fp.Image
                // Check raw image dimensions, Y axis is first, X axis is second
                Console.WriteLine(" Image size = {0} x {1} (width x height)", fp.Image.GetLength(1), fp.Image.GetLength(0));

                // Initialize empty person object and set its properties
                person.Id = id;
                person.Guid = Guid.NewGuid().ToString();
                person.Name = name;
                person.IdNum = idnum;
                person.Phone = phone;
                person.Email = email;
                // Add fingerprint to the person
                person.Fingerprints.Add(fp);

                // Execute extraction in order to initialize fp.Template
                //Console.WriteLine(" Extracting template..." + BBDataConverter.BitmapToByte(BBDataConverter.BitmapSourceToBitmap(fp.AsBitmapSource)).Length);
                Console.WriteLine(" Extracting template...");
                afis.Extract(person);
                //foreach (MyFingerprint item_0 in person.Fingerprints)
                //{
                //    SourceAFIS.Templates.TemplateBuilder local_1 = new SourceAFIS.Extraction.Extractor().Extract(item_0.Image, 500);
                //    item_0.Decoded = new SourceAFIS.Templates.SerializedFormat().Export(local_1);
                //}
                // Check template size
                Console.WriteLine(" Template size = {0} bytes", fp.Template.Length);

            }
            catch (Exception fnfe)
            {
                Console.WriteLine("a:" + fnfe.StackTrace);
                //throw fnfe;
            }
            return person;
        }

        [Obsolete("Not used file", true)]
        public void showDatabase()
        {
            foreach (MyPerson person in loadUsers())
            {
                Console.WriteLine(person.Id + ":" + person.Name);
            }
        }

        [Obsolete("Not used file", true)]
        public List<MyPerson> loadDatabaseFile()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            if(!File.Exists(DATABASE))
            {
                File.Create(DATABASE).Close();
            }
            Console.WriteLine("Reloading database...");
            try
            {
                using (FileStream stream = File.OpenRead(DATABASE)) UserManager.database = (List<MyPerson>)formatter.Deserialize(stream);
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return UserManager.database;
            }
            return UserManager.database;
        }

        [Obsolete("Not used file", true)]
        public int savePersonFile(MyPerson user)
        {
            try
            {
                loadUsers().Add(user);
                BinaryFormatter formatter = new BinaryFormatter();
                Console.WriteLine("Saving database...");
                using (Stream stream = File.Open(DATABASE, FileMode.Create)) formatter.Serialize(stream, UserManager.database);

                return UserManager.database.Count;
            }
            catch
            {
                return 0;
            }
        }

        public List<MyPerson> loadUsers()
        {
            Console.WriteLine("Reloading database...");
            try
            {
                List<MyPerson> result = db.SelectISPSUsers();
                Console.WriteLine("loaded count: [" + result.Count + "]");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public int InsertUser(string filename, string username)
        {
            return InsertUser(Enroll(filename, username));
        }

        public int InsertUser(MyPerson user)
        {
            Console.Write("Saving database...[");
            try
            {
                int executeCnt = db.InsertISPSUser(user);
                Console.WriteLine(executeCnt + "]");
                return executeCnt;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "]");
                return 0;
            }
        }

        public int UpdateUser(MyPerson user)
        {
            Console.Write("Updating database...[");
            try
            {
                int executeCnt = db.UpdateISPSUser(user);
                Console.WriteLine(executeCnt + "]");
                return executeCnt;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "]");
                return 0;
            }
        }

        public int SaveUser(MyPerson user)
        {
            int executeCnt = 0;

            executeCnt = UpdateUser(user);
            if (executeCnt <= 0)
            {
                executeCnt = InsertUser(user);
            }

            return executeCnt;
        }
    }

}
