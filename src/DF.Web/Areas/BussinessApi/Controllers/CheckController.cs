using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
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
    [Description("盘点管理")]
    public class CheckController : BaseApiController
    {
        /// <summary>
        /// 物料信息
        /// </summary>
        public Bussiness.Contracts.ICheckContract CheckContract { set; get; }

        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }

        public Bussiness.Contracts.IMaterialContract MaterialContract { get; set; }

        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取盘点信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                var query = CheckContract.CheckDtos;
                // 查询条件，根据用户名称查询
                var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.Code.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WarehouseCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.WareHouseCode.Contains(value)|| p.WareHouseName.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "PDAStatus");
                if (filterRule != null)
                {
                    int value = Convert.ToInt32( filterRule.Value.ToString());
                    query = query.Where(p => p.Status < value || p.Status==5);
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "ContainerCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.ContainerCode== value);
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                var list = query.OrderByDesc(a=>a.CreatedTime).ToPage(pageCondition);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [LogFilter(Type = LogType.Operate, Name = "创建盘点单信息")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.CheckMain entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.CreateCheckEntity(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "创建盘点单任务信息")]
        [HttpPost]
        public HttpResponseMessage PostDoCreateCheck(Bussiness.Entitys.CheckList entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.CreateCheckListEntity(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "编辑盘点单信息")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(Bussiness.Entitys.CheckMain entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.EditCheck(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "作废盘点单信息")]
        [HttpPost]
        public HttpResponseMessage PostDoCancel(Bussiness.Entitys.CheckMain entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.CancelCheck(entity.Id).ToMvcJson());
            return response;
        }
        /// <summary>
        /// 提交盘点单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "提交盘点单信息")]
        [HttpPost]
        public HttpResponseMessage PostDoSubmit(Bussiness.Entitys.CheckMain entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.SubmitCheckResult(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 查看盘点详情
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "查看盘点详情")]
        [HttpGet]
        public HttpResponseMessage GetCheckDetailPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = CheckContract.CheckDetailDtos;
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }

            var labelRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialLabel");
            if (labelRule != null)
            {
                string value = labelRule.Value.ToString();
                query = query.Where(p => p.MaterialLabel.Contains(value));
                pageCondition.FilterRuleCondition.Remove(labelRule);

            }

            var locationRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "LocationCode");
            if (locationRule != null)
            {
                string value = locationRule.Value.ToString();
                query = query.Where(p => p.LocationCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(locationRule);

            }

            var list = query.ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "手动执行盘点单")]
        [HttpPost]
        public HttpResponseMessage PostDoHandCheck(Bussiness.Entitys.CheckDetail entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.HandCheckDetail(entity).ToMvcJson());
            return response;
        }
        /// <summary>
        /// 客户端执行盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "客户端执行盘点")]
        [HttpPost]
        public HttpResponseMessage PostDoHandCheckClient(CheckDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.HandCheckDetailClient(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// PDA-盘点完成
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = " 盘点完成")]
        [HttpPost]
        public HttpResponseMessage PostPDACheckComplete(CheckDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.ConfirmCheck(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取仓库列表")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseList()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.WareHouses.ToList().ToMvcJson());
            return response;
        }
        [LogFilter(Type = LogType.Operate, Name = "获取库位列表")]
        [HttpGet]
        public HttpResponseMessage GetLocationList(string query,string WareHouseCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.Locations.Where(a=>a.Code.Contains(query) && a.WareHouseCode==WareHouseCode).ToList().ToMvcJson());
            var a2 = WareHouseContract.Locations.Where(a => a.Code.Contains(query) && a.WareHouseCode == WareHouseCode).ToList();
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "重盘")]
        [HttpPost]
        public HttpResponseMessage PostDoCheckAgain(Bussiness.Entitys.CheckMain entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.CheckAgain(entity).ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "获取物料信息")]
        [HttpGet]
        public HttpResponseMessage GetMaterialList(string KeyValue)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MaterialContract.Materials.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue)).ToList().ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "创建盘点详情")]
        [HttpPost]
        public HttpResponseMessage PostDoCreateDetail(Bussiness.Entitys.CheckDetail entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckContract.CreateCheckDetailForHand(entity).ToMvcJson());
            return response;
        }

        #endregion

        #region

        [LogFilter(Type = LogType.Operate, Name = "获取区域信息")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseAreaList(string WareHouseCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.Containers.Where(a => a.WareHouseCode == WareHouseCode).ToList().ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "获取盘点区域")]
        [HttpGet]
        public HttpResponseMessage GetCheckAreaList(string CheckCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,CheckContract.CheckAreaRepository.Query().Where(a=>a.CheckCode== CheckCode).ToList().ToMvcJson());
            return response;
        }
        #endregion

    }
}

