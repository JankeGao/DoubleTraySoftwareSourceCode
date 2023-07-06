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
    /// 入库任务管理
    /// </summary>
    [Description("入库任务管理")]
    public class InTaskController : BaseApiController
    {
        /// <summary>
        /// 入库数据库操作
        /// </summary>
        public Bussiness.Contracts.IInTaskContract InTaskContract { set; get; }
        /// <summary>
        /// 物料管理接口
        /// </summary>
        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }
        /// <summary>
        /// 仓库管理接口
        /// </summary>
        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }
        
        /// <summary>
        ///供应商管理接口
        /// </summary>
        public Bussiness.Contracts.ISupplyContract SupplyContract { set; get; }


        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取入库任务单信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = InTaskContract.InTaskDtos;
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
        /// 获取入库任务物料信息
        /// </summary>
        /// <param name="inTaskCode"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取入库任务物料信息")]
        [HttpGet]
        public HttpResponseMessage GetInTaskMaterialList(string inTaskCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.InTaskMaterialDtos.Where(a=>a.InTaskCode== inTaskCode).ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取入库任务物料信息
        /// </summary>
        /// <param name="inTaskCode"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取入库任务物料信息")]
        [HttpGet]
        public HttpResponseMessage GetInTaskMaterialStatusList(string inTaskCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.InTaskMaterialDtos.Where(a => a.InTaskCode == inTaskCode && a.Status < 2).ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取物料信息
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
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
        /// 创建入库任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建入库任务单")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.In entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.CreateInTaskEntity(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取仓库信息")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseList()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.WareHouses.ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除入库任务单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除入库任务单")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.InTask entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.RemoveInTask(entity.Id).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取库位码信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取可存放的库位码信息")]
        [HttpGet]
        public HttpResponseMessage GetLocationList([FromUri] Bussiness.Entitys.InTaskMaterial entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.CheckAvailableLocation(entity).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取库位码信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取可存放的库位码信息-客户端")]
        [HttpPost]
        public HttpResponseMessage PostClientLocationList(InTaskMaterialDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.CheckClientLocation(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 手动执行上架
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "上架")]
        [HttpPost]
        public HttpResponseMessage PostDoHandShelf(InTaskMaterialDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.HandShelf(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 启动货架
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "启动货架")]
        [HttpPost]
        public HttpResponseMessage PostDoStartContrainer(InTaskMaterialDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.HandStartContainer(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 存入货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "存入货架")]
        [HttpPost]
        public HttpResponseMessage PostRestoreContrainer(InTaskMaterialDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.HandRestoreContainer(entity).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "发布上架任务")]
        [HttpPost]
        public HttpResponseMessage PostDoSendOrder(InTask entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.SendOrderToPTL(entity).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 客户端手动上架
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "手动上架")]
        [HttpPost]
        public HttpResponseMessage PostManualInList(InTaskDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InTaskContract.HandShelfClient(entity).ToMvcJson());
            return response;
        }
        #endregion
    }
}

