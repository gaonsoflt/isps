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

            UpdateStatusMessage("Connected: " + client.EndPoint.ToString());
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
            sender.Close();
            sender = null;

            UpdateStatusMessage("Connected: ...");
            Invoke((MethodInvoker)delegate
            {
                DialogResult res = MessageBox.Show("Client Disconnected\nClear Data?", "서버 메시지", MessageBoxButtons.YesNo);
                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    lstText.Items.Clear();
                    pbImage.Image = null;
                    pbFPRef.Image = null;
                    tbId.Text = "";
                    tbName.Text = "";
                }
            });
        }

        void client_OnSend(Client sender, int sent)
        {
            String msg = string.Format("Data Sent:{0}\n", sent);
            UpdateStatusMessage(msg);
            AddListBoxItem(msg);
        }

        void client_DataReceived(Client sender, ReceiveBuffer e)
        {
            BinaryReader r = new BinaryReader(e.BufStream);
            Commands header = (Commands)r.ReadInt32();

            switch (header)
            {
                case Commands.STRING:
                    {
                        string s = r.ReadString();
                        AddListBoxItem(s);
                        sender.SendText(s);
                    }
                    break;
                case Commands.IMAGE:
                    {
                        int imageBytesLen = r.ReadInt32();
                        byte[] iBytes = r.ReadBytes(imageBytesLen);
                        Invoke((MethodInvoker)delegate
                        {
                            pbImage.Image = Image.FromStream(new MemoryStream(iBytes));
                        });
                    }
                    break;
                case Commands.RECOG:
                    {
                        int imageBytesLen = r.ReadInt32();
                        byte[] iBytes = r.ReadBytes(imageBytesLen);
                        UserManager fpm = new UserManager();
                        MyPerson guest = fpm.Enroll(iBytes, "guest");
                        MyPerson match = fpm.recognition(guest);
                        if (match != null)
                        {
                            pbImage.Image = Image.FromStream(new MemoryStream(iBytes));
                            Invoke((MethodInvoker)delegate
                            {
                                tbId.Text = match.Id.ToString();
                                tbName.Text = match.Name.ToString();
                                pbFPRef.Image = match.Fingerprints[0].AsBitmap;
                            });
                            UpdateStatusMessage("Matched person(" + tbName.Text + ")");
                        }
                        else
                        {
                            UpdateStatusMessage("No matching person found.");
                        }
                    }
                    break;
                case Commands.ENROLL:
                    {
                        int imageBytesLen = r.ReadInt32();
                        byte[] iBytes = r.ReadBytes(imageBytesLen);
                        String name = ShowInputDialog("Input user name", "Enrollment");
                        UserManager fpm = new UserManager();
                        MyPerson user = fpm.Enroll(iBytes, name);
                        int rtn = fpm.saveUser(user);
                        Invoke((MethodInvoker)delegate
                        {
                            pbImage.Image = Image.FromStream(new MemoryStream(iBytes));
                        });
                        if (rtn > 0)
                        {
                            UpdateStatusMessage("Success enrolled user(" + name + ")");
                        }
                    }
                    break;
                case Commands.AUTH:
                    {
                        int userId = r.ReadInt32();
                        int response = r.ReadInt32();
                        int dataLen = r.ReadInt32();
                        byte[] iBytes = r.ReadBytes(dataLen);
                        AddListBoxItem("[AUTH] userId / dataLen");
                        AddListBoxItem(userId + " / " + dataLen);
                        UserManager fpm = new UserManager();
                        MyPerson guest = fpm.Enroll(iBytes, "guest");
                        MyPerson match = fpm.recognition(guest);
                        if (match != null)
                        {
                            pbImage.Image = Image.FromStream(new MemoryStream(iBytes));
                            Invoke((MethodInvoker)delegate
                            {
                                tbId.Text = match.Id.ToString();
                                tbName.Text = match.Name.ToString();
                                pbFPRef.Image = match.Fingerprints[0].AsBitmap;
                            });
                            UpdateStatusMessage("Matched person(" + tbName.Text + ")");
                            if (userId == match.Id)
                            {
                                sender.SendResponseAuthUserByFingerPrint(userId, Client.PKT_ACK, match.Guid);
                                sender.setLoginUser(match);
                            } else
                            {
                                sender.SendResponseAuthUserByFingerPrint(userId, Client.PKT_NACK, "");
                            }
                        }
                        else
                        {
                            sender.SendResponseAuthUserByFingerPrint(userId, Client.PKT_NACK, "");
                            UpdateStatusMessage("No matching person found.");
                        }
                    }
                    break;
                case Commands.PASSENGER:
                    {
                        try
                        {
                            int userId = r.ReadInt32();
                            int response = r.ReadInt32();
                            int dataLen = r.ReadInt32();
                            string guid = BBStringConverter.ByteToString(r.ReadBytes(dataLen - 4));
                            int passengerCnt = BitConverter.ToInt32(r.ReadBytes(4), 0);
                            AddListBoxItem("[PASSENGER] userId / dataLen / guid / passengerCnt");
                            AddListBoxItem(userId + " / " + dataLen + " / " + guid + "/" + passengerCnt);
                            /*
                             * DB 에서 Guid로 검색해 미리 입력해놓은 동승자 현황을 가져와 비교해야 함
                             */
                            int accessPassengerCnt = 0;
                            accessPassengerCnt = new AccessInfoDB().SelectAccessPsgCnt(guid);
                            AddListBoxItem("Accessed passenger count: " + accessPassengerCnt);

                            // 로그인 된 유저인지 확인
                            if (guid == sender.getLoginUser().Guid)
                            {
                                if (passengerCnt == accessPassengerCnt)
                                {
                                    sender.SendResponsePassengerCount(userId, Client.PKT_ACK);
                                }
                                else
                                {
                                    sender.SendResponsePassengerCount(userId, Client.PKT_NACK);
                                }
                            } else
                            {
                                sender.SendResponsePassengerCount(userId, Client.PKT_NACK);
                            }
                        } catch (Exception ee) {
                            sender.SendError(sender.getLoginUser().Id, StringUtil.StringToByte(ee.Message));
                        }
                    }
                    break;
                case Commands.ORDER:
                    {
                        try
                        {
                            int userId = r.ReadInt32();
                            int response = r.ReadInt32();
                            int dataLen = r.ReadInt32();
                            string guid = BBStringConverter.ByteToString(r.ReadBytes(dataLen));
                            AddListBoxItem("[ORDER] userId / dataLen / guid");
                            AddListBoxItem(userId + " / " + dataLen + " / " + guid);
                            /*
                             * DB 에서 Guid로 검색해 미리 입력해 놓은 지시서를 가져와야 함
                             */
                            byte[] orderData = new byte[0];
                            if (guid == sender.getLoginUser().Guid)
                            {
                                orderData = BBImageConverter.ImageToByte(Image.FromFile(@"C:\Users\gaonsoft\Downloads\항만보안\order.jpg", true));
                                sender.SendResponseOrder(userId, Client.PKT_ACK, orderData);
                            }
                            else
                            {
                                sender.SendResponseOrder(userId, Client.PKT_NACK, orderData);
                            }
                        }
                        catch (Exception ee)
                        {
                            sender.SendError(sender.getLoginUser().Id, StringUtil.StringToByte(ee.Message));
                        }
                    }
                    break;
            }
        }

        public static string ShowInputDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void btnEnroll_Click(object sender, EventArgs e)
        {
            ManagerDialog eDlg = new ManagerDialog();
            eDlg.ShowDialog();
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
                            byte[] iBytes = BBImageConverter.ImageToByte(BBImageConverter.GrayRawToBitmap(fingerSensor.getRawImage(), 320, 240));
                            UserManager fpm = new UserManager();
                            MyPerson guest = fpm.Enroll(iBytes, "guest");
                            MyPerson match = fpm.recognition(guest);

                            pbImage.Image = Image.FromStream(new MemoryStream(iBytes));
                            if (match != null)
                            {
                                tbId.Text = match.Id.ToString();
                                tbName.Text = match.Name.ToString();
                                pbFPRef.Image = match.Fingerprints[0].AsBitmap;
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

        private void UpdateStatusMessage(String msg)
        {
            Console.WriteLine(msg);
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = msg;
            });
        }

        private void AddListBoxItem(String msg)
        {
            Console.WriteLine(msg);
            Invoke((MethodInvoker)delegate
            {
                lstText.Items.Add(msg);
            });
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
            AddListBoxItem("psg count: " + new AccessInfoDB().SelectAccessPsgCnt("e3135a87-2c34-485e-9070-1352b7ec31c8"));
        }
    }
}
