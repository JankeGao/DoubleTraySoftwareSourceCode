using System;
using wms.Client.LogicCore.UserAttribute;

namespace wms.Client.LogicCore.Enums
{
    /// <summary>
    /// 模块类型
    /// </summary>
    public enum ModuleType
    {
        [Description("手动管理", "ArrowDownCircle")]
        InManage,

        [Description("任务管理", "ArrowUpCircle")]
        OutManage,

        [Description("模具管理", "Bandage")]
        ModuleManage,

        [Description("货柜管理", "HomeAnalytics")]
        WarehouseManagement,

        [Description("设备管理", "Nas")]
        EquipmentManagement,

        //[Description("插件管理", "\xe63b")]
        //PluginManagement,

        //[Description("参数配置", "\xe6ee")]
        //ParameterConfiguration,

        //[Description("调试软件", "\xe629")]
        //DebuggingSoftware,

        //[Description("演示平台", "\xe667")]
        //DemoPlatform,

        //[Description("演示软件", "\xe6a0")]
        //DemoSoftware,

        //[Description("标签功能", "\xe75c")]
       // LabelFunction,

        //[Description("基础数据", "Nature")]
        //BasicData,

        //[Description("公共数据", "\xe610")]
        //PublicData,

        //[Description("文档中心", "\xe64d")]
        //DocumentCenter,

        [Description("系统设置", "Cog")]
        SystemSettings,
    }

    /// <summary>
    /// 模块特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// 模块构造函数
        /// </summary>
        /// <param name="code">模块编码</param>
        /// <param name="name">模块名称</param>
        /// <param name="icon">ICON</param>
        public ModuleAttribute(ModuleType moduleType, string code, string name, string icon = "")
        {
            this.moduleType = moduleType;
            this.code = code;
            this.name = name;
            this.icon = icon;
        }

        #region private

        private ModuleType moduleType;
        private string code;
        private string name;
        private string icon;

        #endregion

        #region 只读属性

        public string Code { get { return code; } }

        /// <summary>
        /// 图标
        /// </summary>
        public string ICON
        {
            get { return icon; }
        }

        /// <summary>
        /// 菜单名
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        public ModuleType ModuleType
        {
            get { return moduleType; }
        }

        public string ModuleTypeName
        {
            get { return GetEnumAttrbute.GetDescription(ModuleType).Caption; }
        }


        #endregion
    }

    /// <summary>
    /// 模块类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute
    {
        protected string caption = string.Empty;
        protected string remark = string.Empty;

        public string Caption { get { return caption; } }
        public string Remark { get { return remark; } }
        
        public DescriptionAttribute(string caption, string remark = "BorderAll")
        {
            this.caption = caption;
            this.remark = remark;
        }

    }
}
