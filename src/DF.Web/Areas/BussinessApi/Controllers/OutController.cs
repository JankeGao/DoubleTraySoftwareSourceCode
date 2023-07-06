using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Bussiness.Entitys;
using Bussiness.Enums;
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
    [Description("出库管理")]
    public class OutController : BaseApiController
    {
        /// <summary>
        /// 入库数据库操作
        /// </summary>
        public Bussiness.Contracts.IOutContract OutContract { set; get; }

        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }

        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }


        public Bussiness.Contracts.IStockContract StockContract { set; get; }

        public HPC.BaseService.Contracts.IDictionaryContract DictionaryContract { set; get; }
        public ISequenceContract SequenceContract { set; get; }
        public object response { get; private set; }

        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取出库单信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = OutContract.OutDtos;
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Status");
            if (filterRule != null)
            {
                int value = Convert.ToInt32(filterRule.Value.ToString());
                query = query.Where(p => p.Status == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var list = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取出库物料信息")]
        [HttpGet]
        public HttpResponseMessage GetOutMaterialList(string OutCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,
                OutContract.OutMaterialDtos.Where(a => a.OutCode == OutCode).ToList().ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "获取物料信息")]
        [HttpGet]
        public HttpResponseMessage GetMaterialList(string KeyValue)
        {
            var list = MaterialContract.Materials.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue));
            list = list.Where(a => !a.IsDeleted&& a.MaterialType == (int)MaterialTypeEnum.Nomarl);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,
                list.Take(20).ToList().ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "创建出库单")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.Out entity)
        {
            HttpResponseMessage response =
                Request.CreateResponse(HttpStatusCode.OK, OutContract.CreateOutEntity(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取仓库信息")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseList()
        {
            HttpResponseMessage response =
                Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.WareHouses.ToList().ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "删除出库单")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.Out entity)
        {
            HttpResponseMessage response =
                Request.CreateResponse(HttpStatusCode.OK, OutContract.RemoveOut(entity.Id).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "编辑出库单")]
        [HttpPost]
        public HttpResponseMessage PostDoUpdate(Bussiness.Entitys.Out entity)
        {
            HttpResponseMessage response =
                Request.CreateResponse(HttpStatusCode.OK, OutContract.EditOut(entity).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 作废入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "作废入库单")]
        [HttpPost]
        public HttpResponseMessage PostDoCancel(Bussiness.Entitys.Out entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutContract.CancelOut(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取物料信息")]
        [HttpGet]
        public HttpResponseMessage GetEditMaterialList(string outCode)
        {
            List<string> list = OutContract.OutMaterials.Where(a => a.OutCode == outCode).Select(a => a.MaterialCode)
                .ToList();
            var materialList = MaterialContract.Materials.Where(a => list.Contains(a.Code)).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, materialList.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 接口同步入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "同步出库单")]
        public HttpResponseMessage GetInterfaceOut()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, OutContract.CreateOutEntityInterFace().ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "获取物料可用库存")]
        [HttpGet]
        public HttpResponseMessage GetAvailableStock(string MaterialCode, string WareHouseCode)
        {

            var result = OutContract.CheckAvailableStock(MaterialCode, WareHouseCode); 
            
            //var AvailableStock = StockContract.Stocks
            //    .Where(a => a.MaterialCode == MaterialCode && a.WareHouseCode == WareHouseCode)
            //    .Sum(a => a.Quantity - a.LockedQuantity);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.Data);
            return response;
        }

        ///// <summary>
        ///// 获取待下架物料
        ///// </summary>
        ///// <returns></returns>
        //[LogFilter(Type = LogType.Operate, Name = "获取待上架物料条码信息")]
        //[HttpGet]
        //public HttpResponseMessage GetNeedShelfPageRecords([FromUri]MvcPageCondition pageCondition)
        //{
        //    var query = InContract.InMaterialDtos.Where(a => a.Status == (int)Bussiness.Enums.InStatusCaption.WaitingForShelf);
        //    // 查询条件，根据用户名称查询
        //    var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
        //    if (filterRule != null)
        //    {
        //        string value = filterRule.Value.ToString();
        //        query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
        //        pageCondition.FilterRuleCondition.Remove(filterRule);

        //    }
        //    var inCodeRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "InCode");
        //    if (inCodeRule!=null)
        //    {
        //        string value = inCodeRule.Value.ToString();
        //        query = query.Where(p =>p.InCode.Contains(value));
        //        pageCondition.FilterRuleCondition.Remove(inCodeRule);
        //    }
        //    var list = query.ToPage(pageCondition);
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
        //    return response;
        //}


        /// <summary>
        /// 获取库位码信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取库位码信息")]
        [HttpGet]
        public HttpResponseMessage GetLocationList(string KeyValue)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,
                WareHouseContract.Locations.Where(a => a.Code.Contains(KeyValue)).ToList().ToMvcJson());
            return response;
        }

        ///// <summary>
        ///// 手动执行上架
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //[LogFilter(Type = LogType.Operate, Name = "上架")]
        //[HttpPost]
        //public HttpResponseMessage PostDoHandShelf(Bussiness.Entitys.InMaterial entity)
        //{
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.HandShelf(entity).ToMvcJson());
        //    return response;
        //}

        ///// <summary>
        ///// 获取物料条码信息
        ///// </summary>
        ///// <returns></returns>
        //[LogFilter(Type = LogType.Operate, Name = "获取物料条码信息")]
        //[HttpGet]
        //public HttpResponseMessage GetInMaterialByLabel(string MaterialLabel)
        //{
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,InContract.InMaterialDtos.FirstOrDefault(a => a.MaterialLabel==MaterialLabel).ToMvcJson());
        //    return response;
        //}





        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        //[LogFilter(Type = LogType.Operate, Name = "获取待下架条码数据")]
        //[HttpGet]
        //public HttpResponseMessage GetOutLabelPageRecords([FromUri] MvcPageCondition pageCondition)
        //{
        //    var query = OutContract.OutMaterialLabelDtos;
        //    // 根据出库单号
        //    var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
        //    if (filterRule != null)
        //    {
        //        string value = filterRule.Value.ToString();
        //        query = query.Where(p => p.OutCode.Contains(value));
        //        pageCondition.FilterRuleCondition.Remove(filterRule);

        //    }
        //    filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialLabel");
        //    if (filterRule != null)
        //    {
        //        string value = filterRule.Value.ToString();
        //        query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
        //        pageCondition.FilterRuleCondition.Remove(filterRule);

        //    }
        //    var list = query.OrderByDesc(a=>a.CreatedTime).ToPage(pageCondition);
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
        //    return response;
        //}



        //[LogFilter(Type = LogType.Operate, Name = "手动下架")]
        //[HttpPost]
        //public HttpResponseMessage PostDoHandShelfDown(Bussiness.Entitys.OutMaterialLabel entity)
        //{
        //    HttpResponseMessage response =
        //        Request.CreateResponse(HttpStatusCode.OK, OutContract.ExcuteHandShelfDown(entity).ToMvcJson());
        //    return response;
        //}


        /// <summary>
        /// 获取合并拣货信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取待下架条码数据")]
        [HttpGet]
        public HttpResponseMessage GetCombineOutMaterialList(string OutCode)
        {
            var query = OutContract.OutMaterialDtos;
            var list = query.Where(a => a.OutCode == OutCode &&
                                        (a.Status == (int) Bussiness.Enums.OutStatusCaption.PartSending ||
                                         a.Status == (int) Bussiness.Enums.OutStatusCaption.SendedPickOrder)).ToList();
            // 合并
            List<Bussiness.Dtos.OutMaterialDto> dtoList = new List<Bussiness.Dtos.OutMaterialDto>();
            var group = list.GroupBy(a => a.MaterialCode);
            foreach (var item in group)
            {
                Bussiness.Dtos.OutMaterialDto dto = new Bussiness.Dtos.OutMaterialDto();
                dto.AvailableStock = item.FirstOrDefault().AvailableStock;
                dto.BatchCode = item.FirstOrDefault().BatchCode;
                dto.BillCode = item.FirstOrDefault().BillCode;
                dto.CreatedTime = item.FirstOrDefault().CreatedTime;
                dto.CreatedUserCode = item.FirstOrDefault().CreatedUserCode;
                dto.CreatedUserName = item.FirstOrDefault().CreatedUserName;
                dto.Id = item.FirstOrDefault().Id;
                dto.IsDeleted = item.FirstOrDefault().IsDeleted;
                dto.ItemNo = item.FirstOrDefault().ItemNo;
                ;
                dto.MaterialCode = item.Key;
                dto.MaterialName = item.FirstOrDefault().MaterialName;
                dto.MaterialUnit = item.FirstOrDefault().MaterialUnit;
                dto.OutCode = item.FirstOrDefault().OutCode;
                dto.PickedQuantity = item.Sum(a => a.PickedQuantity);
                dto.PickedTime = item.FirstOrDefault().PickedTime;
                dto.Quantity = item.Sum(a => a.Quantity);
                dto.Status = item.FirstOrDefault().Status;
                dto.SuggestLocation = item.FirstOrDefault().SuggestLocation;
                dto.WareHouseCode = item.FirstOrDefault().WareHouseCode;
                dtoList.Add(dto);
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, dtoList.ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取复核或待复核条码
        /// </summary>
        /// <returns></returns>
        //[LogFilter(Type = LogType.Operate, Name = "获取出库单信息")]
        //    [HttpGet]
        //    public HttpResponseMessage GetWaiitingForCheckOrCheckedLabel(string OutCode, int Status, string MaterialLabel)
        //    {
        //        var query = OutContract.OutMaterialLabelDtos.Where(a => a.Status == Status && a.OutCode == OutCode);
        //        if (!string.IsNullOrEmpty(MaterialLabel))
        //        {
        //            query = query.Where(a => a.MaterialLabel.Contains(MaterialLabel));
        //        }
        //        // 查询条件，根据用户名称查询
        //        var list = query.ToList();

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
        //        return response;
        //    }


        //[LogFilter(Type = LogType.Operate, Name = "复核")]
        //[HttpPost]
        //public HttpResponseMessage ConfirmCheckLabel(Bussiness.Entitys.OutMaterialLabel entity)
        //{
        //    HttpResponseMessage response =
        //        Request.CreateResponse(HttpStatusCode.OK, OutContract.ConfirmCheckLabel(entity).ToMvcJson());
        //    return response;
        //}

        #endregion


        /// <summary>
        /// 导入出库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "发布下架任务")]
        [HttpPost]
        public HttpResponseMessage DoUpLoadOutInfo()
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            HttpPostedFile file = files[0]; //取得第一个文件
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
                var tb = Bussiness.Common.ExcelHelper.ReadExeclToDataTable("sheet1", 0, file.InputStream);
                if (tb.Rows.Count <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("Excel无内容").ToMvcJson());
                }

                Bussiness.Entitys.Out outEntity = new Bussiness.Entitys.Out();
                outEntity.BillCode = tb.Rows[0]["来源单号"].ToString();
                outEntity.Code = SequenceContract.Create(outEntity.GetType()); //tb.Rows[0]["出库单号"].ToString();
                outEntity.WareHouseCode = tb.Rows[0]["仓库编码"].ToString();
                if (!WareHouseContract.WareHouses.Any(a=>a.Code==outEntity.WareHouseCode))
                {
                    Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("仓库编码:" + outEntity.WareHouseCode + "系统不存在").ToMvcJson());

                }
                outEntity.CreatedTime = DateTime.Now;
                outEntity.IsDeleted = false;
                outEntity.OutDict = DictionaryContract.Dictionaries.Where(a => a.TypeCode == "OutType").FirstOrDefault()
                    .Code;
                outEntity.Status = 0;
                outEntity.AddMaterial = new List<Bussiness.Entitys.OutMaterial>();
                int i = 0;
                foreach (DataRow item in tb.Rows)
                {
                    Bussiness.Entitys.OutMaterial outmaterial = new Bussiness.Entitys.OutMaterial();
                    outmaterial.BatchCode = "";
                    outmaterial.BillCode = outEntity.BillCode;
                    outmaterial.CheckedQuantity = 0;
                    outmaterial.IsDeleted = false;
                    outmaterial.ItemNo = (i + 1).ToString().PadLeft(6, '0');
                    outmaterial.MaterialCode = item["物料编码"].ToString();
                    if (MaterialContract.Materials.FirstOrDefault(a => a.Code == outmaterial.MaterialCode) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            DataProcess.Failure("物料编码:" + outmaterial.MaterialCode + "系统不存在").ToMvcJson());
                    }
                    outmaterial.PickedQuantity = 0;
                    outmaterial.Quantity = Convert.ToDecimal(item["数量"].ToString());
                    // 核查导入数量
                    if (outmaterial.Quantity <= 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请核对导入出库单明细数量，物料：" + outmaterial.MaterialCode).ToMvcJson());
                    }
                    outmaterial.Status = 0;
                    outmaterial.OutCode = outEntity.Code;
                    outEntity.AddMaterial.Add(outmaterial);


                }
                return Request.CreateResponse(HttpStatusCode.OK, OutContract.CreateOutEntity(outEntity).ToMvcJson());
            }
            catch (System.Exception ex)
            {

                return null;
            }

        }




        /// <summary>
        /// 从WebAPI下载文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTemp()
        {
            try
            {
                string path = "/Assets/themes/Excel/出库单导入模版.xlsx";
                string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                var stream = new FileStream(filePath, FileMode.Open);
                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                //response.Content = new StreamContent(stream);
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = "出库单.xlsx"
                //};
                //return response;

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"出库单导入模版.xlsx";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}

