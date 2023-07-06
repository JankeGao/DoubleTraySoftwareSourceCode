using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Core.Sequence;
using HP.Data.Entity.Pagination;
using HP.Utility.Data;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 移库查看
    /// </summary>
    [Description("移库查看")]
    public class MobileLocationController : BaseApiController
    {
        /// <summary>
        /// 仓库信息
        /// </summary>
        public IWareHouseContract WareHouseContract { set; get; }

        public IMobileLocationContract MobileLocationContract { set; get; }

        public IRepository<Stock,int> StockRepository { set; get; }

        public IRepository<Bussiness.Entitys.Location, int> LocationRepository { get; set; }

        public IMaterialContract MaterialContract { set; get; }

        public IRepository<StockVM,int> StockVmRepository { set; get; }

        public ISequenceContract SequenceContract { set; get; }

        public IRepository<Bussiness.Entitys.MobileLocation, int> MobileRepository { get; set; }

        /// <summary>
        /// 库存信息
        /// </summary>
        public IStockContract StockContract { set; get; }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetWareHouseList()
        {
            return Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.WareHouses.ToList().ToMvcJson());
        }

        /// <summary>
        /// 根据仓库编码获取物料条码信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetMaterialLabelListByWareHouseCode(string WareHouseCode)
        {
            return Request.CreateResponse(HttpStatusCode.OK, StockVmRepository.Query().Where(a => a.WareHouseCode == WareHouseCode).ToList().ToMvcJson());
        }

        /// <summary>
        /// 获取仓库下的区域库位信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取仓库信息")]
        [HttpGet]
        public HttpResponseMessage GetAreaTreeData(string WareHouseCode)
        {

            List<Bussiness.Dtos.WareHouseTree> list = new List<Bussiness.Dtos.WareHouseTree>();
            var wareHouseList = WareHouseContract.WareHouses.Where(a => a.Code == WareHouseCode).ToList();
            foreach (var item in wareHouseList)
            {
                //Bussiness.Dtos.WareHouseTree tree = new Bussiness.Dtos.WareHouseTree();
                //tree.Code = item.Code;
                //tree.Name = item.Name;
                //List<Bussiness.Entitys.Area> areaList = WareHouseContract.Areas.Where(a => a.WareHouseCode == item.Code).ToList();
                //List<Bussiness.Entitys.Channel> channels = WareHouseContract.ChannelRepository.Query().Where(a => a.WareHouseCode == item.Code).ToList();
                //List<Bussiness.Entitys.Location> locationList = WareHouseContract.Locations.Where(a => a.WareHouseCode == item.Code).ToList();
                //tree.AreaList = areaList;
                //tree.LocationList = locationList;
                //tree.ChannelList = channels;
                //list.Add(tree);

            }
            var aa = list.ToMvcJson();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;

        }

        /// <summary>
        /// 根据区域编码和巷道编码获取库位信息
        /// </summary>
        /// <param name="AreaCode">区域编码</param>
        /// <param name="ChannelCode">巷道编码</param>
        /// <returns></returns>
        //public HttpResponseMessage GetLocation(string AreaCode,string ChannelCode)
        //{
        //    Channel channel = WareHouseContract.ChannelRepository.Query().FirstOrDefault( a => a.AreaCode == AreaCode && a.Code == ChannelCode);
        //    return Request.CreateResponse(HttpStatusCode.OK,WareHouseContract.Locations.Where(a => a.ChannelId == channel.Id).ToList().ToMvcJson());
        //}

        /// <summary>
        /// 获取首页分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取首页信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri] MvcPageCondition pageCondition)
        {
           /*
            var query = MobileLocationContract.MobileLocations
                .LeftJoin(MobileLocationContract.MobileLocationDtos, (mobile, dto) => mobile.Code == dto.Code).Select(
                    (mobile, dto) => new MobileLocationDto()
                    {
                        Code = mobile.Code,
                        Status = mobile.Status,
                        MaterialCode = mobile.MaterialCode,
                        MaterialLable = mobile.MaterialLable,
                        MaterialName = mobile.MaterialName,
                        WareHuoseCode = mobile.WareHuoseCode,
                        WareHouseName = mobile.WareHouseName,
                        NewLocationCode = mobile.NewLocationCode,
                        OldLocationCode = mobile.OldLocationCode,
                        CreatedUserCode = mobile.CreatedUserCode,
                        CreatedUserName = mobile.CreatedUserName,
                        CreatedTime = mobile.CreatedTime,
                        UpdatedUserCode = mobile.UpdatedUserCode,
                        UpdatedUserName = mobile.UpdatedUserName,
                        UpdatedTime = mobile.UpdatedTime
                    });
                    */
           var query = MobileRepository.Query();

            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.Code.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "WareHouseCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.WareHouseCode.Contains(value) || q.WareHouseName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "NewLocationCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(q => q.NewLocationCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            HP.Utility.Data.PageResult<Bussiness.Entitys.MobileLocation> list = query.ToPage(pageCondition);
            return Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取库位信息")]
        [HttpGet]
        public HttpResponseMessage GetLocationPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Level");
                pageCondition.FilterRuleCondition.Remove(filterRule);
                int Level = int.Parse(filterRule.Value.ToString());
                var CodeRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
                string Code = "";
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
                if (Level == 3) // 查询库位
                {
                    int channelId = int.Parse(Code);
                    var query = WareHouseContract.LocationVMs.Where(a => a.TrayId == channelId);
                    if (!string.IsNullOrEmpty(WareHouseCode))
                    {
                        var stock = StockVmRepository.Query().ToList();
                        query = query.Where(a => a.WareHouseCode == WareHouseCode);
                        if (stock.Count != 0)
                        {
                            foreach (var stockVm in stock)
                            {
                                var result = query.FirstOrDefault(a => a.Code == stockVm.LocationCode);
                                if (result != null)
                                {
                                    query = query.Where(a => a.Code != stockVm.LocationCode);
                                }
                            }
                        }
                    }
                    var list = query.ToPage(pageCondition);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
                    return response;
                }
                else // 查询单个库位
                {
                    var query = WareHouseContract.LocationVMs.Where(a => a.Code == Code);
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
        /// 创建库位移动单
        /// </summary>
        /// <param name="moblieLocation"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建库位移动单")]
        [HttpPost]
        public HttpResponseMessage CreateMobileLocation(Bussiness.Entitys.MobileLocation moblieLocation)
        {

            try
            {
                var entity = this.StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == moblieLocation.MaterialLable);
                if (entity != null)
                {
                    if (entity.IsLocked == true)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            DataProcess.Failure("该ReelId已被锁定(发料单或拆盘单占用)").ToMvcJson());
                    }
                    //1验证库位码合法性

                    var locationEntity = this.LocationRepository.Query().FirstOrDefault(a => a.Code == moblieLocation.NewLocationCode);
                    if (locationEntity == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,DataProcess.Failure("库位码错误").ToMvcJson());
                    }
                    bool IsExistStock = this.StockRepository.GetCount(a => a.LocationCode == moblieLocation.NewLocationCode) > 0;
                    if (IsExistStock)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            DataProcess.Failure("该库位上已有条码").ToMvcJson());
                    }

                    moblieLocation.Code = SequenceContract.Create("MobileLocation");
                    moblieLocation.MaterialName = MaterialContract.Materials.FirstOrDefault(a => a.Code == moblieLocation.MaterialCode).Name;
                    moblieLocation.Status = 0;
                    moblieLocation.WareHouseName = WareHouseContract.WareHouses.FirstOrDefault(a => a.Code == moblieLocation.WareHouseCode).Name;
                    if (MobileRepository.Query().Any(a => a.Code == moblieLocation.Code))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            DataProcess.Failure(string.Format("库位移动单{0}已存在，请勿重复创建！", moblieLocation.Code)).ToMvcJson());
                    }
                    MobileRepository.Insert(moblieLocation);

                    entity.LocationCode = moblieLocation.NewLocationCode;
                    this.StockRepository.Update(entity);

                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success().ToMvcJson());
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("未在货架上找到该库存").ToMvcJson());
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
            }
        }

        [LogFilter(Type = LogType.Operate, Name = "获取移库详情")]
        [HttpGet]
        public HttpResponseMessage GetMaterialList(string KeyValue)
        {
            var list = MobileRepository.Query().Where(a => a.Code.Contains(KeyValue)).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
        }
    }
}