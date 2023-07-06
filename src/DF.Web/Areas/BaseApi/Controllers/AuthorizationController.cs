using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using HP.Core.Functions;
using HP.Core.Logging;
using HP.Core.Security;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace DF.Web.Areas.BaseApi.Controllers
{
    [Description("权限管理")]
    public class AuthorizationController : BaseApiController
    {
        public IAuthorizationContract AuthorizationContract { set; get; }
        public IEntityInfoContract EntityInfoContract { set; get; }
        public IIdentityContract IdentityContract { set; get; }

        [LogApiFilter(Type = LogType.Operate, Name = "获取授权菜单")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetModuleList()
        {
            string typeCode = "Menu";
            string moduleType = ModuleType.None.ToString();
            List<HP.Core.Functions.Module> moduleList =
                AuthorizationContract.Modules
                    .Select((module) => new HP.Core.Functions.Module
                    {
                        Id = module.Id,
                        Code = module.Code,
                        Name = module.Name,
                        Address = module.Address,
                        ParentCode = module.ParentCode,
                        Sort = module.Sort,
                        Icon = module.Icon,
                        Target = module.Target,
                        Enabled = module.Enabled,
                        Type = module.Type
                    })
                    .Where(a => a.Type != moduleType && a.Type == typeCode && a.Target == "Page")
                    .OrderBy(a => a.Sort)
                    .ToList();
            // var result = JsonConvert.SerializeObject(moduleList).t;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, moduleList.ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "获取授权树菜单")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetModuleTreeList()
        {
            // && auth.TypeCode == typeCode
            int type = 1;
            string typeCode = "Menu";
            string moduleType = ModuleType.None.ToString();
            List<HPC.BaseService.Models.Module> moduleList =
                AuthorizationContract.Modules.LeftJoin(AuthorizationContract.ModuleAuths,
                        (module, auth) =>
                            module.Code == auth.ModuleCode && auth.Type == type && auth.TypeCode == typeCode)
                    .Select((module, auth) => new HPC.BaseService.Models.Module
                    {
                        Id = module.Id,
                        Code = module.Code,
                        Name = module.Name,
                        Address = module.Address,
                        ParentCode = module.ParentCode,
                        Sort = module.Sort,
                        Icon = module.Icon,
                        Target = module.Target,
                        Enabled = module.Enabled,
                        Type = module.Type
                    })
                    .Where(a => a.Type != moduleType && a.Type == typeCode)
                    .OrderBy(a => a.Sort)
                    .ToList();


            return Request.CreateResponse(HttpStatusCode.OK, AuthorizationContract.InitModuleFunctionTree(moduleList, "").ToMvcJson());
        }
        [LogApiFilter(Type = LogType.Operate, Name = "角色授权")]
        [HttpPost]
        public HttpResponseMessage PostSetAuthorization(AuthInputDto inputDto)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, AuthorizationContract.SetAuthorization(inputDto).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 功能权限--用户组
        /// </summary>
        /// <param name="typeCode">类型编码</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetModuleAuth(string typeCode,int type)
        {
            string moduleType = ModuleType.None.ToString();
            List<HP.Core.Functions.Module> modules =
                AuthorizationContract.Modules.InnerJoin(AuthorizationContract.ModuleAuths,
                        (module, auth) =>
                            module.Code == auth.ModuleCode && auth.Type == type && auth.TypeCode == typeCode)
                    .Select((module, auth) => new HP.Core.Functions.Module
                    {
                        Id = module.Id,
                        Code = module.Code,
                        Name = module.Name,
                        ParentCode = module.ParentCode,
                        Sort = module.Sort,
                        Icon = module.Icon,
                        Target = module.Target,
                        Enabled = auth.Id != null,
                        Type = module.Type
                    })
                    .Where(a => a.Type != moduleType)
                    .OrderBy(a => a.Sort)
                    .ToList();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, modules.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 功能权限--用户组权限-Role
        /// </summary>
        /// <param name="typeCode">类型编码</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetModuleUserRoleAuth(string typeCode)
        {
            var roles = IdentityContract.RoleUsersMaps.Where(a => a.UserCode == typeCode).Select(a => a.RoleCode).ToList();
            var list= AuthorizationContract.ModuleAuths.InnerJoin(AuthorizationContract.Modules, (auth, module) => auth.ModuleCode == module.Code)
                .Select((auth, module) => new ModuleAuthOutputDto
                {
                    Id = module.Id,
                    Code = module.Code,
                    Icon = module.Icon,
                    Area = module.Area,
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
                .Where(a => a.Type == (int)AuthorizationType.Role && roles.Contains(a.TypeCode)).ToList();

            var userlist = AuthorizationContract.ModuleAuths.InnerJoin(AuthorizationContract.Modules, (auth, module) => auth.ModuleCode == module.Code)
                .Select((auth, module) => new ModuleAuthOutputDto
                {
                    Id = module.Id,
                    Code = module.Code,
                    Icon = module.Icon,
                    Area = module.Area,
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
                .Where(a => a.Type == (int)AuthorizationType.User && typeCode == a.TypeCode).ToList();

            var result = list.Concat(userlist);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "模块创建")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(HPC.BaseService.Models.Module entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, AuthorizationContract.CreateModule(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "模块编辑")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(HPC.BaseService.Models.Module entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, AuthorizationContract.EditModule(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "模块编辑")]
        [HttpPost]
        public HttpResponseMessage PostDoRemove(HPC.BaseService.Models.Module entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, AuthorizationContract.RemoveModule(entity.Id).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取实体数据规则列表
        /// </summary>
        /// <param name="type">类别</param>
        /// <param name="typeCode">类别</param>
        /// <returns></returns>
        [AuthorizationApiFilter(ActionName = "Authorization")]
        public ActionResult GetEntityDataAuthRules(int type, string typeCode)
        {
            return EntityInfoContract.EntityInfos
                .LeftJoin(AuthorizationContract.DataRules, (entity, auth) => entity.Id == auth.EntityInfoId && auth.Type == type && auth.TypeCode == typeCode)
                .Select((entity, auth) => new
                {
                    entity.Id,
                    entity.Name,
                    entity.PropertyNamesJson,
                    auth.DataFilterRule
                })
                .ToList()
                .ToMvcJson();
        }

    }
}
