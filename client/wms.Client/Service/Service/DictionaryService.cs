using System.Threading.Tasks;
using wms.Client.Core.Interfaces;
using wms.Client.Model.Query;
using wms.Client.Model.RequestModel;
using wms.Client.Model.ResponseModel;

namespace wms.Client.Service.Service
{
    public class DictionaryService : IDictionariesService
    {
        public async Task<DictionariesResponse> GetDictionariesAsync(DictionariesParameters parameters)
        {
            BaseServiceRequest<DictionariesResponse> baseService = new BaseServiceRequest<DictionariesResponse>();
            var r = await baseService.GetRequest<DictionariesResponse>(new DictionariesRequest() { parameters = parameters },RestSharp.Method.GET);
            return r;
        }
    }
}
