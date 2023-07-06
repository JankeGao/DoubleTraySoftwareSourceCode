using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using HP.Core.Logging;
using HP.Core.Sequence;
using HP.Core.Sequence.Entitys;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;

namespace DF.Web.Areas.BaseApi.Controllers
{
    [Description("编码规则管理")]
    public class CodeRuleController : BaseApiController
    {
        public ICodeRuleContract CodeRuleContract { set; get; }


        /// <summary>
        /// 查询启用编码规则信息
        /// </summary>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetCodeRules()
        {
            return Request.CreateResponse(HttpStatusCode.OK, CodeRuleContract.CodeRules.ToMvcJson());
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetCodeRuleList([FromUri]MvcPageCondition pageCondition)
        {
            var query = CodeRuleContract.CodeRules;
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Name");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var pageResult = query.ToPage(pageCondition).ToMvcJson();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, pageResult);
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "编码规则创建")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PostDoCreate(CodeRuleInputDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CodeRuleContract.CreateCodeRule(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "编码规则编辑")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PostDoEdit(CodeRuleInputDto entity)
        {          
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CodeRuleContract.EditCodeRule(entity).ToMvcJson());
            return response;
        }

        [LogApiFilter(Type = LogType.Operate, Name = "编码规则删除")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PostDoRemove(CodeRule entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CodeRuleContract.RemoveCodeRule(entity.Id).ToMvcJson());
            return response;
        }

        #region 查询

        /// <summary>
        /// 查询启用编码规则信息
        /// </summary>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetItems(string codeRuleId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, CodeRuleContract.CodeRuleItems.Where(p => p.CodeRuleId == codeRuleId).OrderBy(p => p.Sort).ToList().ToMvcJson());
        }

        /// <summary>
        /// 查询编码规则
        /// </summary>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetRuleAssemblys()
        {
            return Request.CreateResponse(HttpStatusCode.OK, SequenceRuleFactory.GetInstance().GetRuleAssemblys().ToMvcJson());
        }

        /// <summary>
        /// 查询重置程序集
        /// </summary>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetResetAssemblys()
        {
            return Request.CreateResponse(HttpStatusCode.OK, SequenceResetFactory.GetInstance().GetResetAssemblys().ToMvcJson());
        }

        #endregion
    }
}
