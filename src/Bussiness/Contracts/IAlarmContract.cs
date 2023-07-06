using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IAlarmContract : IScopeDependency
    {
        IRepository<Entitys.Alarm, int> AlarmRepository { get; }

        IQuery<Entitys.Alarm> Alarms { get; }

        IQuery<AlarmDto> AlarmDtos { get; }

        DataResult CreateAlarm(Entitys.Alarm entity);

        DataResult EditAlarm(Entitys.Alarm entity);

        DataResult DeleteAlarm(int id);

        // 核查预警信息
        DataResult UpdateAlarm(Alarm entity);



        /// <summary>
        /// 核查库存预警
        /// </summary>
        /// <returns></returns>
        DataResult CheckAlarm();

    }
}
