using Bussiness.Dtos;

namespace wms.Client.Model.RequestModel
{

    /// <summary>
    /// 设备报警信息--更新服务器状态
    /// </summary>
    public class GetTopOutMaterialsRequest : BaseRequest
    {
        public override string route { get => ServerIP + "api/Dashboard/GetTopOutMaterialsClient"; }
    }

    /// <summary>
    /// 设备报警信息--更新服务器状态
    /// </summary>
    public class GetTopInMaterialsRequest : BaseRequest
    {
        public override string route { get => ServerIP + "api/Dashboard/GetTopInMaterialsClient"; }
    }
}
