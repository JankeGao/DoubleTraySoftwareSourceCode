using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    [Description("属性组管理")]
    public class MaterialPropertyController : BaseApiController
    {
        /// <summary>
        /// 物料属性组信息
        /// </summary>
        public Bussiness.Contracts.IMaterialPropertyContract MaterialPropertyContract { set; get; }

        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取物料属性组信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = MaterialPropertyContract.MaterialPropertys;
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Name.Contains(value)|| p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var list = query.ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,list.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取物料属性组列表")]
        [HttpGet]
        public HttpResponseMessage GetMaterialPropertyList()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MaterialPropertyContract.MaterialPropertys.Where(a => a.Enabled).ToList().ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "创建物料属性组信息")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.MaterialProperty entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MaterialPropertyContract.CreateMaterialProperty(entity).ToMvcJson());
            return response;
        }
        [LogFilter(Type = LogType.Operate, Name = "编辑物料属性组信息")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(Bussiness.Entitys.MaterialProperty entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MaterialPropertyContract.EditMaterialProperty(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "删除物料属性组信息")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.MaterialProperty entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MaterialPropertyContract.DeleteMaterialProperty(entity.Id).ToMvcJson());
            return response;
        }

        #endregion

    }
}

