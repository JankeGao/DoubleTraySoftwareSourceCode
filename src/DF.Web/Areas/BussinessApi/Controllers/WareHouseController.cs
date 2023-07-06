using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Core.Security;
using HP.Core.Security.Permissions;
using HP.Data.Entity.Pagination;
using HP.Data.Orm.Entity;
using HP.Utility.Data;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;
using Newtonsoft.Json;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 仓库
    /// </summary>
    [Description("仓库信息")]
    public class WareHouseController : BaseApiController
    {
        /// <summary>
        /// 物料信息
        /// </summary>
        public IWareHouseContract WareHouseContract { set; get; }
        public IMaterialContract MaterialContract { set; get; }
        public IStockContract StockContract { set; get; }

        public IRepository<Location, int> LocationRepository { get; set; }
        public IIdentityContract IdentityContract { get; set; }

        #region 查询

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取库位信息")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseTreeData(int type)
        {
            
                List<Bussiness.Dtos.WareHouseTree> list = new List<Bussiness.Dtos.WareHouseTree>();
                var query = WareHouseContract.WareHouses;
                 List<int> trayIdList = new List<int>();
                 List<string> containerCodeList = new List<string>();
                var identity = HP.Core.Security.Permissions.IdentityManager.Identity.UserData;
                var currentUser = IdentityContract.Users.FirstOrDefault(a => a.Code == identity.Code);
                if ( !identity.IsSystem && type==0)
                {
                    if (!string.IsNullOrEmpty(currentUser.StockArea))
                    {
                      var trayList = currentUser.StockArea.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList();
                        foreach (var item in trayList)
                        {
                          trayIdList.Add(Convert.ToInt32(item.Split('-')[1]));
                        }
                       var warehouseCodeList = this.WareHouseContract.Trays.Where(a => trayIdList.Contains(a.Id)).Select(a => a.WareHouseCode).Distinct().ToList();
                    containerCodeList = this.WareHouseContract.Trays.Where(a => trayIdList.Contains(a.Id)).Select(a => a.ContainerCode).Distinct().ToList();
                    if (warehouseCodeList!=null && warehouseCodeList.Count>0)
                        {
                        query = query.Where(a => warehouseCodeList.Contains(a.Code));
                        }
                    }
                }
                if (containerCodeList==null && type==0)
                {
                    containerCodeList = new List<string>();
                }
                if (trayIdList==null &&  type == 0)
                {
                    trayIdList = new List<int>();
                }
               var wareHouseList = query.ToList();//WareHouseContract.WareHouses.ToList();
                foreach (var item in wareHouseList)
                {
                    Bussiness.Dtos.WareHouseTree tree = new Bussiness.Dtos.WareHouseTree();
                    tree.Code = item.Code;
                    tree.Name = item.Name;
                    var containerQuery = WareHouseContract.ContainerDtos.Where(a => a.WareHouseCode == item.Code);
                    if (!currentUser.IsSystem && type==0)
                    {
                        containerQuery = containerQuery.Where(a => containerCodeList.Contains(a.Code));
                    }
                    var trayQuery = WareHouseContract.TrayRepository.Query().Where(a => a.WareHouseCode == item.Code );
                    if (!currentUser.IsSystem && type==0)
                    {
                        trayQuery = trayQuery.Where(a => trayIdList.Contains(a.Id));
                    }
                  //  List<Bussiness.Entitys.Location> locationList = WareHouseContract.Locations.Where(a => a.WareHouseCode == item.Code).ToList();
                    tree.ContainerList = containerQuery.ToList();
                   // tree.LocationList = locationList;
                    tree.TrayList = trayQuery.ToList();
                    list.Add(tree);

                }
                var aa = list.ToMvcJson();
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                return response;
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "查询建议物料储位信息")]
        [HttpGet]
        public HttpResponseMessage GetMaterialPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                var query = WareHouseContract.LocationVMs;
                // 查询条件，根据用户名称查询
                var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "BoxCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.BoxCode.Contains(value) || p.BoxName.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.SuggestMaterialCode.Contains(value) || p.SuggestMaterialName.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "LocationCode");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.Code.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Level");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                //以倒叙方式查询显示
                var proList = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, proList.ToMvcJson());
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
        [LogFilter(Type = LogType.Operate, Name = "获取库位信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
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
                if (SearchCodeRule!=null)
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
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "BoxCode");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "LocationCode");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                if (Level == 0) //查询仓库
                {
                    var query = WareHouseContract.WareHouses;
                    var list = query.ToPage(pageCondition);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                    return response;
                }
                else if (Level == 1) //查询货柜
                {
                    var query = WareHouseContract.ContainerDtos.Where(a => a.WareHouseCode == Code);
                    if (!string.IsNullOrEmpty(searchCode))
                    {
                        query = query.Where(a => a.Code.Contains(searchCode) );
                    }
                    var list = query.ToPage(pageCondition);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                    return response;
                }
                else if (Level == 2)
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
                else if (Level == 3) // 查询储位
                {
                    int trayId = int.Parse(Code);
                   // var li = WareHouseContract.LocationVMs.ToList();
                    var query = WareHouseContract.LocationVMs.Where(a => a.TrayId == trayId);
                    if (!string.IsNullOrEmpty(WareHouseCode))
                    {
                        query = query.Where(a => a.WareHouseCode == WareHouseCode);
                    }
                    if (!string.IsNullOrEmpty(searchCode))
                    {
                        query = query.Where(a => a.Code.Contains(searchCode));
                    }
                    var list = query.OrderBy(a => a.XLight).ThenBy(A=>A.YLight).ToPage(pageCondition);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                    return response;
                }
                else // 查询单个库位
                {
                    var query = WareHouseContract.LocationVMs.Where(a => a.Code == Code);
                    var list = query.OrderBy(a=>a.XLight).ToPage(pageCondition);
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
        /// 根据LayoutId获取库位信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetLocationByLayoutId(string layoutId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.Locations.Where(a => a.LayoutId == layoutId).FirstOrDefault().ToMvcJson());
            return response;

            }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
            [HttpGet]
            public HttpResponseMessage GetLocationByTrayId(int trayId)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.Locations.Where(a => a.TrayId == trayId).ToList().ToMvcJson());
                return response;
            }

            /// <summary>
            /// 根据Id获取托盘信息
            /// </summary>
            /// <returns></returns>
            [HttpGet]
        public HttpResponseMessage GetTrayById(int trayId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.Trays.Where(a=>a.Id==trayId).FirstOrDefault().ToMvcJson());
            return response;
        }

        private class LocationColor
        {
            public string Color { get; set; }
            public string LayoutId { get; set; }
        }

        private class TrayColor
        {
            public string JsonLayout { get; set; }
            public List<LocationColor> LocationList { get; set; }
        }

        /// <summary>
        /// 根据Id获取托盘信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetTrayLayoutById(int trayId)
        {
            var returndata = new TrayColor();

            //string sql = "SELECT B.LayoutId,CASE WHEN A.Quantity>=C.BoxCount THEN '#F56C6C' WHEN A.Quantity < C.BoxCount AND A.Quantity >= (C.BoxCount / 2) THEN '#E6A23C' WHEN A.Quantity < C.BoxCount THEN '#67C23A'END AS Color FROM tb_wms_stock A left JOIN tb_wms_location B ON A.LocationCode = B.Code INNER JOIN TB_WMS_MATERIAL_BOX C ON A.MaterialCode = C.MaterialCode AND B.BoxCode = C.BoxCode where b.trayId=" + trayId;

            string sql = "select a.LayoutId , CASE WHEN b.Quantity>=C.BoxCount THEN '#F56C6C' WHEN b.Quantity < C.BoxCount AND b.Quantity >= (C.BoxCount / 2) THEN '#E6A23C' WHEN b.Quantity < C.BoxCount THEN '#67C23A'END AS Color from tb_wms_location a left join tb_wms_stock b  on b.LocationCode = a.Code left join TB_WMS_MATERIAL_BOX C ON b.MaterialCode = C.MaterialCode AND a.BoxCode = C.BoxCode where a.TrayId =" + trayId;

            //1、获取托盘信息
            var trayEntity = WareHouseContract.Trays.Where(a => a.Id == trayId).FirstOrDefault();
            returndata.JsonLayout = trayEntity.LayoutJson;
            //2、获取该托盘下的库位
            //var locationList = WareHouseContract.LocationRepository.Query().Where(a => a.TrayId == trayId).ToList();
            var returnList = new List<LocationColor>();

            var list = StockContract.StockRepository.UnitOfWork.DbContext.SqlQuery<object>(sql).ToList();

            returnList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LocationColor>>(Newtonsoft.Json.JsonConvert.SerializeObject(list));
            //foreach (var item in locationList)
            //{

            //    if (StockContract.StockDtos.Any(a => a.LayoutId == item.LayoutId))
            //    {
            //        var stock = StockContract.StockDtos.FirstOrDefault(a =>
            //            a.LayoutId == item.LayoutId);

            //        if (MaterialContract.MaterialBoxMaps.Any(a =>
            //            a.BoxCode == stock.BoxCode && a.MaterialCode == stock.MaterialCode))
            //        {
            //            var box = MaterialContract.MaterialBoxMapRepository.Query().FirstOrDefault(a =>
            //                a.BoxCode == stock.BoxCode && a.MaterialCode == stock.MaterialCode);
            //            stock.BoxMaxCount = box.BoxCount;

            //            var color = new LocationColor();
            //            color.LayoutId = item.LayoutId;
            //            if (stock.Quantity >= stock.BoxMaxCount)
            //            {
            //                color.Color = "#F56C6C";
            //            }
            //            else if (stock.Quantity >= (stock.BoxMaxCount / 2) && stock.Quantity < stock.BoxMaxCount)
            //            {
            //                color.Color = "#E6A23C";
            //            }
            //            else
            //            {
            //                color.Color = "#67C23A";
            //            }
            //            returnList.Add(color);
            //        }
            //    }
            //}

            returndata.LocationList = returnList;

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, returndata.ToMvcJson());
            return response;
        }
        #endregion



        /// <summary>
        /// 分页数据-获取不同类别仓库数据信息，根据用户编码
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取库位信息")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseTreeDataByUserCode(string type, string userCode)
        {
            var query = WareHouseContract.WareHouseAuthDtos;
            var identity = HP.Core.Security.Permissions.IdentityManager.Identity.UserData;
            var currentUser = IdentityContract.Users.FirstOrDefault(a => a.Code == identity.Code);

            List<Bussiness.Dtos.WareHouseTree> list = new List<Bussiness.Dtos.WareHouseTree>();

            if (!string.IsNullOrEmpty(type))
            {
                query = WareHouseContract.WareHouses.Where(a => a.CategoryDict == type);
            }
            var wareHouseList = query.ToList();
            foreach (var item in wareHouseList)
            {
                Bussiness.Dtos.WareHouseTree tree = new Bussiness.Dtos.WareHouseTree();
                tree.Code = item.Code;
                tree.Name = item.Name;
                List<Bussiness.Dtos.ContainerDto> areaList = WareHouseContract.ContainerDtos.Where(a => a.WareHouseCode == item.Code).ToList();
                List<Bussiness.Entitys.Location> locationList = WareHouseContract.Locations.Where(a => a.WareHouseCode == item.Code).ToList();
                tree.ContainerList = areaList;
                tree.LocationList = locationList;
                list.Add(tree);

            }
            var aa = list.ToMvcJson();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;

        }


        /// <summary>
        /// 核验人员权限
        /// </summary>
        /// <param name="trayId"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "核验人员操作权限")]
        [HttpGet]
        public HttpResponseMessage GetDoCheckAuth(int trayId)
        {
            var result = 1;
            // 进行人员权限的筛选
            IdentityTicket ticket = IdentityManager.Identity;

            // 如果不是超级管理员，则进行人员权限筛选
            if (!ticket.UserData.IsSystem)
            {
                if (!WareHouseContract.TrayUserDtos.Any(a => a.UserCode == ticket.UserData.Code && trayId == a.TrayId))
                {
                    result = 0;
                }
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 核验人员权限
        /// </summary>
        /// <param name="trayId"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "核验人员操作权限-客户端")]
        [HttpGet]
        public HttpResponseMessage GetDoCheckAuthClient(int trayId)
        {
            var result = DataProcess.Success("操作成功");
            // 进行人员权限的筛选
            IdentityTicket ticket = IdentityManager.Identity;

            // 如果不是超级管理员，则进行人员权限筛选
            if (!ticket.UserData.IsSystem)
            {
                if (!WareHouseContract.TrayUserDtos.Any(a => a.UserCode == ticket.UserData.Code && trayId == a.TrayId))
                {
                    result = DataProcess.Failure("操作成功");
                }
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建仓库")]
        [HttpPost]
        public HttpResponseMessage PostDoCreateWareHouse(Bussiness.Entitys.WareHouse entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.CreateWareHouse(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 编辑仓库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑仓库")]
        [HttpPost]
        public HttpResponseMessage PostDoEditWareHouse(Bussiness.Entitys.WareHouse entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.EditWareHouse(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除仓库")]
        [HttpPost]
        public HttpResponseMessage PostDoDeleteWareHouse(Bussiness.Entitys.WareHouse entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.RemoveWareHouse(entity.Id).ToMvcJson());
            return response;
        }



        /// <summary>
        /// 创建货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建货柜")]
        [HttpPost]
        public HttpResponseMessage PostDoCreateContainer(ContainerDto entityDto)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.CreateContainer(entityDto).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 编辑货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑货柜")]
        [HttpPost]
        public HttpResponseMessage PostDoEditContainer(Bussiness.Entitys.Container entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.EditContainer(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除货柜")]
        [HttpPost]
        public HttpResponseMessage PostDoDeleteContainer(Bussiness.Entitys.Container entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.RemoveContainer(entity.Id).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 维护货柜权限
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "维护货柜权限")]
        [HttpPost]
        public HttpResponseMessage AddBatchTrayUserMap(TrayUserMap entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.AddBatchTrayUserMap(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取托盘权限
        /// </summary>
        /// <param name="trayId"></param>
        /// <returns></returns>
        [HttpGet]
        [LogFilter(Type = LogType.Operate, Name = "获取托盘权限")]
        public HttpResponseMessage GetTrayUserMapByTrayId(int trayId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.TrayUserDtos.Where(a => a.TrayId == trayId).ToList().ToMvcJson());
            return response;
        }


        /// <summary>
        /// 创建托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建托盘")]
        [HttpPost]
        public HttpResponseMessage PostDoCreateTray(Tray entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.CreateTray(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 编辑托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑托盘")]
        [HttpPost]
        public HttpResponseMessage PostDoEditTray(Tray entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.EditTray(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除托盘")]
        [HttpPost]
        public HttpResponseMessage PostDoDeleteTray(Tray entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.RemoveTray(entity.Id).ToMvcJson());
            return response;
        }



        /// <summary>
        /// 维护托盘储位
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "维护托盘储位")]
        [HttpPost]
        public HttpResponseMessage PostDoEditTrayLocation(Tray entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.EditTrayLocation(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除库位")]
        [HttpPost]
        public HttpResponseMessage PostDoDeleteLocation(Bussiness.Entitys.Location entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.RemoveLocation(entity.Id).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 删除库位--根据layoutId
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除库位")]
        [HttpPost]
        public HttpResponseMessage PostDoDeleteLocationByLayoutId(Bussiness.Entitys.Location entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.RemoveLocationByLayoutId(entity.LayoutId).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "根据托盘id删除库位")]
        [HttpPost]
        public HttpResponseMessage PostDoDeleteLocationByTrayId(Bussiness.Entitys.Tray entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.RemoveLocationByTrayId(entity).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取物料信息
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取物料信息")]
        [HttpGet]
        public HttpResponseMessage GetMaterialList(string KeyValue)
        {
            var list = MaterialContract.Materials.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue)).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取仓库库位信息——打印
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取仓库库位信息")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseLocations(string KeyValue)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.LocationVMs.Where(a=>a.WareHouseCode== KeyValue).ToList().ToMvcJson());
            return response;
        }

        /// <summary>  
        /// 获取仓库库位信息——打印
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取仓库库位信息")]
        [HttpGet]
        public HttpResponseMessage GetAreaLocations(string KeyValue)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.LocationVMs.Where(a => a.ContainerCode == KeyValue).ToList().ToMvcJson());
            return response;
        }

        /// <summary>  
        /// 获取仓库库位信息——打印
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取仓库库位信息")]
        [HttpGet]
        public HttpResponseMessage GetTrayLocations(string KeyValue)
        {
            int trayId = int.Parse(KeyValue);
            // var li = WareHouseContract.LocationVMs.ToList();
            var query = WareHouseContract.LocationVMs.Where(a => a.TrayId == trayId);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, query.ToList().ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取货柜下的所有储位")]
        [HttpGet]
        public HttpResponseMessage GetLocations(string KeyValue)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.LocationVMs.Where(a => a.ContainerCode == KeyValue).ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 启动货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "启动货柜")]
        [HttpPost]
        public HttpResponseMessage PostStartContainer(Bussiness.Entitys.Tray entity)
        {
            var container = WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode);
            var result = DataProcess.Failure();
            if (container != null && container.IsVirtual == false)
            {
                Bussiness.Common.RunningContainer runningContainer = new Bussiness.Common.RunningContainer();
                runningContainer.ContainerCode = container.Code;
                runningContainer.IpAddress = container.Ip;
                runningContainer.Port = Convert.ToInt32(container.Port);
                runningContainer.TrayCode = Convert.ToInt32(entity.Code);
                runningContainer.XLight = 0;
                runningContainer.XLenght = 0;
                runningContainer.ContainerType = container.ContainerType;
                if (container.ContainerType == 2)
                {
                    runningContainer.BracketNumber = entity.BracketNumber;
                    runningContainer.TrayCode = entity.BracketTrayNumber;
                }
                var serverAddress = System.Configuration.ConfigurationSettings.AppSettings["PLCServerAddress"].ToString();
                //if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Kardex)
                //{

                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningC3000Container", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningC3000Container(runningContainer);
                //}
                //else  if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Hanel)
                //{
                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningHanelContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningHanelContainer(runningContainer);
                //}
                //else
                //{

                //}

               var result1 = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));


                result = DataProcess.SetDataResult(result1.Success, result1.Message, result1.Data);
            }
            else
            {
                result = DataProcess.Failure("未找到货柜或者货柜未虚拟货柜");
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 存入货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "存入货柜")]
        [HttpPost]
        public HttpResponseMessage PostRestoreContainer(Bussiness.Entitys.Location entity)
        {
            var container = WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode);
            var result = DataProcess.Failure();
            if (container != null && container.IsVirtual == false)
            {
                Bussiness.Common.RunningContainer runningContainer = new Bussiness.Common.RunningContainer();
                runningContainer.ContainerCode = container.Code;
                runningContainer.IpAddress = container.Ip;
                runningContainer.Port = Convert.ToInt32(container.Port);
                runningContainer.TrayCode = Convert.ToInt32(entity.Code);
                runningContainer.XLight = 0;
                runningContainer.XLenght = 0;
                runningContainer.ContainerType = container.ContainerType;
                var serverAddress = System.Configuration.ConfigurationSettings.AppSettings["PLCServerAddress"].ToString();
                //if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Kardex)
                //{

                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningC3000Container", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningC3000Container(runningContainer);
                //}
                //else  if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Hanel)
                //{
                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningHanelContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningHanelContainer(runningContainer);
                //}
                //else
                //{

                //}

              var  result1 = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRestoreContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));


                result = DataProcess.SetDataResult(result1.Success, result1.Message, result1.Data);
            }
            else
            {
                result = DataProcess.Failure("未找到货柜或者货柜未虚拟货柜");
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }


        /// <summary>
        /// 导出区域库位信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLocaitonInfo([FromUri] MvcPageCondition pageCondition)
        {




            var list = new List<Bussiness.Entitys.LocationVM>();


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
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "BoxCode");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "LocationCode");
                pageCondition.FilterRuleCondition.Remove(filterRule);

                 if (Level == 2)
                {
                  list = WareHouseContract.LocationVMs.Where(a=>a.ContainerCode==Code).OrderBy(a=>a.TrayCode).ThenBy(a=>a.XLight).ThenBy(a=>a.YLight).ToList();
                }
                 if (Level == 3) // 查询储位
                {
                    int trayId = int.Parse(Code);
                     list = WareHouseContract.LocationVMs.Where(a => a.TrayId == trayId).OrderBy(a => a.XLight).ThenBy(A => A.YLight).ToList();
                    //var query = WareHouseContract.LocationVMs.Where(a => a.TrayId == trayId);
                    //if (!string.IsNullOrEmpty(WareHouseCode))
                    //{
                    //    query = query.Where(a => a.WareHouseCode == WareHouseCode);
                    //}
                    //if (!string.IsNullOrEmpty(searchCode))
                    //{
                    //    query = query.Where(a => a.Code.Contains(searchCode));
                    //}
                    //var list = query.OrderBy(a => a.XLight).ThenBy(A => A.YLight).ToPage(pageCondition);
                    //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                    //return response;
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }


            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"ContainerCode","货柜编码"},
              //  {"TrayCode","托盘编号"},
                {"Code","储位编码"},
                {"SuggestMaterialCode","物料编码"},
                {"SuggestMaterialName","物料名称"},
                {"StockQuantity","库存数量"},
                {"MinStockQuantity","最小库存"},
                {"MaxStockQuantity","最大库存"},
                //{"BoxWidth","储位宽度(mm)"},
                //{"BoxLength","储位长度(mm)"}
            };

            var fileName = string.Format(@"库位信息{0}.xlsx", string.Format("{0:G}", DateTime.Now));// "货位信息.xlsx"; //string.Format(@"库位信息{0}.xlsx", string.Format("{0:G}", DateTime.Now));
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
                result.Content.Headers.ContentDisposition.FileName = $"库位信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// 获取当前仓库的所有货柜、托盘、库位信息
        /// </summary>
        /// <param name="wareHouseCode"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取当前仓库的所有货柜、托盘、库位信息")]
        [HttpGet]
        public HttpResponseMessage GetLocationListByWareHouseCodeAndShelfCode(string wareHouseCode)
        {
            var result = new { Containers = new List<List<Bussiness.Dtos.ContainerDto>>(), Trays = new List<List<Bussiness.Entitys.Tray>>(), Locations = new List<List<Bussiness.Entitys.Location>>() };
            // 获取当前仓库中的所有货柜
            var containers = WareHouseContract.ContainerDtos.Where(a => a.WareHouseCode.Equals(wareHouseCode)).Distinct().ToList();
            result.Containers.Add(containers);
            foreach (var container in containers) 
            {
                // 获取当前货柜中的所有托盘
                var trayList = WareHouseContract.Trays.Where(a => a.WareHouseCode.Equals(wareHouseCode) && a.ContainerCode.Equals(container.Code)).Distinct().ToList();
                result.Trays.Add(trayList);
                foreach (var tray in trayList)
                {
                    // 获取当前托盘中所有的库位
                    var locations = WareHouseContract.Locations.Where(a => a.WareHouseCode.Equals(wareHouseCode) && a.ContainerCode.Equals(container.Code) && a.TrayId.Equals(tray.Id) && a.Enabled == true).Distinct().ToList();
                    result.Locations.Add(locations);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
        }


        /// <summary>
        /// 导入物料储位信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "导入物料储位信息")]
        [HttpPost]
        public HttpResponseMessage DoUpLoadInInfo()
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            HttpPostedFile file = files[0];//取得第一个文件
            if (file == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请选择导入文件！").ToMvcJson());
            }

            var extensionName = System.IO.Path.GetExtension(file.FileName);
            if (extensionName != ".xls" && extensionName != ".xlsx")
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请导入Excel文件！").ToMvcJson());
            }

            try
            {
                var tb = Bussiness.Common.ExcelHelper.ReadExeclToDataTable("Sheet1", 1, file.InputStream);
                if (tb.Rows.Count <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("Excel无内容").ToMvcJson());
                }
                int i = 0;
                WareHouseContract.LocationRepository.UnitOfWork.TransactionEnabled = true;
                foreach (System.Data.DataRow item in tb.Rows)
                {
                    Bussiness.Entitys.Location entity = new Bussiness.Entitys.Location();
                    entity.TrayCode = item["货柜编码"].ToString();
                    if (string.IsNullOrEmpty(entity.TrayCode))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("未填写货柜编码").ToMvcJson());
                    }
                    entity.Code = item["储位编码"].ToString();
                    if (WareHouseContract.Locations.FirstOrDefault(a => a.Code == entity.Code) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("储位编码:" + entity.Code + "系统不存在").ToMvcJson());
                    }
                    var updateEntity = WareHouseContract.Locations.FirstOrDefault(a => a.Code == entity.Code);
                    updateEntity.SuggestMaterialCode= item["物料编码"].ToString();
                    if (!string.IsNullOrEmpty(updateEntity.SuggestMaterialCode)) {
                        if (MaterialContract.Materials.FirstOrDefault(a => a.Code == updateEntity.SuggestMaterialCode) == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("物料编码:" + updateEntity.SuggestMaterialCode + "系统不存在").ToMvcJson());
                        }
                    }

                    if (WareHouseContract.LocationRepository.Update(updateEntity) < 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("储位信息更新失败："+ updateEntity.Code).ToMvcJson());
                    }
                }
                WareHouseContract.LocationRepository.UnitOfWork.Commit();
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success().ToMvcJson());
            }
            catch (System.Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
            }
        }
    }
}

