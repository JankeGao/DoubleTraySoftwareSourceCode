namespace HPC.BaseService.Dtos
{
    public class FunctionAuthInputDto
    {
        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleId { set; get; }
        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleId { set; get; }
        /// <summary>
        /// 功能编码
        /// </summary>
        public string FunctionCode { set; get; }
    }
}
