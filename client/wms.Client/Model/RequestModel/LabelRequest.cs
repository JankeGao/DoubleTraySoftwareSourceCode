using Bussiness.Entitys;
using wms.Client.Model.Query;

namespace wms.Client.Model.RequestModel
{
    /// <summary>
    /// 菜单查询
    /// </summary>
    public class PostCreateLabelRequest : BaseRequest
    {
        public override string route { get { return ServerIP + "api/Label/PostDoCreate"; } }

        public Label Entity { get; set; }
    }
}
