
namespace HPC.BaseService.Dtos
{
    public class AuthInputDto
    {
        /// <summary>
        /// 类别
        /// </summary>
        public int Type { set; get; }
        /// <summary>
        /// 类别编码
        /// </summary>
        public string TypeCode { set; get; }
        /// <summary>
        /// 模块授权
        /// </summary>
        public string ModuleAuthJson { set; get; }
        /// <summary>
        /// 功能授权
        /// </summary>
        public string FunctionAuthJson { set; get; }
        /// <summary>
        /// 数据规则授权
        /// </summary>
        public string DataRuleJson { set; get; }
        /// <summary>
        /// 接口权限
        /// </summary>
        public string WebApiAuthJson { set; get; }
    }
}
