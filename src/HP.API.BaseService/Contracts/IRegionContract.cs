using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;
using HPC.BaseService.Models;

namespace HPC.BaseService.Contracts
{
    public interface IRegionContract: IScopeDependency
    {
        IQuery<Region> Regions { get; }

        /// <summary>
        /// 创建行政区域
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateRegion(Region entity);

        /// <summary>
        /// 编辑行政区域
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditRegion(Region entity);

        /// <summary>
        /// 移除行政区域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveRegion(int id);
    }
}