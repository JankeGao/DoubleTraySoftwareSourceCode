using HP.Core.Data;

namespace HPC.BaseService.Dtos
{
    public class ModuleAuthOutputDto:IOutputDto
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Area { set; get; }
        /// <summary>
        /// 模块图标
        /// </summary>
        public string Icon { set; get; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 模块地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 模块目标
        /// </summary>
        public string Target { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }
        /// <summary>
        /// 上级模块
        /// </summary>
        public string ParentCode { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { set; get; }
        /// <summary>
        /// 是否公共模块
        /// </summary>
        public string AuthType { set; get; }
        /// <summary>
        /// 模块类型
        /// </summary>
        public string ModuleType { set; get; }
        /// <summary>
        /// 授权类型
        /// </summary>
        public int Type { set; get; }
        /// <summary>
        /// 授权类型ID
        /// </summary>
        public string TypeCode { set; get; }
    }
}
