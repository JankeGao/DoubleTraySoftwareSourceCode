using System;
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
using HPC.BaseService.Models;
using Module = HPC.BaseService.Models.Module;

namespace HPC.BaseService.Services
{
    public partial class AuthorizationService
    {
        public IQuery<ModuleAuth> ModuleAuths
        {
            get { return ModuleAuthRepository.Query(); }
        }

        /// <summary>
        /// 当前用户模块权限输出DTO
        /// </summary>
        public IQuery<ModuleAuthOutputDto> ModuleAuthOutputDto
        {
            get
            {
                IdentityTicket ticket = IdentityManager.Identity;
                string authType = AuthTypeEnum.Logined.ToString();
                return ModuleAuths
                    .InnerJoin(Modules, (auth, module) => auth.ModuleCode == module.Code)
                    .Select((auth, module) => new ModuleAuthOutputDto
                    {
                        Id = module.Id,
                        Code = module.Code,
                        Icon = module.Icon,
                        Area=module.Area,
                        Name = module.Name,
                        Address = module.Address,
                        Target = module.Target,
                        Sort = module.Sort,
                        ParentCode = module.ParentCode,
                        Enabled = module.Enabled,
                        ModuleType = module.Type,
                        AuthType = module.AuthType,
                        Type = auth.Type,
                        TypeCode = auth.TypeCode
                    })
                    .Where(a =>
                        a.AuthType == authType
                        ||
                        (a.Type == (int) AuthorizationType.Role && ticket.UserData.RoleIds.Contains(a.TypeCode))
                        ||
                        (a.Type == (int) AuthorizationType.UserGroup && ticket.UserData.GroupId == a.TypeCode)
                        ||
                        (a.Type == (int) AuthorizationType.User && ticket.UserData.Code == a.TypeCode)
                    );
            }
        }

        /// <summary>
        /// 模块授权
        /// </summary>
        /// <param name="type">类别</param>
        /// <param name="typeCode">类别编码</param>
        /// <param name="auths">模块权限列表</param>
        /// <returns></returns>
        private DataResult ModuleAuthorization(int type, string typeCode, List<ModuleAuth> auths)
        {
            //原始模块授权
            //List<string> oriModuleAuthIds =
            //    ModuleAuths.Where(a => a.Type == type && a.TypeCode == typeCode)
            //        .Select(a => a.ModuleCode)
            //        .ToList();
            //List<string> oriModuleAuthIds =
            //    ModuleAuths.InnerJoin(Modules, (auth, module) => module.Code == auth.ModuleCode)
            //        .Where((auth, module) => auth.Type == type && auth.TypeCode == typeCode && module.Type != "Page")
            //        .Select((auth, module) => auth.ModuleCode)
            //        .ToList();

            //原始模块授权
            List<string> oriModuleAuthIds =
                ModuleAuths.InnerJoin(Modules, (auth, module) => module.Code == auth.ModuleCode)
                    .Where((auth, module) => auth.Type == type && auth.TypeCode == typeCode)
                    .Select((auth, module) => auth.ModuleCode)
                    .ToList();



            //当前模块授权
            List<string> moduleAuthCodes = auths.Select(a => a.ModuleCode).ToList();
            List<string> subModulesAuthCodes = new List<string>();
            var query = ModuleAuthOutputDto.Where(a => a.Enabled);

            //获取子模块授权
            foreach (var item in moduleAuthCodes)
            {
                // 查询区域名称
                var list = query.Where(a => a.Area == item).ToList();
                if (list.Count>0)
                {
                    var areaCode = query.FirstOrDefault(a => a.Area == item).Area;
                    if (!areaCode.IsNullOrEmpty())
                    {
                        var subModules = GetAuthorizationModules(item);
                        subModulesAuthCodes.AddRange(subModules.Select(a => a.Code).ToList());
                    }
                }
            } 
            // 添加模块授权
            if (subModulesAuthCodes.Count > 0)
            {
                moduleAuthCodes.AddRange(subModulesAuthCodes);
            }

            //待插入
            var moduleForInsert = moduleAuthCodes.Except(oriModuleAuthIds);
            foreach (string moduleCode in moduleForInsert)
            {
                if (!ModuleAuthRepository.Insert(new ModuleAuth()
                {
                    Type = type,
                    TypeCode = typeCode,
                    ModuleCode = moduleCode
                }))
                {
                    return
                        DataProcess.Failure(
                            "{0}({1})系统模块权限创建失败！".FormatWith(EnumHelper.GetCaption(typeof(AuthorizationType),
                                type), typeCode));
                }
            }

            //待删除
            var moduleForDeleted = oriModuleAuthIds.Except(moduleAuthCodes);
            foreach (string moduleCode in moduleForDeleted)
            {
                if (
                    ModuleAuthRepository.Delete(
                        a => a.Type == type && a.TypeCode == typeCode && a.ModuleCode == moduleCode) == 0)
                {
                    return
                        DataProcess.Failure(
                            "{0}({1})原始系统模块权限移除失败！".FormatWith(EnumHelper.GetCaption(typeof(AuthorizationType),
                                type), typeCode));
                }
            }


            return DataProcess.Success();
        }

        /// <summary>
        /// 根据模块编码获取授权的子模块列表     parentCode
        /// </summary>
        /// <param name="moduleCode">模块编码</param>
        /// <param name="type">模块类型</param>
        /// <returns></returns>
        public List<Module> GetAuthorizationModules(string moduleCode)
        {
            var query = ModuleAuthOutputDto.Where(
                a => a.Enabled);

            if (moduleCode.IsNullOrEmpty())
            {
                return null;
            }
            else
            {
                query = query.Where(a => a.Area == moduleCode);
            }

            return query
                .GroupBy(a => a.Id)
                .AndBy(a => a.Code)
                .AndBy(a => a.Icon)
                .AndBy(a => a.Name)
                .AndBy(a => a.Address)
                .AndBy(a => a.Target)
                .AndBy(a => a.Sort)
                .AndBy(a => a.ParentCode)
                .Select(a => new Module
                {
                    Id = a.Id,
                    Code = a.Code,
                    Icon = a.Icon,
                    Name = a.Name,
                    Address = a.Address,
                    Target = a.Target,
                    Sort = a.Sort,
                    ParentCode = a.ParentCode
                })
                .OrderBy(a => a.Sort)
                .ToList();
        }
    }
}
