using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static AsyncSocketServer.CarInfoManager;
using static AsyncSocketServer.UserManager;

namespace AsyncSocketServer
{
    public partial class ManagerDialog : Form
    {

        const int TAB_AVERAGE = 0;
        const int TAB_ACCESS = 1;
        const int TAB_USER = 2;
        const int TAB_CAR = 3;
        const int TAB_ACCESS_HIS = 4;

        int currentPage = 1;
        int pageTotal = 1;
        int total = 0;
        //int pageCount = 10;

        string[] selectCountCbDatas = { "ALL", "5", "10", "20", "30", "50" };

        UserDB m_userDB;
        AccessInfoDB m_accessDB;
        CarInfoDB m_carDB;
        AccessHisDB m_historyDB;

        AverageInfoManager m_avrMgr;

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

            m_avrMgr = new AverageInfoManager();

            m_user = new MyPerson();
            m_car = new CarInfo();

            toolStripCbCount.Items.AddRange(selectCountCbDatas);
            toolStripCbCount.SelectedIndex = 2;
            toolStripCbCount.DropDownStyle = ComboBoxStyle.DropDownList;

            dtpAvgDate.Format = DateTimePickerFormat.Custom;
            dtpAvgDate.CustomFormat = "yyyy년 MM월 dd일 (ddd)";

            InitDataGridViews();
            UpdateComponents();
        }

        private DataTable getUserDB()
        {
            string keyword = tbKeyword.Text;
            return m_userDB.GetUserDBTable(keyword);
        }

        private void updateAccessDB()
        {
            //dgvAccessInfo.DataSource = m_accessDB.GetAccessInfoDBTable(m_user.Id);
            dgvAccessInfo.DataSource = m_accessDB.GetAccessInfoDBTable(m_user.Id, currentPage, GetPageCount());
            dgvAccessInfo.Refresh();
            total = Int32.Parse(dgvAccessInfo.Rows[0].Cells["count"].Value.ToString());
        }

        private int GetPageCount()
        {
            int count = 0;
            try
            {
                count = Int32.Parse(toolStripCbCount.SelectedItem.ToString());
            }
            catch
            {
                count = 0;
            }
            return count;
        }

        private void UpdateComponents()
        {
            UpdateStatusMessage("");
            gbGroup.Text = tabControl1.SelectedTab.Text;

            switch (tabControl1.SelectedIndex)
            {
                case TAB_AVERAGE:
                    Console.WriteLine("TAB_AVERAGE");
                    EnableCRUDButton(false);
                    EnableSearchComponent(false);
                    toolStripPaging.Enabled = false;
                    UpdateAccessTotal();
                    UpdateChart();
                    break;
                case TAB_ACCESS:
                    Console.WriteLine("TAB_ACCESS");
                    lbKeyword.Text = "이름";
                    dgvAccessUser.DataSource = m_userDB.GetUserDBTable(tbKeyword.Text, currentPage, GetPageCount());
                    dgvAccessUser.Refresh();
                    EnableCRUDButton(true);
                    EnableSearchComponent(true);
                    toolStripPaging.Enabled = true;
                    break;
                case TAB_USER:
                    Console.WriteLine("TAB_USER");
                    lbKeyword.Text = "이름";
                    EnableCRUDButton(true);
                    EnableSearchComponent(true);
                    toolStripPaging.Enabled = true;
                    break;
                case TAB_CAR:
                    Console.WriteLine("TAB_CAR");
                    lbKeyword.Text = "번호";
                    EnableCRUDButton(true);
                    EnableSearchComponent(true);
                    toolStripPaging.Enabled = true;
                    break;
                case TAB_ACCESS_HIS:
                    Console.WriteLine("TAB_ACCESS_HIS");
                    lbKeyword.Text = "이름";
                    EnableCRUDButton(false);
                    EnableSearchComponent(true);
                    toolStripPaging.Enabled = true;
                    break;
            }

            // datagridview paging
            currentPage = 1;
            RebindGridForPageChange(tbKeyword.Text);
            RefreshPagination();
            toolStripLbTotal.Text = total.ToString();
        }

        private void InitDataGridViews()
        {
            /*
             * datagridview access user
             */
            dgvAccessUser.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn accessCol_id = new DataGridViewTextBoxColumn();
            accessCol_id.DataPropertyName = "user_id";
            accessCol_id.Name = "user_id";
            accessCol_id.HeaderText = "아이디";

            DataGridViewTextBoxColumn accessCol_guid = new DataGridViewTextBoxColumn();
            accessCol_guid.DataPropertyName = "user_guid";
            accessCol_guid.Name = "user_guid";
            accessCol_guid.HeaderText = "GUID";

            DataGridViewTextBoxColumn accessCol_name = new DataGridViewTextBoxColumn();
            accessCol_name.DataPropertyName = "user_nm";
            accessCol_name.Name = "user_nm";
            accessCol_name.HeaderText = "이름";

            DataGridViewTextBoxColumn accessCol_idNum = new DataGridViewTextBoxColumn();
            accessCol_idNum.DataPropertyName = "user_idnum";
            accessCol_idNum.Name = "user_idnum";
            accessCol_idNum.HeaderText = "주민번호";

            DataGridViewTextBoxColumn accessCol_phone = new DataGridViewTextBoxColumn();
            accessCol_phone.DataPropertyName = "phone";
            accessCol_phone.Name = "phone";
            accessCol_phone.HeaderText = "연락처";

            DataGridViewImageColumn accessCol_fp = new DataGridViewImageColumn();
            accessCol_fp.DataPropertyName = "fp_data";
            accessCol_fp.Name = "fp_data";
            accessCol_fp.HeaderText = "지문정보";

            DataGridViewTextBoxColumn accessCol_count = new DataGridViewTextBoxColumn();
            accessCol_count.DataPropertyName = "count";
            accessCol_count.Name = "count";
            accessCol_count.HeaderText = "전체";
            accessCol_count.Visible = false;

            dgvAccessUser.Columns.Add(accessCol_id);
            dgvAccessUser.Columns.Add(accessCol_guid);
            dgvAccessUser.Columns.Add(accessCol_name);
            dgvAccessUser.Columns.Add(accessCol_idNum);
            dgvAccessUser.Columns.Add(accessCol_phone);
            dgvAccessUser.Columns.Add(accessCol_fp);
            dgvAccessUser.Columns.Add(accessCol_count);

            foreach (DataGridViewColumn col in dgvAccessUser.Columns)
            {
                col.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Set the column size automatically
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvAccessUser.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOrange;
            dgvAccessUser.EnableHeadersVisualStyles = false;
            dgvAccessUser.AllowUserToAddRows = false;


            /*
             * datagridview user
             */
            dgvUser.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn userCol_id = new DataGridViewTextBoxColumn();
            userCol_id.DataPropertyName = "user_id";
            userCol_id.Name = "user_id";
            userCol_id.HeaderText = "아이디";

            DataGridViewTextBoxColumn userCol_guid = new DataGridViewTextBoxColumn();
            userCol_guid.DataPropertyName = "user_guid";
            userCol_guid.Name = "user_guid";
            userCol_guid.HeaderText = "GUID";

            DataGridViewTextBoxColumn userCol_name = new DataGridViewTextBoxColumn();
            userCol_name.DataPropertyName = "user_nm";
            userCol_name.Name = "user_nm";
            userCol_name.HeaderText = "이름";

            DataGridViewTextBoxColumn userCol_idNum = new DataGridViewTextBoxColumn();
            userCol_idNum.DataPropertyName = "user_idnum";
            userCol_idNum.Name = "user_idnum";
            userCol_idNum.HeaderText = "주민번호";

            DataGridViewTextBoxColumn userCol_phone = new DataGridViewTextBoxColumn();
            userCol_phone.DataPropertyName = "phone";
            userCol_phone.Name = "phone";
            userCol_phone.HeaderText = "연락처";

            DataGridViewImageColumn userCol_fp = new DataGridViewImageColumn();
            userCol_fp.DataPropertyName = "fp_data";
            userCol_fp.Name = "fp_data";
            userCol_fp.HeaderText = "지문정보";

            DataGridViewTextBoxColumn userCol_count = new DataGridViewTextBoxColumn();
            userCol_count.DataPropertyName = "count";
            userCol_count.Name = "count";
            userCol_count.HeaderText = "전체";
            userCol_count.Visible = false;

            dgvUser.Columns.Add(userCol_id);
            dgvUser.Columns.Add(userCol_guid);
            dgvUser.Columns.Add(userCol_name);
            dgvUser.Columns.Add(userCol_idNum);
            dgvUser.Columns.Add(userCol_phone);
            dgvUser.Columns.Add(userCol_fp);
            dgvUser.Columns.Add(userCol_count);

            foreach (DataGridViewColumn col in dgvUser.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                // Set the column size automatically
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvUser.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOrange;
            dgvUser.EnableHeadersVisualStyles = false;
            dgvUser.AllowUserToAddRows = false;


            /*
             * datagridview car
             */
            dgvCar.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn carCol_carId = new DataGridViewTextBoxColumn();
            carCol_carId.DataPropertyName = "car_id";
            carCol_carId.Name = "car_id";
            carCol_carId.HeaderText = "차량번호";

            DataGridViewTextBoxColumn carCol_carOwner = new DataGridViewTextBoxColumn();
            carCol_carOwner.DataPropertyName = "car_owner";
            carCol_carOwner.Name = "car_owner";
            carCol_carOwner.HeaderText = "차량소유자";

            DataGridViewTextBoxColumn carCol_regDt = new DataGridViewTextBoxColumn();
            carCol_regDt.DataPropertyName = "reg_dt";
            carCol_carOwner.Name = "reg_dt";
            carCol_regDt.HeaderText = "등록일";
            carCol_regDt.DefaultCellStyle.Format = "yyyy년 MM월 dd일 HH시 mm분";

            DataGridViewTextBoxColumn carCol_count = new DataGridViewTextBoxColumn();
            carCol_count.DataPropertyName = "count";
            carCol_count.Name = "count";
            carCol_count.HeaderText = "전체";
            carCol_count.Visible = false;

            dgvCar.Columns.Add(carCol_carId);
            dgvCar.Columns.Add(carCol_carOwner);
            dgvCar.Columns.Add(carCol_regDt);
            dgvCar.Columns.Add(carCol_count);

            foreach (DataGridViewColumn col in dgvCar.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                // Set the column size automatically
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvCar.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOrange;
            dgvCar.EnableHeadersVisualStyles = false;
            dgvCar.AllowUserToAddRows = false;

            /*
             * datagridview history
             */
            dgvHistory.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn hisCol_regDt = new DataGridViewTextBoxColumn();
            hisCol_regDt.DataPropertyName = "reg_dt";
            hisCol_regDt.Name = "reg_dt";
            hisCol_regDt.HeaderText = "일자";
            hisCol_regDt.DefaultCellStyle.Format = "yyyy년 MM월 dd일 HH시 mm분 ss초";

            DataGridViewTextBoxColumn hisCol_rtCode = new DataGridViewTextBoxColumn();
            hisCol_rtCode.DataPropertyName = "rt_code";
            hisCol_rtCode.Name = "rt_code";
            hisCol_rtCode.HeaderText = "처리결과";

            DataGridViewTextBoxColumn hisCol_userId = new DataGridViewTextBoxColumn();
            hisCol_userId.DataPropertyName = "user_id";
            hisCol_userId.Name = "user_id";
            hisCol_userId.HeaderText = "아이디";

            DataGridViewTextBoxColumn hisCol_userNm = new DataGridViewTextBoxColumn();
            hisCol_userNm.DataPropertyName = "user_nm";
            hisCol_userNm.Name = "user_nm";
            hisCol_userNm.HeaderText = "이름";

            DataGridViewTextBoxColumn hisCol_ip = new DataGridViewTextBoxColumn();
            hisCol_ip.DataPropertyName = "ip";
            hisCol_ip.Name = "ip";
            hisCol_ip.HeaderText = "아이피";

            DataGridViewTextBoxColumn hisCol_count = new DataGridViewTextBoxColumn();
            hisCol_count.DataPropertyName = "count";
            hisCol_count.Name = "count";
            hisCol_count.HeaderText = "전체";
            hisCol_count.Visible = false;

            dgvHistory.Columns.Add(hisCol_regDt);
            dgvHistory.Columns.Add(hisCol_rtCode);
            dgvHistory.Columns.Add(hisCol_userId);
            dgvHistory.Columns.Add(hisCol_userNm);
            dgvHistory.Columns.Add(hisCol_ip);
            dgvHistory.Columns.Add(hisCol_count);

            foreach (DataGridViewColumn col in dgvHistory.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                // Set the column size automatically
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOrange;
            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.AllowUserToAddRows = false;


            /*
             * datagridview average history
             */
            dgvAvgAccessHis.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn avrHisCol_regDt = new DataGridViewTextBoxColumn();
            avrHisCol_regDt.DataPropertyName = "access_dt";
            avrHisCol_regDt.Name = "access_dt";
            avrHisCol_regDt.HeaderText = "출입일시";
            avrHisCol_regDt.DefaultCellStyle.Format = "yyyy년 MM월 dd일 HH시 mm분 ss초";

            DataGridViewTextBoxColumn avrHisCol_userNm = new DataGridViewTextBoxColumn();
            avrHisCol_userNm.DataPropertyName = "user_nm";
            avrHisCol_userNm.Name = "user_nm";
            avrHisCol_userNm.HeaderText = "출입자";

            DataGridViewTextBoxColumn avrHisCol_car = new DataGridViewTextBoxColumn();
            avrHisCol_car.DataPropertyName = "car_id";
            avrHisCol_car.Name = "car_id";
            avrHisCol_car.HeaderText = "차량번호";

            dgvAvgAccessHis.Columns.Add(avrHisCol_regDt);
            dgvAvgAccessHis.Columns.Add(avrHisCol_userNm);
            dgvAvgAccessHis.Columns.Add(avrHisCol_car);

            foreach (DataGridViewColumn col in dgvAvgAccessHis.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                // Set the column size automatically
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvAvgAccessHis.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOrange;
            dgvAvgAccessHis.EnableHeadersVisualStyles = false;
            dgvAvgAccessHis.AllowUserToAddRows = false;

            /*
             * datagridview access info
             */
            dgvAccessInfo.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn accessCol_seq = new DataGridViewTextBoxColumn();
            accessCol_seq.DataPropertyName = "access_info_sq";
            accessCol_seq.Name = "access_info_sq";
            accessCol_seq.HeaderText = "SEQ";

            DataGridViewTextBoxColumn accessCol_psgCnt = new DataGridViewTextBoxColumn();
            accessCol_psgCnt.DataPropertyName = "psg_cnt";
            accessCol_psgCnt.Name = "psg_cnt";
            accessCol_psgCnt.HeaderText = "동승자수";

            DataGridViewTextBoxColumn accessCol_startDt = new DataGridViewTextBoxColumn();
            accessCol_startDt.DataPropertyName = "allow_start_dt";
            accessCol_startDt.Name = "allow_start_dt";
            accessCol_startDt.HeaderText = "출입시작일시";
            accessCol_startDt.DefaultCellStyle.Format = "yyyy년 MM월 dd일 HH시 mm분";

            DataGridViewTextBoxColumn accessCol_endDt = new DataGridViewTextBoxColumn();
            accessCol_endDt.DataPropertyName = "allow_end_dt";
            accessCol_endDt.Name = "allow_end_dt";
            accessCol_endDt.HeaderText = "출입종료일시";
            accessCol_endDt.DefaultCellStyle.Format = "yyyy년 MM월 dd일 HH시 mm분";

            DataGridViewTextBoxColumn accessCol_purpose = new DataGridViewTextBoxColumn();
            accessCol_purpose.DataPropertyName = "purpose";
            accessCol_purpose.Name = "purpose";
            accessCol_purpose.HeaderText = "출입목적";

            DataGridViewTextBoxColumn accessCol_accessDt = new DataGridViewTextBoxColumn();
            accessCol_accessDt.DataPropertyName = "access_dt";
            accessCol_accessDt.Name = "access_dt";
            accessCol_accessDt.HeaderText = "출입시간";
            accessCol_accessDt.DefaultCellStyle.Format = "yyyy년 MM월 dd일 HH시 mm분 ss초";

            DataGridViewTextBoxColumn accessCol_count2 = new DataGridViewTextBoxColumn();
            accessCol_count2.DataPropertyName = "count";
            accessCol_count2.Name = "count";
            accessCol_count2.HeaderText = "전체";
            accessCol_count2.Visible = false;

            dgvAccessInfo.Columns.Add(accessCol_seq);
            dgvAccessInfo.Columns.Add(accessCol_psgCnt);
            dgvAccessInfo.Columns.Add(accessCol_startDt);
            dgvAccessInfo.Columns.Add(accessCol_endDt);
            dgvAccessInfo.Columns.Add(accessCol_purpose);
            dgvAccessInfo.Columns.Add(accessCol_accessDt);
            dgvAccessInfo.Columns.Add(accessCol_count2);

            foreach (DataGridViewColumn col in dgvAccessInfo.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                // Set the column size automatically
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvAccessInfo.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOrange;
            dgvAccessInfo.EnableHeadersVisualStyles = false;
            dgvAccessInfo.AllowUserToAddRows = false;
        }

        private void EnableCRUDButton(bool enabled)
        {
            btnDelete.Enabled = enabled;
            btnEnroll.Enabled = enabled;
            btnModify.Enabled = enabled;
        }

        private void EnableSearchComponent(bool enabled)
        {
            tbKeyword.Enabled = enabled;
            btnSearch.Enabled = enabled;
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

        private void dgvAccessUser_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvAccessUser.SelectedRows)
            {
                m_user.Id = Int32.Parse(row.Cells["user_id"].Value.ToString());
                m_user.Name = row.Cells["user_nm"].Value.ToString();
                UpdateStatusMessage("Selected row: USER_ID[" + m_user.Id + "]");

                //updateAccessDB();
                RebindGridForPageChange("");
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvAccessInfo.SelectedRows)
            {
                m_accessSeq = Int32.Parse(row.Cells["access_info_sq"].Value.ToString());
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

        private void ToolStripButtonClick(object sender, EventArgs e)
        {
            try
            {
                ToolStripButton ToolStripButton = ((ToolStripButton)sender);

                //Determining the current page
                if (ToolStripButton == btnBackward)
                    currentPage--;
                else if (ToolStripButton == btnForward)
                    currentPage++;
                else if (ToolStripButton == btnLast)
                    currentPage = pageTotal;
                else if (ToolStripButton == btnFirst)
                    currentPage = 1;
                else
                    currentPage = Convert.ToInt32(ToolStripButton.Text, CultureInfo.InvariantCulture);

                if (currentPage < 1)
                    currentPage = 1;
                else if (currentPage > pageTotal)
                    currentPage = pageTotal;

                //Rebind the Datagridview with the data.
                RebindGridForPageChange(tbKeyword.Text.ToString());

                //Change the pagiantions buttons according to page number
                RefreshPagination();
            }
            catch (Exception) { }
        }

        private void RefreshPagination()
        {
            ToolStripButton[] items = new ToolStripButton[] { toolStripButton1, toolStripButton2, toolStripButton3, toolStripButton4, toolStripButton5 };

            //pageStartIndex contains the first button number of pagination.
            int pageStartIndex = 1;

            if (pageTotal > 5 && currentPage > 2)
                pageStartIndex = currentPage - 2;

            if (pageTotal > 5 && currentPage > pageTotal - 2)
                pageStartIndex = pageTotal - 4;

            for (int i = pageStartIndex; i < pageStartIndex + 5; i++)
            {
                if (i > pageTotal)
                {
                    items[i - pageStartIndex].Visible = false;
                }
                else
                {
                    //Changing the page numbers
                    items[i - pageStartIndex].Text = i.ToString(CultureInfo.InvariantCulture);
                    items[i - pageStartIndex].Visible = true;

                    //Setting the Appearance of the page number buttons
                    if (i == currentPage)
                    {
                        items[i - pageStartIndex].BackColor = Color.Black;
                        items[i - pageStartIndex].ForeColor = Color.White;
                    }
                    else
                    {
                        items[i - pageStartIndex].BackColor = Color.White;
                        items[i - pageStartIndex].ForeColor = Color.Black;
                    }
                }
            }

            //Enabling or Disalbing pagination first, last, previous , next buttons
            if (currentPage == 1)
                btnBackward.Enabled = btnFirst.Enabled = false;
            else
                btnBackward.Enabled = btnFirst.Enabled = true;

            if (currentPage == pageTotal)
                btnForward.Enabled = btnLast.Enabled = false;

            else
                btnForward.Enabled = btnLast.Enabled = true;
        }

        //private void RefreshPagination2()
        //{
        //    ToolStripButton[] items = new ToolStripButton[] { toolStripButton8, toolStripButton9, toolStripButton10, toolStripButton11, toolStripButton12 };

        //    //pageStartIndex contains the first button number of pagination.
        //    int pageStartIndex = 1;

        //    if (pageTotal > 5 && currentPage > 2)
        //        pageStartIndex = currentPage - 2;

        //    if (pageTotal > 5 && currentPage > pageTotal - 2)
        //        pageStartIndex = pageTotal - 4;

        //    for (int i = pageStartIndex; i < pageStartIndex + 5; i++)
        //    {
        //        if (i > pageTotal)
        //        {
        //            items[i - pageStartIndex].Visible = false;
        //        }
        //        else
        //        {
        //            //Changing the page numbers
        //            items[i - pageStartIndex].Text = i.ToString(CultureInfo.InvariantCulture);
        //            items[i - pageStartIndex].Visible = true;

        //            //Setting the Appearance of the page number buttons
        //            if (i == currentPage)
        //            {
        //                items[i - pageStartIndex].BackColor = Color.Black;
        //                items[i - pageStartIndex].ForeColor = Color.White;
        //            }
        //            else
        //            {
        //                items[i - pageStartIndex].BackColor = Color.White;
        //                items[i - pageStartIndex].ForeColor = Color.Black;
        //            }
        //        }
        //    }

        //    //Enabling or Disalbing pagination first, last, previous , next buttons
        //    if (currentPage == 1)
        //        btnBackward.Enabled = btnFirst.Enabled = false;
        //    else
        //        btnBackward.Enabled = btnFirst.Enabled = true;

        //    if (currentPage == pageTotal)
        //        btnForward.Enabled = btnLast.Enabled = false;

        //    else
        //        btnForward.Enabled = btnLast.Enabled = true;
        //}

        private void RebindGridForPageChange(string keyword)
        {
            total = 0;
            try
            {
                switch (tabControl1.SelectedIndex)
                {
                    case TAB_AVERAGE:
                        dgvAvgAccessHis.DataSource = m_avrMgr.SelectAccessHistoryInfo(dtpAvgDate.Value);
                        dgvAvgAccessHis.Refresh();
                        break;
                    case TAB_ACCESS:
                        //dt = m_userDB.GetUserDBTable(keyword, currentPage, GetPageCount());
                        //dgvAccessUser.DataSource = dt;
                        //dgvAccessUser.Refresh();
                        //total = Int32.Parse(dgvAccessUser.Rows[0].Cells["count"].Value.ToString());
                        dgvAccessInfo.DataSource = m_accessDB.GetAccessInfoDBTable(m_user.Id, currentPage, GetPageCount());
                        dgvAccessInfo.Refresh();
                        total = Int32.Parse(dgvAccessInfo.Rows[0].Cells["count"].Value.ToString());
                        break;
                    case TAB_USER:
                        dgvUser.DataSource = m_userDB.GetUserDBTable(keyword, currentPage, GetPageCount());
                        dgvUser.Refresh();
                        total = Int32.Parse(dgvUser.Rows[0].Cells["count"].Value.ToString());
                        break;
                    case TAB_CAR:
                        dgvCar.DataSource = m_carDB.GetCarInfoDBTable(keyword, currentPage, GetPageCount());
                        dgvCar.Refresh();
                        total = Int32.Parse(dgvCar.Rows[0].Cells["count"].Value.ToString());
                        break;
                    case TAB_ACCESS_HIS:
                        dgvHistory.DataSource = m_historyDB.GetAccessHisDBTable(keyword, currentPage, GetPageCount());
                        dgvHistory.Refresh();
                        total = Int32.Parse(dgvHistory.Rows[0].Cells["count"].Value.ToString());
                        break;
                }
                pageTotal = Convert.ToInt32(Math.Ceiling(total * 1.0 / ((GetPageCount()) < 1 ? 1 : GetPageCount())));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void UpdateAccessTotal()
        {
            List<Dictionary<string, object>> result = m_avrMgr.SelectAccessTotalInfo(dtpAvgDate.Value);
            long req = 0;
            long ok = 0;
            long no = 0;
            if(result != null)
            {
                req = (long)result[0]["req_total"];
                ok = (long)result[0]["access_cnt"];
                no = (long)result[0]["not_access_cnt"];
            }
            dataAccessReqTotal.Text = req.ToString();
            dataAccessTotal.Text = ok.ToString();
            dataNotAccessTotal.Text = no.ToString();
        }

        private void UpdateChart()
        {
            chart1.Series.Clear();
            Series accessUserCnt = chart1.Series.Add("accessUserCnt");
            accessUserCnt.ChartType = SeriesChartType.Bar;
        }

        private void dtpAvgDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateComponents();
        }
    }
}
