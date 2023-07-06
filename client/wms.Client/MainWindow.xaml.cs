using System;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Enums;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.Service;
using wms.Client.Template;
using wms.Client.ViewModel;

namespace wms.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dTimer;

        // 定义定时器
        private Timer _timer;
        // 第一次触发弹窗的起始计算时间
        private DateTime _lastDismissTime;

        private Timer _dateTime;

        public MainWindow()
        {
            InitializeComponent(); 
            this.Zone.MouseDoubleClick += (sender, e) => { Max(); };
            //构造一个DispatcherTimer类实例
            dTimer = new System.Windows.Threading.DispatcherTimer();
            //设置事件处理函数
            dTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            EventManager.RegisterClassHandler(typeof(Button), Button.ClickEvent, new RoutedEventHandler(UpdataText));

            method = new MessageBoxShow(showMessage);

            this.ReadConfigInfo();
            // 初始化定时器，每分钟触发一次
            _timer = new Timer(60000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            _dateTime = new Timer(10000);
            _dateTime.Elapsed += DateTimeElapsed;
            _dateTime.Start();


        }
        /// <summary>
        /// 定时10秒获取一次出入库任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateTimeElapsed(object sender, ElapsedEventArgs e)
        {
            var intaskService = ServiceProvider.Instance.Get<IInTaskService>();
            intaskService.GetInterfaceIn();
            intaskService.GetInterfaceOut();
        }

        public void ReadConfigInfo()
        {
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
            if (File.Exists(cfgINI))
            {
                IniFile ini = new IniFile(cfgINI);
                string lastDismissTimees = ini.IniReadValue("ClientInfo", "DismissTime");

                _lastDismissTime = DateTime.Parse(lastDismissTimees);
            }
        }

        // 定时器到达触发时间时，显示保养弹窗提示
        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // 判断是否需要半年后再次触发
            if (_lastDismissTime.AddDays(182) <= DateTime.Now)
            {
                // 切换回UI线程显示弹窗提示
                Dispatcher.Invoke(() =>
                {
                    var result = MaintenancePopover.ShowDialoges();
                    if (result == true)
                    {
                        _lastDismissTime = DateTime.Now;
                        string DismissTimeed = _lastDismissTime.ToString();
                        string cfgINI = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
                        IniFile ini = new IniFile(cfgINI);
                        ini.IniWriteValue("ClientInfo", "DismissTime", DismissTimeed);
                    }
                    else
                    {
                        // 如果验证码不正确，则将定时器间隔设置为一天，等待下一次触发
                        _timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
                    }
                });
            }
            // 判断是否需要重新触发
            else if (_lastDismissTime.AddDays(182) > DateTime.Now)
            {
                // 设置重新触发
                _timer.Interval = 60000;
            }

        }

        public delegate MessageBoxResult MessageBoxShow(string msg);
        public static MessageBoxShow method;

        public MessageBoxResult MessageBoxShow_F(string msg)
        {
           return MessageBox.Show(msg, "提示信息", MessageBoxButton.OK);
        }

        public  MessageBoxResult showMessage(string message)
        {
            return MessageBox.Show(message, "提示信息", MessageBoxButton.OK);
            //this.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //   return MessageBox.Show(message, "提示信息", MessageBoxButton.YesNo);
            //}));
        }
        #region Messenger

        /// <summary>
        /// 最大化
        /// </summary>
        /// <param name="msg"></param>
        public void Max(bool Mask = false)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        /// <summary>
        /// 如果在时间没无人操作任何按钮，退出全部页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // 清除登录信息
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
            IniFile ini = new IniFile(cfgINI);
            ini.IniWriteValue("Login", "UserCode", "");
            ini.IniWriteValue("Login", "UserName", "");
            ini.IniWriteValue("Login", "PictureUrl", "");
            ini.IniWriteValue("Login", "LoginTime", "");
            ini.IniWriteValue("Login", "Name", "");
            GlobalData.loginTime = "";
            var obj = new MainViewModel();
            if (obj == null) return;
            obj.ExitPage(MenuBehaviorType.ExitAllPage, "");
        }


        private void UpdataText(object sender, RoutedEventArgs e)
        {
            Button textBox = sender as Button;
            KeyEventArgs keyEventArgs = e as KeyEventArgs;
            int i = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["NoOp"].ToString())*60;
            //定时器时间间隔1s
            if (dTimer.Interval != null)
            {
                dTimer.Interval = new TimeSpan(0, 0, i);
                dTimer.Start();
            }
        }

        //触摸后重新给“i”赋值
        //private void SurfaceWindow_TouchDown(object sender, TouchEventArgs e)
        //{
        //    int i = 60;
        //    //定时器时间间隔1s
        //    if (dTimer.Interval != null)
        //    {
        //        dTimer.Interval = new TimeSpan(0, 0, i);
        //        dTimer.Start();
        //    }
        //}
        //private void SurfaceWindow_TouchDown(object sender, MouseEventArgs e)
        //{
        //    int i = 30;
        //    //定时器时间间隔1s
        //    if (dTimer.Interval != null)
        //    {
        //        dTimer.Interval = new TimeSpan(0, 0, i);
        //        dTimer.Start();
        //    }
        //}
        #endregion

    }
}
