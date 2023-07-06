using Bussiness.Dtos;

namespace wms.Client.Model.RequestModel
{
    #region 查找/新增/更新/删除用户

    /// <summary>
    /// 用户登录
    /// </summary>
    public class InTaskServiceRequest : BaseRequest
    {

        public override string route { get => ServerIP + "api/User/Login"; }

        public string account { get; set; }

        public string passWord { get; set; }
    }

    public class PostDoHandShelfRequest : BaseRequest
    {

        public override string route { get => ServerIP + "api/InTask/PostDoHandShelf"; }

        public InTaskMaterialDto Entity { get; set; }
    }

    public class GetInterfaceInRequest : BaseRequest
    {

        public override string route { get => ServerIP + "api/In/GetInterfaceIn"; }

    }

    public class GetInterfaceOutRequest : BaseRequest
    {

        public override string route { get => ServerIP + "api/Out/GetInterfaceOut"; }

    }

    public class PostClientLocationList : BaseRequest
    {

        public override string route { get => ServerIP + "api/InTask/PostClientLocationList"; }

        public InTaskMaterialDto Entity { get; set; }
    }

    public class PostManualInList : BaseRequest
    {

        public override string route { get => ServerIP + "api/InTask/PostManualInList"; }

        public InTaskMaterialDto Entity { get; set; }
    }

    #endregion

}
