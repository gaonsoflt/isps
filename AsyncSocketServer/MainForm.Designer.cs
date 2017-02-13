namespace AsyncSocketServer
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.lstText = new System.Windows.Forms.ListBox();
            this.btnListen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pbFPRef = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbId = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbGuid = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPhone = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnEnroll = new System.Windows.Forms.Button();
            this.cbConnetType = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbComport = new System.Windows.Forms.ComboBox();
            this.cbRate = new System.Windows.Forms.ComboBox();
            this.btnSerialClose = new System.Windows.Forms.Button();
            this.btnSerialOpen = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFPRef)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbImage
            // 
            this.pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbImage.Location = new System.Drawing.Point(8, 36);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(320, 240);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 0;
            this.pbImage.TabStop = false;
            // 
            // lstText
            // 
            this.lstText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstText.FormattingEnabled = true;
            this.lstText.HorizontalScrollbar = true;
            this.lstText.ItemHeight = 12;
            this.lstText.Location = new System.Drawing.Point(16, 28);
            this.lstText.Name = "lstText";
            this.lstText.ScrollAlwaysVisible = true;
            this.lstText.Size = new System.Drawing.Size(952, 400);
            this.lstText.TabIndex = 1;
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(13, 20);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(161, 37);
            this.btnListen.TabIndex = 2;
            this.btnListen.Text = "START";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // btnClose
            // 
            this.btnClose.Enabled = false;
            this.btnClose.Location = new System.Drawing.Point(13, 63);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(161, 37);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "STOP";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Label2.ForeColor = System.Drawing.Color.DarkOrange;
            this.Label2.Location = new System.Drawing.Point(14, 9);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(72, 12);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Client Log";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.SteelBlue;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "지문(Client)";
            // 
            // pbFPRef
            // 
            this.pbFPRef.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbFPRef.Location = new System.Drawing.Point(334, 36);
            this.pbFPRef.MaximumSize = new System.Drawing.Size(320, 240);
            this.pbFPRef.MinimumSize = new System.Drawing.Size(320, 240);
            this.pbFPRef.Name = "pbFPRef";
            this.pbFPRef.Size = new System.Drawing.Size(320, 240);
            this.pbFPRef.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFPRef.TabIndex = 6;
            this.pbFPRef.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.SteelBlue;
            this.label3.Location = new System.Drawing.Point(332, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "지문(DB_Reference)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.DarkOrange;
            this.label4.Location = new System.Drawing.Point(658, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "아이디";
            // 
            // tbId
            // 
            this.tbId.Location = new System.Drawing.Point(660, 36);
            this.tbId.Name = "tbId";
            this.tbId.ReadOnly = true;
            this.tbId.Size = new System.Drawing.Size(204, 21);
            this.tbId.TabIndex = 10;
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(660, 80);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(204, 21);
            this.tbName.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.DarkOrange;
            this.label5.Location = new System.Drawing.Point(658, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "이름";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tbGuid);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.tbPhone);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbId);
            this.groupBox1.Controls.Add(this.pbFPRef);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.pbImage);
            this.groupBox1.Location = new System.Drawing.Point(16, 434);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(954, 283);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "유저정보";
            // 
            // tbGuid
            // 
            this.tbGuid.Location = new System.Drawing.Point(662, 181);
            this.tbGuid.Name = "tbGuid";
            this.tbGuid.Size = new System.Drawing.Size(204, 21);
            this.tbGuid.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.DarkOrange;
            this.label7.Location = new System.Drawing.Point(660, 165);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "GUID";
            // 
            // tbPhone
            // 
            this.tbPhone.Location = new System.Drawing.Point(660, 131);
            this.tbPhone.Name = "tbPhone";
            this.tbPhone.Size = new System.Drawing.Size(204, 21);
            this.tbPhone.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.DarkOrange;
            this.label6.Location = new System.Drawing.Point(658, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "연락처";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.btnConfirm);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.btnEnroll);
            this.groupBox2.Controls.Add(this.cbConnetType);
            this.groupBox2.Controls.Add(this.btnListen);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.btnClose);
            this.groupBox2.Location = new System.Drawing.Point(976, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(186, 696);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Enabled = false;
            this.btnConfirm.Location = new System.Drawing.Point(13, 652);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(161, 37);
            this.btnConfirm.TabIndex = 5;
            this.btnConfirm.Text = "사용자 확인";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 609);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 37);
            this.button1.TabIndex = 17;
            this.button1.Text = "TEST";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnEnroll
            // 
            this.btnEnroll.Location = new System.Drawing.Point(13, 296);
            this.btnEnroll.Name = "btnEnroll";
            this.btnEnroll.Size = new System.Drawing.Size(161, 37);
            this.btnEnroll.TabIndex = 4;
            this.btnEnroll.Text = "시스템관리";
            this.btnEnroll.UseVisualStyleBackColor = true;
            this.btnEnroll.Click += new System.EventHandler(this.btnEnroll_Click);
            // 
            // cbConnetType
            // 
            this.cbConnetType.AutoSize = true;
            this.cbConnetType.Location = new System.Drawing.Point(12, 106);
            this.cbConnetType.Name = "cbConnetType";
            this.cbConnetType.Size = new System.Drawing.Size(84, 16);
            this.cbConnetType.TabIndex = 17;
            this.cbConnetType.Text = "Serial 포함";
            this.cbConnetType.UseVisualStyleBackColor = true;
            this.cbConnetType.CheckedChanged += new System.EventHandler(this.cbConnetType_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbComport);
            this.groupBox3.Controls.Add(this.cbRate);
            this.groupBox3.Controls.Add(this.btnSerialClose);
            this.groupBox3.Controls.Add(this.btnSerialOpen);
            this.groupBox3.Location = new System.Drawing.Point(6, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(173, 162);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Serial";
            // 
            // cbComport
            // 
            this.cbComport.FormattingEnabled = true;
            this.cbComport.Location = new System.Drawing.Point(7, 20);
            this.cbComport.Name = "cbComport";
            this.cbComport.Size = new System.Drawing.Size(160, 20);
            this.cbComport.TabIndex = 3;
            // 
            // cbRate
            // 
            this.cbRate.FormattingEnabled = true;
            this.cbRate.Location = new System.Drawing.Point(7, 46);
            this.cbRate.Name = "cbRate";
            this.cbRate.Size = new System.Drawing.Size(160, 20);
            this.cbRate.TabIndex = 2;
            // 
            // btnSerialClose
            // 
            this.btnSerialClose.Enabled = false;
            this.btnSerialClose.Location = new System.Drawing.Point(7, 117);
            this.btnSerialClose.Name = "btnSerialClose";
            this.btnSerialClose.Size = new System.Drawing.Size(161, 37);
            this.btnSerialClose.TabIndex = 1;
            this.btnSerialClose.Text = "CLOSE";
            this.btnSerialClose.UseVisualStyleBackColor = true;
            this.btnSerialClose.Click += new System.EventHandler(this.btnSerialClose_Click);
            // 
            // btnSerialOpen
            // 
            this.btnSerialOpen.Enabled = false;
            this.btnSerialOpen.Location = new System.Drawing.Point(6, 74);
            this.btnSerialOpen.Name = "btnSerialOpen";
            this.btnSerialOpen.Size = new System.Drawing.Size(161, 37);
            this.btnSerialOpen.TabIndex = 0;
            this.btnSerialOpen.Text = "OPEN";
            this.btnSerialOpen.UseVisualStyleBackColor = true;
            this.btnSerialOpen.Click += new System.EventHandler(this.btnSerialOpen_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 723);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1178, 22);
            this.statusStrip1.TabIndex = 15;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(16, 17);
            this.toolStripStatusLabel1.Text = "...";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 582);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(161, 21);
            this.textBox1.TabIndex = 17;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1178, 745);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.lstText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFPRef)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.ListBox lstText;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbFPRef;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbId;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSerialClose;
        private System.Windows.Forms.Button btnSerialOpen;
        private System.Windows.Forms.ComboBox cbComport;
        private System.Windows.Forms.ComboBox cbRate;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.Button btnEnroll;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.CheckBox cbConnetType;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TextBox tbGuid;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPhone;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
    }
}

