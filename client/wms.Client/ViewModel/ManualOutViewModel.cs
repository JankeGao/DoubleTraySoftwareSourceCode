using System;
using System.Collections.Generic;
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
using HP.Utility;
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;
using MaterialDesignThemes.Wpf;
using SearchableTextBox.Models;
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
using wms.Client.ViewModel;
using ILabelContract = Bussiness.Contracts.ILabelContract;
using RunningContainer = wms.Client.Model.Entity.RunningContainer;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 手动出库
    /// </summary>
    [Module(ModuleType.OutManage, "ManualOutDlg", "手动出库")]
    public class ManualOutViewModel : Base.DataProcess<OutTask>
    {
        private readonly string _basePath = ConfigurationManager.AppSettings["ServerIP"];


        /// <summary>
        /// 仓库契约
        /// </summary>
        private readonly IWareHouseContract WareHouseContract;


        /// <summary>
        /// 仓库契约
        /// </summary>
        private readonly IStockContract StockContract;
        
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
        private readonly ISupplyContract SupplyContract;

        /// <summary>
        /// 字典契约
        /// </summary>
        private readonly IDictionaryContract DictionaryContract;

        /// <summary>
        /// 条码契约
        /// </summary>
        private readonly IMapper Mapper;


        private readonly IRepository<Bussiness.Entitys.Container, int> ContainerRepository;

        private readonly IRepository<Material, int> MaterialRepository;


        // 称重接口
        WeightReader weightReader = new WeightReader();
        Action<string> updateWeightAction;
        private string COM;
        public ManualOutViewModel()
        {
            MaterialRepository = IocResolver.Resolve<IRepository<Material, int>>();
            ContainerRepository = IocResolver.Resolve<IRepository<Bussiness.Entitys.Container, int>>();
            SupplyContract = IocResolver.Resolve<ISupplyContract>();
            DictionaryContract = IocResolver.Resolve<IDictionaryContract>();
            StockContract = IocResolver.Resolve<IStockContract>();
            LabelContract = IocResolver.Resolve<ILabelContract>();
            WareHouseContract = IocResolver.Resolve<IWareHouseContract>();
            Mapper = IocResolver.Resolve<IMapper>();
            MaterialContract = IocResolver.Resolve<IMaterialContract>();
            ScanBarcodeCommand = new RelayCommand<string>(ScanBarcode);
            StraightOutTrayCommand = new RelayCommand(StraightOutTray);
            StraightInTrayCommand = new RelayCommand(StraightInTray);
            SynchronousQuantityCommand = new RelayCommand(SynchronousQuantity);
            HandShelfCommand = new RelayCommand (HandShelf);
            PrintItemCommand = new RelayCommand<OutTaskMaterialLabelDto>(PrintItem);
            RunningCommand = new RelayCommand(RunningContainer);
            RunningTakeInCommand = new RelayCommand(RunningTakeInContainer); 
             SubmitCommand = new RelayCommand(Submit);
            LoginOutCommand = new RelayCommand<string>(LoginOut);
            DelectItemCommand = new RelayCommand<OutTaskMaterialLabelDto>(DelectOutTaskItem);
            MouseEnterCommand = new RelayCommand(ImageMouseEnter);

            weightReader.StartKey = string.Empty;
            weightReader.UnitKey = string.Empty;
            weightReader.MatchPattern = @".+[\r|\n]";
            COM = System.Configuration.ConfigurationSettings.AppSettings["WeightCOM"].ToString();
            weightReader.Changed += new EventHandler(weightReader_Changed);


            this.ReadConfigInfo(); // 读取货柜信息
            this.GetAllPageData();
            this.SelectInit();
            this.ShowLogin();
            this.GetAllMaterial();
        }


        private bool _IsCancel = true;
        public ImageWindow imageWindow;
        /// <summary>
        /// 禁用按钮
        /// </summary>
        public bool IsCancel
        {
            get { return _IsCancel; }
            set { _IsCancel = value; RaisePropertyChanged(); }
        }

        private bool _isWeighingQuantity = false;
        /// <summary>
        /// 同步数量按钮是否可用
        /// </summary>
        public bool IsWeighingQuantity
        {
            get { return _isWeighingQuantity; }
            set { _isWeighingQuantity = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<SearchModel> SearchItemsSourceCollection { get; set; }

        private void GetAllMaterial()
        {
            SearchItemsSourceCollection = new ObservableCollection<SearchModel>();
            //for (int i = 0; i < 10000; i++)
            //{
            //    SearchModel sm = new SearchModel();
            //    sm.Name = "测试数据" + i;
            //    sm.Id = Guid.NewGuid().ToString();
            //    sm.SearchField = "搜索测试" + i + "附加数据" + i;
            //    SearchItemsSourceCollection.Add(sm);
            //}
            var list = this.MaterialRepository.Query().Select(a => new SearchModel()
            {
                Name = a.Code + "-" + a.Name,
                Id = a.Code,
                SearchField = a.Code+a.Name


            }).ToList();
            list.ForEach(p => SearchItemsSourceCollection.Add(p));
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

        // 物料分组
        private ObservableCollection<Material> _ModuleGroups = new ObservableCollection<Material>();
        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<Material> ModuleGroups
        {
            get { return _ModuleGroups; }
            set { _ModuleGroups = value; RaisePropertyChanged(); }
        }



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
        /// 储位分组
        /// </summary>
        private ObservableCollection<LocationVIEW> _LocationGroups = new ObservableCollection<LocationVIEW>();
        public ObservableCollection<LocationVIEW> LocationGroups
        {
            get { return _LocationGroups; }
            set { _LocationGroups = value; RaisePropertyChanged(); }
        }


        // 供应商分组
        private ObservableCollection<Stock> _BatchCodeGroups = new ObservableCollection<Stock>();
        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<Stock> BatchCodeGroups
        {
            get { return _BatchCodeGroups; }
            set { _BatchCodeGroups = value; RaisePropertyChanged(); }
        }

        // 供应商分组
        private ObservableCollection<Supply> _SupplyGroups = new ObservableCollection<Supply>();
        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<Supply> SupplyGroups
        {
            get { return _SupplyGroups; }
            set { _SupplyGroups = value; RaisePropertyChanged(); }
        }

        // 供应商分组
        private ObservableCollection<Dictionary> _InTypeGroups = new ObservableCollection<Dictionary>();
        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<Dictionary> InTypeGroups
        {
            get { return _InTypeGroups; }
            set { _InTypeGroups = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        private ObservableCollection<OutTaskMaterialLabelDto> _OutTaskMaterial = new ObservableCollection<OutTaskMaterialLabelDto>();
        public ObservableCollection<OutTaskMaterialLabelDto> OutTaskMaterial
        {
            get { return _OutTaskMaterial; }
            set { _OutTaskMaterial = value; RaisePropertyChanged(); }
        }


        #endregion

        #region 命令(Binding Command)



        /// <summary>
        /// 退出登录
        /// </summary>
        private RelayCommand _LoginOutCommand;

        public RelayCommand<string> LoginOutCommand { get; private set; }
        public RelayCommand MouseEnterCommand
        {
            get;
            private set;
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
        /// 选择本次出库的物料
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
        /// 选择本次出库的物料
        /// </summary>
        private RelayCommand _locationCommand;

        public RelayCommand LocationCommand
        {
            get
            {
                if (_locationCommand == null)
                {
                    _locationCommand = new RelayCommand(() => SelectLocation());
                }
                return _locationCommand;
            }
        }

        private RelayCommand _PrintItemCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<OutTaskMaterialLabelDto> PrintItemCommand { get; private set; }

        /// <summary>
        /// 选择本次出库的物料
        /// </summary>
        private RelayCommand _batchCodeCommand;

        public RelayCommand BatchCodeCommand
        {
            get
            {
                if (_batchCodeCommand == null)
                {
                    _batchCodeCommand = new RelayCommand(() => SelectBatch());
                }
                return _batchCodeCommand;
            }
        }


        /// <summary>
        /// 完成提交
        /// </summary>
        private RelayCommand _supplerCommand;

        public RelayCommand SupplerCommand { get; private set; }

        private RelayCommand _scanBarcodeCommand;

        /// <summary>
        /// 扫描出库条码
        /// </summary>
        public RelayCommand<string> ScanBarcodeCommand { get; private set; }

        /// <summary>
        /// 直接取出托盘
        /// </summary>
        public RelayCommand StraightOutTrayCommand { get; private set; }
        public RelayCommand StraightInTrayCommand { get; private set; }
        public RelayCommand SynchronousQuantityCommand { get; private set; }



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

        private RelayCommand _delectItemCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<OutTaskMaterialLabelDto> DelectItemCommand { get; private set; }

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
        /// 客户端货柜编码
        /// </summary>
        private string ContainerCode = string.Empty;


        /// <summary>
        /// 手动出库的物料
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
        /// 当前操作储位
        /// </summary>
        private string locationCode = string.Empty;
        public string SelectLocationCode
        {
            get { return locationCode; }
            set { locationCode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前选择供应商
        /// </summary>
        private string selectSupplerCode = string.Empty;
        public string SelectSupplerCode
        {
            get { return selectSupplerCode; }
            set { selectSupplerCode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 搜索条码
        /// </summary>
        private string selectSuppler = string.Empty;
        public string SelectSupplerName
        {
            get { return selectSuppler; }
            set { selectSuppler = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前操作储位
        /// </summary>
        private decimal outQuantity = 1;
        public decimal OutQuantity
        {
            get { return outQuantity; }
            set { outQuantity = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 生产日期
        /// </summary>
        private DateTime? manufactureDate = DateTime.Now;
        public DateTime? ManufactureDate
        {
            get { return manufactureDate; }
            set { manufactureDate = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 关联订单
        /// </summary>
        private string billCode = string.Empty;
        public string BillCode
        {
            get { return billCode; }
            set { billCode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 关联订单
        /// </summary>
        private string remark = string.Empty;
        public string Remark
        {
            get { return remark; }
            set { remark = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 搜索条码
        /// </summary>
        private string batchCode = string.Empty;
        public string SelectBatchCode
        {
            get { return batchCode; }
            set { batchCode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 关联订单
        /// </summary>
        private string inType = string.Empty;
        public string InType
        {
            get { return inType; }
            set { inType = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 搜索条码
        /// </summary>
        private string searchBarcode = string.Empty;
        public string SearchBarcode
        {
            get { return searchBarcode; }
            set { searchBarcode = value; RaisePropertyChanged(); }
        }


        private string boxName = string.Empty;
        public string BoxName
        {
            get { return boxName; }
            set { boxName = value; RaisePropertyChanged(); }
        }

        private string boxUrl = string.Empty;
        public string BoxUrl
        {
            get { return boxUrl; }
            set { boxUrl = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 自动存入
        /// </summary>
        private bool _AutoOprate = false;
        public bool AutoOprate
        {
            get { return _AutoOprate; }
            set { _AutoOprate = value; RaisePropertyChanged(); }
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
        /// <summary>
        /// 当前托盘-货柜运行后
        /// </summary>
        private string curTratCode = string.Empty;
        public string CurTratCode
        {
            get { return curTratCode; }
            set { curTratCode = value; RaisePropertyChanged(); }
        }
        private int orgXLight = 0;


        private decimal _AviQuantity = 0;
        public decimal AviQuantity
        {
            get { return _AviQuantity; }
            set { _AviQuantity = value; RaisePropertyChanged(); }
        }
        private decimal _AllQuantity = 0;
        public decimal AllQuantity
        {
            get { return _AllQuantity; }
            set { _AllQuantity = value; RaisePropertyChanged(); }
        }

        private decimal _AviCount = 0;
        public decimal AviCount
        {
            get { return _AviCount; }
            set { _AviCount = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 手动入库的物料
        /// </summary>
        private string _IsMoreThanTwo = "Hidden";
        public string IsMoreThanTwo
        {
            get { return _IsMoreThanTwo; }
            set { _IsMoreThanTwo = value; RaisePropertyChanged(); }
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
        /// 当前操作的任务明细
        /// </summary>
        private OutTaskMaterialLabelDto outTaskMaterialLabelEntity = new OutTaskMaterialLabelDto();
        public OutTaskMaterialLabelDto OutTaskMaterialLabelEntity
        {
            get { return outTaskMaterialLabelEntity; }
            set { outTaskMaterialLabelEntity = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前操作的任务明细
        /// </summary>
        private List<StockDto> aviStockList = new List<StockDto>();
        public List<StockDto> AviStockList
        {
            get { return aviStockList; }
            set { aviStockList = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 扫描的条码实体
        /// </summary>
        public LabelClient LabelEntity { get; set; } = new LabelClient();


        private string materialUrl = string.Empty;
        public string MaterialUrl
        {
            get { return materialUrl; }
            set { materialUrl = value; RaisePropertyChanged(); }
        }
        #endregion


        #region 颜色变化
        private string _ScanColor = "MediumPurple";
        public string ScanColor
        {
            get { return _ScanColor; }
            set { _ScanColor = value; RaisePropertyChanged(); }
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
        private string _FourthStepColor = "MediumPurple";
        public string FourthStepColor
        {
            get { return _FourthStepColor; }
            set { _FourthStepColor = value; RaisePropertyChanged(); }
        }

        private string _WeightCheckColor = "MediumPurple";

        public string WeightCheckColor
        {
            get { return _WeightCheckColor; }
            set { _WeightCheckColor = value; RaisePropertyChanged(); }
        }
        private string _StraightInTrayColor = "MediumPurple";

        public string StraightInTrayColor
        {
            get { return _StraightInTrayColor; }
            set { _StraightInTrayColor = value; RaisePropertyChanged(); }
        }

        private string _StraightOutTrayColor = "MediumPurple";

        public string StraightOutTrayColor
        {
            get { return _StraightOutTrayColor; }
            set { _StraightOutTrayColor = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 当前取出托盘号
        /// </summary>
        public string TakeInTrayNumberes { get; set; }

        public void ChangeColor(string Operation)
        {
            try
            {
                switch (Operation)
                {
                    case "Scan":
                        this.ScanColor = "Green";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.FourthStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        this.StraightInTrayColor = "MediumPurple";
                        this.StraightOutTrayColor = "MediumPurple";
                        break;
                    case "FirstStep":
                        this.ScanColor = "MediumPurple";
                        this.FirstStepColor = "Green";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.FourthStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        this.StraightInTrayColor = "MediumPurple";
                        this.StraightOutTrayColor = "MediumPurple";
                        break;
                    case "SecondStep":
                        this.ScanColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "Green";
                        this.ThirdStepColor = "MediumPurple";
                        this.FourthStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        this.StraightInTrayColor = "MediumPurple";
                        this.StraightOutTrayColor = "MediumPurple";
                        break;
                    case "ThirdStep":
                        this.ScanColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "Green";
                        this.FourthStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        this.StraightInTrayColor = "MediumPurple";
                        this.StraightOutTrayColor = "MediumPurple";
                        break;
                    case "FourthStep":
                        this.ScanColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.FourthStepColor = "Green";
                        this.WeightCheckColor = "MediumPurple";
                        this.StraightInTrayColor = "MediumPurple";
                        this.StraightOutTrayColor = "MediumPurple";
                        break;
                    case "WeightCheck":
                        this.ScanColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.FourthStepColor = "MediumPurple";
                        this.WeightCheckColor = "Green";
                        this.StraightInTrayColor = "MediumPurple";
                        this.StraightOutTrayColor = "MediumPurple";
                        break;
                    case "StraightInTrayCheck":
                        this.ScanColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.FourthStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        this.StraightInTrayColor = "Green";
                        this.StraightOutTrayColor = "MediumPurple";
                        break;
                    case "StraightOutTrayCheck":
                        this.ScanColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.FourthStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        this.StraightInTrayColor = "MediumPurple";
                        this.StraightOutTrayColor = "Green";
                        break;
                    default:
                        this.ScanColor = "MediumPurple";
                        this.FirstStepColor = "MediumPurple";
                        this.SecondStepColor = "MediumPurple";
                        this.ThirdStepColor = "MediumPurple";
                        this.FourthStepColor = "MediumPurple";
                        this.WeightCheckColor = "MediumPurple";
                        this.StraightInTrayColor = "MediumPurple";
                        this.StraightOutTrayColor = "MediumPurple";
                        break;
                }
            }
            catch (Exception)
            {


            }
        }
        #endregion

        public void ImageMouseEnter()
        {
            Uri uri = new Uri(this.LabelEntity.MaterialUrl);
            BitmapImage image = new BitmapImage(uri);

            imageWindow = new ImageWindow();
            imageWindow.ImageSrc.Source = image;

            //imageWindow.ShowDialog("");
            var dialog = ServiceProvider.Instance.Get<IShowContent>();
            dialog.BindDataContext(imageWindow, new ImageShowModel());
            dialog.Show();

        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="code"></param>
        /// 


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
        public async void PrintItem(Bussiness.Dtos.OutTaskMaterialLabelDto entity)
        {
            try
            {
                // 判断是否启用单包条码管理
                var materialEntity = MaterialContract.Materials.FirstOrDefault(a => a.Code == entity.MaterialCode);

                decimal tempQuantity = 0;
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
                    var outLabel = StockContract.StockDtos.FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel);
                    label.Quantity = outLabel.Quantity;
                    label.LabelCode = outLabel.MaterialLabel;
                }
                else
                {
                    label.Quantity = 0;
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
        public async void ShowLogin()
        {
            try
            {
                GlobalData.LoginModule = "OutTask";
                GlobalData.LoginPageCode = "ManualOutDlg";
                GlobalData.LoginPageName = "手动出库";
                //如果登录
                if (await CheckLogin())
                {
                    this.GetAllPageData();
                    GlobalData.IsFocus = true;
                }
                else //如果未登录
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
                var runningContainer = baseControlService.PostStartScanBarcodeKeyDown(runingEntity);
                if (runningContainer.Result.Success)
                {
                    //unitWeight = runingEntity.UnitWeight;
                    //TakeInTrayNumber = runingEntity.TrayCode.ToString();
                }
                else
                {
                    GlobalData.IsFocus = true;
                    Msg.Error(runningContainer.Result.Message);
                }
            }
            catch (Exception ex)
            {

                Msg.Error(ex.Message); ;
            }
        }

        /// <summary>
        /// 每隔200毫秒触发一次物料称重刷新
        /// </summary>
        /// <returns></returns>
        public async Task StartGetWeighingQuantity()
        {
            while (true)
            {
                if (String.IsNullOrEmpty(SearchBarcode))
                {
                    return;
                }
                GetWeighingQuantity();
                if (isAlarmRaised || WeighingQuantity == 0)
                {
                    return;
                }
                await Task.Delay(200);
            }
        }

        private static bool isAlarmRaised = false;

        /// <summary>
        /// 返回当前称重物料数量
        /// </summary>
        public async void GetWeighingQuantity()
        {

            try
            {
                if (GlobalData.DeviceStatus == (int)DeviceStatusEnum.Fault)
                {
                    GlobalData.IsFocus = true;
                    Msg.Warning("货柜PLC离线状态，无法传递物料单重！");
                    isAlarmRaised = true;
                    return;
                }
                // 读取PLC 状态信息
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

                // 返回当前称重物料数量
                var weightResult = await baseControlService.GetBackWeighingQuantity();
                if (weightResult.Success)
                {
                    WeighingQuantity = decimal.Parse(weightResult.Data.ToString());
                    if (weighingQuantity > 0)
                    {
                        OutQuantity = weighingQuantity;
                        IsWeighingQuantity = true;
                    }
                    else
                    {
                        IsWeighingQuantity = false;
                    }
                }
                else
                {
                    GlobalData.IsFocus = true;
                    Msg.Error(weightResult.Message);
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// 同步物料数量
        /// </summary>
        public void SynchronousQuantity()
        {
            OutQuantity = WeighingQuantity;
        }

        /// <summary>
        /// 扫描出库条码
        /// </summary>
        public async void ScanBarcode(string code)
        {
            try
            {
                ChangeColor("Scan");
                // 清空料盒提示
                IsMoreThanTwo = "Hidden";
                OutQuantity = 1;

                if (String.IsNullOrEmpty(SearchBarcode))
                {
                    return;
                }
                var dialog = ServiceProvider.Instance.Get<IShowContent>();
                var labelEnity = LabelContract.LabelDtos.Where(a => a.Code == SearchBarcode).FirstOrDefault();
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

                    // 如果是储位码
                    var locationCode = WareHouseContract.Locations.FirstOrDefault(a => a.Code == SearchBarcode);

                    //if (locationCode != null && locationCode.Code == SelectLocationCode)
                    //{
                    //    // 查找该储位的库存
                    //    var stock = AviStockList.FirstOrDefault(a => a.LocationCode == locationCode.Code
                    //                                                    && a.MaterialCode == SelectMaterialCode
                    //                                                    && a.BatchCode == SelectBatchCode);
                    //    LabelEntity.MaterialCode = stock.MaterialCode;
                    //    LabelEntity.MaterialName = stock.MaterialName;
                    //    LabelEntity.MaterialUrl = _basePath + stock.PictureUrl;
                    //    OutQuantity = stock.Quantity - stock.LockedQuantity;
                    //}
                    if (locationCode != null )
                    {
                        // 查找该储位的库存
                        var stock = StockContract.StockDtos.FirstOrDefault(a => a.LocationCode == locationCode.Code);
                        if (stock == null)
                        {
                            Msg.Warning("该出库物料无库存！");
                            isAlarmRaised = true;
                            return;
                        }
                        LabelEntity.MaterialCode = stock.MaterialCode;
                        LabelEntity.MaterialName = stock.MaterialName;
                        LabelEntity.MaterialUrl = _basePath + stock.PictureUrl;
                        SelectMaterialCode = stock.MaterialCode;
                        SelectMaterialName = stock.MaterialName;
                        var materialEntity = MaterialContract.Materials.FirstOrDefault(a => a.Code == stock.MaterialCode);
                        if (materialEntity!=null)
                        {
                            UnitWeight = materialEntity.UnitWeight;
                        }
                        // OutQuantity = stock.Quantity - stock.LockedQuantity;
                    }
                    else
                    {
                        var materialEntity = MaterialContract.Materials.FirstOrDefault(a => a.Code == SearchBarcode);
                        if (materialEntity != null)
                        {
                            SelectMaterialCode = materialEntity.Code;
                            SelectMaterialName = materialEntity.Name;
                            UnitWeight = materialEntity.UnitWeight;
                            LabelEntity.MaterialCode = materialEntity.Code;
                            LabelEntity.MaterialName = materialEntity.Name;
                            SelectMaterialName = materialEntity.Name;
                            SelectMaterialCode = materialEntity.Code;
                            UnitWeight = materialEntity.UnitWeight;
                            LabelEntity.MaterialUrl = _basePath + materialEntity.PictureUrl;
                        }
                        else
                        {
                            Clear();
                            Msg.Warning("未获取到条码信息!");
                            isAlarmRaised = true;
                            return;
                        }
                    }
                    SelectMaterial();
                    TransferWeight();
                    await StartGetWeighingQuantity();
                }
                else // 如果是物料条码
                {
                    //本次出库数量-需要核验
                    if (SelectMaterialCode != labelEnity.MaterialCode)
                    {
                        Msg.Warning("扫描的物料与待出库物料不同，请核验");
                        isAlarmRaised = true;
                        return;
                    }
                    //本次出库数量-需要核验
                    if (SelectBatchCode != labelEnity.BatchCode)
                    {
                        Msg.Warning("扫描的物料的批次与待出库物料批次不同，请核验");
                        isAlarmRaised = true;
                        return;
                    }
                    // 查找该储位的库存
                    var stock = AviStockList.FirstOrDefault(a => a.MaterialLabel == labelEnity.Code);

                    //本次出库数量-需要核验
                    if (SelectLocationCode != stock.LocationCode)
                    {
                        Msg.Warning("扫描的物料的不在此储位，请核验");
                        isAlarmRaised = true;
                        return;
                    }
                    LabelEntity.LabelCode = labelEnity.Code;
                    LabelEntity.MaterialCode = labelEnity.MaterialCode;
                    LabelEntity.MaterialName = labelEnity.MaterialName;
                    LabelEntity.Quantity = labelEnity.Quantity;
                    LabelEntity.SupplyName = labelEnity.SupplyName;
                    LabelEntity.BatchCode = labelEnity.BatchCode;
                    LabelEntity.MaterialUrl = _basePath + labelEnity.MaterialUrl;
                    // 本次出库数量
                    OutQuantity = labelEnity.Quantity;
                    var material = MaterialContract.Materials.FirstOrDefault(a => a.Code == labelEnity.MaterialCode);
                    if (material == null)
                    {
                        Msg.Warning("该储位未维护存放物料，请扫描物料信息！");
                        isAlarmRaised = true;
                        return;
                    }
                    UnitWeight = material.UnitWeight;
                    SelectMaterial();
                    TransferWeight();
                    await StartGetWeighingQuantity();
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }



        /// <summary>
        /// 获取本货柜下全部物料
        /// </summary>
        public  async void GetAllPageData()
        {
            try
            {
                // 查询货柜下可取出的物料-不包括模具
                //var materialGroup = StockContract.StockDtos.Where(it => it.ContainerCode == ContainerCode&& it.MaterialType != (int)MaterialTypeEnum.Mould)
                //    .GroupBy(it => it.MaterialCode)
                //    .AndBy(it => it.MaterialName)
                //    .Select(it => new { MaterialCode = it.MaterialCode , MaterialName = it.MaterialName }).ToList();


                //_ModuleGroups.Clear();

                //foreach (var item in materialGroup)
                //{
                //    var materialItem = new Material()
                //    {
                //        Code = item.MaterialCode,
                //        Name = item.MaterialName
                //    };
                //    _ModuleGroups.Add(materialItem);
                //}
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// 选择物料-查询批次
        /// </summary>
        public async void SelectMaterial()
        {
            try 
            {
                if (String.IsNullOrEmpty(SelectMaterialCode))
                {
                    return;
                }

                MaterialUrl = _basePath + MaterialContract.Materials.FirstOrDefault(a => a.Code == SelectMaterialCode).PictureUrl;
                OutTaskMaterialLabelEntity = new OutTaskMaterialLabelDto()
                {
                    BillCode = BillCode,
                    BatchCode = SelectBatchCode,
                    MaterialCode = SelectMaterialCode,
                    MaterialLabel = SearchBarcode,
                    OutTaskMaterialQuantity = OutQuantity,
                    ContainerCode = ContainerCode,
                    
                };
                var outtaskService = ServiceProvider.Instance.Get<IOutTaskService>();
                var outTask = outtaskService.PostClientStockList(OutTaskMaterialLabelEntity);

                if (outTask.Result.Success)
                {
                    AviStockList = JsonHelper.DeserializeObject<List<StockDto>>(outTask.Result.Data.ToString());

                    if (AviStockList.Count == 0)
                    {
                        Msg.Warning("您暂无操作该物料权限或无可出库库存");
                        isAlarmRaised = true;
                        return;
                    }

                    var trayList = AviStockList.GroupBy(a => a.BatchCode).OrderBy(a => a.Key).ToArray();
                    _BatchCodeGroups.Clear();

                    foreach (var item in trayList)
                    {
                        var stockBatch = new Stock()
                        {
                            BatchCode = item.Key
                        };
                        _BatchCodeGroups.Add(stockBatch);
                    }

                    if (AviStockList.Count>0)
                    {
                        SelectBatchCode = _BatchCodeGroups[0].BatchCode;
                    }
                }
                else
                {
                    // 未授权则登录
                    if (outTask.Result.Message == "Unauthorized")
                    {
                        var dialog = ServiceProvider.Instance.Get<IShowContent>();
                        dialog.BindDataContext(new UserLoginWindow(), new UserLoginModel());
                        // 系统登录
                        await dialog.Show();
                    }
                    else
                    {
                        Msg.Error("获取可存放托盘失败：" + outTask.Result.Message);
                        isAlarmRaised = true;
                    }
                }

                if (AutoOprate)
                {
                    HandShelf();
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// 根据批次进行筛选可出库的库存及所在托盘
        /// </summary>
        public async void SelectBatch()
        {
            try
            {
                // 查询该物料可存放的储位
                var trayList = AviStockList.Where(a => a.BatchCode==SelectBatchCode).OrderBy(a => a.TrayCode).ToArray();
                var trayCodeList = trayList.GroupBy(a => a.TrayCode).ToList();

                _TaryGroups.Clear();
                foreach (var item in trayCodeList)
                {
                    var LocationVIEW = new LocationVIEW()
                    {
                        TrayCode = item.Key
                    };
                    _TaryGroups.Add(LocationVIEW);
                }

                if (trayCodeList.Count>0)
                {
                    SelectTrayCode = trayList[0].TrayCode;
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
                var locationGroup = AviStockList.Where(a => a.TrayCode == SelectTrayCode&& a.BatchCode == SelectBatchCode).ToList();
     
                var locationCodeGroup = locationGroup.GroupBy(a => new {  a.TrayId,  a.LocationCode }).ToList();

                _LocationGroups.Clear();

                foreach (var item in locationCodeGroup)
                {
                    TrayId = (int)item.Key.TrayId;
                    var LocationVIEW = new LocationVIEW()
                    {
                        Code = item.Key.LocationCode
                    };
                    _LocationGroups.Add(LocationVIEW);
                }

                if (locationCodeGroup.Count>0)
                {
                    SelectLocationCode = _LocationGroups[0].Code;
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// 选择储位
        /// </summary>
        public async void SelectLocation()
        {
            try
            {
                // 查询该储位最多可取出的数量
                var stockList = AviStockList.Where(a => a.LocationCode == SelectLocationCode).ToList();
                if (stockList.Count<=0)
                {
                    return;
                }
                var location = WareHouseContract.LocationVMs.FirstOrDefault(a => a.Code == SelectLocationCode);
                BoxUrl = _basePath + location.BoxUrl;
                BoxName = location.BoxName;
                AviQuantity = stockList.Sum(a=>a.Quantity-a.LockedQuantity);
                AllQuantity = AviStockList.Sum(a => a.Quantity - a.LockedQuantity);
                XLight = location.XLight;
                YLight = location.YLight;
                XLightLenght = location.XLenght.GetValueOrDefault(0);
                AviCount = AviStockList.Count;
                if (SelectTrayCode == CurTratCode)
                {
                    RunningContainer();
                }

                if (AviCount>1)
                {
                    IsMoreThanTwo = "Visible";
                }

            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// ComboBox 下拉数据初始化
        /// </summary>
        public async void SelectInit()
        {
            try
            {
                // 查询系统中的供应商
                var list = SupplyContract.Supplys.Where(a => a.Code.Contains(SelectSupplerCode) || a.Name.Contains(SelectSupplerCode)).Take(20).ToList();
                
                _SupplyGroups.Clear();
                foreach (var item in list)
                {
                    _SupplyGroups.Add(item);
                }


                // 查询系统中的出库业务类别
                var inTypelist = DictionaryContract.Dictionaries.Where(a => a.TypeCode == "OutType" && a.Enabled).ToList();

                _InTypeGroups.Clear();
                foreach (var item in inTypelist)
                {
                    _InTypeGroups.Add(item);
                }

            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// 直接取出托盘号
        /// </summary>
        private string _TakeOutTrayNumber = "";
        public string TakeOutTrayNumber
        {
            get { return _TakeOutTrayNumber; }
            set { _TakeOutTrayNumber = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 直接存入托盘号
        /// </summary>
        private string _TakeInTrayNumber = "";
        public string TakeInTrayNumber
        {
            get { return _TakeInTrayNumber; }
            set { _TakeInTrayNumber = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 直接取出托盘
        /// </summary>
        public async void StraightOutTray()
        {
            try
            {
                ChangeColor("StraightInTrayCheck");

                string CurrentRunningTray = this.TakeOutTrayNumber;

                if (!int.TryParse(this.TakeOutTrayNumber, out int trayNumber))
                {
                    Msg.Error("输入的托盘号格式不正确");
                    return;
                }

                if (GlobalData.DeviceStatus == (int)DeviceStatusEnum.Fault)
                {
                    GlobalData.IsFocus = true;
                    Msg.Warning("设备离线状态，无法启动货柜！");
                    return;
                }


                if (CurrentRunningTray != null)
                {
                    GlobalData.IsFocus = true;

                    if (!await Msg.Question("是否驱动目标托盘到取料口？"))
                    {
                        return;
                    }
                }

                var trayEntity = WareHouseContract.Trays.Where(a => a.Code == TakeOutTrayNumber).FirstOrDefault();

                var TrayId = trayEntity?.Id;

                // 核查用户是否有此模块操作权限
                var user = ServiceProvider.Instance.Get<IUserService>();
                var authCheck = user.GetCheckTrayAuth((int)TrayId);
                if (!authCheck.Result.Success)
                {
                    Msg.Warning("抱歉，您无操作该托盘权限！");
                    return;
                }

                // await dialog.c
                // 读取PLC 状态信息
                var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

                var runingEntity = new RunningContainer()
                {
                    ContainerCode = ContainerCode,
                    TrayCode = trayNumber,
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
                    TakeInTrayNumber = runingEntity.TrayCode.ToString();
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
        /// 启动货柜
        /// </summary>
        public async void RunningContainer()
        {
            try
            {
                ChangeColor("FirstStep");
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
                    Msg.Warning("该物料没有维护存放位置，请先维护位置！");

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
                //else
                //{
                //    if (XLight != 0)
                //    {
                //        Msg.Warning("储位在同层！");
                //    }
                //}

                // 核查用户是否有此模块操作权限
                var user = ServiceProvider.Instance.Get<IUserService>();
                var authCheck = user.GetCheckTrayAuth((int)TrayId);
                if (!authCheck.Result.Success)
                {
                    Msg.Warning("抱歉，您无操作该托盘权限！");
                    return;
                }
                // 判断是否启用单包条码管理
                if (OutQuantity > AviQuantity)
                {
                    GlobalData.IsFocus = true;
                    Msg.Warning("超出该容器可存放的最多数量！");
                    return;
                }

                // await dialog.c
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
                    if (CurTratCode != SelectTrayCode)
                    {
                        Msg.Info("正在取出托盘,请确认托盘到达指定位置后再关闭窗口");
                    }
                    CurTratCode = SelectTrayCode;
                    orgXLight = XLight;
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
        /// 直接存入托盘
        /// </summary>
        public async void StraightInTray()
        {
            try
            {
                ChangeColor("StraightOutTrayCheck");

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
                    var runningContainer = baseControlService.PostStartRestoreContainer(runingEntity).Result;
                    if (runningContainer.Success)
                    {
                        Msg.Info("正在存入托盘,请确认托盘已存入货柜后再关闭窗口");
                    }
                    else
                    {
                        Msg.Error(runningContainer.Message);
                    }

                    GlobalData.IsFocus = true;
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
                ChangeColor("FourthStep");

                //ContainerSettingViewModel containerSettingViewModel = new ContainerSettingViewModel();

                //string TakeInTrayNumber = wms.Client.ViewModel.ContainerSettingViewModel.TakeInTrayNumber;
                string cfgINIes = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
                if (File.Exists(cfgINIes))
                {
                    IniFile ini = new IniFile(cfgINIes);
                    TakeInTrayNumberes = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                }
                if (CurTratCode != SelectTrayCode && TakeInTrayNumberes != SelectTrayCode)
                {
                    Msg.Warning("当前存入托盘号非已取出托盘，请核验后存入！");
                    return;
                }

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
                if (OutQuantity == 0)
                {
                    Msg.Warning("请输入出库数量");
                    return;
                }
                // 判断是否启用单包条码管理
                if (OutQuantity > AviQuantity)
                {
                    GlobalData.IsFocus = true;
                    Msg.Warning("超出该容器最多可取出数量！");
                    return;
                }
                if (CurTratCode != SelectTrayCode)
                {
                    Msg.Warning("当前货柜未运行至此托盘，请启动货柜！");
                    return;
                }
                if (orgXLight != XLight)
                {
                    Msg.Warning("当前货柜未运行至此储位，请启动货柜！");
                    return;
                }
                if (OutTaskMaterial.Any(a => a.LocationCode == SelectLocationCode))
                {
                    Msg.Warning("该储位在手动出库明细中，请重新选择存放储位或删除该明细重新出库！");
                    return;
                }

                GlobalData.IsFocus = false;

                // 判断是否启用单包条码管理
                var materialEntity = MaterialContract.Materials.FirstOrDefault(a => a.Code ==SelectMaterialCode);


                // 判断是否启用单包条码管理
                var labelEnity = LabelContract.LabelDtos.Where(a => a.Code == SearchBarcode).FirstOrDefault();

                // 判断是否启用条码
                if (materialEntity.IsPackage)
                {
                    // 判断条码是否为空，以及是否为物料条码
                    if (String.IsNullOrEmpty(SearchBarcode))
                    {
                        OutQuantity = 0;
                        Msg.Warning("该物料已经启用条码管理，请扫描出库条码");
                        return;
                    }
                    if (labelEnity == null)
                    {
                        OutQuantity = 0;
                        Msg.Warning("该物料已经启用条码管理，请扫描出库条码");
                        return;
                    }
                    else
                    {
                        OutTaskMaterialLabelEntity.MaterialLabel = SearchBarcode;
                    }
                } // 如果未启用单包
                else
                {
                    // 从库存中获取
                    var outLabel = StockContract.Stocks
                        .FirstOrDefault(a => a.LocationCode == SelectLocationCode
                                             && a.MaterialCode == SelectMaterialCode && a.BatchCode == SelectBatchCode);
                    OutTaskMaterialLabelEntity.MaterialLabel = outLabel.MaterialLabel;
                }

                this.IsCancel = false;
                OutTaskMaterialLabelEntity.PickedTime = DateTime.Now;
                OutTaskMaterialLabelEntity.LocationCode = SelectLocationCode;
                OutTaskMaterialLabelEntity.TrayCode = SelectTrayCode;
                OutTaskMaterialLabelEntity.MaterialName = materialEntity.Name;
                OutTaskMaterialLabelEntity.Quantity = OutQuantity;
                OutTaskMaterialLabelEntity.RealPickedQuantity= OutQuantity;
                OutTaskMaterialLabelEntity.BatchCode = SelectBatchCode;
                OutTaskMaterialLabelEntity.OutTaskMaterialQuantity = OutQuantity;
                OutTaskMaterialLabelEntity.ContainerCode = ContainerCode;
                OutTaskMaterialLabelEntity.SupplierCode = SelectSupplerCode;
                OutTaskMaterialLabelEntity.OutDict = InType;
                // 新增出库任务明细
                OutTaskMaterial.Add(OutTaskMaterialLabelEntity);
                GlobalData.IsFocus = true;

                // 灭灯
                XLight = 0;
                YLight = 0;
                XLightLenght = 1;
                // RunningContainer();
               //  OffXLight();

                Clear();
            }
            catch (Exception ex)
            {
                Msg.Error("添加手动出库物料失败！");
            }
            finally
            {
                this.IsCancel = true;
            }
        }
        public async void OffXLight()
        {
            if (GlobalData.DeviceStatus == (int)DeviceStatusEnum.Fault)
            {
                GlobalData.IsFocus = true;
                Msg.Warning("设备离线状态，无法启动货柜！");
                return;
            }

            GlobalData.IsFocus = false;
            // await dialog.c
            // 读取PLC 状态信息
            var baseControlService = ServiceProvider.Instance.Get<IBaseControlService>();

            var runingEntity = new RunningContainer()
            {
                ContainerCode = ContainerCode,
                TrayCode = Convert.ToInt32(SelectTrayCode),
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
                CurTratCode = SelectTrayCode;
                orgXLight = XLight;
            }
            else
            {
                GlobalData.IsFocus = true;
                Msg.Error(runningContainer.Result.Message);
            }
        }

        /// <summary>
        /// 删除入库项目
        /// </summary>
        /// <param name="entity"></param>
        public async void DelectOutTaskItem(Bussiness.Dtos.OutTaskMaterialLabelDto entity)
        {
            try
            {
                if (entity != null)
                {
                    if (await Msg.Question("是否删除本入库明细，请确保已将物料从货柜中取出!"))
                    {
                        OutTaskMaterial.Remove(entity);
                    }
                }

            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }



        /// <summary>
        /// 完成提交
        /// </summary>
        public void Submit()
        {
            try
            {
                ChangeColor("ThirdStep");
                if (OutTaskMaterial.Count==0)
                {
                    Msg.Warning("未执行出库操作,无法提交");
                    return;
                }

                var outMaterial = JsonHelper.SerializeObject(OutTaskMaterial);

                var outtaskService = ServiceProvider.Instance.Get<IOutTaskService>();

                var outTask = new OutTaskDto()
                {
                    OutTaskMaterialList= outMaterial,
                    Remark = Remark
                };
                var outTaskPost = outtaskService.PostManualOutList(outTask);

                // 仍有未完成的任务
                if (outTaskPost.Result.Success)
                {
                    Clear();
                    OutTaskMaterial.Clear();
;                   Msg.Info("手动出库执行成功！");
                    // 刷新界面
                    GlobalData.LoginModule = "OutTask";
                    GlobalData.LoginPageCode = "ManualOutDlg";
                    GlobalData.LoginPageName = "手动出库";
                    var obj = new MainViewModel();
                    if (obj == null) return;
                    // obj.ClosePage(GlobalData.LoginPageName);
                    obj.UpdatePage(GlobalData.LoginPageName, GlobalData.LoginPageCode);
                    return;
                }
                else // 直接跳转至任务界面
                {
                    Msg.Warning("手动出库执行失败:" + outTaskPost.Result.Message);
                }
            }
            catch (Exception ex)
            {
                Msg.Question("物料出库失败：");
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
                // 清空
                LabelEntity.MaterialName = "";
                LabelEntity.MaterialCode = "";
                LabelEntity.BatchCode = "";
                LabelEntity.Quantity = 0;
                LabelEntity.SupplyName = "";
                SelectMaterialName = "";
                MaterialUrl = "";
                OutQuantity = 1;
                XLight =0;
                YLight = 0;
                SelectBatchCode = "";
                SelectMaterialCode = "";
                SelectTrayCode = "";
                BoxUrl = "";
                BoxName = "";
                AviQuantity = 0;
                SelectSupplerCode = "";
                SearchBarcode = "";
                LabelEntity.MaterialUrl = "";
                AviCount = 0;
                AllQuantity = 0;
                GlobalData.IsFocus = true;
                IsMoreThanTwo = "Hidden";
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
                TakeInTrayNumber = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
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
