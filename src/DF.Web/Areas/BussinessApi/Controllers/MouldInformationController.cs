using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 模具信息
    /// </summary>
    [Description("模具信息的管理")]
    public class MouldInformationController : BaseApiController
    {
        /// <summary>
        ///
        /// </summary>
        public Bussiness.Contracts.IMouldInformationContract MouldInformationContract { set; get; }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取模具信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri] MvcPageCondition pageCondition)
        {        
            var query = MouldInformationContract.MouldInformationDtos.Where(a => a.IsDeleted == false);
            // 查询条件，根据编码称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialLabel");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.MaterialLabel.Contains(value) || p.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MouldState");
            if (filterRule != null)
            {
                int value = Convert.ToInt32(filterRule.Value.ToString());
                query = query.Where(p => p.MouldState == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.WareHouseCode == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            //以倒叙方式查询显示
            var proList = query.OrderByDesc(a => a.LastTimeReceiveDatetime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, proList.ToMvcJson());
            return response;
        }
        /// <summary>
        /// post方式-添加数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.MouldInformation entity)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MouldInformationContract.CreateMouldInformation(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 更改数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑模具信息")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(Bussiness.Entitys.MouldInformation entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MouldInformationContract.EditMouldInformation(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除模具信息")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.MouldInformation entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MouldInformationContract.DeleteMouldInformation(entity.Id).ToMvcJson());
            return response;
        }
    }
}