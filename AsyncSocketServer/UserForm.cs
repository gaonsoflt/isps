using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AsyncSocketServer.UserManager;

namespace AsyncSocketServer
{
    public partial class UserForm : Form
    {
        UserDB m_db;
        MyPerson m_user;
        //int m_userId = 0;
        int m_accessSeq = 0;

        public UserForm()
        {
            InitializeComponent();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            m_db = new UserDB();
            m_user = new MyPerson();
            //updateUserDB();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            updateUserDB();
        }

        private void updateUserDB()
        {
            string keyword = tbKeyword.Text;
            dataGridView1.DataSource = m_db.GetUserDBTable(keyword);
            if (dataGridView1.DataSource != null)
            {
                dataGridView1.Columns["user_id"].HeaderText = "아이디";
                dataGridView1.Columns["user_guid"].HeaderText = "GUID";
                dataGridView1.Columns["user_nm"].HeaderText = "이름";
                dataGridView1.Columns["user_idnum"].HeaderText = "주민번호";
                dataGridView1.Columns["phone"].HeaderText = "연락처";
                dataGridView1.Columns["fp_data"].HeaderText = "지문정보";
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.Columns["user_id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns["user_guid"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns["user_nm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns["user_idnum"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns["phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                dataGridView1.AllowUserToAddRows = false;
            }
        }

        private void updateAccessDB()
        {
            dataGridView2.DataSource = m_db.GetAccessDBTable(m_user.Id);
            if (dataGridView2.DataSource != null)
            {
                dataGridView2.Columns["access_info_sq"].HeaderText = "SEQ";
                dataGridView2.Columns["psg_cnt"].HeaderText = "동승자수";
                dataGridView2.Columns["allow_start_dt"].HeaderText = "출입시작일시";
                dataGridView2.Columns["allow_end_dt"].HeaderText = "출입종료일시";
                dataGridView2.Columns["is_access"].HeaderText = "출입여부";
                dataGridView2.Columns["access_dt"].HeaderText = "출입시간";
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView2.Columns["access_info_sq"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView2.Columns["psg_cnt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView2.Columns["is_access"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                for (int i = 0; i < dataGridView2.Columns.Count; i++)
                {
                    dataGridView2.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                dataGridView2.AllowUserToAddRows = false;
            }
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
                m_user.Id = Int32.Parse(row.Cells["user_id"].Value.ToString());
                m_user.Name = row.Cells["user_nm"].Value.ToString();
                UpdateStatusMessage("Selected row: USER_ID[" + m_user.Id + "]");
                updateAccessDB();
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView2.SelectedRows)
            {
                m_accessSeq = Int32.Parse(row.Cells[0].Value.ToString());
                UpdateStatusMessage("Selected row: USER_ID[" + m_user.Id + "] ACCESS_INFO_SQ[" + m_accessSeq + "]");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("사용자[" + m_user.Id + "]를 삭제하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (m_db.DeleteISPSUser(m_user.Id) > 0)
                {
                    UpdateStatusMessage("Success delete user: " + m_user.Id);
                    updateUserDB();
                }
                else
                {
                    UpdateStatusMessage("Filed delete user: " + m_user.Id);
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
            UserDialog userDlg = new UserDialog(UserManager.MODE.MODIFY, m_user.Id);
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

        private void btnAccessEnroll_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                AccessInfoManager.AccessInfo accessInfo = new AccessInfoManager.AccessInfo();
                accessInfo.seq = m_accessSeq;
                accessInfo.user = m_user;
                AccessDialog accessDlg = new AccessDialog(AccessInfoManager.DIALOG_MODE.SAVE, accessInfo);
                accessDlg.ShowDialog();
            }
            else
            {
                MessageBox.Show("유저 정보가 없습니다.", "알림", MessageBoxButtons.OK);
            }
        }

        private void btnAccessUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView2.RowCount > 0) {
                AccessInfoManager.AccessInfo accessInfo = new AccessInfoManager.AccessInfo();
                accessInfo.seq = m_accessSeq;
                accessInfo.user = m_user;
                AccessDialog accessDlg = new AccessDialog(AccessInfoManager.DIALOG_MODE.MODIFY, accessInfo);
                accessDlg.ShowDialog();
            }
            else
            {
                MessageBox.Show("출입 정보가 없습니다.", "알림", MessageBoxButtons.OK);
            }
        }
    }
}
