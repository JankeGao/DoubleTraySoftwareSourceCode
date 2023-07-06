
namespace wms.Client.Core.share.HttpContact.Request
{
    /// <summary>
    /// 获取功能按钮请求
    /// </summary>
    public class AuthItemRequest : BaseRequest
    {
        public override string route { get => "api/AuthItem/GetAll"; }
    }
}
