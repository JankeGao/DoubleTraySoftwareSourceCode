using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;
using HPC.BaseService.Models;

namespace HPC.BaseService.Contracts
{
    public interface IDictionaryContract: IScopeDependency
    {
        #region 字典

        IQuery<Dictionary> Dictionaries { get; }

        /// <summary>
        /// 创建字典
        /// </summary>
        /// <returns></returns>
        DataResult CreateDictionary(Dictionary entity);

        /// <summary>
        /// 编辑字典类型
        /// </summary>
        /// <returns></returns>
        DataResult EditDictionary(Dictionary entity);

        /// <summary>
        /// 移除字典类型
        /// </summary>
        /// <returns></returns>
        DataResult RemoveDictionary(int id);

        #endregion

        #region 字典类别

        IQuery<DictionaryType> DictionaryTypes { get; }

        /// <summary>
        /// 创建字典类型
        /// </summary>
        /// <returns></returns>
        DataResult CreateDictionaryType(DictionaryType entity);

        /// <summary>
        /// 编辑字典类型
        /// </summary>
        /// <returns></returns>
        DataResult EditDictionaryType(DictionaryType entity);

        /// <summary>
        /// 移除字典类型
        /// </summary>
        /// <returns></returns>
        DataResult RemoveDictionaryType(int id);

        #endregion
    }
}