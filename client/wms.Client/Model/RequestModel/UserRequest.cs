using wms.Client.Model.Entity;
using wms.Client.Model.Query;

namespace wms.Client.Model.RequestModel
{
    #region 查找/新增/更新/删除用户

    /// <summary>
    /// 用户登录
    /// </summary>
    public class UserLoginRequest : BaseRequest
    {

        public override string route { get => ServerIP + "api/Login/PostLoginTest"; }

        public string Code { get; set; }

        public string Password { get; set; }
    }

    public class GetMenuRequest : BaseRequest
    {

        public override string route { get => ServerIP + "api/Login/GetCheckAuth"; }

        public string moduleCode { get; set; }
    }

    public class GetDoCheckAuthRequest : BaseRequest
    {

        public override string route { get => ServerIP + "api/WareHouse/GetDoCheckAuthClient"; }

        public int trayId { get; set; }
    }
    #endregion

}
