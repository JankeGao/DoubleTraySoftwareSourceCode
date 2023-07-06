
namespace HPC.BaseService.Dtos
{
    public class UserRolesMapInputDto
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { set; get; }
        /// <summary>
        /// 角色成员列表
        /// </summary>
        public string RoleCodes { set; get; }
    }
}
