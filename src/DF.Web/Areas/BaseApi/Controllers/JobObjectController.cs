using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Data.Orm.Extensions;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Pagination;
using HP.JobSchedulers.Contracts;
using HP.JobSchedulers.Models;

namespace DF.Web.Areas.BaseApi.Controllers
{
    /// <summary>
    /// 定时任务对象管理
    /// </summary>
    [Description("定时任务对象管理")]
    public class JobObjectController : BaseApiController
    {
        public IJobSchedulerContract JobSchedulerContract { set; get; }


        /// <summary>
        /// 查询启用定时任务信息
        /// </summary>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetJobObjects()
        {
            return Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.JobObjects.ToList().ToMvcJson());
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetJobObjectList([FromUri]MvcPageCondition pageCondition)
        {
            var query = JobSchedulerContract.JobObjects;
            //查询条件，根据定时对象名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Name");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            var jobObjectList = query.ToPage(pageCondition).ToMvcJson();
            return Request.CreateResponse(HttpStatusCode.OK, jobObjectList);
        }

        [LogApiFilter(Type = LogType.Operate, Name = "定时任务创建")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(JobObject entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.CreateJobObject(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "定时任务编辑")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(JobObject entity)
        {          
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.EditJobObject(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "定时任务删除")]
        [HttpPost]
        public HttpResponseMessage PostDoRemove(JobObject entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JobSchedulerContract.RemoveJobObject(entity.Id).ToMvcJson());
            return response;
        }

    }
}
