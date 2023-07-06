using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Core.Sequence;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    [Description("SMT上架管理")]
    public class ShelfController : BaseApiController
    {
        /// <summary>
        /// 标签信息
        /// </summary>
        public Bussiness.Contracts.SMT.IShelfContract ShelfContract { set; get; }

        public IRepository<Bussiness.Entitys.StockVM, int> StockVMRepository { get; set; }

        public Bussiness.Contracts.ILabelContract LabelContract { set; get; }

        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }

        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }

        public Bussiness.Contracts.ISupplyContract SupplyContract { set; get; }

        public ISequenceContract SequenceContract { set; get; }
        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取标签信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {

            var query = ShelfContract.WmsShelfMainVMRepository.Query();

            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.ReplenishCode.Contains(value));
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
        [LogFilter(Type = LogType.Operate, Name = "获取上架详情")]
        [HttpGet]
        public HttpResponseMessage GetShelfDetailList(string code)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ShelfContract.WmsShelfDetailRepository.Query().Where(a => a.ReplenishCode == code).ToList().ToMvcJson());
            return response;
        }
        [LogFilter(Type = LogType.Operate, Name = "获取标签信息")]
        [HttpGet]
        public HttpResponseMessage GetLabelInfoByLabel(string label)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, LabelContract.LabelRepository.Query().FirstOrDefault(a=>a.Code==label).ToMvcJson());
            return response;
        }
        [LogFilter(Type = LogType.Operate, Name = "WEB确认上架")]
        [HttpPost]
        public HttpResponseMessage WebConfirmShelf(Bussiness.Entitys.Label entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ShelfContract.WebConfirmShelf(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "WEB强制完成")]
        [HttpPost]
        public HttpResponseMessage CompelFinishedReplenishOrder(Bussiness.Entitys.SMT.WmsShelfMain entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ShelfContract.CompelFinishedReplenishOrder(entity.ReplenishCode).ToMvcJson());
            return response;
        }


        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetLocationList()
        {
            try
            { 
                var codeList = WareHouseContract.Locations.ToList();
                var stock = StockVMRepository.Query();
                var list = new ArrayList();
                // 筛选出空物料的库位
                foreach (var location in codeList)
                {
                    var locationCode = stock.Any(a => a.LocationCode == location.Code);
                    if (!locationCode)
                    {
                        list.Add(location);
                    }
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage();
            }

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

        [LogFilter(Type = LogType.Operate, Name = "获取供应商信息")]
        [HttpGet]
        public HttpResponseMessage GetSupplierList(string KeyValue)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, SupplyContract.Supplys.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue)).Take(20).ToList().ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "生产条码")]
        [HttpPost]
        public HttpResponseMessage GenerateLabel()
        {
            string label = SequenceContract.Create("Label");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, label.ToMvcJson());
            return response;
        }




        [LogFilter(Type = LogType.Operate, Name = "第一次扫描条码")]
        [HttpPost]
        public HttpResponseMessage FirstShelf(Bussiness.Entitys.Label label)
        {
           
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.ShelfContract.FirstShelf(label).ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "第一次扫描条码")]
        [HttpPost]
        public HttpResponseMessage PdaConfirmShelf(Bussiness.Entitys.Label label)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.ShelfContract.PdaConfirmShelf(label).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "第一次扫描条码")]
        [HttpPost]
        public HttpResponseMessage NextShelf(Bussiness.Entitys.Label label)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.ShelfContract.NextShelf(label).ToMvcJson());
            return response;
        }
        [LogFilter(Type = LogType.Operate, Name = "完成当前上架任务")]
        [HttpPost]
        public HttpResponseMessage FinishCurrentReplenish(Bussiness.Entitys.Label label)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.ShelfContract.FinishCurrentReplenish(label).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "跳过当前库位")]
        [HttpPost]
        public HttpResponseMessage SkipCurrentLightLocation(Bussiness.Entitys.Label label)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.ShelfContract.SkipCurrentLightLocation(label).ToMvcJson());
            return response;
        }
        #endregion

    }
}

