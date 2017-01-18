using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace AsyncSocketClient
{
    public partial class ClientForm : Form
    {
        Client client;
        FingerSensor fingerSensor;

        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            fingerSensor = new FingerSensor(this);
            client = new Client();
            client.OnConnect += new Client.OnConnectEventHandler(client_OnConnect);
            client.OnSend += new Client.OnSendEventHandler(client_OnSend);
            client.OnDisconnect += new Client.OnDisconnectEventHandler(client_OnDisconnect);
            client.OnDisconnectByServer += new Client.OnDisconnectByServerEventHandler(client_OnDisconnectByServer);
            client.DataReceived += new Client.DataReceivedEventHandler(client_DataReceived);

            string[] arrPort = { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9" };
            string[] arrRate = { "2400", "4800", "9600", "14400", "19200", "38400", "57600", "115200" };
            cbComport.Items.AddRange(arrPort);
            cbComport.SelectedIndex = 2;
            cbRate.Items.AddRange(arrRate);
            cbRate.SelectedIndex = 2;
        }

        void client_OnDisconnectByServer(Client sender)
        {
            MessageBox.Show("서버로부터 연결이 끊겼습니다.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Stop);
 
            btnConnect.Enabled = true;
            btnSendText.Enabled = false;
            btnRecognition.Enabled = false;
            btnEnrollment.Enabled = false;
            btnDisconnect.Enabled = false;

            StatusMessage("Disconnected", "");
        }

        void client_OnDisconnect(Client sender)
        {
            StatusMessage("Disconnected");
        }

        void client_OnRecevice(Client sender, String text)
        {
            StatusMessage(text);
        }

        void client_OnSend(Client sender, int sent)
        {
            StatusMessage(null, string.Format("Data Sent:{0}", sent));
        }

        void client_OnConnect(Client sender, bool connected)
        {
            if (connected)
                StatusMessage("Connected");
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
                        StatusMessage(null, "receviced: " + s);
                    }
                    break;
                case Commands.AUTH:
                    {
                        int userId = r.ReadInt32();
                        int response = r.ReadInt32();
                        int dataLen = r.ReadInt32();
                        if(response == Client.PKT_ACK)
                        {
                            byte[] iBytes = r.ReadBytes(dataLen);
                            userGuid = StringUtil.ByteToString(iBytes);
                            StatusMessage("Success login.", userGuid);
                            btnPassenger.Enabled = true;
                        } else
                        {
                            StatusMessage("Failed login.", "");
                        }
                    }
                    break;
                case Commands.PASSENGER:
                    {
                        int userId = r.ReadInt32();
                        int response = r.ReadInt32();
                        int dataLen = r.ReadInt32();
                        if (response == Client.PKT_ACK)
                        {
                            StatusMessage("Success passenger count.");
                            btnReqOrder.Enabled = true;
                        }
                        else
                        {
                            StatusMessage("Failed passenger count.");
                        }
                    }
                    break;
                case Commands.ORDER:
                    {
                        int userId = r.ReadInt32();
                        int response = r.ReadInt32();
                        int dataLen = r.ReadInt32();
                        if (response == Client.PKT_ACK)
                        {
                            byte[] iBytes = r.ReadBytes(dataLen);
                            Invoke((MethodInvoker)delegate
                            {
                                pictureBox2.Image = Image.FromStream(new MemoryStream(iBytes));
                            });
                            StatusMessage("Success receive order.");
                            btnPassenger.Enabled = false;
                            btnReqOrder.Enabled = false;
                        }
                        else
                        {
                            StatusMessage("Failed receive order.");
                        }
                    }
                    break;

            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            { 
                if (!client.Connected)
                {
                    client.Connect(tbIp.Text, Int32.Parse(tbPort.Text));

                    EnableSocketComponent(true);

                    OpenSerialPort();
                }
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSendText_Click(object sender, EventArgs e)
        {
            if (client.Connected)
                client.SendText(textBox1.Text);
            else
                StatusMessage("서버 접속을 하시기 바랍니다. !!!");
        }

        

        private void btnEnrollment_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                using (OpenFileDialog o = new OpenFileDialog())
                {
                    o.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.tif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.tif; *.png; *.bmp";
                    o.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        client.SendEnrollUser(o.FileName);
                    }
                }
            }
        }

        private void btnRecognition_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                using (OpenFileDialog o = new OpenFileDialog())
                {
                    o.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.tif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.tif; *.png; *.bmp";
                    o.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        client.SendRecogFingerPrint(o.FileName);
                    }
                }
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                client.Disconnect();
                EnableSocketComponent(false);

                CloseSerialPort();
                StatusMessage("Disconnected", "");
                btnAuth.Enabled = false;
                btnPassenger.Enabled = false;
                btnReqOrder.Enabled = false;
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

        private void StatusMessage(String msg)
        {
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = msg;
            });
        }

        private void StatusMessage(String msg1, String msg2)
        {
            Invoke((MethodInvoker)delegate
            {
                if (msg1 != null) toolStripStatusLabel1.Text = msg1;
                if (msg2 != null) toolStripStatusLabel2.Text = msg2;
            });
        }

        private void CloseSerialPort()
        {
            if (fingerSensor.CmdClose() == 0)
            {
                fingerSensor.CmdChangeBaudrate(9600);
                fingerSensor.CloseSerialPort();
                EnableSerialComponent(true);
                StatusMessage("시리얼 포트가 해제되었습니다.");
            }
        }

        private void OpenSerialPort()
        {
            cbRate.SelectedIndex = 2;
            OpenSerial();
            cbRate.SelectedIndex = 7;
            if (fingerSensor.CmdChangeBaudrate(Convert.ToInt32(cbRate.SelectedItem.ToString())) == 0)
            {
                fingerSensor.CloseSerialPort();
                OpenSerial();
                fingerSensor.CmdOpen();
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
                    StatusMessage("시리얼 포트가 연결되었습니다.");
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

        private void EnableSocketComponent(bool isConnect)
        {
            btnConnect.Enabled = !isConnect;
            btnSendText.Enabled = isConnect;
            btnRecognition.Enabled = isConnect;
            btnEnrollment.Enabled = isConnect;
            btnDisconnect.Enabled = isConnect;
            tbIp.Enabled = !isConnect;
            tbPort.Enabled = !isConnect;
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
            btnAuth.Enabled = enable;
            btnPassenger.Enabled = !enable;
            btnReqOrder.Enabled = !enable;
        }

        public int userId = 3;
        public String userGuid;

        private void btnAuth_Click(object sender, EventArgs e)
        {
            try
            {
                EnableFingerPrintComponent(false);
                if (fingerSensor.CmdCmosLed(true) == 0)
                {
                    StatusMessage("Input your finger on sensor.");
                    if (fingerSensor.CmdCaptureFinger() == 0)
                    {
                        StatusMessage("Exporting deleted fingerprint data");
                        if (fingerSensor.CmdGetRawImage() == 0)
                        {
                            StatusMessage("Succeed export fingerprint data.");
                            byte[] iBytes = ImageUtil.ImageToByte(ImageUtil.ConvertGrayRawToBitmap(fingerSensor.getRawImage(), 320, 240));
                            pictureBox1.Image = Image.FromStream(new MemoryStream(iBytes));

                            try
                            {
                                userId = Int32.Parse(tbUserId.Text);
                            } catch
                            {
                                userId = 0;
                            }
                            // send data
                            client.SendAuthUserByFingerPrint(userId, iBytes);
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
                EnableFingerPrintComponent(true);
            }
        }

        private void btnPassenger_Click(object sender, EventArgs e)
        {
            /*
             * 동승자 판별하여 몇명인지 계산후 전송
             */
            int passengerCnt = 0;
            try
            {
                passengerCnt = Int32.Parse(tbPsgCnt.Text);
            }
            catch
            {
                passengerCnt = 0;
            }
            client.SendPassengerCount(userId, userGuid, passengerCnt);
        }

        private void btnReqOrder_Click(object sender, EventArgs e)
        {
            client.SendRequestOrder(userId, userGuid);
        }

        private void tbUserId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void tbPsgCnt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }
    }
}
