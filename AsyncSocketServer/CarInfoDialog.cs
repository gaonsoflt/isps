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

namespace AsyncSocketServer
{
    public partial class CarInfoDialog : Form
    {
        CarInfoManager.DIALOG_MODE mode;
        CarInfo m_car;
        CarInfoManager m_carMgr;

        public CarInfoDialog(CarInfoManager.DIALOG_MODE mode)
        {
            InitializeComponent();
            this.mode = mode;
        }

        public CarInfoDialog(CarInfoManager.DIALOG_MODE mode, string carId)
        {
            InitializeComponent();
            this.mode = mode;
            m_car = new CarInfo();
            m_car.id = carId;
        }

        private void CarInfoDialog_Load(object sender, EventArgs e)
        {
            m_carMgr = new CarInfoManager();
            UpdateComponents();
        }

        private void LoadCarInfo(string carId)
        {
            m_car = new CarInfoDB().SelectCarInfo(carId);
        }

        private void UpdateComponents()
        {
            switch (mode)
            {
                case DIALOG_MODE.SAVE:
                    this.Text = "등록";
                    tbCarId.ReadOnly = false;
                    break;
                case DIALOG_MODE.MODIFY:
                    this.Text = "수정";
                    LoadCarInfo(m_car.id);
                    if (m_car != null)
                    {
                        tbCarId.Text = m_car.id.ToString();
                        tbOwner.Text = m_car.owner.ToString();
                    }
                    else
                    {
                        StatusMessage("차량 정보가 없습니다.");
                    }
                    tbCarId.ReadOnly = true;
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("차량 등록/수정을 취소 하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tbCarId.Text == "" || tbCarId == null)
            {
                MessageBox.Show("차량번호를 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }
            else if (tbOwner.Text == "" || tbOwner == null)
            {
                MessageBox.Show("차량소유자를 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int rtn = 0;
                switch (mode)
                {
                    case DIALOG_MODE.SAVE:
                        m_car.id = tbCarId.Text.ToString();
                        m_car.owner = tbOwner.Text.ToString();
                        rtn = m_carMgr.SaveCarInfo(m_car);
                        break;
                    case DIALOG_MODE.MODIFY:
                        m_car.id = tbCarId.Text.ToString();
                        m_car.owner = tbOwner.Text.ToString();
                        rtn = m_carMgr.UpdateCarInfo(m_car);
                        break;
                }
                if (rtn > 0)
                {
                    StatusMessage("차량(" + m_car.id + ") 정보가 저장되었습니다.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    StatusMessage("차량(" + m_car.id + ") 정보가 저장되지 않았습니다.");
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
    }
}
