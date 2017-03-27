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
using static AsyncSocketServer.DataPacket;
using AsyncSocketServer;

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
            client = new Client();
            client.OnConnect += new Client.OnConnectEventHandler(client_OnConnect);
            client.OnSend += new Client.OnSendEventHandler(client_OnSend);
            client.OnDisconnect += new Client.OnDisconnectEventHandler(client_OnDisconnect);
            client.OnDisconnectByServer += new Client.OnDisconnectByServerEventHandler(client_OnDisconnectByServer);
            client.DataReceived += new Client.DataReceivedEventHandler(client_DataReceived);

            fingerSensor = FingerSensor.GetFingerSensorInstance();

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
            btnDisconnect.Enabled = false;

            UpdateStatusMessage("Disconnected", "");
        }

        void client_OnDisconnect(Client sender)
        {
            UpdateStatusMessage("Disconnected");
        }

        void client_OnRecevice(Client sender, String text)
        {
            UpdateStatusMessage(text);
        }

        void client_OnSend(Client sender, int sent)
        {
            UpdateStatusMessage(null, string.Format("Data Sent:{0}", sent));
        }

        void client_OnConnect(Client sender, bool connected)
        {
            if (connected)
                UpdateStatusMessage("Connected");
        }

        void client_DataReceived(Client sender, ReceiveBuffer e)
        {
            Packet pkt = DataPacket.DataParser(e.BufStream, e.pkt);
            UpdateCompLogMsg("Received data: " + pkt.ToString());
            switch (pkt.type)
            {
                case PktType.AUTH:
                    if (pkt.response == PKT_ACK)
                    {
                        UpdateStatusMessage("Success login.", pkt.guid);
                        guid = pkt.guid;
                        UpdateCompLogMsg(pkt.guid);
                    }
                    else
                    {
                        UpdateStatusMessage("Failed login.", pkt.errMsg);
                    }
                    break;
                case PktType.PASSENGER:
                    if (pkt.response == PKT_ACK)
                    {
                        UpdateStatusMessage("Success passenger count.", pkt.accessId.ToString());
                        accessId = pkt.accessId;
                        UpdateCompLogMsg(pkt.accessId.ToString());
                    }
                    else
                    {
                        UpdateStatusMessage("Failed passenger count.", pkt.errMsg);
                    }
                    break;
                case PktType.ORDER:
                    if (pkt.response == PKT_ACK)
                    {
                        UpdateStatusMessage("Success receive order.", pkt.order.orderId);
                        UpdateCompLogMsg(pkt.order.orderId);
                    }
                    else
                    {
                        UpdateStatusMessage("Failed receive order.", pkt.errMsg);
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
                    EnableSerialComponent(false);
                    if (cbSerial.Checked) OpenSerialPort();
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

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                client.Disconnect();
                EnableSocketComponent(false);

                if (cbSerial.Checked) CloseSerialPort();
                UpdateStatusMessage("Disconnected", "");
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

        private void UpdateStatusMessage(String msg)
        {
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = msg;
            });
        }

        private void UpdateStatusMessage(String msg1, String msg2)
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
                UpdateStatusMessage("시리얼 포트가 해제되었습니다.");
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

        private void EnableSocketComponent(bool isConnect)
        {
            btnConnect.Enabled = !isConnect;
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
            btnPassenger.Enabled = enable;
            btnReqOrder.Enabled = enable;
        }

        public string guid;
        public int accessId;

        private int GetUserId()
        {
            int userId;
            try
            {
                userId = Int32.Parse(tbUserId.Text);
            }
            catch
            {
                userId = 0;
            }
            return userId;
        }

        private int GetPsgCnt()
        {
            int psgCnt;
            try
            {
                psgCnt = Int32.Parse(tbPsgCnt.Text);
            }
            catch
            {
                psgCnt = 0;
            }
            return psgCnt;
        }

        private void btnAuth_Click(object sender, EventArgs e)
        {
            try
            {
                btnAuth.Enabled = false;
                if (cbSerial.Checked)
                {
                    if (fingerSensor.CmdCmosLed(true) == 0)
                    {
                        UpdateStatusMessage("Input your finger on sensor.");
                        if (fingerSensor.CmdCaptureFinger() == 0)
                        {
                            UpdateStatusMessage("Exporting deleted fingerprint data");
                            if (fingerSensor.CmdGetRawImage() == 0)
                            {
                                UpdateStatusMessage("Succeed export fingerprint data.");
                                //byte[] iBytes = ImageUtil.ImageToByte(BBDataConverter.GrayRawToBitmap(fingerSensor.getRawImage(), 320, 240));
                                pictureBox1.Image = BBDataConverter.GrayRawToBitmap(fingerSensor.getRawImage160x120(), FingerSensorPacket.SIZE_FP_WIDTH, FingerSensorPacket.SIZE_FP_HEIGHT);

                                // send data
                                client.SendAuthUserByFingerPrint(GetUserId(), tbCarId.Text, pictureBox1.Image);
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
                } else
                {
                    client.SendAuthUserByFingerPrint(GetUserId(), tbCarId.Text, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                UpdateStatusMessage("Failed export fingerparint data.");
            }
            finally
            {
                if (cbSerial.Checked) fingerSensor.CmdCmosLed(false);
                btnAuth.Enabled = true;
            }
        }

        private void btnPassenger_Click(object sender, EventArgs e)
        {
            client.SendPassengerCount(GetUserId(), tbCarId.Text, guid, GetPsgCnt());
        }

        private void btnReqOrder_Click(object sender, EventArgs e)
        {
            client.SendRequestOrder(GetUserId(), tbCarId.Text, guid, accessId);
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

        private void UpdateCompLogMsg(String msg)
        {
            Console.WriteLine(msg);
            if (lstText.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lstText.Items.Add(msg);
                });
            }
            else
            {
                lstText.Items.Add(msg);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client.Connected)
            {
                client.Disconnect();
            }
            if (fingerSensor.sPort != null && fingerSensor.sPort.IsOpen)
            {
                CloseSerialPort();
            }
        }
    }
}
