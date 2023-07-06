using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class AuthorizationService
    {
        /// <summary>
        /// 功能码仓储
        /// </summary>
        public IRepository<FunctionTemplate, int> FunctionTemplateRepository { set; get; }

        public IQuery<FunctionTemplate> FunctionTemplates
        {
            get { return FunctionTemplateRepository.Query(); }
        }

        public DataResult CreateFunctionTemplate(FunctionTemplate entity)
        {
            DataResult result = ValidateFunctionTemplate(entity);
            if (!result.Success) return result;

            if (FunctionTemplates.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure("功能码模版({0})已经存在！".FormatWith(entity.Code));
            }

            if (!FunctionTemplateRepository.Insert(entity))
            {
                return DataProcess.Failure("功能码模版(0})创建失败！".FormatWith(entity.Code));
            }

            return DataProcess.Success("功能码模版({0})创建成功！".FormatWith(entity.Code));
        }

        public DataResult EditFunctionTemplate(FunctionTemplate entity)
        {
            DataResult result = ValidateFunctionTemplate(entity);
            if (!result.Success) return result;

            var oriEntity = FunctionTemplates.FirstOrDefault(a => a.Id == entity.Id);
            oriEntity.CheckNotNull("oriEntity");

            if (FunctionTemplateRepository.Update(a=>new FunctionTemplate
            {
                Name=entity.Name,
                Icon=entity.Icon,
                Sort=entity.Sort
            },a=>a.Code==entity.Code) == 0)
            {
                return DataProcess.Failure("功能码模版({0})编辑失败！".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("功能码模版({0})编辑成功！".FormatWith(oriEntity.Code));
        }

        public DataResult RemoveFunctionTemplate(int id)
        {
            id.CheckGreaterThan("id", 0);

            var oriEntity = FunctionTemplates.FirstOrDefault(a => a.Id == id);
            oriEntity.CheckNotNull("oriEntity");

            if (FunctionTemplateRepository.Delete(id) == 0)
            {
                return DataProcess.Failure("功能码模版({0})移除失败！".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("功能码模版({0})移除成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private DataResult ValidateFunctionTemplate(FunctionTemplate entity)
        {
            entity.CheckNotNull("entity");

            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("编码不能为空");
            }
            if (entity.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("名称不能为空！");
            }

            return DataProcess.Success();
        }
    }
}
