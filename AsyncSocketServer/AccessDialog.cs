using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AsyncSocketServer.AccessInfoManager;

namespace AsyncSocketServer
{
    public partial class AccessDialog : Form
    {
        public AccessInfo accessInfo;
        AccessInfoManager accessMgr;
        CarInfoManager carMgr;
        DIALOG_MODE mode;
        const string customDateFormat = "yyyy년 MM월 dd일 (ddd) HH시 mm분";
        TimeSpan minAccessDt;
        DateTime oldStartDt;
        DateTime oldEndDt;
        DataTable carId;

        public AccessDialog(DIALOG_MODE mode, AccessInfo accessInfo)
        {
            InitializeComponent();
            this.mode = mode;
            this.accessInfo = accessInfo;
        }

        private void AccessDialog_Load(object sender, EventArgs e)
        {
            accessMgr = new AccessInfoManager();
            carMgr = new CarInfoManager();
            minAccessDt = new TimeSpan(1, 0, 0);
            allowStartDt.Format = DateTimePickerFormat.Custom;
            allowStartDt.CustomFormat = customDateFormat;
            allowEndDt.Format = DateTimePickerFormat.Custom;
            allowEndDt.CustomFormat = customDateFormat;

            carId = carMgr.GetCarIdDBTable();
            cbCarId.DataSource = carId;
            cbCarId.DisplayMember = "car_id";
            cbCarId.ValueMember = "car_id";
            cbCarId.SelectedIndex = -1;

            InitComponents();
        }

        private void LoadAccessInfoDB()
        {
            //accessInfo = new AccessInfoDB().SelectAccessInfo(accessInfo.seq);
            accessInfo = accessMgr.SelectAccessInfoWithOrder(accessInfo.seq);
        }

        private void InitComponents()
        {
            switch (mode)
            {
                case DIALOG_MODE.SAVE:
                    this.Text = "등록";
                    if (accessInfo != null)
                    {
                        tbUserId.Text = accessInfo.user.Id.ToString();
                        tbUserNm.Text = accessInfo.user.Name.ToString();
                        DateTime dt = DateTime.Now;
                        allowEndDt.Value = dt + minAccessDt;
                        allowStartDt.Value = dt;
                    }
                    break;
                case DIALOG_MODE.MODIFY:
                    this.Text = "수정";
                    LoadAccessInfoDB();
                    if (accessInfo != null)
                    {
                        tbSeq.Text = accessInfo.seq.ToString();
                        tbUserId.Text = accessInfo.user.Id.ToString();
                        tbUserNm.Text = accessInfo.user.Name.ToString();
                        CheckAccessDt(accessInfo.access_dt);
                        nudPsgCnt.Value = accessInfo.psgCnt;
                        allowEndDt.Value = accessInfo.allowEndDt;
                        allowStartDt.Value = accessInfo.allowStartDt;
                        cbCarId.Text = accessInfo.carId;
                        tbPurpose.Text = accessInfo.purpose;
                        if(accessInfo.order != null)
                        {
                            tbOrderId.Text = accessInfo.order.orderId;
                        }
                    }
                    else
                    {
                        UpdateStatusMessage("출입 정보가 없습니다.");
                    }
                    break;
            }
        }

        private void CheckAccessDt(DateTime accessDt)
        {
            // [BBAEK] null 체크가 안됨
            if (accessDt == default(DateTime)) {
                tbIsAccess.Text = "미출입";
            }
            else
            {
                tbIsAccess.Text = "출입";
                tbAccessDt.Text = accessDt.ToString(customDateFormat);
                btnApply.Enabled = false;
                btnOrder.Enabled = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (allowStartDt.Value == default(DateTime))
            {
                MessageBox.Show("출입시간일시를 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }
            else if (allowEndDt.Value == default(DateTime))
            {
                MessageBox.Show("출입종료일시를 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }
            else if (tbPurpose.Text == "")
            {
                MessageBox.Show("출입 목적을 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }
            else if (cbCarId.SelectedIndex == -1)
            {
                MessageBox.Show("차량번호를 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                accessInfo.psgCnt = Int32.Parse(nudPsgCnt.Value.ToString());
                accessInfo.allowStartDt = allowStartDt.Value;
                accessInfo.allowEndDt = allowEndDt.Value;
                accessInfo.purpose = tbPurpose.Text;
                accessInfo.carId = cbCarId.Text;
                switch (mode)
                {
                    case DIALOG_MODE.SAVE:
                        accessInfo.user.Id = Int32.Parse(tbUserId.Text);
                        break;
                    case DIALOG_MODE.MODIFY:
                        accessInfo.seq = Int32.Parse(tbSeq.Text.ToString());
                        break;
                }
                if (accessMgr.SaveAccessInfoWithOrder(accessInfo) > 0)
                {
                    UpdateStatusMessage("정상적으로 처리 되었습니다.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    UpdateStatusMessage("정상적으로 처리되지 않았습니다.");
                }
            }
        }

        private void UpdateStatusMessage(String msg)
        {
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = msg;
            });
        }

        private void allowStartDt_ValueChanged(object sender, EventArgs e)
        {
            
            TimeSpan ts = CalcurateDiffAccessDt();
            if (ts >= minAccessDt)
            {
                oldStartDt = allowStartDt.Value;
                UpdateDiffAccessDt(ts);
            }
            else
            {
                allowStartDt.Value = oldStartDt;
                MessageBox.Show("출입허용시간은 최소 1시간 이상 되어야 합니다.", "알림", MessageBoxButtons.OK);
            }
        }

        private void allowEndDt_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan ts = CalcurateDiffAccessDt();
            if (ts >= minAccessDt)
            {
                oldEndDt = allowEndDt.Value;
                UpdateDiffAccessDt(ts);
            }
            else
            {
                allowEndDt.Value = oldEndDt;
                MessageBox.Show("출입허용시간은 최소 1시간 이상 되어야 합니다.", "알림", MessageBoxButtons.OK);
            }
        }

        private void UpdateDiffAccessDt(TimeSpan ts)
        {
            tbAccessDay.Text = ts.Days.ToString() + " 일";
            tbAccessHour.Text = ts.Hours.ToString() + " 시간";
            tbAccessMin.Text = ts.Minutes.ToString() + " 분";
        }

        private TimeSpan CalcurateDiffAccessDt()
        {
            return allowEndDt.Value - allowStartDt.Value;
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            OrderInfoDialog dlg = new OrderInfoDialog(this);
            UpdateDialogResult(dlg.ShowDialog());
        }

        private void UpdateDialogResult(DialogResult dlgRt)
        {
            if (dlgRt == DialogResult.OK)
            {
                UpdateStatusMessage("정상적으로 처리 되었습니다.");
                // 작업지시서 등록 여부 표시(tbOrderId)
                if(accessInfo.order != null && accessInfo.order.orderId != null)
                {
                    tbOrderId.Text = accessInfo.order.orderId;
                }
            }
            else
            {
                UpdateStatusMessage("취소되었습니다.");
            }
        }
    }
}
