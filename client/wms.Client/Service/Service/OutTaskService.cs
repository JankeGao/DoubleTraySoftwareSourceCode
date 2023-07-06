using System.Threading.Tasks;
using Bussiness.Dtos;
using HP.Utility.Data;
using wms.Client.Core.Interfaces;
using wms.Client.Model.RequestModel;

namespace wms.Client.Service.Service
{
    public class OutTaskService : IOutTaskService
    {
        public async Task<DataResult> ConfirmHandPicked(OutTaskMaterialDto model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new ConfirmHandPickedRequest() , model, RestSharp.Method.POST);
            return r;
        }


        public async Task<DataResult> PostClientStockList(OutTaskMaterialLabelDto model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostClientStockList(), model, RestSharp.Method.POST);
            return r;
        }

        public async Task<DataResult> PostManualOutList(OutTaskDto model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostManualOutList(), model, RestSharp.Method.POST);
            return r;
        }

    }
}
