using Bussiness.Dtos;

namespace wms.Client.Model.RequestModel
{
    #region 查找/新增/更新/删除用户

    public class PostDoHandCheckClient : BaseRequest
    {

        public override string route { get => ServerIP + "api/Check/PostDoHandCheckClient"; }

        public CheckDto Entity { get; set; }
    }
    public class PostPDACheckComplete : BaseRequest
    {

        public override string route { get => ServerIP + "api/Check/PostPDACheckComplete"; }

        public CheckDto Entity { get; set; }
    }
    #endregion

}
