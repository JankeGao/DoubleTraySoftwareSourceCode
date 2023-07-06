namespace PLCServer
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.checkBoxAutoRun = new System.Windows.Forms.CheckBox();
            this.labelAutoRun = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.labelFilePath = new System.Windows.Forms.Label();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemShow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.BtnGetDeviceStatus = new System.Windows.Forms.Button();
            this.BtnGetAlarmInfo = new System.Windows.Forms.Button();
            this.BtnHopperSetting = new System.Windows.Forms.Button();
            this.BtnEmergencyDoorSetting = new System.Windows.Forms.Button();
            this.BtnResetAlarm = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.textTrayNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textXNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textPLCAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textPLCPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.BtnKardexTakeOut = new System.Windows.Forms.Button();
            this.BtnKardexTakeIn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.BtnStartContainer = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxAutoRun
            // 
            this.checkBoxAutoRun.AutoSize = true;
            this.checkBoxAutoRun.Location = new System.Drawing.Point(120, 148);
            this.checkBoxAutoRun.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxAutoRun.Name = "checkBoxAutoRun";
            this.checkBoxAutoRun.Size = new System.Drawing.Size(44, 19);
            this.checkBoxAutoRun.TabIndex = 12;
            this.checkBoxAutoRun.Text = "是";
            this.checkBoxAutoRun.UseVisualStyleBackColor = true;
            this.checkBoxAutoRun.CheckedChanged += new System.EventHandler(this.checkBoxAutoRun_CheckedChanged);
            // 
            // labelAutoRun
            // 
            this.labelAutoRun.AutoSize = true;
            this.labelAutoRun.Location = new System.Drawing.Point(16, 148);
            this.labelAutoRun.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAutoRun.Name = "labelAutoRun";
            this.labelAutoRun.Size = new System.Drawing.Size(82, 15);
            this.labelAutoRun.TabIndex = 11;
            this.labelAutoRun.Text = "开机启动：";
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(513, 69);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(100, 29);
            this.buttonStop.TabIndex = 14;
            this.buttonStop.Text = "断开连接";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonStart.Location = new System.Drawing.Point(513, 32);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(100, 29);
            this.buttonStart.TabIndex = 13;
            this.buttonStart.Text = "连接PLC";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(121, 35);
            this.textBoxFilePath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(368, 25);
            this.textBoxFilePath.TabIndex = 9;
            this.textBoxFilePath.TextChanged += new System.EventHandler(this.textBoxFilePath_TextChanged);
            // 
            // labelFilePath
            // 
            this.labelFilePath.AutoSize = true;
            this.labelFilePath.Location = new System.Drawing.Point(15, 39);
            this.labelFilePath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFilePath.Name = "labelFilePath";
            this.labelFilePath.Size = new System.Drawing.Size(82, 15);
            this.labelFilePath.TabIndex = 8;
            this.labelFilePath.Text = "服务地址：";
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemShow,
            this.menuItemExit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(129, 52);
            // 
            // menuItemShow
            // 
            this.menuItemShow.Name = "menuItemShow";
            this.menuItemShow.Size = new System.Drawing.Size(128, 24);
            this.menuItemShow.Text = "还原(&R)";
            this.menuItemShow.Click += new System.EventHandler(this.menuItemShow_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(128, 24);
            this.menuItemExit.Text = "退出(&X)";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "服务端口";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // BtnGetDeviceStatus
            // 
            this.BtnGetDeviceStatus.Location = new System.Drawing.Point(268, 325);
            this.BtnGetDeviceStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnGetDeviceStatus.Name = "BtnGetDeviceStatus";
            this.BtnGetDeviceStatus.Size = new System.Drawing.Size(100, 29);
            this.BtnGetDeviceStatus.TabIndex = 16;
            this.BtnGetDeviceStatus.Text = "设备状态";
            this.BtnGetDeviceStatus.UseVisualStyleBackColor = true;
            this.BtnGetDeviceStatus.Click += new System.EventHandler(this.BtnGetDeviceStatus_Click);
            // 
            // BtnGetAlarmInfo
            // 
            this.BtnGetAlarmInfo.Location = new System.Drawing.Point(17, 372);
            this.BtnGetAlarmInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnGetAlarmInfo.Name = "BtnGetAlarmInfo";
            this.BtnGetAlarmInfo.Size = new System.Drawing.Size(100, 29);
            this.BtnGetAlarmInfo.TabIndex = 16;
            this.BtnGetAlarmInfo.Text = "报警信息";
            this.BtnGetAlarmInfo.UseVisualStyleBackColor = true;
            this.BtnGetAlarmInfo.Click += new System.EventHandler(this.BtnGetAlarmInfo_Click);
            // 
            // BtnHopperSetting
            // 
            this.BtnHopperSetting.Location = new System.Drawing.Point(16, 325);
            this.BtnHopperSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnHopperSetting.Name = "BtnHopperSetting";
            this.BtnHopperSetting.Size = new System.Drawing.Size(100, 29);
            this.BtnHopperSetting.TabIndex = 16;
            this.BtnHopperSetting.Text = "料斗行程";
            this.BtnHopperSetting.UseVisualStyleBackColor = true;
            this.BtnHopperSetting.Click += new System.EventHandler(this.BtnHopperSetting_Click);
            // 
            // BtnEmergencyDoorSetting
            // 
            this.BtnEmergencyDoorSetting.Location = new System.Drawing.Point(141, 325);
            this.BtnEmergencyDoorSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnEmergencyDoorSetting.Name = "BtnEmergencyDoorSetting";
            this.BtnEmergencyDoorSetting.Size = new System.Drawing.Size(100, 29);
            this.BtnEmergencyDoorSetting.TabIndex = 16;
            this.BtnEmergencyDoorSetting.Text = "安全门行程";
            this.BtnEmergencyDoorSetting.UseVisualStyleBackColor = true;
            this.BtnEmergencyDoorSetting.Click += new System.EventHandler(this.BtnEmergencyDoorSetting_Click);
            // 
            // BtnResetAlarm
            // 
            this.BtnResetAlarm.Location = new System.Drawing.Point(141, 372);
            this.BtnResetAlarm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnResetAlarm.Name = "BtnResetAlarm";
            this.BtnResetAlarm.Size = new System.Drawing.Size(100, 29);
            this.BtnResetAlarm.TabIndex = 16;
            this.BtnResetAlarm.Text = "复位报警";
            this.BtnResetAlarm.UseVisualStyleBackColor = true;
            this.BtnResetAlarm.Click += new System.EventHandler(this.BtnResetAlarm_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(655, 15);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(604, 445);
            this.textBox1.TabIndex = 17;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(655, 469);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(100, 29);
            this.button7.TabIndex = 18;
            this.button7.Text = "清空信息";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // textTrayNumber
            // 
            this.textTrayNumber.Location = new System.Drawing.Point(123, 240);
            this.textTrayNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textTrayNumber.Name = "textTrayNumber";
            this.textTrayNumber.Size = new System.Drawing.Size(153, 25);
            this.textTrayNumber.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 244);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "托盘序号：";
            // 
            // textXNumber
            // 
            this.textXNumber.Location = new System.Drawing.Point(123, 279);
            this.textXNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textXNumber.Name = "textXNumber";
            this.textXNumber.Size = new System.Drawing.Size(153, 25);
            this.textXNumber.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 285);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "X轴灯号：";
            // 
            // textPLCAddress
            // 
            this.textPLCAddress.Location = new System.Drawing.Point(121, 69);
            this.textPLCAddress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textPLCAddress.Name = "textPLCAddress";
            this.textPLCAddress.Size = new System.Drawing.Size(368, 25);
            this.textPLCAddress.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 72);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "PLC地址：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 106);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "PLC端口：";
            // 
            // textPLCPort
            // 
            this.textPLCPort.Location = new System.Drawing.Point(120, 102);
            this.textPLCPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textPLCPort.Name = "textPLCPort";
            this.textPLCPort.Size = new System.Drawing.Size(368, 25);
            this.textPLCPort.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 446);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 15);
            this.label2.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(19, 198);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 15);
            this.label6.TabIndex = 21;
            this.label6.Text = "测试功能";
            // 
            // BtnKardexTakeOut
            // 
            this.BtnKardexTakeOut.Location = new System.Drawing.Point(21, 469);
            this.BtnKardexTakeOut.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnKardexTakeOut.Name = "BtnKardexTakeOut";
            this.BtnKardexTakeOut.Size = new System.Drawing.Size(100, 29);
            this.BtnKardexTakeOut.TabIndex = 22;
            this.BtnKardexTakeOut.Text = "卡迪斯取出";
            this.BtnKardexTakeOut.UseVisualStyleBackColor = true;
            this.BtnKardexTakeOut.Click += new System.EventHandler(this.BtnKardexTakeOut_Click);
            // 
            // BtnKardexTakeIn
            // 
            this.BtnKardexTakeIn.Location = new System.Drawing.Point(176, 471);
            this.BtnKardexTakeIn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnKardexTakeIn.Name = "BtnKardexTakeIn";
            this.BtnKardexTakeIn.Size = new System.Drawing.Size(100, 29);
            this.BtnKardexTakeIn.TabIndex = 23;
            this.BtnKardexTakeIn.Text = "卡迪斯存入";
            this.BtnKardexTakeIn.UseVisualStyleBackColor = true;
            this.BtnKardexTakeIn.Click += new System.EventHandler(this.BtnKardexTakeIn_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(288, 471);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 29);
            this.button1.TabIndex = 24;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(423, 471);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 29);
            this.button2.TabIndex = 25;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // BtnStartContainer
            // 
            this.BtnStartContainer.Location = new System.Drawing.Point(288, 240);
            this.BtnStartContainer.Margin = new System.Windows.Forms.Padding(4);
            this.BtnStartContainer.Name = "BtnStartContainer";
            this.BtnStartContainer.Size = new System.Drawing.Size(100, 29);
            this.BtnStartContainer.TabIndex = 16;
            this.BtnStartContainer.Text = "启动货柜";
            this.BtnStartContainer.UseVisualStyleBackColor = true;
            this.BtnStartContainer.Click += new System.EventHandler(this.BtnStartContainer_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1287, 515);
            this.Controls.Add(this.BtnStartContainer);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BtnKardexTakeIn);
            this.Controls.Add(this.BtnKardexTakeOut);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textPLCPort);
            this.Controls.Add(this.textPLCAddress);
            this.Controls.Add(this.textXNumber);
            this.Controls.Add(this.textTrayNumber);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.BtnResetAlarm);
            this.Controls.Add(this.BtnEmergencyDoorSetting);
            this.Controls.Add(this.BtnHopperSetting);
            this.Controls.Add(this.BtnGetAlarmInfo);
            this.Controls.Add(this.BtnGetDeviceStatus);
            this.Controls.Add(this.checkBoxAutoRun);
            this.Controls.Add(this.labelAutoRun);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelFilePath);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "朗杰设备控制服务";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxAutoRun;
        private System.Windows.Forms.Label labelAutoRun;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Label labelFilePath;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItemShow;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button BtnGetDeviceStatus;
        private System.Windows.Forms.Button BtnGetAlarmInfo;
        private System.Windows.Forms.Button BtnHopperSetting;
        private System.Windows.Forms.Button BtnEmergencyDoorSetting;
        private System.Windows.Forms.Button BtnResetAlarm;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox textTrayNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textXNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textPLCAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textPLCPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button BtnKardexTakeOut;
        private System.Windows.Forms.Button BtnKardexTakeIn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button BtnStartContainer;
    }
}