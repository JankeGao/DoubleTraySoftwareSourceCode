using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Logging;
using HP.Core.Sequence;
using HP.Data.Entity.Pagination;
using HP.Utility.Data;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 入库管理
    /// </summary>
    [Description("入库管理")]
    public class InController : BaseApiController
    {
        /// <summary>
        /// 入库数据库操作
        /// </summary>
        public Bussiness.Contracts.IInContract InContract { set; get; }
        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }

        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }
        // 供应商管理接口
        public Bussiness.Contracts.ISupplyContract SupplyContract { set; get; }

        public IDictionaryContract DictionaryContract { set; get; }

        public ISequenceContract SequenceContract { set; get; }

        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取入库单信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = InContract.InDtos;
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
                query = query.Where(p => p.Status== value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var list = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取入库物料信息")]
        [HttpGet]
        public HttpResponseMessage GetInMaterialList(string InCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.InMaterialDtos.Where(a=>a.InCode==InCode).ToList().ToMvcJson());
            return response;
        }


        [LogFilter(Type = LogType.Operate, Name = "获取物料信息")]
        [HttpGet]
        public HttpResponseMessage GetMaterialList(string KeyValue)
        {
            var list = MaterialContract.Materials.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue));
            list = list.Where(a => !a.IsDeleted);
            var aa = list.Take(20).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, aa.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 创建入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建入库单")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.In entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,InContract.CreateInEntity(entity).ToMvcJson());
            return response;
        }
        /// <summary>
        /// restful接口同步入库单
        /// </summary>
        /// <returns></returns>
        //[LogFilter(Type = LogType.Operate, Name = "同步入库单")]
        //[HttpGet]
        //public HttpResponseMessage GetInterfaceIned()
        //{
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.CreateInEntityInterFacedes().ToMvcJson());
        //    return response;
        //}

        /// <summary>
        /// 接口同步入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "同步入库单")]
        public HttpResponseMessage GetInterfaceIn()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.CreateInEntityInterFace().ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取仓库信息")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseList()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.WareHouses.ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除入库单")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.In entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.RemoveIn(entity.Id).ToMvcJson());
            return response;
        }
        /// <summary>
        /// 编辑入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑入库单")]
        [HttpPost]
        public HttpResponseMessage PostDoUpdate(Bussiness.Entitys.In entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.EditIn(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 作废入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "作废入库单")]
        [HttpPost]
        public HttpResponseMessage PostDoCancel(Bussiness.Entitys.In entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.CancelIn(entity).ToMvcJson());
            return response;
        }

        [LogFilter(Type = LogType.Operate, Name = "获取物料信息")]
        [HttpGet]
        public HttpResponseMessage GetEditMaterialList(string inCode)
        {
            List<string> list = InContract.InMaterialRepository.Query().Where(a => a.InCode == inCode).Select(a => a.MaterialCode).ToList();
            var materialList = MaterialContract.Materials.Where(a => list.Contains(a.Code)).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, materialList.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 执行亮灯操作
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "上架")]
        [HttpPost]
        public HttpResponseMessage PostDoShelf(Bussiness.Entitys.In entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.RemoveIn(entity.Id).ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取待上架物料条码
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取待上架物料条码信息")]
        [HttpGet]
        public HttpResponseMessage GetNeedShelfPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = InContract.InMaterialDtos;//.Where(a => a.Status == (int)Bussiness.Enums.InStatusCaption.WaitingForShelf|| a.Status == (int)Bussiness.Enums.InStatusCaption.Sheling);
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value) || p.MaterialLabel.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var inCodeRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "InCode");
            if (inCodeRule!=null)
            {
                string value = inCodeRule.Value.ToString();
                query = query.Where(p =>p.InCode.Contains(value));
                pageCondition.FilterRuleCondition.Remove(inCodeRule);
            }
            var list = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取库位码信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取库位码信息")]
        [HttpGet]
        public HttpResponseMessage GetLocationList(string KeyValue,string WareHouseCode)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.Locations.Where(a => a.Code.Contains(KeyValue) && a.WareHouseCode== WareHouseCode).Take(10).ToList().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 手动执行上架
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "上架")]
        [HttpPost]
        public HttpResponseMessage PostDoHandShelf(Bussiness.Entitys.InMaterial entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.HandShelf(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取物料条码信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取物料条码信息")]
        [HttpGet]
        public HttpResponseMessage GetInMaterialByLabel(string MaterialLabel)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,InContract.InMaterialDtos.FirstOrDefault(a => a.MaterialLabel==MaterialLabel).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "发布上架任务")]
        [HttpPost]
        public HttpResponseMessage PostDoSendOrder(In entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, InContract.SendOrderToPTL(entity).ToMvcJson());
            return response;
        }

        #endregion


        /// <summary>
        /// 导入入库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "导入入库信息")]
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
                var tb = Bussiness.Common.ExcelHelper.ReadExeclToDataTable("sheet1", 0, file.InputStream);
                if (tb.Rows.Count <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("Excel无内容").ToMvcJson());
                }

                Bussiness.Entitys.In inEntity = new Bussiness.Entitys.In();
                inEntity.BillCode = tb.Rows[0]["来源单号"].ToString();
                inEntity.WareHouseCode = tb.Rows[0]["仓库编码"].ToString();
                if (!WareHouseContract.WareHouses.Any(a=>a.Code==inEntity.WareHouseCode))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("仓库编码:" + inEntity.WareHouseCode + "系统不存在").ToMvcJson());
                }
                inEntity.CreatedTime = DateTime.Now;
                inEntity.IsDeleted = false;
                inEntity.InDict = DictionaryContract.Dictionaries.Where(a => a.TypeCode == "InType").FirstOrDefault().Code;
                inEntity.Status = 0;
                inEntity.AddMaterial = new List<Bussiness.Entitys.InMaterial>();
                inEntity.Code = SequenceContract.Create(inEntity.GetType());
                int i = 0;
                foreach (DataRow item in tb.Rows)
                {
                    Bussiness.Entitys.InMaterial inmaterial = new Bussiness.Entitys.InMaterial();
                    inmaterial.BatchCode = item["批次"].ToString();
                    inmaterial.BillCode = inEntity.BillCode;
                    inmaterial.IsDeleted = false;
                    inmaterial.ItemNo = (i + 1).ToString().PadLeft(6, '0');
                    inmaterial.MaterialCode = item["物料编码"].ToString();
                    if (MaterialContract.Materials.FirstOrDefault(a => a.Code == inmaterial.MaterialCode) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("物料编码:" + inmaterial.MaterialCode + "系统不存在").ToMvcJson());
                    }
                    else  // 存在就
                    {

                    }
                    inmaterial.SupplierCode = item["供应商编码"].ToString();
                    if (!string.IsNullOrEmpty(inmaterial.SupplierCode))
                    {
                        if (SupplyContract.Supplys.FirstOrDefault(a => a.Code == inmaterial.SupplierCode) == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("供应商编码:" + inmaterial.SupplierCode + "系统不存在").ToMvcJson());
                        }
                    }
                    inmaterial.InCode = inEntity.Code;
                    inmaterial.LocationCode = "";
                    inmaterial.ManufactrueDate = DateTime.Parse(item["生产日期"].ToString()) ;
                    // 核查生产日期
                    if (string.IsNullOrEmpty(inmaterial.ManufactrueDate.ToString()))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请输入物料生产日期或日期格式不正确，物料：" + inmaterial.MaterialCode).ToMvcJson());
                    }
                    inmaterial.Quantity = Convert.ToDecimal(item["数量"].ToString());
                    // 核查导入数量
                    if (inmaterial.Quantity<=0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请输入物料数量，物料：" + inmaterial.MaterialCode).ToMvcJson());
                    }
                    inmaterial.Status = 0;
                    inEntity.AddMaterial.Add(inmaterial);
                }
                return Request.CreateResponse(HttpStatusCode.OK, InContract.CreateInEntity(inEntity).ToMvcJson());
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Equals("输入字符串的格式不正确。"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        DataProcess.Failure("数量字段中含有非法字符，请核查数量格式后重新导入").ToMvcJson());
                }
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
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
                string path = "/Assets/themes/Excel/入库单导入模版.xlsx";
                string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                var stream = new FileStream(filePath, FileMode.Open);
                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                //response.Content = new StreamContent(stream);
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = "入库单.xlsx"
                //};
                //return response;

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"入库单导入模版.xlsx";
                return result;

            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}

