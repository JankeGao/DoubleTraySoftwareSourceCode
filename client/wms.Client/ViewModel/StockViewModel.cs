using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Enums;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.LogicCore.Interface;
using wms.Client.Model.Entity;
using wms.Client.Service;
using wms.Client.View;
using wms.Client.ViewModel.Base;
using wms.Client.ViewModel;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 系统设置
    /// </summary>
    [Module(ModuleType.WarehouseManagement, "StockDlg", "库存管理")]
    public class StockViewModel : DataProcess<Stock>
    {
        private readonly string _basePath = ConfigurationManager.AppSettings["ServerIP"];
        /// <summary>
        /// 入库任务契约
        /// </summary>
        private readonly IStockContract StockContract;

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

        public StockViewModel()
        {
            StockContract = IocResolver.Resolve<IStockContract>();
            LabelContract = IocResolver.Resolve<ILabelContract>();
            WareHouseContract = IocResolver.Resolve<IWareHouseContract>();
            Mapper = IocResolver.Resolve<IMapper>();
            MaterialContract = IocResolver.Resolve<IMaterialContract>();
            ScanBarcodeCommand= new RelayCommand<string>(ScanBarcode);
            SelectItemCommand= new RelayCommand<StockDto>(SelectInTaskItem);
            LoginOutCommand = new RelayCommand<string>(LoginOut);
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

        private ObservableCollection<InTaskGroup> _ModuleGroups = new ObservableCollection<InTaskGroup>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<InTaskGroup> ModuleGroups
        {
            get { return _ModuleGroups; }
            set { _ModuleGroups = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<StockDto> _StockMaterial= new ObservableCollection<StockDto>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<StockDto> StockMaterial
        {
            get { return _StockMaterial; }
            set { _StockMaterial = value; RaisePropertyChanged(); }
        }


        #endregion

        #region 命令(Binding Command)

        private RelayCommand _userLoginCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> UserLoginCommand { get; private set; }


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
        public RelayCommand<StockDto> SelectItemCommand { get; private set; }


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
        /// 退出登录
        /// </summary>
        private RelayCommand _LoginOutCommand;

        public RelayCommand<string> LoginOutCommand { get; private set; }

        public LabelClient LabelEntity { get; set; } = new LabelClient();

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
        private decimal inQuantity = 0;
        public decimal InQuantity
        {
            get { return inQuantity; }
            set { inQuantity = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前操作的任务明细
        /// </summary>
        private StockDto inTaskMaterialEntity = new StockDto();
        public StockDto InTaskMaterialEntity
        {
            get { return inTaskMaterialEntity; }
            set { inTaskMaterialEntity = value; RaisePropertyChanged(); }
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
        public async void ShowLogin()
        {
            GlobalData.LoginModule = "StockInfo";
            GlobalData.LoginPageCode = "StockDlg";
            GlobalData.LoginPageName = "库存管理";
            //如果登录
            if (await CheckLogin())
            {
                // 获取当前任务的明细
                var stockList = StockContract.StockDtos.Where(a => a.ContainerCode == ContainerCode).ToList();
                StockMaterial.Clear();
                stockList.ForEach((arg) => StockMaterial.Add(arg));
                GlobalData.IsFocus = true;
            }
            else    //如果未登录
            {
                var dialog = ServiceProvider.Instance.Get<IShowContent>();
                dialog.BindDataContext(new UserLoginWindow(), new UserLoginModel());
                dialog.Show();
            }
        }


        /// <summary>
        /// 条码
        /// </summary>
        public async void ScanBarcode(string code)
        {
            var labelEnity=StockContract.StockDtos.Where(a => a.MaterialLabel == code).FirstOrDefault();
            if (labelEnity==null)
            {
                Msg.Warning("未获取到条码信息");
                return;
            }

            LabelEntity.LabelCode = labelEnity.MaterialLabel;
            LabelEntity.MaterialName = labelEnity.MaterialName;
            LabelEntity.Quantity = labelEnity.Quantity;
            LabelEntity.BatchCode = labelEnity.BatchCode;

            BoxUrl = _basePath + labelEnity.BoxUrl;
            MaterialUrl = _basePath + labelEnity.PictureUrl;
            BoxName = labelEnity.BoxName;
        }

        /// <summary>
        /// 获取本机货柜任务
        /// </summary>
        public async void GetMyPageData()
        {
            try
            {
                var query = StockContract.StockDtos.Where(a => a.ContainerCode == ContainerCode);
                // 根据入库单或订单号查询
                if (!String.IsNullOrEmpty(SearchText))
                {
                    query = query.Where(p => p.MaterialCode.Contains(SearchText));
                }

                // 查询全部任务
                var stockList = query.ToList();
                StockMaterial.Clear();
                stockList.ForEach((arg) => StockMaterial.Add(arg));
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }
        /// <summary>
        /// 选择任务行项目
        /// </summary>
        public async void SelectInTaskItem(StockDto entity)
        {
            // 验证是否有操作权限
            TrayCode = entity.TrayCode;
            LocationCode = entity.LocationCode;
            InTaskMaterialEntity = entity;

            var stcokEnity = StockContract.StockDtos.Where(a => a.MaterialLabel == entity.MaterialLabel).FirstOrDefault();
            BoxUrl = _basePath + stcokEnity.BoxUrl;
            MaterialUrl = _basePath + stcokEnity.PictureUrl;
            BoxName = stcokEnity.BoxName;
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
                SearchText = "";
                GetMyPageData();
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
