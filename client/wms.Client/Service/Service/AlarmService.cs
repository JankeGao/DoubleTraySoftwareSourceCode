using System.Threading.Tasks;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Utility.Data;
using wms.Client.Core.Interfaces;
using wms.Client.Model.RequestModel;

namespace wms.Client.Service.Service
{
    public class AlarmService : IAlarmService
    {
        /// <summary>
        /// 复位报警信息-服务端 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResult> PostRestAllAlarmServer(DeviceAlarm model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new ResetAlarmServerRequest(), model,RestSharp.Method.POST);
            return r;
        }

    }
}
