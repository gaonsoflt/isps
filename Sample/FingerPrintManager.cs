using System;
using System.Windows.Media.Imaging;
using System.IO;
using SourceAFIS.Simple; // import namespace SourceAFIS.Simple
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace Sample
{
    public class FingerPrintManager
    {
        public static int SAVE = 0;
        public static int LOAD = 1;
        public static int MODIFY = 2;
        public static int DELETE = 3;
        public static int RECOG = 4;

        // Shared AfisEngine instance (cannot be shared between different threads though)
        private static AfisEngine afis = new AfisEngine();
        private static List<MyPerson> database = new List<MyPerson>();
        private static string DATABASE = "database.dat";

        public FingerPrintManager()
        {
            // Look up the probe using Threshold = 10
            FingerPrintManager.afis.Threshold = 10;
        }

        // Inherit from Fingerprint in order to add Filename field
        [Serializable]
        public class MyFingerprint : Fingerprint
        {
            public string Filename;
        }

        // Inherit from Person in order to add Name field
        [Serializable]
        public class MyPerson : Person
        {
            public string Name;
        }


        public void recognition(string filename, string username)
        {
            try
            {
                MyPerson probe = Enroll(filename, username);
                MyPerson match = FingerPrintManager.afis.Identify(probe, loadDatabase()).FirstOrDefault() as MyPerson;
                if (match == null)
                {
                    Console.WriteLine("No matching person found.");
                    Console.Out.WriteLine();
                    return;
                }
                // Print out any non-null result
                Console.WriteLine("Probe {0} matches registered person {1}", probe.Name, match.Name);

                // Compute similarity score
                float score = FingerPrintManager.afis.Verify(probe, match);
                Console.WriteLine("Similarity score between {0} and {1} = {2:F3}", probe.Name, match.Name, score);
                Console.Out.WriteLine();
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Out.WriteLine();
            }
        }

        // Take fingerprint image file and create Person object from the image
        public MyPerson Enroll(string filename, string name)
        {
            Console.WriteLine("Enrolling {0}...", name);

            // Initialize empty fingerprint object and set properties
            MyFingerprint fp = new MyFingerprint();
            fp.Filename = filename;
            try
            {
                // Load image from the file
                Console.WriteLine(" Loading image from {0}...", filename);
                BitmapImage image = new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute));
                fp.AsBitmapSource = image;
                // Above update of fp.AsBitmapSource initialized also raw image in fp.Image
                // Check raw image dimensions, Y axis is first, X axis is second
                Console.WriteLine(" Image size = {0} x {1} (width x height)", fp.Image.GetLength(1), fp.Image.GetLength(0));

                // Initialize empty person object and set its properties
                MyPerson person = new MyPerson();
                person.Name = name;
                // Add fingerprint to the person
                person.Fingerprints.Add(fp);

                // Execute extraction in order to initialize fp.Template
                Console.WriteLine(" Extracting template...");
                afis.Extract(person);
                // Check template size
                Console.WriteLine(" Template size = {0} bytes", fp.Template.Length);

                return person;
            } catch (Exception fnfe)
            {
                throw fnfe;
            }
        }

        public void showDatabase()
        {
            foreach (MyPerson person in loadDatabase())
            {
                Console.WriteLine(person.Id + ":" + person.Name);
            }
        }

        public List<MyPerson> loadDatabase()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            if(!File.Exists(DATABASE))
            {
                File.Create(DATABASE).Close();
            }
            Console.WriteLine("Reloading database...");
            try
            {
                using (FileStream stream = File.OpenRead(DATABASE)) FingerPrintManager.database = (List<MyPerson>)formatter.Deserialize(stream);
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return FingerPrintManager.database;
            }
            return FingerPrintManager.database;
        }

        public int savePerson(string filename, string username)
        {
            try
            {
                loadDatabase().Add(Enroll(filename, username));
                BinaryFormatter formatter = new BinaryFormatter();
                Console.WriteLine("Saving database...");
                using (Stream stream = File.Open(DATABASE, FileMode.Create)) formatter.Serialize(stream, FingerPrintManager.database);

                return FingerPrintManager.database.Count;
            } catch
            {
                return 0;
            }
        }
    }
}
