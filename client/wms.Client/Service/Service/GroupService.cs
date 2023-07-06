using System.Threading.Tasks;
using wms.Client.Core.Interfaces;
using wms.Client.Model.Query;
using wms.Client.Model.RequestModel;
using wms.Client.Model.ResponseModel;

namespace wms.Client.Service.Service
{
    public class GroupService : IGroupService
    {
        public async Task<GroupResponse> GetGroupsAsync(GroupParameters parameters)
        {
            BaseServiceRequest<GroupResponse> baseService = new BaseServiceRequest<GroupResponse>();
            var r = await baseService.GetRequest<GroupResponse>(new GroupRequest() { parameters = parameters },RestSharp.Method.GET);
            return r;
        }
    }
}
