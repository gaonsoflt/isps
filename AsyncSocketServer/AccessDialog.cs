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
        AccessInfo accessInfo;
        AccessInfoManager accessMgr;
        CarInfoManager carMgr;
        DIALOG_MODE mode;
        const string customDateFormat = "yyyy년 MM월 dd일 (ddd) HH시 mm분";
        TimeSpan minAccessDt;
        DateTime oldStartDt;
        DateTime oldEndDt;

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

            tbCarId.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tbCarId.AutoCompleteSource = AutoCompleteSource.CustomSource;

            InitComponents();
        }

        private void LoadAccessInfoDB()
        {
            accessInfo = new AccessInfoDB().SelectAccessInfo(accessInfo.seq);
        }

        private void InitComponents()
        {
            switch (mode)
            {
                case DIALOG_MODE.SAVE:
                    this.Text = "등록";
                    if (accessInfo != null)
                    {
                        tbSeq.Text = accessInfo.seq.ToString();
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
                        ShowAccessText(accessInfo.isAccess, accessInfo.access_dt);
                        nudPsgCnt.Value = accessInfo.psgCnt;
                        allowEndDt.Value = accessInfo.allowEndDt;
                        allowStartDt.Value = accessInfo.allowStartDt;
                        tbCarId.Text = accessInfo.carId;
                        tbPurpose.Text = accessInfo.purpose;
                    }
                    else
                    {
                        UpdateStatusMessage("출입 정보가 없습니다.");
                    }
                    break;
            }
        }

        private void ShowAccessText(Boolean isAccess, DateTime AccessDt)
        {
            tbIsAccess.Text = isAccess ? "출입" : "미출입";
            if (isAccess)
            {
                if (AccessDt != null) {
                    tbAccessDt.Text = AccessDt.ToString(customDateFormat);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                AccessInfo info = new AccessInfo();
                info.psgCnt = Int32.Parse(nudPsgCnt.Value.ToString());
                info.allowStartDt = allowStartDt.Value;
                info.allowEndDt = allowEndDt.Value;
                info.purpose = tbPurpose.Text;
                info.carId = tbCarId.Text;
                switch (mode)
                {
                    case DIALOG_MODE.SAVE:
                        info.user.Id = Int32.Parse(tbUserId.Text);
                        break;
                    case DIALOG_MODE.MODIFY:
                        info.seq = Int32.Parse(tbSeq.Text.ToString());
                        break;
                }
                if (accessMgr.SaveAccessInfo(info) > 0)
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

        private void tbCarId_TextChanged(object sender, EventArgs e)
        {
            //TextBox t = sender as TextBox;
            //if (t != null)
            //{
            //    //say you want to do a search when user types 3 or more chars
            //    if (t.Text.Length >= 2)
            //    {
            //        //SuggestStrings will have the logic to return array of strings either from cache/db
            //        string[] arr = carMgr.SuggestStrings(t.Text);
            //        if (arr != null)
            //        {
            //            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            //            collection.AddRange(arr);
            //            tbCarId.AutoCompleteCustomSource = collection;
            //        }
            //    }
            //}
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            OrderInfoDialog dlg = new OrderInfoDialog(accessInfo.seq);
            UpdateDialogResult(dlg.ShowDialog());
        }

        private void UpdateDialogResult(DialogResult dlgRt)
        {
            if (dlgRt == DialogResult.OK)
            {
                // 작업지시서 등록 여부 표시(tbOrderId)
                UpdateStatusMessage("정상적으로 처리 되었습니다.");
            }
            else
            {
                UpdateStatusMessage("취소되었습니다.");
            }
        }
    }
}
