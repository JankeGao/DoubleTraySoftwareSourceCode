
using wms.Client.Core.share.Dto;

namespace wms.Client.Core.share.HttpContact.Request
{
    /// <summary>
    /// 用户登录请求
    /// </summary>
    public class UserLoginRequest : BaseRequest
    {
        public override string route { get => "api/Login/PostTestLogin"; }

        public LoginDto Parameter { get; set; }
    }

    /// <summary>
    /// 用户权限请求
    /// </summary>
    public class UserPermRequest : BaseRequest
    {
        public override string route { get => "api/User/Perm"; }

        public string account { get; set; }
    }

}
