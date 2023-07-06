using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Data.Orm.Extensions;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HP.Utility.Files;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using HP.Web.Mvc.Utility;
using HP.Web.Mvc.Utility.Net;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;
using HPC.BaseService.Resources;

namespace DF.Web.Areas.BaseApi.Controllers
{
    [Description("用户管理")]
    public class UserController : BaseApiController
    {
        public IIdentityContract IdentityContract { get; set; }

        public IRepository<Bussiness.Entitys.UserSetting, int> UserSettingRepository { get; set; }
        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取用户信息")]
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetUserList([FromUri]MvcPageCondition pageCondition)
        {

            var query = IdentityContract.Users;

            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Name");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var userList = query.Where(a => !a.IsSystem).MapTo<UserOutputDto>().ToPage(pageCondition);
            foreach (var item in userList.Rows)
            {
                item.Role = IdentityContract.RoleUsersMaps
                    .InnerJoin(IdentityContract.Roles, (map, role) => map.RoleCode == role.Code)
                    .Select((map, role) => new { map, role })
                    .Where(a => a.map.UserCode == item.Code)
                    .Select(a => new Role
                    {
                        Id = a.role.Id,
                        Code = a.role.Code,
                        Name = a.role.Name,
                        Enabled = a.role.Enabled
                    })
                    .ToList();
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, userList.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetUserInfos()
        {
            return Request.CreateResponse(HttpStatusCode.OK, IdentityContract.Users.Where(a => !a.IsDeleted).ToList().ToMvcJson());
        }

        /// <summary>
        /// 提示获取用户信息
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取用户信息")]
        [HttpGet]
        public HttpResponseMessage GetUserlInfoList(string KeyValue)
        {
            var list = IdentityContract.Users.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue));
            var aa = list.Take(20).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, aa.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 根据编码获取用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public HttpResponseMessage GetUserInfo(string code)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.Users.FirstOrDefault(a => a.Code == code).ToMvcJson());
            return response;
        }



        #endregion

        //#region 创建

        [LogApiFilter(Type = LogType.Operate, Name = "用户创建")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(UserInputDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.CreateUser(entity).ToMvcJson());
            return response;
        }


        [LogApiFilter(Type = LogType.Operate, Name = "用户编辑")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(UserInputDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.EditUser(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 编辑管理区域
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public HttpResponseMessage PostDoEditUserArea(User entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.EditUserArea(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "用户个人中心编辑")]
        [HttpPost]
        public HttpResponseMessage PostDoEditUserCenter(UserInputDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.EditUser(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "用户移除")]
        [HttpPost]
        public HttpResponseMessage PostDoRemove(User entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.RemoveUser(entity.Id).ToMvcJson());
            return response;
        }

        [HttpPost]
        public HttpResponseMessage DoHeaderEdit(string code)
        {
            string header = null;
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            HttpResponseMessage response = Request.CreateResponse();
            if (files.Count > 0 && files[0].ContentLength > 0)
            {
                var file = files[0];
                var extensionName = Path.GetExtension(file.FileName);
                if (extensionName != ".jpg" && extensionName != ".jpeg")
                {
                    response = Request.CreateResponse(HttpStatusCode.OK,
                        DataProcess.Failure("请选择JPG图片！").ToMvcJson().ToMvcJson());
                }
                if (file.ContentLength / 1024 / 1024 > 2)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK,
                        DataProcess.Failure("请选择小于2M的图片！").ToMvcJson().ToMvcJson());
                }

                header = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string fileName =
                    FileHelper.GetAbsolutePath("{0}\\{1}".FormatWith(CatalogResource.Catalog_Header, header));
                file.SaveAs(fileName);
            }
            else
            {
                User entity = new User()
                {
                    Code = code,
                    Header = header
                };

                response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.EditUserHeader(entity).ToMvcJson());
            }

            return response;
        }


        [LogApiFilter(Type = LogType.Operate, Name = "获取用户信息")]
        public HttpResponseMessage GetUserInfoByRole(string role)
        {
            try
            {
                var query = IdentityContract.RoleUsersMaps.Where(a => a.RoleCode == role)
                    .InnerJoin(IdentityContract.Users,
                        (map, user) => map.UserCode == user.Code && !user.IsDeleted && user.Enabled)
                    .Select((map, user) => new UserOutputDto
                    {
                        Code = user.Code,
                        Name = user.Name,
                        Sex = user.Sex,
                        Role = new List<Role>()
                    }).ToList();
                foreach (var item in query)
                {
                    item.Role = IdentityContract.RoleUsersMaps
                        .InnerJoin(IdentityContract.Roles, (map, roleCode) => map.RoleCode == roleCode.Code)
                        .Select((map, roleCode) => new { map, roleCode })
                        .Where(a => a.map.UserCode == item.Code)
                        .Select(a => new Role
                        {
                            Id = a.roleCode.Id,
                            Code = a.roleCode.Code,
                            Name = a.roleCode.Name,
                            Enabled = a.roleCode.Enabled
                        })
                        .ToList();
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, query.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

