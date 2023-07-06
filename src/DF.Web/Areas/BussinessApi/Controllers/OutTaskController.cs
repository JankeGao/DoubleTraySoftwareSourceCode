using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 出库任务管理 
    /// </summary>
    [Description("出库任务管理")]
    public class OutTaskController : BaseApiController
    {
        /// <summary>
        /// 出库数据库操作
        /// </summary>
        public Bussiness.Contracts.IOutTaskContract OutTaskContract { set; get; }

        /// <summary>
        /// 出库数据库操作
        /// </summary>
        public Bussiness.Contracts.IOutContract OutContract { set; get; }
        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }

        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }
        // 供应商管理接口
        public Bussiness.Contracts.ISupplyContract SupplyContract { set; get; }


        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取出库任务单信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = OutTaskContract.OutTaskDtos;
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Status");
            if (filterRule != null)
            {
                int value = Convert.ToInt32(filterRule.Value.ToString());
                query = query.Where(p => p.Status== value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "PdaStatus");
            if (filterRule != null)
            {
                int value = Convert.ToInt32(filterRule.Value.ToString());
                query = query.Where(p => p.Status < value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var list = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取出库任务物料信息
        /// </summary>
        /// <param name="OutTaskCode"></param>
        /// <returns></returns>
        //[LogFilter(Type = LogType.Operate, Name = "获取出库任务物料信息")]
        //[HttpGet]
        //public HttpResponseMessage GetOutTaskMaterialList(string OutTaskCode)
        //{
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.OutTaskMaterialLabelDtos.Where(a=>a.TaskCode== OutTaskCode).ToList().ToMvcJson());
        //    var va = OutTaskContract.OutTaskMaterialLabelDtos.Where(a => a.TaskCode == OutTaskCode).ToList();
        //    return response;
        //}

        /// <summary>
        /// 获取出库任务物料信息
        /// </summary>
        /// <param name="OutTaskCode"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取出库任务物料信息")]
        [HttpGet]
        public HttpResponseMessage GetOutTaskMaterialList(string OutTaskCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.OutTaskMaterialDtos.Where(a => a.OutTaskCode == OutTaskCode).ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取出库任务物料信息
        /// </summary>
        /// <param name="OutTaskCode"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取出库任务物料信息")]
        [HttpGet]
        public HttpResponseMessage GetOutTaskMaterialStatusList(string OutTaskCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.OutTaskMaterialDtos.Where(a => a.OutTaskCode == OutTaskCode && a.Status < 2).ToList().ToMvcJson());
            return response;
        }
        [LogFilter(Type = LogType.Operate, Name = "获取物料信息")]
        [HttpGet]
        public HttpResponseMessage GetMaterialList(string KeyValue)
        {
            var list = MaterialContract.Materials.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue));
            var aa = list.Take(20).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, aa.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取推荐物料信息
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取推荐物料信息")]
        [HttpPost]
        public HttpResponseMessage PostTaskMaterialLabelList(OutTaskMaterialDto entity)
        {
            var list = OutTaskContract.OutTaskMaterialLabelDtos.Where(a =>a.TaskCode== entity.OutTaskCode&&a.LocationCode== entity.SuggestLocation&&a.MaterialCode==entity.MaterialCode&&a.BatchCode==entity.BatchCode).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 创建出库任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建出库任务单")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.Out entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.CreateOutTaskEntity(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取仓库信息")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseList()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.WareHouses.ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除出库任务单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除出库任务单")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.OutTask entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.RemoveOutTask(entity.Id).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 手动执行下架
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //[LogFilter(Type = LogType.Operate, Name = "手动下架")]
        //[HttpPost]
        //public HttpResponseMessage ConfirmHandPicked(OutTaskMaterialLabelDto entity)
        //{
        //    HttpResponseMessage response =
        //        Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.ConfirmHandPicked(entity).ToMvcJson());
        //    return response;
        //}


        [LogFilter(Type = LogType.Operate, Name = "手动下架")]
        [HttpPost]
        public HttpResponseMessage ConfirmHandPicked(OutTaskMaterialDto entity)
        {
            HttpResponseMessage response =
                Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.ConfirmHandPicked(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 启动货架
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "启动货架")]
        [HttpPost]
        public HttpResponseMessage PostDoStartContrainer(OutTaskMaterialDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.HandStartContainer(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 存入货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "存入货架")]
        [HttpPost]
        public HttpResponseMessage PostRestoreContrainer(OutTaskMaterialDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.HandRestoreContainer(entity).ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取库位码信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取可存放的库位码信息-客户端")]
        [HttpPost]
        public HttpResponseMessage PostClientStockList(OutTaskMaterialLabelDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.CheckAvailableStock(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "发布上架任务")]
        [HttpPost]
        public HttpResponseMessage PostDoSendOrder(OutTask entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.SendOrderToPTL(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 客户端手动出库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "手动出库")]
        [HttpPost]
        public HttpResponseMessage PostManualOutList(OutTaskDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutTaskContract.HandPickClient(entity).ToMvcJson());
            return response;
        }

        #endregion



    }
}

