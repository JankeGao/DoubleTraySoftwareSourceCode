using System.Threading.Tasks;
using Bussiness.Dtos;
using HP.Utility.Data;

namespace wms.Client.Core.Interfaces
{
    public interface IOutTaskService
    {
        Task<DataResult> ConfirmHandPicked(OutTaskMaterialDto model);

        Task<DataResult> PostClientStockList(OutTaskMaterialLabelDto model);

        /// <summary>
        /// 手动入库提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DataResult> PostManualOutList(OutTaskDto model);

    }
}
