using System.Threading.Tasks;
using Bussiness.Dtos;
using HP.Utility.Data;
using wms.Client.Core.Interfaces;
using wms.Client.Model.RequestModel;

namespace wms.Client.Service.Service
{
    public class InTaskService : IInTaskService
    {
        public async Task<DataResult> PostDoHandShelf(InTaskMaterialDto model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostDoHandShelfRequest() , model, RestSharp.Method.POST);
            return r;
        }

        public async Task GetInterfaceIn()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            await baseService.GetRequest<DataResult>(new GetInterfaceInRequest(), RestSharp.Method.GET);
        }

        public async Task GetInterfaceOut()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            await baseService.GetRequest<DataResult>(new GetInterfaceOutRequest(), RestSharp.Method.GET);
        }

        public async Task<DataResult> PostClientLocationList(InTaskMaterialDto model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostClientLocationList(), model, RestSharp.Method.POST);
            return r;
        }

        public async Task<DataResult> PostManualInList(InTaskDto model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostManualInList(), model, RestSharp.Method.POST);
            return r;
        }

    }
}
