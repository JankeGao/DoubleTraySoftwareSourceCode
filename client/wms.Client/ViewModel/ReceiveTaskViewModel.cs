using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
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
using wms.Client.ViewModel.Base;
using wms.Client.ViewModel;
using RunningContainer = wms.Client.Model.Entity.RunningContainer;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 系统设置
    /// </summary>
    [Module(ModuleType.ModuleManage, "ReceiveTaskDlg", "领用归还")]
    public class ReceiveTaskViewModel : DataProcess<Receive>
    {
        private readonly string _basePath = ConfigurationManager.AppSettings["ServerIP"];
        /// <summary>
        /// 领用任务契约
        /// </summary>
        private readonly IReceiveTaskContract ReceiveTaskContract;

        /// <summary>
        /// 仓库契约
        /// </summary>
        private readonly IWareHouseContract WareHouseContract;


        /// <summary>
        /// 物料契约
        /// </summary>
        private readonly IMaterialContract MaterialContract;

        /// <summary>
        /// 条码契约
        /// </summary>
        private readonly ILabelContract LabelContract;

        /// <summary>
        /// 条码契约
        /// </summary>
        private readonly IMapper Mapper;
        private readonly IRepository<Bussiness.Entitys.Container, int> ContainerRepository;
        public ReceiveTaskViewModel()
        {

            ContainerRepository = IocResolver.Resolve<IRepository<Bussiness.Entitys.Container, int>>();
            ReceiveTaskContract = IocResolver.Resolve<IReceiveTaskContract>();
            LabelContract = IocResolver.Resolve<ILabelContract>();
            WareHouseContract = IocResolver.Resolve<IWareHouseContract>();
            Mapper = IocResolver.Resolve<IMapper>();
            MaterialContract = IocResolver.Resolve<IMaterialContract>();
            UserLoginCommand = new RelayCommand<string>(ShowLogin);
            ScanBarcodeCommand= new RelayCommand<string>(ScanBarcode);
            SelectItemCommand= new RelayCommand<ReceiveTaskDetailDto>(SelectReceiveTaskItem);
            HandShelfCommand= new RelayCommand (HandShelf);
            HandReturnCommand = new RelayCommand(HandReturn);
            SubmitCommand = new RelayCommand(Submit);
            RunningCommand = new RelayCommand(RunningContainer);
            RunningTakeInCommand = new RelayCommand(RunningTakeInContainer);
            LoginOutCommand = new RelayCommand<string>(LoginOut);
            this.ReadConfigInfo();
            this.GetAllPageData();
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

        private ObservableCollection<ReceiveTaskGroup> _ModuleGroups = new ObservableCollection<ReceiveTaskGroup>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<ReceiveTaskGroup> ModuleGroups
        {
            get { return _ModuleGroups; }
            set { _ModuleGroups = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<ReceiveTaskDetailDto> _ReceiveTaskMaterial= new ObservableCollection<ReceiveTaskDetailDto>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<ReceiveTaskDetailDto> ReceiveTaskMaterial
        {
            get { return _ReceiveTaskMaterial; }
            set { _ReceiveTaskMaterial = value; RaisePropertyChanged(); }
        }


        #endregion

        #region 命令(Binding Command)

        /// <summary>
        /// 退出登录
        /// </summary>
        private RelayCommand _LoginOutCommand;

        public RelayCommand<string> LoginOutCommand { get; private set; }
        /// <summary>
        /// 驱动货柜运转
        /// </summary>
        private RelayCommand _RunningCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand RunningCommand { get; private set; }
        public RelayCommand RunningTakeInCommand { get; private set; }
        
        private RelayCommand _signCommand;

        public RelayCommand SignCommand
        {
            get
            {
                if (_signCommand == null)
                {
                    _signCommand = new RelayCommand(() => GetMyPageData());
                }
                return _signCommand;
            }
        }

        private RelayCommand _allCommand;

        public RelayCommand AllCommand
        {
            get
            {
                if (_allCommand == null)
                {
                    _allCommand = new RelayCommand(() => GetAllPageData());
                }
                return _allCommand;
            }
        }

        private RelayCommand _userLoginCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> UserLoginCommand { get; private set; }



        private RelayCommand _scanBarcodeCommand;

        /// <summary>
        /// 扫描领用条码
        /// </summary>
        public RelayCommand<string> ScanBarcodeCommand { get; private set; }



        private RelayCommand _ReceiveTaskCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> ReceiveTaskCommand { get; private set; }



        private RelayCommand _selectItemCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<ReceiveTaskDetailDto> SelectItemCommand { get; private set; }


        /// <summary>
        /// 确认取出指令
        /// </summary>
        private RelayCommand _handShelfCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand  HandShelfCommand { get; private set; }


        /// <summary>
        /// 确认归还指令
        /// </summary>
        private RelayCommand _handReturnCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand HandReturnCommand { get; private set; }


        /// <summary>
        /// 完成提交
        /// </summary>
        private RelayCommand _submitCommand;
        public RelayCommand SubmitCommand { get; private set; }


        /// <summary>
        /// 完成提交
        /// </summary>
        private RelayCommand _returnCommand;

        public RelayCommand ReturnCommand
        {
            get
            {
                if (_returnCommand == null)
                {
                    _returnCommand = new RelayCommand(() => ChangeTabPageIndex());
                }
                return _returnCommand;
            }
        }


        /// <summary>
        /// 客户端货柜编码
        /// </summary>
        private string ContainerCode = string.Empty;

        /// <summary>
        /// 搜索条码
        /// </summary>
        private string searchBarcode = string.Empty;
        public string SearchBarcode
        {
            get { return searchBarcode; }
            set { searchBarcode = value; RaisePropertyChanged(); }
        }

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
        /// 当前操作储位
        /// </summary>
        private string locationCode = string.Empty;
        public string LocationCode
        {
            get { return locationCode; }
            set { locationCode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前操作储位
        /// </summary>
        private decimal outQuantity = 0;
        public decimal OutQuantity
        {
            get { return outQuantity; }
            set { outQuantity = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 自动存入
        /// </summary>
        private bool _AutoOprate = true;
        public bool AutoOprate
        {
            get { return _AutoOprate; }
            set { _AutoOprate = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前操作的任务明细
        /// </summary>
        private ReceiveTaskDetailDto receiveTaskMaterialEntity = new ReceiveTaskDetailDto();
        public ReceiveTaskDetailDto ReceiveTaskMaterialEntity
        {
            get { return receiveTaskMaterialEntity; }
            set { receiveTaskMaterialEntity = value; RaisePropertyChanged(); }
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

        private string boxUrl = string.Empty;
        public string BoxUrl
        {
            get { return boxUrl; }
            set { boxUrl = value; RaisePropertyChanged(); }
        }

        private string materialUrl = string.Empty;
        public string MaterialUrl
        {
            get { return materialUrl; }
            set { materialUrl = value; RaisePropertyChanged(); }
        }

        private string boxName = string.Empty;
        public string BoxName
        {
            get { return boxName; }
            set { boxName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前托盘-货柜运行后
        /// </summary>
        private string curTratCode = string.Empty;
        public string CurTratCode
        {
            get { return curTratCode; }
            set { curTratCode = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        private RelayCommand _ClearCommand;

        public RelayCommand ClearCommand
        {
            get
            {
                if (_ClearCommand == null)
                {
                    _ClearCommand = new RelayCommand(() => Clear());
                }
                return _ClearCommand;
            }
        }
        /// <summary>
        /// 扫描的条码实体
        /// </summary>
        public LabelClient LabelEntity { get; set; } = new LabelClient();

        #endregion

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
        public async void ShowLogin(string code)
        {
            try
            {
                // 重新读取登录信息
                //this.ReadConfigInfo();
                if (string.IsNullOrWhiteSpace(code)) return;

                // 查询全部任务
                var containerCode = ReceiveTaskContract.ReceiveTaskDtos.FirstOrDefault(a => a.Code == code).ContainerCode;
                if (containerCode != ContainerCode)
                {
                    Msg.Warning("该任务不属于本货柜,请选择正确的任务执行");
                    return;
                }
                GlobalData.LoginModule = "ReceiveMission";
                GlobalData.LoginPageCode = "ReceiveTaskDlg";
                GlobalData.LoginPageName = "领用归还";
                //如果登录
                if (await CheckLogin())
                {
                    // 获取当前任务的明细
                    // 获取当前任务的明细
                    var ReceiveTaskMaterialList = ReceiveTaskContract.ReceiveTaskDetailDtos.Where(a => a.TaskCode == code).OrderBy(a => a.Status).ToList();
                    ReceiveTaskMaterial.Clear();
                    ReceiveTaskMaterialList.ForEach((arg) => ReceiveTaskMaterial.Add(arg));

                    TabPageIndex = 1;
                    GlobalData.IsFocus = true;
                }
                else    //如果未登录
                {
                    var dialog = ServiceProvider.Instance.Get<IShowContent>();
                    dialog.BindDataContext(new UserLoginWindow(), new UserLoginModel());
                    dialog.Show();
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }



        /// <summary>
        /// 扫描领用条码
        /// </summary>
        public async void ScanBarcode(string code)
        {
            try
            {
                if (String.IsNullOrEmpty(SearchBarcode))
                {
                    return;
                }
                //var dialog = ServiceProvider.Instance.Get<IShowContent>();
                var labelEnity = LabelContract.LabelDtos.Where(a => a.Code == SearchBarcode).FirstOrDefault();
                if (labelEnity == null)
                {
                   // Clear();
                    //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "请扫描领用的模具条码！", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                    //dialog.Show();
                    //await Task.Delay(3000);
                    //DialogHost.CloseDialogCommand.Execute(null, null);
                   // Msg.Warning("请扫描领用的模具条码!");
                  //  return;
                }
               // else // 如果是物料条码
                {
                    if (ReceiveTaskMaterialEntity.MaterialLabel != SearchBarcode)
                    {
                        var receiveEntity= ReceiveTaskMaterial.FirstOrDefault(a => a.MaterialLabel == SearchBarcode);
                        if (receiveEntity != null)
                        {
                            if (await Msg.Question("该模具不属于储位，是否运转货柜至其储位？"))
                            {
                                if (GlobalData.DeviceStatus == (int)DeviceStatusEnum.Fault)
                                {
                                    GlobalData.IsFocus = true;
                                    Msg.Warning("设备离线状态，无法启动货柜！");
                                    return;
                                }
                                SelectReceiveTaskItem(receiveEntity);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            Msg.Warning("该模具不属于本领用行项目!");
                            return;
                        }
                    }

                    // 获取入库任务行项目
                    //var selectOuttask = ReceiveTaskMaterial.FirstOrDefault(a => a.MaterialLabel == SearchBarcode && a.Status != (int)InTaskStatusCaption.Finished);
                    //// 判断是否为物料编码
                    //if (selectOuttask != null)
                    //{
                    //    if (String.IsNullOrEmpty(TrayCode) || TrayCode != selectOuttask.TrayCode)
                    //    {
                    //        // 默认选择一项相同物料的
                    //        SelectReceiveTaskItem(selectOuttask);
                    //    }
                    //}
                    LabelEntity.LabelCode = labelEnity.Code;
                    LabelEntity.MaterialCode = labelEnity.MaterialCode;
                    LabelEntity.MaterialName = labelEnity.MaterialName;
                    LabelEntity.Quantity = labelEnity.Quantity;
                    LabelEntity.SupplyName = labelEnity.SupplyName;
                    LabelEntity.BatchCode = labelEnity.BatchCode;
                    LabelEntity.MaterialUrl = _basePath + labelEnity.MaterialUrl;
                    // 本次出库数量
                    OutQuantity = labelEnity.Quantity;

                    // 如果是自动取出
                    if (AutoOprate && OutQuantity > 0)
                    {
                        if (ReceiveTaskMaterialEntity.Status == (int)ReceiveTaskEnum.Proceed)
                        {
                            if (await Msg.Question("是否归还该模具？"))
                            {
                                HandReturn();
                            }
                        }
                        else if (ReceiveTaskMaterialEntity.Status == (int)ReceiveTaskEnum.Wait)
                        {
                            HandShelf();
                        }
                        else
                        {
                            Msg.Warning("该模具不属于本领用行项目!");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }


        /// <summary>
        /// 选择任务行项目
        /// </summary>
        public async void SelectReceiveTaskItem(Bussiness.Dtos.ReceiveTaskDetailDto entity)
        {
            try
            {
                GlobalData.IsFocus = false;
                if (entity == null)
                {
                    Msg.Warning("未获取到选中信息");
                    return;
                }

                if (entity.Status == (int)ReceiveTaskEnum.Finish)
                {
                    Msg.Warning("该物料已完成");
                    return;
                }
                // 验证是否有操作权限
                // 核查用户是否有此模块操作权限
                var user = ServiceProvider.Instance.Get<IUserService>();
                var authCheck = user.GetCheckTrayAuth((int)entity.TrayId);
                if (!authCheck.Result.Success)
                {
                    Msg.Warning("抱歉，您无操作该托盘权限！");
                    return;
                }


                TrayCode = entity.TrayCode;
                LocationCode = entity.LocationCode;
                ReceiveTaskMaterialEntity = entity;
                BoxUrl = _basePath + entity.BoxUrl;
                MaterialUrl = _basePath + entity.MaterialUrl;
                XLight = entity.XLight;
                BoxName = entity.BoxName;

                RunningContainer();
                GlobalData.IsFocus = true;
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        

        /// <summary>
        /// 获取全部货柜任务
        /// </summary>
        public  async void GetAllPageData()
        {
            try
            {
                var query = ReceiveTaskContract.ReceiveTaskDtos.Where(a =>
                    a.Status != (int)ReceiveEnum.Finish);

                if (!String.IsNullOrEmpty(SearchText))
                {
                    query = query.Where(p => p.Code.Contains(SearchText));
                }

                // 查询全部任务
                var groupList = query.GroupBy(a=>a.ContainerCode).Select(a=>a.ContainerCode).ToList();
                _ModuleGroups.Clear();
                // 根据货柜分组
                //  var groupList = ReceiveTaskList.GroupBy(a => new { a.ContainerCode });

                foreach (var item in groupList)
                {
                    var entity = item.FirstOrDefault();
                    var container = WareHouseContract.ContainerDtos.FirstOrDefault(a => a.Code == item);

                    var ReceiveTaskGroup = new ReceiveTaskGroup()
                    {
                        GroupIcon = container.BrandDescription,
                        GroupName = "货柜号：" + container.Code,
                        GroupWarehouse = _basePath + container.PictureUrl
                    };
                    // 查询全部任务
                    var ReceiveTaskContainerList = ReceiveTaskContract.ReceiveTaskDtos.Where(a => a.Status != (int)ReceiveEnum.Cancellation && a.Status != (int)ReceiveEnum.Finish && a.ContainerCode == item).ToList();

                    foreach (var ReceiveTask in ReceiveTaskContainerList)
                    {
                        var ReceiveTaskItem = new ReceiveTaskItem()
                        {
                            Code = ReceiveTask.Code,
                            Name = ReceiveTask.StatusDescription,
                            InCode = ReceiveTask.ReceiveCode,
                            ContainerCode = container.Code
                        };
                        ReceiveTaskGroup.Modules.Add(ReceiveTaskItem);
                    }

                    _ModuleGroups.Add(ReceiveTaskGroup);
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// 获取本机货柜任务
        /// </summary>
        public async void GetMyPageData()
        {
            try
            {
                var container = WareHouseContract.ContainerDtos.FirstOrDefault(a => a.Code == ContainerCode);
                _ModuleGroups.Clear();
                var ReceiveTaskGroup = new ReceiveTaskGroup()
                {
                    GroupIcon = container.BrandDescription,
                    GroupName = "货柜号：" + container.Code,
                    GroupWarehouse = _basePath + container.PictureUrl
                };
                var query = ReceiveTaskContract.ReceiveTaskDtos.Where(a => a.Status != (int)ReceiveEnum.Cancellation &&
                    a.Status != (int)ReceiveEnum.Finish && a.ContainerCode == ContainerCode);
                if (!String.IsNullOrEmpty(SearchText))
                {
                    query = query.Where(p => p.Code.Contains(SearchText));
                }


                    
                // 查询全部任务
                var ReceiveTaskContainerList = query.ToList();

                foreach (var ReceiveTask in ReceiveTaskContainerList)
                {
                    var ReceiveTaskItem = new ReceiveTaskItem()
                    {
                        Code = ReceiveTask.Code,
                        Name = ReceiveTask.StatusDescription,
                        InCode = ReceiveTask.ReceiveCode,
                        ContainerCode = container.Code
                    };
                    ReceiveTaskGroup.Modules.Add(ReceiveTaskItem);
                }

                _ModuleGroups.Add(ReceiveTaskGroup);
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// 启动货柜
        /// </summary>
        public async void RunningContainer()
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
                if (String.IsNullOrEmpty(TrayCode))
                {
                    GlobalData.IsFocus = true;
                    Msg.Warning("未选择入库的行项目，请先选择一项！");

                    return;
                }

                if (TrayCode != CurTratCode)
                {
                    GlobalData.IsFocus = true;

                    if (!await Msg.Question("是否驱动目标托盘到取料口？"))
                    {
                        return;
                    }
                }


                // await dialog.c
                // 读取PLC 状态信息
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

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
                if (!string.IsNullOrEmpty(LocationCode))
                {
                    var locationEntity = WareHouseContract.Locations.FirstOrDefault(a => a.Code == LocationCode);
                    if (locationEntity != null)
                    {
                        runingEntity.XLenght = locationEntity.XLenght.GetValueOrDefault(0);
                    }
                }
                // 货柜运行
                var runningContainer = baseControlService.PostStartRunningContainer(runingEntity);

                if (runningContainer.Result.Success)//
                {
                    GlobalData.IsFocus = true;
                    //var dialog = ServiceProvider.Instance.Get<IShowContent>();
                    //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "货柜运行中", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                    //dialog.Show();
                    //await Task.Delay(2000);
                    //DialogHost.CloseDialogCommand.Execute(null, null);
                    CurTratCode = TrayCode;

                }
                else
                {
                    GlobalData.IsFocus = true;
                    Msg.Error(runningContainer.Result.Message);
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
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
                    // var runningContainer = baseControlService.WriteD650_In(runingEntity).Result;
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
        /// 确认领用
        /// </summary>
        public void HandShelf()
        {
            try
            {
                // 判断条码是否为空
                if (String.IsNullOrEmpty(SearchBarcode))
                {
                    Msg.Warning("请扫描模具编码");
                    return;
                }
                // 判断是否为本项目条码
                if (SearchBarcode != ReceiveTaskMaterialEntity.MaterialLabel)
                {
                    Msg.Warning("该模具条码不属于本领用项目，请核查！");
                    return;
                }

                this.IsCancel = false;

                ReceiveTaskMaterialEntity.MaterialLabel = SearchBarcode;
                ReceiveTaskMaterialEntity.Quantity = (decimal) LabelEntity.Quantity;

                // 实际上架储位-后期可维护成可做修改的
                ReceiveTaskMaterialEntity.LocationCode = LocationCode;

                var ReceiveTaskService = ServiceProvider.Instance.Get<IReceiveTaskService>();

                //   物料实体映射

                var ReceiveTask = ReceiveTaskService.PostHandShelfReceiveTask(ReceiveTaskMaterialEntity);

                if (ReceiveTask.Result.Success)
                {
                    Clear();
                   // Msg.Info("物料领用成功！");
                    // 获取当前任务的明细
                    var ReceiveTaskMaterialList = ReceiveTaskContract.ReceiveTaskDetailDtos
                        .Where(a => a.TaskCode == ReceiveTaskMaterialEntity.TaskCode).OrderBy(a=>a.Status).ToList();
                    ReceiveTaskMaterial.Clear();
                    ReceiveTaskMaterialList.ForEach((arg) => ReceiveTaskMaterial.Add(arg));

                    if (ReceiveTaskMaterial.Count > 0)
                    {
                        // 获取入库任务行项目
                        var selectOutTask = ReceiveTaskMaterial
                            .FirstOrDefault(a =>
                                a.Status == (int)ReceiveTaskEnum.Wait);
                        if (selectOutTask != null)
                        {
                            // 默认选择一项相同物料的
                            SelectReceiveTaskItem(selectOutTask);
                        }
                    }
                }
                else
                {
                    Msg.Error("物料领用失败：" + ReceiveTask.Result.Message);
                }
            }
            catch (Exception ex)
            {
                Msg.Error("物料领用失败!");
            }
            finally
            {
                this.IsCancel = true;
            }
        }


        /// <summary>
        /// 确认归还
        /// </summary>
        public void HandReturn()
        {
            try
            {
                // 判断条码是否为空
                if (String.IsNullOrEmpty(SearchBarcode))
                {
                    Msg.Warning("请扫描模具条码！");
                    return;
                }

                this.IsCancel = false;

                ReceiveTaskMaterialEntity.MaterialLabel = SearchBarcode;
                ReceiveTaskMaterialEntity.Quantity = (decimal)LabelEntity.Quantity;
                ReceiveTaskMaterialEntity.LocationCode = LocationCode;
                ReceiveTaskMaterialEntity.LastTimeReturnName = UserCode;
                ReceiveTaskMaterialEntity.LastTimeReturnDatetime = DateTime.Now;
                var ReceiveTaskService = ServiceProvider.Instance.Get<IReceiveTaskService>();

                //   物料实体映射

                var ReceiveTask = ReceiveTaskService.PostHandShelfReturn(ReceiveTaskMaterialEntity);

                if (ReceiveTask.Result.Success)
                {
                    Clear();
                    // 获取当前任务的明细
                    var ReceiveTaskMaterialList = ReceiveTaskContract.ReceiveTaskDetailDtos.Where(a => a.TaskCode == ReceiveTaskMaterialEntity.TaskCode&& a.Status != (int)ReceiveTaskEnum.Finish&& a.Status != (int)ReceiveTaskEnum.Cancellation).ToList();
                    ReceiveTaskMaterial.Clear();
                    ReceiveTaskMaterialList.ForEach((arg) => ReceiveTaskMaterial.Add(arg));

                    if (ReceiveTaskMaterial.Count > 0)
                    {
                        // 获取入库任务行项目
                        var selectOutTask = ReceiveTaskMaterial
                            .FirstOrDefault(a =>
                                a.Status == (int)ReceiveTaskEnum.Proceed);
                        if (selectOutTask != null)
                        {
                            // 默认选择一项相同物料的
                            SelectReceiveTaskItem(selectOutTask);
                        }
                    }

                }
                else
                {
                    Msg.Error("物料归还失败：" + ReceiveTask.Result.Message);
                }

            }
            catch (Exception ex)
            {
                Msg.Error("物料领用失败：");
            }
            finally
            {
                this.IsCancel = true;
            }
        }



        /// <summary>
        /// 完成提交
        /// </summary>
        public void Submit()
        {
            try
            {
                if (ReceiveTaskMaterialEntity.ReceiveCode == null)
                {
                    Msg.Warning("未执行领用操作");
                    return;
                }

                var ReceiveTaskList = ReceiveTaskContract.ReceiveTaskDtos.Where(a => a.ReceiveCode == ReceiveTaskMaterialEntity.ReceiveCode && a.Status != (int)ReceiveEnum.Finish).ToList();

                // 仍有未完成的任务
                if (ReceiveTaskList.Count > 0)
                {
                    // 全局变量设置需要指引的领用单
                    GlobalData.GuideCode = ReceiveTaskMaterialEntity.ReceiveCode;
                    GlobalData.GuideType = (int) ModuleType.ModuleManage;
                    var dialog = ServiceProvider.Instance.Get<IShowContent>();
                    dialog.BindDataContext(new GuideWindow(), new GuideModel());
                    // 系统登录
                    dialog.Show();
                } 
                else // 直接跳转至任务界面
                {
                    TabPageIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Msg.Question("物料领用失败：");
            }
            finally
            {
                this.IsCancel = true;
            }
        }

        public void ChangeTabPageIndex()
        {
            TabPageIndex = 0;
            this.GetAllPageData();
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

        /// <summary>
        /// 清空扫描
        /// </summary>
        private void Clear()
        {
            try
            {
                GlobalData.IsFocus = false;
                // 清空
                LabelEntity.MaterialName = "";
                LabelEntity.MaterialCode = "";
                LabelEntity.BatchCode = "";
                LabelEntity.Quantity = 0;
                LabelEntity.SupplyName = "";
                OutQuantity = 0;
                BoxUrl = "";
                BoxName = "";
                SearchBarcode = "";
                LabelEntity.MaterialUrl = "";
                GlobalData.IsFocus = true;
            }
            catch (Exception ex)
            {
                Msg.Question("物料入库失败");
            }
            finally
            {
                this.IsCancel = true;
            }
        }

    }
}
