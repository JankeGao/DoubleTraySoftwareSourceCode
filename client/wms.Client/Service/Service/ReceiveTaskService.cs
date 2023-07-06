using System.Threading.Tasks;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Utility.Data;
using wms.Client.Core.Interfaces;
using wms.Client.Model.RequestModel;

namespace wms.Client.Service.Service
{
    public class ReceiveTaskService : IReceiveTaskService
    {
        public async Task<DataResult> PostHandShelfReceiveTask(ReceiveTaskDetail model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostHandShelfReceiveTask() , model, RestSharp.Method.POST);
            return r;
        }


        public async Task<DataResult> PostHandShelfReturn(ReceiveTaskDetail model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostReturn(), model, RestSharp.Method.POST);
            return r;
        }

        //public async Task<DataResult> PostManualOutList(OutTaskDto model)
        //{
        //    BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
        //    var r = await baseService.GetRequest<DataResult>(new PostManualOutList(), model, RestSharp.Method.POST);
        //    return r;
        //}

    }
}
