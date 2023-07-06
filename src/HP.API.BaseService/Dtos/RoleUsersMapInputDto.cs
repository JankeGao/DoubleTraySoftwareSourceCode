
namespace HPC.BaseService.Dtos
{
    public class RoleUsersMapInputDto
    {
        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleCode { set; get; }
        /// <summary>
        /// 用户成员列表
        /// </summary>
        public string UserCodes { set; get; }
    }
}
