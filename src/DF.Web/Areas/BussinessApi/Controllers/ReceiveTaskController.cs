using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Bussiness.Entitys;
using HP.Core.Logging;
using HP.Core.Sequence;
using HP.Data.Entity.Pagination;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Description("领用任务管理")]
    public class ReceiveTaskController : BaseApiController
    {
        /// <summary>
        /// 领用任务数据库操作
        /// </summary>
        /// 
        public Bussiness.Contracts.IReceiveTaskContract ReceiveTaskContract { get; set; }

        /// <summary>
        /// 获取仓库
        /// </summary>
        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }

        /// <summary>
        /// 获取领用任务数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取领用任务信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = ReceiveTaskContract.ReceiveTaskDtos.Where(a => a.IsDeleted == false);
            // 查询条件，根据领用编码查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value) || p.Code.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Status");
            if (filterRule != null)
            {
                int value = Convert.ToInt32(filterRule.Value.ToString());
                query = query.Where(p => p.Status == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            //以倒叙方式查询显示
            var proList = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, proList.ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取领用任务数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取领用历史信息")]
        [HttpGet]
        public HttpResponseMessage GetHistoryPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = ReceiveTaskContract.ReceiveHistoryDetailDtos;
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "QueryCondition");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.TaskCode.Contains(value)
                                         || p.MaterialCode.Contains(value)
                                         || p.MaterialName.Contains(value) 
                                         || p.LastTimeReceiveName.Contains(value) 
                                         || p.LastTimeReturnName.Contains(value)
                );
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var begin = pageCondition.FilterRuleCondition.Find(a => a.Field == "begin");
            var end = pageCondition.FilterRuleCondition.Find(a => a.Field == "end");
            if (begin != null && end != null)
            {
                var value1 = Convert.ToDateTime(begin.Value.ToString());
                var value2 = Convert.ToDateTime(end.Value.ToString());
                query = query.Where(p => (p.LastTimeReceiveDatetime) >= value1 && p.LastTimeReceiveDatetime <= value2);
                pageCondition.FilterRuleCondition.Remove(begin);
                pageCondition.FilterRuleCondition.Remove(end);
            }
            //以倒叙方式查询显示
            var proList = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, proList.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取领用任务明细信息
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取领用任务明细信息")]
        [HttpGet]
        public HttpResponseMessage GetReceiveTaskDetailList(string Code)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveTaskContract.ReceiveTaskDetailDtos.Where(a => a.TaskCode == Code).ToList().ToMvcJson());
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
        /// 添加数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.Receive entity)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveTaskContract.CreateReceiveTaskEntity(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除领用任务单")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.ReceiveTask entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveTaskContract.RemoveReceiveTask(entity.Id).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除领用任务单")]
        [HttpPost]
        public HttpResponseMessage PostHandShelfReceiveTask(Bussiness.Entitys.ReceiveTaskDetail entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveTaskContract.HandShelfReceiveTask(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 归还
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "归还")]
        [HttpPost]
        public HttpResponseMessage PostReturn(Bussiness.Entitys.ReceiveTaskDetail entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveTaskContract.ReturnReceiveTask(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 导出历史出库信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTemp([FromUri] MvcPageCondition pageCondition)
        {
            var query = ReceiveTaskContract.ReceiveHistoryDetailDtos;
            // 查询条件，根据时间范围查询、根据出库单号查询、根据出库人查询、根据物料编码、物料名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "QueryCondition");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.TaskCode.Contains(value) || p.MaterialCode.Contains(value)
                                                                    || p.MaterialName.Contains(value) || p.LastTimeReceiveName.Contains(value) || p.LastTimeReturnName.Contains(value)
                );
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var begin = pageCondition.FilterRuleCondition.Find(a => a.Field == "begin");
            var end = pageCondition.FilterRuleCondition.Find(a => a.Field == "end");
            if (begin != null && end != null)
            {
                var value1 = Convert.ToDateTime(begin.Value.ToString());
                var value2 = Convert.ToDateTime(end.Value.ToString());
                query = query.Where(p => (p.LastTimeReceiveDatetime) >= value1 && p.LastTimeReceiveDatetime <= value2);
                pageCondition.FilterRuleCondition.Remove(begin);
                pageCondition.FilterRuleCondition.Remove(end);
            }

            var list = query.ToList();
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"Id","序号"},
                {"TaskCode","领用单号"},
                {"MaterialLabel","物料条码"},
                {"MaterialCode","物料编码"},
                {"LastTimeReceiveName","领用人"},
                {"LastTimeReceiveDatetime","领用时间"},
                {"Quantity","数量" },
                {"MaterialUnit","单位" },
                {"MaterialName","物料名称"},
                {"WareHouseCode","仓库编码" },
                {"CreatedTime","出库日期" },
            };

            var fileName = "历史领用信息.xlsx";
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
                result.Content.Headers.ContentDisposition.FileName = $"历史领用信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}

