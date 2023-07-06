using System;
using wms.Client.Core.share.Common.Enums;

namespace wms.Client.Core.share.Common.Attributes
{
    /// <summary>
    /// 模块特性, 标记该特性表示属于应用模块的部分
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string name, ModuleType moduleType)
        {
            this.name = name;
            this.moduleType = moduleType;
        }

        private string name;

        /// <summary>
        /// 描述
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        private ModuleType moduleType;

        /// <summary>
        /// 模块类型
        /// </summary>
        public ModuleType ModuleType
        {
            get { return moduleType; }
        }
    }
}
