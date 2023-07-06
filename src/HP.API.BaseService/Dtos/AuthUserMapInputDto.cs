
namespace HPC.BaseService.Dtos
{
    public class AuthUserMapInputDto
    {
        /// <summary>
        /// 授权类型编码
        /// </summary>
        public string AuthTypeId { set; get; }
        /// <summary>
        /// 角色成员列表
        /// </summary>
        public string UserCodes { set; get; }
    }
}
