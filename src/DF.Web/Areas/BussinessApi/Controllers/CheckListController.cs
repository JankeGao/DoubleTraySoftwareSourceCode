using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Data.Orm;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using Newtonsoft.Json;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 盘点单CheckListController
    /// </summary>
    [Description("盘点单管理")]
    public class CheckListController : BaseApiController
    {
        /// <summary>
        /// 盘点单信息
        /// </summary>
        public Bussiness.Contracts.ICheckListContract CheckListContract { set; get; }

        /// <summary>
        /// 仓库信息
        /// </summary>
        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }

        
        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取盘点单信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                var query = CheckListContract.CheckListDtos;
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
                    query = query.Where(p => p.WareHouseCode.Contains(value) || p.WareHouseName.Contains(value));
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

        /// <summary>
        /// 货柜
        /// </summary>
        public IRepository<Bussiness.Entitys.Container, int> ContainerRepository { get; set; }

        /// <summary>
        /// 托盘
        /// </summary>
        public IRepository<Tray, int> TrayRepository { get; set; }

        /// <summary>
        /// 储位
        /// </summary>
        public IRepository<Bussiness.Entitys.Location, int> LocationRepository { get; set; }
        public IQuery<Location> Locations => LocationRepository.Query();

        /// <summary>
        /// 获取仓库
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取仓库列表")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseList()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.WareHouses.ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 查看盘点单详情-搜索
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "查看盘点单详情")]
        [HttpGet]
        public HttpResponseMessage GetCheckListDetailPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = CheckListContract.CheckListDetailDtos;
                    
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

            var list = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取盘点单明细
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取盘点单明细")]
        [HttpGet]
        public HttpResponseMessage GetCheckListDetailList(string Code)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckListContract.CheckListDetailDtos.Where(a => a.CheckCode == Code).ToList().ToMvcJson());
            var va = CheckListContract.CheckListDetailDtos.Where(a => a.CheckCode == Code).ToList();
            return response;
        }

        /// <summary>
        /// 根据仓库获取货柜
        /// </summary>
        /// <param name="WareHosueCode"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "根据仓库获取货柜")]
        [HttpGet]
        public HttpResponseMessage GetCheckListContainerList(string WareHosueCode)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ContainerRepository.Query().Where(a => a.WareHouseCode == WareHosueCode).ToList().ToMvcJson());
            return response;
        }


        /// <summary>
        /// 选择盘点的内容下拉数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取货柜列表")]
        [HttpGet]
        public HttpResponseMessage GetContainerList()
        {
            // 获取所有仓库信息
            List<WareHouse> wareList = WareHouseContract.WareHouses.ToList();

            foreach (var item in wareList)
            {
                item.Name = item.Name;
                // 根据仓库获取每个仓库下所有的货柜
                List<ContainerDto> containers = WareHouseContract.ContainerDtos.Where(a => a.WareHouseCode == item.Code).ToList();
                foreach (var con in containers)
                {
                    // 获取每个货柜下的所有托盘
                    List<Tray> trays = TrayRepository.Query().Where(a => a.ContainerCode == con.Code).ToList();
                    con.children = trays;
                }
                item.children = containers;
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(wareList));
            return response;
        }

        /// <summary>
        /// 添加数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建盘点单")]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.CheckList entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckListContract.CreateCheckListEntity(entity).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取托盘编码
        /// </summary>
        /// <param name="query"></param>
        /// <param name="WareHouseCode"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取托盘信息")]
        [HttpGet]
        public HttpResponseMessage GetLocationList(string query, string WareHouseCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, Locations.Where(a => a.Code.Contains(query) && a.WareHouseCode == WareHouseCode).ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 作废盘点单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "作废盘点单")]
        [HttpPost]
        public HttpResponseMessage PostDoCancellatione(Bussiness.Entitys.CheckList entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, CheckListContract.CancellationeCheckList(entity).ToMvcJson());
            return response;
        }
    }
}

