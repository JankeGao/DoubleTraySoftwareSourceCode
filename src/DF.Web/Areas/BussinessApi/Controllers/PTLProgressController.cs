using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Utility.Data;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    [Description("PTL任务进度查看")]
    public class PTLProgressController : BaseApiController
    {
        /// <summary>
        /// 物料信息
        /// </summary>
        /// 
        public Bussiness.Contracts.SMT.IPickContract PickContract { get; set; }

        public Bussiness.Contracts.SMT.IShelfContract ShelfContract { get; set; }
        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }

        public Bussiness.Contracts.SMT.IStockLightContract StockLightContract { set; get; }


        public IRepository<Bussiness.Entitys.PTL.DpsInterfaceMain, int> DpsInterfaceMainRepository { get; set; }

        public IRepository<Bussiness.Entitys.PTL.DpsInterface, int> DpsInterfaceRepository { get; set; }

        public IRepository<Bussiness.Entitys.PTL.DpsInterfaceVM, int> DpsInterfaceVMRepository { get; set; }

        public Bussiness.Contracts.ICheckContract CheckContract { get; set; }

        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取区域并查看任务详情")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = WareHouseContract.Areas;
            // 查询条件，根据用户名称查询
            //var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            //if (filterRule != null)
            //{
            //    string value = filterRule.Value.ToString();
            //    query = query.Where(p => p.WareHouseCode.Contains(value)|| p.WareHouseCode.Contains(value));
            //    pageCondition.FilterRuleCondition.Remove(filterRule);

            //}
            //filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Remark");
            //if (filterRule != null)
            //{
            //    string value = filterRule.Value.ToString();
            //    query = query.Where(p => p.Remark.Contains(value));
            //    pageCondition.FilterRuleCondition.Remove(filterRule);

            //}
            var list = query.OrderBy(a=>a.Code).ToPage(pageCondition);

            foreach (var item in list.Rows)
            {
                var pickOrder = this.PickContract.WmsPickOrderAreaRepository.Query().FirstOrDefault(a => a.WareHouseCode == item.WareHouseCode && a.AreaId == item.Code && a.Status == 1);
                if (pickOrder!=null)
                {
                    item.OrderCode = pickOrder.PickOrderCode;
                    item.OrderType = 0;
                    item.OrderTypeName = "拣货单";
                    item.IsExistOrder = true;
                    item.OrderCount = this.PickContract.WmsPickOrderAreaDetailRepository.GetCount(a => a.PickOrderCode == pickOrder.PickOrderCode && a.AreaId == item.Code && a.Status != 2);
                    item.FinishedOrderCount = this.PickContract.WmsPickOrderAreaDetailRepository.GetCount(a => a.PickOrderCode == pickOrder.PickOrderCode && a.AreaId == item.Code && a.Status > 2);
                    item.OrderProgress =Convert.ToInt32( Math.Floor(((double)item.FinishedOrderCount / (double)item.OrderCount) * 100));
                }

                var splitOrder = this.PickContract. WmsSplitAreaRepository.Query().FirstOrDefault(a => a.WareHouseCode == item.WareHouseCode && a.AreaId == item.Code && a.Status == 1);
                if (splitOrder!=null)
                {
                    item.OrderCode = splitOrder.SplitNo;
                    item.OrderTypeName = "拆盘单";
                    item.OrderType = 1;
                    item.IsExistOrder = true;
                    item.OrderCount = this.PickContract.WmsSplitAreaReelRepository.GetCount(a => a.SplitNo == splitOrder.SplitNo && a.AreaId == item.Code);
                    item.FinishedOrderCount = this.PickContract.WmsSplitAreaReelRepository.GetCount(a => a.SplitNo == splitOrder.SplitNo && a.AreaId == item.Code && a.Status > 1);
                    item.OrderProgress = Convert.ToInt32(Math.Floor(((double)item.FinishedOrderCount / (double)item.OrderCount) * 100));
                }

                var shelfOrder = this.ShelfContract.WmsShelfMainRepository.Query().FirstOrDefault(a => a.AreaId == item.Code && a.WareHouseCode == item.WareHouseCode && a.Status == 0);
                if (shelfOrder!=null)
                {
                    item.OrderCode = shelfOrder.ReplenishCode;
                    item.OrderTypeName = "补料单";
                    item.OrderType = 2;
                    item.IsExistOrder = true;
                    item.OrderCount = this.ShelfContract.WmsShelfDetailRepository.GetCount(a => a.ReplenishCode == shelfOrder.ReplenishCode);
                    item.FinishedOrderCount = this.ShelfContract.WmsShelfDetailRepository.GetCount(a => a.ReplenishCode == shelfOrder.ReplenishCode && a.Status >= 3);
                    item.OrderProgress = Convert.ToInt32(Math.Floor(((double)item.FinishedOrderCount / (double)item.OrderCount) * 100));
                }

                var stockLightOrder = this.StockLightContract.WmsReelLightAreaRepository.Query().FirstOrDefault(a => a.AreaId == item.Code && a.WareHouseCode == item.WareHouseCode && a.Status == 0);
                if (stockLightOrder != null)
                {
                    item.OrderCode = stockLightOrder.LightCode;
                    item.OrderTypeName = "库存点亮任务";
                    item.OrderType = 3;
                    item.IsExistOrder = true;
                    item.OrderCount = this.StockLightContract.WmsReelLightAreaDetailRepository.GetCount(a => a.LightCode == stockLightOrder.LightCode && a.AreaId==item.Code);
                    item.FinishedOrderCount = this.StockLightContract.WmsReelLightAreaDetailRepository.GetCount(a => a.LightCode == stockLightOrder.LightCode && a.AreaId == item.Code && a.Status==1);
                    item.OrderProgress = Convert.ToInt32(Math.Floor(((double)item.FinishedOrderCount / (double)item.OrderCount) * 100));
                }

                var checkAreaOrder = this.CheckContract.CheckAreaRepository.Query().FirstOrDefault(a => a.AreaCode == item.Code && a.WareHouseCode == item.WareHouseCode && a.Status == 3);

                if (checkAreaOrder != null)
                {
                    item.OrderCode = checkAreaOrder.CheckCode;
                    item.OrderTypeName = "库存盘点任务";
                    item.OrderType = 4;
                    item.IsExistOrder = true;
                    item.OrderCount = this.CheckContract.CheckDetailRepository.GetCount(a => a.CheckCode == checkAreaOrder.CheckCode && a.AreaCode == item.Code);
                    item.FinishedOrderCount = this.CheckContract.CheckDetailRepository.GetCount(a => a.CheckCode == checkAreaOrder.CheckCode && a.AreaCode == item.Code && a.Status >3 && a.Status!=5);
                    item.OrderProgress = Convert.ToInt32(Math.Floor(((double)item.FinishedOrderCount / (double)item.OrderCount) * 100));
                }

            }

            //list.Rows
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,list.ToMvcJson());
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
            var list = this.WareHouseContract.WareHouses.ToList();
            //list.Rows
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }



        [LogFilter(Type = LogType.Operate, Name = "手动完成任务")]
        [HttpPost]
        public HttpResponseMessage HandFinishOrder(Bussiness.Entitys.Area area)
        {
            DataResult dataResult = DataProcess.Success();
            if (area.IsExistOrder==true)
            {
                if (area.OrderType==0)
                {
                    // this.PickContract.PickTaskDoFinish()
                    var list = this.PickContract.WmsPickOrderAreaRepository.Query().Where(a => a.PickOrderCode == area.OrderCode && a.AreaId == area.Code).ToList();
                    dataResult=  this.PickContract.PickTaskDoFinish(list);
                }
                else if(area.OrderType==1)
                {
                    var list = this.PickContract.WmsSplitAreaRepository.Query().Where(a => a.SplitNo == area.OrderCode && a.AreaId == area.Code).ToList();
                    dataResult = this.PickContract.SplitTaskDoFinish(list);
                }
                else if (area.OrderType==2)
                {
                    dataResult = ShelfContract.CompelFinishedReplenishOrder(area.OrderCode);
                }
                else if(area.OrderType == 3)
                {
                    dataResult = StockLightContract.OffAreaLight(area.OrderCode, area.Code);
                }
                else
                {
                    var list = this.CheckContract.CheckAreaRepository.Query().Where(a => a.CheckCode == area.OrderCode && a.AreaCode == area.Code).ToList();
                    dataResult = this.CheckContract.CheckTaskDoFinish(list);
                }
            }

            //var list = this.WareHouseContract.WareHouses.ToList();
            //list.Rows
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, dataResult.ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "手动完成单个任务")]
        [HttpPost]
        public HttpResponseMessage HandFinishOneLocation(Bussiness.Entitys.PTL.DpsInterfaceVM dpsInterfaceVM)
        {
            DataResult dataResult = DataProcess.Success();
         //   if (area.IsExistOrder == true)
            {
                Bussiness.PtlServer.PtlServer ptlServer = new Bussiness.PtlServer.PtlServer();
                //var dps = DpsInterfaceRepository.Query().FirstOrDefault(a => a.RelationId == area.Id);
                //if (dps != null)
                //{
                //    var result= ptlServer.FinishOneLocation(area.Code, dps.OperateId, dps.Quantity.GetValueOrDefault(0), true);
                //    dataResult.Success = result.Success;
                //    dataResult.Message = result.Message;
                //}
                dpsInterfaceVM.UserId = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
                var result = ptlServer.FinishOneLocation("", dpsInterfaceVM.OperateId, dpsInterfaceVM.RealQuantity.GetValueOrDefault(-1), true,"");
                dataResult.Success = result.Success;
                dataResult.Message = result.Message;
            }

            //var list = this.WareHouseContract.WareHouses.ToList();
            //list.Rows
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, dataResult.ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "手动停止任务")]
        [HttpPost]
        public HttpResponseMessage HandStopOrder(Bussiness.Entitys.Area area)
        {
            DataResult dataResult = DataProcess.Success();
            if (area.IsExistOrder == true)
            {
                Bussiness.PtlServer.PtlServer ptlServer = new Bussiness.PtlServer.PtlServer();
                if (area.OrderType == 0)
                {
                   var entity = this.PickContract.WmsPickOrderAreaRepository.Query().FirstOrDefault(a => a.PickOrderCode == area.OrderCode && a.AreaId == area.Code);
                    if (entity!=null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        ptlServer.StopAreaOrder(area.Code, entity.ProofId);
                    }

                }
                else if (area.OrderType == 1)
                {
                    var entity = this.PickContract.WmsSplitAreaRepository.Query().FirstOrDefault(a => a.SplitNo == area.OrderCode && a.AreaId == area.Code);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        ptlServer.StopAreaOrder(area.Code, entity.ProofId);
                    }
                }
                else if (area.OrderType == 2)
                {
                    var entity = this.ShelfContract.WmsShelfDetailRepository.Query().FirstOrDefault(a => a.ReplenishCode == area.OrderCode && a.Status==0);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        ptlServer.StopAreaOrder(area.Code, entity.ProofId);
                    }
                }
                else
                {
                    var entity = this.StockLightContract.WmsReelLightAreaRepository.Query().FirstOrDefault(a => a.LightCode == area.OrderCode && a.AreaId == area.Code);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        ptlServer.StopAreaOrder(area.Code, entity.ProofId);
                    }
                }
            }

            //var list = this.WareHouseContract.WareHouses.ToList();
            //list.Rows
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, dataResult.ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "手动恢复任务")]
        [HttpPost]
        public HttpResponseMessage HandContinueOrder(Bussiness.Entitys.Area area)
        {
            DataResult dataResult = DataProcess.Success();
            if (area.IsExistOrder == true)
            {
                Bussiness.PtlServer.PtlServer ptlServer = new Bussiness.PtlServer.PtlServer();
                if (area.OrderType == 0)
                {
                    var entity = this.PickContract.WmsPickOrderAreaRepository.Query().FirstOrDefault(a => a.PickOrderCode == area.OrderCode && a.AreaId == area.Code);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        ptlServer.ContinueAreaOrder(area.Code, entity.ProofId);
                    }

                }
                else if (area.OrderType == 1)
                {
                    var entity = this.PickContract.WmsSplitAreaRepository.Query().FirstOrDefault(a => a.SplitNo == area.OrderCode && a.AreaId == area.Code);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        ptlServer.ContinueAreaOrder(area.Code, entity.ProofId);
                    }
                }
                else if (area.OrderType == 2)
                {
                    var entity = this.ShelfContract.WmsShelfDetailRepository.Query().FirstOrDefault(a => a.ReplenishCode == area.OrderCode && a.Status == 0);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        ptlServer.ContinueAreaOrder(area.Code, entity.ProofId);
                    }
                }
                else
                {
                    var entity = this.StockLightContract.WmsReelLightAreaRepository.Query().FirstOrDefault(a => a.LightCode == area.OrderCode && a.AreaId == area.Code);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        ptlServer.ContinueAreaOrder(area.Code, entity.ProofId);
                    }
                }
            }

            //var list = this.WareHouseContract.WareHouses.ToList();
            //list.Rows
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, dataResult.ToMvcJson());
            return response;
        }


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取区域并查看任务详情")]
        [HttpGet]
        public HttpResponseMessage GetDetailList([FromUri]MvcPageCondition pageCondition)
        {
            // var list = new List<Bussiness.Entitys.PTL.DpsInterfaceVM>();
            var query = DpsInterfaceVMRepository.Query();


            Bussiness.Entitys.Area area = new Area();

            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "IsExistOrder");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                area.IsExistOrder = Convert.ToBoolean(value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var  OrderFilterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "OrderType");
            if (OrderFilterRule != null)
            {
                string value = OrderFilterRule.Value.ToString();
                area.OrderType = Convert.ToInt32(value);
                pageCondition.FilterRuleCondition.Remove(OrderFilterRule);

            }

            var OrderCodeFilterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "OrderCode");
            if (OrderCodeFilterRule!=null)
            {
                string value = OrderCodeFilterRule.Value.ToString();
                area.OrderCode = value;
                pageCondition.FilterRuleCondition.Remove(OrderCodeFilterRule);
            }

            var CodeFilterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (CodeFilterRule != null)
            {
                string value = CodeFilterRule.Value.ToString();
                area.Code = value;
                pageCondition.FilterRuleCondition.Remove(CodeFilterRule);
            }

            if (area.IsExistOrder == true)
            {
                Bussiness.PtlServer.PtlServer ptlServer = new Bussiness.PtlServer.PtlServer();
                if (area.OrderType == 0)
                {
                    var entity = this.PickContract.WmsPickOrderAreaRepository.Query().FirstOrDefault(a => a.PickOrderCode == area.OrderCode && a.AreaId == area.Code);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {

                        query = query.Where(a => a.ProofId == entity.ProofId);
                    }

                }
                else if (area.OrderType == 1)
                {
                    var entity = this.PickContract.WmsSplitAreaRepository.Query().FirstOrDefault(a => a.SplitNo == area.OrderCode && a.AreaId == area.Code);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        query = query.Where(a => a.ProofId == entity.ProofId);
                    }
                }
                else if (area.OrderType == 2)
                {
                    var detailList = this.ShelfContract.WmsShelfDetailRepository.Query().Where(a => a.ReplenishCode == area.OrderCode).ToList();
                    List<string> strList = new List<string>();
                    foreach (var item in detailList)
                    {
                        strList.Add(item.ProofId);
                    }
                    if (strList.Count > 0)
                    {
                        query = query.Where(a => a.OrderCode == area.OrderCode && strList.Contains(a.ProofId));
                    }
                    else
                    {
                        query = query.Where(a => a.OrderCode == area.OrderCode);
                    }

                }
                else if(area.OrderType==3)
                {
                    var entity = this.StockLightContract.WmsReelLightAreaRepository.Query().FirstOrDefault(a => a.LightCode == area.OrderCode && a.AreaId == area.Code);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        query = query.Where(a => a.ProofId == entity.ProofId);
                    }
                }
                else
                {
                    var entity = this.CheckContract.CheckAreaRepository.Query().FirstOrDefault(a => a.CheckCode == area.OrderCode && a.AreaCode == area.Code);
                    if (entity != null && !string.IsNullOrEmpty(entity.ProofId))
                    {
                        query = query.Where(a => a.ProofId == entity.ProofId);
                    }
                }

                //if (area.Status!=null)
                //{
                //    query = query.Where(a => a.Status == area.Status);
                //}
            }
            else
            {
                return null;
            }
            //var list = this.WareHouseContract.WareHouses.ToList();
            //list.Rows

            var list = query.OrderByDesc(a => a.GoodsName).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }


        #endregion

    }
}

