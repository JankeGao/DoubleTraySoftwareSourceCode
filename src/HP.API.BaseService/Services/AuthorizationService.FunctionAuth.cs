using System.Collections.Generic;
using System.Linq;
using HP.Core.Functions;
using HP.Core.Security;
using HP.Core.Security.Permissions;
using HP.Data.Orm;
using HP.Utility;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Dtos;

namespace HPC.BaseService.Services
{
    public partial class AuthorizationService
    {
        public IQuery<FunctionAuth> FunctionAuths
        {
            get { return FunctionAuthRepository.Query(); }
        }

        //public IQuery<FunctionAuthOutputDto> FunctionAuthOutputDto
        //{
        //    get
        //    {
        //        IdentityTicket ticket = IdentityManager.Identity;

        //        return FunctionAuths
        //            .InnerJoin(Modules, (auth, module) => auth.ModuleId == module.Id)
        //            .InnerJoin(Functions,
        //                (auth, module, func) => auth.ModuleId == func.ModuleId && auth.FunctionCode == func.Code)
        //            .Select((auth, module, func) => new FunctionAuthOutputDto
        //            {
        //                Id = auth.Id,
        //                ModuleId = module.Id,
        //                ModuleIsPublic = module.IsPublic,
        //                Type = auth.Type,
        //                TypeId = auth.TypeId,
        //                Code = func.Code,
        //                Name = func.Name,
        //                Icon = func.Icon
        //            }).Where(a =>
        //                a.ModuleId == _authData.ModuleId
        //                &&
        //                (
        //                    a.ModuleIsPublic
        //                    ||
        //                    (a.Type == (int) AuthorizationTypeEnum.Role &&
        //                     ticket.UserData.RoleIds.Contains(a.TypeId))
        //                    ||
        //                    (a.Type == (int) AuthorizationTypeEnum.Position &&
        //                     ticket.UserData.PositionId == a.TypeId)
        //                    ||
        //                    (a.Type == (int) AuthorizationTypeEnum.Job && ticket.UserData.JobId == a.TypeId)
        //                    ||
        //                    (a.Type == (int) AuthorizationTypeEnum.UserGroup &&
        //                     ticket.UserData.GroupId == a.TypeId)
        //                    ||
        //                    (a.Type == (int) AuthorizationTypeEnum.User && ticket.UserData.Id == a.TypeId)
        //                    )
        //            );
        //    }
        //}

        /// <summary>
        /// 获取按钮授权查询
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        private IQuery<FunctionAuthOutputDto> GetFunctionAuthQuery(string moduleCode)
        {
            IdentityTicket ticket = IdentityManager.Identity;

            return FunctionAuths
                .InnerJoin(Modules, (auth, module) => auth.ModuleCode == module.Code)
                .InnerJoin(Functions,
                    (auth, module, func) => auth.ModuleCode == func.ModuleCode && auth.FunctionCode == func.Code)
                .Select((auth, module, func) => new FunctionAuthOutputDto
                {
                    Id = auth.Id,
                    ModuleCode = module.Code,
                    ModuleAuthType = module.AuthType,
                    Type = auth.Type,
                    TypeCode = auth.TypeCode,
                    Code = func.Code,
                    Name = func.Name,
                    Icon = func.Icon
                }).Where(a =>
                    a.ModuleCode == moduleCode
                    &&
                    (
                        a.ModuleAuthType==AuthTypeEnum.Logined.ToString()
                        ||
                        (a.Type == (int)AuthorizationType.Role &&
                         ticket.UserData.RoleIds.Contains(a.TypeCode))
                        ||
                        (a.Type == (int)AuthorizationType.UserGroup &&
                         ticket.UserData.GroupId == a.TypeCode)
                        ||
                        (a.Type == (int)AuthorizationType.User && ticket.UserData.Code == a.TypeCode)
                        )
                );
        }

        /// <summary>
        /// 功能授权
        /// </summary>
        /// <param name="type">类别</param>
        /// <param name="typeCode">类别编码</param>
        /// <param name="auths">功能权限列表</param>
        /// <returns></returns>
        private DataResult FunctionAuthorization(int type, string typeCode, List<FunctionAuth> auths)
        {
            //原始功能授权
            var oriFuncAuthIds =
                FunctionAuths.Where(a => a.Type == type && a.TypeCode == typeCode)
                    .Select(a => new { a.ModuleCode, a.FunctionCode })
                    .ToList();
            //功能授权
            var funcAuthIds = auths.Select(a => new { a.ModuleCode, a.FunctionCode });

            //待插入
            var funcForInsert = funcAuthIds.Except(oriFuncAuthIds);
            foreach (var function in funcForInsert)
            {
                if (!FunctionAuthRepository.Insert(new FunctionAuth()
                {
                    Type = type,
                    TypeCode = typeCode,
                    ModuleCode = function.ModuleCode,
                    FunctionCode = function.FunctionCode
                }))
                {
                    return
                        DataProcess.Failure(
                            "{0}({1})功能权限创建失败！".FormatWith(EnumHelper.GetCaption(typeof (AuthorizationType),
                                type), typeCode));
                }
            }

            //待删除
            var funcForDeleted = oriFuncAuthIds.Except(funcAuthIds);
            foreach (var function in funcForDeleted)
            {
                if (
                    FunctionAuthRepository.Delete(
                        a => a.Type == type && a.TypeCode == typeCode && a.ModuleCode == function.ModuleCode && a.FunctionCode == function.FunctionCode) == 0)
                {
                    return
                        DataProcess.Failure(
                            "{0}({1})原始功能权限移除失败！".FormatWith(EnumHelper.GetCaption(typeof(AuthorizationType),
                               type), typeCode));
                }
            }

            return DataProcess.Success();
        }
    }
}
