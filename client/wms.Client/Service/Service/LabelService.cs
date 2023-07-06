using System.Threading.Tasks;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Utility.Data;
using wms.Client.Core.Interfaces;
using wms.Client.Model.RequestModel;

namespace wms.Client.Service.Service
{
    public class LabelService : ILabelService
    {
        public async Task<DataResult> PostCreateLabel(Label model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new PostCreateLabelRequest() , model, RestSharp.Method.POST);
            return r;
        }

    }
}
