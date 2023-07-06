using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface INumAlarmContract : IScopeDependency
    {
        IRepository<Entitys.NumAlarm, int> NumAlarmRepository { get; }

        IQuery<Entitys.NumAlarm> NumAlarms { get; }

        IQuery<NumAlarmDto> NumAlarmDtos { get; }

        DataResult UpdateNumAlarm(NumAlarm entity);

        /// <summary>
        /// 核查库存上下限预警
        /// </summary>
        /// <returns></returns>
        DataResult CheckNumAlarm();
    }
}