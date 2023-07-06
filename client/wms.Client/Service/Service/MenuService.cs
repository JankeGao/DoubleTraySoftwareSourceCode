using System.Threading.Tasks;
using wms.Client.Core.Interfaces;
using wms.Client.Model.Query;
using wms.Client.Model.RequestModel;
using wms.Client.Model.ResponseModel;

namespace wms.Client.Service.Service
{
    public class MenuService : IMenuService
    {
        public async Task<MenuResponse> GetMenusAsync(MenuParameters parameters)
        {
            BaseServiceRequest<MenuResponse> baseService = new BaseServiceRequest<MenuResponse>();
            var r = await baseService.GetRequest<MenuResponse>(new MenuRequest() { parameters = parameters },RestSharp.Method.GET);
            return r;
        }
    }
}
