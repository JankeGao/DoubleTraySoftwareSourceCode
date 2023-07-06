
namespace HPC.BaseService.Dtos
{
    public class UserGroupUsersMapInputDto
    {
        /// <summary>
        /// 用户组编码
        /// </summary>
        public string UserGroupCode { set; get; }
        /// <summary>
        /// 用户成员列表
        /// </summary>
        public string UserCodes { set; get; }
    }
}
