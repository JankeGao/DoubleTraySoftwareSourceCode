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
using HP.Core.Data;
using HP.Core.Logging;
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
    /// 
    /// </summary>
    [Description("物料管理")]
    public class MaterialController : BaseApiController
    {
        /// <summary>
        /// 物料信息
        /// </summary>
        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }
       
        /// <summary>
        /// 
        /// </summary>
        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }

        public IDictionaryContract DictionaryContract { set; get; }

        /// <summary>
        /// 载具箱仓储
        /// </summary>
        public IRepository<Box, int> BoxRepository { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取物料信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = MaterialContract.MaterialDtos.Where(a => a.IsDeleted == false);
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value) || p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Remark");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Remark1.Contains(value) || p.Remark2.Contains(value) || p.Remark3.Contains(value) || p.Remark4.Contains(value) || p.Remark5.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var list = query.OrderByDesc(a => a.Id).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建物料信息")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(MaterialDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MaterialContract.CreateMaterial(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑物料信息")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(MaterialDto entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MaterialContract.EditMaterial(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除物料信息")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.Material entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, MaterialContract.DeleteMaterial(entity.Id).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 导出物料信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoad([FromUri] MvcPageCondition pageCondition)
        {
            var query = MaterialContract.MaterialDtos.Where(a => a.IsDeleted == false);
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value) || p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Remark");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Remark1.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var list = query.ToList();
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"Code", "物料编码"},
                {"Name", "物料名称"},
                {"Unit", "物料单位"},
                {"MinNum", "最小库存"},
                {"MaxNum", "最大库存"},
                {"PackageQuantity", "单包装数量"},
                {"ValidityPeriod", "有效期"},
                {"MaterialType", "物料类别"},
                {"IsBatch", "是否批次管理"},
                {"IsNeedBlock", "是否存储锁定"},
                {"IsMaxBatch", "是否混批存放"},
                {"IsPackage", "是否单包管理"},
                {"UnitWeight", "单位重量"},
                {"CostCenter", "成本中心"},
                {"BoxCode", "载具编码"},
                {"Remark1", "备注1"},
                {"Remark2", "价格"},
                {"Remark3", "用途"},
                {"Remark4", "备注4"},
                {"Remark5", "备注5"},
            };

            var fileName = "物料信息.xlsx";
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
                result.Content.Headers.ContentDisposition.FileName = $"物料基本信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
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
                string path = "/Assets/themes/Excel/物料主数据导入模版.xlsx";
                string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                var stream = new FileStream(filePath, FileMode.Open);
                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                //response.Content = new StreamContent(stream);
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = "物料主数据.xlsx"
                //};
                //return response;

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"物料主数据导入模版.xlsx";
                return result;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }


        /// <summary>
        /// 导入入库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "导入物料主数据")]
        [HttpPost]
        public HttpResponseMessage DoUpLoadMaterialInfo()
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

                foreach (DataRow item in tb.Rows)
                {
                    MaterialDto entity = new MaterialDto();

                    entity.Code = item["物料编码"].ToString();
                    if (string.IsNullOrEmpty(entity.Code))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(("存在物料编码为空的数据，请核对后再导入")));
                    }
                    entity.Name = item["物料名称"].ToString();
                    if (string.IsNullOrEmpty(entity.Name))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(string.Format("导入的物料编码：{0}未有名称或名称为空，请核对后再导入", entity.Code)));
                    }

                    // 默认无先进先出
                    entity.FIFOType = 1;
                    entity.Unit = item["物料单位"].ToString();
                    entity.MaxNum = Convert.ToDecimal(item["最大库存"].ToString());
                    entity.MinNum = Convert.ToDecimal(item["最小库存"].ToString());
                    entity.PackageQuantity = Convert.ToDecimal(item["单包装数量"].ToString());
                    entity.ValidityPeriod = Convert.ToInt32(item["有效期"].ToString());
                    entity.MaterialType = Convert.ToInt32(item["物料类别"].ToString());

                    if (HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MaterialTypeEnum), entity.MaterialType.Value)==null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(string.Format("物料类别：{0}在系统中不存在，请核对后再导入", entity.MaterialType)));
                    }

                    entity.IsBatch = Convert.ToInt32(item["是否批次管理"].ToString()) == 1 ? true : false;
                    entity.IsNeedBlock = Convert.ToInt32(item["是否存储锁定"].ToString()) == 1 ? true : false;
                    entity.IsMaxBatch = Convert.ToInt32(item["是否混批存放"].ToString()) == 1 ? true : false;
                    entity.IsPackage = Convert.ToInt32(item["是否单包管理"].ToString()) == 1 ? true : false;
                    entity.UnitWeight = Convert.ToDecimal(item["单位重量"].ToString());
                    entity.CostCenter = item["成本中心"].ToString();
                    entity.Remark1 = item["备注1"].ToString();
                    entity.Remark2 = item["价格"].ToString();
                    entity.Remark3 = item["用途"].ToString();
                    entity.Remark4 = item["备注4"].ToString();
                    entity.Remark5= item["备注5"].ToString();
                    entity.CreatedTime = DateTime.Now;
                    entity.IsDeleted = false;

                    entity.BoxCode = item["载具类别编号"].ToString();
                    string boxCount= item["载具最大存放数量"].ToString();
                    if (!string.IsNullOrEmpty(boxCount))
                    {
                        entity.BoxCount = Convert.ToInt32(boxCount);
                    }
                    else
                    {
                        entity.BoxCount = 20;
                    }
                
                    // 如果存在则更新
                    if (MaterialContract.Materials.Any(a => a.Code == entity.Code))
                    {
                        //导入物料方法
                        var result = MaterialContract.EditMateraiInfo(entity);
                        if (!result.Success)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(result.ToMvcJson()));
                        }

                    }
                    else // 不存在则创建
                    {
                        //新建物料方法
                        var result = MaterialContract.CreateMaterial(entity);
                        if (!result.Success)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(result.ToMvcJson()));
                        }
                    }

                }
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success());
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
    }
}

