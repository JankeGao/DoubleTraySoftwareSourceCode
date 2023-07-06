using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Core.Mapping;
using MaterialDesignThemes.Wpf;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Enums;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.LogicCore.Interface;
using wms.Client.Service;
using wms.Client.View;
using wms.Client.ViewModel.Base;
using wms.Client.ViewModel;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 系统设置
    /// </summary>
    [Module(ModuleType.EquipmentManagement, "ContainerSettingDlg", "货柜设置")]
    public class ContainerSettingViewModel : ViewModelBase
    {

        /// <summary>
        /// 任务契约
        /// </summary>
      //  private readonly IDeviceAlarmContract DeviceAlarmContract;

        private int tabpageIndex;
        public int TabPageIndex { get { return tabpageIndex; } set { tabpageIndex = value; RaisePropertyChanged(); } }
        private readonly IRepository<Bussiness.Entitys.Container, int> ContainerRepository;
        public ContainerSettingViewModel()
        {
            //  DeviceAlarmContract = IocResolver.Resolve<IDeviceAlarmContract>();
            VerticalList = new ObservableCollection<string>();
            VerticalList.Add("步骤 1 开始垂直行程学习");
            VerticalList.Add("步骤 2 监视M340 on表示学习结束");
            VerticalList.Add("步骤 3 确认垂直行程学习结束");

            ContainerRepository = IocResolver.Resolve<IRepository<Bussiness.Entitys.Container, int>>();
            this.ReadConfigInfo();
            this.ShowLogin();
        }


        private bool _IsCancel = true;

        /// <summary>
        /// 禁用按钮
        /// </summary>
        public bool IsCancel
        {
            get { return _IsCancel; }
            set { _IsCancel = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<string> _verticalList;
        public ObservableCollection<string> VerticalList { get { return _verticalList; } set { _verticalList = value; RaisePropertyChanged(); } }

        #region 任务组

        private ObservableCollection<DeviceAlarm> _DeviceAlarmList = new ObservableCollection<DeviceAlarm>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<DeviceAlarm> DeviceAlarmList
        {
            get { return _DeviceAlarmList; }
            set { _DeviceAlarmList = value; RaisePropertyChanged(); }
        }


        #endregion

        #region 命令(Binding Command)

        private RelayCommand _userLoginCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> UserLoginCommand { get; private set; }



        private RelayCommand _scanBarcodeCommand;

        /// <summary>
        /// 扫描入库条码
        /// </summary>
        public RelayCommand<string> ScanBarcodeCommand { get; private set; }



        private RelayCommand _inTaskCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> InTaskCommand { get; private set; }



        private RelayCommand _selectItemCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<InTaskMaterialDto> SelectItemCommand { get; private set; }


        /// <summary>
        /// 确认存入指令
        /// </summary>
        private RelayCommand _handShelfCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand  HandShelfCommand { get; private set; }



        /// <summary>
        /// 完成提交
        /// </summary>
        private RelayCommand _submitCommand;
        public RelayCommand SubmitCommand { get; private set; }




        /// <summary>
        /// 客户端货柜编码
        /// </summary>
        private string ContainerCode = string.Empty;

       
        /// <summary>
        /// 用户名
        /// </summary>
        private string userCode = string.Empty;
        public string UserCode
        {
            get { return userCode; }
            set { userCode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        private string userName = string.Empty;
        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        private string _pictureUrl = string.Empty;
        public string PictureUrl
        {
            get { return _pictureUrl; }
            set { _pictureUrl = value; RaisePropertyChanged(); }
        }

        private string _loginTime = string.Empty;
        public string loginTime
        {
            get { return _loginTime; }
            set { _loginTime = value; RaisePropertyChanged(); }
        }



        #endregion


        /// <summary>
        /// 核验登录
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckLogin()
        {
            // 系统用户注销时间
            var checkTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["LoginOutTime"].ToString());
            // 如果未登录
            if (string.IsNullOrWhiteSpace(GlobalData.loginTime))
            {
                return false;
            }
            else
            {
                var login = Convert.ToDateTime(GlobalData.loginTime).AddMinutes(checkTime);
                var now = DateTime.Now;

                // 如果时间已过期
                if (DateTime.Compare(now, login) > 0)
                {
                    // 系统登录
                    return false;
                }
                else
                {
                    // 核查用户是否有此模块操作权限
                    var user = ServiceProvider.Instance.Get<IUserService>();
                    var authCheck = user.GetCheckAuth(GlobalData.LoginModule);
                    if (!authCheck.Result.Success)
                    {
                        if (await Msg.Question("抱歉，您无该模块操作权限！"))
                        {
                            return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }


        /// <summary>
        /// 核验登录人员
        /// </summary>
        /// <param name="code"></param>
        public async void ShowLogin()
        {
            GlobalData.LoginModule = "ContainerSetting";
            GlobalData.LoginPageCode = "ContainerSettingDlg";
            GlobalData.LoginPageName = "货柜设置";
            //如果登录
            if (await CheckLogin())
            {
                // 核查用户是否有此模块操作权限
                //var user = ServiceProvider.Instance.Get<IUserService>();
                //var authCheck = user.GetCheckAuth(GlobalData.LoginModule);
                //if (!authCheck.Result.Success)
                //{
                //    Msg.Warning("抱歉，您无该模块操作权限！");
                //    // 刷新界面
                //    var obj = new MainViewModel();
                //    obj.ClosePage(GlobalData.LoginPageName);
                //    return;
                //}
                // 获取当前设备下的所有报警信息
                //var alarmList = DeviceAlarmContract.DeviceAlarmDtos.Where(a => a.ContainerCode == ContainerCode && a.Status == (int)DeviceAlarmStateEnum.Urgencye).ToList();
                //DeviceAlarmList.Clear();
                //alarmList.ForEach((arg) => DeviceAlarmList.Add(arg));
            }
            else    //如果未登录
            {
                var dialog = ServiceProvider.Instance.Get<IShowContent>();
                dialog.BindDataContext(new UserLoginWindow(), new UserLoginModel());
                dialog.Show();
            }
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
                TakeInTrayNumber = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                //string CurrentRunningTray = "";
                //string cfgINI = AppDomain.CurrentDomain.BaseDirectory + wms.Client.LogicCore.Configuration.SerivceFiguration.INI_CFG;
                //if (System.IO.File.Exists(cfgINI))
                //{
                //    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                //    CurrentRunningTray = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                //}
                //return CurrentRunningTray;
            }
        }


        #region 垂直运行学习
        private RelayCommand _VerticalNextCommand;

        public RelayCommand VerticalNextCommand
        {
            get
            {
                if (_VerticalNextCommand == null)
                {
                    _VerticalNextCommand = new RelayCommand(() => VerticalNext());
                }
                return _VerticalNextCommand;
            }
        }
        private int _verticalStep = 1;
        public int VerticalStep
        {
            get { return _verticalStep; }
            set { _verticalStep = value; RaisePropertyChanged(); }
        }

        private bool _verticalNextEnable = true;
        public bool VerticalNextEnable
        {
            get { return _verticalNextEnable; }
            set { _verticalNextEnable = value; RaisePropertyChanged(); }
        }
        public async void VerticalNext()
        {
            //this.VerticalStep++;
            //if (this.VerticalStep>this.VerticalList.Count+1)
            //{
            //    this.VerticalStep = 1;
            //}

            if (this.VerticalStep==1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result =  baseControlService.StartM300().Result;
                if (result.Success)
                {
                    VerticalFirstColor = "green";
                    VerticalSecondColor = "yellow";
                    this.VerticalNextEnable = false;
                    this.VerticalStep++;
                    //定义一个线程
                    ThreadPool.QueueUserWorkItem(new WaitCallback(VerticalProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }
              
            }
            else if (this.VerticalStep==2)
            {
                VerticalFirstColor = "green";
                VerticalSecondColor = "green";
                VerticalThirdColor = "yellow";
                this.VerticalStep++;
                this.VerticalNextEnable = true;
            }
            else if (this.VerticalStep==3)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.FinishM341().Result;
                if (result.Success)
                {
                    VerticalFirstColor = "green";
                    VerticalSecondColor = "green";
                    VerticalThirdColor = "green";
                    this.VerticalStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }
   
            }
            else if(this.VerticalStep == 4)
            {
                VerticalFirstColor = "yellow";
                VerticalSecondColor = "gray";
                VerticalThirdColor = "gray";
                this.VerticalStep = 1;
            }
            else
            {
                VerticalFirstColor = "yellow";
                VerticalSecondColor = "gray";
                VerticalThirdColor = "gray";
                this.VerticalStep = 1;
            }
        }

        private void VerticalProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.VerticalStep==2)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM340().Result;
                if (result.Success && (bool)result.Data ==true)
                {
                    VerticalNext();
                    break;
                }
               
            }
        }
        private string _verticalFirstColor = "yellow";
        public string VerticalFirstColor
        {
            get { return _verticalFirstColor; }
            set { _verticalFirstColor = value; RaisePropertyChanged(); }
        }
        private string _verticalSecondColor = "gray";
        public string VerticalSecondColor
        {
            get { return _verticalSecondColor; }
            set { _verticalSecondColor = value; RaisePropertyChanged(); }
        }

        private string _verticalThirdColor = "gray";
        public string VerticalThirdColor
        {
            get { return _verticalThirdColor; }
            set { _verticalThirdColor = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 手动垂直运行学习
        private RelayCommand _ManualVerticalNextCommand;

        public RelayCommand ManualVerticalNextCommand
        {
            get
            {
                if (_ManualVerticalNextCommand == null)
                {
                    _ManualVerticalNextCommand = new RelayCommand(() => ManualVerticalNext());
                }
                return _ManualVerticalNextCommand;
            }
        }
        private int _ManualVerticalStep = 1;
        public int ManualVerticalStep
        {
            get { return _ManualVerticalStep; }
            set { _ManualVerticalStep = value; RaisePropertyChanged(); }
        }

        private bool _ManualVerticalNextEnable = true;
        public bool ManualVerticalNextEnable
        {
            get { return _ManualVerticalNextEnable; }
            set { _ManualVerticalNextEnable = value; RaisePropertyChanged(); }
        }
        public async void ManualVerticalNext()
        {
            //this.ManualVerticalStep++;
            //if (this.ManualVerticalStep>this.ManualVerticalList.Count+1)
            //{
            //    this.ManualVerticalStep = 1;
            //}

            if (this.ManualVerticalStep == 1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                Model.Entity.RunningContainer runningContainer = new Model.Entity.RunningContainer();
                if (!int.TryParse(this.ManualVerticalInputTray, out int trayNumber))
                {
                    Msg.Error("输入的托架号格式不正确");
                    return;
                }
                runningContainer.TrayCode = trayNumber;
                var result = baseControlService.WriteD500(runningContainer).Result;
                if (result.Success)
                {
                    ManualVerticalFirstColor = "green";
                    ManualVerticalSecondColor = "yellow";
                    this.ManualVerticalStep++;
                    this.ManualVerticalInputEnabled = false;
                    //定义一个线程
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteTrayProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.ManualVerticalStep == 2)
            {


                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.StartM500().Result;
                if (result.Success)
                {
                    ManualVerticalFirstColor = "green";
                    ManualVerticalSecondColor = "green";
                    ManualVerticalThirdColor = "yellow";
                    this.ManualVerticalStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else if (this.ManualVerticalStep == 3)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.StartM501().Result;
                if (result.Success)
                {
                    ManualVerticalFirstColor = "green";
                    ManualVerticalSecondColor = "green";
                    ManualVerticalThirdColor = "green";
                    this.ManualVerticalStep++;
                    this.ManualVerticalInputEnabled = true;
                    this.ManualVerticalInputTray = "";
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.ManualVerticalStep == 4)
            {
                ManualVerticalFirstColor = "yellow";
                ManualVerticalSecondColor = "gray";
                ManualVerticalThirdColor = "gray";
                this.ManualVerticalStep = 1;
            }
            else
            {
                ManualVerticalFirstColor = "yellow";
                ManualVerticalSecondColor = "gray";
                ManualVerticalThirdColor = "gray";
                this.ManualVerticalStep = 1;
            }
        }

        private void ManualVerticalProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.ManualVerticalStep == 2)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM340().Result;
                if (result.Success && (bool)result.Data == true)
                {
                    ManualVerticalNext();
                    break;
                }

            }
        }
        private string _ManualVerticalFirstColor = "yellow";
        public string ManualVerticalFirstColor
        {
            get { return _ManualVerticalFirstColor; }
            set { _ManualVerticalFirstColor = value; RaisePropertyChanged(); }
        }
        private string _ManualVerticalSecondColor = "gray";
        public string ManualVerticalSecondColor
        {
            get { return _ManualVerticalSecondColor; }
            set { _ManualVerticalSecondColor = value; RaisePropertyChanged(); }
        }

        private string _ManualVerticalThirdColor = "gray";
        public string ManualVerticalThirdColor
        {
            get { return _ManualVerticalThirdColor; }
            set { _ManualVerticalThirdColor = value; RaisePropertyChanged(); }
        }
        private bool _ManualVerticalInputEnabled = true;
        public bool ManualVerticalInputEnabled
        {
            get { return _ManualVerticalInputEnabled; }
            set { _ManualVerticalInputEnabled = value; RaisePropertyChanged(); }
        }
        private string _ManualVerticalInputTray = "";
        public string ManualVerticalInputTray
        {
            get { return _ManualVerticalInputTray; }
            set { _ManualVerticalInputTray = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 水平行程学习
        private RelayCommand _HorizontalNextCommand;

        public RelayCommand HorizontalNextCommand
        {
            get
            {
                if (_HorizontalNextCommand == null)
                {
                    _HorizontalNextCommand = new RelayCommand(() => HorizontalNext());
                }
                return _HorizontalNextCommand;
            }
        }
        private int _horizontalStep = 1;
        public int HorizontalStep
        {
            get { return _horizontalStep; }
            set { _horizontalStep = value; RaisePropertyChanged(); }
        }

        private bool _HorizontalNextEnable = true;
        public bool HorizontalNextEnable
        {
            get { return _HorizontalNextEnable; }
            set { _HorizontalNextEnable = value; RaisePropertyChanged(); }
        }
        public async void HorizontalNext()  
        {
            //this.HorizontalStep++;
            //if (this.HorizontalStep>this.HorizontalList.Count+1)
            //{
            //    this.HorizontalStep = 1;
            //}

            if (this.HorizontalStep == 1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.StartM400().Result;
                if (result.Success)
                {
                    HorizontalFirstColor = "green";
                    HorizontalSecondColor = "yellow";
                    this.HorizontalNextEnable = false;
                    this.HorizontalStep++;
                    //定义一个线程
                    ThreadPool.QueueUserWorkItem(new WaitCallback(HorizontalProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.HorizontalStep == 2)
            {
                HorizontalFirstColor = "green";
                HorizontalSecondColor = "green";
                HorizontalThirdColor = "yellow";
                this.HorizontalStep++;
                this.HorizontalNextEnable = true;
            }
            else if (this.HorizontalStep == 3)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.FinishM441().Result;
                if (result.Success)
                {
                    HorizontalFirstColor = "green";
                    HorizontalSecondColor = "green";
                    HorizontalThirdColor = "green";
                    this.HorizontalStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.HorizontalStep == 4)
            {
                HorizontalFirstColor = "yellow";
                HorizontalSecondColor = "gray";
                HorizontalThirdColor = "gray";
                this.HorizontalStep = 1;
            }
            else
            {
                HorizontalFirstColor = "yellow";
                HorizontalSecondColor = "gray";
                HorizontalThirdColor = "gray";
                this.HorizontalStep = 1;
            }
        }

        private void HorizontalProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.HorizontalStep == 2)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM440().Result;
                if (result.Success && (bool)result.Data == true)
                {
                    HorizontalNext();
                    break;
                }

            }
        }
        private string _horizontalFirstColor = "yellow";
        public string HorizontalFirstColor
        {
            get { return _horizontalFirstColor; }
            set { _horizontalFirstColor = value; RaisePropertyChanged(); }
        }
        private string _horizontalSecondColor = "gray";
        public string HorizontalSecondColor
        {
            get { return _horizontalSecondColor; }
            set { _horizontalSecondColor = value; RaisePropertyChanged(); }
        }

        private string _horizontalThirdColor = "gray";
        public string HorizontalThirdColor
        {
            get { return _horizontalThirdColor; }
            set { _horizontalThirdColor = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 手动水平运行学习
        private RelayCommand _ManualHorizontalNextCommand;

        public RelayCommand ManualHorizontalNextCommand
        {
            get
            {
                if (_ManualHorizontalNextCommand == null)
                {
                    _ManualHorizontalNextCommand = new RelayCommand(() => ManualHorizontalNext());
                }
                return _ManualHorizontalNextCommand;
            }
        }

        private RelayCommand _ManualHorizontalNext2Command;

        public RelayCommand ManualHorizontalNext2Command
        {
            get
            {
                if (_ManualHorizontalNext2Command == null)
                {
                    _ManualHorizontalNext2Command = new RelayCommand(() => ManualHorizontal2Next());
                }
                return _ManualHorizontalNext2Command;
            }
        }

        private RelayCommand _ManualHorizontalM410Command;

        public RelayCommand ManualHorizontalM410Command
        {
            get
            {
                if (_ManualHorizontalM410Command == null)
                {
                    _ManualHorizontalM410Command = new RelayCommand(() => M410Command());
                }
                return _ManualHorizontalM410Command;
            }
        }

        private RelayCommand _ManualHorizontalM420Command;

        public RelayCommand ManualHorizontalM420Command
        {
            get
            {
                if (_ManualHorizontalM420Command == null)
                {
                    _ManualHorizontalM420Command = new RelayCommand(() => M420Command());
                }
                return _ManualHorizontalM420Command;
            }
        }
        private RelayCommand _ManualHorizontalM460Command;

        public RelayCommand ManualHorizontalM460Command
        {
            get
            {
                if (_ManualHorizontalM460Command == null)
                {
                    _ManualHorizontalM460Command = new RelayCommand(() => M460Command());
                }
                return _ManualHorizontalM460Command;
            }
        }



        private RelayCommand _ManualHorizontalM430Command;

        public RelayCommand ManualHorizontalM430Command
        {
            get
            {
                if (_ManualHorizontalM430Command == null)
                {
                    _ManualHorizontalM430Command = new RelayCommand(() => M430Command());
                }
                return _ManualHorizontalM430Command;
            }
        }

        private RelayCommand _ManualHorizontalM450Command;

        public RelayCommand ManualHorizontalM450Command
        {
            get
            {
                if (_ManualHorizontalM450Command == null)
                {
                    _ManualHorizontalM450Command = new RelayCommand(() => M450Command());
                }
                return _ManualHorizontalM450Command;
            }
        }
        private RelayCommand _ManualHorizontalM470Command;

        public RelayCommand ManualHorizontalM470Command
        {
            get
            {
                if (_ManualHorizontalM470Command == null)
                {
                    _ManualHorizontalM470Command = new RelayCommand(() => M470Command());
                }
                return _ManualHorizontalM470Command;
            }
        }


        public async void M410Command()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.SetM410True().Result;
            if (result.Success)
            {
                Msg.Info("操作成功呢");
            }
            else
            {
                Msg.Error(result.Message);
            }
        }
        public async void M420Command()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.SetM420True().Result;
            if (result.Success)
            {
                Msg.Info("操作成功呢");
            }
            else
            {
                Msg.Error(result.Message);
            }
        }
        public async void M460Command()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.SetM460True().Result;
            if (result.Success)
            {
                Msg.Info("操作成功呢");
            }
            else
            {
                Msg.Error(result.Message);
            }
        }
        public async void M430Command()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.SetM430True().Result;
            if (result.Success)
            {
                Msg.Info("操作成功呢");
            }
            else
            {
                Msg.Error(result.Message);
            }
        }
        public async void M450Command()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.SetM450True().Result;
            if (result.Success)
            {
                Msg.Info("操作成功呢");
            }
            else
            {
                Msg.Error(result.Message);
            }
        }
        public async void M470Command()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.SetM470True().Result;
            if (result.Success)
            {
                Msg.Info("操作成功呢");
            }
            else
            {
                Msg.Error(result.Message);
            }
        }



        private int _ManualHorizontalStep = 1;
        public int ManualHorizontalStep
        {
            get { return _ManualHorizontalStep; }
            set { _ManualHorizontalStep = value; RaisePropertyChanged(); }
        }


        private int _ManualHorizontal2Step = 1;
        public int ManualHorizontal2Step
        {
            get { return _ManualHorizontal2Step; }
            set { _ManualHorizontal2Step = value; RaisePropertyChanged(); }
        }

        private bool _ManualHorizontalNextEnable = true;
        public bool ManualHorizontalNextEnable
        {
            get { return _ManualHorizontalNextEnable; }
            set { _ManualHorizontalNextEnable = value; RaisePropertyChanged(); }
        }

        private bool _ManualHorizontalNext2Enable = true;
        public bool ManualHorizontalNext2Enable
        {
            get { return _ManualHorizontalNext2Enable; }
            set { _ManualHorizontalNext2Enable = value; RaisePropertyChanged(); }
        }


        public async void ManualHorizontalNext()
        {
            //this.ManualHorizontalStep++;
            //if (this.ManualHorizontalStep>this.ManualHorizontalList.Count+1)
            //{
            //    this.ManualHorizontalStep = 1;
            //}

            if (this.ManualHorizontalStep == 1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.SetM410True().Result;
                if (result.Success)
                {
                    ManualHorizontalFirstColor = "green";
                   // ManualHorizontalSecondColor = "yellow";
                    this.ManualHorizontalStep++;
                    this.ManualHorizontalNext2Enable = false;
                    //定义一个线程
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteTrayProgress), this);

                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.ManualHorizontalStep == 2)
            {
                if (await Msg.Question("是否选择选择M420,确认将选择M420，取消则选择M460"))
                {
                    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                    var result = baseControlService.SetM420True().Result;
                    if (result.Success)
                    {
                        ManualHorizontalFirstColor = "green";
                        ManualHorizontalSecondColor = "green";
                        ManualHorizontalThirdColor = "gray";
                        this.ManualHorizontalStep++;
                    }
                    else
                    {
                        Msg.Error(result.Message);
                    }
                }
                else
                {
                    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                    var result = baseControlService.SetM460True().Result;
                    if (result.Success)
                    {
                        ManualHorizontalFirstColor = "green";
                        ManualHorizontalSecondColor = "gray";
                        ManualHorizontalThirdColor = "green";
                        this.ManualHorizontalStep++;
                    }
                    else
                    {
                        Msg.Error(result.Message);
                    }
                }

        
            }
            else if (this.ManualHorizontalStep == 3)
            {
                ManualHorizontalFirstColor = "yellow";
                ManualHorizontalSecondColor = "gray";
                ManualHorizontalThirdColor = "gray";
                this.ManualHorizontalStep = 1;
                this.ManualHorizontalNext2Enable = true;

            }
            else if (this.ManualHorizontalStep == 4)
            {
                ManualHorizontalFirstColor = "yellow";
                ManualHorizontalSecondColor = "gray";
                ManualHorizontalThirdColor = "gray";
                this.ManualHorizontalStep = 1;
                this.ManualHorizontalNext2Enable = true;
            }
            else
            {
                ManualHorizontalFirstColor = "yellow";
                ManualHorizontalSecondColor = "gray";
                ManualHorizontalThirdColor = "gray";
                this.ManualHorizontalStep = 1;
                this.ManualHorizontalNext2Enable = true;

            }
        }
        public async void ManualHorizontal2Next()
        {
            //this.ManualHorizontalStep++;
            //if (this.ManualHorizontalStep>this.ManualHorizontalList.Count+1)
            //{
            //    this.ManualHorizontalStep = 1;
            //}
            if (this.ManualHorizontal2Step == 1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.SetM430True().Result;
                if (result.Success)
                {
                    ManualHorizontalFirst2Color = "green";
                    // ManualHorizontalSecondColor = "yellow";
                    this.ManualHorizontal2Step++;
                    this.ManualHorizontalNextEnable = false;
                    //定义一个线程
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteTrayProgress), this);

                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.ManualHorizontal2Step == 2)
            {
                if (await Msg.Question("是否选择选择M450,确认将选择M450，取消则选择M470"))
                {
                    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                    var result = baseControlService.SetM450True().Result;
                    if (result.Success)
                    {
                        ManualHorizontalFirst2Color = "green";
                        ManualHorizontalSecond2Color = "green";
                        ManualHorizontalThird2Color = "gray";
                        this.ManualHorizontal2Step++;
                    }
                    else
                    {
                        Msg.Error(result.Message);
                    }
                }
                else
                {
                    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                    var result = baseControlService.SetM470True().Result;
                    if (result.Success)
                    {
                        ManualHorizontalFirst2Color = "green";
                        ManualHorizontalSecond2Color = "gray";
                        ManualHorizontalThird2Color = "green";
                        this.ManualHorizontal2Step++;
                    }
                    else
                    {
                        Msg.Error(result.Message);
                    }
                }


            }
            else if (this.ManualHorizontal2Step == 3)
            {
                ManualHorizontalFirst2Color = "yellow";
                ManualHorizontalSecond2Color = "gray";
                ManualHorizontalThird2Color = "gray";
                this.ManualHorizontal2Step = 1;
                this.ManualHorizontalNextEnable = true;

            }
            else if (this.ManualHorizontalStep == 4)
            {
                ManualHorizontalFirst2Color = "yellow";
                ManualHorizontalSecond2Color = "gray";
                ManualHorizontalThird2Color = "gray";
                this.ManualHorizontal2Step = 1;
                this.ManualHorizontalNextEnable = true;
            }
            else
            {
                ManualHorizontalFirst2Color = "yellow";
                ManualHorizontalSecond2Color = "gray";
                ManualHorizontalThird2Color = "gray";
                this.ManualHorizontal2Step = 1;
                this.ManualHorizontalNextEnable = true;
            }
        }

        private string _ManualHorizontalFirstColor = "yellow";
        public string ManualHorizontalFirstColor
        {
            get { return _ManualHorizontalFirstColor; }
            set { _ManualHorizontalFirstColor = value; RaisePropertyChanged(); }
        }
        private string _ManualHorizontalSecondColor = "gray";
        public string ManualHorizontalSecondColor
        {
            get { return _ManualHorizontalSecondColor; }
            set { _ManualHorizontalSecondColor = value; RaisePropertyChanged(); }
        }

        private string _ManualHorizontalThirdColor = "gray";
        public string ManualHorizontalThirdColor
        {
            get { return _ManualHorizontalThirdColor; }
            set { _ManualHorizontalThirdColor = value; RaisePropertyChanged(); }
        }



        private string _ManualHorizontalFirst2Color = "yellow";
        public string ManualHorizontalFirst2Color
        {
            get { return _ManualHorizontalFirst2Color; }
            set { _ManualHorizontalFirst2Color = value; RaisePropertyChanged(); }
        }
        private string _ManualHorizontalSecond2Color = "gray";
        public string ManualHorizontalSecond2Color
        {
            get { return _ManualHorizontalSecond2Color; }
            set { _ManualHorizontalSecond2Color = value; RaisePropertyChanged(); }
        }

        private string _ManualHorizontalThird2Color = "gray";
        public string ManualHorizontalThird2Color
        {
            get { return _ManualHorizontalThird2Color; }
            set { _ManualHorizontalThird2Color = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 自动存取托盘
        #region 取出
        private RelayCommand _TakeOutNextCommand;

        public RelayCommand TakeOutNextCommand
        {
            get
            {
                if (_TakeOutNextCommand == null)
                {
                    _TakeOutNextCommand = new RelayCommand(() => TakeOutNext());
                }
                return _TakeOutNextCommand;
            }
        }
        private int _TakeOutStep = 1;
        public int TakeOutStep
        {
            get { return _TakeOutStep; }
            set { _TakeOutStep = value; RaisePropertyChanged(); }
        }

        private bool _TakeOutNextEnable = true;
        public bool TakeOutNextEnable
        {
            get { return _TakeOutNextEnable; }
            set { _TakeOutNextEnable = value; RaisePropertyChanged(); }
        }
        public async void TakeOutNext()
        {
            //ManualInViewModel manualInViewModel = new ManualInViewModel();
            //manualInViewModel.ChangeColor("FirstStep");
            string CurrentRunningTray = this.TakeOutTrayNumber;
            if (!int.TryParse(this.TakeOutTrayNumber, out int trayNumber))
            {
                Msg.Error("输入的托盘号格式不正确");
                return;
            }

            if (!short.TryParse(this.TakeOutLightX, out short lightX))
            {
                //Msg.Error("输入的灯号格式不正确");
                //return;
                lightX = 0;
            }
            if (!short.TryParse(this.TakeOutLightXLenght, out short lightXLenght))
            {
                lightXLenght = 1;
            }
            if (!string.IsNullOrEmpty(CurrentRunningTray))
            {
                // await dialog.c
                // 读取PLC 状态信息
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

                var runingEntity = new wms.Client.Model.Entity.RunningContainer()
                {
                    ContainerCode = ContainerCode,
                    TrayCode = trayNumber,
                    XLight = lightX,
                    XLenght = lightXLenght

                };
                var container = ContainerRepository.Query().FirstOrDefault(a => a.Code == ContainerCode);
                if (container != null)
                {
                    runingEntity.ContainerType = container.ContainerType;
                    runingEntity.IpAddress = container.Ip;
                    runingEntity.Port = int.Parse(container.Port);
                }

                // 货柜运行
                //  var runningContainer = baseControlService.WriteD650_In(runingEntity).Result;
                var runningContainer = baseControlService.PostStartRunningContainer(runingEntity).Result;
                if (runningContainer.Success)
                {
                    //var result = baseControlService.SetM654True().Result;
                    //if (!result.Success)
                    //{
                    //    Msg.Error(result.Message);
                    //}
                    TakeInTrayNumber = runingEntity.TrayCode.ToString();
                    //string CurrentRunningTray = "";
                    //string cfgINI = AppDomain.CurrentDomain.BaseDirectory + wms.Client.LogicCore.Configuration.SerivceFiguration.INI_CFG;
                    //if (System.IO.File.Exists(cfgINI))
                    //{
                    //    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                    //    CurrentRunningTray = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                    //}
                    //return CurrentRunningTray;
                }
                else
                {
                    Msg.Error(runningContainer.Message);
                }

                GlobalData.IsFocus = true;
                // baseControlService.PostStartRunningContainer(runingEntity);
            }



            //if (this.TakeOutStep == 1)
            //{
            //    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            //    Model.Entity.RunningContainer runningContainer = new Model.Entity.RunningContainer();
            //    if (!int.TryParse(this.TakeOutTrayNumber, out int trayNumber))
            //    {
            //        Msg.Error("输入的托盘号格式不正确");
            //        return;
            //    }

            //    if (!int.TryParse(this.TakeOutLightX, out int lightX))
            //    {
            //        //Msg.Error("输入的灯号格式不正确");
            //        //return;
            //        lightX = 0;
            //    }
            //    runningContainer.TrayCode = trayNumber;
            //    runningContainer.XLight = lightX;
            //    var result = baseControlService.WriteD650(runningContainer).Result;
            //    if (result.Success)
            //    {
            //        TakeOutFirstColor = "green";
            //        TakeOutSecondColor = "yellow";
            //        this.TakeOutTrayInputEnabled = false;
            //        this.TakeOutLightXInputEnabled = false;
            //        this.TakeOutStep++;
            //    }
            //    else
            //    {
            //        Msg.Error(result.Message);
            //    }

            //}
            //else if (this.TakeOutStep == 2)
            //{

            //    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            //    var result = baseControlService.StartM650().Result;
            //    if (result.Success)
            //    {
            //        TakeOutFirstColor = "green";
            //        TakeOutSecondColor = "green";
            //        TakeOutThirdColor = "yellow";
            //        this.TakeOutStep++;
            //    }
            //    else
            //    {
            //        Msg.Error(result.Message);
            //    }
            //}
            //else if (this.TakeOutStep == 3)
            //{
            //    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            //    var result = baseControlService.StartM651().Result;
            //    if (result.Success)
            //    {
            //        TakeOutFirstColor = "green";
            //        TakeOutSecondColor = "green";
            //        TakeOutThirdColor = "green";
            //        this.TakeOutStep++;
            //    }
            //    else
            //    {
            //        Msg.Error(result.Message);
            //    }

            //}
            //else if (this.TakeOutStep == 4)
            //{
            //    TakeOutFirstColor = "yellow";
            //    TakeOutSecondColor = "gray";
            //    TakeOutThirdColor = "gray";
            //    this.TakeOutTrayInputEnabled = true;
            //    this.TakeOutLightXInputEnabled = true;
            //    this.TakeOutTrayNumber = "";
            //    this.TakeOutLightX = "0";
            //    this.TakeOutStep = 1;
            //}
            //else
            //{
            //    TakeOutFirstColor = "yellow";
            //    TakeOutSecondColor = "gray";
            //    TakeOutThirdColor = "gray";
            //    this.TakeOutTrayInputEnabled = true;
            //    this.TakeOutLightXInputEnabled = true;
            //    this.TakeOutTrayNumber = "";
            //    this.TakeOutLightX = "0";
            //    this.TakeOutStep = 1;
            //}
        }

        private void TakeOutProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.TakeOutStep == 2)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM340().Result;
                if (result.Success && (bool)result.Data == false)
                {
                    TakeOutNext();
                    break;
                }

            }
        }
        private string _TakeOutFirstColor = "yellow";
        public string TakeOutFirstColor
        {
            get { return _TakeOutFirstColor; }
            set { _TakeOutFirstColor = value; RaisePropertyChanged(); }
        }
        private string _TakeOutSecondColor = "gray";
        public string TakeOutSecondColor
        {
            get { return _TakeOutSecondColor; }
            set { _TakeOutSecondColor = value; RaisePropertyChanged(); }
        }

        private string _TakeOutThirdColor = "gray";
        public string TakeOutThirdColor
        {
            get { return _TakeOutThirdColor; }
            set { _TakeOutThirdColor = value; RaisePropertyChanged(); }
        }

        private string _TakeOutTrayNumber = "";
        public string TakeOutTrayNumber
        {
            get { return _TakeOutTrayNumber; }
            set { _TakeOutTrayNumber = value; RaisePropertyChanged(); }
        }

        private bool _TakeOutTrayInputEnabled = true;
        public bool TakeOutTrayInputEnabled
        {
            get { return _TakeOutTrayInputEnabled; }
            set { _TakeOutTrayInputEnabled = value; RaisePropertyChanged(); }
        }


        private string _TakeOutLightX = "0";
        public string TakeOutLightX
        {
            get { return _TakeOutLightX; }
            set { _TakeOutLightX = value; RaisePropertyChanged(); }
        }

        private string _TakeOutLightXLenght = "1";
        public string TakeOutLightXLenght
        {
            get { return _TakeOutLightXLenght; }
            set { _TakeOutLightXLenght = value; RaisePropertyChanged(); }
        }

        private bool _TakeOutLightXInputEnabled = true;
        public bool TakeOutLightXInputEnabled
        {
            get { return _TakeOutLightXInputEnabled; }
            set { _TakeOutLightXInputEnabled = value; RaisePropertyChanged(); }
        }

        #endregion


        #region 存入
        private RelayCommand _TakeInNextCommand;

        public RelayCommand TakeInNextCommand
        {
            get
            {
                if (_TakeInNextCommand == null)
                {
                    _TakeInNextCommand = new RelayCommand(() => TakeInNext());
                }
                return _TakeInNextCommand;
            }
        }
        private int _TakeInStep = 1;
        public int TakeInStep
        {
            get { return _TakeInStep; }
            set { _TakeInStep = value; RaisePropertyChanged(); }
        }

        private bool _TakeInNextEnable = true;
        public bool TakeInNextEnable
        {
            get { return _TakeInNextEnable; }
            set { _TakeInNextEnable = value; RaisePropertyChanged(); }
        }
        public async void TakeInNext()
        {
            //ManualInViewModel manualInViewModel = new ManualInViewModel();
            //manualInViewModel.ChangeColor("FourthStep");
            if (!int.TryParse(this.TakeInTrayNumber, out int trayNumber))
            {
                Msg.Error("输入的托盘号格式不正确");
                return;
            }
            //Msg.Error("输入的托盘号格式不正确"+ TakeInTrayNumber);
            //return;
            string CurrentRunningTray = this.TakeInTrayNumber;
            if (!string.IsNullOrEmpty(CurrentRunningTray))
            {
                // await dialog.c
                // 读取PLC 状态信息
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();


                string LastRunningTray = "";
                string cfgINI = AppDomain.CurrentDomain.BaseDirectory + wms.Client.LogicCore.Configuration.SerivceFiguration.INI_CFG;
                if (System.IO.File.Exists(cfgINI))
                {
                    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                    LastRunningTray = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                }


                if (CurrentRunningTray!= LastRunningTray)
                {
                   // Msg.Question("");
                   // return;


                    if (await Msg.Question("输入的托盘号与上一次取出的托盘号不对,禁止非维修人员执行,是否强制执行??",true) == true)
                    {

                    }
                    else
                    {
                        return;
                    }
                }


                var runingEntity = new wms.Client.Model.Entity.RunningContainer()
                {
                    ContainerCode = ContainerCode,
                    TrayCode = Convert.ToInt32(CurrentRunningTray),
                    //XLight = XLight
                };
                var container = ContainerRepository.Query().FirstOrDefault(a => a.Code == ContainerCode);
                if (container != null)
                {
                    runingEntity.ContainerType = container.ContainerType;
                    runingEntity.IpAddress = container.Ip;
                    runingEntity.Port = int.Parse(container.Port);
                }

                // 货柜运行
                //  var runningContainer = baseControlService.WriteD650_In(runingEntity).Result;
                var runningContainer = baseControlService.PostStartRestoreContainer(runingEntity).Result;
                if (runningContainer.Success)
                {
                    //var result = baseControlService.SetM654True().Result;
                    //if (!result.Success)
                    //{
                    //    Msg.Error(result.Message);
                    //}
                }
                else
                {
                    Msg.Error(runningContainer.Message);
                }

                GlobalData.IsFocus = true;
                // baseControlService.PostStartRunningContainer(runingEntity);
            }
            //if (this.TakeInStep == 1)
            //{
            //    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            //    Model.Entity.RunningContainer runningContainer = new Model.Entity.RunningContainer();
            //    if (!int.TryParse(this.TakeInTrayNumber, out int trayNumber))
            //    {
            //        Msg.Error("输入的托盘号格式不正确");
            //        return;
            //    }
            //    runningContainer.TrayCode = trayNumber;
            //    var result = baseControlService.WriteD650_In(runningContainer).Result;
            //    if (result.Success)
            //    {
            //        TakeInFirstColor = "green";
            //        TakeInSecondColor = "yellow";
            //        this.TakeInTrayInputEnabled = false;
            //        this.TakeInStep++;
            //    }
            //    else
            //    {
            //        Msg.Error(result.Message);
            //    }

            //}
            //else if (this.TakeInStep == 2)
            //{

            //    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            //    var result = baseControlService.SetM654True().Result;
            //    if (result.Success)
            //    {
            //        TakeInFirstColor = "green";
            //        TakeInSecondColor = "green";
            //        this.TakeInStep++;
            //    }
            //    else
            //    {
            //        Msg.Error(result.Message);
            //    }
            //}
            //else if (this.TakeInStep == 3)
            //{

            //    TakeInFirstColor = "yellow";
            //    TakeInSecondColor = "gray";
            //    this.TakeInStep = 1;
            //    this.TakeInTrayInputEnabled = true;

            //    this.TakeInTrayNumber = "";

            //}
            //else
            //{
            //    TakeInFirstColor = "yellow";
            //    TakeInSecondColor = "gray";
            //    this.TakeInStep = 1;
            //    this.TakeInTrayInputEnabled = true;

            //    this.TakeInTrayNumber = "";
            //}
        }

        private void TakeInProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.TakeInStep == 2)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM340().Result;
                if (result.Success && (bool)result.Data == false)
                {
                    TakeInNext();
                    break;
                }

            }
        }
        private string _TakeInFirstColor = "yellow";
        public string TakeInFirstColor
        {
            get { return _TakeInFirstColor; }
            set { _TakeInFirstColor = value; RaisePropertyChanged(); }
        }
        private string _TakeInSecondColor = "gray";
        public string TakeInSecondColor
        {
            get { return _TakeInSecondColor; }
            set { _TakeInSecondColor = value; RaisePropertyChanged(); }
        }

        private string _TakeInThirdColor = "";
        public string TakeInThirdColor
        {
            get { return _TakeInThirdColor; }
            set { _TakeInThirdColor = value; RaisePropertyChanged(); }
        }

        private  string _TakeInTrayNumber = "";
        public string TakeInTrayNumber
        {
            get { return _TakeInTrayNumber; }
            set { _TakeInTrayNumber = value; RaisePropertyChanged(); }
        }

        private bool _TakeInTrayInputEnabled = false;
        public bool TakeInTrayInputEnabled
        {
            get { return _TakeInTrayInputEnabled; }
            set { _TakeInTrayInputEnabled = value; RaisePropertyChanged(); }
        }


        private string _TakeInLightX = "";
        public string TakeInLightX
        {
            get { return _TakeInLightX; }
            set { _TakeInLightX = value; RaisePropertyChanged(); }
        }

        private bool _TakeInLightXInputEnabled = true;
        public bool TakeInLightXInputEnabled
        {
            get { return _TakeInLightXInputEnabled; }
            set { _TakeInLightXInputEnabled = value; RaisePropertyChanged(); }
        }

        #endregion
        #endregion

        #region 自动门行程学习
        private RelayCommand _AutomaticNextCommand;

        public RelayCommand AutomaticNextCommand
        {
            get
            {
                if (_AutomaticNextCommand == null)
                {
                    _AutomaticNextCommand = new RelayCommand(() => AutomaticNext());
                }
                return _AutomaticNextCommand;
            }
        }
        private RelayCommand _AutomaticOpenDoorCommand;

        public RelayCommand AutomaticOpenDoorCommand
        {
            get
            {
                if (_AutomaticOpenDoorCommand == null)
                {
                    _AutomaticOpenDoorCommand = new RelayCommand(() => AutomaticOpenDoor());
                }
                return _AutomaticOpenDoorCommand;
            }
        }
        private RelayCommand _AutomaticCloseDoorCommand;

        public RelayCommand AutomaticCloseDoorCommand
        {
            get
            {
                if (_AutomaticCloseDoorCommand == null)
                {
                    _AutomaticCloseDoorCommand = new RelayCommand(() => AutomaticCloseDoor());
                }
                return _AutomaticCloseDoorCommand;
            }
        }
        private int _AutomaticStep = 1;
        public int AutomaticStep
        {
            get { return _AutomaticStep; }
            set { _AutomaticStep = value; RaisePropertyChanged(); }
        }

        private bool _AutomaticNextEnable = true;
        public bool AutomaticNextEnable
        {
            get { return _AutomaticNextEnable; }
            set { _AutomaticNextEnable = value; RaisePropertyChanged(); }
        }

        private bool _AutomaticOpenDoorEnable = true;
        public bool AutomaticOpenDoorEnable
        {
            get { return _AutomaticOpenDoorEnable; }
            set { _AutomaticOpenDoorEnable = value; RaisePropertyChanged(); }
        }

        private bool _AutomaticCloseDoorEnable = true;
        public bool AutomaticCloseDoorEnable
        {
            get { return _AutomaticCloseDoorEnable; }
            set { _AutomaticCloseDoorEnable = value; RaisePropertyChanged(); }
        }
        public async void AutomaticNext()
        {
            //this.AutomaticStep++;
            //if (this.AutomaticStep>this.AutomaticList.Count+1)
            //{
            //    this.AutomaticStep = 1;
            //}

            if (this.AutomaticStep == 1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.SetM8True().Result;
                if (result.Success)
                {
                    AutomaticFirstColor = "green";
                    AutomaticSecondColor = "yellow";
                    // this.AutomaticNextEnable = false;
                    this.AutomaticOpenDoorEnable = false;
                    this.AutomaticCloseDoorEnable = false;
                    this.AutomaticStep++;
                    //定义一个线程
                   //   ThreadPool.QueueUserWorkItem(new WaitCallback(AutomaticProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.AutomaticStep == 2)
            {
                //AutomaticFirstColor = "green";
                //AutomaticSecondColor = "green";
                //AutomaticThirdColor = "yellow";
                //this.AutomaticStep++;
                //this.AutomaticNextEnable = true;
                AutomaticFirstColor = "yellow";
                AutomaticSecondColor = "gray";
                AutomaticThirdColor = "gray";
                this.AutomaticStep = 1;
                this.AutomaticOpenDoorEnable = true;
                this.AutomaticCloseDoorEnable = true;
            }
            else if (this.AutomaticStep == 3)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.FinishM491().Result;
                if (result.Success)
                {
                    AutomaticFirstColor = "green";
                    AutomaticSecondColor = "green";
                    AutomaticThirdColor = "green";
                    this.AutomaticStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.AutomaticStep == 4)
            {
                AutomaticFirstColor = "yellow";
                AutomaticSecondColor = "gray";
                AutomaticThirdColor = "gray";
                this.AutomaticStep = 1;
            }
            else
            {
                AutomaticFirstColor = "yellow";
                AutomaticSecondColor = "gray";
                AutomaticThirdColor = "gray";
                this.AutomaticStep = 1;
            }
        }

        public void AutomaticOpenDoor()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.SetM9True().Result;
            if (result.Success)
            {
                Msg.Info("自动门开启成功");

            }
            else
            {
                Msg.Error(result.Message);
            }
        }

        public void AutomaticCloseDoor()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.SetM10True().Result;
            if (result.Success)
            {
                Msg.Info("自动门关闭成功");

            }
            else
            {
                Msg.Error(result.Message);
            }
        }
        private void AutomaticProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.AutomaticStep == 2)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM490().Result;
                if (result.Success && (bool)result.Data == false)
                {
                    AutomaticNext();
                    break;
                }

            }
        }
        private string _AutomaticFirstColor = "yellow";
        public string AutomaticFirstColor
        {
            get { return _AutomaticFirstColor; }
            set { _AutomaticFirstColor = value; RaisePropertyChanged(); }
        }
        private string _AutomaticSecondColor = "gray";
        public string AutomaticSecondColor
        {
            get { return _AutomaticSecondColor; }
            set { _AutomaticSecondColor = value; RaisePropertyChanged(); }
        }

        private string _AutomaticThirdColor = "gray";
        public string AutomaticThirdColor
        {
            get { return _AutomaticThirdColor; }
            set { _AutomaticThirdColor = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 托盘扫描
        private RelayCommand _TrayScanNextCommand;

        public RelayCommand TrayScanNextCommand
        {
            get
            {
                if (_TrayScanNextCommand == null)
                {
                    _TrayScanNextCommand = new RelayCommand(() => TrayScanNext());
                }
                return _TrayScanNextCommand;
            }
        }

        private RelayCommand _TrayScanFourStepCommand;

        public RelayCommand TrayScanFourStepCommand
        {
            get
            {
                if (_TrayScanFourStepCommand == null)
                {
                    _TrayScanFourStepCommand = new RelayCommand(() => TrayScanFourStep());
                }
                return _TrayScanFourStepCommand;
            }
        }
        private int _TrayScanStep = 1;
        public int TrayScanStep
        {
            get { return _TrayScanStep; }
            set { _TrayScanStep = value; RaisePropertyChanged(); }
        }

        private bool _TrayScanNextEnable = true;
        public bool TrayScanNextEnable
        {
            get { return _TrayScanNextEnable; }
            set { _TrayScanNextEnable = value; RaisePropertyChanged(); }
        }
        private bool _TrayScanFourStepEnable = true;
        public bool TrayScanFourStepEnable
        {
            get { return _TrayScanFourStepEnable; }
            set { _TrayScanFourStepEnable = value; RaisePropertyChanged(); }
        }

        

        public async void TrayScanNext()
        {
            //this.TrayScanStep++;
            //if (this.TrayScanStep>this.TrayScanList.Count+1)
            //{
            //    this.TrayScanStep = 1;
            //}

            if (this.TrayScanStep == 1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.StartM350().Result;
                if (result.Success)
                {
                    TrayScanFirstColor = "green";
                    TrayScanSecondColor = "yellow";
                  //  this.TrayScanNextEnable = false;
                    this.TrayScanStep++;
                    //定义一个线程
                   // ThreadPool.QueueUserWorkItem(new WaitCallback(TrayScanProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.TrayScanStep == 2)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.SetM390True().Result;
                if (result.Success)
                {
                    TrayScanSecondColor = "green";
                    TrayScanThirdColor = "yellow";
                    this.TrayScanStep++;
                    //定义一个线程
                    // ThreadPool.QueueUserWorkItem(new WaitCallback(TrayScanProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else if (this.TrayScanStep == 3)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.StartM391().Result;
                if (result.Success)
                {
                    TrayScanThirdColor = "green";
                    TrayScanFourthColor = "yellow";
                    this.TrayScanStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.TrayScanStep == 4)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.SetM370True().Result;
                if (result.Success)
                {
                    TrayScanFourthColor = "green";
                    TrayScanFifthColor = "yellow";
                    this.TrayScanStep++;
                    this.TrayInputEnable = true;
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else if (this.TrayScanStep == 5)
            {
                
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                Model.Entity.RunningContainer runningContainer = new Model.Entity.RunningContainer();
                if (!int.TryParse(this.TrayNumber, out int traNumber))
                {
                    Msg.Error("输入的托盘号格式不正确");
                    return;
                }
                runningContainer.TrayCode = traNumber;
                var result = baseControlService.WriteD392(runningContainer).Result;
                if (result.Success)
                {
                    TrayScanFifthColor = "green";
                    TrayScanSixthColor = "yellow";
                    this.TrayScanStep++;
                    this.TrayInputEnable = false;
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else if (this.TrayScanStep == 6)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.SetM371True().Result;
                if (result.Success)
                {

                    TrayScanSixthColor = "green";
                    TrayScanSeventhColor = "yellow";
                    this.TrayScanStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else if (this.TrayScanStep == 7)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.ConfirmM394().Result;
                if (result.Success)
                {
                    TrayScanSeventhColor = "green";
                    TrayScanEighthColor = "yellow";
                    this.TrayScanStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else if (this.TrayScanStep == 8)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.GetM395().Result;
                if (result.Success )
                {
                    if ((bool)result.Data==true)
                    {
                       
                        TrayScanEighthColor = "green";
                        TrayScanNinthColor = "yellow";
                        this.TrayScanStep++;
                    }
                    else
                    {
                        //返回第四步
                        this.TrayScanStep = 4;
                        this.TrayNumber = "";
                        TrayScanFourthColor = "yellow";
                        TrayScanFifthColor = "gray";
                        TrayScanSixthColor = "gray";
                        TrayScanSeventhColor = "gray";
                        TrayScanEighthColor = "gray";
                        TrayScanNinthColor = "gray";
                    }
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else if (this.TrayScanStep == 9)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.ConfirmM396().Result;
                if (result.Success)
                {
                    TrayScanNinthColor = "green";
                    this.TrayScanStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else
            {
                TrayScanFirstColor = "yellow";
                TrayScanSecondColor = "gray";
                TrayScanThirdColor = "gray";
                TrayScanFourthColor = "gray";
                TrayScanFifthColor = "gray";
                TrayScanSixthColor = "gray";
                TrayScanSeventhColor = "gray";
                TrayScanEighthColor = "gray";
                TrayScanNinthColor = "gray";
                this.TrayNumber = "";
                this.TrayScanStep = 1;
            }
        }

        public  void TrayScanFourStep()
        {
            //返回第四步
            this.TrayScanStep = 3;
            this.TrayNumber = "";
            TrayScanFirstColor = "green";
            TrayScanSecondColor = "green";
            TrayScanThirdColor = "yellow";
            TrayScanFourthColor = "gray";
            TrayScanFifthColor = "gray";
            TrayScanSixthColor = "gray";
            TrayScanSeventhColor = "gray";
            TrayScanEighthColor = "gray";
            TrayScanNinthColor = "gray";

            this.TrayScanFourStepEnable = false;



        }

        private void TrayScanProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.TrayScanStep == 2)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM490().Result;
                if (result.Success && (bool)result.Data == false)
                {
                    TrayScanNext();
                    break;
                }

            }
        }




        private string _TrayScanFirstColor = "yellow";
        public string TrayScanFirstColor
        {
            get { return _TrayScanFirstColor; }
            set { _TrayScanFirstColor = value; RaisePropertyChanged(); }
        }
        private string _TrayScanSecondColor = "gray";
        public string TrayScanSecondColor
        {
            get { return _TrayScanSecondColor; }
            set { _TrayScanSecondColor = value; RaisePropertyChanged(); }
        }

        private string _TrayScanThirdColor = "gray";
        public string TrayScanThirdColor
        {
            get { return _TrayScanThirdColor; }
            set { _TrayScanThirdColor = value; RaisePropertyChanged(); }
        }
        private string _TrayScanFourthColor = "gray";
        public string TrayScanFourthColor
        {
            get { return _TrayScanFourthColor; }
            set { _TrayScanFourthColor = value; RaisePropertyChanged(); }
        }
        private string _TrayScanFifthColor = "gray";
        public string TrayScanFifthColor
        {
            get { return _TrayScanFifthColor; }
            set { _TrayScanFifthColor = value; RaisePropertyChanged(); }
        }
        private string _TrayScanSixthColor = "gray";
        public string TrayScanSixthColor
        {
            get { return _TrayScanSixthColor; }
            set { _TrayScanSixthColor = value; RaisePropertyChanged(); }
        }
        private string _TrayScanSeventhColor = "gray";
        public string TrayScanSeventhColor
        {
            get { return _TrayScanSeventhColor; }
            set { _TrayScanSeventhColor = value; RaisePropertyChanged(); }
        }
        private string _TrayScanEighthColor = "gray";
        public string TrayScanEighthColor
        {
            get { return _TrayScanEighthColor; }
            set { _TrayScanEighthColor = value; RaisePropertyChanged(); }
        }
        private string _TrayScanNinthColor = "gray";
        public string TrayScanNinthColor
        {
            get { return _TrayScanNinthColor; }
            set { _TrayScanNinthColor = value; RaisePropertyChanged(); }
        }
        
        private bool _TrayInputEnable = false;


        public bool TrayInputEnable
        {
            get { return _TrayInputEnable; }
            set { _TrayInputEnable = value; RaisePropertyChanged(); }
        }
        private string _TrayNumber = "";
        public string TrayNumber
        {
            get { return _TrayNumber; }
            set { _TrayNumber = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 添加托盘
        private RelayCommand _AddTrayNextCommand;

        public RelayCommand AddTrayNextCommand
        {
            get
            {
                if (_AddTrayNextCommand == null)
                {
                    _AddTrayNextCommand = new RelayCommand(() => AddTrayNext());
                }
                return _AddTrayNextCommand;
            }
        }
        private int _AddTrayStep = 1;
        public int AddTrayStep
        {
            get { return _AddTrayStep; }
            set { _AddTrayStep = value; RaisePropertyChanged(); }
        }

        private bool _AddTrayNextEnable = true;
        public bool AddTrayNextEnable
        {
            get { return _AddTrayNextEnable; }
            set { _AddTrayNextEnable = value; RaisePropertyChanged(); }
        }
        public async void AddTrayNext()
        {
            //this.AddTrayStep++;
            //if (this.AddTrayStep>this.AddTrayList.Count+1)
            //{
            //    this.AddTrayStep = 1;
            //}

            if (this.AddTrayStep == 1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                Model.Entity.RunningContainer runningContainer = new Model.Entity.RunningContainer();
                if (!int.TryParse(this.AddTrayNumber,out int trayNumber))
                {
                    Msg.Error("输入的托盘号格式不正确");
                    return;
                }
                runningContainer.TrayCode = trayNumber;
                var result = baseControlService.WriteD700(runningContainer).Result;
                if (result.Success)
                {
                    AddTrayFirstColor = "green";
                    AddTraySecondColor = "yellow";
                    this.AddTrayStep++;
                    this.AddInputTrayEnabled = false;
                    //定义一个线程
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(AddTrayProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.AddTrayStep == 2)
            {

                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.StartM700().Result;
                if (result.Success)
                {
                    AddTraySecondColor = "green";
                    AddTrayThirdColor = "yellow";
                    this.AddTrayStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else if (this.AddTrayStep == 3)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.GetM701().Result;
                if (result.Success)
                {
                    if ((bool)result.Data==true)
                    {
                        AddTrayThirdColor = "green";
                        AddTrayFourthColor = "yellow";
                        this.AddTrayStep++;
                        //Msg.Info("空间足够");
                    }
                    else
                    {
                        Msg.Warning("空间不足");
                    }
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.AddTrayStep == 4)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.ConfirmM702().Result;
                if (result.Success)
                {
                    AddTrayFourthColor = "green";
                    this.AddTrayStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else
            {
                AddTrayFirstColor = "yellow";
                AddTraySecondColor = "gray";
                AddTrayThirdColor = "gray";
                AddTrayFourthColor = "gray";
                this.AddTrayStep = 1;
                this.AddTrayNumber = "";
                this.AddInputTrayEnabled = true;
            }
        }

        private void AddTrayProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.AddTrayStep == 2)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM490().Result;
                if (result.Success && (bool)result.Data == false)
                {
                    AddTrayNext();
                    break;
                }

            }
        }
        private string _AddTrayFirstColor = "yellow";
        public string AddTrayFirstColor
        {
            get { return _AddTrayFirstColor; }
            set { _AddTrayFirstColor = value; RaisePropertyChanged(); }
        }
        private string _AddTraySecondColor = "gray";
        public string AddTraySecondColor
        {
            get { return _AddTraySecondColor; }
            set { _AddTraySecondColor = value; RaisePropertyChanged(); }
        }

        private string _AddTrayThirdColor = "gray";
        public string AddTrayThirdColor
        {
            get { return _AddTrayThirdColor; }
            set { _AddTrayThirdColor = value; RaisePropertyChanged(); }
        }
        private string _AddTrayFourthColor = "gray";
        public string AddTrayFourthColor
        {
            get { return _AddTrayFourthColor; }
            set { _AddTrayFourthColor = value; RaisePropertyChanged(); }
        }

        private string _AddTrayNumber = "";
        public string AddTrayNumber
        {
            get { return _AddTrayNumber; }
            set { _AddTrayNumber = value; RaisePropertyChanged(); }
        }

        private bool _AddInputTrayEnabled = true;
        public bool AddInputTrayEnabled
        {
            get { return _AddInputTrayEnabled; }
            set { _AddInputTrayEnabled = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 删除托盘
        private RelayCommand _DeleteTrayNextCommand;

        public RelayCommand DeleteTrayNextCommand
        {
            get
            {
                if (_DeleteTrayNextCommand == null)
                {
                    _DeleteTrayNextCommand = new RelayCommand(() => DeleteTrayNext());
                }
                return _DeleteTrayNextCommand;
            }
        }
        private int _DeleteTrayStep = 1;
        public int DeleteTrayStep
        {
            get { return _DeleteTrayStep; }
            set { _DeleteTrayStep = value; RaisePropertyChanged(); }
        }

        private bool _DeleteTrayNextEnable = true;
        public bool DeleteTrayNextEnable
        {
            get { return _DeleteTrayNextEnable; }
            set { _DeleteTrayNextEnable = value; RaisePropertyChanged(); }
        }
        public async void DeleteTrayNext()
        {
            if (this.DeleteTrayStep == 1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                Model.Entity.RunningContainer runningContainer = new Model.Entity.RunningContainer();
                if (!int.TryParse(this.DeleteTrayNumber, out int trayNumber))
                {
                    Msg.Error("输入的托盘号格式不正确");
                    return;
                }
                runningContainer.TrayCode = trayNumber;
                var result = baseControlService.WriteD750(runningContainer).Result;
                if (result.Success)
                {
                    DeleteTrayFirstColor = "green";
                    DeleteTraySecondColor = "yellow";
                    this.DeleteTrayStep++;
                    this.DeleteTrayInputEnabled = false;
                    //定义一个线程
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteTrayProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.DeleteTrayStep == 2)
            {

                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.StartM750().Result;
                if (result.Success)
                {
                    DeleteTraySecondColor = "green";
                    DeleteTrayThirdColor = "yellow";
                    this.DeleteTrayStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }
            else if (this.DeleteTrayStep == 3)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.GetD751().Result;
                if (result.Success)
                {
                    DeleteTrayThirdColor = "green";
                    DeleteTrayFourthColor = "yellow";
                    this.DeleteTrayStep++;
                    Msg.Info("托架号:" + result.Data.ToString().ToString());
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.DeleteTrayStep == 4)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.StartM751().Result;
                if (result.Success)
                {
                    DeleteTrayFourthColor = "green";
                    DeleteTrayFifthColor = "yellow";
                    this.DeleteTrayStep++;
                    this.DeleteTrayNextEnable = false;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteTrayProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }
            }

            else if (this.DeleteTrayStep == 5)
            {
                DeleteTrayFifthColor = "green";
                DeleteTraySixthColor = "yellow";
                this.DeleteTrayStep++;
                this.DeleteTrayNextEnable = true;
            }
            else if (this.DeleteTrayStep == 6)
            {
                if (await Msg.Question("确认删除?") == true)
                {
                    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                    var result = baseControlService.ConfirmM753().Result;
                    if (result.Success)
                    {
                        DeleteTraySixthColor = "green";
                        this.DeleteTrayStep++;
                    }
                    else
                    {
                        Msg.Error(result.Message);
                    }
                }
                else
                {
                    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                    var result = baseControlService.ConfirmM754().Result;
                    if (result.Success)
                    {
                        DeleteTraySixthColor = "green";
                        this.DeleteTrayStep++;
                    }
                    else
                    {
                        Msg.Error(result.Message);
                    }
                }



            }
            else
            {
                DeleteTrayFirstColor = "yellow";
                DeleteTraySecondColor = "gray";
                DeleteTrayThirdColor = "gray";
                DeleteTrayFourthColor = "gray";
                DeleteTrayFifthColor = "gray";
                DeleteTraySixthColor = "gray";
                this.DeleteTrayStep = 1;
                this.DeleteTrayNumber = "";
                this.DeleteTrayInputEnabled = true;
            }
        }

        private void DeleteTrayProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.DeleteTrayStep == 5)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM752().Result;
                if (result.Success && (bool)result.Data == false)
                {
                    DeleteTrayNext();
                    break;
                }

            }
        }
        private string _DeleteTrayFirstColor = "yellow";
        public string DeleteTrayFirstColor
        {
            get { return _DeleteTrayFirstColor; }
            set { _DeleteTrayFirstColor = value; RaisePropertyChanged(); }
        }
        private string _DeleteTraySecondColor = "gray";
        public string DeleteTraySecondColor
        {
            get { return _DeleteTraySecondColor; }
            set { _DeleteTraySecondColor = value; RaisePropertyChanged(); }
        }

        private string _DeleteTrayThirdColor = "gray";
        public string DeleteTrayThirdColor
        {
            get { return _DeleteTrayThirdColor; }
            set { _DeleteTrayThirdColor = value; RaisePropertyChanged(); }
        }
        private string _DeleteTrayFourthColor = "gray";
        public string DeleteTrayFourthColor
        {
            get { return _DeleteTrayFourthColor; }
            set { _DeleteTrayFourthColor = value; RaisePropertyChanged(); }
        }
        private string _DeleteTrayFifthColor = "gray";
        public string DeleteTrayFifthColor
        {
            get { return _DeleteTrayFifthColor; }
            set { _DeleteTrayFifthColor = value; RaisePropertyChanged(); }
        }
        private string _DeleteTraySixthColor = "gray";
        public string DeleteTraySixthColor
        {
            get { return _DeleteTraySixthColor; }
            set { _DeleteTraySixthColor = value; RaisePropertyChanged(); }
        }

        private string _DeleteTrayNumber = "";
        public string DeleteTrayNumber
        {
            get { return _DeleteTrayNumber; }
            set { _DeleteTrayNumber = value; RaisePropertyChanged(); }
        }

        private bool _DeleteTrayInputEnabled = true;
        public bool DeleteTrayInputEnabled
        {
            get { return _DeleteTrayInputEnabled; }
            set { _DeleteTrayInputEnabled = value; RaisePropertyChanged(); }
        }
        #endregion


        #region 整理存储空间
        private RelayCommand _OrganizeFrontCommand;

        public RelayCommand OrganizeFrontCommand
        {
            get
            {
                if (_OrganizeFrontCommand == null)
                {
                    _OrganizeFrontCommand = new RelayCommand(() => OrganizeFront());
                }
                return _OrganizeFrontCommand;
            }
        }


        private RelayCommand _OrganizeBehindCommand;

        public RelayCommand OrganizeBehindCommand
        {
            get
            {
                if (_OrganizeBehindCommand == null)
                {
                    _OrganizeBehindCommand = new RelayCommand(() => OrganizeBehind());
                }
                return _OrganizeBehindCommand;
            }
        }
        private int _OrganizeStep = 1;
        public int OrganizeStep
        {
            get { return _OrganizeStep; }
            set { _OrganizeStep = value; RaisePropertyChanged(); }
        }

        private bool _OrganizeNextEnable = true;
        public bool OrganizeNextEnable
        {
            get { return _OrganizeNextEnable; }
            set { _OrganizeNextEnable = value; RaisePropertyChanged(); }
        }

        public async void OrganizeFront()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.StartM810().Result;
            if (result.Success)
            {
                Msg.Info("整理前侧空间成功");
            }
            else
            {
                Msg.Error(result.Message);
            }
        }
        public async void OrganizeBehind()
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            var result = baseControlService.StartM800().Result;
            if (result.Success)
            {
                Msg.Info("整理后侧空间成功");
            }
            else
            {
                Msg.Error(result.Message);
            }
        }
        public async void OrganizeNext()
        {
            //this.OrganizeStep++;
            //if (this.OrganizeStep>this.OrganizeList.Count+1)
            //{
            //    this.OrganizeStep = 1;
            //}

            if (this.OrganizeStep == 1)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.StartM800().Result;
                if (result.Success)
                {
                    OrganizeFirstColor = "green";
                    OrganizeSecondColor = "yellow";
                    this.OrganizeNextEnable = false;
                    this.OrganizeStep++;
                    //定义一个线程
                    ThreadPool.QueueUserWorkItem(new WaitCallback(OrganizeProgress), this);
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.OrganizeStep == 2)
            {
                OrganizeFirstColor = "green";
                OrganizeSecondColor = "green";
                OrganizeThirdColor = "yellow";
                this.OrganizeStep++;
                this.OrganizeNextEnable = true;
            }
            else if (this.OrganizeStep == 3)
            {
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
                var result = baseControlService.ConfirmM802().Result;
                if (result.Success)
                {
                    OrganizeFirstColor = "green";
                    OrganizeSecondColor = "green";
                    OrganizeThirdColor = "green";
                    this.OrganizeStep++;
                }
                else
                {
                    Msg.Error(result.Message);
                }

            }
            else if (this.OrganizeStep == 4)
            {
                OrganizeFirstColor = "yellow";
                OrganizeSecondColor = "gray";
                OrganizeThirdColor = "gray";
                this.OrganizeStep = 1;
            }
            else
            {
                OrganizeFirstColor = "yellow";
                OrganizeSecondColor = "gray";
                OrganizeThirdColor = "gray";
                this.OrganizeStep = 1;
            }
        }

        private void OrganizeProgress(object obj)
        {
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();
            while (this.OrganizeStep == 2)
            {
                Thread.Sleep(5000);
                var result = baseControlService.GetM801().Result;
                if (result.Success && (bool)result.Data == false)
                {
                    result = baseControlService.GetD800().Result;
                    OrganizeNext();
                    App.Current.Dispatcher.Invoke(new Action(() => {

                        Msg.Info("空间利用率:" + result.Data.ToString());
                    }));
                      
                    break;
                }

            }
        }
        private string _OrganizeFirstColor = "yellow";
        public string OrganizeFirstColor
        {
            get { return _OrganizeFirstColor; }
            set { _OrganizeFirstColor = value; RaisePropertyChanged(); }
        }
        private string _OrganizeSecondColor = "gray";
        public string OrganizeSecondColor
        {
            get { return _OrganizeSecondColor; }
            set { _OrganizeSecondColor = value; RaisePropertyChanged(); }
        }

        private string _OrganizeThirdColor = "gray";
        public string OrganizeThirdColor
        {
            get { return _OrganizeThirdColor; }
            set { _OrganizeThirdColor = value; RaisePropertyChanged(); }
        }
        #endregion
    }
}
