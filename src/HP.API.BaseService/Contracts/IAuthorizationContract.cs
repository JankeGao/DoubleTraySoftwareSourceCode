using System.Collections.Generic;
using HP.Core.Dependency;
using HP.Core.Functions;
using HP.Core.Security;
using HP.Data.Orm;
using HP.Utility.Data;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Contracts
{
    public interface IAuthorizationContract : IScopeDependency
    {
        #region 模块

        IQuery<Models.Module> Modules { get; }
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="entity">dto</param>
        /// <returns></returns>
        DataResult CreateModule(Models.Module entity);
        /// <summary>
        /// 编辑模块
        /// </summary>
        /// <param name="entity">dto</param>
        /// <returns></returns>
        DataResult EditModule(Models.Module entity);
        /// <summary>
        /// 移除模块
        /// </summary>
        /// <param name="id">模块主键</param>
        /// <returns></returns>
        DataResult RemoveModule(int id);


        #endregion

        #region 功能

        IQuery<Function> Functions { get; }
        /// <summary>
        /// 创建模块功能
        /// </summary>
        /// <param name="entity">模块功能实体</param>
        /// <returns></returns>
        DataResult CreateFunction(Function entity);
        /// <summary>
        /// 编辑模块功能
        /// </summary>
        /// <param name="entity">模块功能实体</param>
        /// <returns></returns>
        DataResult EditFunction(Function entity);
        /// <summary>
        /// 移除模块功能
        /// </summary>
        /// <param name="id">模块功能主键</param>
        /// <returns></returns>
        DataResult RemoveFunction(int id);

        /// <summary>
        /// 批量创建模块功能
        /// </summary>
        /// <param name="inputDto">DTO</param>
        /// <returns></returns>
        DataResult BatchCreateFunction(FunctionInputDto inputDto);

        #endregion

        #region 功能模版

        IQuery<FunctionTemplate> FunctionTemplates { get; }
        DataResult CreateFunctionTemplate(FunctionTemplate entity);
        DataResult EditFunctionTemplate(FunctionTemplate entity);
        DataResult RemoveFunctionTemplate(int id);

        #endregion

        #region 授权

        /// <summary>
        /// 初始化模块功能树
        /// </summary>
        /// <param name="modules">权限模块</param>
        /// <param name="parentId">上级模块编码</param>
        /// <returns></returns>
        List<ModuleTreeOutputDto> InitModuleFunctionTree(List<Models.Module> modules, string parentId);

        /// <summary>
        /// 模块授权查询
        /// </summary>
        IQuery<ModuleAuth> ModuleAuths { get; }

        /// <summary>
        /// 当前用户模块权限输出DTO
        /// </summary>
        IQuery<ModuleAuthOutputDto> ModuleAuthOutputDto { get; }

        /// <summary>
        /// 功能授权查询
        /// </summary>
        IQuery<FunctionAuth> FunctionAuths { get; }

        /// <summary>
        /// 数据规则查询
        /// </summary>
        IQuery<DataRule> DataRules { get; }

        /// <summary>
        /// 设置授权
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        DataResult SetAuthorization(AuthInputDto inputDto);

        /// <summary>
        /// 根据模块编码获取授权的子模块列表
        /// </summary>
        /// <param name="moduleCode">模块编码</param>
        /// <param name="type">模块类型</param>
        /// <returns></returns>
        List<Models.Module> GetAuthorizationModules(string moduleCode);

        #endregion
    }
}