using System.Threading.Tasks;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Utility.Data;

namespace wms.Client.Core.Interfaces
{
    public interface IReceiveTaskService
    {
        Task<DataResult> PostHandShelfReceiveTask(ReceiveTaskDetail model);

        Task<DataResult> PostHandShelfReturn(ReceiveTaskDetail model);

        /// <summary>
        /// 手动入库提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
       // Task<DataResult> PostManualOutList(OutTaskDto model);

    }
}
