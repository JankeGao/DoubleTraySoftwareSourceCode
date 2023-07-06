using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using GalaSoft.MvvmLight.Command;
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
    [Module(ModuleType.EquipmentManagement, "AlarmDlg", "设备报警")]
    public class AlarmViewModel : DataProcess<DeviceAlarm>
    {

        /// <summary>
        /// 任务契约
        /// </summary>
        private readonly IDeviceAlarmContract DeviceAlarmContract;


        public AlarmViewModel()
        {
            DeviceAlarmContract = IocResolver.Resolve<IDeviceAlarmContract>();
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



        private RelayCommand _RestAllCommand;

        public RelayCommand RestAllCommand
        {
            get
            {
                if (_RestAllCommand == null)
                {
                    _RestAllCommand = new RelayCommand(() => RestAllAlarm());
                }
                return _RestAllCommand;
            }
        }


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
            GlobalData.LoginModule = "DeviceAlarm";
            GlobalData.LoginPageCode = "AlarmDlg";
            GlobalData.LoginPageName = "设备报警";
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
                DateTime dateTime = DateTime.Parse( DateTime.Now.ToString("yyyy-MM-dd"));
                var alarmList = DeviceAlarmContract.DeviceAlarmDtos.Where(a => a.ContainerCode == ContainerCode && a.CreatedTime> dateTime).OrderByDesc(a=>a.CreatedTime).ToList();
                //&& a.Status == (int)DeviceAlarmStateEnum.Urgencye
                DeviceAlarmList.Clear();
                alarmList.ForEach((arg) => DeviceAlarmList.Add(arg));
            }
            else    //如果未登录
            {
                var dialog = ServiceProvider.Instance.Get<IShowContent>();
                dialog.BindDataContext(new UserLoginWindow(), new UserLoginModel());
                dialog.Show();
            }
        }

        public async void RestAllAlarm()
        {
            // 读取PLC 状态信息
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

            // 物料实体映射
            var inTask = baseControlService.PostRestAllAlarm();
            // 设备在线
            if (inTask.Result.Success)
            {
                var deviceEnity = new DeviceAlarm()
                {
                    ContainerCode=ContainerCode
                };
                // 读取PLC 状态信息
                var alarmService = ServiceProvider.Instance.Get<IAlarmService>();
                var serverRest = alarmService.PostRestAllAlarmServer(deviceEnity);

                if (serverRest.Result.Success)
                {
                    // 报警弹窗复位
                    GlobalData.Comfirm = false;
                    // 获取当前设备下的所有报警信息
                    //var alarmList = DeviceAlarmContract.DeviceAlarmDtos.Where(a => a.ContainerCode == ContainerCode && a.Status == (int)DeviceAlarmStateEnum.Urgencye).ToList();
                    //DeviceAlarmList.Clear();
                    //alarmList.ForEach((arg) => DeviceAlarmList.Add(arg));
                    DateTime dateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                    var alarmList = DeviceAlarmContract.DeviceAlarmDtos.Where(a => a.ContainerCode == ContainerCode && a.CreatedTime > dateTime).OrderByDesc(a => a.CreatedTime).ToList();
                    //&& a.Status == (int)DeviceAlarmStateEnum.Urgencye
                    DeviceAlarmList.Clear();
                    alarmList.ForEach((arg) => DeviceAlarmList.Add(arg));
                    Msg.Warning("全部报警复位成功！");
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
                Msg.Warning(inTask.Result.Message);
                return;
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
            }
        }
    }
}
