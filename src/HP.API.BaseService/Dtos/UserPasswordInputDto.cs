using HP.Core.Data;

namespace HPC.BaseService.Dtos
{
    public class UserPasswordInputDto : IInputDto<int>
    {
        public int Id { set; get; }
        /// <summary>
        /// 用户编码
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 原始密码
        /// </summary>
        public string OriginalPassword { set; get; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { set; get; }
    }
}
