using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class DictionaryService : IDictionaryContract
    {
        /// <summary>
        /// 字典仓储
        /// </summary>
        public IRepository<Dictionary, int> DictionaryRepository { set; get; }


        public IQuery<Dictionary> Dictionaries
        {
            get { return DictionaryRepository.Query(); }
        }

        /// <summary>
        /// 创建字典
        /// </summary>
        /// <returns></returns>
        public DataResult CreateDictionary(Dictionary entity)
        {
            var result = ValidateDictionary(entity);
            if (!result.Success) return result;

            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("请输入字典编码！");
            }

            if (entity.TypeCode.IsNullOrEmpty())
            {
                return DataProcess.Failure("请输入字典类别编码！");
            }

            if (Dictionaries.Any(a => a.Id == entity.Id))
            {
                return DataProcess.Failure("字典({0})已经存在！".FormatWith(entity.Code));
            }

            if (!DictionaryRepository.Insert(entity))
            {
                return DataProcess.Failure("字典({0})创建失败！".FormatWith(entity.Id));
            }

            return DataProcess.Success("字典({0})创建成功！".FormatWith(entity.Id));
        }

        /// <summary>
        /// 编辑字典类型
        /// </summary>
        /// <returns></returns>
        public DataResult EditDictionary(Dictionary entity)
        {
            entity.Id.CheckGreaterThan("id", 0);

            var result = ValidateDictionary(entity);
            if (!result.Success) return result;

            var code = Dictionaries.FirstOrDefault(a => a.Id == entity.Id);

            if (DictionaryRepository.Update(a=>new Dictionary
            {
                Name=entity.Name,
                ParentCode=entity.ParentCode,
                Remark=entity.Remark,
                Value=entity.Value,
                Enabled=entity.Enabled,
                Sort=entity.Sort
            },a=>a.Id==entity.Id) == 0)
            {
                return DataProcess.Failure("字典({0})编辑失败！".FormatWith(code));
            }
            return DataProcess.Success("字典({0})编辑成功！".FormatWith(code));
        }

        /// <summary>
        /// 移除字典类型
        /// </summary>
        /// <returns></returns>
        public DataResult RemoveDictionary(int id) 
        {
            id.CheckGreaterThan("id",0);

            if (DictionaryRepository.Delete(id) == 0)
            {
                return DataProcess.Failure("字典({0})移除失败！".FormatWith(id));
            }

            return DataProcess.Success("字典({0})移除成功！".FormatWith(id));
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        private DataResult ValidateDictionary(Dictionary entity)
        {
            entity.CheckNotNull("entity");

            if (entity.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("请输入字典名称！");
            }

            return DataProcess.Success();
        }
    }
}
