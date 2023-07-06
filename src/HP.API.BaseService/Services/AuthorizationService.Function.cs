using HP.Core.Functions;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class AuthorizationService
    {
        /// <summary>
        /// 模块功能查询
        /// </summary>
        public IQuery<Function> Functions
        {
            get { return FunctionRepository.Query(); }
        }

        /// <summary>
        /// 创建模块功能
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateFunction(Function entity)
        {
            DataResult result = ValidateFunction(entity);
            if (!result.Success) return result;

            entity.Enabled = true;
            if (Functions.Any(a => a.Code == entity.Code && a.ModuleCode == entity.ModuleCode))
            {
                return DataProcess.Failure("功能码({0})已经存在！".FormatWith(entity.Code));
            }

            if (!FunctionRepository.Insert(entity))
            {
                return DataProcess.Failure("模块({0})功能({1})创建失败！".FormatWith(entity.ModuleCode, entity.Code));
            }

            return DataProcess.Success("模块({0})功能({1})创建成功！".FormatWith(entity.ModuleCode, entity.Code));
        }

        /// <summary>
        /// 编辑模块功能按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditFunction(Function entity)
        {
            DataResult result = ValidateFunction(entity);
            if (!result.Success) return result;

            if (FunctionRepository.Update(a=>new Function
            {
                Name=entity.Name,
                Icon=entity.Icon,
                Sort=entity.Sort
            },a=>a.Id==entity.Id) == 0)
            {
                return DataProcess.Failure("模块({0})功能({1})编辑失败！".FormatWith(entity.ModuleCode, entity.Code));
            }

            return DataProcess.Success("模块({0})功能({1})编辑成功！".FormatWith(entity.ModuleCode, entity.Code));
        }

        /// <summary>
        /// 移除模块功能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveFunction(int id)
        {
            id.CheckGreaterThan("id", 0);

            Function entity = Functions.Where(a => a.Id == id).FirstOrDefault();
            entity.CheckNotNull("entity");

            if(FunctionRepository.Delete(id) ==0)
            {
                return DataProcess.Failure("模块({0})功能({1})移除失败！".FormatWith(entity.ModuleCode, entity.Code));
            }

            return DataProcess.Success("模块({0})功能({1})移除成功！".FormatWith(entity.ModuleCode, entity.Code));
        }

        /// <summary>
        /// 批量创建模块功能
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public DataResult BatchCreateFunction(FunctionInputDto inputDto)
        {
            inputDto.CheckNotNull("inputDto");
            inputDto.ModuleCode.CheckNotNullOrEmpty("ModuleCode");

            if (inputDto.FunctionCodes == null || inputDto.FunctionCodes.Count == 0)
            {
                return DataProcess.Failure("至少选择一项模块功能！");
            }

            FunctionRepository.UnitOfWork.TransactionEnabled = true;

            //功能码
            foreach (string functionCode in inputDto.FunctionCodes)
            {
                FunctionTemplate functionTemplate = FunctionTemplates.Where(a => a.Code == functionCode).FirstOrDefault();
                if (functionTemplate == null) continue;

                if (!FunctionRepository.Insert(new Function()
                {
                    Name = functionTemplate.Name,
                    Code = functionCode,
                    ModuleCode = inputDto.ModuleCode,
                    Sort = functionTemplate.Sort,
                    Icon = functionTemplate.Icon,
                    Enabled = true
                }))
                {
                    return DataProcess.Failure("模块({0})功能({1})创建失败！".FormatWith(inputDto.ModuleCode, functionCode));
                }
            }

            FunctionRepository.UnitOfWork.Commit();

            return DataProcess.Success("模块功能创建成功！");
        }

        /// <summary>
        /// 验证模块功能码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private DataResult ValidateFunction(Function entity)
        {
            entity.CheckNotNull("entity");
            entity.ModuleCode.CheckNotNullOrEmpty("ModuleCode");

            if (entity.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("功能名称不能为空！");
            }
            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("功能码不能为空！");
            }
            return DataProcess.Success();
        }
    }
}
