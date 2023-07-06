using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using wms.Client.LogicCore.Enums;

namespace wms.Client.LogicCore.Common
{
    [Serializable]
    /// <summary>
    /// 菜单组数据结构
    /// </summary>
    public class MenuGroupModel : ViewModelBase
    {
        public MenuGroupModel()
        {
            Nodes = new List<MenuGroupModel>();
        }

        private bool _IsChecked;

        /// <summary>
        /// 主键序号
        /// </summary>
        public int ID
        {
            get; set;
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName
        {
            get; set;
        }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public ModuleType MenuType { get; set; }

        /// <summary>
        /// 菜单代码
        /// </summary>
        public string MenuCode { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentId
        {
            get; set;
        }

        /// <summary>
        /// 权限值
        /// </summary>
        public int? AuthValue { get; set; }

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsChecked { get { return _IsChecked; } set { _IsChecked = value; RaisePropertyChanged(); } }

        public List<MenuGroupModel> Nodes { get; set; }
    }
}
