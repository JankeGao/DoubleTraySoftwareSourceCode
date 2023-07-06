using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Contracts
{
    public interface ILabelContract: IScopeDependency
    {
        void Test();
        IQuery<Label> Labels { get; }

        IQuery<LabelMap> LabelMaps { get; }

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateLabel(Label entity);

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult AddLabel(LabelMapInputDto entity);

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveLabel(int id);
    }
}