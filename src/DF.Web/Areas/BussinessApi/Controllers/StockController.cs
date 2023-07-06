using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using Bussiness.Services;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Core.Sequence;
using HP.Data.Entity.Pagination;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Filter;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;
using Newtonsoft.Json;
using NPOI.SS.Util;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    [Description("库存查看")]
    public class StockController : BaseApiController
    {
        /// <summary>
        /// 物料信息
        /// </summary>
        public IStockContract StockContract { set; get; }

        public IDictionaryContract DictionaryContract { set; get; }
        public IWareHouseContract WareHouseContract { set; get; }

        public IRepository<Location, int> LocationDetailRepository { get; set; }


        public IMaterialContract MaterialContract { set; get; }

        public ISupplyContract IupplyContract { set; get; }

        public ISequenceContract SequenceContract { get; set; }

        public IIdentityContract IdentityContract { get; set; }

        #region 首页


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取物料库存信息")]
        [HttpGet]
        public HttpResponseMessage GetMaterialPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                if (!StockContract.CheckOldStock().Success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, StockContract.CheckOldStock().ToMvcJson());
                }
                List<int?> trayIdList = new List<int?>();
                List<string> containerCodeList = new List<string>();
                var identity = HP.Core.Security.Permissions.IdentityManager.Identity.UserData;
                var currentUser = IdentityContract.Users.FirstOrDefault(a => a.Code == identity.Code);
                if (!identity.IsSystem)
                {
                    if (!string.IsNullOrEmpty(currentUser.StockArea))
                    {
                        var trayList = currentUser.StockArea.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList();
                        foreach (var item in trayList)
                        {
                            trayIdList.Add(Convert.ToInt32(item.Split('-')[1]));
                        }
                    }
                }
                if (trayIdList == null)
                {
                    trayIdList = new List<int?>();
                }
                var query = StockContract.StockDtos;
                if (!currentUser.IsSystem)
                {
                    query = query.Where(a => trayIdList.Contains(a.TrayId));
                }
                // 查询条件，根据用户名称查询
                HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.WareHouseName.Contains(value) || p.WareHouseCode.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "ContainerCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.ContainerCode == value);
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "AreaArray");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    List<int?> areaList = new List<int?>();
                    foreach (var item in value.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList())
                    {
                        areaList.Add(Convert.ToInt32(item));
                    }
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                    query = query.Where(a => areaList.Contains(a.TrayId));
                }
                var stockquery = StockContract.Stocks;
                var materailquery = query
                    .GroupBy(p => p.MaterialCode)
                    .AndBy(p => p.MaterialName)
                    .AndBy(p => p.WareHouseCode)
                    .AndBy(p => p.WareHouseName)
                    .AndBy(p => p.MaterialUnit)
                    .AndBy(p=>p.ContainerCode)
                    .AndBy(p=>p.Price)
                    .AndBy(p=>p.Use)
                    .AndBy(p => p.LocationCode)
                    .AndBy(p => p.ShelfTime)
                    .Select(a => new StockDto()
                    {
                        MaterialCode = a.MaterialCode,
                        MaterialName = a.MaterialName,
                        WareHouseCode = a.WareHouseCode,
                        WareHouseName=a.WareHouseName,
                        MaterialUnit=a.MaterialUnit,
                        ContainerCode = a.ContainerCode,
                        Price = a.Price,
                        Use = a.Use,
                        LocationCode = a.LocationCode,
                        ShelfTime = a.ShelfTime,
                        Quantity = stockquery.Where(q=>q.MaterialCode==a.MaterialCode&&q.WareHouseCode==a.WareHouseCode && q.ContainerCode == a.ContainerCode).Sum(x=>x.Quantity),
                        LockedQuantity= stockquery.Where(q => q.MaterialCode == a.MaterialCode && q.WareHouseCode == a.WareHouseCode && q.ContainerCode == a.ContainerCode).Sum(x => x.LockedQuantity),
                    });


                HP.Utility.Data.PageResult<Bussiness.Dtos.StockDto> list = materailquery.ToPage(pageCondition);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取库存信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                //if (!StockContract.CheckOldStock().Success)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK, StockContract.CheckOldStock().ToMvcJson());
                //}

                List<int?> trayIdList = new List<int?>();
                List<string> containerCodeList = new List<string>();
                var identity = HP.Core.Security.Permissions.IdentityManager.Identity.UserData;
                var currentUser = IdentityContract.Users.FirstOrDefault(a => a.Code == identity.Code);
                if (!identity.IsSystem)
                {
                    if (!string.IsNullOrEmpty(currentUser.StockArea))
                    {
                        var trayList = currentUser.StockArea.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList();
                        foreach (var item in trayList)
                        {
                            trayIdList.Add(Convert.ToInt32(item.Split('-')[1]));
                        }
                    }
                }
                if (trayIdList == null)
                {
                    trayIdList = new List<int?>();
                }
                var query = StockContract.StockDtos;
                if (!currentUser.IsSystem)
                {
                    query = query.Where(a => trayIdList.Contains(a.TrayId));
                }
                // 查询条件，根据用户名称查询
                HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.WareHouseName.Contains(value) || p.WareHouseCode.Contains(value) );
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "ContainerCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.ContainerCode == value);
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }

                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "TrayCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.TrayCode == value);
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "SupplierCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.SupplierCode.Contains(value) || p.SupplierName.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "LocationCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.LocationCode.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "AreaArray");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    List<int?> areaList = new List<int?>();
                    foreach (var item in value.Split(',').Where(a=>!string.IsNullOrEmpty(a)).ToList())
                    {
                        areaList.Add(Convert.ToInt32(item));
                    }
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                    query = query.Where(a => areaList.Contains(a.TrayId));
                }
                var begin = pageCondition.FilterRuleCondition.Find(a => a.Field == "begin");
                var end = pageCondition.FilterRuleCondition.Find(a => a.Field == "end");
                if (begin != null && end != null)
                {
                    var value1 = Convert.ToDateTime(begin.Value.ToString());
                    var value2 = Convert.ToDateTime(end.Value.ToString());
                    query = query.Where(p => p.CreatedTime >= value1 && p.CreatedTime <= value2);
                    pageCondition.FilterRuleCondition.Remove(begin);
                    pageCondition.FilterRuleCondition.Remove(end);
                }

                HP.Utility.Data.PageResult<Bussiness.Dtos.StockDto> list = query.ToPage(pageCondition);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [LogFilter(Type = LogType.Operate, Name = "获取库存信息")]
        [HttpGet]
        public HttpResponseMessage GetStockLabel(string code)
        {
            var b = StockContract.StockDtos.Where(a => a.MaterialLabel == code).FirstOrDefault();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, StockContract.StockDtos.Where(a => a.MaterialLabel == code).FirstOrDefault().ToMvcJson());
            return response;
        }

        #endregion


        /// <summary>
        /// 获取库位库存信息-设备信息，可视化部分
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取库位库存信息")]
        [HttpGet]
        public HttpResponseMessage GetLocationStockByLayoutId(string layoutId)
        {
            try
            {
                var returnList = new List<StockDto>();
                if (StockContract.StockDtos.Any(a => a.LayoutId == layoutId))
                {
                    var stock = StockContract.StockDtos.FirstOrDefault(a =>
                        a.LayoutId == layoutId);
          
                    if (MaterialContract.MaterialBoxMaps.Any(a =>
                        a.BoxCode == stock.BoxCode && a.MaterialCode == stock.MaterialCode))
                    {
                        var box = MaterialContract.MaterialBoxMaps.FirstOrDefault(a =>
                            a.BoxCode == stock.BoxCode && a.MaterialCode == stock.MaterialCode);
                        stock.BoxMaxCount = box.BoxCount;
                        returnList.Add(stock);
                    }
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, returnList.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [LogFilter(Type = LogType.Operate, Name = "获取库存有效期")]
        [HttpGet]
        public HttpResponseMessage GetAlarmStockLabel(string code)
        {
            var b = StockContract.StockDtos.Where(a => a.MaterialLabel == code).FirstOrDefault();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, StockContract.StockDtos.Where(a => a.MaterialLabel == code).FirstOrDefault().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取库存呆滞料信息
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = ("获取库存呆滞料信息"))]
        [HttpGet]
        public HttpResponseMessage GetInactiveStockPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            
            var query = StockContract.InactiveStockVMs;

            HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.WareHouseCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.MaterialCode.Contains(value) || q.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            var begin = pageCondition.FilterRuleCondition.Find(a => a.Field == "begin");
            var end = pageCondition.FilterRuleCondition.Find(a => a.Field == "end");
            if (begin != null && end != null)
            {
                var value1 = Convert.ToDateTime(begin.Value.ToString());
                var value2 = Convert.ToDateTime(end.Value.ToString());
                query = query.Where(p => p.OutTime >= value1 && p.OutTime <= value2);
                pageCondition.FilterRuleCondition.Remove(begin);
                pageCondition.FilterRuleCondition.Remove(end);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "InactiveDays");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                if (value.Contains("OneMonth"))
                {
                    query = query.Where(q => q.Days >= 30);
                } 
                if (value.Contains("Trimester"))
                {
                    query = query.Where(q => q.Days >= 90);
                }
                if (value.Contains("HalfAYear"))
                {
                    query = query.Where(q => q.Days >= 180);
                } 
                if (value.Contains("AYear"))
                {
                    query = query.Where(q => q.Days >= 30);
                }
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            else
            {
                // 设定呆滞物料天数
                // 获取提前预警天数
                List<Dictionary> dic = DictionaryContract.Dictionaries.Where(a => a.Code == "InactiveStock").ToList();
                int overDay = Convert.ToInt32(dic[0].Value);
                query = query.Where(q => q.Days >= overDay);
            }

            return Request.CreateResponse(HttpStatusCode.OK,query.ToPage(pageCondition).ToMvcJson());
        }

        /// <summary>
        /// 获取物料状态信息
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate,Name = ("获取物料状态信息"))]
        [HttpGet]
        public HttpResponseMessage GetMaterialStatusPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = StockContract.MaterialStatusS;

            FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.WareHouseCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.MaterialCode.Contains(value) || q.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            return Request.CreateResponse(HttpStatusCode.OK, query.ToPage(pageCondition).ToMvcJson());
        }

        /// <summary>
        /// 获取单个物料状态信息
        /// </summary>
        /// <param name="MaterialCode">查询的物料编码</param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = ("获取物料状态信息"))]
        [HttpGet]
        public HttpResponseMessage GetMaterialStatusList(string MaterialCode)
        {
            var query = StockContract.MaterialStatusS.Where(a => a.MaterialCode == MaterialCode).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, query.ToMvcJson());
        }

        /// <summary>
        /// 获取库存状态信息
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = ("获取库存状态信息"))]
        [HttpGet]
        public HttpResponseMessage GetInventoryStatusPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = StockContract.InventoryStatus;

            FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.WareHouseCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.MaterialCode.Contains(value) || q.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            return Request.CreateResponse(HttpStatusCode.OK, query.ToPage(pageCondition).ToMvcJson());
        }


        /// <summary>
        /// 从WebAPI下载文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTemp()
        {
            try
            {
                //string path = "/Assets/themes/Excel/库存信息.xlsx";
                //string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                //var stream = new FileStream(filePath, FileMode.Open);
                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                //response.Content = new StreamContent(stream);
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = "库存信息.xlsx"
                //};
                //return response;

                string path = "/Assets/themes/Excel/库存信息.xlsx";
                string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                var stream = new FileStream(filePath, FileMode.Open);
                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                //response.Content = new StreamContent(stream);
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = "入库单.xlsx"
                //};
                //return response;

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"库存导入模板.xlsx";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }



        /// <summary>
        /// 导入库存信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "导入库存信息")]
        [HttpPost]
        public HttpResponseMessage DoUpLoadInInfo()
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            HttpPostedFile file = files[0];//取得第一个文件
            if (file == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请选择导入文件！").ToMvcJson());
            }

            var extensionName = System.IO.Path.GetExtension(file.FileName);
            if (extensionName != ".xls" && extensionName != ".xlsx")
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请导入Excel文件！").ToMvcJson());
            }

            try
            {
                var tb = Bussiness.Common.ExcelHelper.ReadExeclToDataTable("Sheet1", 0, file.InputStream);
                if (tb.Rows.Count <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("Excel无内容").ToMvcJson());
                }
                int i = 0;
                StockContract.StockRepository.UnitOfWork.TransactionEnabled = true;
                foreach (DataRow item in tb.Rows)
                {
                    Bussiness.Entitys.Stock entity = new Bussiness.Entitys.Stock();
                    entity.MaterialCode = item["物料编码"].ToString();
                    if (string.IsNullOrEmpty(entity.MaterialCode))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("未填写物料编码").ToMvcJson());
                    }

                    if (MaterialContract.Materials.FirstOrDefault(a => a.Code == entity.MaterialCode) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("物料编码:" + entity.MaterialCode + "系统不存在").ToMvcJson());
                    }

                    var locationEntity = WareHouseContract.Locations.FirstOrDefault(a => a.SuggestMaterialCode == entity.MaterialCode);
                    if (locationEntity==null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("物料编码:" + entity.MaterialCode + "未绑定库位").ToMvcJson());
                    }
                    entity.LocationCode = locationEntity.Code;
                    //entity.locationcode = item["库存地址"].tostring();
                    //var locationentity = warehousecontract.locations.firstordefault(a => a.code == entity.locationcode);
                    //if (locationentity == null)
                    //{
                    //    return request.createresponse(httpstatuscode.ok, dataprocess.failure("库位编码:" + entity.locationcode + "系统不存在").tomvcjson());
                    //}
                    // 获取TrayId
                    entity.TrayId = locationEntity.TrayId;

                    entity.LockedQuantity = 0;
                    entity.ManufactureDate = DateTime.Now;//设置物料导入时间
       

                    //if (locationEntity.SuggestMaterialCode != entity.MaterialCode)
                    //{
                    //    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(string.Format("库位{0}推荐物料与填写物料不匹配",entity.LocationCode)).ToMvcJson());
                    //}
           
                    entity.Quantity = Convert.ToDecimal(item["数量"].ToString());
                    // 核查导入数量
                    if (entity.Quantity <= 0)
                    {
                        //return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请核对库存数量，物料：" + entity.MaterialCode).ToMvcJson());
                        continue;
                    }
                    entity.WareHouseCode = item["仓库编码"].ToString();
                    if (WareHouseContract.WareHouses.FirstOrDefault(a => a.Code == entity.WareHouseCode) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("仓库编码:" + entity.LocationCode + "系统不存在").ToMvcJson());
                    }
                    //entity.MaterialLabel = item["物料条码"].ToString();
                    //if (!string.IsNullOrEmpty(entity.MaterialLabel))
                    //{
                    //    if (StockContract.Stocks.FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel) != null)
                    //    {
                    //        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("物料条码:" + entity.MaterialLabel + "库存中已存在").ToMvcJson());
                    //    }
                    //}
                    //else
                    //{

                    //}
                    entity.MaterialLabel = SequenceContract.Create("Label");
                    entity.ContainerCode = item["货柜编码"].ToString();
                    if (WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("货柜编码:" + entity.ContainerCode + "系统不存在").ToMvcJson());
                    }
                    entity.SupplierCode = item["供应商编码"].ToString();
                    if (!string.IsNullOrEmpty(entity.SupplierCode))
                    {
                        if (IupplyContract.Supplys.FirstOrDefault(a => a.Code == entity.SupplierCode) == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("供应商编码:" + entity.SupplierCode + "系统不存在").ToMvcJson());
                        }
                    }
                    entity.MaterialStatus = 0;
                    entity.StockStatus = 0;
                    entity.BatchCode= item["批次"].ToString();
                    entity.InDate = DateTime.Now.ToString("yyyy-MM-dd");
                    entity.ShelfTime = DateTime.Now;
                    StockContract.CreateStockEntity(entity);
                }
                StockContract.StockRepository.UnitOfWork.Commit();
                return Request.CreateResponse(HttpStatusCode.OK,DataProcess.Success().ToMvcJson());
            }
            catch (System.Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
            }
        }


        /// <summary>
        /// 导出区域库位信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadLabelStock([FromUri] MvcPageCondition pageCondition)
        {

            List<int?> trayIdList = new List<int?>();
            List<string> containerCodeList = new List<string>();
            var identity = HP.Core.Security.Permissions.IdentityManager.Identity.UserData;
            var currentUser = IdentityContract.Users.FirstOrDefault(a => a.Code == identity.Code);
            if (!identity.IsSystem)
            {
                if (!string.IsNullOrEmpty(currentUser.StockArea))
                {
                    var trayList = currentUser.StockArea.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    foreach (var item in trayList)
                    {
                        trayIdList.Add(Convert.ToInt32(item.Split('-')[1]));
                    }
                }
            }
            if (trayIdList == null)
            {
                trayIdList = new List<int?>();
            }
            var query = StockContract.StockDtos;
            if (!currentUser.IsSystem)
            {
                query = query.Where(a => trayIdList.Contains(a.TrayId));
            }
            // 查询条件，根据用户名称查询
            // 查询条件，根据用户名称查询
            HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.WareHouseName.Contains(value) || p.WareHouseCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "ContainerCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.ContainerCode == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "TrayCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.TrayCode == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "SupplierCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.SupplierCode.Contains(value) || p.SupplierName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "LocationCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.LocationCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "AreaArray");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                List<int?> areaList = new List<int?>();
                foreach (var item in value.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList())
                {
                    areaList.Add(Convert.ToInt32(item));
                }
                pageCondition.FilterRuleCondition.Remove(filterRule);
                query = query.Where(a => areaList.Contains(a.TrayId));
            }

            var begin = pageCondition.FilterRuleCondition.Find(a => a.Field == "begin");
            var end = pageCondition.FilterRuleCondition.Find(a => a.Field == "end");
            if (begin != null && end != null)
            {
                var value1 = Convert.ToDateTime(begin.Value.ToString());
                var value2 = Convert.ToDateTime(end.Value.ToString());
                query = query.Where(p => p.CreatedTime >= value1 && p.CreatedTime <= value2);
                pageCondition.FilterRuleCondition.Remove(begin);
                pageCondition.FilterRuleCondition.Remove(end);
            }

            var aba = query.ToPage(pageCondition);

            var list = query.ToList();
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"WareHouseCode","仓库编码"},
                {"WareHouseName","仓库名称"},
                {"ContainerCode","货柜编码"},
                {"LocationCode","储位编码"},
                {"MaterialCode","物料编码"},
                {"MaterialName","物料名称"},
                {"Price","价格"},
                {"Use","用途"},
                {"MaterialLabel","物料条码"},
                {"Quantity","总数量"},
                {"LockedQuantity","锁定数量"},
                {"MaterialUnit","单位"},
                {"SupplierName","供应商名称"},
                {"SupplierCode","供应商编码"},
                {"BatchCode","批次"},
                {"ManufactureDate","入库时间"},
            };

            var fileName = "单盒物料库存信息.xlsx"; //string.Format(@"库位信息{0}.xlsx", string.Format("{0:G}", DateTime.Now));
            var excelFile = Bussiness.Common.ExcelHelper.ListToExecl(list, fileName, divFields);
            MemoryStream ms = new MemoryStream();
            excelFile.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            //获取导出文件流
            var stream = ms;
            if (stream == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            try
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"单盒物料库存信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
        
        
        /// <summary>
        /// 导出呆滞料信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoInactiveStock([FromUri] MvcPageCondition pageCondition)
        {
            var query = StockContract.InactiveStockVMs;

            HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.WareHouseCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.MaterialCode.Contains(value) || q.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            var begin = pageCondition.FilterRuleCondition.Find(a => a.Field == "begin");
            var end = pageCondition.FilterRuleCondition.Find(a => a.Field == "end");
            if (begin != null && end != null)
            {
                var value1 = Convert.ToDateTime(begin.Value.ToString());
                var value2 = Convert.ToDateTime(end.Value.ToString());
                query = query.Where(p => p.OutTime >= value1 && p.OutTime <= value2);
                pageCondition.FilterRuleCondition.Remove(begin);
                pageCondition.FilterRuleCondition.Remove(end);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "InactiveDays");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                if (value.Contains("OneMonth"))
                {
                    query = query.Where(q => q.Days >= 30);
                }
                if (value.Contains("Trimester"))
                {
                    query = query.Where(q => q.Days >= 90);
                }
                if (value.Contains("HalfAYear"))
                {
                    query = query.Where(q => q.Days >= 180);
                }
                if (value.Contains("AYear"))
                {
                    query = query.Where(q => q.Days >= 30);
                }
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            else
            {
                query = query.Where(q => q.Days >= 1);
            }

            var list = query.ToList();
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"MaterialCode","物料编码"},
                {"MaterialName","物料名称"},
                {"WareHouseCode","仓库编码"},
                {"WareHouseName","仓库名称"},       
                {"InTime","最早入库时间"},
                //{"OutTime","最后出库时间"},
                {"Quantity","库存数量"},
            };

            var fileName = "呆滞料信息.xlsx"; //string.Format(@"库位信息{0}.xlsx", string.Format("{0:G}", DateTime.Now));
            var excelFile = Bussiness.Common.ExcelHelper.ListToExecl(list, fileName, divFields);
            MemoryStream ms = new MemoryStream();
            excelFile.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            //获取导出文件流
            var stream = ms;
            if (stream == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            try
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"呆滞料信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// 导出库龄报表信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownStockAge([FromUri] MvcPageCondition pageCondition)
        {
            DateTime date = DateTime.Now;           
            var query = StockContract.StockDtos;
            // 查询条件，根据用户名称查询
            HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.WareHouseName.Contains(value) || p.WareHouseCode.Contains(value) || p.AreaCode.Contains(value) || p.LocationCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "SupplierCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.SupplierCode.Contains(value) || p.SupplierName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            var begin = pageCondition.FilterRuleCondition.Find(a => a.Field == "begin");
            var end = pageCondition.FilterRuleCondition.Find(a => a.Field == "end");
            if (begin != null && end != null)
            {
                var value1 = Convert.ToDateTime(begin.Value.ToString());
                var value2 = Convert.ToDateTime(end.Value.ToString());
                query = query.Where(p => p.CreatedTime >= value1 && p.CreatedTime <= value2);
                pageCondition.FilterRuleCondition.Remove(begin);
                pageCondition.FilterRuleCondition.Remove(end);
            }
            var list = query.ToList();
            foreach (var item in list)
            {
                item.StockAge = date.Subtract(Convert.ToDateTime(item.CreatedTime)).TotalDays.ToString();
            }
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"StockAge", "库龄(天)"},
                {"ManufactureDate","上架日期"},
                {"WareHouseCode","仓库编码"},
                {"WareHouseName","仓库名称"},
                {"ContainerCode","货柜编码"},
                //{"TrayCode","托盘编码"},
                {"LocationCode","库位编码"},
                {"MaterialCode","物料编码"},
                {"MaterialName","物料名称"},
                {"MaterialLabel","物料条码"},
                {"Quantity","总数量"},
                {"LockedQuantity","锁定数量"},
                {"MaterialUnit","单位"},
                {"SupplierName","供应商名称"},
                {"SupplierCode","供应商编码"},
                {"BatchCode","批次"},
            };

            var fileName = "物料库龄报表信息.xlsx"; //string.Format(@"库位信息{0}.xlsx", string.Format("{0:G}", DateTime.Now));
            var excelFile = Bussiness.Common.ExcelHelper.ListToExecl(list, fileName, divFields);
            MemoryStream ms = new MemoryStream();
            excelFile.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            //获取导出文件流
            var stream = ms;
            if (stream == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            try
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"物料库龄报表信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// 导出库存信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DoDownLoadMaterialStock([FromUri] MvcPageCondition pageCondition)
        {

            List<int?> trayIdList = new List<int?>();
            List<string> containerCodeList = new List<string>();
            var identity = HP.Core.Security.Permissions.IdentityManager.Identity.UserData;
            var currentUser = IdentityContract.Users.FirstOrDefault(a => a.Code == identity.Code);
            if (!identity.IsSystem)
            {
                if (!string.IsNullOrEmpty(currentUser.StockArea))
                {
                    var trayList = currentUser.StockArea.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    foreach (var item in trayList)
                    {
                        trayIdList.Add(Convert.ToInt32(item.Split('-')[1]));
                    }
                }
            }
            if (trayIdList == null)
            {
                trayIdList = new List<int?>();
            }
            var query = StockContract.StockDtos;
            if (!currentUser.IsSystem)
            {
                query = query.Where(a => trayIdList.Contains(a.TrayId));
            }
            // 查询条件，根据用户名称查询
            HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.WareHouseName.Contains(value) || p.WareHouseCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "ContainerCode");


            //if (filterRule != null)
            //{
            //    string value = filterRule.Value.ToString();
            //    query = query.Where(p => p.SupplierCode.Contains(value) || p.SupplierName.Contains(value));
            //    pageCondition.FilterRuleCondition.Remove(filterRule);
            //}
            //filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "LocationCode");


            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.ContainerCode == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "AreaArray");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                List<int?> areaList = new List<int?>();
                foreach (var item in value.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList())
                {
                    areaList.Add(Convert.ToInt32(item));
                }
                pageCondition.FilterRuleCondition.Remove(filterRule);
                query = query.Where(a => areaList.Contains(a.TrayId));
            }

            var stockquery = StockContract.Stocks;
            var list = query
                .GroupBy(p => p.MaterialCode)
                .AndBy(p => p.MaterialName)
                .AndBy(p => p.WareHouseCode)
                .AndBy(p => p.WareHouseName)
                .AndBy(p => p.MaterialUnit)
                .AndBy(p => p.Price)
                .AndBy(p => p.Use)
                .AndBy(p => p.LocationCode)
                .AndBy(p => p.ShelfTime)
                .Select(a => new StockDto()
                {
                    MaterialCode = a.MaterialCode,
                    MaterialName = a.MaterialName,
                    WareHouseCode = a.WareHouseCode,
                    WareHouseName = a.WareHouseName,
                    MaterialUnit = a.MaterialUnit,
                    Price = a.Price,
                    Use = a.Use,
                    LocationCode = a.LocationCode,
                    ShelfTime = a.ShelfTime,
                    Quantity = stockquery.Where(q => q.MaterialCode == a.MaterialCode && q.WareHouseCode == a.WareHouseCode).Sum(x => x.Quantity),
                    LockedQuantity = stockquery.Where(q => q.MaterialCode == a.MaterialCode && q.WareHouseCode == a.WareHouseCode).Sum(x => x.LockedQuantity),
                }).ToList();
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"WareHouseCode","仓库编码"},
                {"WareHouseName","仓库名称"},
                {"MaterialCode","物料编码"},
                {"MaterialName","物料名称"},
                {"Price","价格"},
                {"Use","用途"},
                {"LocationCode","上架储位"},
                {"Quantity","总数量"},
                {"LockedQuantity","锁定数量"},
                {"MaterialUnit","单位"},
                {"ShelfTime","入库时间"},
            };

            var fileName = "物料库存信息.xlsx"; //string.Format(@"库位信息{0}.xlsx", string.Format("{0:G}", DateTime.Now));
            var excelFile = Bussiness.Common.ExcelHelper.ListToExecl(list, fileName, divFields);
            MemoryStream ms = new MemoryStream();
            excelFile.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            //获取导出文件流
            var stream = ms;
            if (stream == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            try
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"物料库存信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }


        [LogFilter(Type = LogType.Operate, Name = "获取库存信息")]
        [HttpGet]
        public HttpResponseMessage GetStockByMaterialLabel(string MaterialLabel,string LocationCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.StockContract.GetStockByMaterialLabel(MaterialLabel, LocationCode).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "移库")]
        [HttpPost]
        public HttpResponseMessage StockMoveLocationCode(Bussiness.Entitys.Stock stock)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, StockContract.StockMoveLocationCode(stock.MaterialLabel,stock.NewLocationCode).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取仓库下的所有库存信息")]
        [HttpGet]
        public HttpResponseMessage GetLocationStockListByWareHouseCodeAndShelfCode(string wareHouseCode)
        {
            // 结果集
            var query = StockContract.Stocks.Where(a => a.WareHouseCode.Equals(wareHouseCode)).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, query.ToMvcJson());
            return response;
        }

    }
}

