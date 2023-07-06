using wms.Client.Model.Query;

namespace wms.Client.Model.RequestModel
{
    /// <summary>
    /// 组查询
    /// </summary>
    public class GroupRequest : BaseRequest
    {
        public override string route { get { return ServerIP + "api/Group/GetGroup"; } }

        public GroupParameters parameters { get; set; }
    }
}
