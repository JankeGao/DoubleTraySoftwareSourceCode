using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IDeviceInfoContract : IScopeDependency
    {
        IRepository<Entitys.DeviceInfo, int> DeviceInfo { get; }
        IQuery<Entitys.DeviceInfo> DeviceInfos { get; }
        IQuery<Dtos.MaterialDto> MaterialDtoList { get; }


        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateDeviceInfo(Entitys.DeviceInfo entity);
        /// <summary>
        /// 编辑设备
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditDeviceInfo(Entitys.Container entity);

        /// <summary>
        /// 移除设备
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //DataResult RemoveDeviceInfo(int id);

    }
}