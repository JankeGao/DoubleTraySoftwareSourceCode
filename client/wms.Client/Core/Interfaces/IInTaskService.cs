using System.Threading.Tasks;
using Bussiness.Dtos;
using HP.Utility.Data;

namespace wms.Client.Core.Interfaces
{
    public interface IInTaskService
    {
        Task<DataResult> PostDoHandShelf(InTaskMaterialDto model);

        Task<DataResult> PostClientLocationList(InTaskMaterialDto model);

        Task GetInterfaceIn();

        Task GetInterfaceOut();

        /// <summary>
        /// 手动入库提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DataResult> PostManualInList(InTaskDto model);

    }
}
