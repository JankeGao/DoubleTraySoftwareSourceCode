using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Pagination;
using HP.JobSchedulers.Contracts;
using HP.JobSchedulers.Models;

namespace DF.Web.Areas.BaseApi.Controllers
{
    /// <summary>
    /// 定时任务管理
    /// </summary>
    [Description("定时任务管理")]
    public class JobSchedulerController : BaseApiController
    {
        /// <summary>
        /// 定时任务契约
        /// </summary>
        public IJobSchedulerContract JobSchedulerContract { set; get; }


        /// <summary>
        /// 查询启用定时任务信息
        /// </summary>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetJobSchedulers()
        {
            return Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.JobSchedulers.ToMvcJson());
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetJobSchedulerList([FromUri]MvcPageCondition pageCondition)
        {
            var query = JobSchedulerContract.JobSchedulerOutputDtos;
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Name");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.JobName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var pageResult = query.ToPage(pageCondition).ToMvcJson();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, pageResult);
            return response;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogApiFilter(Type = LogType.Operate, Name = "定时任务创建")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(JobScheduler entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.CreateJobScheduler(entity).ToMvcJson());
            return response;
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogApiFilter(Type = LogType.Operate, Name = "定时任务编辑")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(JobScheduler entity)
        {          
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.EditJobScheduler(entity).ToMvcJson());
            return response;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogApiFilter(Type = LogType.Operate, Name = "定时任务删除")]
        [HttpPost]
        public HttpResponseMessage PostDoRemove(JobScheduler entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.RemoveJobScheduler(entity.Id).ToMvcJson());
            return response;
        }
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogApiFilter(Type = LogType.Operate, Name = "定时任务启动")]
        [HttpPost]
        public HttpResponseMessage DoStart(JobScheduler entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.StartJobScheduler(entity.Id).ToMvcJson());
            return response;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogApiFilter(Type = LogType.Operate, Name = "定时任务停止")]
        [HttpPost]
        public HttpResponseMessage DoStop(JobScheduler entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.StopJobScheduler(entity.Id, false).ToMvcJson());
            return response;
        }
    }
}
