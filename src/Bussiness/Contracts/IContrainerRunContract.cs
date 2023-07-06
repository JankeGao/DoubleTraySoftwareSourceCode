using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IContrainerRunContract : IScopeDependency
    {
        /// <summary>
        /// 运行货柜接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult StartRunningContainer(RunningContainer entity);


        /// <summary>
        /// 路径行程设定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult HopperSetting(RunningContainer entity);

        /// <summary>
        /// 安全门行程设定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EmergencyDoorSetting(RunningContainer entity);

        /// <summary>
        /// 重置报警
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult ResetAlarm(RunningContainer entity);

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult GetPlcDeivceStatus(RunningContainer entity);

        /// <summary>
        /// 获取PLC报警信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult GetAlarmInformation(RunningContainer entity);

        /// <summary>
        /// 扫描传递物料单重接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult StartScanBarcodeKeyDown(RunningContainer entity);
    }
}
