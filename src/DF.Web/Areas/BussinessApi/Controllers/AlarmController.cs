using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
    [Description("库存预警管理")]
    public class AlarmController : BaseApiController
    {
        /// <summary>
        /// 物料信息
        /// </summary>
        public Bussiness.Contracts.IStockContract StockContract { set; get; }

        /// <summary>
        /// 物料信息
        /// </summary>
        public Bussiness.Contracts.IAlarmContract AlarmContract { set; get; }


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取报警信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                var query = AlarmContract.AlarmDtos;
                // 查询条件，根据用户名称查询
                var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.MaterialCode.Contains(value)|| p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.WareHouseCode.Contains(value) || p.WareHouseName.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                var list = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
                foreach (var item in list.Rows)
                {
                    item.LimitDate = StockContract.StockDtos.FirstOrDefault(a => a.MaterialLabel == item.MaterialLabel).LimitDate;
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        /// <summary>
        /// 核查库存是否存在报警
        /// </summary>
        public HttpResponseMessage PostDoCheck()
        {
            AlarmContract.CheckAlarm();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// 根据仓库编码获取报警数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAlarmList(string type)
        {
            try
            {
                var list = AlarmContract.AlarmDtos.ToList();
                // 判断获取类型
                if (!type.ToLower().Equals("all"))
                {
                    list = list.Where(alarm => alarm.WareHouseCode.Equals(type)).ToList();
                }
                return Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ex.Message.ToMvcJson());
                return response;
            }

        }

        /// <summary>
        /// 导出信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTemp([FromUri] MvcPageCondition pageCondition)
        {

            var query = AlarmContract.AlarmDtos;
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
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
                query = query.Where(p => p.WareHouseCode.Contains(value) || p.WareHouseName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }      
            var list = query.ToList();
            foreach (var item in list)
            {
                item.LimitDate = StockContract.StockDtos.FirstOrDefault(a => a.MaterialLabel == item.MaterialLabel).LimitDate;
            }
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"StatusCaption","报警状态"},
                {"LimitDate","有效到期时间"},
                {"MaterialName","物料名称"},
                {"MaterialLabel","条码"},
                {"Quantity","数量"},
                {"MaterialCode","物料编码" },
                {"WareHouseCode","仓库编码" },
                {"WareHouseName","仓库名称" },
                {"LocationCode","库位地址" },
                {"ManufactureDateFormat","生产日期" },
                {"ValidityPeriod","设置有效期(天)" }

            };
            var fileName = "库存有效期信息.xlsx";
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
                result.Content.Headers.ContentDisposition.FileName = $"库存有效期信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}

