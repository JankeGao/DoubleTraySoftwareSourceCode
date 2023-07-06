using Bussiness.Dtos;

namespace wms.Client.Model.RequestModel
{

    public class ConfirmHandPickedRequest : BaseRequest
    {

        public override string route { get => ServerIP + "api/OutTask/ConfirmHandPicked"; }

        public OutTaskMaterialDto Entity { get; set; }
    }

    public class PostClientStockList : BaseRequest
    {

        public override string route { get => ServerIP + "api/OutTask/PostClientStockList"; }

        public OutTaskMaterialLabelDto Entity { get; set; }
    }

    public class PostManualOutList : BaseRequest
    {

        public override string route { get => ServerIP + "api/OutTask/PostManualOutList"; }

        public OutTaskMaterialLabelDto Entity { get; set; }
    }
}
