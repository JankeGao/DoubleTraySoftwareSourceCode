using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HP.Core.Dependency;
using MaterialDesignThemes.Wpf;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Helpers.Files;
using System.Windows.Threading;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Enums;
using wms.Client.LogicCore.Enums;
using wms.Client.Service;
using wms.Client.ViewModel;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 指引
    /// </summary>
    public class GuideModel : ViewModelBase
    {
        private readonly string _basePath = ConfigurationManager.AppSettings["ServerIP"];
        /// <summary>
        /// 入库任务契约
        /// </summary>
        private readonly IInTaskContract InTaskContract;

        /// <summary>
        /// 出库任务契约
        /// </summary>
        private readonly IOutTaskContract OutTaskContract;

        /// <summary>
        /// 领用任务契约
        /// </summary>
        private readonly IReceiveTaskContract ReceiveTaskContract;


        /// <summary>
        /// 盘点任务契约
        /// </summary>
        private readonly ICheckContract CheckContract;

        /// <summary>
        /// 仓库契约
        /// </summary>
        private readonly IWareHouseContract WareHouseContract;

        public GuideModel()
        {
            InTaskContract = IocResolver.Resolve<IInTaskContract>();
            OutTaskContract = IocResolver.Resolve<IOutTaskContract>();
            ReceiveTaskContract = IocResolver.Resolve<IReceiveTaskContract>();
            CheckContract = IocResolver.Resolve<ICheckContract>();
            WareHouseContract = IocResolver.Resolve<IWareHouseContract>();
            GuideCode = GlobalData.GuideCode;
            this.GetAllPageData();
            this.ReadConfigInfo();
        }

        #region 任务组

        private ObservableCollection<object> _ModuleGroups = new ObservableCollection<object>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<object> ModuleGroups
        {
            get { return _ModuleGroups; }
            set { _ModuleGroups = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<InTaskMaterialDto> _InTaskMaterial = new ObservableCollection<InTaskMaterialDto>();

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


        private RelayCommand _confirmCommand;

        public RelayCommand ConfirmCommand
        {
            get
            {
                if (_confirmCommand == null)
                {
                    _confirmCommand = new RelayCommand(() => Confirm());
                }
                return _confirmCommand;
            }
        }

        /// <summary>
        /// 客户端货柜编码
        /// </summary>
        private string ContainerCode = string.Empty;


        /// <summary>
        /// 用户名
        /// </summary>
        private string guideCode = string.Empty;
        public string GuideCode
        {
            get { return guideCode; }
            set { guideCode = value; RaisePropertyChanged(); }
        }

        #endregion


        /// <summary>
        /// 获取全部货柜任务
        /// </summary>
        public async void GetAllPageData()
        {
            try
            {
                switch (GlobalData.GuideType)
                {
                    case (int)ModuleType.InManage: // 入库指引
                        InGuide();
                        break;
                    case (int)ModuleType.OutManage:// 出库指引
                        OutGuide();
                        break;
                    case (int)ModuleType.ModuleManage:// 模具指引
                        ReceiveGuide();
                        break;
                    case (int)ModuleType.WarehouseManagement:// 盘点指引
                        InGuide();
                        break;
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        public async void InGuide()
        {
            var query = InTaskContract.InTaskDtos.Where(a =>
                a.Status != (int)InTaskStatusCaption.Finished);

            if (!String.IsNullOrEmpty(GlobalData.GuideCode))
            {
                query = query.Where(p => p.InCode.Contains(GlobalData.GuideCode) || p.BillCode.Contains(GlobalData.GuideCode));
            }

            // 查询全部任务
            var groupList = query.GroupBy(a => a.ContainerCode).Select(a => a.ContainerCode).ToList();
            _ModuleGroups.Clear();
            // 根据货柜分组
            //  var groupList = inTaskList.GroupBy(a => new { a.ContainerCode });

            foreach (var item in groupList)
            {
                var container = WareHouseContract.ContainerDtos.FirstOrDefault(a => a.Code == item);

                var inTaskGroup = new InTaskGroup()
                {
                    GroupIcon = container.BrandDescription,
                    GroupName = "货柜号：" + container.Code,
                    GroupWarehouse = _basePath + container.PictureUrl
                };
                // 查询全部任务
                var inTaskContainerList = InTaskContract.InTaskDtos.Where(a => a.Status != (int)InTaskStatusCaption.Finished && a.ContainerCode == item && a.InCode == GlobalData.GuideCode).ToList();

                foreach (var intask in inTaskContainerList)
                {
                    var inTaskItem = new InTaskItem()
                    {
                        Code = intask.Code,
                        Name = intask.StatusCaption,
                        InCode = intask.InCode,
                        ContainerCode = container.Code
                    };
                    inTaskGroup.Modules.Add(inTaskItem);
                }

                _ModuleGroups.Add(inTaskGroup);
            }
        }

        public async void OutGuide()
        {
            var query = OutTaskContract.OutTaskDtos.Where(a =>
                a.Status != (int)OutTaskStatusCaption.Finished);

            if (!String.IsNullOrEmpty(GlobalData.GuideCode))
            {
                query = query.Where(p => p.OutCode.Contains(GlobalData.GuideCode) || p.BillCode.Contains(GlobalData.GuideCode));
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
                var OutTaskContainerList = OutTaskContract.OutTaskDtos.Where(a => a.Status != (int)OutTaskStatusCaption.Finished && a.ContainerCode == item && a.OutCode == GlobalData.GuideCode).ToList();

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

        public async void ReceiveGuide()
        {
            var query = ReceiveTaskContract.ReceiveTaskDtos.Where(a =>
                a.Status != (int)ReceiveEnum.Finish);

            if (!String.IsNullOrEmpty(GlobalData.GuideCode))
            {
                query = query.Where(p => p.Code.Contains(GlobalData.GuideCode));
            }

            // 查询全部任务
            var groupList = query.GroupBy(a => a.ContainerCode).Select(a => a.ContainerCode).ToList();
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
                var ReceiveTaskContainerList = ReceiveTaskContract.ReceiveTaskDtos.Where(a => a.Status != (int)ReceiveEnum.Finish && a.ContainerCode == item && a.ReceiveCode == GlobalData.GuideCode).ToList();

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

        /// <summary>
        /// 获取本机货柜任务
        /// </summary>
        public async void GetMyPageData()
        {
            try
            {
                var container = WareHouseContract.ContainerDtos.FirstOrDefault(a => a.Code == ContainerCode);
                _ModuleGroups.Clear();
                var inTaskGroup = new InTaskGroup()
                {
                    GroupIcon = container.BrandDescription,
                    GroupName = "货柜号：" + container.Code,
                    GroupWarehouse = _basePath + container.PictureUrl
                };
                var query = InTaskContract.InTaskDtos.Where(a =>
                    a.Status != (int)InTaskStatusCaption.Finished && a.ContainerCode == ContainerCode);

                // 根据入库单或订单号查询
                if (!String.IsNullOrEmpty(GlobalData.GuideCode))
                {
                    query = query.Where(p => p.InCode.Contains(GlobalData.GuideCode) || p.BillCode.Contains(GlobalData.GuideCode));
                }

                // 查询全部任务
                var inTaskContainerList = query.ToList();

                foreach (var intask in inTaskContainerList)
                {
                    var inTaskItem = new InTaskItem()
                    {
                        Code = intask.Code,
                        Name = intask.StatusCaption,
                        InCode = intask.InCode,
                        ContainerCode = container.Code
                    };
                    inTaskGroup.Modules.Add(inTaskItem);
                }

                _ModuleGroups.Add(inTaskGroup);
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        /// <summary>
        /// 确认指引
        /// </summary>
        public async void Confirm()
        {
            // 刷新界面
            var obj = new MainViewModel();
            if (obj == null) return;
            obj.ClosePage(GlobalData.LoginPageName);
            DialogHost.CloseDialogCommand.Execute(null, null);
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
