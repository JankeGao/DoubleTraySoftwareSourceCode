using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
    /// 设备报警
    /// </summary>
    public class DeviceAlarmController : BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        public Bussiness.Contracts.IDeviceAlarmContract DeviceAlarmContract { set; get; }   
        // <summary>
        /// 分页数据
        ///</summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取设备警报信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = DeviceAlarmContract.DeviceAlarmDtos;
            // 查询条件，根据设备信息查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "ContainerCode");

            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.ContainerCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Status");           
            if (filterRule != null)
            {
                int value = Convert.ToInt32(filterRule.Value.ToString());
                query = query.Where(p => p.Status == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            // 根据时间查询
            var begin = pageCondition.FilterRuleCondition.Find(a => a.Field == "begin");
            var end = pageCondition.FilterRuleCondition.Find(a => a.Field == "end");
            if (begin != null && end != null)
            {
                var value1 = Convert.ToDateTime(begin.Value.ToString());
                var value2 = Convert.ToDateTime(end.Value.ToString());
                query = query.Where(p => (p.CreatedTime) >= value1 && p.CreatedTime <= value2);
                pageCondition.FilterRuleCondition.Remove(begin);
                pageCondition.FilterRuleCondition.Remove(end);
            }
            //以倒叙方式查询显示
            var proList = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, proList.ToMvcJson());
            return response;
        }
        /// <summary>
        /// post方式数据-添加数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.DeviceAlarm entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DeviceAlarmContract.CreateDeviceAlarm(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 复位
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "复位时调用")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage PostDoEdit(Bussiness.Entitys.DeviceAlarm entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DeviceAlarmContract.EditDeviceAlarm(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 导出信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTemp([FromUri] MvcPageCondition pageCondition)
        {

            var query = DeviceAlarmContract.DeviceAlarmDtos;           
            // 查询条件，根据设备信息查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "ContainerCode");
            
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value) || p.ContainerCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Status");
            if (filterRule != null)
            {
                int value = Convert.ToInt32(filterRule.Value.ToString());
                query = query.Where(p => p.Status == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }
            // 根据时间查询
            var begin = pageCondition.FilterRuleCondition.Find(a => a.Field == "begin");
            var end = pageCondition.FilterRuleCondition.Find(a => a.Field == "end");
            if (begin != null && end != null)
            {
                var value1 = Convert.ToDateTime(begin.Value.ToString());
                var value2 = Convert.ToDateTime(end.Value.ToString());
                query = query.Where(p => (p.CreatedTime) >= value1 && p.CreatedTime <= value2);
                pageCondition.FilterRuleCondition.Remove(begin);
                pageCondition.FilterRuleCondition.Remove(end);
            }
            var list = query.ToList();
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"Code","报警编码"},
                {"ContainerCode","设备编码"},
                {"CreatedTime","报警时间"},
                {"ContinueTime","延续时间"},
                {"UpdatedTime","复位时间"},
                {"StatusDescription","状态" },
                {"WarehouseName","报警区域" },
                {"AlarmStatusDescription","报警描述" }

            };
            var fileName = "设备报警信息.xlsx";
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
                result.Content.Headers.ContentDisposition.FileName = $"设备报警信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}





