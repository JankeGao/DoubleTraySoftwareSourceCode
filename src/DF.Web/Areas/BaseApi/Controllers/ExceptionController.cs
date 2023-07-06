using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;


namespace DF.Web.Areas.BaseApi.Controllers
{
    [Description("异常管理")]
    public class ExceptionController : BaseApiController
    {
        public IExceptionContract ExceptionContract { set; get; }


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetExceptionList([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, ExceptionContract.Exceptions.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition).ToMvcJson());
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
