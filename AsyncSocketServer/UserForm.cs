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
    public partial class UserForm : Form
    {
        UserDB db;
        int userId = 0;

        public UserForm()
        {
            InitializeComponent();
            db = new UserDB();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            updateUserDB();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            updateUserDB();
        }

        private void updateUserDB()
        {
            string keyword = tbKeyword.Text;
            //string sql = "SELECT USER_ID, USER_GUID, USER_NM, FP_DATA FROM ICBM_USER WHERE USER_ID = :USER_ID";

            string sql = "SELECT USER_ID, USER_GUID, USER_NM, FP_DATA FROM ISPS_USER";
            if (keyword != null && keyword != String.Empty)
            {
                sql += " WHERE USER_NM LIKE '%" + keyword + "%'";
            }
            dataGridView1.DataSource = db.GetDBTable(sql);
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void tbKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                updateUserDB();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateStatusMessage(String msg)
        {
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = msg;
            });
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                userId = Int32.Parse(row.Cells[0].Value.ToString());
                UpdateStatusMessage("Selected row USER_ID: " + userId);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("사용자[" + userId + "]를 삭제하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (db.DeleteISPSUser(userId) > 0)
                {
                    UpdateStatusMessage("Success delete user: " + userId);
                }
                else
                {
                    UpdateStatusMessage("Filed delete user: " + userId);
                }
            }
        }

        private void btnEnroll_Click(object sender, EventArgs e)
        {
            UserDialog userDlg = new UserDialog(UserManager.MODE.SAVE);
            userDlg.ShowDialog();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            UserDialog userDlg = new UserDialog(UserManager.MODE.MODIFY, userId);
            userDlg.ShowDialog();
        }

        private void UserForm_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("Activated");
            updateUserDB();
        }

        private void UserForm_Enter(object sender, EventArgs e)
        {
            Console.WriteLine("Enter");
        }
    }
}
