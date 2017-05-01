using AsyncSocketServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServerWPF
{
    public class ConfWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ConfDataBase conf;

        public ConfWindowModel()
        {
            conf = new ConfDataBase();
            vendorList = new ObservableCollection<string>();
            vendorList.Add("PostgreSql");
            //vendorList.Add("Oracle");
            InitForm();
        }

        private void InitForm()
        {
            Vendor = conf.Vendor;
            IP = conf.IP;
            Port = conf.Port;
            User = conf.User;
            Password = conf.Password;
            SID = conf.SID;
        }

        public void SaveForm()
        {
            conf.Vendor = Vendor;
            conf.IP = IP;
            conf.Port = Port;
            conf.User = User;
            conf.Password = Password;
            conf.SID = SID;
            conf.Save();
        }

        private string vendor;
        public string Vendor
        {
            get { return vendor; }
            set
            {
                vendor = value;
                OnPropertyUpdate("Vendor");
            }
        }

        private ObservableCollection<string> vendorList;
        public ObservableCollection<string> VendorList
        {
            get { return (this.vendorList != null) ? this.vendorList : new ObservableCollection<string>(); }
            set
            {
                this.vendorList = value;
                OnPropertyUpdate("VendorList");
            }
        }

        private string ip;
        public string IP
        {
            get { return ip; }
            set
            {
                ip = value;
                OnPropertyUpdate("IP");
            }
        }

        private string port;
        public string Port
        {
            get { return port; }
            set
            {
                port = value;
                OnPropertyUpdate("Port");
            }
        }

        private string user;
        public string User
        {
            get { return user; }
            set
            {
                user = value;
                OnPropertyUpdate("User");
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyUpdate("Password");
            }
        }

        private string sid;
        public string SID
        {
            get { return sid; }
            set
            {
                sid = value;
                OnPropertyUpdate("SID");
            }
        }

        private void OnPropertyUpdate(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
