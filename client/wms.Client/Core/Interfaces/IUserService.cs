using System.Threading.Tasks;
using HP.Utility.Data;
using HPC.BaseService.Dtos;
using wms.Client.Model.Entity;
using wms.Client.Model.Query;
using wms.Client.Model.ResponseModel;

namespace wms.Client.Core.Interfaces
{
    public interface IUserService 
    {
        Task<DataResult> LoginAsync(LoginInfo inputDto);

        Task<DataResult> GetCheckAuth(string moduleCode);

        Task<DataResult> GetCheckTrayAuth(int trayId);
    }
}
