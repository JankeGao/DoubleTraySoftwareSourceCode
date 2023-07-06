using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class DictionaryService
    {
        /// <summary>
        /// 字典类别仓储
        /// </summary>
        public IRepository<DictionaryType, int> DictionaryTypeRepository { set; get; }

        public IQuery<DictionaryType> DictionaryTypes
        {
            get { return DictionaryTypeRepository.Query(); }
        }

        /// <summary>
        /// 创建字典类型
        /// </summary>
        /// <returns></returns>
        public DataResult CreateDictionaryType(DictionaryType entity)
        {
            var result = ValidateDictionaryType(entity);
            if (!result.Success)
            {
                return DataProcess.Failure(result.Message);
            }

            if (DictionaryTypes.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure("字典分类编码({0})已经存在！".FormatWith(entity.Code));
            }

            if (!DictionaryTypeRepository.Insert(entity))
            {
                return DataProcess.Failure("字典分类({0})创建失败！".FormatWith(entity.Code));
            }

            return DataProcess.Success("字典分类编码({0})创建成功！".FormatWith(entity.Id));
        }

        /// <summary>
        /// 编辑字典类型
        /// </summary>
        /// <returns></returns>
        public DataResult EditDictionaryType(DictionaryType entity)
        {
            var result = ValidateDictionaryType(entity);
            if (!result.Success) return result;

            entity.Id.CheckGreaterThan("Id", 0);

            var code = DictionaryTypes.Where(a => a.Id == entity.Id).Select(a => a.Code).FirstOrDefault();

            if (DictionaryTypeRepository.Update(a =>
                new DictionaryType
                {
                    Name = entity.Name,
                    Sort = entity.Sort,
                    Enabled = entity.Enabled,
                    Remark = entity.Remark,
                    ParentCode=entity.ParentCode
                }, a => a.Id == entity.Id) == 0)
            {
                return DataProcess.Failure("字典分类({0})编辑失败！".FormatWith(code));
            }

            return DataProcess.Success("字典分类({0})编辑成功！".FormatWith(code));
        }

        /// <summary>
        /// 移除字典类型
        /// </summary>
        /// <returns></returns>
        public DataResult RemoveDictionaryType(int id)
        {
            id.CheckGreaterThan("id", 0);

            var oriEntity = DictionaryTypes.Where(a => a.Id == id)
                .Select(a => new {a.Code, a.ParentCode})
                .FirstOrDefault();

            if (DictionaryTypes.Any(a => a.ParentCode == oriEntity.Code))
            {
                return DataProcess.Failure("字典分类({0})存在子字典分类，无法移除！".FormatWith(oriEntity.ParentCode));
            }

            DictionaryTypeRepository.UnitOfWork.TransactionEnabled = true;

            //删除字典分类
            if (DictionaryTypeRepository.Delete(id) == 0)
            {
                return DataProcess.Failure("字典分类({0})移除失败！".FormatWith(oriEntity.Code));
            }

            //删除字典
            if (Dictionaries.Any(a => a.TypeCode == oriEntity.Code))
            {
                if (DictionaryRepository.Delete(a => a.TypeCode == oriEntity.Code) == 0)
                {
                    return DataProcess.Failure("字典分类({0})所属字典移除失败！".FormatWith(oriEntity.Code));
                }
            }

            DictionaryTypeRepository.UnitOfWork.Commit();

            return DataProcess.Success("字典分类编码({0})移除成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        private DataResult ValidateDictionaryType(DictionaryType entity)
        {
            entity.CheckNotNull("entity");
            if (entity.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("字典分类名称不能为空！");
            }
            return DataProcess.Success();
        }
    }
}
