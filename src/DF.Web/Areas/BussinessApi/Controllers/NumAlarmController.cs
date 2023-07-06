using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Bussiness.Dtos;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Utility.Data;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    public class NumAlarmController : BaseApiController
    {
        /// <summary>
        /// 库存信息
        /// </summary>
        public Bussiness.Contracts.IStockContract StockContract { set; get; }

        /// <summary>
        /// 库存上下限预警信息
        /// </summary>
        public Bussiness.Contracts.INumAlarmContract NumAlarmContract { set; get; }



        [LogFilter(Type = LogType.Operate, Name = "获取预警信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            try
            {
                var query = NumAlarmContract.NumAlarmDtos;

                var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }

                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.WareHouseCode.Contains(value) || p.WareHouseName.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }

                var stockquery = StockContract.Stocks;
                var materailquery = query
                    .GroupBy(p => p.MaterialCode)
                    .AndBy(p => p.MaterialName)
                    .AndBy(p => p.MaxNum)
                    .AndBy(p => p.MinNum)
                    .AndBy(p => p.Status)
                    .AndBy(p => p.ContainerCode)
                    .AndBy(p => p.LocationCode)
                    .Select(a => new NumAlarmDto()
                    {
                        Status = a.Status,
                        MinNum = a.MinNum,
                        MaxNum = a.MaxNum,
                        MaterialCode = a.MaterialCode,
                        MaterialName = a.MaterialName,
                        ContainerCode = a.ContainerCode,
                        LocationCode = a.LocationCode,
                        Quantity = stockquery.Where(q => q.MaterialCode == a.MaterialCode).Sum(x => x.Quantity)
                    });

                HP.Utility.Data.PageResult<Bussiness.Dtos.NumAlarmDto> list = materailquery.ToPage(pageCondition);
                //var list = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);

                return Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(e.Message).ToMvcJson());
            }
        }

        
        /// <summary>
        /// 核查库存是否存在报警
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage PostDoCheck()
        {
            return Request.CreateResponse(HttpStatusCode.OK, NumAlarmContract.CheckNumAlarm().ToMvcJson());
        }

        /// <summary>
        /// 导出信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTemp([FromUri] MvcPageCondition pageCondition)
        {

            var query = NumAlarmContract.NumAlarmDtos;

            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.WareHouseCode.Contains(value) || p.WareHouseName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }


            var stockquery = StockContract.Stocks;
            var materailquery = query
                .GroupBy(p => p.MaterialCode)
                .AndBy(p => p.MaterialName)
                .AndBy(p => p.MaxNum)
                .AndBy(p => p.MinNum)
                .AndBy(p => p.Status)
                .AndBy(p => p.ContainerCode)
                .AndBy(p => p.LocationCode)
                .Select(a => new NumAlarmDto()
                {
                    Status = a.Status,
                    MinNum = a.MinNum,
                    MaxNum = a.MaxNum,
                    MaterialCode = a.MaterialCode,
                    MaterialName = a.MaterialName,
                    ContainerCode = a.ContainerCode,
                    LocationCode = a.LocationCode,
                    Quantity = stockquery.Where(q => q.MaterialCode == a.MaterialCode).Sum(x => x.Quantity)
                });

            //var stockquery = StockContract.Stocks;
            //var materailquery = query
            //    .GroupBy(p => p.MaterialCode)
            //    .AndBy(p => p.MaterialName)
            //    .AndBy(p => p.WareHouseCode)
            //    .AndBy(p => p.WareHouseName)
            //    .AndBy(p => p.LocationCode)
            //    .AndBy(p => p.ContainerCode)
            //    .AndBy(p => p.TrayCode)
            //    .AndBy(p => p.MaxNum)
            //    .AndBy(p => p.MinNum)
            //    .AndBy(p => p.Status)
            //    .Select(a => new NumAlarmDto()
            //    {
            //        Status = a.Status,
            //        MinNum = a.MinNum,
            //        MaxNum = a.MaxNum,
            //        MaterialCode = a.MaterialCode,
            //        MaterialName = a.MaterialName,
            //        WareHouseCode = a.WareHouseCode,
            //        WareHouseName = a.WareHouseName,
            //        LocationCode = a.LocationCode,
            //        Quantity = stockquery.Where(q => q.MaterialCode == a.MaterialCode && q.WareHouseCode == a.WareHouseCode && q.LocationCode == a.LocationCode).Sum(x => x.Quantity),
            //        ContainerCode = a.ContainerCode,
            //        TrayCode = a.TrayCode
            //    });
            var list = query.ToList();
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"StatusCaption","报警状态"},
                {"MaxNum","库存上限"},
                {"MinNum","库存下限"},
                {"Quantityes","库存数量"},
                {"MaterialCode","物料编码"},
                {"ContainerCode","货柜"},
                {"LocationCode","上架储位"},
                {"MaterialName","物料名称" }
                //{"WareHouseCode","仓库编码" },
                //{"WareHouseName","仓库名称" },
                //{"LocationCode","库位地址" },

            };
            var fileName = "库存上下限信息.xlsx";
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
                result.Content.Headers.ContentDisposition.FileName = $"库存上下限信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}