using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 设备信息管理
    /// </summary>
    public class DeviceInfoController : BaseApiController
    {
 
       /// <summary>
       /// 设备信息
       /// </summary>
        public IDeviceInfoContract DeviceInfoContract { get; set; }

        public IStockContract StockContract { set; get; }

        /// <summary>
        /// 仓库
        /// </summary>
        public IWareHouseContract WareHouseContract { set; get; }

        /// <summary>
        /// 物料信息
        /// </summary>
        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Login, Name = "获取仓库信息")]
        [HttpGet]
        public HttpResponseMessage GetDeviceInfo([FromUri]MvcPageCondition pageCondition)
        {

            List<Bussiness.Dtos.WareHouseTree> list = new List<Bussiness.Dtos.WareHouseTree>();
            var wareHouseList = WareHouseContract.WareHouses.ToList();
            foreach (var item in wareHouseList)
            {
                Bussiness.Dtos.WareHouseTree tree = new Bussiness.Dtos.WareHouseTree();
                tree.Code = item.Code;
                tree.Name = item.Name;
                List<ContainerDto> contarinerList = WareHouseContract.ContainerDtos.Where(a => a.WareHouseCode == item.Code).ToList();
                List<Bussiness.Entitys.Tray> trays = WareHouseContract.TrayRepository.Query().Where(a => a.WareHouseCode == item.Code).ToList();
                List<Bussiness.Entitys.Location> locationList = WareHouseContract.Locations.Where(a => a.WareHouseCode == item.Code).ToList();
                tree.ContainerList = contarinerList;
                tree.LocationList = locationList;
                tree.TrayList = trays;
                list.Add(tree);

            }
            var aa = list.ToMvcJson();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Login, Name = "根据仓库获取设备信息")]
        [HttpGet]
        public HttpResponseMessage GetDevices([FromUri] MvcPageCondition pageCondition)
        {

            try
            {
                var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Level");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                int Level = int.Parse(filterRule.Value.ToString());

                var CodeRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
                string Code = "";
                string searchCode = "";
                var SearchCodeRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "SearchCode");

                if (SearchCodeRule != null)
                {
                    searchCode = SearchCodeRule.Value.ToString();
                    pageCondition.FilterRuleCondition.Remove(SearchCodeRule);
                }
                if (CodeRule != null)
                {
                    Code = CodeRule.Value.ToString();
                    pageCondition.FilterRuleCondition.Remove(CodeRule);
                }

                string WareHouseCode = "";
                var WareHouseCodeRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");

                if (WareHouseCodeRule != null)
                {
                    WareHouseCode = WareHouseCodeRule.Value.ToString();
                    pageCondition.FilterRuleCondition.Remove(WareHouseCodeRule);
                }
                if (Level == 0) //查询仓库
                {
                    var query = WareHouseContract.ContainerDtos;
                    var list = query.ToPage(pageCondition);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                    return response;
                }               
                else if (Level == 1) //查询货柜
                {
                    var query = WareHouseContract.ContainerDtos;
                    if (!String.IsNullOrEmpty(Code))
                    {
                        query = WareHouseContract.ContainerDtos.Where(a => a.WareHouseCode == Code);
                    }
                    if (!string.IsNullOrEmpty(searchCode))
                    {
                        query = query.Where(a => a.Code.Contains(searchCode));
                    }
                    var list = query.ToPage(pageCondition); 
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                    return response;
                }
                else if(Level == 2)  // 查询货柜下的物料明细
                {
                    // 查询所有
                    var query = StockContract.StockDtos;
                    if (!String.IsNullOrEmpty(Code))
                    {
                        // 筛选该货柜下数据
                        query = query.Where(a => a.ContainerCode == Code);
                    }
                    // 判断有无条件，根据条件继续筛选
                    var LocationCodeRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "LocationCode");
                    if (LocationCodeRule != null)
                    {
                        string value = LocationCodeRule.Value.ToString();
                        query = query.Where(p => p.LocationCode.Contains(value) );
                        pageCondition.FilterRuleCondition.Remove(SearchCodeRule);
                    }
                    var MaterialCodeRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                    if (MaterialCodeRule != null)
                    {
                        string value = MaterialCodeRule.Value.ToString();
                        query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value));
                        pageCondition.FilterRuleCondition.Remove(MaterialCodeRule);
                    }                 
                    var list = query.OrderBy(a=>a.TrayCode).ToPage(pageCondition);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                    return response;
                }
                else 
                {
                    var query = WareHouseContract.TrayRepository.Query().Where(a => a.ContainerCode == Code);
                    if (!string.IsNullOrEmpty(searchCode))
                    {
                        query = query.Where(a => a.Code.Contains(searchCode));
                    }
                    var list = query.ToPage(pageCondition);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                    return response;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [LogFilter(Type = LogType.Operate, Name = "创建设备信息")]
        public HttpResponseMessage PostDoCreate(DeviceInfo entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DeviceInfoContract.CreateDeviceInfo(entity).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [LogFilter(Type = LogType.Operate, Name = "修改设备信息")]
        public HttpResponseMessage PostDoEdit(Container entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DeviceInfoContract.EditDeviceInfo(entity).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //[HttpPost]
        //[LogFilter(Type = LogType.Operate, Name = "删除设备信息")]
        //public HttpResponseMessage PostDoDelete(DeviceInfo entity)
        //{
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DeviceInfoContract.RemoveVideoInfo(entity.Id).ToMvcJson());
        //    return response;
        //}


    }
}