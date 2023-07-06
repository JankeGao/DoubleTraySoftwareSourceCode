using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.CommonHelper;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Core.Security.Permissions;
using HP.Utility.Data;
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

namespace DF.Web.Areas.BaseApi.Controllers
{
    [Description("登录")]
    public class LoginController : BaseApiController
    {
        public IIdentityContract IdentityContract { set; get; }
        public IAuthorizationContract AuthorizationContract { set; get; }
        public IRepository<DataLogItem, Guid> DataLogItemRepository { set; get; }

        [AllowAnonymous]
        [LogFilter(Type = LogType.Login, Name = "用户登录")]
        // GET 方法测试
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
            //if (CheckLicense())
            //{
            //    HttpResponseMessage response1 = Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("系统授权已到期，请联系管理员！").ToMvcJson());
            //    return response1;
            //}

            inputDto.SessionId = SessionHelper.GetSessionId();
            inputDto.ClientIp = RequestHelper.GetClientIp();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.Login(inputDto).ToMvcJson());
            return response;
        }

        private bool CheckLicense()
        {
            string strExprationDate = System.Configuration.ConfigurationManager.AppSettings["CheckTime"];
            strExprationDate = Bussiness.Common.EncryptHelper.DESDecrypt(strExprationDate, Bussiness.Common.EncryptHelper.EncryptPsw);
            DateTime dt = DateTime.Now;
            if (!string.IsNullOrEmpty(strExprationDate))
            {
                DateTime.TryParse(strExprationDate, out dt);
            }

            if ((dt - DateTime.Now).TotalDays <= 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 客户端用户登录
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>

        [AllowAnonymous]
        [LogApiFilter(Type = LogType.Login, Name = "用户登录")]
        [HttpPost]
        public HttpResponseMessage PostLoginTest(LoginInfo inputDto)
        {
            //if (CheckLicense())
            //{
            //    HttpResponseMessage response1 = Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("系统授权已到期，请联系管理员！").ToMvcJson());
            //    return response1;
            //}
            inputDto.SessionId = SessionHelper.GetSessionId();
            inputDto.ClientIp = RequestHelper.GetClientIp();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.UserLogin(inputDto).ToMvcJson());
            return response;
        }

        //[LogApiFilter(Type = LogType.Login, Name = "用户注销")]
        //[HttpPost]
        //public HttpResponseMessage Postlogout()
        //{
        //    IdentityManager.SignOut(IdentityManager.Identity);       
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        [LogApiFilter(Type = LogType.Read, Name = "获取用户信息")]
        [HttpGet]

        public HttpResponseMessage GetUserInfo()
        {
            var values = Request.Headers.GetValues("token").ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.GetUserInfo(values.FirstOrDefault()).Data.ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Read, Name = "获取动态菜单")]
        [HttpGet]
        public HttpResponseMessage GetMenu()
        {
            var moduleList = AuthorizationContract.ModuleAuthOutputDto.Where(a => a.Enabled != false ).Select(a => new HP.Core.Functions.Module
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

        /// <summary>
        /// 验证是否有模块的权限
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        [LogApiFilter(Type = LogType.Operate, Name = "模块权限验证")]
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetCheckAuth(string moduleCode)
        {
            // 查询模块权限
            var moduleList = AuthorizationContract.ModuleAuthOutputDto.Where(a => a.Enabled != false && a.Code == moduleCode).ToList();
            if (moduleList.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success().ToMvcJson());
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure().ToMvcJson());
            }
        }
    }
}
