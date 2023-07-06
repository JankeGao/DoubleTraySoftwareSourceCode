using System.Threading.Tasks;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Utility.Data;
using wms.Client.Core.Interfaces;
using wms.Client.Model.RequestModel;

namespace wms.Client.Service.Service
{
    public class DashboardService : IDashboardService
    {
        /// <summary>
        /// 复位报警信息-服务端 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResult> GetTopOutMaterials()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetTopOutMaterialsRequest(){},RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 复位报警信息-服务端 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResult> GetTopInMaterials()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetTopInMaterialsRequest() { }, RestSharp.Method.GET);
            return r;
        }
    }
}
