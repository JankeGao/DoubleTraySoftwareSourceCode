using HP.Core.Data;

namespace HPC.BaseService.Dtos
{
    public class FunctionAuthOutputDto : IOutputDto
    {
        public int Id { set; get; }
        /// <summary>
        /// 模块ID
        /// </summary>
        public string ModuleCode { set; get; }
        /// <summary>
        /// 模块授权类型
        /// </summary>
        public string ModuleAuthType { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { set; get; }
        /// <summary>
        /// 类型ID
        /// </summary>
        public string TypeCode { set; get; }
        /// <summary>
        /// 模块功能编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 模块功能名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 模块功能图标
        /// </summary>
        public string Icon { set; get; }
    }
}
