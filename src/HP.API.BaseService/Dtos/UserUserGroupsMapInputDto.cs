
namespace HPC.BaseService.Dtos
{
    public class UserUserGroupsMapInputDto
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { set; get; }
        /// <summary>
        /// 用户组列表
        /// </summary>
        public string UserGroupCodes { set; get; }
    }
}
