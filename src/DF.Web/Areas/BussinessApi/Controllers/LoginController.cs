using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.CommonHelper;
using HP.Core.Data;
using HP.Core.Data.Attributes;
using HP.Core.Functions;
using HP.Core.Logging;
using HP.Core.Security.Permissions;
//using HP.Core.Security.PermSimpleWmsions;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Utility;
using HP.Web.Mvc.Utility.Net;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;
using Newtonsoft.Json;
using Module = HPC.BaseService.Models.Module;

namespace SimpleWms.Web.Areas.OpenApi.Controllers
{
    [Ignore]
    [AllowAnonymous]
    [Description("登录")]
    public class LoginController : BaseApiController
    {
        public IIdentityContract IdentityContract { set; get; }
        public IAuthorizationContract AuthorizationContract { set; get; }
        public IRepository<DataLogItem, Guid> DataLogItemRepository { set; get; }
        public IRepository<ToDo, int> ToDoRepository { set; get; }

        [AllowAnonymous]
        [LogFilter(Type = LogType.Login, Name = "用户登录")]
        // GET 方法测试.
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "admin");
            return response;
        }


        [AllowAnonymous]
        [LogApiFilter(Type = LogType.Login, Name = "用户登录")]
        [HttpPost]
        public HttpResponseMessage PostLogin(LoginInfo inputDto)
        {
            inputDto.SessionId = SessionHelper.GetSessionId();
            inputDto.ClientIp = RequestHelper.GetClientIp();
            // Write the list to the response body.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.Login(inputDto).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Login, Name = "用户注销")]
        [HttpPost]
        public HttpResponseMessage Postlogout()
        {
            IdentityManager.SignOut(IdentityManager.Identity);       
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [LogApiFilter(Type = LogType.Login, Name = "获取用户信息")]
        [HttpGet]
        public HttpResponseMessage GetUserInfo()
        {
            var values = Request.Headers.GetValues("token").ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.GetUserInfo(values.FirstOrDefault()).Data.ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Login, Name = "获取动态菜单")]
        [HttpGet]
        public HttpResponseMessage GetMenu()
        {
            var moduleList = AuthorizationContract.ModuleAuthOutputDto.Where(a => a.Enabled != false ).Select(a => new Module
                {
                    Id = a.Id,
                    Code = a.Code,
                    Name = a.Name,
                    ParentCode = a.ParentCode,
                    Sort = a.Sort,
                    Icon = a.Icon,
                    Address = a.Address,
                    Enabled = a.Enabled,
                    Target = a.Target,
                    AuthType = a.AuthType,
                    Type = a.ModuleType
                })
                .OrderBy(p => p.Sort)
                .ToList();
            // 权限去重
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(moduleList.MyDistinct(s => s.Code).ToList()));
            return response;
        }


    }
}
