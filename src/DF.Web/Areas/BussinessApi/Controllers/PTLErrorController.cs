using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    [Description("PTL错误信息")]
    public class PTLErrorController : BaseApiController
    {
        /// <summary>
        /// 物料信息
        /// </summary>
        public Bussiness.Contracts.SMT.IPTLErrorContract PTLErrorContract { set; get; }



        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取PTL设备错误日志")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                var query = PTLErrorContract.PTLErrors;
                // 查询条件，根据用户名称查询
                //HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                //if (filterRule != null)
                //{
                //    string value = filterRule.Value.ToString();
                //    query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
                //    pageCondition.FilterRuleCondition.Remove(filterRule);

                //}
                //filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
                //if (filterRule != null)
                //{
                //    string value = filterRule.Value.ToString();
                //    query = query.Where(p => p.WareHouseName.Contains(value) || p.WareHouseCode.Contains(value) || p.AreaCode.Contains(value) || p.LocationCode.Contains(value));
                //    pageCondition.FilterRuleCondition.Remove(filterRule);
                //}
                //filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "SupplierCode");
                //if (filterRule != null)
                //{
                //    string value = filterRule.Value.ToString();
                //    query = query.Where(p => p.SupplierCode.Contains(value) || p.SupplierName.Contains(value));
                //    pageCondition.FilterRuleCondition.Remove(filterRule);
                //}

                HP.Utility.Data.PageResult<Bussiness.Entitys.PTL.PTLError> list = query.ToPage(pageCondition);
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
        [LogFilter(Type = LogType.Operate, Name = "获取PLT执行错误日志")]
        [HttpGet]
        public HttpResponseMessage GetPTLExcuteErrorPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                var query = PTLErrorContract.PTLExcuteErrors;
                // 查询条件，根据用户名称查询
                //HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                //if (filterRule != null)
                //{
                //    string value = filterRule.Value.ToString();
                //    query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
                //    pageCondition.FilterRuleCondition.Remove(filterRule);

                //}
                //filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
                //if (filterRule != null)
                //{
                //    string value = filterRule.Value.ToString();
                //    query = query.Where(p => p.WareHouseName.Contains(value) || p.WareHouseCode.Contains(value) || p.AreaCode.Contains(value) || p.LocationCode.Contains(value));
                //    pageCondition.FilterRuleCondition.Remove(filterRule);
                //}
                //filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "SupplierCode");
                //if (filterRule != null)
                //{
                //    string value = filterRule.Value.ToString();
                //    query = query.Where(p => p.SupplierCode.Contains(value) || p.SupplierName.Contains(value));
                //    pageCondition.FilterRuleCondition.Remove(filterRule);
                //}

                HP.Utility.Data.PageResult<Bussiness.Entitys.PTL.PTLExcuteError> list = query.ToPage(pageCondition);
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
        [LogFilter(Type = LogType.Operate, Name = "标记为已解决而")]
        [HttpPost]
        public HttpResponseMessage HandleExcuteError(Bussiness.Entitys.PTL.PTLExcuteError error)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, this.PTLErrorContract.HandleExcuteError(error).ToMvcJson());
            return response;
        }

        #region PTL接口日志
        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取PLT执行错误日志")]
        [HttpGet]
        public HttpResponseMessage GetPTLInterefaceLogPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                var query = PTLErrorContract.PTLInterfaceLogs;
                // 查询条件，根据用户名称查询
                //HP.Utility.Filter.FilterRule filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                //if (filterRule != null)
                //{
                //    string value = filterRule.Value.ToString();
                //    query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
                //    pageCondition.FilterRuleCondition.Remove(filterRule);

                //}
                //filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
                //if (filterRule != null)
                //{
                //    string value = filterRule.Value.ToString();
                //    query = query.Where(p => p.WareHouseName.Contains(value) || p.WareHouseCode.Contains(value) || p.AreaCode.Contains(value) || p.LocationCode.Contains(value));
                //    pageCondition.FilterRuleCondition.Remove(filterRule);
                //}
                //filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "SupplierCode");
                //if (filterRule != null)
                //{
                //    string value = filterRule.Value.ToString();
                //    query = query.Where(p => p.SupplierCode.Contains(value) || p.SupplierName.Contains(value));
                //    pageCondition.FilterRuleCondition.Remove(filterRule);
                //}

                HP.Utility.Data.PageResult<Bussiness.Entitys.PTL.PTLInterfaceLog> list = query.ToPage(pageCondition);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

    }
}

