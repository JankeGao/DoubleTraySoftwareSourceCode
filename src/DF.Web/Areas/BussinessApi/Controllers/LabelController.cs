using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Dtos;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 标签管理
    /// </summary>
    [Description("标签管理")]
    public class LabelController : BaseApiController
    {
        /// <summary>
        /// 标签信息
        /// </summary>
        public Bussiness.Contracts.ILabelContract LabelContract { set; get; }

        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取标签信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = LabelContract.LabelDtos;
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "SupplyCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.SupplierCode.Contains(value) || p.SupplyName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var list = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,list.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取标签")]
        [HttpGet]
        public HttpResponseMessage GetLabelByCode(string code)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, LabelContract.LabelDtos.Where(a => a.Code == code).FirstOrDefault().ToMvcJson());
            return response;
        }
        [LogFilter(Type = LogType.Operate, Name = "创建标签信息")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.Label entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, LabelContract.CreateLabel(entity).ToMvcJson());
            return response;
        }
        [LogFilter(Type = LogType.Operate, Name = "编辑标签信息")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(Bussiness.Entitys.Label entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, LabelContract.EditLabel(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "删除标签信息")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.Label entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, LabelContract.DeleteLabel(entity.Id).ToMvcJson());
            return response;
        }
        #endregion

        /// <summary>
        /// 生成条码
        /// </summary>
        /// <param name="entityDto">入库物料行项目</param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "批量创建条码")]
        [HttpPost]
        public HttpResponseMessage PostDoCreateBatchLabel(LabelDto entityDto)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, LabelContract.CreateBatchLabel(entityDto).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 查询历史标签信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取历史标签")]
        [HttpPost]
        public HttpResponseMessage PostQueryHistoryLabel(Bussiness.Entitys.Label entity)
        {
            // 当天结束时间
            var end = entity.ManufactrueDate.Value.AddDays(1).AddSeconds(-1);
            //相同物料编码，相同生产日期，相同批次，相同供应商
            var list = LabelContract.LabelDtos.Where(a =>
                a.MaterialCode == entity.MaterialCode && a.ManufactrueDate >= entity.ManufactrueDate && a.ManufactrueDate <= end &&
                    a.BatchCode == entity.BatchCode && a.SupplierCode == entity.SupplierCode).OrderByDesc(a => a.CreatedTime).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }

    }
}

