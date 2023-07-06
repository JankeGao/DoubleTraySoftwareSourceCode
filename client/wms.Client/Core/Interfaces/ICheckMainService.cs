using System.Threading.Tasks;
using Bussiness.Dtos;
using HP.Utility.Data;

namespace wms.Client.Core.Interfaces
{
    public interface ICheckMainService
    {
        Task<DataResult> PostDoHandCheckClient(CheckDto model);

        Task<DataResult> PostPDACheckComplete(CheckDto model);
    }
}
