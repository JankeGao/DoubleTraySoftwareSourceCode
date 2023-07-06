using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.Functions;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;
using Newtonsoft.Json;

namespace DF.Web.Areas.BaseApi.Controllers
{
    [Description("角色管理")]
    public class RoleController : BaseApiController
    {
        public IIdentityContract IdentityContract { set; get; }
        public IAuthorizationContract AuthorizationContract { set; get; }

        #region 查询

        /// <summary>
        /// 查询启用角色信息
        /// </summary>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetRole()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.Roles.Where(a => !a.IsSystem && a.Enabled).OrderBy(a => a.Id).ToList().ToMvcJson());
            return response;
        }
        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetRoleList([FromUri]MvcPageCondition pageCondition)
        {
            var query = IdentityContract.RoleDtos;
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Name");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "DepartmentCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.DepartmentCode.Contains(value)||p.ParentCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var pageResult = query.Where(a=>!a.IsSystem).ToPage(pageCondition).ToMvcJson();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, pageResult);
            return response;
        }


        #endregion

        [LogApiFilter(Type = LogType.Operate, Name = "角色创建")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Role entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.CreateRole(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "角色编辑")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(Role entity)
        {          
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,IdentityContract.EditRole(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "角色删除")]
        [HttpPost]
        public HttpResponseMessage PostDoRemove(Role entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.RemoveRole(entity.Id).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "获取授权菜单")]
        [HttpGet]
        public HttpResponseMessage GetModuleList(int type, string typeCode)
        {
            // 
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
            var result = JsonConvert.SerializeObject(moduleList);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }
    }
}
