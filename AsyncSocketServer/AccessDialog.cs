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
        AccessInfoManager.DIALOG_MODE mode;
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
            minAccessDt = new TimeSpan(1, 0, 0);
            allowStartDt.Format = DateTimePickerFormat.Custom;
            allowStartDt.CustomFormat = customDateFormat;
            allowEndDt.Format = DateTimePickerFormat.Custom;
            allowEndDt.CustomFormat = customDateFormat;
            UpdateComponents();
        }

        private void LoadAccessInfoDB()
        {
            accessInfo = new AccessInfoDB().SelectAccessInfo(accessInfo.seq);
        }

        private void UpdateComponents()
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

                        Console.WriteLine(allowStartDt.Value.Date);
                    }
                    else
                    {
                        StatusMessage("출입 정보가 없습니다.");
                    }
                    break;
            }
        }

        private void ShowAccessText(Boolean isAccess, DateTime AccessDt)
        {
            tbIsAccess.Text = isAccess ? "출입" : "미출입";
            if (isAccess)
            {

                tbAccessDt.Text = AccessDt.ToString(customDateFormat);
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
                int executeCnt = 0;
                AccessInfo info = new AccessInfo();
                switch (mode)
                {
                    case DIALOG_MODE.SAVE:
                        info.user.Id = Int32.Parse(tbUserId.Text);
                        info.psgCnt = Int32.Parse(nudPsgCnt.Value.ToString());
                        info.allowStartDt = allowStartDt.Value;
                        info.allowEndDt = allowEndDt.Value;
                        executeCnt = accessMgr.SaveAccessInfo(info);
                        break;
                    case DIALOG_MODE.MODIFY:
                        info.seq = Int32.Parse(tbSeq.Text.ToString());
                        info.psgCnt = Int32.Parse(nudPsgCnt.Value.ToString());
                        info.allowStartDt = allowStartDt.Value;
                        info.allowEndDt = allowEndDt.Value;
                        executeCnt = accessMgr.UpdateAccessInfo(info);
                        break;
                }
                if (executeCnt > 0)
                {
                    StatusMessage("저장되었습니다.");
                    this.Close();
                }
                else
                {
                    StatusMessage("저장되지 않았습니다.");
                }
            }
        }

        private void StatusMessage(String msg)
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
    }
}
