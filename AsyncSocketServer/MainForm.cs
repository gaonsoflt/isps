using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using static AsyncSocketServer.UserManager;
using System.IO.Ports;
using System.Threading;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Data.OracleClient;
using Npgsql;
using static AsyncSocketServer.DataPacket;

namespace AsyncSocketServer
{
    public partial class MainForm : Form
    {
        Listener listener;
        static List<Client> ClientList;
        FingerSensor fingerSensor;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //this.Size = new Point(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.WindowState = FormWindowState.Maximized;
            // 공통메시지 초기화
            CommonConfig.NewInstance();
            fingerSensor = FingerSensor.GetFingerSensorInstance();
            ClientList = new List<Client>();

            string[] arrPort = { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9" };
            string[] arrRate = { "2400", "4800", "9600", "14400", "19200", "38400", "57600", "115200" };
            
            //cbComport.BeginUpdate();
            //foreach (string comport in SerialPort.GetPortNames())
            //{
            //    cbComport.Items.Add(comport);
            //}
            //cbComport.EndUpdate();
            cbComport.Items.AddRange(arrPort);
            cbComport.SelectedIndex = 2;
            cbRate.Items.AddRange(arrRate);
            cbRate.SelectedIndex = 2;

            InitFuction();
        }

        private void InitFuction()
        {
            Client.UpdateLogMsg += new Client.SetLogHandler(UpdateCompLogMsg);
            Client.UpdateMatchedUser += new Client.SetMatchedUserHandler(UpdateCompMatchedUser);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseSocket())
            {
                if (cbConnetType.Checked)
                {
                    CloseSerialPort();
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
                UpdateStatusMessage("Server started!!! (127.0.0.1:" + port + ")");
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

                UpdateStatusMessage("...");

                lstText.Items.Clear();
                pbImage.Image = null;
                pbFPRef.Image = null;

                EnableSocketComponent(true);
            }
            catch
            {
                return false;
            }
            return true;
        }

        void btnListen_Click(object sender, EventArgs e)
        {
            if(StartSocket())
            {
                if(cbConnetType.Checked)
                {
                    OpenSerialPort();
                }
            }
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

        void btnClose_Click(object sender, EventArgs e)
        {
            string msg = "서버" + (cbConnetType.Checked ? "(시리얼 포함)" : "") + "를 종료하시겠습니까?";
            if (MessageBox.Show(msg, "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (CloseSocket())
                {
                    if (cbConnetType.Checked)
                    {
                        CloseSerialPort();
                    }
                }
            }
        }

        private void btnSerialOpen_Click(object sender, EventArgs e)
        {
            OpenSerialPort();
        }

        private void btnSerialClose_Click(object sender, EventArgs e)
        {
            CloseSerialPort();
        }

        private void OpenSerialPort()
        {
            try
            {
                cbRate.SelectedIndex = 2; // 9600
                OpenSerial();
                cbRate.SelectedIndex = 7; // 115200
                if (fingerSensor.CmdChangeBaudrate(Convert.ToInt32(cbRate.SelectedItem.ToString())) == 0)
                {
                    fingerSensor.CloseSerialPort();
                    OpenSerial();
                    fingerSensor.CmdOpen();
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OpenSerial()
        {
            try
            {
                fingerSensor.OpenSerialPort(cbComport.SelectedItem.ToString(), Convert.ToInt32(cbRate.SelectedItem.ToString()));
                if (fingerSensor.sPort.IsOpen)
                {
                    EnableSerialComponent(false);
                    UpdateStatusMessage("시리얼 포트가 연결되었습니다.");
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
                        UpdateStatusMessage("시리얼 포트가 해제되었습니다.");
                    }
                }
            }
        }

        void client_Disconnected(Client sender)
        {
            UpdateCompLogMsg("Disconnected client: " + sender.name);
            sender.Close();
            sender = null;
            Invoke((MethodInvoker)delegate
            {
                DialogResult res = MessageBox.Show("Client Disconnected\nClear Data?", "서버 메시지", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    //lstText.Items.Clear();
                    pbImage.Image = null;
                    pbFPRef.Image = null;
                    tbId.Text = "";
                    tbName.Text = "";
                    tbGuid.Text = "";
                    tbPhone.Text = "";
                }
            });
        }

        void client_OnSend(Client sender, int sent)
        {
            String msg = string.Format(sender.name + " - Data Sent:{0}\n", sent);
            UpdateCompLogMsg(msg);
        }

        public static T ByteToType<T>(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        void client_DataReceived(Client sender, ReceiveBuffer e)
        {
            Packet pkt = DataPacket.ByteToStruct(e.BufStream);
            PktType header = pkt.type;
            UpdateCompLogMsg(sender.name + " - Received data: " + pkt.ToString());
            switch (header)
            {
                case PktType.AUTH:
                    pbImage.Image = pkt.fingerPrint;
                    sender.RunAuth(pkt);
                    break;
                case PktType.PASSENGER:
                    sender.RunPassenger(pkt);
                    break;
                case PktType.ORDER:
                    sender.RunOrder(pkt);
                    break;
            }
        }

        private void btnEnroll_Click(object sender, EventArgs e)
        {
            ManagerDialog dlg = new ManagerDialog();
            dlg.ShowDialog();
        }

        private void EnableSocketComponent(bool enable)
        {
            cbConnetType.Enabled = enable;
            btnListen.Enabled = enable;
            btnClose.Enabled = !enable;
        }

        private void EnableSerialComponent(bool enable)
        {
            //btnSerialOpen.Enabled = enable;
            //btnSerialClose.Enabled = !enable;
            EnableFingerPrintComponent(!enable);
            cbComport.Enabled = enable;
            cbRate.Enabled = enable;
        }

        private void EnableFingerPrintComponent(bool enable)
        {
            btnEnroll.Enabled = enable;
            btnConfirm.Enabled = enable;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                EnableFingerPrintComponent(false);
                if (fingerSensor.CmdCmosLed(true) == 0)
                {
                    UpdateStatusMessage("Input your finger on sensor.");
                    if (fingerSensor.CmdCaptureFinger() == 0)
                    {
                        UpdateStatusMessage("Exporting deleted fingerprint data");
                        if (fingerSensor.CmdGetRawImage() == 0)
                        {
                            UpdateStatusMessage("Succeed export fingerprint data.");
                            byte[] iBytes = BBDataConverter.ImageToByte(BBDataConverter.GrayRawToBitmap(fingerSensor.getRawImage(), 320, 240));
                            UserManager fpm = new UserManager();
                            MyPerson guest = fpm.Enroll(iBytes, "guest");
                            MyPerson match = fpm.recognition(guest);

                            pbImage.Image = Image.FromStream(new MemoryStream(iBytes));
                            if (match != null)
                            {
                                UpdateCompMatchedUser(match);
                                UpdateStatusMessage("Matched person(" + tbName.Text + ")");
                            }
                            else
                            {
                                UpdateStatusMessage("No matching person found.");
                            }
                        }
                        else
                        {
                            UpdateStatusMessage("Failed export fingerparint data.");
                        }
                    }
                    else
                    {
                        UpdateStatusMessage("Time out or can not delected fingerprint.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                UpdateStatusMessage("Failed export fingerparint data.");
            }
            finally
            {
                fingerSensor.CmdCmosLed(false);
                EnableFingerPrintComponent(true);
            }
        }

        public void UpdateStatusMessage(string msg)
        {
            Console.WriteLine(msg);
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = msg;
            });
        }

        private void UpdateCompMatchedUser(MyPerson match)
        {
            Invoke((MethodInvoker)delegate
            {
                tbId.Text = match.Id.ToString();
                tbName.Text = match.Name.ToString();
                tbPhone.Text = match.Phone.ToString();
                tbGuid.Text = match.Guid.ToString();
                pbFPRef.Image = match.Fingerprints[0].AsBitmap;
            });
        }

        private void UpdateCompLogMsg(string msg)
        {
            Console.WriteLine(msg);
            string now = "[" + DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + "] ";
            if(lstText.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lstText.Items.Add(now + msg);
                });
            } else
            {
                lstText.Items.Add(now + msg);
            }
        }

        private void cbConnetType_CheckedChanged(object sender, EventArgs e)
        {
            if(cbConnetType.Checked)
            {
                btnSerialOpen.Enabled = false;
            } else
            {
                btnSerialOpen.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //AddListBoxItem("psg count: " + new AccessInfoDB().SelectAccessPsgCnt("e3135a87-2c34-485e-9070-1352b7ec31c8"));
            UpdateCompLogMsg(CommonConfig.Message.GetMessage2(textBox1.Text.ToString()));
        }
    }
}
