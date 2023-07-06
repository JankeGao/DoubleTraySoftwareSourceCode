using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using wms.Client.LogicCore.Enums;

namespace wms.Client.LogicCore.Common
{
    /// <summary>
    /// 模块组
    /// </summary>
    public class OutTaskGroup : ViewModelBase
    {
        private int groupid;
        private string _groupIcon = "BlockHelper";
        private string _groupName;
        private string _groupWarehouse;
        private ModuleType _moduleType;
        private ObservableCollection<OutTaskItem> modules = new ObservableCollection<OutTaskItem>();

        /// <summary>
        /// 模块ICO
        /// </summary>
        public string GroupIcon
        {
            get { return _groupIcon; }
            set { _groupIcon = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string GroupWarehouse
        {
            get { return _groupWarehouse; }
            set { _groupWarehouse = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 模块类型
        /// </summary>
        public ModuleType ModuleType
        {
            get { return _moduleType; }
            set { _moduleType = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 父模块ID
        /// </summary>
        public int GroupId
        {
            get { return groupid; }
            set { groupid = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 子模块集合
        /// </summary>
        public ObservableCollection<OutTaskItem> Modules
        {
            get { return modules; }
            set { modules = value; RaisePropertyChanged(); }
        }
    }

    /// <summary>
    /// 模块类
    /// </summary>
    public class OutTaskItem
    {


        public string Code { get; set; }

        public string InCode { get; set; }
        /// <summary>
        /// 图标-IconFont
        /// </summary>
        public string ICON { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 货柜品牌
        /// </summary>
        public string Brand { get; set; }


        /// <summary>
        /// 权限值
        /// </summary>
        public int? Authorities { get; set; }


    }
}
