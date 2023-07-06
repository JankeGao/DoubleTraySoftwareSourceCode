using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HP.Core.Dependency;
using HP.Core.Mapping;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Models;
using SqlSugar;
using wms.Client.Common;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Enums;
using wms.Client.LogicCore.Helpers;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.LogicCore.Interface;
using wms.Client.LogicCore.UserAttribute;
using wms.Client.Service;
using wms.Client.View;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 手动入库
    /// </summary>
   // [Module(ModuleType.InManage, "TempInDlg", "离线入库")]
    public class TempInViewModel : Base.DataProcess<InTask>
    {

        ///// <summary>
        ///// 入库任务契约
        ///// </summary>
        //private readonly IInTaskContract InTaskContract;

        ///// <summary>
        ///// 仓库契约
        ///// </summary>
        //private readonly IWareHouseContract WareHouseContract;


        ///// <summary>
        ///// 物料契约
        ///// </summary>
        //private readonly IMaterialContract MaterialContract;

        ///// <summary>
        ///// 条码契约
        ///// </summary>
        //private readonly ILabelContract LabelContract;

        ///// <summary>
        ///// 条码契约
        ///// </summary>
        //private readonly IMapper Mapper;

        // 手动执行入库，调用本地数据库
        public static string connStr = System.Configuration.ConfigurationSettings.AppSettings["connectionString"].ToString();
        //public static string connStr = ConfigurationManager.ConnectionStrings["Default"].ToString();

        SqlSugarClient db = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = connStr,
                DbType = DbType.SqlServer,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });

        public TempInViewModel()
        {
            //InTaskContract = IocResolver.Resolve<IInTaskContract>();
            //LabelContract = IocResolver.Resolve<ILabelContract>();
            //WareHouseContract = IocResolver.Resolve<IWareHouseContract>();
            //Mapper = IocResolver.Resolve<IMapper>();
            //MaterialContract = IocResolver.Resolve<IMaterialContract>();
            //UserLoginCommand = new RelayCommand<string>(ShowLogin);
            ScanBarcodeCommand= new RelayCommand<string>(ScanBarcode);
            SelectItemCommand= new RelayCommand<Material>(SelectInTaskItem);
            HandShelfCommand= new RelayCommand (HandShelf);
            SubmitCommand = new RelayCommand(Submit);
            this.GetAllPageData();
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

        private ObservableCollection<InTaskMaterialDto> _InTaskMaterial= new ObservableCollection<InTaskMaterialDto>();

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

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<InTaskMaterialDto> InTaskMaterial
        {
            get { return _InTaskMaterial; }
            set { _InTaskMaterial = value; RaisePropertyChanged(); }
        }


        #endregion

        #region 命令(Binding Command)

        /// <summary>
        /// 选择本次入库的物料
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
        /// 选择本次入库的物料
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

        private RelayCommand _allCommand;

        public RelayCommand cmb1_SelectionChanged
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
        public RelayCommand<Material> SelectItemCommand { get; private set; }


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
        /// 客户端货柜编码
        /// </summary>
        private string ContainerCode = string.Empty;


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
        /// 当前操作储位
        /// </summary>
        private decimal inQuantity = 0;
        public decimal InQuantity
        {
            get { return inQuantity; }
            set { inQuantity = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 搜索条码
        /// </summary>
        private string batchCode = string.Empty;
        public string BatchCode
        {
            get { return batchCode; }
            set { batchCode = value; RaisePropertyChanged(); }
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
        /// 当前操作的任务明细
        /// </summary>
        private InTaskMaterialDto inTaskMaterialEntity = new InTaskMaterialDto();
        public InTaskMaterialDto InTaskMaterialEntity
        {
            get { return inTaskMaterialEntity; }
            set { inTaskMaterialEntity = value; RaisePropertyChanged(); }
        }

        #endregion


        /// <summary>
        /// 核验登录人员
        /// </summary>
        /// <param name="code"></param>
        public async void ShowLogin()
        {
            // 重新读取登录信息
            this.ReadConfigInfo();

            // 系统用户注销时间
            var checkTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["LoginOutTime"].ToString());

            var dialog = ServiceProvider.Instance.Get<IShowContent>();
            dialog.BindDataContext(new UserLoginWindow(), new UserLoginModel());
            // 如果未登录
            if (string.IsNullOrWhiteSpace(loginTime))
            {
                // 系统登录
                dialog.Show();
            }
            else
            {
                var login = Convert.ToDateTime(loginTime).AddMinutes(checkTime);
                var now = DateTime.Now;

                // 如果时间已过期
                if (DateTime.Compare(now, login) > 0)
                {
                    // 系统登录
                    dialog.Show();
                }
                else
                {
                    this.GetAllPageData();
                }
            }
        }


        /// <summary>
        /// 扫描入库条码
        /// </summary>
        public async void ScanBarcode(string code)
        {
            //var labelEnity=LabelContract.LabelDtos.Where(a => a.Code == code).FirstOrDefault();
            //if (labelEnity==null)
            //{
            //    Msg.Warning("未获取到条码信息");
            //    return;
            //}

            //LabelEntity.LabelCode = labelEnity.Code;
            //LabelEntity.MaterialName = labelEnity.MaterialName;
            //LabelEntity.Quantity = labelEnity.Quantity;
            //LabelEntity.SupplyName = labelEnity.SupplyName;
            //LabelEntity.BatchCode = labelEnity.BatchCode;

            // 本次入库数量
            //InQuantity = labelEnity.Quantity;
        }


        /// <summary>
        /// 选择任务行项目
        /// </summary>
        public async void SelectInTaskItem(Material entity)
        {
            // 验证是否有操作权限


            if (entity == null)
            {
                Msg.Warning("未获取到选中信息");
                return;
            }

            //if (entity.Status== (int)InTaskStatusCaption.Finished)
            //{
            //    Msg.Warning("该物料已完成");
            //    return;
            //}

            //TrayCode = entity.SuggestTrayCode;
            //LocationCode = entity.SuggestLocation;
            //InTaskMaterialEntity = entity;
        }

        

        /// <summary>
        /// 获取本货柜下全部物料
        /// </summary>
        public  async void GetAllPageData()
        {
            try
            {
                // 手动执行入库，调用本地数据库
                //string connStr = System.Configuration.ConfigurationSettings.AppSettings["connectionString"].ToString();
                //SqlSugarClient db = new SqlSugarClient(
                //    new ConnectionConfig()
                //    {
                //        ConnectionString = connStr,
                //        DbType = DbType.SqlServer,//设置数据库类型
                //        IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                //        InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
                //    });


                // 查询货柜下可存放的物料
                var materialGroup = db.Queryable<LocationVIEW>().Where(it => it.ContainerCode == ContainerCode)
                    .GroupBy(it => it.MaterialCode)
                    .GroupBy(it => it.MaterialName)
                    .Select(it => new { MaterialCode = it.MaterialCode , MaterialName = it.MaterialName }).ToList();


                _ModuleGroups.Clear();

                foreach (var item in materialGroup)
                {
                    var materialItem = new Material()
                    {
                        Code = item.MaterialCode,
                        Name = item.MaterialName
                    };
                    _ModuleGroups.Add(materialItem);
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// 选择物料
        /// </summary>
        public async void SelectMaterial()
        {
            try
            {
                // 查询该物料可存放的托盘-根据人员权限筛选
                //var trayGroup = db.Queryable<LocationVIEW, TrayUserMap>((st, sc) => new object[] {
                //        JoinType.Left,st.TrayId==sc.TrayId})
                //    .Where((st, sc) => st.ContainerCode == ContainerCode && st.SuggestMaterialCode == SelectMaterialCode&& sc.UserCode== UserCode)
                //    .GroupBy((st, sc) => st.TrayCode)
                //    .Select((st, sc) => new { TrayCode = st.TrayCode }).ToList();

                // 获取物料实体
                var mateialEntity = db.Queryable<Material>().Where(a => a.Code == SelectMaterialCode).First();

                /* 储位分配逻辑
                1、查找该物料可存放的载具,确定全部可存放储位
                2、查看该物料是存储锁定
                3、查看是否混批
                */
                // 可存放的载具列表，库位绑定载具，载具绑定物料，库位码关联库存表
                // 查询可存放库位，本身存放该物料，或者不存放该物料，但是不是存储锁定的

                var query = db.Queryable<LocationVIEW, TrayUserMap>((st, sc) => new object[]
                    {
                        JoinType.Inner, st.TrayId == sc.TrayId
                    })
                    .Where((st, sc) =>
                        st.ContainerCode == ContainerCode  && sc.UserCode == UserCode);


                query = query.Where((st, sc) => st.MaterialCode == SelectMaterialCode || !st.IsNeedBlock);

                // 入库是存储锁定，则必须存放在所绑定的载具中,及该载具维护的物料即为待入库物料
                if (mateialEntity.IsNeedBlock)
                {
                    query = query.Where((st, sc) => st.SuggestMaterialCode == SelectMaterialCode);
                }

                //如果不允许混批
                if (!mateialEntity.IsMaxBatch)
                {
                    // 批次相等，或者库存中没有存放物料
                    query = query.Where((st, sc) => st.BatchCode == BatchCode || st.MaterialLabel == null);
                }

                // 根据载具可存放的数量进行储位筛选，分配数量，明确哪个储位存放多少数量，以重量做限制
                var trayList = query.GroupBy((st, sc) => st.TrayCode).Select(it => new { TrayCode = it.TrayCode }).ToList();


                _TaryGroups.Clear();

                foreach (var item in trayList)
                {
                    var LocationVIEW = new LocationVIEW()
                    {
                        TrayCode= item.TrayCode
                    };
                    _TaryGroups.Add(LocationVIEW);
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
                var locationGroup = db.Queryable<LocationVIEW>().Where(it => it.ContainerCode == ContainerCode && it.SuggestMaterialCode == SelectMaterialCode && it.TrayCode == SelectTrayCode)
                    .GroupBy(it => it.Code)
                    .Select(it => new { Code = it.Code }).ToList();

                _LocationGroups.Clear();

                foreach (var item in locationGroup)
                {
                    var LocationVIEW = new LocationVIEW()
                    {
                        Code = item.Code
                    };
                    _LocationGroups.Add(LocationVIEW);
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
                // 查询该储位最多可存放的物料数量


            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }



        /// <summary>
        /// 获取物料条码可存放的储位--客户端
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CheckClientLocation()
        {
            var inLocationList = new List<LocationVM>();

            // 获取物料实体
            var mateialEntity = db.Queryable<Material>().Where(a => a.Code == SelectMaterialCode).First();

            //验证该物料是否维护了载具信息
            if (!db.Queryable<MaterialBoxMap>().Any(it => it.MaterialCode == SelectMaterialCode))
            {
                Msg.Error(string.Format("物料{0}未维护存放载具，请先维护", SelectMaterialCode));
            }


            /* 储位分配逻辑
            1、查找该物料可存放的载具,确定全部可存放储位
            2、查看该物料是存储锁定
            3、查看是否混批
            4、分配货柜及托盘时，查看是否超重
            */
            // 可存放的载具列表，库位绑定载具，载具绑定物料，库位码关联库存表
            // 查询可存放库位，本身存放该物料，或者不存放该物料，但是不是存储锁定的
            var query = db.Queryable<LocationVIEW>().Where(a => a.ContainerCode == ContainerCode);

            query = query.Where(a =>
                a.MaterialCode == SelectMaterialCode || !a.IsNeedBlock);

            // 入库是存储锁定，则必须存放在所绑定的载具中,及该载具维护的物料即为待入库物料
            if (mateialEntity.IsNeedBlock)
            {
                query = query.Where(a => a.SuggestMaterialCode == SelectMaterialCode);
            }

            //如果不允许混批
            if (!mateialEntity.IsMaxBatch)
            {
                // 批次相等，或者库存中没有存放物料
                query = query.Where(a => a.BatchCode == BatchCode || a.MaterialLabel == null);
            }

            //判断此批物料的重量，进行储位筛选


            // 是否维护单包数量
            decimal? packageWeight = mateialEntity.UnitWeight; // 单个的重量
            decimal packCount = 1; // 单包数量
            if (mateialEntity.IsPackage && mateialEntity.PackageQuantity > 0)
            {
                packageWeight = mateialEntity.UnitWeight * mateialEntity.PackageQuantity; // 单包数量的重量
                packCount = (decimal)mateialEntity.PackageQuantity;
            }

            // 根据载具可存放的数量进行储位筛选，分配数量，明确哪个储位存放多少数量，以重量做限制
            var locationCodeList = query.GroupBy(a => a.Code).Select(a => a.Code).ToList();


            foreach (var code in locationCodeList)
            {
                // 入库仍有数量需要分配
                if (inQuantity > 0)
                {
                    /* 计算储位可存放的数量*/
                    // 储位实体
                    var locationEntity = db.Queryable<LocationVIEW>()
                        .Where(a => a.Code == code && a.MaterialCode == SelectMaterialCode)
                        .First();

                    // 当前储位已存放的数量
                    decimal lockQuantity =
                        db.Queryable<LocationVIEW>().Where(a => a.Code == code).Sum(a => a.Quantity) ==
                        null
                            ? 0
                            : (decimal)db.Queryable<LocationVIEW>().Where(a => a.Code == code)
                                .Sum(a => a.Quantity);

                    // 当前储位可存放的数量
                    var available = (decimal)locationEntity.BoxCount - lockQuantity;


                    // 储位是否可存放所有的数量
                    if (available > inQuantity)
                    {

                      //  inLocationList.Add(avLocation);
                    }
                }
                else
                {
                    break;
                }
            }

            return DataProcess.Success(string.Format("获取可存放库位成功"), inLocationList);
        }



        /// <summary>
        /// 确认存入
        /// </summary>
        public void HandShelf()
        {
        //    try
        //    {
        //        // 判断是否启用单包条码管理
        //        var materialEntity =
        //        MaterialContract.Materials.FirstOrDefault(a => a.Code == InTaskMaterialEntity.MaterialCode);

        //    // 判断是否启用条码
        //    if (materialEntity.IsPackage)
        //    {
        //        // 判断条码是否为空
        //        if (String.IsNullOrEmpty(SearchBarcode))
        //        {
        //            InQuantity = 0;
        //            Msg.Warning("该物料已经启用条码管理，请扫描入库条码");
        //            return;
        //        }
        //    }

        //    this.IsCancel = false;

        //    InTaskMaterialEntity.MaterialLabel = SearchBarcode;
        ////    InTaskMaterialEntity.InTaskMaterialQuantity = (decimal)LabelEntity.Quantity;

        //    // 实际上架储位-后期可维护成可做修改的
        //    InTaskMaterialEntity.LocationCode = SelectLocationCode;

        //    var intaskService = ServiceProvider.Instance.Get<IInTaskService>();

        //    // 物料实体映射

        //    var inTask = intaskService.PostDoHandShelf(InTaskMaterialEntity);

        //    if (inTask.Result.Success)
        //    {
        //        Msg.Info("物料入库成功！");
        //        // 获取当前任务的明细
        //        var inTaskMaterialList = InTaskContract.InTaskMaterialDtos.Where(a => a.InTaskCode == InTaskMaterialEntity.InTaskCode).ToList();
        //        InTaskMaterial.Clear();
        //        inTaskMaterialList.ForEach((arg) => InTaskMaterial.Add(arg));
        //    }
        //    else
        //    {
        //        Msg.Error("物料入库失败：" + inTask.Result.Message);
        //    }

        //    }
        //    catch (Exception ex)
        //    {
        //        Msg.Error("物料入库失败：");
        //    }
        //    finally
        //    {
        //        this.IsCancel = true;
        //    }
        }



        /// <summary>
        /// 完成提交
        /// </summary>
        public void Submit()
        {
            try
            {
                //if (InTaskMaterialEntity.InCode == null)
                //{
                //    Msg.Warning("未执行入库操作");
                //    return;
                //}

                //var inTaskList = InTaskContract.InTaskDtos.Where(a => a.InCode == InTaskMaterialEntity.InCode&& a.Status != (int)InTaskStatusCaption.Finished).ToList();

                //// 仍有未完成的任务
                //if (inTaskList.Count > 0)
                //{
                //    // 全局变量设置需要指引的入库单
                //    GlobalData.InCode = InTaskMaterialEntity.InCode;

                //    var dialog = ServiceProvider.Instance.Get<IShowContent>();
                //    dialog.BindDataContext(new GuideWindow(), new GuideModel());
                //    // 系统登录
                //    dialog.Show();
                //} 
                //else // 直接跳转至任务界面
                //{
                //    TabPageIndex = 0;
                //}
            }
            catch (Exception ex)
            {
                Msg.Question("物料入库失败：");
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
                UserCode = ini.IniReadValue("Login", "UserCode");
                UserName = ini.IniReadValue("Login", "UserName");
                PictureUrl = ini.IniReadValue("Login", "PictureUrl");
                //Name = ini.IniReadValue("Login", "PictureUrl");
                loginTime = ini.IniReadValue("Login", "LoginTime");
                ContainerCode = ini.IniReadValue("ClientInfo", "code");
            }
        }


        /// <summary>
        /// 刷新界面
        /// </summary>
        public void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);
            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {
            }
        }
        private object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }


        public MaterialEntity MaterialItem{ get; set; } = new MaterialEntity();

        public class MaterialEntity : INotifyPropertyChanged
        {
            private string _materialCode;
            public string MaterialCode
            {
                get { return _materialCode; }
                set { _materialCode = value; NotifyPropertyChanged(); }
            }

            private string _batchCode;
            public string BatchCode
            {
                get { return _batchCode; }
                set { _batchCode = value; NotifyPropertyChanged(); }
            }

            private decimal? _quantity;
            public decimal? Quantity
            {
                get { return _quantity; }
                set { _quantity = value; NotifyPropertyChanged(); }
            }

            private string _supplyName;
            public string SupplyName
            {
                get { return _supplyName; }
                set { _supplyName = value; NotifyPropertyChanged(); }
            }

            private string _materialName;
            public string MaterialName
            {
                get { return _materialName; }
                set { _materialName = value; NotifyPropertyChanged(); }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}
