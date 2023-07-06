using Bussiness.Dtos;
using Bussiness.Entitys;

namespace wms.Client.Model.RequestModel
{
    #region 查找/新增/更新/删除用户

    public class PostHandShelfReceiveTask : BaseRequest
    {

        public override string route { get => ServerIP + "api/ReceiveTask/PostHandShelfReceiveTask"; }

        public ReceiveTaskDetail Entity { get; set; }
    }

    public class PostReturn : BaseRequest
    {

        public override string route { get => ServerIP + "api/ReceiveTask/PostReturn"; }

        public ReceiveTaskDetail Entity { get; set; }
    }

    //public class PostManualOutList : BaseRequest
    //{

    //    public override string route { get => ServerIP + "api/OutTask/PostManualOutList"; }

    //    public OutTaskMaterialLabelDto Entity { get; set; }
    //}

    #endregion

}
