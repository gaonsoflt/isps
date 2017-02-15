namespace AsyncSocketServer
{
    partial class AccessDialog
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
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbUserId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.allowStartDt = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.allowEndDt = new System.Windows.Forms.DateTimePicker();
            this.nudPsgCnt = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSeq = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.tbUserNm = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbIsAccess = new System.Windows.Forms.TextBox();
            this.tbAccessDt = new System.Windows.Forms.TextBox();
            this.tbAccessMin = new System.Windows.Forms.TextBox();
            this.tbAccessHour = new System.Windows.Forms.TextBox();
            this.tbAccessDay = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbPurpose = new System.Windows.Forms.TextBox();
            this.btnOrder = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.tbOrderId = new System.Windows.Forms.TextBox();
            this.cbCarId = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudPsgCnt)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(14, 396);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(154, 38);
            this.btnCancel.TabIndex = 35;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(174, 396);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(154, 38);
            this.btnApply.TabIndex = 40;
            this.btnApply.Text = "저장";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.DarkOrange;
            this.label4.Location = new System.Drawing.Point(12, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "아이디";
            // 
            // tbUserId
            // 
            this.tbUserId.Location = new System.Drawing.Point(14, 74);
            this.tbUserId.Name = "tbUserId";
            this.tbUserId.ReadOnly = true;
            this.tbUserId.Size = new System.Drawing.Size(152, 21);
            this.tbUserId.TabIndex = 19;
            this.tbUserId.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.DarkOrange;
            this.label5.Location = new System.Drawing.Point(172, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 12);
            this.label5.TabIndex = 20;
            this.label5.Text = "동승자";
            // 
            // allowStartDt
            // 
            this.allowStartDt.Location = new System.Drawing.Point(14, 221);
            this.allowStartDt.Name = "allowStartDt";
            this.allowStartDt.Size = new System.Drawing.Size(229, 21);
            this.allowStartDt.TabIndex = 4;
            this.allowStartDt.Value = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.allowStartDt.ValueChanged += new System.EventHandler(this.allowStartDt_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.DarkOrange;
            this.label1.Location = new System.Drawing.Point(12, 206);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 23;
            this.label1.Text = "출입시작일시";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.DarkOrange;
            this.label2.Location = new System.Drawing.Point(12, 254);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 24;
            this.label2.Text = "출입종료일시";
            // 
            // allowEndDt
            // 
            this.allowEndDt.Location = new System.Drawing.Point(14, 269);
            this.allowEndDt.Name = "allowEndDt";
            this.allowEndDt.Size = new System.Drawing.Size(229, 21);
            this.allowEndDt.TabIndex = 5;
            this.allowEndDt.Value = new System.DateTime(2017, 1, 20, 15, 27, 9, 0);
            this.allowEndDt.ValueChanged += new System.EventHandler(this.allowEndDt_ValueChanged);
            // 
            // nudPsgCnt
            // 
            this.nudPsgCnt.Location = new System.Drawing.Point(174, 123);
            this.nudPsgCnt.Name = "nudPsgCnt";
            this.nudPsgCnt.Size = new System.Drawing.Size(152, 21);
            this.nudPsgCnt.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.DarkOrange;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 12);
            this.label3.TabIndex = 27;
            this.label3.Text = "SEQ";
            // 
            // tbSeq
            // 
            this.tbSeq.Location = new System.Drawing.Point(14, 25);
            this.tbSeq.Name = "tbSeq";
            this.tbSeq.ReadOnly = true;
            this.tbSeq.Size = new System.Drawing.Size(152, 21);
            this.tbSeq.TabIndex = 28;
            this.tbSeq.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 445);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(338, 22);
            this.statusStrip1.TabIndex = 29;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(16, 17);
            this.toolStripStatusLabel1.Text = "...";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.DarkOrange;
            this.label6.Location = new System.Drawing.Point(172, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 12);
            this.label6.TabIndex = 30;
            this.label6.Text = "이름";
            // 
            // tbUserNm
            // 
            this.tbUserNm.Location = new System.Drawing.Point(174, 74);
            this.tbUserNm.Name = "tbUserNm";
            this.tbUserNm.ReadOnly = true;
            this.tbUserNm.Size = new System.Drawing.Size(152, 21);
            this.tbUserNm.TabIndex = 31;
            this.tbUserNm.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.DarkOrange;
            this.label7.Location = new System.Drawing.Point(12, 302);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 12);
            this.label7.TabIndex = 32;
            this.label7.Text = "출입여부";
            // 
            // tbIsAccess
            // 
            this.tbIsAccess.Location = new System.Drawing.Point(14, 318);
            this.tbIsAccess.Name = "tbIsAccess";
            this.tbIsAccess.ReadOnly = true;
            this.tbIsAccess.Size = new System.Drawing.Size(81, 21);
            this.tbIsAccess.TabIndex = 33;
            this.tbIsAccess.TabStop = false;
            // 
            // tbAccessDt
            // 
            this.tbAccessDt.Location = new System.Drawing.Point(96, 318);
            this.tbAccessDt.Name = "tbAccessDt";
            this.tbAccessDt.ReadOnly = true;
            this.tbAccessDt.Size = new System.Drawing.Size(230, 21);
            this.tbAccessDt.TabIndex = 34;
            this.tbAccessDt.TabStop = false;
            // 
            // tbAccessMin
            // 
            this.tbAccessMin.Location = new System.Drawing.Point(6, 67);
            this.tbAccessMin.Name = "tbAccessMin";
            this.tbAccessMin.ReadOnly = true;
            this.tbAccessMin.Size = new System.Drawing.Size(65, 21);
            this.tbAccessMin.TabIndex = 35;
            this.tbAccessMin.TabStop = false;
            this.tbAccessMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbAccessHour
            // 
            this.tbAccessHour.Location = new System.Drawing.Point(6, 43);
            this.tbAccessHour.Name = "tbAccessHour";
            this.tbAccessHour.ReadOnly = true;
            this.tbAccessHour.Size = new System.Drawing.Size(65, 21);
            this.tbAccessHour.TabIndex = 36;
            this.tbAccessHour.TabStop = false;
            this.tbAccessHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbAccessDay
            // 
            this.tbAccessDay.Location = new System.Drawing.Point(6, 19);
            this.tbAccessDay.Name = "tbAccessDay";
            this.tbAccessDay.ReadOnly = true;
            this.tbAccessDay.Size = new System.Drawing.Size(65, 21);
            this.tbAccessDay.TabIndex = 37;
            this.tbAccessDay.TabStop = false;
            this.tbAccessDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbAccessHour);
            this.groupBox1.Controls.Add(this.tbAccessDay);
            this.groupBox1.Controls.Add(this.tbAccessMin);
            this.groupBox1.Location = new System.Drawing.Point(249, 206);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(77, 100);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "허용시간";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.DarkOrange;
            this.label8.Location = new System.Drawing.Point(12, 107);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 12);
            this.label8.TabIndex = 39;
            this.label8.Text = "차량번호";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.ForeColor = System.Drawing.Color.DarkOrange;
            this.label9.Location = new System.Drawing.Point(10, 155);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 12);
            this.label9.TabIndex = 41;
            this.label9.Text = "출입목적";
            // 
            // tbPurpose
            // 
            this.tbPurpose.Location = new System.Drawing.Point(12, 171);
            this.tbPurpose.Name = "tbPurpose";
            this.tbPurpose.Size = new System.Drawing.Size(314, 21);
            this.tbPurpose.TabIndex = 3;
            this.tbPurpose.TabStop = false;
            // 
            // btnOrder
            // 
            this.btnOrder.Location = new System.Drawing.Point(172, 352);
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(154, 38);
            this.btnOrder.TabIndex = 30;
            this.btnOrder.Text = "작업지시서";
            this.btnOrder.UseVisualStyleBackColor = true;
            this.btnOrder.Click += new System.EventHandler(this.btnOrder_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.ForeColor = System.Drawing.Color.DarkOrange;
            this.label10.Location = new System.Drawing.Point(12, 353);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 12);
            this.label10.TabIndex = 44;
            this.label10.Text = "작업지시서";
            // 
            // tbOrderId
            // 
            this.tbOrderId.Location = new System.Drawing.Point(14, 369);
            this.tbOrderId.Name = "tbOrderId";
            this.tbOrderId.ReadOnly = true;
            this.tbOrderId.Size = new System.Drawing.Size(152, 21);
            this.tbOrderId.TabIndex = 45;
            this.tbOrderId.TabStop = false;
            // 
            // cbCarId
            // 
            this.cbCarId.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbCarId.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbCarId.FormattingEnabled = true;
            this.cbCarId.Location = new System.Drawing.Point(12, 123);
            this.cbCarId.Name = "cbCarId";
            this.cbCarId.Size = new System.Drawing.Size(154, 20);
            this.cbCarId.TabIndex = 46;
            // 
            // timer1
            // 
            this.timer1.Interval = 1500;
            // 
            // AccessDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 467);
            this.Controls.Add(this.cbCarId);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbOrderId);
            this.Controls.Add(this.btnOrder);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbPurpose);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbAccessDt);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbIsAccess);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbUserNm);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbSeq);
            this.Controls.Add(this.nudPsgCnt);
            this.Controls.Add(this.allowEndDt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.allowStartDt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbUserId);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.MaximumSize = new System.Drawing.Size(354, 506);
            this.MinimumSize = new System.Drawing.Size(354, 506);
            this.Name = "AccessDialog";
            this.Text = "출입관리";
            this.Load += new System.EventHandler(this.AccessDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudPsgCnt)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbUserId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker allowStartDt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker allowEndDt;
        private System.Windows.Forms.NumericUpDown nudPsgCnt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSeq;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbUserNm;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbIsAccess;
        private System.Windows.Forms.TextBox tbAccessDt;
        private System.Windows.Forms.TextBox tbAccessMin;
        private System.Windows.Forms.TextBox tbAccessHour;
        private System.Windows.Forms.TextBox tbAccessDay;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbPurpose;
        private System.Windows.Forms.Button btnOrder;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbOrderId;
        private System.Windows.Forms.ComboBox cbCarId;
        private System.Windows.Forms.Timer timer1;
    }
}