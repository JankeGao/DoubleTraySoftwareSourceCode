using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Contracts;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    [Description("拆盘管理")]
    public class SplitController : BaseApiController
    {
        /// <summary>
        /// 物料信息
        /// </summary>
        public Bussiness.Contracts.SMT.IPickContract PickContract { set; get; }

        public IWareHouseContract WareHouseContract { set; get; }

        public IMaterialContract MaterialContract { set; get; }

        public ISupplyContract IupplyContract { set; get; }

        public Bussiness.Contracts.ILabelContract LabelContract { get; set; }

        #region 首页



        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Read, Name = "获取拆盘点详情")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                var query = PickContract.WmsSplitMainRepository.Query();
                // 查询条件，根据用户名称查询
                HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.SplitNo.Contains(value) || p.PickOrderCode.Contains(value) );
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.WareHouseCode.Contains(value) || p.WareHouseCode.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }
                HP.Utility.Data.PageResult<Bussiness.Entitys.SMT.WmsSplitMain> list = query.ToPage(pageCondition);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [LogFilter(Type = LogType.Read, Name = "获取拆盘拣货单信息")]
        [HttpGet]
        public HttpResponseMessage GetSplitOrderIssueList(string SplitNo)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, PickContract.WmsSplitIssueVMRepository.Query().Where(a=>a.SplitNo == SplitNo).ToList().ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Read, Name = "获取拆盘区域详情")]
        [HttpGet]
        public HttpResponseMessage GetSplitAreaList(string SplitNo)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, PickContract.WmsSplitAreaRepository.Query().Where(a => a.SplitNo == SplitNo).ToList().ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Read, Name = "获取拆盘区域条码")]
        [HttpGet]
        public HttpResponseMessage GetSplitAreaReelList(string SplitNo,string WareHouseCode,string AreaId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, PickContract.WmsSplitAreaReelRepository.Query().Where(a => a.SplitNo == SplitNo && a.WareHouseCode==WareHouseCode && a.AreaId==AreaId).ToList().ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Read, Name = "获取拆盘条码详情")]
        [HttpGet]
        public HttpResponseMessage GetSplitAreaReelDetailList(string SplitNo,string WareHouseCode, string AreaId, string ReelId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, PickContract.WmsSplitAreaReelDetailRepository.Query().Where(a => a.SplitNo == SplitNo && a.WareHouseCode==WareHouseCode && a.AreaId==AreaId && a.OrgReelId ==ReelId).ToList().ToMvcJson());
            return response;
        }

        #endregion

        #region 操作
        [LogFilter(Type = LogType.Read, Name = "分配库存")]
        [HttpPost]
        public HttpResponseMessage DoCheckStock(List<Bussiness.Entitys.SMT.WmsPickOrderMain> list)
        {
            list = list.OrderBy(a => a.Index).ToList();
            List<string> PickOrderCodeList = new List<string>();
            foreach (var item in list)
            {
                PickOrderCodeList.Add(item.PickOrderCode);
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, PickContract.CheckPickOrder(PickOrderCodeList).ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Read, Name = "作废拣货单")]
        [HttpPost]
        public HttpResponseMessage DoCancel(Bussiness.Entitys.SMT.WmsPickOrderMain main)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, PickContract.DoCancel(main.PickOrderCode).ToMvcJson());
            return response;
        }
        #endregion


        /// <summary>
        /// 从WebAPI下载文件
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[AllowAnonymous]
        //public HttpResponseMessage DoDownLoadTemp()
        //{
        //    try
        //    {
        //        string path = "/Assets/themes/Excel/库存信息.xlsx";
        //        string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
        //        var stream = new FileStream(filePath, FileMode.Open);
        //        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new StreamContent(stream);
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = "库存信息.xlsx"
        //        };
        //        return response;
        //    }
        //    catch
        //    {
        //        return new HttpResponseMessage(HttpStatusCode.NoContent);
        //    }
        //}



        /// <summary>
        /// 导入库存信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //[LogFilter(Type = LogType.Operate, Name = "导入库存信息")]
        //[HttpPost]
        //public HttpResponseMessage DoUpLoadInInfo()
        //{
        //    HttpFileCollection files = HttpContext.Current.Request.Files;
        //    HttpPostedFile file = files[0];//取得第一个文件
        //    if (file == null)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请选择导入文件！").ToMvcJson());
        //    }

        //    var extensionName = System.IO.Path.GetExtension(file.FileName);
        //    if (extensionName != ".xls" && extensionName != ".xlsx")
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请导入Excel文件！").ToMvcJson());
        //    }

        //    try
        //    {
        //        var tb = Bussiness.Common.ExcelHelper.ReadExeclToDataTable("sheet1", 0, file.InputStream);
        //        if (tb.Rows.Count <= 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("Excel无内容").ToMvcJson());
        //        }
        //        int i = 0;
        //        foreach (DataRow item in tb.Rows)
        //        {
        //            Bussiness.Entitys.Stock entity = new Bussiness.Entitys.Stock();
        //            entity.LocationCode = item["库存地址"].ToString();
        //            if (WareHouseContract.Locations.FirstOrDefault(a => a.Code == entity.LocationCode) == null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("库位编码:" + entity.LocationCode + "系统不存在").ToMvcJson());
        //            }
        //            entity.LockedQuantity = 0;
        //            entity.ManufactureDate= item["生产日期"].ToString();
        //            entity.MaterialCode = item["物料编码"].ToString();
        //            if (MaterialContract.Materials.FirstOrDefault(a => a.Code == entity.MaterialCode) == null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("物料编码:" + entity.MaterialCode + "系统不存在").ToMvcJson());
        //            }
        //            entity.Quantity = Convert.ToDecimal(item["数量"].ToString());
        //            // 核查导入数量
        //            if (entity.Quantity <= 0)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请核对库存数量，物料：" + entity.MaterialCode).ToMvcJson());
        //            }
        //            entity.WareHouseCode = item["仓库编码"].ToString();
        //            if (WareHouseContract.WareHouses.FirstOrDefault(a => a.Code == entity.WareHouseCode) == null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("仓库编码:" + entity.LocationCode + "系统不存在").ToMvcJson());
        //            }
        //            entity.MaterialLabel = item["物料条码"].ToString();
        //            if (StockContract.Stocks.FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel) != null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("物料条码:" + entity.MaterialLabel + "库存中已存在").ToMvcJson());
        //            }
        //            entity.AreaCode = item["区域编码"].ToString();
        //            if (WareHouseContract.Areas.FirstOrDefault(a => a.Code == entity.AreaCode) == null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("区域编码:" + entity.AreaCode + "系统不存在").ToMvcJson());
        //            }
        //            entity.SupplierCode = item["供应商编码"].ToString();
        //            if (IupplyContract.Supplys.FirstOrDefault(a => a.Code == entity.SupplierCode) == null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("供应商编码:" + entity.SupplierCode + "系统不存在").ToMvcJson());
        //            }
        //            entity.MaterialStatus = 0;
        //            entity.StockStatus = 0;
        //            entity.BatchCode= item["批次"].ToString();
        //            StockContract.CreateStockEntity(entity);
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    catch (System.Exception ex)
        //    {

        //        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
        //    }
        //}


        ///// <summary>
        ///// 导出区域库位信息
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public HttpResponseMessage DoDownLoadLabelStock()
        //{

        //    var list = StockContract.StockDtos.ToList(); ;

        //    var divFields = new Dictionary<string, string>//显示的字段与名称
        //    {
        //        {"WareHouseCode","仓库编码"},
        //        {"WareHouseName","仓库名称"},
        //        {"AreaCode","区域编码"},
        //        {"AreaName","区域名称"},
        //        {"LocationCode","库位编码"},
        //        {"MaterialCode","物料编码"},
        //        {"MaterialName","物料名称"},
        //        {"MaterialLabel","物料条码"},
        //        {"Quantity","总数量"},
        //        {"LockedQuantity","锁定数量"},
        //        {"MaterialUnit","单位"},
        //        {"SupplierName","供应商名称"},
        //        {"SupplierCode","供应商编码"},
        //        {"BatchCode","批次"},
        //        {"ManufactureDate","生产日期"},
        //    };

        //    var fileName = "物料条码库存信息.xlsx"; //string.Format(@"库位信息{0}.xlsx", string.Format("{0:G}", DateTime.Now));
        //    var excelFile = Bussiness.Common.ExcelHelper.ListToExecl(list, fileName, divFields);
        //    MemoryStream ms = new MemoryStream();
        //    excelFile.Write(ms);
        //    ms.Seek(0, SeekOrigin.Begin);

        //    //获取导出文件流
        //    var stream = ms;
        //    if (stream == null)
        //    {
        //        return new HttpResponseMessage(HttpStatusCode.NoContent);
        //    }
        //    try
        //    {
        //        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //        result.Content = new StreamContent(stream);
        //        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
        //        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //        result.Content.Headers.ContentDisposition.FileName = $"物料条码库存信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
        //        return result;
        //    }
        //    catch
        //    {
        //        return new HttpResponseMessage(HttpStatusCode.NoContent);
        //    }
        //}

        ///// <summary>
        ///// 导出区域库位信息
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public HttpResponseMessage DoDownLoadMaterialStock()
        //{
        //    // var list = WareHouseContract.LocationVMs.Where(a => a.WareHouseCode == ware && a.AreaCode == area).ToList();

        //    var query = StockContract.StockDtos;
        //    var stockquery = StockContract.Stocks;
        //    var list = query
        //        .GroupBy(p => p.MaterialCode)
        //        .AndBy(p => p.MaterialName)
        //        .AndBy(p => p.WareHouseCode)
        //        .AndBy(p => p.WareHouseName)
        //        .AndBy(p => p.MaterialUnit)
        //        .Select(a => new StockDto()
        //        {
        //            MaterialCode = a.MaterialCode,
        //            MaterialName = a.MaterialName,
        //            WareHouseCode = a.WareHouseCode,
        //            WareHouseName = a.WareHouseName,
        //            MaterialUnit = a.MaterialUnit,
        //            Quantity = stockquery.Where(q => q.MaterialCode == a.MaterialCode && q.WareHouseCode == a.WareHouseCode).Sum(x => x.Quantity),
        //            LockedQuantity = stockquery.Where(q => q.MaterialCode == a.MaterialCode && q.WareHouseCode == a.WareHouseCode).Sum(x => x.LockedQuantity),
        //        }).ToList();


        //    var divFields = new Dictionary<string, string>//显示的字段与名称
        //    {
        //        {"WareHouseCode","仓库编码"},
        //        {"WareHouseName","仓库名称"},
        //        {"MaterialCode","物料编码"},
        //        {"MaterialName","物料名称"},
        //        {"Quantity","总数量"},
        //        {"LockedQuantity","锁定数量"},
        //        {"MaterialUnit","单位"},
        //    };

        //    var fileName = "物料库存信息.xlsx"; //string.Format(@"库位信息{0}.xlsx", string.Format("{0:G}", DateTime.Now));
        //    var excelFile = Bussiness.Common.ExcelHelper.ListToExecl(list, fileName, divFields);
        //    MemoryStream ms = new MemoryStream();
        //    excelFile.Write(ms);
        //    ms.Seek(0, SeekOrigin.Begin);

        //    //获取导出文件流
        //    var stream = ms;
        //    if (stream == null)
        //    {
        //        return new HttpResponseMessage(HttpStatusCode.NoContent);
        //    }
        //    try
        //    {
        //        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //        result.Content = new StreamContent(stream);
        //        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
        //        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //        result.Content.Headers.ContentDisposition.FileName = $"物料库存信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
        //        return result;
        //    }
        //    catch
        //    {
        //        return new HttpResponseMessage(HttpStatusCode.NoContent);
        //    }
        //}

        /// <summary>
        /// 获取拣货详情
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取拣货单明细")]
        [HttpGet]
        public HttpResponseMessage GetPickOrderDetailList([FromUri] MvcPageCondition pageCondition)
        {
            var query = this.PickContract.WmsPickOrderDetailRepository.Query();
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            // string pickOrderCode = filterRule.Value.ToString();
            //if (pageCondition.Sort == "Line_Id")
            //{
            //    string sort = pageCondition.Sort;
            //    string Order = pageCondition.Order;
            //    pageCondition.Sort = "Id";
            //    pageCondition.Order = "desc";

            //    var a = this.PickContract.WmsPickOrderDetailRepository.GetPageRecords(pageCondition);
            //    var detailList = this.PickContract.WmsPickOrderDetailRepository.SqlQuery("SELECT * FROM TB_WMS_PICK_ORDER_DETAIL WHERE PICKORDERCODE='" + pickOrderCode + "'");
            //    if (Order == "desc")
            //    {
            //        var list = detailList.OrderByDescending(A => A.OverQuantity).Take(a.PageSize).ToList();
            //        a.Rows = list;

            //    }
            //    else
            //    {
            //        var list = detailList.OrderBy(A => A.OverQuantity).Take(a.PageSize).ToList();
            //        a.Rows = list;
            //    }
            //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,
            //      a.ToMvcJson());
            //    return response;
            //}
            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,
            //a.ToMvcJson());
            //return response;
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.PickOrderCode == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            var list = query.ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;



        }
        /// <summary>
        /// 获取拣货单区域明细
        /// </summary>
        /// <param name="PickOrderDetailId"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取库位码信息")]
        [HttpGet]
        public HttpResponseMessage GetPickOrderAreaDetalList(string PickOrderDetailId)
        {
            //return RTJson(PickOrderService.WmsPickOrderAreaDetailRepository.Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE PICKORDERDETAILID='" + PickOrderDetailId + "'"));
            var list = this.PickContract.WmsPickOrderAreaDetailRepository.SqlQuery("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE PICKORDERDETAILID='" + PickOrderDetailId + "'");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 作废拆盘单
        /// </summary>
        /// <param name="SplitNo"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "作废拆盘单")]
        [HttpPost]

        public HttpResponseMessage CancelSplitOrder(Bussiness.Entitys.SMT.WmsShelfMain main)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.PickContract.CancelSplitOrder(main.SplitNo).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 区域列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "启动拆盘区域下架")]
        [HttpPost]

        public HttpResponseMessage SplitTaskDoStart(List<Bussiness.Entitys.SMT.WmsSplitArea> list)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.PickContract.SplitTaskDoStart(list).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 区域列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "结束拆盘区域下架")]
        [HttpPost]

        public HttpResponseMessage SplitTaskDoFinish(List<Bussiness.Entitys.SMT.WmsSplitArea> list)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.PickContract.SplitTaskDoFinish(list).ToMvcJson());
            return response;
        }



        [LogFilter(Type = LogType.Operate, Name = "获取拆盘条码")]
        [HttpGet]
        public HttpResponseMessage GetSplitReelList(string SplitNo)
        {
            var list = this.PickContract.WmsSplitAreaReelRepository.Query().Where(a => a.SplitNo == SplitNo).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "获取拆盘条码详情")]
        [HttpGet]
        public HttpResponseMessage GetSplitReelDetailList(string SplitNo,string ReelId)
        {
            var list = this.PickContract.WmsSplitAreaReelDetailRepository.Query().Where(a => a.SplitNo == SplitNo && a.OrgReelId==ReelId).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "确认拆盘条码")]
        [HttpPost]

        public HttpResponseMessage ConfirmSplitReel(Bussiness.Entitys.SMT.WmsSplitAreaReel wmsSplitArea)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.PickContract.ConfirmSplitReel(wmsSplitArea.ReelId, wmsSplitArea.SplitNo).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "作废拆盘条码")]
        [HttpPost]

        public HttpResponseMessage CancelSplitReel(Bussiness.Entitys.SMT.WmsSplitAreaReel wmsSplitArea)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.PickContract.CancelSplitReel(wmsSplitArea.ReelId, wmsSplitArea.SplitNo).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "拆盘上架")]
        [HttpPost]

        public HttpResponseMessage ConfirmShelfSplitReel(Bussiness.Entitys.SMT.WmsSplitAreaReelDetail splitAreaReelDetail)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.PickContract.WebConfirmShelfSplitReel(splitAreaReelDetail.SplitReelId, splitAreaReelDetail.SplitNo, splitAreaReelDetail.LocationCode).ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "获取拆盘Label信息")]
        [HttpGet]
        public HttpResponseMessage GetSplitReelDetailLabelInfo(string ReelId)
        {
            var label = LabelContract.LabelRepository.Query().FirstOrDefault(a => a.Code == ReelId);
            if (label==null)
            {
                label = new Bussiness.Entitys.Label();
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, label.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 0 带下架 1带分盘确认 2 待上架
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取待下架的拆盘但")]
        [HttpGet]
        public HttpResponseMessage GetSplitOrderList(int type)
        {
            var list = new List<Bussiness.Entitys.SMT.WmsSplitMain>();
            if (type==0)
            {
                list = this.PickContract.WmsSplitMainRepository.Query().Where(a => a.Status == 0 || a.Status == 1).OrderByDesc(a => a.CreatedTime).ToList();
            }
            if (type==1)
            {
                list = this.PickContract.WmsSplitMainRepository.Query().Where(a=>a.Status==2).OrderByDesc(a => a.CreatedTime).ToList();
            }
            if (type==2)
            {
                list = this.PickContract.WmsSplitMainRepository.Query().Where(a => a.Status == 3).OrderByDesc(a=>a.CreatedTime).ToList();
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }


        /// <summary>
        /// 0 带下架 1带分盘确认 2 待上架
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取待下架的拆盘但")]
        [HttpGet]
        public HttpResponseMessage GetReelIdByLocationCodeForSplit(string SplitNo, string ReelId, string LocationCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.PickContract.GetReelIdByLocationCodeForSplit(SplitNo,ReelId,LocationCode).ToMvcJson());
            return response;
        }

    }
}

