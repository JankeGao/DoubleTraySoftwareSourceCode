using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Contracts;
using HP.Utility.Data;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    [Description("首页数据统计")]
    public class DashboardController : BaseApiController
    {
        /// <summary>
        /// 入库
        /// </summary>
        public IInContract InContract { set; get; }
        /// <summary>
        /// 旧版出库
        /// </summary>
        public IOutContract OutContract { set; get; }


        /// <summary>
        /// 盘点
        /// </summary>
        public ICheckContract CheckContract { set; get; }

        /// <summary>
        /// 物料信息
        /// </summary>
        public INumAlarmContract NumAlarmContract { set; get; }

        /// <summary>
        /// 获取本日入库单数据
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetTodayIn()
        {
            DateTime dt = DateTime.Parse(DateTime.Now.ToShortDateString());
            return Request.CreateResponse(HttpStatusCode.OK, InContract.InDtos.Where(a => a.CreatedTime >= dt).ToList().ToMvcJson());
        }

        /// <summary>
        /// 获取本日出库单数据
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetTodayOut()
        {
            DateTime dt = DateTime.Parse(DateTime.Now.ToShortDateString());
            return Request.CreateResponse(HttpStatusCode.OK, OutContract.OutDtos.Where(a => a.CreatedTime >= dt).ToList().ToMvcJson());
        }

        /// <summary>
        /// 获取本日盘点单
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetTodayCheck()
        {
            DateTime dt = DateTime.Parse(DateTime.Now.ToShortDateString());
            return Request.CreateResponse(HttpStatusCode.OK, CheckContract.CheckDtos.Where(a => a.CreatedTime >= dt).ToList().ToMvcJson());
        }

        /// <summary>
        /// 获取库存预警
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetTodayAlarm()
        {
            DateTime dt = DateTime.Parse(DateTime.Now.ToShortDateString());
            return Request.CreateResponse(HttpStatusCode.OK, NumAlarmContract.NumAlarmDtos.ToList().ToMvcJson());
        }


        /// <summary>
        /// 获取当周每一天入库单价数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetWeekIns()
        {
            List<decimal?> list = new List<decimal?>();
            DateTime dt = DateTime.Parse(DateTime.Now.ToShortDateString());
            var query = OutContract.OutDtos;

            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    list.Add(query.Where(a => a.CreatedTime >= dt).ToList().Count);
                    break;
                case DayOfWeek.Tuesday:
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-1) && a.CreatedTime < dt).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt).ToList().Count);

                    break;
                case DayOfWeek.Wednesday:
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-2) && a.CreatedTime < dt.AddDays(-1)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-1) && a.CreatedTime < dt).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt).ToList().Count);
                    break;
                case DayOfWeek.Thursday:
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-3) && a.CreatedTime < dt.AddDays(-2)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-2) && a.CreatedTime < dt.AddDays(-1)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-1) && a.CreatedTime < dt).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt).ToList().Count);
                    break;
                case DayOfWeek.Friday:
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-4) && a.CreatedTime < dt.AddDays(-3)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-3) && a.CreatedTime < dt.AddDays(-2)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-2) && a.CreatedTime < dt.AddDays(-1)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-1) && a.CreatedTime < dt).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt).ToList().Count);
                    break;
                case DayOfWeek.Saturday:
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-5) && a.CreatedTime < dt.AddDays(-4)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-4) && a.CreatedTime < dt.AddDays(-3)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-3) && a.CreatedTime < dt.AddDays(-2)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-2) && a.CreatedTime < dt.AddDays(-1)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-1) && a.CreatedTime < dt).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt).ToList().Count);
                    break;
                case DayOfWeek.Sunday:
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-6) && a.CreatedTime < dt.AddDays(-5)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-5) && a.CreatedTime < dt.AddDays(-4)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-4) && a.CreatedTime < dt.AddDays(-3)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-3) && a.CreatedTime < dt.AddDays(-2)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-2) && a.CreatedTime < dt.AddDays(-1)).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt.AddDays(-1) && a.CreatedTime < dt).ToList().Count);
                    list.Add(query.Where(a => a.CreatedTime >= dt).ToList().Count);
                    break;
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取当月领用物料Top5
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetTopOutMaterials()
        {
            DateTime date = DateTime.Now;
            DateTime dtFirstDay = new DateTime(DateTime.Now.Year, date.Month, 1);
            DateTime dtEndDay = date;
            var query = OutContract.OutMaterialDtos.Where(a => a.CreatedTime >= dtFirstDay && a.CreatedTime <= dtEndDay)
                .Select(a => new
                {
                  a.MaterialCode,
                  //a.MaterialName,
                  a.Quantity,
                });
            var codeList = query
                .GroupBy(p => p.MaterialCode )
                .Select(a => new { a.MaterialCode })
                .ToList();
            var result = new List<TopSales>();
            foreach (var codeItem in codeList)
            {
                var item = new TopSales
                {
                    Name = codeItem.MaterialCode,
                    Value = query.Where(a => a.MaterialCode == codeItem.MaterialCode).Sum(a => a.Quantity)
                };
                result.Add(item);
            }
            result = result.OrderByDescending(a => a.Value).Take(5).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
        }

        /// <summary>
        /// 获取当月领用物料Top5
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetTopOutMaterialsClient()
        {
            DateTime date = DateTime.Now;
            DateTime dtFirstDay = new DateTime(DateTime.Now.Year, date.Month, 1);
            DateTime dtEndDay = date;
            var query = OutContract.OutMaterialDtos.Where(a => a.CreatedTime >= dtFirstDay && a.CreatedTime <= dtEndDay)
                .Select(a => new
                {
                    a.MaterialCode,
                    a.Quantity,
                });
            var codeList = query
                .GroupBy(p => p.MaterialCode)
                .Select(a => new { a.MaterialCode })
                .ToList();
            var result = new List<TopSales>();
            foreach (var codeItem in codeList)
            {
                var item = new TopSales
                {
                    Name = codeItem.MaterialCode,
                    Value = query.Where(a => a.MaterialCode == codeItem.MaterialCode).Sum(a => a.Quantity)
                };
                result.Add(item);
            }
            result = result.OrderByDescending(a => a.Value).Take(5).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success("查询成功", result));
        }
        /// <summary>
        /// 获取当月入库物料Top5
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetTopInMaterials()
        {
            DateTime date = DateTime.Now;
            DateTime dtFirstDay = new DateTime(DateTime.Now.Year, date.Month, 1);
            DateTime dtEndDay = date;
            var query = InContract.InMaterials.Where(a => a.CreatedTime >= dtFirstDay && a.CreatedTime <= dtEndDay)
                .Select(a => new
                {
                    a.MaterialCode,
                    a.Quantity,
                });
            var codeList = query
                .GroupBy(p => p.MaterialCode)
                .Select(a => new { ProcessingCenterName = a.MaterialCode })
                .ToList();
            var result = new List<TopSales>();
            foreach (var codeItem in codeList)
            {
                var item = new TopSales
                {
                    Name = codeItem.ProcessingCenterName,
                    Value = query.Where(a => a.MaterialCode == codeItem.ProcessingCenterName).Sum(a => a.Quantity)
                };
                result.Add(item);
            }
            result = result.OrderByDescending(a => a.Value).Take(5).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
        }


        /// <summary>
        /// 获取当月入库物料Top5
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetTopInMaterialsClient()
        {
            DateTime date = DateTime.Now;
            DateTime dtFirstDay = new DateTime(DateTime.Now.Year, date.Month, 1);
            DateTime dtEndDay = date;
            var query = InContract.InMaterialDtos.Where(a => a.CreatedTime >= dtFirstDay && a.CreatedTime <= dtEndDay)
                .Select(a => new
                {
                    a.MaterialCode,
                    a.Quantity,
                });
            var codeList = query
                .GroupBy(p => p.MaterialCode)
                .Select(a => new { ProcessingCenterName = a.MaterialCode })
                .ToList();
            var result = new List<TopSales>();
            foreach (var codeItem in codeList)
            {
                var item = new TopSales
                {
                    Name = codeItem.ProcessingCenterName,
                    Value = query.Where(a => a.MaterialCode == codeItem.ProcessingCenterName).Sum(a => a.Quantity)
                };
                result.Add(item);
            }
            result = result.OrderByDescending(a => a.Value).Take(5).ToList();
            
            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success("查询成功", result));
        }
        /// <summary>
        /// 获取每月出库单量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetMonthOutMaterials()
        {
            DateTime date = DateTime.Now;
            var montth = date.Month;
            var array = new decimal?[12];
            for (var i = 1; i <= montth; i++)
            {
                DateTime dtFirstDay = new DateTime(DateTime.Now.Year, i, 1);
                DateTime dtEndDay = dtFirstDay.AddMonths(1).AddSeconds(-1);
                var query = OutContract.Outs
                    .Where(a => a.CreatedTime >= dtFirstDay && a.CreatedTime <= dtEndDay).ToList().Count;
                array[i - 1] = query;
            }
            return Request.CreateResponse(HttpStatusCode.OK, array.ToMvcJson());
        }


        private class TopSales
        {
            public string Name { get; set; }
            public decimal? Value { get; set; }
        }
    }
}
