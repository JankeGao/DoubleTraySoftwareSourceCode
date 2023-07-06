using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Dtos;
using HP.Core.Logging;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;
using HP.Data.Entity.Pagination;
using HP.Data.Orm.Extensions;
using HPC.BaseService.Dtos;

namespace DF.Web.Areas.BaseApi.Controllers
{
    /// <summary>
    /// 部门管理
    /// </summary>
    [Description("部门管理")]
    public class DepartmentController : BaseApiController
    {
        public IDepartmentContract DepartmentContract { set; get; }


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取库位信息")]
        [HttpGet]
        public HttpResponseMessage GetDepartmentTreeData()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DepartmentContract.DepartmentTree.ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取部门信息")]
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetList([FromUri]MvcPageCondition pageCondition)
        {
            var a = DepartmentContract.DepartmentTree.ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DepartmentContract.InitModuleFunctionTree(DepartmentContract.DepartmentTree.ToList(), "").ToMvcJson());
            return response;
        }

        #region 创建

        //[Description("创建")]
        //public HttpResponseMessage Create(string organizationCode,string code)
        //{
        //    ViewBag.OrganizationCode = organizationCode;
        //    ViewBag.ParentCode = code;
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, IdentityContract.CreateRole(entity).ToMvcJson());
        //    return response;
        //}

        [System.Web.Mvc.HttpPost]
        [AuthorizationFilter(ActionName = "Create")]
        [LogFilter(Name = "部门创建")]
        public HttpResponseMessage PostDoCreate(Department entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DepartmentContract.CreateDepartment(entity).ToMvcJson());
            return response;
        }

        #endregion

        #region 编辑

        [Description("编辑")]
        public HttpResponseMessage Edit(int id)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DepartmentContract.Departments.FirstOrDefault(a => a.Id == id).ToMvcJson());
            return response;
        }

        [System.Web.Mvc.HttpPost]
        [AuthorizationFilter(ActionName = "Edit")]
        [LogFilter(Name = "部门编辑")]
        public HttpResponseMessage PostDoEdit(Department entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DepartmentContract.EditDepartment(entity).ToMvcJson());
            return response;
        }

        #endregion

        #region 移除

        [System.Web.Mvc.HttpPost]
        [Description("移除")]
        [AuthorizationFilter(ActionName = "Remove")]
        [LogFilter(Name = "部门移除")]
        public HttpResponseMessage PostDoRemove(Department entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DepartmentContract.RemoveDepartment(entity.Id).ToMvcJson());
            return response;
        }

        #endregion

        #region 查询

        //[Description("获取部门树")]
        //public HttpResponseMessage GetTree(MvcPageCondition pageCondition)
        //{
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DepartmentContract.GetTree(pageCondition).ToMvcJson());
        //    return response;
        //}
        #endregion
    }
}
