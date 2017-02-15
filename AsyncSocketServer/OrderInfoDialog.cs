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
        OrderInfoManager orderMgr;
        const string customDateFormat = "yyyy년 MM월 dd일 (ddd)";
        OrderInfo order;
        AccessDialog parents;

        public OrderInfoDialog(AccessDialog parents)
        {
            InitializeComponent();
            this.parents = parents;
            //this.order = order;
        }

        private void OrderInfoDialog_Load(object sender, EventArgs e)
        {
            orderMgr = new OrderInfoManager();
            workDt.CustomFormat = customDateFormat;

            //LoadOrderInfoDB();
            UpdateComponents(parents.accessInfo.order);
        }

        private void UpdateComponents(OrderInfo order)
        {
            if(order != null)
            {
                tbOrderId.Text = order.orderId.ToString();
                workDt.Value = order.work_dt;
            }
            else
            {
                workDt.Value = DateTime.Now;
            }
        }

        //private void LoadOrderInfoDB()
        //{
        //    OrderInfo orderInfo = orderMgr.FindOrderInfoByAccessId(this.accessId);
        //    if (orderInfo != null)
        //    {
        //        UpdateComponents(orderInfo);
        //    }
        //}

        private void UpdateStatusMessage(String msg)
        {
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = msg;
            });
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (tbOrderId.Text == "")
            {
                MessageBox.Show("지시서번호를 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if(parents.accessInfo.order == null)
                {
                    parents.accessInfo.order = new OrderInfo();
                }
                //OrderInfo orderInfo = new OrderInfo();
                //orderInfo.orderId = tbOrderId.Text;
                //orderInfo.work_dt = workDt.Value;
                //order = orderInfo;
                parents.accessInfo.order.orderId = tbOrderId.Text;
                parents.accessInfo.order.work_dt = workDt.Value;

                UpdateStatusMessage("정상적으로 처리 되었습니다.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
