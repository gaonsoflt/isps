using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AsyncSocketServer.UserManager;

namespace AsyncSocketServer
{
    public partial class UserDialog : Form
    {
        FingerSensor fingerSensor;
        UserManager userManager;
        MyPerson m_user = null;
        UserManager.MODE mode;

        public UserDialog(UserManager.MODE mode)
        {
            InitializeComponent();
            this.mode = mode;
        }

        public UserDialog(UserManager.MODE mode, int userId)
        {
            InitializeComponent();
            this.mode = mode;
            m_user = new MyPerson();
            m_user.Id = userId;
        }

        private void UserDialog_Load(object sender, EventArgs e)
        {
            fingerSensor = FingerSensor.GetFingerSensorInstance();
            userManager = new UserManager();

            switch(mode)
            {
                case MODE.SAVE:
                    this.Text = "등록";
                    break;
                case MODE.MODIFY:
                    this.Text = "수정";
                    LoadUser(m_user.Id);
                    UpdateComponents();
                    break;
            }
        }

        private void LoadUser(int userId)
        {
            m_user = new UserDB().SelectISPSUser(userId);
        }

        private void UpdateComponents()
        {
            if (m_user != null)
            {
                tbId.Text = m_user.Id.ToString();
                tbName.Text = m_user.Name.ToString();
                tbIdnum.Text = m_user.IdNum.ToString();
                tbPhone.Text = m_user.Phone.ToString();
                tbEmail.Text = m_user.Email.ToString();
                pbFingerPrint.Image = m_user.Fingerprints[0].AsBitmap;
            } else
            {
                StatusMessage("인원 정보가 없습니다.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("인원 등록/수정을 취소 하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tbName.Text == "" || tbName == null)
            {
                MessageBox.Show("이름을 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }
            else if (tbIdnum.Text == "" || tbIdnum == null)
            {
                MessageBox.Show("주민번호를 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }
            else if (tbPhone.Text == "" || tbPhone== null)
            {
                MessageBox.Show("연락처를 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }
            else if (pbFingerPrint.Image == null)
            {
                MessageBox.Show("지문을 입력하세요.", "알림", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int rtn = 0;
                byte[] iBytes = BBDataConverter.ImageToByte(BBDataConverter.GrayRawToBitmap(fingerSensor.getRawImage(), 320, 240));
                m_user = userManager.Enroll(iBytes, tbName.Text, tbIdnum.Text, tbPhone.Text, tbEmail.Text);
                switch (mode)
                {
                    case MODE.SAVE:
                        rtn = userManager.saveUser(m_user);
                        break;
                    case MODE.MODIFY:
                        m_user.Id = Int32.Parse(tbId.Text.ToString());
                        rtn = userManager.updateUser(m_user);
                        break;
                }
                if (rtn > 0)
                {
                    StatusMessage("사용자(" + m_user.Name + ") 정보가 저장되었습니다.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                } else
                {
                    StatusMessage("사용자(" + m_user.Name + ") 정보가 저장되지 않았습니다.");
                }
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            if(pbFingerPrint.Image != null)
            {
                if (MessageBox.Show("지문 정보를 변경하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            try
            {
                EnableFingerPrintButton(false);
                if (fingerSensor.CmdCmosLed(true) == 0)
                {
                    StatusMessage("Input your finger on sensor.");
                    if (fingerSensor.CmdCaptureFinger() == 0)
                    {
                        StatusMessage("Exporting deleted fingerprint data");
                        if (fingerSensor.CmdGetRawImage() == 0)
                        {
                            StatusMessage("Succeed export fingerprint data.");
                            Invoke((MethodInvoker)delegate
                            {
                                pbFingerPrint.Image = BBDataConverter.GrayRawToBitmap(fingerSensor.getRawImage(), 320, 240);
                            });
                        }
                        else
                        {
                            StatusMessage("Failed export fingerparint data.");
                        }
                    }
                    else
                    {
                        StatusMessage("Time out or can not delected fingerprint.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StatusMessage("Failed export fingerparint data.");
            }
            finally
            {
                fingerSensor.CmdCmosLed(false);
                EnableFingerPrintButton(true);
            }
        }

        private void EnableFingerPrintButton(Boolean enable)
        {
            btnCancel.Enabled = enable;
            btnSave.Enabled = enable;
            btnScan.Enabled = enable;
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
