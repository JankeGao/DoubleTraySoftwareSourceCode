using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.Data;
using HP.Core.Data.Attributes;
using HP.Core.Logging;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.API.BaseService.Contracts;
using HP.API.BaseService.Models;
using HP.Core.Functions;
using HP.Data.Entity.Pagination;
using HP.Web.Mvc.Pagination;
using Newtonsoft.Json;

namespace Hellopets.Web.Areas.OpenApi.Controllers
{
    [Ignore]
    [AllowAnonymous]
    [Description("登录")]
    public class ModuleController : BaseApiController
    {
    public IIdentityContract IdentityContract { set; get; }
    public IAuthorizationContract AuthorizationContract { set; get; }
    public IRepository<DataLogItem, Guid> DataLogItemRepository { set; get; }
    public IRepository<ToDo, int> ToDoRepository { set; get; }


    #region 查询

    /// <summary>
    /// 分页数据
    /// </summary>
    /// <param name="pageCondition"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [LogFilter(Type = LogType.Operate, Name = "获取角色信息")]
    // GET 方法测试 [FromUri]
    public HttpResponseMessage GetRoleList([FromUri] MvcPageCondition pageCondition)
    {
        var query = IdentityContract.Roles;
        var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Name");
        pageCondition.FilterRuleCondition.Remove(pageCondition.FilterRuleCondition.Find(a => a.Field == "Sort"));
        pageCondition.FilterRuleCondition.Remove(pageCondition.FilterRuleCondition.Find(a => a.Field == "Rows"));
        pageCondition.FilterRuleCondition.Remove(pageCondition.FilterRuleCondition.Find(a => a.Field == "Page"));
        if (filterRule != null)
        {
            string value = filterRule.Value.ToString();
            query = query.Where(p => p.Name.Contains(value));
            pageCondition.FilterRuleCondition.Remove(filterRule);

        }
        var pageResult = query.ToPage(pageCondition).ToMvcJson();
        // var b = IdentityContract.Roles.Where(pageCondition).OrderByDesc(a => a.Id).ToList().ToMvcJson();
        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, pageResult);
        return response;
    }


    #endregion

    [AllowAnonymous]
    [LogApiFilter(Type = LogType.Operate, Name = "角色创建")]
    [HttpPost]
    public HttpResponseMessage PostDoCreate(Role entity)
    {
        HttpResponseMessage response =
            Request.CreateResponse(HttpStatusCode.OK, IdentityContract.CreateRole(entity).ToMvcJson());
        return response;
    }

    [LogApiFilter(Type = LogType.Operate, Name = "角色编辑")]
    [HttpPost]
    public HttpResponseMessage PostDoEdit(Role entity)
    {
        HttpResponseMessage response =
            Request.CreateResponse(HttpStatusCode.OK, IdentityContract.EditRole(entity).ToMvcJson());
        return response;
    }

    [LogApiFilter(Type = LogType.Operate, Name = "获取授权菜单")]
    [HttpGet]
    public HttpResponseMessage GetModuleList(int type, string typeCode)
    {
        // 
        string moduleType = ModuleType.None.ToString();
        List<Module> moduleList =
            AuthorizationContract.Modules.LeftJoin(AuthorizationContract.ModuleAuths,
                    (module, auth) =>
                        module.Code == auth.ModuleCode && auth.Type == type && auth.TypeCode == typeCode)
                .Select((module, auth) => new Module
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
