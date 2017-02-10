using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AsyncSocketServer.OrderInfoManager;

namespace AsyncSocketServer
{
    public partial class OrderInfoDialog : Form
    {
        int accessId;
        OrderInfoManager orderMgr;
        const string customDateFormat = "yyyy년 MM월 dd일 (ddd) HH시 mm분";

        public OrderInfoDialog(int accessId)
        {
            InitializeComponent();
            this.accessId = accessId;
        }

        private void OrderInfoDialog_Load(object sender, EventArgs e)
        {
            orderMgr = new OrderInfoManager();
            DateTime dt = DateTime.Now;
            workDt.Value = dt;
            LoadOrderInfoDB();
        }

        private void UpdateComponents(OrderInfo info)
        {
            tbOrderId.Text = info.orderId.ToString();
            workDt.Value = info.work_dt;
        }

        private void LoadOrderInfoDB()
        {
            OrderInfo orderInfo = orderMgr.FindOrderInfoByAccessId(this.accessId);
            if (orderInfo != null)
            {
                UpdateComponents(orderInfo);
            }
        }

        private void UpdateStatusMessage(String msg)
        {
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = msg;
            });
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                OrderInfo orderInfo = new OrderInfo();
                orderInfo.accessId = this.accessId;
                orderInfo.orderId = tbOrderId.Text;
                orderInfo.work_dt = workDt.Value;

                if (orderMgr.SaveOrderInfo(orderInfo) > 0)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
