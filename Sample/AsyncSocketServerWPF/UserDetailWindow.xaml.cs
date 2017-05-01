using AsyncSocketServer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static AsyncSocketServer.UserManager;

namespace AsyncSocketServerWPF
{
    /// <summary>
    /// UserDetailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserDetailWindow : Window
    {
        FingerSensor fingerSensor;
        UserManager userManager;
        MyPerson m_user = null;
        UserManager.MODE mode;
        MyFingerprint fp;

        public UserDetailWindow(UserManager.MODE mode)
        {
            InitializeComponent();
            this.mode = mode;
        }

        public UserDetailWindow(UserManager.MODE mode, int userId)
        {
            InitializeComponent();
            this.mode = mode;
            m_user = new MyPerson();
            m_user.Id = userId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fingerSensor = FingerSensor.GetFingerSensorInstance();
            userManager = new UserManager();
            fp = new MyFingerprint();
            InitComponents();
        }

        private void LoadUser(int userId)
        {
            m_user = new UserDB().SelectISPSUser(userId);
        }

        private void InitComponents()
        {
            switch (mode)
            {
                case MODE.SAVE:
                    this.Title = "출입자 등록";
                    break;
                case MODE.MODIFY:
                    this.Title = "출입자 수정";
                    try
                    {
                        LoadUser(m_user.Id);
                        tbId.Text = m_user.Id.ToString();
                        tbName.Text = m_user.Name.ToString();
                        tbIdNum.Text = m_user.IdNum.ToString();
                        tbPhone.Text = m_user.Phone.ToString();
                        tbEmail.Text = m_user.Email.ToString();
                        fp.AsBitmap = m_user.Fingerprints[0].AsBitmap;
                        UpdateReceivedImage(m_user.Fingerprints[0].AsBitmap);
                    }
                    catch (Exception e)
                    {
                        if (MessageBox.Show("인원 정보를 찾을 수 없습니다.", "알림", MessageBoxButton.OK) == MessageBoxResult.OK)
                        {
                            this.Close();
                        }
                    }
                    break;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("인원 등록/수정을 취소 하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text == "" || tbName == null)
            {
                MessageBox.Show("이름을 입력하세요.", "알림", MessageBoxButton.OK);
                return;
            }
            else if (tbIdNum.Text == "" || tbIdNum == null)
            {
                MessageBox.Show("주민번호를 입력하세요.", "알림", MessageBoxButton.OK);
                return;
            }
            else if (tbPhone.Text == "" || tbPhone == null)
            {
                MessageBox.Show("연락처를 입력하세요.", "알림", MessageBoxButton.OK);
                return;
            }
            else if (pbFingerPrint.Source == null)
            {
                MessageBox.Show("지문을 입력하세요.", "알림", MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int executeCnt = 0;
                byte[] fpBytes = BBDataConverter.ImageToByte(fp.AsBitmap);
                m_user = userManager.Enroll(fpBytes, UserId, tbName.Text, tbIdNum.Text, tbPhone.Text, tbEmail.Text);
                executeCnt = userManager.SaveUser(m_user);
                if (executeCnt > 0)
                {
                    MessageBox.Show("사용자(" + m_user.Name + ") 정보가 저장되었습니다.", "알림", MessageBoxButton.OK);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("사용자(" + m_user.Name + ") 정보가 저장되지 않았습니다.", "알림", MessageBoxButton.OK);
                }
            }
        }

        int UserId
        {
            get
            {
                int a;
                try
                {
                    a = Int32.Parse(tbId.Text.ToString());
                }
                catch
                {
                    a = -1;
                }
                return a;
            }
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            ScanImage();
        }

        private void ScanImage()
        {
            if (pbFingerPrint.Source != null)
            {
                if (MessageBox.Show("지문 정보를 변경하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            try
            {
                EnableFingerPrintButton(false);
                if (fingerSensor.CmdCmosLed(true) == 0)
                {
                    Console.WriteLine("Input your finger on sensor.");
                    if (fingerSensor.CmdCaptureFinger() == 0)
                    {
                        Console.WriteLine("Exporting deleted fingerprint data");
                        if (fingerSensor.CmdGetRawImage() == 0)
                        {
                            Console.WriteLine("Succeed export fingerprint data.");
                            Bitmap receivedImage = BBDataConverter.GrayRawToBitmap(fingerSensor.getRawImage(), FingerSensorPacket.SIZE_FP_WIDTH, FingerSensorPacket.SIZE_FP_HEIGHT);
                            UpdateReceivedImage(receivedImage);
                            fp.AsBitmap = receivedImage;
                        }
                        else
                        {
                            Console.WriteLine("Failed export fingerparint data.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Time out or can not delected fingerprint.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Failed export fingerparint data.");
            }
            finally
            {
                fingerSensor.CmdCmosLed(false);
                EnableFingerPrintButton(true);
            }
        }

        private void EnableFingerPrintButton(bool enable)
        {
            btnCancel.IsEnabled = enable;
            btnSave.IsEnabled = enable;
            btnScan.IsEnabled = enable;
        }

        private void UpdateReceivedImage(Bitmap bitmap)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                pbFingerPrint.Source = BBDataConverter.BitmapToImageSource(bitmap);
            }));
        }
    }
}
