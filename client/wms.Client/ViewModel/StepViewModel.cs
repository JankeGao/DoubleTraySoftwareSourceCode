using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
using wms.Client.Model.Entity;
using wms.Client.Service;
using wms.Client.UiCore.Template;
using wms.Client.View;
using wms.Client.ViewModel;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 系统设置
    /// </summary>
    [Module(ModuleType.SystemSettings, "StepDlg", "系统配置")]
    public class StepViewModel : ViewModelBase
    {

        private readonly string _ServerIP = ConfigurationManager.AppSettings["ServerIP"];
        private readonly string _DeviceIP = ConfigurationManager.AppSettings["DeviceIP"];

        private readonly IRepository<Bussiness.Entitys.Container, int> ContainerRepository;

        public StepViewModel()
        {

            ContainerRepository = IocResolver.Resolve<IRepository<Bussiness.Entitys.Container, int>>();
            LoginOutCommand = new RelayCommand<string>(LoginOut);
            RunningCommand = new RelayCommand(RunningContainer);
            RunningTakeInCommand = new RelayCommand(RunningTakeInContainer);
            SaveCommand = new RelayCommand(SaveSetting);
            HopperSettingCommand= new RelayCommand(GetHopperSetting);
            EmergencyDoorSettingCommand = new RelayCommand(GetEmergencyDoorSetting);
            //PrintItemCommand = new RelayCommand<InTaskMaterialDto>(PrintItem);
            this.ReadConfigInfo();
            this.ShowLogin();
        }


        private RelayCommand _userLoginCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> UserLoginCommand { get; private set; }

        /// <summary>
        /// 退出登录
        /// </summary>
        private RelayCommand _LoginOutCommand;

        public RelayCommand<string> LoginOutCommand { get; private set; }

        /// <summary>
        /// 保存
        /// </summary>
        private RelayCommand _SaveCommand;

        public RelayCommand SaveCommand { get; private set; }
        

        /// <summary>
        /// 驱动货柜运转
        /// </summary>
        private RelayCommand _RunningCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand RunningCommand { get; private set; }
        public RelayCommand RunningTakeInCommand { get; private set; }
        

        /// <summary>
        /// 驱动货柜运转
        /// </summary>
        private RelayCommand _HopperSetting;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand HopperSettingCommand { get; private set; }


        /// <summary>
        /// 驱动货柜运转
        /// </summary>
        private RelayCommand _EmergencyDoorSetting;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand EmergencyDoorSettingCommand { get; private set; }



        /// <summary>
        /// 设备IP
        /// </summary>
        private string deviceIP = string.Empty;
        public string DeviceIP
        {
            get { return deviceIP; }
            set { deviceIP = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 设备端口号
        /// </summary>
        private string serverIP = string.Empty;
        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前操作储位
        /// </summary>
        private int _XLight = 0;
        public int XLight
        {
            get { return _XLight; }
            set { _XLight = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前操作托盘号
        /// </summary>
        private string trayCode = string.Empty;
        public string TrayCode
        {
            get { return trayCode; }
            set { trayCode = value; RaisePropertyChanged(); }
        }

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
                    // 如果离线使用，不核验
                    if (GlobalData.OutLineUse)
                    {
                        return true;
                    }
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
        /// 退出登录
        /// </summary>
        /// <param name="code"></param>
        public async void LoginOut(string code)
        {
            // 重新读取登录信息
            if (await Msg.Question("是否退出登录？"))
            {
                // 清除登录信息
                GlobalData.loginTime = "";
                GlobalData.UserCode = "";
                GlobalData.UserName = "";
                GlobalData.PictureUrl = "";
                var obj = new MainViewModel();
                if (obj == null) return;
                obj.ExitPage(MenuBehaviorType.ExitAllPage, "");
            }
        }


        /// <summary>
        /// 核验登录人员
        /// </summary>
        /// <param name="code"></param>
        public async void ShowLogin()
        {
            // 入库任务
            GlobalData.LoginModule = "Role";
            GlobalData.LoginPageCode = "StepDlg";
            GlobalData.LoginPageName = "系统配置";
            //如果登录
            if (await CheckLogin())
            {
                // this.GetAllPageData();
                DeviceIP = _DeviceIP;
                ServerIP = _ServerIP;
                GlobalData.IsFocus = true;
            }
            else //如果未登录
            {
                var dialog = ServiceProvider.Instance.Get<IShowContent>();
                dialog.BindDataContext(new UserLoginWindow(), new UserLoginModel());
                dialog.Show();
            }
        }

        /// <summary>
        /// 启动货柜---系统设置
        /// </summary>
        public async void RunningContainer()
        {
            GlobalData.IsFocus = false;

            // await dialog.c
            // 读取PLC 状态信息
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

            //运行货柜传递的参数
            var runingEntity = new RunningContainer()
            {
                ContainerCode = ContainerCode,
                TrayCode = Convert.ToInt32(TrayCode),
                XLight = XLight
            };
            var container = ContainerRepository.Query().FirstOrDefault(a => a.Code == ContainerCode);
            if (container != null)
            {
                runingEntity.ContainerType = container.ContainerType;
                runingEntity.IpAddress = container.Ip;
                runingEntity.Port = int.Parse(container.Port);
            }
            // 货柜运行
            var runningContainer = baseControlService.PostStartRunningContainer(runingEntity);
            if (runningContainer.Result.Success)
            {
                GlobalData.IsFocus = true;
                Msg.Warning("货柜运行中");
            }
            else
            {
                GlobalData.IsFocus = true;
                Msg.Error(runningContainer.Result.Message);
            }
        }
        /// <summary>
        /// 存入货柜
        /// </summary>
        public async void RunningTakeInContainer()
        {
            try
            {
                if (GlobalData.DeviceStatus == (int)DeviceStatusEnum.Fault)
                {
                    GlobalData.IsFocus = true;
                    Msg.Warning("设备离线状态，无法启动货柜！");
                    return;
                }

                GlobalData.IsFocus = false;


                string CurrentRunningTray = "";
                string cfgINI = AppDomain.CurrentDomain.BaseDirectory + wms.Client.LogicCore.Configuration.SerivceFiguration.INI_CFG;
                if (System.IO.File.Exists(cfgINI))
                {
                    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                    CurrentRunningTray = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                }

                if (!string.IsNullOrEmpty(CurrentRunningTray))
                {
                    // await dialog.c
                    // 读取PLC 状态信息
                    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

                    var runingEntity = new RunningContainer()
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
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);

            }

        }
        /// <summary>
        /// 料斗行程设定
        /// </summary>
        public async void GetHopperSetting()
        {
            if (!await Msg.Question("料斗行程设定将驱动货柜运转3-5分钟，在此期间，请不要操作货柜，是否进行料斗行程设定？"))
            {
                return;
            }
            GlobalData.IsFocus = false;

            // await dialog.c
            // 读取PLC 状态信息
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

            // 货柜运行
            var runningContainer = baseControlService.GetHopperSetting();

            if (runningContainer.Result.Success)
            {
                GlobalData.IsFocus = true;
                //var dialog = ServiceProvider.Instance.Get<IShowContent>();
                //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "料斗行程设定中", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                //dialog.Show();
                //await Task.Delay(2000);
                //DialogHost.CloseDialogCommand.Execute(null, null);
                Msg.Warning("料斗行程设定中");

            }
            else
            {
                GlobalData.IsFocus = true;
                Msg.Error(runningContainer.Result.Message);
            }
        }

        /// <summary>
        /// 安全门行程设定
        /// </summary>
        public async void GetEmergencyDoorSetting()
        {
            if (!await Msg.Question("安全门行程将对安全门进行行程校验，是否进行安全门行程设定？"))
            {
                return;
            }

            GlobalData.IsFocus = false;

            // await dialog.c
            // 读取PLC 状态信息
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

            // 货柜运行
            var runningContainer = baseControlService.GetEmergencyDoorSetting();

            if (runningContainer.Result.Success)
            {
                //GlobalData.IsFocus = true;
                Msg.Warning("安全门行程设定中");
                //var dialog = ServiceProvider.Instance.Get<IShowContent>();
                //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "安全门行程设定中", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                //dialog.Show();
                //await Task.Delay(2000);
                //DialogHost.CloseDialogCommand.Execute(null, null);
            }
            else
            {
                GlobalData.IsFocus = true;
                Msg.Error(runningContainer.Result.Message);
            }
        }



        /// <summary>
        /// 核验登录人员
        /// </summary>
        /// <param name="code"></param>
        public async void SaveSetting()
        {
            if (String.IsNullOrEmpty(DeviceIP))
            {
                Msg.Error("设备服务API端口配置为空");
            }
            if (String.IsNullOrEmpty(ServerIP))
            {
                Msg.Error("服务端API端口配置为空");
            }

            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings["ServerIP"].Value = ServerIP;
                configuration.AppSettings.Settings["DeviceIP"].Value = DeviceIP;
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                Msg.Info("配置保存成功");
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
    }
}
