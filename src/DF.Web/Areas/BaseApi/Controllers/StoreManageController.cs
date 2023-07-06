using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Data.Entity.Pagination;
using HP.OrgService.Contracts;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Pagination;

namespace Hellopets.Web.Areas.OpenApi.Controllers
{
    [Description("门店管理")]
    public class StoreManageController : BaseApiController
    {
        public IOrganizationContract OrganizationContract { set; get; }

        #region 首页

        #endregion



        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetStoreInfoList([FromUri]MvcPageCondition pageCondition)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OrganizationContract.OrganizationTree.ToPage(pageCondition).ToMvcJson());
            return response;
        }

        //#region 创建

        //[Description("创建")]
        //public ActionResult Create(string code)
        //{
        //    ViewBag.ParentCode = code;
        //    return View();
        //}

        //[AuthorizationFilter(ActionName = "Create")]
        //[LogFilter(Name = "公司创建")]
        //public ActionResult DoCreate(Organization entity)
        //{
        //    return OrganizationContract.CreateOrganization(entity).ToMvcJson();
        //}

        //#endregion

        //#region 编辑

        //[Description("编辑")]
        //public ActionResult Edit(int id)
        //{
        //    return View(OrganizationContract.Organizations.Where(a => a.Id == id).FirstOrDefault());
        //}

        //[AuthorizationFilter(ActionName = "Edit")]
        //[LogFilter(Name = "公司编辑")]
        //public ActionResult DoEdit(Organization entity)
        //{
        //    return OrganizationContract.EditOrganization(entity).ToMvcJson();
        //}

        //#endregion

        //#region 移除

        //[Description("移除")]
        //[AuthorizationFilter(ActionName = "Remove")]
        //[LogFilter(Name = "公司移除")]
        //public ActionResult DoRemove(int id)
        //{
        //    return OrganizationContract.RemoveOrganization(id).ToMvcJson();
        //}

        //#endregion

        //#region 查询

        ///// <summary>
        ///// 获取机构树
        ///// </summary>
        ///// <param name="pageCondition"></param>
        ///// <returns></returns>
        //[Description("获取组织机构树")]
        //public ActionResult GetTree(PageCondition pageCondition)
        //{
        //    OrganizationContract.OrganizationTree.Where(pageCondition).ToList().ToMvcJson();

        //    return OrganizationContract.GetTree(pageCondition).ToMvcJson();
        //}

        //#endregion
    }
}