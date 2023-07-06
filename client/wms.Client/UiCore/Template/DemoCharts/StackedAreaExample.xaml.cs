using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Bussiness.Contracts;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Dependency;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.Service;
using Container = Bussiness.Entitys.Container;

namespace wms.Client.UiCore.Template.DemoCharts
{
    /// <summary>
    /// 设备状态信息
    /// </summary>
    public partial class StackedAreaExample : UserControl
    {
        /// <summary>
        /// 入库任务契约
        /// </summary>
        private readonly IWareHouseContract WareHouseContract;

        /// <summary>
        /// 入库任务契约
        /// </summary>
        private readonly IDeviceAlarmContract DeviceAlarmContract;

        DispatcherTimer _mainTimer;
        public StackedAreaExample()
        {
            InitializeComponent();
            WareHouseContract = IocResolver.Resolve<IWareHouseContract>();
            DeviceAlarmContract = IocResolver.Resolve<IDeviceAlarmContract>();
            ReadConfigInfo();
            //GetMyPageData();
            IsReading = false;
            DataContext = this;
            _mainTimer = new DispatcherTimer();
            _mainTimer.Interval = TimeSpan.FromSeconds(2);
            _mainTimer.Tick += new EventHandler(_mainTimer_Tick);
            _mainTimer.IsEnabled = true;
            //  this.InjectStopOnClick();

        }
        public bool IsReading { get; set; }

        void _mainTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => { GetMyPageData(); }));
        }

        /// <summary>
        /// 获取本机货柜任务
        /// </summary>
        public void GetMyPageData()
        {
            string step = "0";
            try
            {

                if (GlobalData.IsOnLine)
                {
                    var query = WareHouseContract.Containers.FirstOrDefault(a => a.Code == ContainerCode);
                    step = "1";
                    DeviceIP = query.Ip;
                    DevicePort = query.Port;
                    AlarmStatus = query.ALarmStatusCaption;
                    ConStatus = query.StatusCaption;
                    if (query.AlarmStatus == (int)DeviceAlarmStateEnum.Urgencye)
                    {

                        var alarm = DeviceAlarmContract.DeviceAlarms.FirstOrDefault(a => a.Status == (int)DeviceAlarmStateEnum.Urgencye && a.ContainerCode == ContainerCode);
                        step = "2";
                        if (alarm != null)
                        {
                            AlarmStatusCap = alarm.AlarmStatusDescription;
                        }
                    }
                    else
                    {
                        AlarmStatusCap = "";
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalData.IsOnLine = false;
                Msg.Error("step:" + step + "---StackedAreaExampleError:" + ex.StackTrace + "--------" + ex.InnerException.Message + "-------" + ex.Source + "-------" + ex.InnerException.StackTrace);
            }
        }


        public void InjectStopOnClick()
        {
            IsReading = !IsReading;
            if (IsReading)
            {
                Task.Factory.StartNew(GetMyPageData);
            }
        }


        private void DisplayFormThread()
        {
            try
            {
                MainWindow ObjMain = new MainWindow();
                ObjMain.Show();
                ObjMain.Closed += (s, e) => System.Windows.Threading.Dispatcher.ExitAllFrames();

                System.Windows.Threading.Dispatcher.Run();
            }
            catch (Exception ex)
            {
                //LoggerHelper.Log(ex);
            }
        }

        /// <summary>
        /// 获取本机货柜任务
        /// </summary>
        public void Rest_Alarm(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GlobalData.IsOnLine)
                {
                    // 读取PLC 状态信息
                    var baseCpntrolService = ServiceProvider.Instance.Get<IBaseControlService>();

                    // 物料实体映射
                    var inTask = baseCpntrolService.PostRestAllAlarm();


                    // 设备在线
                    if (inTask.Result.Success)
                    {
                        var deviceEnity = new DeviceAlarm()
                        {
                            ContainerCode = ContainerCode
                        };
                        // 读取PLC 状态信息
                        var alarmService = ServiceProvider.Instance.Get<IAlarmService>();
                        var serverRest = alarmService.PostRestAllAlarmServer(deviceEnity);

                        if (serverRest.Result.Success)
                        {
                            // 报警弹窗复位
                            GlobalData.Comfirm = false;
                            // 获取当前设备下的所有报警信息
                            Msg.Info("全部报警复位成功！");
                            return;
                        }
                        else
                        {
                            Msg.Warning("服务端报警复位失败！");
                            return;
                        }
                    }
                    else
                    {
                        Msg.Error(inTask.Result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }


        /// <summary>
        /// 客户端货柜编码
        /// </summary>
        private string ContainerCode = string.Empty;

        public string ContainerCurCode
        {
            get { return ContainerCode; }
            set { ContainerCode = value; OnPropertyChanged("ContainerCurCode"); }
        }

        /// <summary>
        /// 读取本地配置信息--人员登录时间
        /// </summary>
        public void ReadConfigInfo()
        {
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
            if (File.Exists(cfgINI))
            {
                IniFile ini = new IniFile(cfgINI);
                ContainerCode = ini.IniReadValue("ClientInfo", "code");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 新建静态属性变更通知
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        /// <summary>
        /// UserCode
        /// </summary>
        private static string _ConStatus = string.Empty;
        public static string ConStatus
        {
            get
            {
                return _ConStatus;
            }
            set
            {
                _ConStatus = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(ConStatus)));
            }
        }

        /// <summary>
        /// UserCode
        /// </summary>
        private static string _DeviceIP = string.Empty;
        public static string DeviceIP
        {
            get
            {
                return _DeviceIP;
            }
            set
            {
                _DeviceIP = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(DeviceIP)));
            }
        }

        /// <summary>
        /// UserCode
        /// </summary>
        private static string _DevicePort = string.Empty;
        public static string DevicePort
        {
            get
            {
                return _DevicePort;
            }
            set
            {
                _DevicePort = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(DevicePort)));
            }
        }


        private static string _AlarmStatus = string.Empty;
        public static string AlarmStatus
        {
            get
            {
                return _AlarmStatus;
            }
            set
            {
                _AlarmStatus = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AlarmStatus)));
            }
        }

        private static string _AlarmStatusCap = string.Empty;
        public static string AlarmStatusCap
        {
            get
            {
                return _AlarmStatusCap;
            }
            set
            {
                _AlarmStatusCap = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AlarmStatusCap)));
            }
        }

    }
}
