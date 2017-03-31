using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AsyncSocketServer;
using System.Net.Sockets;
using static AsyncSocketServer.UserManager;
using System.Drawing;

namespace AsyncSocketServerWPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ServerWindow : Window
    {
        Listener listener;
        static List<Client> ClientList;
        FingerSensor fingerSensor;

        string[] arrPort = { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9" };
        string[] arrRate = { "2400", "4800", "9600", "14400", "19200", "38400", "57600", "115200" };

        public ServerWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            CommonConfig.NewInstance();
            fingerSensor = FingerSensor.GetFingerSensorInstance();
            ClientList = new List<Client>();

            cbComPort.ItemsSource = arrPort;
            cbComPort.SelectedIndex = 2;
            cbComRate.ItemsSource = arrRate;
            cbComRate.SelectedIndex = 2;

            InitFuction();
        }

        private void InitFuction()
        {
            Client.UpdateLogMsg += new Client.SetLogHandler(UpdateCompLogMsg);
        }

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            if(StartSocket())
            {
                if(cbConnetType.IsChecked.Value)
                {
                    OpenSerialPort();
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            string msg = "서버를 종료하시겠습니까?";
            if (MessageBox.Show(msg, "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (CloseSocket())
                {
                    if(cbConnetType.IsChecked.Value)
                    {
                        CloseSerialPort();
                    }
                }
            }
        }

        bool StartSocket()
        {
            try
            {
                int port = 8192;
                listener = new Listener();
                listener.Accepted += new Listener.SocketAcceptedHandler(listener_Accepted);
                listener.Start(port);
                UpdateCompLogMsg("Server started!!! (" + Listener.GetLocalIPAddress() + ": " + port + ")");
                EnableSocketComponent(false);
            }
            catch
            {
                CloseSocket();
                return false;
            }
            return true;
        }

        bool CloseSocket()
        {
            try
            {
                foreach (Client client in ClientList)
                {
                    if (client != null)
                    {
                        client.Close();
                    }
                }
                ClientList.Clear();

                if (listener != null && listener.Running)
                    listener.Stop();

                lstText.Items.Clear();
                //pbImage.Image = null;
                //pbFPRef.Image = null;

                EnableSocketComponent(true);
            }
            catch
            {
                return false;
            }
            return true;
        }

        void listener_Accepted(Socket e)
        {
            Client client = new Client(e);
            ClientList.Add(client);

            client.OnSend += new Client.OnSendEventHandler(client_OnSend);
            client.DataReceived += new Client.DataReceivedEventHandler(client_DataReceived);
            client.Disconnected += new Client.DisconnectedEventHandler(client_Disconnected);
            client.ReceiveAsync();

            UpdateCompLogMsg("Connected client: " + client.EndPoint.ToString());
        }

        void client_Disconnected(Client sender)
        {
            UpdateCompLogMsg("Disconnected client: " + sender.name);
            sender.Close();
            sender = null;
            MessageBoxResult res = MessageBox.Show("Client Disconnected\nClear Data?", "서버 메시지", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
                {
                    //lstText.Items.Clear();
                }
        }

        void client_OnSend(Client sender, int sent)
        {
            string msg = string.Format(sender.name + " - Data Sent:{0}\n", sent);
            UpdateCompLogMsg(msg);
        }

        void client_DataReceived(Client sender, ReceiveBuffer e)
        {
            try
            {
                DataPacket.Packet pkt = DataPacket.DataParser(e.BufStream, e.pkt);
                PktType header = pkt.type;
                UpdateCompLogMsg(sender.name + " - Received data: " + pkt.ToString());
                switch (header)
                {
                    case PktType.AUTH:
                        UpdateReceivedImage(pkt.fingerPrint);
                        UpdateCompMatchedUser(sender.RunAuth(pkt));
                        //sender.RunAuth(pkt);
                        break;
                    case PktType.PASSENGER:
                        sender.RunPassenger(pkt);
                        break;
                    case PktType.ORDER:
                        sender.RunOrder(pkt);
                        break;
                }
            }
            catch (Exception ee)
            {
                UpdateCompLogMsg(ee.Message);
            }
        }

        private void UpdateCompLogMsg(string msg)
        {
            Console.WriteLine(msg);
            string now = "[" + DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + "] ";
            Dispatcher.Invoke(new Action(() =>{
                lstText.Items.Add(now + msg);
                lstText.SelectedIndex = lstText.Items.Count - 1;
            }));
        }

        private void UpdateCompMatchedUser(MyPerson match)
        {
            Dispatcher.Invoke(new Action(() => {
                if (match != null)
                {
                    tbId.Text = match.Id.ToString();
                    tbName.Text = match.Name.ToString();
                    tbPhone.Text = match.Phone.ToString();
                    tbGuid.Text = match.Guid.ToString();
                    pbFPRef.Source = BBDataConverter.BitmapToImageSource(match.Fingerprints[0].AsBitmap);
                }
                else
                {
                    tbId.Text = "";
                    tbName.Text = "";
                    tbPhone.Text = "";
                    tbGuid.Text = "";
                    pbFPRef.Source = null;
                }
            }));
        }

        private void UpdateReceivedImage(Bitmap bitmap)
        {
            Dispatcher.Invoke(new Action(() => {
                pbImage.Source = BBDataConverter.BitmapToImageSource(bitmap);
            }));
        }

        private void btnSerialOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenSerialPort();
        }

        private void btnSerialClose_Click(object sender, RoutedEventArgs e)
        {
            CloseSerialPort();
        }
        
        private void cbConnetType_Click(object sender, RoutedEventArgs e)
        {
            if (cbConnetType.IsChecked.Value)
            {
                btnSerialOpen.IsEnabled = false;
            }
            else
            {
                btnSerialOpen.IsEnabled = true;
            }
        }

        private void EnableSocketComponent(bool enable)
        {
            cbConnetType.IsEnabled = enable;
            btnListen.IsEnabled = enable;
            btnClose.IsEnabled = !enable;
        }

        private void EnableSerialComponent(bool enable)
        {
            btnSerialOpen.IsEnabled = enable;
            btnSerialClose.IsEnabled = !enable;

            cbComPort.IsEnabled = enable;
            cbComRate.IsEnabled = enable;

            EnableFingerPrintComponent(!enable);
        }

        private void EnableFingerPrintComponent(bool enable)
        {
            btnTestAuth.IsEnabled = enable;
        }

        private void OpenSerialPort()
        {
            try
            {
                cbComRate.SelectedIndex = 2; // 9600
                OpenSerial();
                cbComRate.SelectedIndex = 7; // 115200
                if (fingerSensor.CmdChangeBaudrate(Convert.ToInt32(cbComRate.SelectedItem.ToString())) == 0)
                {
                    fingerSensor.CloseSerialPort();
                    OpenSerial();
                    fingerSensor.CmdOpen();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OpenSerial()
        {
            try
            {
                fingerSensor.OpenSerialPort(cbComPort.SelectedItem.ToString(), Convert.ToInt32(cbComRate.SelectedItem.ToString()));
                if (fingerSensor.sPort.IsOpen)
                {
                    EnableSerialComponent(false);
                    UpdateCompLogMsg("시리얼 포트가 연결되었습니다.");
                }
                else
                {
                    EnableSerialComponent(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CloseSerialPort()
        {
            if (fingerSensor.sPort != null)
            {
                if (fingerSensor.sPort.IsOpen)
                {
                    if (fingerSensor.CmdClose() == 0)
                    {
                        fingerSensor.CmdChangeBaudrate(9600);
                        fingerSensor.CloseSerialPort();
                        EnableSerialComponent(true);
                        UpdateCompLogMsg("시리얼 포트가 해제되었습니다.");
                    }
                }
            }
        }

        private void btnSysMgr_Click(object sender, RoutedEventArgs e)
        {
            SystemMgrWindow window = new SystemMgrWindow();
            window.ShowDialog();
        }

        private void btnTestAuth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnableFingerPrintComponent(false);
                if (fingerSensor.CmdCmosLed(true) == 0)
                {
                    UpdateCompLogMsg("Input your finger on sensor.");
                    if (fingerSensor.CmdCaptureFinger() == 0)
                    {
                        UpdateCompLogMsg("Exporting deleted fingerprint data");
                        if (fingerSensor.CmdGetRawImage() == 0)
                        {
                            UpdateCompLogMsg("Succeed export fingerprint data.");
                            Bitmap receivedImage = BBDataConverter.GrayRawToBitmap(fingerSensor.getRawImage(), FingerSensorPacket.SIZE_FP_WIDTH, FingerSensorPacket.SIZE_FP_HEIGHT);
                            UpdateReceivedImage(receivedImage);
                            UserManager fpm = new UserManager();
                            MyPerson guest = fpm.Enroll(BBDataConverter.ImageToByte(receivedImage), "guest");
                            MyPerson match = fpm.recognition(guest);
                            if (match != null)
                            {
                                UpdateCompLogMsg("Matched person(" + match.Name + "): " + VerifyUserMatchRate(guest, match));
                            }
                            else
                            {
                                UpdateCompLogMsg("No matching person found.");
                            }
                            UpdateCompMatchedUser(match);
                        }
                        else
                        {
                            UpdateCompLogMsg("Failed export fingerparint data.");
                        }
                    }
                    else
                    {
                        UpdateCompLogMsg("Time out or can not delected fingerprint.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                UpdateCompLogMsg("Failed export fingerparint data.");
            }
            finally
            {
                fingerSensor.CmdCmosLed(false);
                EnableFingerPrintComponent(true);
            }
        }
    }
}
