using System.Collections.Generic;
using System.Linq;
using HP.Core.Security;
using HP.Utility;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class AuthorizationService
    {
        /// <summary>
        /// 设置授权
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public DataResult SetAuthorization(AuthInputDto inputDto)
        {
            inputDto.CheckNotNull("inputDto");
            inputDto.TypeCode.CheckNotNullOrEmpty("TypeCode");
            inputDto.Type.CheckGreaterThan("Type", 0);
            if (EnumHelper.GetEnums(typeof(AuthorizationType)).All(a => a.Value != inputDto.Type))
            {
                return DataProcess.Failure("授权类型异常！");
            }

            FunctionAuthRepository.UnitOfWork.TransactionEnabled = true;

            //模块授权
            List<ModuleAuth> moduleAuths = inputDto.ModuleAuthJson.FromJsonString<List<ModuleAuth>>();
            var result = ModuleAuthorization(inputDto.Type, inputDto.TypeCode, moduleAuths);
            if (!result.Success) return result;

            ////功能授权
            //List<FunctionAuth> funcAuths = inputDto.FunctionAuthJson.FromJsonString<List<FunctionAuth>>();
            //result = FunctionAuthorization(inputDto.Type, inputDto.TypeCode, funcAuths);
            //if (!result.Success) return result;

            ////数据规则权限
            //List<DataRule> dataRules = inputDto.DataRuleJson.FromJsonString<List<DataRule>>();
            //result = DataRuleAuthorization(inputDto.Type, inputDto.TypeCode, dataRules);
            //if (!result.Success) return result;

            FunctionAuthRepository.UnitOfWork.Commit();

            return
                DataProcess.Success("{0}({1})权限授权成功！".FormatWith(EnumHelper.GetCaption(typeof(AuthorizationType),
                    inputDto.Type), inputDto.TypeCode));
        }
    }
}