namespace AsyncSocketClient
{
    partial class ClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm));
            this.btnConnect = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbIp = new System.Windows.Forms.TextBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbComport = new System.Windows.Forms.ComboBox();
            this.cbRate = new System.Windows.Forms.ComboBox();
            this.btnSerialClose = new System.Windows.Forms.Button();
            this.btnSerialOpen = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tbPsgCnt = new System.Windows.Forms.TextBox();
            this.tbUserId = new System.Windows.Forms.TextBox();
            this.btnReqOrder = new System.Windows.Forms.Button();
            this.btnPassenger = new System.Windows.Forms.Button();
            this.btnAuth = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lstText = new System.Windows.Forms.ListBox();
            this.tbCarId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbSerial = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(3, 30);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(254, 34);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tbIp);
            this.panel1.Controls.Add(this.tbPort);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.btnDisconnect);
            this.panel1.Controls.Add(this.btnConnect);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 253);
            this.panel1.TabIndex = 1;
            // 
            // tbIp
            // 
            this.tbIp.Location = new System.Drawing.Point(4, 3);
            this.tbIp.Name = "tbIp";
            this.tbIp.Size = new System.Drawing.Size(166, 21);
            this.tbIp.TabIndex = 7;
            this.tbIp.Text = "127.0.0.1";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(176, 3);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(81, 21);
            this.tbPort.TabIndex = 6;
            this.tbPort.Text = "8192";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbSerial);
            this.groupBox1.Controls.Add(this.cbComport);
            this.groupBox1.Controls.Add(this.cbRate);
            this.groupBox1.Controls.Add(this.btnSerialClose);
            this.groupBox1.Controls.Add(this.btnSerialOpen);
            this.groupBox1.Location = new System.Drawing.Point(4, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(253, 137);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Serial";
            // 
            // cbComport
            // 
            this.cbComport.FormattingEnabled = true;
            this.cbComport.Location = new System.Drawing.Point(67, 20);
            this.cbComport.Name = "cbComport";
            this.cbComport.Size = new System.Drawing.Size(89, 20);
            this.cbComport.TabIndex = 7;
            // 
            // cbRate
            // 
            this.cbRate.FormattingEnabled = true;
            this.cbRate.Location = new System.Drawing.Point(162, 20);
            this.cbRate.Name = "cbRate";
            this.cbRate.Size = new System.Drawing.Size(85, 20);
            this.cbRate.TabIndex = 6;
            // 
            // btnSerialClose
            // 
            this.btnSerialClose.Enabled = false;
            this.btnSerialClose.Location = new System.Drawing.Point(6, 89);
            this.btnSerialClose.Name = "btnSerialClose";
            this.btnSerialClose.Size = new System.Drawing.Size(241, 37);
            this.btnSerialClose.TabIndex = 5;
            this.btnSerialClose.Text = "CLOSE";
            this.btnSerialClose.UseVisualStyleBackColor = true;
            this.btnSerialClose.Click += new System.EventHandler(this.btnSerialClose_Click);
            // 
            // btnSerialOpen
            // 
            this.btnSerialOpen.Enabled = false;
            this.btnSerialOpen.Location = new System.Drawing.Point(6, 46);
            this.btnSerialOpen.Name = "btnSerialOpen";
            this.btnSerialOpen.Size = new System.Drawing.Size(241, 37);
            this.btnSerialOpen.TabIndex = 4;
            this.btnSerialOpen.Text = "OPEN";
            this.btnSerialOpen.UseVisualStyleBackColor = true;
            this.btnSerialOpen.Click += new System.EventHandler(this.btnSerialOpen_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(4, 70);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(254, 38);
            this.btnDisconnect.TabIndex = 4;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 599);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(1114, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(16, 17);
            this.toolStripStatusLabel1.Text = "...";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(1083, 17);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // tbPsgCnt
            // 
            this.tbPsgCnt.Location = new System.Drawing.Point(604, 161);
            this.tbPsgCnt.Name = "tbPsgCnt";
            this.tbPsgCnt.Size = new System.Drawing.Size(44, 21);
            this.tbPsgCnt.TabIndex = 12;
            this.tbPsgCnt.Text = "1";
            this.tbPsgCnt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPsgCnt_KeyPress);
            // 
            // tbUserId
            // 
            this.tbUserId.Location = new System.Drawing.Point(602, 76);
            this.tbUserId.Name = "tbUserId";
            this.tbUserId.Size = new System.Drawing.Size(44, 21);
            this.tbUserId.TabIndex = 11;
            this.tbUserId.Text = "3";
            this.tbUserId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbUserId_KeyPress);
            // 
            // btnReqOrder
            // 
            this.btnReqOrder.Enabled = false;
            this.btnReqOrder.Location = new System.Drawing.Point(599, 188);
            this.btnReqOrder.Name = "btnReqOrder";
            this.btnReqOrder.Size = new System.Drawing.Size(188, 33);
            this.btnReqOrder.TabIndex = 10;
            this.btnReqOrder.Text = "REQUEST ORDER";
            this.btnReqOrder.UseVisualStyleBackColor = true;
            this.btnReqOrder.Click += new System.EventHandler(this.btnReqOrder_Click);
            // 
            // btnPassenger
            // 
            this.btnPassenger.Enabled = false;
            this.btnPassenger.Location = new System.Drawing.Point(602, 103);
            this.btnPassenger.Name = "btnPassenger";
            this.btnPassenger.Size = new System.Drawing.Size(136, 33);
            this.btnPassenger.TabIndex = 9;
            this.btnPassenger.Text = "CHECK PASSENGER";
            this.btnPassenger.UseVisualStyleBackColor = true;
            this.btnPassenger.Click += new System.EventHandler(this.btnPassenger_Click);
            // 
            // btnAuth
            // 
            this.btnAuth.Enabled = false;
            this.btnAuth.Location = new System.Drawing.Point(602, 15);
            this.btnAuth.Name = "btnAuth";
            this.btnAuth.Size = new System.Drawing.Size(136, 33);
            this.btnAuth.TabIndex = 8;
            this.btnAuth.Text = "AUTHENTICATION";
            this.btnAuth.UseVisualStyleBackColor = true;
            this.btnAuth.Click += new System.EventHandler(this.btnAuth_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(799, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(303, 259);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // lstText
            // 
            this.lstText.FormattingEnabled = true;
            this.lstText.HorizontalScrollbar = true;
            this.lstText.ItemHeight = 12;
            this.lstText.Location = new System.Drawing.Point(12, 277);
            this.lstText.Name = "lstText";
            this.lstText.ScrollAlwaysVisible = true;
            this.lstText.Size = new System.Drawing.Size(1090, 316);
            this.lstText.TabIndex = 5;
            // 
            // tbCarId
            // 
            this.tbCarId.Location = new System.Drawing.Point(652, 76);
            this.tbCarId.Name = "tbCarId";
            this.tbCarId.Size = new System.Drawing.Size(86, 21);
            this.tbCarId.TabIndex = 13;
            this.tbCarId.Text = "12가1234";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(602, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "userid";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(650, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "carid";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(602, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "passenger count";
            // 
            // cbSerial
            // 
            this.cbSerial.AutoSize = true;
            this.cbSerial.Location = new System.Drawing.Point(7, 21);
            this.cbSerial.Name = "cbSerial";
            this.cbSerial.Size = new System.Drawing.Size(32, 16);
            this.cbSerial.TabIndex = 8;
            this.cbSerial.Text = "P";
            this.cbSerial.UseVisualStyleBackColor = true;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1114, 621);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbCarId);
            this.Controls.Add(this.tbPsgCnt);
            this.Controls.Add(this.lstText);
            this.Controls.Add(this.tbUserId);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnReqOrder);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnPassenger);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnAuth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ClientForm";
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.TextBox tbIp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbComport;
        private System.Windows.Forms.ComboBox cbRate;
        private System.Windows.Forms.Button btnSerialClose;
        private System.Windows.Forms.Button btnSerialOpen;
        private System.Windows.Forms.Button btnAuth;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnReqOrder;
        private System.Windows.Forms.Button btnPassenger;
        private System.Windows.Forms.TextBox tbPsgCnt;
        private System.Windows.Forms.TextBox tbUserId;
        private System.Windows.Forms.ListBox lstText;
        private System.Windows.Forms.TextBox tbCarId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbSerial;
    }
}