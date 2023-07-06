using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.Logging;
using HP.Core.Mapping;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Pagination;
using HPC.Alarm.Contracts;
using HPC.Alarm.Models;

namespace ISS.Web.Areas.OpenApi.Controllers
{
    public class AlarmSettingController : BaseApiController
    {
        public IMapper Mapper { set; get; }
        public IAlarmSettingContract AlarmSettingContract { set; get; }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetAlarmSettingList([FromUri]MvcPageCondition pageCondition)
        {
            try{
                var query = AlarmSettingContract.AlarmSettingOutputs;
                var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Name");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.Code.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                return Request.CreateResponse(HttpStatusCode.OK, query.ToPage(pageCondition).ToMvcJson());
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
        // GET 方法测试 [FromUri]
        public HttpResponseMessage GetAlarmSetting()
        {
            try
            {
                //var AlarmSetting = AlarmSettingContract.AlarmSettingOutputs.ToList();
                //return Request.CreateResponse(HttpStatusCode.OK, InitTree(AlarmSetting, null).ToMvcJson());

                return Request.CreateResponse(HttpStatusCode.OK, AlarmSettingContract.AlarmSettingOutputs.ToList().ToMvcJson());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //private List<AlarmSettingTree> InitTree(List<AlarmSettingOutputDto> list, string parentCode)
        //{
        //    List<AlarmSettingTree> trees = new List<AlarmSettingTree>();
        //    foreach (var type in list.Where(a => a.ParentCode == parentCode))
        //    {
        //        AlarmSettingTree tree = Mapper.MapTo<AlarmSettingTree>(type);
        //        //      tree.State = list.Any(a => a.ParentCode == type.Code) ? TreeStateEnum.Closed.ToString().ToLower() : TreeStateEnum.Open.ToString().ToLower();
        //        tree.TreeId = type.Code;
        //        tree.TreeText = type.Name;
        //        tree.Children = InitTree(list, type.Code);
        //        trees.Add(tree);
        //    }
        //    return trees;
        //}

        [LogApiFilter(Type = LogType.Operate, Name = "添加区域")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(AlarmSetting entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, AlarmSettingContract.CreateAlarmSetting(entity).ToMvcJson());
            return response;
        }
    }
}