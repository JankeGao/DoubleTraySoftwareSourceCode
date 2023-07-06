using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HPC.BaseService.Contracts;

namespace DF.Web.Areas.BaseApi.Controllers
{
    [Description("实体管理")]
    public class EntityInfoController : BaseApiController
    {
        public IEntityInfoContract EntityInfoContract { set; get; }

        /// <summary>
        /// 查询启用角色信息
        /// </summary>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetEntityInfo()
        {
            return Request.CreateResponse(HttpStatusCode.OK, EntityInfoContract.EntityInfos.ToList().ToMvcJson());
        }
    }
}
