using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IDeviceAlarmContract : IScopeDependency
    {
        IRepository<Entitys.DeviceAlarm, int> DeviceAlarmRepository { get; }

        /// <summary>
        /// //查找
        /// </summary>
        IQuery<Entitys.DeviceAlarm> DeviceAlarms { get; }


        IQuery<DeviceAlarmDto> DeviceAlarmDtos { get; }

        /// <summary>
        /// //添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateDeviceAlarm(Entitys.DeviceAlarm entity); 

        /// <summary>
        ///  复位-修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditDeviceAlarm(Entitys.DeviceAlarm entity);

        /// <summary>
        ///  删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult DeleteDeviceAlarm(int id);


    }
}
