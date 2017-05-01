using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class ConfDataBase
    {
        private string vendor;
        private string ip;
        private string port;
        private string user;
        private string password;
        private string sid;

        public ConfDataBase()
        {
            Load(); 
        }

        public void Load()
        {
            vendor = Properties.Settings.Default.Vendor;
            ip = Properties.Settings.Default.IP;
            port = Properties.Settings.Default.Port;
            user = Properties.Settings.Default.User;
            password = Properties.Settings.Default.Password;
            sid = Properties.Settings.Default.SID;
        }

        public void Save()
        {
            Properties.Settings.Default.Vendor = vendor;
            Properties.Settings.Default.IP = ip;
            Properties.Settings.Default.Port = port;
            Properties.Settings.Default.User = user;
            Properties.Settings.Default.Password = password;
            Properties.Settings.Default.SID = sid;
            Properties.Settings.Default.Save();
        }

        public string Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }

        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        public string User
        {
            get { return user; }
            set { user = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string SID
        {
            get { return sid; }
            set { sid = value; }
        }
    }
}
