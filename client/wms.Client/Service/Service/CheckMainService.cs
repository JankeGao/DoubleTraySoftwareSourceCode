using System.Threading.Tasks;
using Bussiness.Dtos;
using HP.Utility.Data;
using wms.Client.Core.Interfaces;
using wms.Client.Model.RequestModel;

namespace wms.Client.Service.Service
{
    public class CheckMainService : ICheckMainService
    {
        public async Task<DataResult> PostDoHandCheckClient(CheckDto model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostDoHandCheckClient(), model, RestSharp.Method.POST);
            return r;
        }

        public async Task<DataResult> PostPDACheckComplete(CheckDto model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostPDACheckComplete(), model, RestSharp.Method.POST);
            return r;
        }
    }
}
