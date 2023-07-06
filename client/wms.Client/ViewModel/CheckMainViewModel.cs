using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using GalaSoft.MvvmLight.Command;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Core.Mapping;
using HP.Utility;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
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
    [Module(ModuleType.WarehouseManagement, "CheckMainDlg", "货柜盘点")]
    public class CheckMainViewModel : DataProcess<CheckMain>
    {
        private readonly string _basePath = ConfigurationManager.AppSettings["ServerIP"];

        /// <summary>
        /// 盘点任务契约
        /// </summary>
        private readonly ICheckContract CheckContract;

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
        /// 仓库契约
        /// </summary>
        private readonly IStockContract StockContract;


        private readonly IRepository<Container, int> ContainerRepository;

        /// <summary>
        /// 条码契约
        /// </summary>
        private readonly IMapper Mapper;

        public CheckMainViewModel()
        {
            ContainerRepository = IocResolver.Resolve<IRepository<Container, int>>();
            CheckContract = IocResolver.Resolve<ICheckContract>();
            LabelContract = IocResolver.Resolve<ILabelContract>();
            WareHouseContract = IocResolver.Resolve<IWareHouseContract>();
            Mapper = IocResolver.Resolve<IMapper>();
            MaterialContract = IocResolver.Resolve<IMaterialContract>();
            UserLoginCommand = new RelayCommand<string>(ShowLogin);
            ScanBarcodeCommand= new RelayCommand<string>(ScanBarcode);
            ScanLocationCommand = new RelayCommand<string>(ScanLocation);
            SelectItemCommand = new RelayCommand<CheckDetailDto>(SelectCheckMainItem);
            RunningCommand = new RelayCommand(RunningContainer);
            RunningTakeInCommand = new RelayCommand(RunningTakeInContainer);
            HandShelfCommand = new RelayCommand (HandShelf);
            LoginOutCommand = new RelayCommand<string>(LoginOut);
            SubmitCommand = new RelayCommand(Submit);
            StockContract = IocResolver.Resolve<IStockContract>();
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

        private ObservableCollection<CheckTaskGroup> _ModuleGroups = new ObservableCollection<CheckTaskGroup>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<CheckTaskGroup> ModuleGroups
        {
            get { return _ModuleGroups; }
            set { _ModuleGroups = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<CheckDetailDto> _CheckDetail= new ObservableCollection<CheckDetailDto>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<CheckDetailDto> CheckDetail
        {
            get { return _CheckDetail; }
            set { _CheckDetail = value; RaisePropertyChanged(); }
        }


        #endregion

        #region 命令(Binding Command)
        /// <summary>
        /// 驱动货柜运转
        /// </summary>
        private RelayCommand _RunningCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand RunningCommand { get; private set; }

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand RunningTakeInCommand { get; private set; }
        /// <summary>
        /// 退出登录
        /// </summary>
        private RelayCommand _LoginOutCommand;

        public RelayCommand<string> LoginOutCommand { get; private set; }

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
        /// 扫描盘点条码
        /// </summary>
        public RelayCommand<string> ScanBarcodeCommand { get; private set; }

        /// <summary>
        /// 扫描储位码
        /// </summary>
        private RelayCommand _scanLocationCommand;
        public RelayCommand<string> ScanLocationCommand { get; private set; }





        private RelayCommand _OutTaskCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> OutTaskCommand { get; private set; }



        private RelayCommand _selectItemCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<CheckDetailDto> SelectItemCommand { get; private set; }


        /// <summary>
        /// 选择本次入库的物料
        /// </summary>
        private RelayCommand _trayCommand;

        public RelayCommand TrayCommand
        {
            get
            {
                if (_trayCommand == null)
                {
                    _trayCommand = new RelayCommand(() => SelectTray());
                }
                return _trayCommand;
            }
        }

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
        /// 选择本次出库的物料
        /// </summary>
        private RelayCommand _materialCommand;

        public RelayCommand MaterialCommand
        {
            get
            {
                if (_returnCommand == null)
                {
                    _returnCommand = new RelayCommand(() => SelectMaterial());
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
        /// 扫描储位码
        /// </summary>
        private string searchLocationCode = string.Empty;
        public string SearchLocationCode
        {
            get { return searchLocationCode; }
            set { searchLocationCode = value; RaisePropertyChanged(); }
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
        public string SelectTrayCode
        {
            get { return trayCode; }
            set { trayCode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前操作的物料
        /// </summary>
        private string materialCode = string.Empty;
        public string SelectMaterialCode
        {
            get { return materialCode; }
            set { materialCode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前操作的物料
        /// </summary>
        private string materialName = string.Empty;
        public string SelectMaterialName
        {
            get { return materialName; }
            set { materialName = value; RaisePropertyChanged(); }
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
        private decimal checkQuantity = 0;
        public decimal CheckQuantity
        {
            get { return checkQuantity; }
            set { checkQuantity = value; RaisePropertyChanged(); }
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
        private int _XLightLenght = 1;
        public int XLightLenght
        {
            get { return _XLightLenght; }
            set { _XLightLenght = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 当前操作储位
        /// </summary>
        private int _YLight = 0;
        public int YLight
        {
            get { return _YLight; }
            set { _YLight = value; RaisePropertyChanged(); }
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
        /// 当前操作托盘号
        /// </summary>
        private int trayId = 0;
        public int TrayId
        {
            get { return trayId; }
            set { trayId = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<CheckDetailDto> CheckDetailList
        {
            get { return _CheckDetailList; }
            set { _CheckDetailList = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<CheckDetailDto> _CheckDetailList = new ObservableCollection<CheckDetailDto>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<CheckDetailDto> CheckDetailListAll
        {
            get { return _CheckDetailListAll; }
            set { _CheckDetailListAll = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<CheckDetailDto> _CheckDetailListAll = new ObservableCollection<CheckDetailDto>();

        /// <summary>
        /// 托盘分组
        /// </summary>
        private ObservableCollection<LocationVIEW> _TaryGroups = new ObservableCollection<LocationVIEW>();
        public ObservableCollection<LocationVIEW> TrayGroups
        {
            get { return _TaryGroups; }
            set { _TaryGroups = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 托盘分组
        /// </summary>
        private ObservableCollection<MaterialDto> _MaterialGroups = new ObservableCollection<MaterialDto>();
        public ObservableCollection<MaterialDto> MaterialGroups
        {
            get { return _MaterialGroups; }
            set { _MaterialGroups = value; RaisePropertyChanged(); }
        }
        public LabelClient LabelEntity { get; set; } = new LabelClient();
        #endregion

        /// <summary>
        /// 启动货柜
        /// </summary>
        public async void RunningContainer()
        {
            if (GlobalData.DeviceStatus == (int)DeviceStatusEnum.Fault)
            {
                GlobalData.IsFocus = true;
                Msg.Warning("设备离线状态，无法启动货柜！");
                return;
            }
            GlobalData.IsFocus = false;
            if (String.IsNullOrEmpty(SelectTrayCode))
            {
                GlobalData.IsFocus = true;
                Msg.Warning("未选择入库的行项目，请先选择一项！");

                return;
            }
            if (SelectTrayCode != CurTratCode)
            {
                GlobalData.IsFocus = true;

                if (!await Msg.Question("是否驱动目标托盘到取料口？"))
                {
                    return;
                }
            }
            // 核查用户是否有此模块操作权限
            var user = ServiceProvider.Instance.Get<IUserService>();
            var authCheck = user.GetCheckTrayAuth((int)TrayId);
            if (!authCheck.Result.Success)
            {
                Msg.Warning("抱歉，您无操作该托盘权限！");
                return;
            }
            // 读取PLC 状态信息
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

            var runingEntity = new RunningContainer()
            {
                ContainerCode = ContainerCode,
                TrayCode = Convert.ToInt32(SelectTrayCode),
                XLight = XLight,
                XLenght = XLightLenght
            };
            var container = ContainerRepository.Query().FirstOrDefault(a => a.Code == ContainerCode);
            runingEntity.ContainerType = container.ContainerType;
            runingEntity.IpAddress = container.Ip;
            runingEntity.Port = int.Parse(container.Port);
            // 货柜运行
            var runningContainer = baseControlService.PostStartRunningContainer(runingEntity);

            if (runningContainer.Result.Success)
            {
                GlobalData.IsFocus = true;
                //var dialog = ServiceProvider.Instance.Get<IShowContent>();
                //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "货柜运行中", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                //dialog.Show();
                //await Task.Delay(2000);
                //DialogHost.CloseDialogCommand.Execute(null, null);
                CurTratCode = SelectTrayCode;

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
                    var runningContainer = baseControlService.WriteD650_In(runingEntity).Result;
                    if (runningContainer.Success)
                    {
                        var result = baseControlService.SetM654True().Result;
                        if (!result.Success)
                        {
                            Msg.Error(result.Message);
                        }
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
            GlobalData.IsFocus = false;
            // 查询全部任务
            if (string.IsNullOrWhiteSpace(code)) return;

            // 查询全部任务
            var containerCode = CheckContract.CheckDtos.FirstOrDefault(a => a.Code == code).ContainerCode;
            if (containerCode != ContainerCode)
            {
                Msg.Warning("该任务不属于本货柜,请选择正确的任务执行");
                return;
            }
            // 盘点任务
            GlobalData.LoginModule = "Inventory";
            GlobalData.LoginPageCode = "CheckMainDlg";
            GlobalData.LoginPageName = "货柜盘点";
            GlobalData.GuideCode = code;
            //如果登录
            if (await CheckLogin())
            {
                // 获取当前任务的明细
                // 获取当前任务的明细
                var List = CheckContract.CheckDetailDtos.Where(a => a.CheckCode == code).ToList();

                // 查询该物料可存放的储位
                var trayList = List.GroupBy(a => a.TrayCode).OrderBy(a=>a.Key).ToArray();
                _TaryGroups.Clear();

                foreach (var item in trayList)
                {
                    var LocationVIEW = new LocationVIEW()
                    {
                        TrayCode = item.Key
                    };
                    _TaryGroups.Add(LocationVIEW);
                }
                TabPageIndex = 1;
            }
            else    //如果未登录
            {
                var dialog = ServiceProvider.Instance.Get<IShowContent>();
                dialog.BindDataContext(new UserLoginWindow(), new UserLoginModel());
                dialog.Show();
            }
            GlobalData.IsFocus = true;
        }




        /// <summary>
        /// 扫描盘点条码
        /// </summary>
        public async void ScanBarcode(string code)
        {
            if (String.IsNullOrEmpty(SearchBarcode))
            {
                return;
            }
            var dialog = ServiceProvider.Instance.Get<IShowContent>();
            var labelEnity = LabelContract.LabelDtos.FirstOrDefault(a => a.Code == SearchBarcode);
            if (labelEnity == null)
            {
                //var material = MaterialContract.MaterialDtos.FirstOrDefault(a => a.Code == SelectMaterialCode);
                //if (material.IsPackage)
                //{
                //    Clear();
                //    dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "该物料已启用单包，请扫描物料条码！", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                //    dialog.Show();
                //    await Task.Delay(3000);
                //    DialogHost.CloseDialogCommand.Execute(null, null);
                //    return;
                //}

                Clear();
                Msg.Warning("未获取到条码信息!");
                return;


                //// 如果是储位码
                //var locationCode = WareHouseContract.Locations.FirstOrDefault(a => a.Code == SearchBarcode);

                //if (locationCode != null && locationCode.Code == LocationCode)
                //{
                //    // 查找该储位的库存
                //    var stock = StockContract.StockDtos.FirstOrDefault(a => a.LocationCode == locationCode.Code && a.MaterialCode == SelectMaterialCode);
                //    LabelEntity.MaterialCode = stock.MaterialCode;
                //    LabelEntity.MaterialName = stock.MaterialName;
                //    LabelEntity.MaterialUrl = _basePath + stock.PictureUrl;
                //    SelectMaterialCode = stock.MaterialCode;
                //}
                //else
                //{
                //    Clear();
                //    dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "未获取到条码信息！", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                //    dialog.Show();
                //    await Task.Delay(3000);
                //    DialogHost.CloseDialogCommand.Execute(null, null);
                //    return;
                //}
            }
            else
            {
                if (labelEnity.MaterialCode!= SelectMaterialCode)
                {
                    Msg.Warning("该物料不属于本储位，请重新扫描！");
                    return;
                }

                if (CheckDetailListAll.Any(a=>a.MaterialLabel== labelEnity.Code))
                {
                    if (!await Msg.Question("该物料已盘点过，是否重盘？"))
                    {
                        return;
                    }
                    else
                    {
                        // 移除
                        CheckDetailListAll.Remove(CheckDetailListAll.FirstOrDefault(a => a.MaterialLabel == labelEnity.Code));
                    }
                }
                LabelEntity.LabelCode = code;
                LabelEntity.MaterialName = labelEnity.MaterialName;
                LabelEntity.Quantity = labelEnity.Quantity;
                LabelEntity.SupplyName = labelEnity.SupplyName;
                LabelEntity.BatchCode = labelEnity.BatchCode;
                // 本次盘点数量
                CheckQuantity = labelEnity.Quantity;
                SelectMaterialCode = labelEnity.MaterialCode;

                // 自动盘点
                HandShelf();
            }
        }

        /// <summary>
        /// 扫描储位码
        /// </summary>
        public async void ScanLocation(string code)
        {
            try
            {
                GlobalData.IsFocus = false;
                // 判断是否启用单包条码管理
                var location = WareHouseContract.LocationVMs.FirstOrDefault(a => a.Code == SearchLocationCode);

                // 扫描储位码
                if (location == null)
                {
                    Msg.Warning("请扫描储位码");
                    return;
                }
                BoxUrl = _basePath + location.BoxUrl;
                BoxName = location.BoxName;
                XLight = location.XLight;
                YLight = location.YLight;
                XLightLenght = location.XLenght.GetValueOrDefault(0);
                LocationCode = code;

                if (String.IsNullOrEmpty(location.LockMaterialCode))
                {
                    SelectMaterialCode = location.SuggestMaterialCode;
                }
                else
                {
                    SelectMaterialCode = location.LockMaterialCode;
                }
                var material = MaterialContract.Materials.FirstOrDefault(a => a.Code == SelectMaterialCode);
                if (material == null)
                {
                    Msg.Warning("物料错误！");
                    return;
                }
                SelectMaterialName = material.Name;
                MaterialUrl = material.PictureUrl;
                RunningContainer();
                //var materialGroup = StockContract.StockDtos.Where(it => it.ContainerCode == ContainerCode&&it.LocationCode== code)
                //    .GroupBy(it => it.MaterialCode)
                //    .AndBy(it => it.MaterialName)
                //    .Select(it => new { MaterialCode = it.MaterialCode, MaterialName = it.MaterialName }).ToList();

                //_MaterialGroups.Clear();

                //foreach (var item in materialGroup)
                //{
                //    var material = new MaterialDto()
                //    {
                //        Code = item.MaterialCode,
                //        Name=item.MaterialName
                //    };
                //    _MaterialGroups.Add(material);
                //}
                GlobalData.IsFocus = true;
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }


        /// <summary>
        /// 选择任务行项目
        /// </summary>
        public async void SelectCheckMainItem(Bussiness.Dtos.CheckDetailDto entity)
        {
            try
            {
                // 验证是否有操作权限
                if (entity == null)
                {
                    Msg.Warning("未获取到选中信息");
                    return;
                }

                if (entity.Status == (int)OutTaskStatusCaption.Finished)
                {
                    Msg.Warning("该物料已完成");
                    return;
                }

                LocationCode = entity.LocationCode;
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
                var query = CheckContract.CheckDtos.Where(a => (a.Status == (int)CheckStatusCaption.WaitingForCheck || a.Status == (int)CheckStatusCaption.CheckAgagin || a.Status == (int)CheckStatusCaption.HandChecking) && a.ContainerCode == ContainerCode);

                if (!String.IsNullOrEmpty(SearchText))
                {
                    query = query.Where(p => p.CheckListCode.Contains(SearchText));
                }

                // 查询全部任务
                var groupList = query.GroupBy(a => a.ContainerCode).Select(a => a.ContainerCode).ToList();
                _ModuleGroups.Clear();

                foreach (var item in groupList)
                {
                    var container = WareHouseContract.ContainerDtos.FirstOrDefault(a => a.Code == item);

                    var CheckTaskGroup = new CheckTaskGroup()
                    {
                        GroupIcon = container.BrandDescription,
                        GroupName = "货柜号：" + container.Code,
                        GroupWarehouse = _basePath + container.PictureUrl
                    };
                    // 查询全部任务
                    var OutTaskContainerList = CheckContract.CheckDtos.Where(a => (a.Status == (int)CheckStatusCaption.WaitingForCheck || a.Status == (int)CheckStatusCaption.CheckAgagin || a.Status == (int)CheckStatusCaption.HandChecking) && a.ContainerCode == item).ToList();

                    foreach (var checkTask in OutTaskContainerList)
                    {
                        var inTaskItem = new CheckTaskItem()
                        {
                            Code = checkTask.Code,
                            Name = checkTask.StatusCaption,
                            InCode = checkTask.CheckListCode,
                            ContainerCode = container.Code
                        };
                        CheckTaskGroup.Modules.Add(inTaskItem);
                    }

                    _ModuleGroups.Add(CheckTaskGroup);
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
                var OutTaskGroup = new CheckTaskGroup()
                {
                    GroupIcon = container.BrandDescription,
                    GroupName = "货柜号：" + container.Code,
                    GroupWarehouse = _basePath + container.PictureUrl
                };
                var query = CheckContract.CheckDtos.Where(a =>(a.Status == (int)CheckStatusCaption.WaitingForCheck || a.Status == (int)CheckStatusCaption.HandChecking || a.Status == (int)CheckStatusCaption.CheckAgagin) && a.ContainerCode == ContainerCode);
                if (!String.IsNullOrEmpty(SearchText))
                {
                    query = query.Where(p => p.Code.Contains(SearchText));
                }
                    
                // 查询全部任务
                var OutTaskContainerList = query.ToList();

                foreach (var checkTask in OutTaskContainerList)
                {
                    var checkTaskItem = new CheckTaskItem()
                    {
                        Code = checkTask.Code,
                        Name = checkTask.StatusCaption,
                        InCode = checkTask.CheckListCode,
                        ContainerCode = container.Code
                    };
                    OutTaskGroup.Modules.Add(checkTaskItem);
                }

                _ModuleGroups.Add(OutTaskGroup);

            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        public async void SelectMaterial()
        {
            try
            {
                if (!String.IsNullOrEmpty(SelectMaterialCode))
                {
                    var material = _MaterialGroups.FirstOrDefault(a => a.Code == SelectMaterialCode);
                    if (material == null)
                    {
                        Msg.Info("物料选择错误！");
                        return;
                    }
                    SelectMaterialName = material.Name;
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }
        
        /// <summary>
        /// 选择托盘
        /// </summary>
        public async void SelectTray()
        {
            try
            {
  
                // 查询该物料可存放的储位
                if (CheckDetailListAll.Count>0)
                {
                    CheckDetailList.Clear();
                    foreach (var item in CheckDetailListAll.Where(a => a.TrayCode == SelectTrayCode).ToList())
                    {
            
                        CheckDetailList.Add(item);
                    }
                }

                TrayId = (int)WareHouseContract.LocationVMs
                    .FirstOrDefault(a => a.ContainerCode == ContainerCode && a.TrayCode == SelectTrayCode).TrayId;


            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }



        /// <summary>
        /// 确认盘点
        /// </summary>
        public async void HandShelf()
        {
            try
            {
                if (String.IsNullOrEmpty(SelectMaterialCode))
                {
                    Msg.Warning("请选择盘点的物料！");
                    return;
                }

                if (CheckQuantity == 0)
                {
                    if (!await Msg.Question("盘点数量为0，是否确认？"))
                    {
                        return;
                    }
                }
                if (CurTratCode != SelectTrayCode)
                {
                    Msg.Warning("当前货柜未运行至此托盘，请启动货柜！");
                    return;
                }
                GlobalData.IsFocus = false;

                var checkEntity = new CheckDetailDto()
                {
                    LocationCode = SearchLocationCode,
                    MaterialCode = SelectMaterialCode,
                    MaterialName= SelectMaterialName,
                    CheckCode = GlobalData.GuideCode,
                    TrayCode = SelectTrayCode,
                    CheckedQuantity = CheckQuantity
                };
                // 判断是否启用单包条码管理
                var materialEntity = MaterialContract.Materials.FirstOrDefault(a => a.Code == SelectMaterialCode);


                // 判断是否启用单包条码管理
                var labelEnity = LabelContract.LabelDtos.Where(a => a.Code == SearchBarcode).FirstOrDefault();

                // 判断是否启用条码
                if (materialEntity.IsPackage)
                {
                    if (CheckQuantity != 0) // 如果盘点数量不为0
                    {
                        // 判断条码是否为空，以及是否为物料条码
                        if (String.IsNullOrEmpty(SearchBarcode))
                        {
                            CheckQuantity = 0;
                            Msg.Warning("该物料已经启用条码管理，请扫描出库条码");
                            return;
                        }
                        if (labelEnity == null)
                        {
                            CheckQuantity = 0;
                            Msg.Warning("该物料已经启用条码管理，请扫描出库条码");
                            return;
                        }
                        else
                        {
                            checkEntity.MaterialLabel = SearchBarcode;
                            checkEntity.MaterialName = LabelEntity.MaterialName;
                        }
                    }
                } // 如果未启用单包
                else
                {
                    // 从库存中获取
                    var outLabel = StockContract.StockDtos
                        .FirstOrDefault(a => a.LocationCode == LocationCode && a.MaterialCode == SelectMaterialCode );
                    if (outLabel != null)
                    {
                        checkEntity.MaterialLabel = outLabel.MaterialLabel;
                        checkEntity.MaterialName = outLabel.MaterialName;
                    }
                }
                this.IsCancel = false;

                // 新增入库任务明细
                CheckDetailListAll.Add(checkEntity);

                if (CheckDetailListAll.Count == 0)
                {
                    Msg.Warning("未执行盘点操作，无法提交");
                    return;
                }

                List<CheckDetail> postlist = new List<CheckDetail>();
                postlist.Add(checkEntity);
                var checkMaterial = JsonHelper.SerializeObject(postlist);

                var checkService = ServiceProvider.Instance.Get<ICheckMainService>();

                var check = new CheckDto()
                {
                    Code = GlobalData.GuideCode,
                    CheckLocationCode= LocationCode,
                    CheckDetailMaterialList = checkMaterial
                };
                var inTaskPost = checkService.PostDoHandCheckClient(check);
                if (inTaskPost.Result.Success)
                {
                    Clear();
                    //Msg.Info("盘点成功！");
                    CheckDetailList.Clear();
                    foreach (var item in CheckDetailListAll.Where(a => a.TrayCode == SelectTrayCode).ToList())
                    {
                        CheckDetailList.Add(item);
                    }
                }
                else
                {
                    Clear();
                    Msg.Error("物料盘点失败：" + inTaskPost.Result.Message);
                }
            }
            catch (Exception ex)
            {
                Msg.Error("物料盘点失败");
            }
            finally
            {
                this.IsCancel = true;
            }
        }



        /// <summary>
        /// 完成提交
        /// </summary>
        public async void Submit()
        {
            try
            {
                if (!await Msg.Question("是否已完成本次盘点？"))
                {
                    return;
                }
                var checkService = ServiceProvider.Instance.Get<ICheckMainService>();

                var inTask = new CheckDto()
                {
                    Code = GlobalData.GuideCode,
                };
                var inTaskPost = checkService.PostPDACheckComplete(inTask);
                if (!inTaskPost.Result.Success)
                {
                    Clear();
                    Msg.Info("物料盘点完成失败！"+ inTaskPost.Result.Message);
                    return;
                }
                var OutTaskList = CheckContract.CheckDtos.Where(a => a.CheckListCode == GlobalData.GuideCode && (a.Status == (int)CheckStatusCaption.WaitingForCheck || a.Status == (int)CheckStatusCaption.HandChecking)).ToList();

                // 仍有未完成的任务
                if (OutTaskList.Count > 0)
                {
                    // 全局变量设置需要指引的盘点单
                    GlobalData.GuideType = (int)ModuleType.ModuleManage;

                    var dialog = ServiceProvider.Instance.Get<IShowContent>();
                    dialog.BindDataContext(new GuideWindow(), new GuideModel());
                    // 系统登录
                    dialog.Show();
                }
                else // 直接跳转至任务界面
                {
                    ChangeTabPageIndex();
                }
            }
            catch (Exception ex)
            {
                Msg.Question("物料盘点失败");
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
        /// 清空扫描
        /// </summary>
        private void Clear()
        {
            try
            {
                GlobalData.IsFocus = false;

                // 如果是不是物料条码，则清空储位
                if (String.IsNullOrEmpty(SearchBarcode))
                {
                    SearchLocationCode = "";
                }

                // 清空
                LabelEntity.MaterialName = "";
                LabelEntity.MaterialCode = "";
                LabelEntity.BatchCode = "";
                LabelEntity.Quantity = 0;
                MaterialUrl = "";
                LocationCode = "";
                LabelEntity.SupplyName = "";
                CheckQuantity = 0;
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
