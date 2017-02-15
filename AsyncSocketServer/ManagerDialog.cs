using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AsyncSocketServer.CarInfoManager;
using static AsyncSocketServer.UserManager;

namespace AsyncSocketServer
{
    public partial class ManagerDialog : Form
    {

        const int TAB_ACCESS = 0;
        const int TAB_USER = 1;
        const int TAB_CAR = 2;
        const int TAB_ACCESS_HIS = 3;

        UserDB m_userDB;
        AccessInfoDB m_accessDB;
        CarInfoDB m_carDB;
        AccessHisDB m_historyDB;

        MyPerson m_user;
        CarInfo m_car;

        //int m_userId = 0;
        int m_accessSeq = 0;

        public ManagerDialog()
        {
            InitializeComponent();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            m_userDB = new UserDB();
            m_accessDB = new AccessInfoDB();
            m_carDB = new CarInfoDB();
            m_historyDB = new AccessHisDB();

            m_user = new MyPerson();
            m_car = new CarInfo();
            UpdateComponents();
        }

        private DataTable getUserDB()
        {
            string keyword = tbKeyword.Text;
            return m_userDB.GetUserDBTable(keyword);
        }

        private void updateAccessDB()
        {
            dgvAccessInfo.DataSource = m_accessDB.GetAccessInfoDBTable(m_user.Id);
            dgvAccessInfo.Columns["access_info_sq"].HeaderText = "SEQ";
            dgvAccessInfo.Columns["psg_cnt"].HeaderText = "동승자수";
            dgvAccessInfo.Columns["allow_start_dt"].HeaderText = "출입시작일시";
            dgvAccessInfo.Columns["allow_end_dt"].HeaderText = "출입종료일시";
            dgvAccessInfo.Columns["purpose"].HeaderText = "출입목적";
            dgvAccessInfo.Columns["access_dt"].HeaderText = "출입시간";
            dgvAccessInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAccessInfo.Columns["access_info_sq"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvAccessInfo.Columns["psg_cnt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvAccessInfo.Columns["purpose"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            for (int i = 0; i < dgvAccessInfo.Columns.Count; i++)
            {
                dgvAccessInfo.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dgvAccessInfo.AllowUserToAddRows = false;
        }

        private void UpdateComponents()
        {
            string keyword = tbKeyword.Text;
            gbGroup.Text = tabControl1.SelectedTab.Text;
            UpdateStatusMessage("");

            switch (tabControl1.SelectedIndex)
            {
                case TAB_ACCESS:
                    Console.WriteLine("TAB_ACCESS");
                    lbKeyword.Text = "이름";
                    dgvAccessUser.DataSource = m_userDB.GetUserDBTable(keyword);
                    dgvAccessUser.Columns["user_id"].HeaderText = "아이디";
                    dgvAccessUser.Columns["user_guid"].HeaderText = "GUID";
                    dgvAccessUser.Columns["user_nm"].HeaderText = "이름";
                    dgvAccessUser.Columns["user_idnum"].HeaderText = "주민번호";
                    dgvAccessUser.Columns["phone"].HeaderText = "연락처";
                    dgvAccessUser.Columns["fp_data"].HeaderText = "지문정보";
                    dgvAccessUser.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvAccessUser.Columns["user_id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvAccessUser.Columns["user_guid"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvAccessUser.Columns["user_nm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvAccessUser.Columns["user_idnum"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvAccessUser.Columns["phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    for (int i = 0; i < dgvAccessUser.Columns.Count; i++)
                    {
                        dgvAccessUser.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    dgvAccessUser.AllowUserToAddRows = false;
                    EnableButton(true);
                    break;
                case TAB_USER:
                    Console.WriteLine("TAB_USER");
                    lbKeyword.Text = "이름";
                    dgvUser.DataSource = m_userDB.GetUserDBTable(keyword);
                    dgvUser.Columns["user_id"].HeaderText = "아이디";
                    dgvUser.Columns["user_guid"].HeaderText = "GUID";
                    dgvUser.Columns["user_nm"].HeaderText = "이름";
                    dgvUser.Columns["user_idnum"].HeaderText = "주민번호";
                    dgvUser.Columns["phone"].HeaderText = "연락처";
                    dgvUser.Columns["fp_data"].HeaderText = "지문정보";
                    dgvUser.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvUser.Columns["user_id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvUser.Columns["user_guid"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvUser.Columns["user_nm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvUser.Columns["user_idnum"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvUser.Columns["phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    for (int i = 0; i < dgvUser.Columns.Count; i++)
                    {
                        dgvUser.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    dgvUser.AllowUserToAddRows = false;
                    EnableButton(true);
                    break;
                case TAB_CAR:
                    Console.WriteLine("TAB_CAR");
                    lbKeyword.Text = "번호";
                    dgvCar.DataSource = m_carDB.GetCarInfoDBTable(keyword);
                    dgvCar.Columns["car_id"].HeaderText = "차량번호";
                    dgvCar.Columns["car_owner"].HeaderText = "차량소유자";
                    dgvCar.Columns["reg_dt"].HeaderText = "등록일";
                    dgvCar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvCar.Columns["car_id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvCar.Columns["car_owner"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    for (int i = 0; i < dgvCar.Columns.Count; i++)
                    {
                        dgvCar.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    dgvCar.AllowUserToAddRows = false;
                    EnableButton(true);
                    break;
                case TAB_ACCESS_HIS:
                    Console.WriteLine("TAB_ACCESS_HIS");
                    lbKeyword.Text = "이름";
                    dgvHistory.DataSource = m_historyDB.GetAccessHisDBTable(keyword);
                    dgvHistory.Columns["reg_dt"].HeaderText = "일자";
                    dgvHistory.Columns["rt_code"].HeaderText = "결과";
                    dgvHistory.Columns["user_id"].HeaderText = "아이디";
                    dgvHistory.Columns["user_nm"].HeaderText = "이름";
                    dgvHistory.Columns["ip"].HeaderText = "아이피";
                    dgvHistory.Columns["reg_dt"].DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss";
                    dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvHistory.Columns["reg_dt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvHistory.Columns["rt_code"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvHistory.Columns["user_id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgvHistory.Columns["user_nm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    for (int i = 0; i < dgvHistory.Columns.Count; i++)
                    {
                        dgvHistory.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    dgvHistory.AllowUserToAddRows = false;
                    EnableButton(false);
                    break;
            }
        }

        private void EnableButton(bool enabled)
        {
            btnDelete.Enabled = enabled;
            btnEnroll.Enabled = enabled;
            btnModify.Enabled = enabled;
        }

        private void tbKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateComponents();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            UpdateComponents();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case TAB_ACCESS:
                    DeleteAccessInfo();
                    break;
                case TAB_USER:
                    DeleteUser();
                    break;
                case TAB_CAR:
                    DeleteCar();
                    break;
            }
        }

        private void btnEnroll_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case TAB_ACCESS:
                    CreateAccessInfo();
                    break;
                case TAB_USER:
                    CreateUser();
                    break;
                case TAB_CAR:
                    CreateCar();
                    break;
            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case TAB_ACCESS:
                    UpdateAccessInfo();
                    break;
                case TAB_USER:
                    UpdateUser();
                    break;
                case TAB_CAR:
                    UpdateCar();
                    break;
            }
        }

        private void CreateAccessInfo()
        {
            if (dgvAccessUser.RowCount > 0)
            {
                AccessInfoManager.AccessInfo accessInfo = new AccessInfoManager.AccessInfo();
                accessInfo.user = m_user;
                AccessDialog dlg = new AccessDialog(AccessInfoManager.DIALOG_MODE.SAVE, accessInfo);
                UpdateDialogResult(dlg.ShowDialog());
            }
            else
            {
                MessageBox.Show("유저 정보가 없습니다.", "알림", MessageBoxButtons.OK);
            }
        }

        private void UpdateAccessInfo()
        {
            if (dgvAccessInfo.RowCount > 0)
            {
                AccessInfoManager.AccessInfo accessInfo = new AccessInfoManager.AccessInfo();
                accessInfo.seq = m_accessSeq;
                accessInfo.user = m_user;
                AccessDialog dlg = new AccessDialog(AccessInfoManager.DIALOG_MODE.MODIFY, accessInfo);
                UpdateDialogResult(dlg.ShowDialog());
            }
            else
            {
                MessageBox.Show("출입 정보가 없습니다.", "알림", MessageBoxButtons.OK);
            }
        }

        private void DeleteAccessInfo()
        {
            if (dgvAccessInfo.RowCount > 0)
            {
                // accessInfo not delete
            }
            else
            {
                MessageBox.Show("출입 정보가 없습니다.", "알림", MessageBoxButtons.OK);
            }
        }

        private void CreateUser()
        {
            UserDialog dlg = new UserDialog(UserManager.MODE.SAVE);
            UpdateDialogResult(dlg.ShowDialog());
        }

        private void UpdateUser()
        {
            if (dgvUser.RowCount > 0)
            {
                UserDialog dlg = new UserDialog(UserManager.MODE.MODIFY, m_user.Id);
                UpdateDialogResult(dlg.ShowDialog());
            }
            else
            {
                MessageBox.Show("사용자 정보가 없습니다.", "알림", MessageBoxButtons.OK);
            }

        }

        private void DeleteUser()
        {
            if (dgvUser.RowCount > 0)
            {
                if (MessageBox.Show("사용자[" + m_user.Id + "]를 삭제하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (m_userDB.DeleteISPSUser(m_user.Id) > 0)
                    {
                        UpdateStatusMessage("Success delete user: " + m_user.Id);
                        UpdateComponents();
                    }
                    else
                    {
                        UpdateStatusMessage("Failed delete user: " + m_user.Id);
                    }
                }
            }
            else
            {
                MessageBox.Show("사용자 정보가 없습니다.", "알림", MessageBoxButtons.OK);
            }
        }

        private void CreateCar()
        {
            CarInfoDialog dlg = new CarInfoDialog(CarInfoManager.DIALOG_MODE.SAVE);
            UpdateDialogResult(dlg.ShowDialog());
        }

        private void UpdateCar()
        {
            if (dgvCar.RowCount > 0)
            {
                CarInfoDialog dlg = new CarInfoDialog(CarInfoManager.DIALOG_MODE.MODIFY, m_car.id);
                UpdateDialogResult(dlg.ShowDialog());
            }
            else
            {
                MessageBox.Show("차량 정보가 없습니다.", "알림", MessageBoxButtons.OK);
            }
        }

        private void DeleteCar()
        {
            if (dgvCar.RowCount > 0)
            {
                if (MessageBox.Show("차량정보[" + m_car.id + "]를 삭제하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (m_carDB.DeleteCarInfo(m_car.id) > 0)
                    {
                        UpdateStatusMessage("Success delete car: " + m_car.id);
                        UpdateComponents();
                    }
                    else
                    {
                        UpdateStatusMessage("Failed delete car: " + m_car.id);
                    }
                }
            }
            else
            {
                MessageBox.Show("차량 정보가 없습니다.", "알림", MessageBoxButtons.OK);
            }
            
        }

        private void UpdateDialogResult(DialogResult dlgRt)
        {
            if (dlgRt == DialogResult.OK)
            {
                UpdateComponents();
                UpdateStatusMessage("정상적으로 처리 되었습니다.");
            }
            else
            {
                UpdateStatusMessage("취소되었습니다.");
            }
        }

        private void UserForm_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("Activated");
        }

        private void UserForm_Enter(object sender, EventArgs e)
        {
            Console.WriteLine("Enter");
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbKeyword.Text = "";
            UpdateComponents();
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
            foreach (DataGridViewRow row in dgvAccessUser.SelectedRows)
            {
                m_user.Id = Int32.Parse(row.Cells["user_id"].Value.ToString());
                m_user.Name = row.Cells["user_nm"].Value.ToString();
                UpdateStatusMessage("Selected row: USER_ID[" + m_user.Id + "]");

                updateAccessDB();
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvAccessInfo.SelectedRows)
            {
                m_accessSeq = Int32.Parse(row.Cells[0].Value.ToString());
                UpdateStatusMessage("Selected row: USER_ID[" + m_user.Id + "] ACCESS_INFO_SQ[" + m_accessSeq + "]");
            }
        }

        private void dgvUser_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvUser.SelectedRows)
            {
                m_user.Id = Int32.Parse(row.Cells["user_id"].Value.ToString());
                m_user.Name = row.Cells["user_nm"].Value.ToString();
                UpdateStatusMessage("Selected row: USER_ID[" + m_user.Id + "]");
            }
        }

        private void dgvCar_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvCar.SelectedRows)
            {
                m_car.id = row.Cells["car_id"].Value.ToString();
                UpdateStatusMessage("Selected row: CAR_ID[" + m_car.id + "]");
            }
        }
    }
}
