using Bussiness.Entitys;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IDeviceContract : IScopeDependency
    {
        IQuery<Device> Devices { get; }
        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateDevice(Device entity);

        /// <summary>
        /// 编辑设备
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditDevice(Device entity);

        /// <summary>
        /// 移除设备
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveDevice(int id);

        

    }
}