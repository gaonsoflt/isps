using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncSocketServer
{
    public partial class AccessDialog : Form
    {
        int userId = 0;
        public AccessDialog(int userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        private void AccessDialog_Load(object sender, EventArgs e)
        {
            allowStartDt.Format = DateTimePickerFormat.Custom;
            allowStartDt.CustomFormat = "yyyy-MM-dd(ddd) HH:mm:ss";
            allowEndDt.Format = DateTimePickerFormat.Custom;
            allowEndDt.CustomFormat = "yyyy-MM-dd(ddd) HH:mm:ss";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
