using System.Threading.Tasks;
using HP.Utility.Data;
using HPC.BaseService.Dtos;
using wms.Client.Core.Interfaces;
using wms.Client.Model.Entity;
using wms.Client.Model.Query;
using wms.Client.Model.RequestModel;
using wms.Client.Model.ResponseModel;

namespace wms.Client.Service.Service
{
    public class UserService : IUserService
    {
        public async Task<DataResult> LoginAsync(LoginInfo inputDto)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new UserLoginRequest() , inputDto, RestSharp.Method.POST);
            return r;
        }

        public async Task<DataResult> GetCheckAuth(string moduleCode)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetMenuRequest() { moduleCode = moduleCode }, RestSharp.Method.GET);
            return r;
        }

        /// <summary>
        /// 核验托盘权限
        /// </summary>
        /// <param name="trayId"></param>
        /// <returns></returns>
        public async Task<DataResult> GetCheckTrayAuth(int trayId)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetDoCheckAuthRequest() { trayId = trayId }, RestSharp.Method.GET);
            return r;
        }
    }
}
