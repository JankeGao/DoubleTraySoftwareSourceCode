
using Microsoft.Win32;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Threading;
using System.Windows.Forms;

namespace PLCServer
{
    public partial class MainForm : Form
    {
        bool needClose;
        WebServiceHost _serviceHost;
       // ServiceHost _serviceHost;
        PlcServer _plcServer;
        public MainForm()
        {
            InitializeComponent();
            this.timer1.Enabled = false;
            this.timer1.Interval = 1000*10;
            this.timer1.Stop();
        }

        /// <summary>
        /// 开机自启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxAutoRun_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Assembly exe = Assembly.GetExecutingAssembly();
                string exeFilePath = exe.Location;
                string exeFileName = Path.GetFileName(exeFilePath);
                using (RegistryKey runItem = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    object existed = runItem.GetValue(exeFileName);

                    if (existed == null && this.checkBoxAutoRun.Checked)
                        runItem.SetValue(exeFileName, exeFilePath);
                    else if (existed != null && !this.checkBoxAutoRun.Checked)
                        runItem.DeleteValue(exeFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                this.label2.Text = "设备连接中...";
                this.Start();

                //this.timer1.Enabled = true;
                //this.timer1.Start();

            }
            catch (Exception ex)
            {
                this.label2.Text = "设备连接失败，请检查网络或PLC地址信息！";
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary> 
        /// 获取当前使用的IP 
        /// </summary> 
        /// <returns></returns> 
        private static string GetLocalIP()
        {
            try
            {
                System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient();
                c.Connect("www.baidu.com", 80);
                string ip = ((System.Net.IPEndPoint)c.Client.LocalEndPoint).Address.ToString();
                c.Close();
                return ip;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            //如果是开机启动的则隐藏
            if (this.checkBoxAutoRun.Checked)
                this.Hide();
            // 开机自启，启动服务
            this.Start();
            _plcServer = new PlcServer();
            PlcServer.IsWorking = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.textBoxFilePath.Text = ConfigurationManager.AppSettings["ServerAddress"].ToString();
                this.textPLCAddress.Text = ConfigurationManager.AppSettings["PLCAddress"].ToString();
                this.textPLCPort.Text = ConfigurationManager.AppSettings["PLCPort"].ToString();
                Assembly exe = Assembly.GetExecutingAssembly();
                string exeFilePath = exe.Location;
                string exeFileName = Path.GetFileName(exeFilePath);
                using (RegistryKey runItem = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
                {
                    object existed = runItem.GetValue(exeFileName);
                    this.checkBoxAutoRun.Checked = existed != null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void menuItemShow_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            this.needClose = true;
            System.Environment.Exit(0);
            //this.Close();
        }

        /// <summary>
        /// 关闭程序-退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.notifyIcon.Visible = false;
                //关闭程序，断开连接
                this.Stop();
            }
            catch (Exception ex)
            {

               
            }
        }
        /// <summary>
        /// 主窗口关闭-最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.needClose)
            {
                this.Hide();

                e.Cancel = true;
            }
        }


        /// <summary>
        /// 清空提示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
        }


        /// <summary>
        /// 启动与PLC的连接
        /// </summary>
        void Start()
        {
            // 发布Web 服务
            if (!string.IsNullOrEmpty(this.textBoxFilePath.Text))
            {
                try
                {
                    //验证
                    bool IsAutoConnect = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsAutoConnect"].ToString());
                    if (IsAutoConnect)
                    {
                        string urlPath = this.textBoxFilePath.Text;
                        if (string.IsNullOrEmpty(urlPath))
                        {
                            MessageBox.Show("未输入地址");
                            return;
                        }
                        string PLCAddress = this.textPLCAddress.Text;
                        string port = this.textPLCPort.Text;
                        if (!int.TryParse(port, out int PLCPort))
                        {
                            MessageBox.Show("PLC端口号必须为整数");
                            return;
                        }
                        _plcServer = new PlcServer();
                        // 先断开之前的连接
                        _plcServer.Stop();
                        var result = _plcServer.StartPLC(PLCAddress, PLCPort);
                        if (!result.Success)
                        {
                            this.label2.Text = "设备连接失败，请检查网络或PLC地址信息！";
                            MessageBox.Show("PCL连接失败:" + result.Message);
                            return;
                        }
                        else
                        {
                            this.label2.Text = "";
                            PlcServer.IsWorking = true;
                            //   MessageBox.Show(this, "PLC 连接成功！", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
 
                    string url = this.textBoxFilePath.Text;
                    Uri baseAddress = new Uri(url);

          

                    var title = string.Format("{0}{1}", "服务端口:", baseAddress.ToString().Substring(baseAddress.ToString().LastIndexOf(":") + 1));
                    this.notifyIcon.Text = title;


                    #region 发布WEB服务

                    _serviceHost = new WebServiceHost(typeof(PlcServer));
                    {
                        //如果不设置MaxBufferSize,当传输的数据特别大的时候，很容易出现“提示:413 Request Entity Too Large”错误信息,最大设置为20M
                        WebHttpBinding binding = new WebHttpBinding
                        {
                            TransferMode = TransferMode.Buffered,
                            MaxBufferSize = 2147483647,
                            MaxReceivedMessageSize = 2147483647,
                            MaxBufferPoolSize = 2147483647,
                            ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                            Security = { Mode = WebHttpSecurityMode.None }
                        };
                        _serviceHost.AddServiceEndpoint(typeof(IPlcServer), binding, baseAddress);

                        ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                        smb.HttpGetEnabled = true;

                        _serviceHost.Description.Behaviors.Add(smb);
                        //添加监测
                        _serviceHost.Opened += delegate
                        {
                            //Console.WriteLine("Web服务已开启...");
                        };
                        _serviceHost.Open();
                        this.buttonStart.Enabled = false;
                        this.buttonStop.Enabled = true;
                        //保存控件值
                        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        configuration.AppSettings.Settings["ServerAddress"].Value = this.textBoxFilePath.Text; ;
                        configuration.AppSettings.Settings["PLCAddress"].Value = this.textPLCAddress.Text; ;
                        configuration.AppSettings.Settings["PLCPort"].Value = this.textPLCPort.Text; ;
                        configuration.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");


                        this.Hide();
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    this.label2.Text = "设备连接失败，请检查网络或PLC地址信息！";
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        /// <summary>
        /// 断开与PLC的连接
        /// </summary>
        void Stop()
        {
            if (this._serviceHost!=null && this._serviceHost.State==CommunicationState.Opened)
            {
                _serviceHost.Close();
            }
            else if(this._serviceHost!=null)
            {
                _serviceHost.Abort();
            }
            //   _plcServer.Stop();
            PlcServer.IsWorking = false;
            this.buttonStart.Enabled = true;
            this.buttonStop.Enabled = false;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
                this.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception)
            {


            }

        }

        private void textBoxFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        #region PLC 控制方法

        /// <summary>
        /// 启动货柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStartContainer_Click(object sender, EventArgs e)
        {

            //string containerCode = this.textContainerCode.Text;
            //if (string.IsNullOrEmpty(containerCode))
            //{
            //    MessageBox.Show("请输入货柜编码");
            //    return;
            //}
            string trayNumber = this.textTrayNumber.Text;
            if (string.IsNullOrEmpty(trayNumber))
            {
                MessageBox.Show("请输入托盘序号");
                return;
            }
            if (!int.TryParse(trayNumber,out int number))
            {
                MessageBox.Show("托盘序号必须为整数");
                return;
            }
            string xNumber = this.textXNumber.Text;
            if (string.IsNullOrEmpty(xNumber))
            {
                MessageBox.Show("请输入X轴灯号");
                return;
            }
            if (!int.TryParse(xNumber, out int x))
            {
                MessageBox.Show("X轴灯号必须为整数");
                return;
            }
            if (_plcServer==null)
            {
                MessageBox.Show("PLC连接尚未开启");
                return;
            }
            if (PlcServer.isConnected==false)
            {
                MessageBox.Show("PLC未连接或连接失败");
                return;
            }
            this.label2.Text = "货柜运行中...";
            Command.RunningContainer runningContainer = new Command.RunningContainer();
            //runningContainer.ContainerCode = containerCode;
            runningContainer.TrayCode = number;
            runningContainer.XLight = (short)x;
            var result = _plcServer.StartRunningContainer(runningContainer);
            this.BeginInvoke(new Action(() => {

                this.textBox1.Text += Newtonsoft.Json.JsonConvert.SerializeObject(result)+ "\r\n";
                this.label2.Text = "";
            }));


        }
        /// <summary>
        /// 料斗行程设定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHopperSetting_Click(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                MessageBox.Show("PLC连接尚未开启");
                return;
            }
            if (PlcServer.isConnected == false)
            {
                MessageBox.Show("PLC未连接或连接失败");
                return;
            }
            this.label2.Text = "料斗行程设定中...";
            var result = _plcServer.HopperSetting();
            this.BeginInvoke(new Action(() => {
                this.label2.Text = "";
                this.textBox1.Text += Newtonsoft.Json.JsonConvert.SerializeObject(result) + "\r\n";
            }));
        }

        /// <summary>
        /// 安全门行程设定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEmergencyDoorSetting_Click(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                MessageBox.Show("PLC连接尚未开启");
                return;
            }
            if (PlcServer.isConnected == false)
            {
                MessageBox.Show("PLC未连接或连接失败");
                return;
            }
            this.label2.Text = "安全门行程设定中...";
            var result = _plcServer.EmergencyDoorSetting();
            this.BeginInvoke(new Action(() => {
                this.label2.Text = "";
                this.textBox1.Text += Newtonsoft.Json.JsonConvert.SerializeObject(result) + "\r\n";
            }));
        }

        /// <summary>
        /// 获取设备通讯状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGetDeviceStatus_Click(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                MessageBox.Show("PLC连接尚未开启");
                return;
            }
            if (PlcServer.isConnected == false)
            {
                MessageBox.Show("PLC未连接或连接失败");
                return;
            }
            this.label2.Text = "设备状态读取中...";
         //   var result = _plcServer.GetPlcDeivceStatus();
            //this.BeginInvoke(new Action(() => {
            //    this.label2.Text = "";
            //    this.textBox1.Text += Newtonsoft.Json.JsonConvert.SerializeObject(result) + "\r\n";
            //}));
        }

        private void BtnGetAlarmInfo_Click(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                MessageBox.Show("PLC连接尚未开启");
                return;
            }
            if (PlcServer.isConnected == false)
            {
                MessageBox.Show("PLC未连接或连接失败");
                return;
            }
            this.label2.Text = "设备报警信息读取中...";
            var result = _plcServer.GetAlarmInformation();
            this.BeginInvoke(new Action(() =>
            {
                this.label2.Text = "";
                this.textBox1.Text += Newtonsoft.Json.JsonConvert.SerializeObject(result) + "\r\n";
            }));
        }

        private void BtnResetAlarm_Click(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                MessageBox.Show("PLC连接尚未开启");
                return;
            }
            if (PlcServer.isConnected == false)
            {
                MessageBox.Show("PLC未连接或连接失败");
                return;
            }

            this.label2.Text = "设备报警重置中...";
            var result = _plcServer.ResetAlarm();
            this.BeginInvoke(new Action(() => {
                this.label2.Text = "";
                this.textBox1.Text += Newtonsoft.Json.JsonConvert.SerializeObject(result) + "\r\n";
            }));
        }


        #endregion
        /// <summary>
        /// 料斗行程手动完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                MessageBox.Show("PLC连接尚未开启");
                return;
            }
            if (PlcServer.isConnected == false)
            {
                MessageBox.Show("PLC未连接或连接失败");
                return;
            }

            this.label2.Text = "料斗行程手动完成...";
           // var result = _plcServer.HandFinishHopperSetting();
            //this.BeginInvoke(new Action(() => {
            //    this.label2.Text = "";
            //    this.textBox1.Text += Newtonsoft.Json.JsonConvert.SerializeObject(result) + "\r\n";
            //}));
        }


        /// <summary>
        /// 安全门行程手动完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                MessageBox.Show("PLC连接尚未开启");
                return;
            }
            if (PlcServer.isConnected == false)
            {
                MessageBox.Show("PLC未连接或连接失败");
                return;
            }

            this.label2.Text = "安全门行程手动完成...";
            var result = _plcServer.HandFinishEmergencyDoorSetting();
            this.BeginInvoke(new Action(() => {
                this.label2.Text = "";
                this.textBox1.Text += Newtonsoft.Json.JsonConvert.SerializeObject(result) + "\r\n";
            }));
        }

        private void BtnKardexTakeOut_Click(object sender, EventArgs e)
        {
            if (_plcServer==null)
            {
                _plcServer = new PlcServer();
            }
            string number = this.textTrayNumber.Text;
            Command.RunningContainer runningContainer = new Command.RunningContainer();
            runningContainer.IpAddress = "172.30.7.1";
            runningContainer.TrayCode = int.Parse(number);
            runningContainer.Port = 81;
            _plcServer.StartRunningC3000Container(runningContainer);
        }

        private void BtnKardexTakeIn_Click(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                _plcServer = new PlcServer();
            }
            string number = this.textTrayNumber.Text;
            Command.RunningContainer runningContainer = new Command.RunningContainer();
            runningContainer.IpAddress = "172.30.7.1";
            runningContainer.TrayCode = int.Parse(number);
            runningContainer.Port = 81;
            _plcServer.StartRestoreC3000Container(runningContainer);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                _plcServer = new PlcServer();
            }
            string number = this.textTrayNumber.Text;
            Command.RunningContainer runningContainer = new Command.RunningContainer();
            runningContainer.IpAddress = "172.16.1.1";
            runningContainer.TrayCode = int.Parse(number);
            runningContainer.Port = 2200;
            _plcServer.StartRunningHanelContainer(runningContainer);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (_plcServer == null)
            {
                _plcServer = new PlcServer();
            }
            string number = this.textTrayNumber.Text;
            Command.RunningContainer runningContainer = new Command.RunningContainer();
            runningContainer.IpAddress = "172.16.1.1";
            runningContainer.TrayCode = int.Parse(number);
            runningContainer.Port = 2200;
            _plcServer.FinishHanellContainer(runningContainer);
        }
    }
}
