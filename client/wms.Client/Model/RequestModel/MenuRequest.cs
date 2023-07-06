using wms.Client.Model.Query;

namespace wms.Client.Model.RequestModel
{
    /// <summary>
    /// 菜单查询
    /// </summary>
    public class MenuRequest : BaseRequest
    {
        public override string route { get { return ServerIP + "api/Menu/GetMenu"; } }

        public MenuParameters parameters { get; set; }
    }
}
