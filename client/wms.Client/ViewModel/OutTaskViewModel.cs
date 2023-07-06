using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
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
using wms.Client.Common;
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
    [Module(ModuleType.OutManage, "OutTaskDlg", "出库任务")]
    public class OutTaskViewModel : DataProcess<OutTask>
    {
        public ImageWindow imageWindow;
        private readonly string _basePath = ConfigurationManager.AppSettings["ServerIP"];
        /// <summary>
        /// 出库任务契约
        /// </summary>
        private readonly IOutTaskContract OutTaskContract;

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
        private readonly IStockContract StockContract;


        /// <summary>
        /// 条码契约
        /// </summary>
        private readonly IMapper Mapper;
        private readonly IRepository<Bussiness.Entitys.Container, int> ContainerRepository;


        // 称重接口
        WeightReader weightReader = new WeightReader();
        Action<string> updateWeightAction;
        private string COM;
        public OutTaskViewModel()
        {
            ContainerRepository = IocResolver.Resolve<IRepository<Bussiness.Entitys.Container, int>>();
            OutTaskContract = IocResolver.Resolve<IOutTaskContract>();
            LabelContract = IocResolver.Resolve<ILabelContract>();
            WareHouseContract = IocResolver.Resolve<IWareHouseContract>();
            StockContract = IocResolver.Resolve<IStockContract>();
            Mapper = IocResolver.Resolve<IMapper>();
            MaterialContract = IocResolver.Resolve<IMaterialContract>();
            UserLoginCommand = new RelayCommand<string>(ShowLogin);
            ScanBarcodeCommand = new RelayCommand<string>(ScanBarcode);
            //   SelectItemCommand= new RelayCommand<OutTaskMaterialDto>(SelectOutTaskItem);
            SelectItemCommand = new RelayCommand<object>(SelectOutTaskItem, true);//< OutTaskMaterialDto >
            LoginOutCommand = new RelayCommand<string>(LoginOut);
            HandShelfCommand = new RelayCommand (HandShelf);
            SubmitCommand = new RelayCommand(Submit);
            RunningCommand = new RelayCommand(RunningContainer);
            RunningTakeInCommand = new RelayCommand(RunningTakeInContainer);
            //   PrintItemCommand = new RelayCommand<OutTaskMaterialDto>(PrintItem);
            PrintItemCommand = new RelayCommand<object>(PrintItem, true);
            MouseEnterCommand = new RelayCommand(ImageMouseEnter);

            weightReader.StartKey = string.Empty;
            weightReader.UnitKey = string.Empty;
            weightReader.MatchPattern = @".+[\r|\n]";
            COM = System.Configuration.ConfigurationSettings.AppSettings["WeightCOM"].ToString();
            weightReader.Changed += new EventHandler(weightReader_Changed);

            this.ReadConfigInfo();
            this.GetAllPageData();
        }

        public RelayCommand MouseEnterCommand
        {
            get;
            private set;
        }
        public void ImageMouseEnter()
        {
            Uri uri = new Uri(this.LabelEntity.MaterialUrl);
            System.Windows.Media.Imaging.BitmapImage image = new BitmapImage(uri);

            imageWindow = new ImageWindow();
            imageWindow.ImageSrc.Source = image;

            //imageWindow.ShowDialog("");
            var dialog = ServiceProvider.Instance.Get<IShowContent>();
            dialog.BindDataContext(imageWindow, new ImageShowModel());
            dialog.Show();

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

        #region 重量核验
        /// <summary>
        /// 当前称重
        /// </summary>
        private decimal inWeight = 0;
        public decimal InWeight
        {
            get { return inWeight; }
            set { inWeight = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 物料单重
        /// </summary>
        private decimal unitWeight = 0;
        public decimal UnitWeight
        {
            get { return unitWeight; }
            set { unitWeight = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 全部库存
        /// </summary>
        private decimal _AllQuantity = 0;
        public decimal AllQuantity
        {
            get { return _AllQuantity; }
            set { _AllQuantity = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 物料称重数量
        /// </summary>
        private decimal weighingQuantity = 0;
        public decimal WeighingQuantity
        {
            get { return weighingQuantity; }
            set { weighingQuantity = value; RaisePropertyChanged(); }
        }

        private bool isCheckUnitWeight = false;
        public bool IsCheckUnitWeight
        {
            get { return isCheckUnitWeight; }
            set { isCheckUnitWeight = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 称重
        /// </summary>
        private RelayCommand _WeightCheckCommand;

        public RelayCommand WeightCheckCommand
        {
            get
            {
                if (_WeightCheckCommand == null)
                {
                    _WeightCheckCommand = new RelayCommand(() => WeightCheck());
                }
                return _WeightCheckCommand;
            }
        }

        #endregion

        #region 任务组

        private ObservableCollection<OutTaskGroup> _ModuleGroups = new ObservableCollection<OutTaskGroup>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<OutTaskGroup> ModuleGroups
        {
            get { return _ModuleGroups; }
            set { _ModuleGroups = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<OutTaskMaterialDto> _OutTaskMaterial= new ObservableCollection<OutTaskMaterialDto>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<OutTaskMaterialDto> OutTaskMaterial
        {
            get { return _OutTaskMaterial; }
            set { _OutTaskMaterial = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 扫描的条码实体
        /// </summary>
        public LabelClient LabelEntity { get; set; } = new LabelClient();

        #endregion


        #region 颜色变化
        private string _ScanColor = "MediumPurple";
        public string ScanColor
        {
            get { return _ScanColor; }
            set { _ScanColor = value; RaisePropertyChanged(); }
        }
        private string _ClearDataColor = "MediumPurple";
        public string ClearDataColor
        {
            get { return _ClearDataColor; }
            set { _ClearDataColor = value; RaisePropertyChanged(); }
        }
        private string _FirstStepColor = "MediumPurple";
        public string FirstStepColor
        {
            get { return _FirstStepColor; }
            set { _FirstStepColor = value; RaisePropertyChanged(); }
        }
        private string _SecondStepColor = "MediumPurple";
        public string SecondStepColor
        {
            get { return _SecondStepColor; }
            set { _SecondStepColor = value; RaisePropertyChanged(); }
        }
        private string _ThirdStepColor = "MediumPurple";
        public string ThirdStepColor
        {
            get { return _ThirdStepColor; }
            set { _ThirdStepColor = value; RaisePropertyChanged(); }
        }

        private string _WeightCheckColor = "MediumPurple";

        public string WeightCheckColor
        {
            get { return _WeightCheckColor; }
            set { _WeightCheckColor = value; RaisePropertyChanged(); }
        }





        public void ChangeColor(string Operation)
        {
            try
            {
                switch (Operation)
                {
                    case "Scan":
                        this.ScanColor = "Green";
                        this.ClearDataColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        break;
                    case "ClearData":
                        this.ScanColor = "MediumPurple";
                        this.ClearDataColor = "Green";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        break;
                    case "FirstStep":
                        this.ScanColor = "MediumPurple";
                        this.ClearDataColor = "MediumPurple";
                        this.FirstStepColor = "Green";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        break;
                    case "SecondStep":
                        this.ScanColor = "MediumPurple";
                        this.ClearDataColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "Green";
                        this.ThirdStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        break;
                    case "ThirdStep":
                        this.ScanColor = "MediumPurple";
                        this.ClearDataColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "Green";
                        this.WeightCheckColor = "MediumPurple";
                        break;
                    case "WeightCheck":
                        this.ScanColor = "MediumPurple";
                        this.ClearDataColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.WeightCheckColor = "Green";
                        break;
                    case "SelectItem":
                        this.ScanColor = "MediumPurple";
                        this.ClearDataColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        break;
                    case "Print":
                        this.ScanColor = "MediumPurple";
                        this.ClearDataColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        break;
                    default:
                        this.ScanColor = "MediumPurple";
                        this.ClearDataColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        break;
                }


                if (button != null)
                {
                    System.Windows.Media.Color color1 = (
 System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("MediumPurple");
                    button.Background = new System.Windows.Media.SolidColorBrush(color1);
                }


            }
            catch (Exception)
            {


            }
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
        /// 扫描出库条码
        /// </summary>
        public RelayCommand<string> ScanBarcodeCommand { get; private set; }



        private RelayCommand _OutTaskCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> OutTaskCommand { get; private set; }



        private RelayCommand _selectItemCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<object> SelectItemCommand { get; private set; }//OutTaskMaterialDto


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


        private RelayCommand _PrintItemCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<object> PrintItemCommand { get; private set; }//OutTaskMaterialDto

        /// <summary>
        /// 退出登录
        /// </summary>
        private RelayCommand _LoginOutCommand;

        public RelayCommand<string> LoginOutCommand { get; private set; }

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
        /// 当前操作的任务明细
        /// </summary>
        private OutTaskMaterialDto outTaskMaterialEntity = new OutTaskMaterialDto();
        public OutTaskMaterialDto OutTaskMaterialEntity
        {
            get { return outTaskMaterialEntity; }
            set { outTaskMaterialEntity = value; RaisePropertyChanged(); }
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
        /// 自动取出
        /// </summary>
        private bool _AutoOprate = true;
        public bool AutoOprate
        {
            get { return _AutoOprate; }
            set { _AutoOprate = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 手动入库的物料
        /// </summary>
        private string selectMaterialCode = string.Empty;
        public string SelectMaterialCode
        {
            get { return selectMaterialCode; }
            set { selectMaterialCode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 手动入库的物料
        /// </summary>
        private string selectMaterialName = string.Empty;
        public string SelectMaterialName
        {
            get { return selectMaterialName; }
            set { selectMaterialName = value; RaisePropertyChanged(); }
        }
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
        /// 打印条码
        /// </summary>
        public async void PrintItem(object obj)//Bussiness.Dtos.OutTaskMaterialDto entity
        {
            try
            {
                ChangeColor("Print");
                object[] multiObj = obj as object[];
                Bussiness.Dtos.OutTaskMaterialDto entity = multiObj[0] as Bussiness.Dtos.OutTaskMaterialDto;
                if (multiObj[1] != null)
                {
                    button = multiObj[1] as System.Windows.Controls.Button;
                    //System.Windows.Controls.Button button = GetControlObject<System.Windows.Controls.Button>(entity.Id.ToString());

                    System.Windows.Media.Color color = (
                    System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("Green");
                    button.Background = new System.Windows.Media.SolidColorBrush(color);
                }
                if (String.IsNullOrEmpty(TrayCode))
                {
                    Msg.Warning("请先选择一项！");
                    return;
                }
                // 判断是否启用单包条码管理
                var materialEntity = MaterialContract.Materials.FirstOrDefault(a => a.Code == entity.MaterialCode);

                var label = new LabelClient()
                {
                    MaterialCode = entity.MaterialCode,
                    MaterialName = entity.MaterialName,
                    BatchCode = entity.BatchCode,
                    SupplyCode = entity.SupplierCode,
                };
                // 判断是否启用条码
                if (materialEntity.IsPackage)
                {
                    label.Quantity = (decimal)materialEntity.PackageQuantity;
                }
                else
                {

                    //var outLabel = OutTaskContract.OutTaskMaterialLabelDtos
                    //    .FirstOrDefault(a => a.LocationCode == OutTaskMaterialEntity.SuggestLocation
                    //                         && a.MaterialCode == OutTaskMaterialEntity.MaterialCode && a.BatchCode == OutTaskMaterialEntity.BatchCode);
                    label.Quantity = 0;
                    // label.LabelCode = outLabel.MaterialLabel;
                }

                var dialog = ServiceProvider.Instance.Get<IShowContent>();
                dialog.BindDataContext(new BarCodeView(), new BarCodeViewModel() { LabelEntity = label });
                //    dialog.D
                // 条码打印
                dialog.Show();
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
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
                var containerCode = OutTaskContract.OutTaskDtos.FirstOrDefault(a => a.Code == code).ContainerCode;
                if (containerCode != ContainerCode)
                {
                    Msg.Warning("该任务不属于本货柜,请选择正确的任务执行");
                    return;
                }
                GlobalData.LoginModule = "OutTask";
                GlobalData.LoginPageCode = "OutTaskDlg";
                GlobalData.LoginPageName = "出库任务";
                //如果登录
                if (await CheckLogin())
                {
                    // 获取当前任务的明细
                    var OutTaskMaterialList = OutTaskContract.OutTaskMaterialDtos.Where(a => a.OutTaskCode == code && a.Status != (int)OutTaskStatusCaption.Finished).OrderBy(a=>a.SuggestTrayCode).ToList();
                    OutTaskMaterial.Clear();
                    OutTaskMaterialList.ForEach((arg) => OutTaskMaterial.Add(arg));
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
        /// 扫描入库条码-物料条码-物料编码-储位码
        /// </summary>
        public async void ScanBarcode(string code)
        {
            try
            {
                ChangeColor("Scan");
                if (String.IsNullOrEmpty(SearchBarcode))
                {
                    return;
                }
                //  var dialog = ServiceProvider.Instance.Get<IShowContent>();
                var labelEnity = LabelContract.LabelDtos.Where(a => a.Code == SearchBarcode).FirstOrDefault();
                if (labelEnity == null)
                {
                    // 获取入库任务行项目
                    var selectOuttask = OutTaskMaterial.FirstOrDefault(a => a.MaterialCode == SearchBarcode && a.Status != (int)InTaskStatusCaption.Finished);
                    // 判断是否为物料编码
                    if (selectOuttask != null)
                    {
                        if (String.IsNullOrEmpty(TrayCode) || TrayCode != selectOuttask.SuggestTrayCode)
                        {
                            if (GlobalData.DeviceStatus == (int)DeviceStatusEnum.Fault)
                            {
                                GlobalData.IsFocus = true;
                                Msg.Warning("设备离线状态，无法启动货柜！");
                                return;
                            }
                            object[] obj = new object[2];
                            obj[0] = selectOuttask;
                            obj[1] = null;
                            // 默认选择一项相同物料的
                            SelectOutTaskItem(obj);
                        }
                        else
                        {
                            Msg.Warning("已选择出库行项目");
                            return;
                        }

                        Clear();
                    }
                    else
                    {
                        // 如果是储位码
                        var locationCode = WareHouseContract.Locations.FirstOrDefault(a => a.Code == SearchBarcode);

                        if (locationCode != null)
                        {
                            // 如果是当前储位
                            if (locationCode.Code == LocationCode)
                            {
                                // 从任务的物料中，找到该条码
                                var outTaskMaterialLocation = OutTaskContract.OutTaskMaterialLabelDtos.FirstOrDefault(a => a.LocationCode == SearchBarcode && a.TaskCode == OutTaskMaterialEntity.OutTaskCode && a.MaterialCode == OutTaskMaterialEntity.MaterialCode && a.BatchCode == OutTaskMaterialEntity.BatchCode);
                                if (outTaskMaterialLocation == null)
                                {
                                    Clear();
                                    //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "未获取到该储位的出库物料信息！", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                                    //dialog.Show();
                                    //await Task.Delay(3000);
                                    //DialogHost.CloseDialogCommand.Execute(null, null);
                                    Msg.Info("未获取到该储位的出库物料信息！");
                                    return;
                                }
                                else
                                {
                                    var material = MaterialContract.MaterialDtos.FirstOrDefault(a => a.Code == OutTaskMaterialEntity.MaterialCode);
                                    LabelEntity.MaterialCode = outTaskMaterialLocation.MaterialCode;
                                    LabelEntity.BatchCode = outTaskMaterialLocation.BatchCode;
                                    LabelEntity.MaterialName = outTaskMaterialLocation.MaterialName;
                                    LabelEntity.MaterialUrl = _basePath + material.PictureUrl;

                                    // 如果是自动取出
                                    if (AutoOprate && OutQuantity > 0)
                                    {
                                        HandShelf();
                                    }
                                    else
                                    {
                                        Clear();
                                        //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "请输入出库数量！", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                                        //dialog.Show();
                                        //await Task.Delay(3000);
                                        //DialogHost.CloseDialogCommand.Execute(null, null);
                                        Msg.Info("请输入出库数量！");
                                        return;
                                    }
                                }

                            }
                            else // 如果不是当前储位
                            {
                                Clear();
                                //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "未选择入库行项目，或该储位不属于该明细！", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                                //dialog.Show();
                                //await Task.Delay(3000);
                                //DialogHost.CloseDialogCommand.Execute(null, null);
                                Msg.Info("未选择入库行项目，或该储位不属于该明细！");
                                return;
                            }
                        }
                        else
                        {
                            Clear();
                            //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "未获取到条码信息！", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                            //dialog.Show();
                            //await Task.Delay(3000);
                            //DialogHost.CloseDialogCommand.Execute(null, null);
                            Msg.Info("未获取到条码信息！");
                            return;
                        }
                    }
                }
                else // 如果是物料条码
                {
                    LabelEntity.LabelCode = labelEnity.Code;
                    LabelEntity.MaterialCode = labelEnity.MaterialCode;
                    LabelEntity.MaterialName = labelEnity.MaterialName;
                    LabelEntity.Quantity = labelEnity.Quantity;
                    LabelEntity.SupplyName = labelEnity.SupplyName;
                    LabelEntity.BatchCode = labelEnity.BatchCode;
                    LabelEntity.MaterialUrl = _basePath + labelEnity.MaterialUrl;

                    // 从任务的物料中，找到该条码
                    var checkLabel = OutTaskContract.OutTaskMaterialLabelDtos.FirstOrDefault(a => a.MaterialLabel == SearchBarcode && a.TaskCode == OutTaskMaterialEntity.OutTaskCode);
                    if (checkLabel == null)
                    {
                        var material = MaterialContract.MaterialDtos.FirstOrDefault(a => a.Code == labelEnity.MaterialCode);
                        var stock = new Stock();
                        // 如果启用的单包
                        if (material.IsPackage)
                        {
                            stock = StockContract.StockDtos.FirstOrDefault(a => a.MaterialLabel == SearchBarcode);
                            // 本次出库数量
                            OutQuantity = labelEnity.Quantity;
                        }
                        else  // 如果没有启用单包
                        {
                            stock = StockContract.StockDtos.FirstOrDefault(a =>
                                a.MaterialCode == labelEnity.MaterialCode &&
                                a.LocationCode == OutTaskMaterialEntity.SuggestLocation &&
                                a.BatchCode == OutTaskMaterialEntity.BatchCode);

                            // 本次出库数量
                            OutQuantity = OutTaskMaterialEntity.Quantity;
                        }

                        if (stock == null)
                        {
                            Clear();
                            //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "该条码不在库存中，请重新扫描！", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                            //dialog.Show();
                            //await Task.Delay(3000);
                            //DialogHost.CloseDialogCommand.Execute(null, null);
                            Msg.Warning("该条码不符合本出库任务，请重新扫描！");
                            return;
                        }
                        // 如果属于本储位，且属于本行项目出库的物料
                        if (stock.MaterialCode == OutTaskMaterialEntity.MaterialCode && stock.LocationCode == OutTaskMaterialEntity.SuggestLocation)
                        {
                            if (!await Msg.Question("该条码不属于本次出库任务推荐物料，可能不符合先进先出规则，是否继续出库？"))
                            {
                                return;
                            }
                        }
                        else
                        {
                            Clear();
                            //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "该条码不属于本此出库行项目，请重新扫描！", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                            //dialog.Show();
                            //await Task.Delay(3000);
                            //DialogHost.CloseDialogCommand.Execute(null, null);
                            Msg.Warning("该条码不属于本此出库行项目，请重新扫描！");
                            return;
                        }
                    }
                    else
                    {
                        OutQuantity = checkLabel.Quantity;
                    }

                    // 如果是自动存入
                    if (AutoOprate)
                    {
                        HandShelf();
                    }
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        private static bool isAlarmRaised = false;

        /// <summary>
        /// 每隔200毫秒触发一次物料称重刷新
        /// </summary>
        /// <returns></returns>
        public async Task StartGetWeighingQuantity()
        {
            while (true)
            {
                if (String.IsNullOrEmpty(UnitWeight.ToString()))
                {
                    return;
                }
                GetWeighingQuantity();
                if (isAlarmRaised)
                {
                    return;
                }
                await Task.Delay(200);
            }
        }

        /// <summary>
        /// 返回当前称重物料数量
        /// </summary>
        public async void GetWeighingQuantity()
        {

            //try
            //{
                if (GlobalData.DeviceStatus == (int)DeviceStatusEnum.Fault)
                {
                    GlobalData.IsFocus = true;
                    isAlarmRaised = true;
                    Msg.Warning("货柜PLC离线状态，无法传递物料单重！");
                    return;
                }
                // 读取PLC 状态信息
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

                // 返回当前称重物料数量
                var weightResult = await baseControlService.GetBackWeighingQuantity();
                if (weightResult.Success)
                {
                    WeighingQuantity = decimal.Parse(weightResult.Data.ToString());
                    
                }
                //else
                //{
                    //GlobalData.IsFocus = true;
                    //Msg.Error(weightResult.Message);
                //}
            //}
            //catch (Exception ex)
            //{
               // Msg.Error(ex.Message);
            //}
        }


        public System.Windows.Controls.Button button;
        /// <summary>
        /// 选择任务行项目
        /// </summary>
        public async void SelectOutTaskItem(object obj)
        {
            try
            {
                ChangeColor("SelectItem");
                object[] multiObj = obj as object[];
                Bussiness.Dtos.OutTaskMaterialDto entity = multiObj[0] as Bussiness.Dtos.OutTaskMaterialDto;
                if (multiObj[1] != null)
                {
                    button = multiObj[1] as System.Windows.Controls.Button;
                    //System.Windows.Controls.Button button = GetControlObject<System.Windows.Controls.Button>(entity.Id.ToString());

                    System.Windows.Media.Color color = (
                    System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("Green");
                    button.Background = new System.Windows.Media.SolidColorBrush(color);
                }
                // 验证是否有操作权限
                GlobalData.IsFocus = false;
                if (entity == null)
                {
                    isAlarmRaised = true;
                    Msg.Warning("未获取到选中信息");
                    return;
                }

                if (entity.Status == (int)OutTaskStatusCaption.Finished)
                {
                    isAlarmRaised = true;
                    Msg.Warning("该物料已完成");
                    return;
                }
                // 核查用户是否有此模块操作权限
                var user = ServiceProvider.Instance.Get<IUserService>();
                var authCheck = user.GetCheckTrayAuth((int)entity.SuggestTrayId);
                var stockEnity = StockContract.StockDtos.Where(a => a.MaterialCode == entity.MaterialCode).FirstOrDefault();
                if (!authCheck.Result.Success)
                {
                    isAlarmRaised = true;
                    Msg.Warning("抱歉，您无操作该托盘权限！");
                    return;
                }
                TrayCode = entity.SuggestTrayCode;
                LocationCode = entity.SuggestLocation;
                OutTaskMaterialEntity = entity;
                BoxUrl = _basePath + entity.BoxUrl;
                MaterialUrl = _basePath + entity.MaterialUrl;
                XLight = entity.XLight;
                YLight = entity.YLight;
                BoxName = entity.BoxName;
                OutQuantity = entity.Quantity;
                SelectMaterialCode = entity.MaterialCode;
                SelectMaterialName = entity.MaterialName;
                UnitWeight = entity.UnitWeight;
                SearchBarcode = entity.SuggestLocation;
                AllQuantity = stockEnity.Quantity;
                GlobalData.IsFocus = true;

                RunningContainer();

                TransferWeight();

                await StartGetWeighingQuantity();
                //if (await Msg.Question("是否需要开始货柜?") == true)
                //{
                //    RunningContainer();
                //}
                //else
                //{
                //    return;
                //}
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }


        /// <summary>
        /// 获取全部货柜任务
        /// </summary>
        public async void GetAllPageData()
        {
            try
            {
                var query = OutTaskContract.OutTaskDtos.Where(a =>
                    a.Status != (int)OutTaskStatusCaption.Finished);

                if (!String.IsNullOrEmpty(SearchText))
                {
                    query = query.Where(p => p.OutCode.Contains(SearchText) || p.BillCode.Contains(SearchText));
                }

                // 查询全部任务
                var groupList = query.GroupBy(a => a.ContainerCode).Select(a => a.ContainerCode).ToList();
                _ModuleGroups.Clear();

                foreach (var item in groupList)
                {
                    var container = WareHouseContract.ContainerDtos.FirstOrDefault(a => a.Code == item);

                    var outTaskGroup = new OutTaskGroup()
                    {
                        GroupIcon = container.BrandDescription,
                        GroupName = "货柜号：" + container.Code,
                        GroupWarehouse = _basePath + container.PictureUrl
                    };
                    
                    // 查询全部任务
                    var OutTaskContainerList = OutTaskContract.OutTaskDtos.Where(a => a.Status != (int)OutTaskStatusCaption.Finished && a.Status != (int)OutTaskStatusCaption.Cancel && a.ContainerCode == item).ToList();

                    foreach (var OutTask in OutTaskContainerList)
                    {
                        var OutTaskItem = new OutTaskItem()
                        {
                            Code = OutTask.Code,
                            Name = OutTask.StatusCaption,
                            InCode = OutTask.OutCode,
                            ContainerCode = container.Code
                        };
                        outTaskGroup.Modules.Add(OutTaskItem);
                    }

                    _ModuleGroups.Add(outTaskGroup);
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
            finally
            {
                this.IsCancel = true;
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
                var outTaskGroup = new OutTaskGroup()
                {
                    GroupIcon = container.BrandDescription,
                    GroupName = "货柜号：" + container.Code,
                    GroupWarehouse = _basePath + container.PictureUrl
                };
                var query = OutTaskContract.OutTaskDtos.Where(a =>
                    a.Status != (int)OutTaskStatusCaption.Finished && a.Status != (int)OutTaskStatusCaption.Cancel && a.ContainerCode == ContainerCode);

                // 根据入库单或订单号查询
                if (!String.IsNullOrEmpty(SearchText))
                {
                    query = query.Where(p => p.OutCode.Contains(SearchText) || p.BillCode.Contains(SearchText));
                }

                // 查询全部任务
                var outTaskContainerList = query.ToList();

                foreach (var OutTask in outTaskContainerList)
                {
                    var OutTaskItem = new OutTaskItem()
                    {
                        Code = OutTask.Code,
                        Name = OutTask.StatusCaption,
                        InCode = OutTask.OutCode,
                        ContainerCode = container.Code
                    };
                    outTaskGroup.Modules.Add(OutTaskItem);
                }

                _ModuleGroups.Add(outTaskGroup);
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
            finally
            {
                this.IsCancel = true;
            }
        }

        /// <summary>
        /// 扫描传递物料单重
        /// </summary>
        public async void TransferWeight()
        {
            try
            {
                if (GlobalData.DeviceStatus == (int)DeviceStatusEnum.Fault)
                {
                    GlobalData.IsFocus = true;
                    Msg.Warning("货柜PLC离线状态，无法传递物料单重！");
                    return;
                }
                // 读取PLC 状态信息
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

                var runingEntity = new RunningContainer()
                {
                    ContainerCode = ContainerCode,
                    UnitWeight = UnitWeight
                };
                var container = ContainerRepository.Query().FirstOrDefault(a => a.Code == ContainerCode);
                if (container != null)
                {
                    runingEntity.ContainerType = container.ContainerType;
                    runingEntity.IpAddress = container.Ip;
                    runingEntity.Port = int.Parse(container.Port);
                }

                // 向货柜PLC传递物料单重
                var runningContainer = await baseControlService.PostStartScanBarcodeKeyDown(runingEntity);
                if (runningContainer.Success)
                {
                    //weighingQuantity = (decimal)runningContainer.Data;
                    //TakeInTrayNumber = runingEntity.TrayCode.ToString();
                }
                else
                {
                    GlobalData.IsFocus = true;
                    Msg.Error(runningContainer.Message);
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>XLight
        /// 启动货柜
        /// </summary>
        public async void RunningContainer()
        {
            try
            {
                //ChangeColor("FirstStep");
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

                string cfgINI = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
                string CurrentRunningTray = string.Empty;
                string CurrentIsTakeIn = string.Empty;
                if (File.Exists(cfgINI))
                {
                    IniFile ini = new IniFile(cfgINI);
                    CurrentRunningTray = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                    CurrentIsTakeIn = ini.IniReadValue("ClientInfo", "IsTakeIn");
                }

                if (CurrentRunningTray == TrayCode && CurrentIsTakeIn == "False")
                {
                    // 读取PLC 状态信息
                    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

                    var runingEntity = new RunningContainer()
                    {
                        ContainerCode = ContainerCode,
                        TrayCode = Convert.ToInt32(TrayCode),
                        XLight = XLight,
                        // XLenght

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
                    var runningContainer = baseControlService.PostStartRunningContainerAgain(runingEntity);

                    if (runningContainer.Result.Success)
                    {
                        GlobalData.IsFocus = true;
                        //var dialog = ServiceProvider.Instance.Get<IShowContent>();
                        //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "货柜运行中", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                        //dialog.Show();
                        //await Task.Delay(2000);
                        //DialogHost.CloseDialogCommand.Execute(null, null);
                        CurTratCode = TrayCode;
                        //Msg.Info("正在取出托盘,请确认托盘到达指定位置后再关闭窗口");
                    }
                    else
                    {
                        GlobalData.IsFocus = true;
                        Msg.Error(runningContainer.Result.Message);
                    }
                }
                else
                {
                    // 读取PLC 状态信息
                    var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

                    var runingEntity = new RunningContainer()
                    {
                        ContainerCode = ContainerCode,
                        TrayCode = Convert.ToInt32(TrayCode),
                        XLight = XLight,
                        // XLenght

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

                    if (runningContainer.Result.Success)
                    {
                        GlobalData.IsFocus = true;
                        //var dialog = ServiceProvider.Instance.Get<IShowContent>();
                        //dialog.BindDataContext(new MsgBox(), new MsgBoxViewModel() { Msg = "货柜运行中", Icon = "CommentProcessingOutline", Color = "#FF4500", BtnHide = true });
                        //dialog.Show();
                        //await Task.Delay(2000);
                        //DialogHost.CloseDialogCommand.Execute(null, null);
                        CurTratCode = TrayCode;
                        //Msg.Info("正在取出托盘,请确认托盘到达指定位置后再关闭窗口");
                    }
                    else
                    {
                        GlobalData.IsFocus = true;
                        Msg.Error(runningContainer.Result.Message);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
            finally
            {
                this.IsCancel = true;
            }
        }
        /// <summary>
        /// 存入货柜
        /// </summary>
        public async void RunningTakeInContainer()
        {
            try
            {
                ChangeColor("ThirdStep");
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
                    //if (!string.IsNullOrEmpty(LocationCode))
                    //{
                    //    var locationEntity = WareHouseContract.Locations.FirstOrDefault(a => a.Code == LocationCode);
                    //    if (locationEntity != null)
                    //    {
                    //        runingEntity.XLenght = locationEntity.XLenght.GetValueOrDefault(0);
                    //    }
                    //}
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
                        Msg.Info("正在存入托盘,请确认托盘已存入货柜后在关闭窗口");
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
        /// 确认取出
        /// </summary>
        public void HandShelf()
        {
            try
            {
                ChangeColor("SecondStep");
                if (String.IsNullOrEmpty(TrayCode))
                {
                    Msg.Warning("未选择入库的行项目，请先选择一项！");
                    return;
                }

                if (CurTratCode != TrayCode)
                {
                    Msg.Warning("当前货柜未运行至此托盘，请启动货柜！");
                    return;
                }


                // 判断是否启用单包条码管理
                var materialEntity =
                    MaterialContract.Materials.FirstOrDefault(a => a.Code == OutTaskMaterialEntity.MaterialCode);
                var labelEnity = LabelContract.LabelDtos.Where(a => a.Code == SearchBarcode).FirstOrDefault();

                // 判断是否启用条码
                if (materialEntity.IsPackage)
                {
                    // 判断条码是否为空，以及是否为物料条码
                    if (String.IsNullOrEmpty(SearchBarcode))
                    {
                        // OutQuantity = 0;
                        SearchBarcode = "";
                        Msg.Warning("该物料已经启用条码管理，请扫描出库条码");
                        return;
                    }

                    if (labelEnity == null)
                    {
                        // OutQuantity = 0;
                        SearchBarcode = "";
                        Msg.Warning("该物料已经启用条码管理，请扫描出库条码");
                        return;
                    }
                    else
                    {
                        OutTaskMaterialEntity.MaterialLabel = SearchBarcode;
                    }
                } // 如果未启用单包
                else
                {
                    // 从推荐库存中获取
                    var outLabel = OutTaskContract.OutTaskMaterialLabelDtos
                        .FirstOrDefault(a => a.LocationCode == OutTaskMaterialEntity.SuggestLocation && a.TaskCode== OutTaskMaterialEntity.OutTaskCode
                                             && a.MaterialCode == OutTaskMaterialEntity.MaterialCode &&
                                             a.BatchCode == OutTaskMaterialEntity.BatchCode);
                    OutTaskMaterialEntity.MaterialLabel = outLabel.MaterialLabel;
                }

                // 核对入库物料是否正确
                if (LabelEntity.MaterialCode != OutTaskMaterialEntity.MaterialCode)
                {
                    Clear();
                    Msg.Warning("该物料条码不属于本出库项目，请核对扫描的物料信息！");
                    return;
                }

                this.IsCancel = false;

                OutTaskMaterialEntity.OutTaskMaterialQuantity = OutQuantity;

                // 实际上架储位-后期可维护成可做修改的
                OutTaskMaterialEntity.LocationCode = LocationCode;

                var OutTaskService = ServiceProvider.Instance.Get<IOutTaskService>();

                //   物料实体映射

                var OutTask = OutTaskService.ConfirmHandPicked(OutTaskMaterialEntity);

                if (OutTask.Result.Success)
                {
                    Clear();
                    // 获取当前任务的明细
                    var OutTaskMaterialList = OutTaskContract.OutTaskMaterialDtos.Where(a =>
                        a.OutTaskCode == OutTaskMaterialEntity.OutTaskCode &&
                        a.Status != (int) OutTaskStatusCaption.Finished).OrderBy(a => a.SuggestTrayCode).ToList();
                    OutTaskMaterial.Clear();
                    OutTaskMaterialList.ForEach((arg) => OutTaskMaterial.Add(arg));
                    if (OutTaskMaterial.Count > 0)
                    {
                        // 获取入库任务行项目
                        var selectOutTask = OutTaskMaterial.FirstOrDefault(a =>
                            a.Status != (int) OutTaskStatusCaption.Finished &&
                            a.Status != (int) OutTaskStatusCaption.Cancel);
                        if (selectOutTask != null)
                        {
                            object[] obj = new object[2];
                            obj[0] = selectOutTask;
                            obj[1] = null;
                            // 默认选择一项相同物料的
                            SelectOutTaskItem(obj);
                        }
                    }
                }
                else
                {
                    Clear();
                    Msg.Error("物料出库失败：" + OutTask.Result.Message);
                }
            }
            catch (Exception ex)
            {
                Clear();
                Msg.Error("物料出库失败" + ex.Message);
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
                if (OutTaskMaterialEntity.OutCode == null)
                {
                    Msg.Warning("未执行出库操作");
                    return;
                }

                var OutTaskList = OutTaskContract.OutTaskDtos.Where(a => a.OutCode == OutTaskMaterialEntity.OutCode && a.Status != (int)OutTaskStatusCaption.Finished).ToList();

                // 仍有未完成的任务
                if (OutTaskList.Count > 0)
                {
                    // 全局变量设置需要指引的出库单
                    GlobalData.GuideCode = OutTaskMaterialEntity.OutCode;
                    GlobalData.GuideType = (int)ModuleType.OutManage;
                    //var dialog = ServiceProvider.Instance.Get<IShowContent>();
                    //dialog.BindDataContext(new GuideWindow(), new GuideModel());
                    //// 系统登录
                    //dialog.Show();

                    string code = "";
                    foreach (var item in OutTaskList)
                    {
                        code += item.ContainerCode + ",";
                    }
                    code = code.Substring(0, code.Length - 1);
                    Msg.Info(string.Format("该单据尚有{0}货柜出库任务未完成", code));

                } 
                else // 直接跳转至任务界面
                {
                    TabPageIndex = 0;
                    this.GetAllPageData();
                }
            }
            catch (Exception ex)
            {
                Msg.Info("物料出库失败：");
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
                ChangeColor("ClearData");
                GlobalData.IsFocus = false;
                // 清空
                LabelEntity.MaterialName = "";
                LabelEntity.MaterialCode = "";
                LabelEntity.BatchCode = "";
                LabelEntity.Quantity = 0;
                LabelEntity.SupplyName = "";
                OutQuantity = 0;
                SearchBarcode = "";
                SelectMaterialCode = "";
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

        #region 重量操作
        /// <summary>
        /// 获取重量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void weightReader_Changed(object sender, EventArgs e)
        {
            InWeight = Convert.ToDecimal(weightReader.WeightInformationObj.WData.ToString());

            if (InWeight > 0)
            {
                // 解除托盘的重量临时锁定
                //  托盘实体
                //var trayEntity = WareHouseContract.TrayWeightMapRepository.GetEntity(a => a.TrayId == TrayId);

                ////  如果维护了托盘最大称重
                //if (trayEntity.MaxWeight > 0)
                //{
                //    var aviWeight = trayEntity.MaxWeight - trayEntity.LockWeight + trayEntity.TempLockWeight;

                //    if (InWeight *1000<= aviWeight)
                //    {
                //        App.Current.Dispatcher.Invoke(new Action(() => {
                //            Msg.Warning("核验成功，重量可存放！");
                //        }));

                //    }
                //    else
                //    {
                //        App.Current.Dispatcher.Invoke(new Action(() => {
                //            Msg.Warning("核验失败，已超过最大存放重量！");
                //        }));

                //    }
                //}
                //赋值数量
                if (!string.IsNullOrEmpty(SelectMaterialCode))
                {
                    //var material = MaterialContract.Materials.FirstOrDefault(a => a.Code == SelectMaterialCode);
                    //decimal weight = material.UnitWeight;
                    if (IsCheckUnitWeight == true)
                    {
                        UnitWeight = InWeight * 1000;
                    }
                    else
                    {
                        if (UnitWeight == 0)
                        {
                            App.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                Msg.Warning("尚未维护该物料单品重量或输入单品重量");
                            }));
                            return;
                        }
                        decimal quantity = Math.Round(InWeight * 1000 / UnitWeight);
                        OutQuantity = quantity;
                    }
                }

            }
        }

        /// <summary>
        /// 核验重量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightCheck()
        {
            try
            {
                ChangeColor("WeightCheck");
                // weightReader.Open(COM, 9600);
                if (weightReader.Open(COM, 9600))
                {
                    var sendData = weightReader.strToHexByte("01 03 00 00 00 02 c4 0b");
                    weightReader.SendData(sendData);
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }
        #endregion
    }
}
