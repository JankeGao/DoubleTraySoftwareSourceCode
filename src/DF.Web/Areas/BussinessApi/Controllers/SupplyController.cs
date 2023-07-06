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
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using NPOI.Util;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    [Description("供应商管理")]
    public class SupplyController : BaseApiController
    {
        /// <summary>
        /// 供应商信息
        /// </summary>
        public Bussiness.Contracts.ISupplyContract SupplyContract { set; get; }

        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取供应商信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = SupplyContract.Supplys.Where(a=>a.IsDeleted==false);
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value)|| p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var list = query.ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,list.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建供应商信息")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.Supply entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, SupplyContract.CreateSupply(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑供应商信息")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(Bussiness.Entitys.Supply entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, SupplyContract.EditSupply(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除供应商信息")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.Supply entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, SupplyContract.DeleteSupply(entity.Id).ToMvcJson());
            return response;
        }

        /// <summary>
        /// post
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取供应商信息")]
        [HttpGet]
        public HttpResponseMessage GetSupplierList(string KeyValue)
        {
            var list = SupplyContract.Supplys.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue)).ToList();
            list = list.Where(a => !(bool)a.IsDeleted).Take(20).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }
        #endregion

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
                string path = "/Assets/themes/Excel/供应商.xlsx";
                string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                var stream = new FileStream(filePath, FileMode.Open);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"供应商导入模版.xlsx";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// 导出信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTempSupply([FromUri] MvcPageCondition pageCondition)
        {

            var query = SupplyContract.Supplys.Where(a => a.IsDeleted == false);
            // 查询条件，根据用户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value) || p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }           
            var list = query.ToList();
            var divFields = new Dictionary<string, string>//显示的字段与名称
            {
                {"Code","供应商编码"},
                {"Name","供应商名称"},
                {"Linkman","联系人"},
                {"Phone","联系方式"},
                {"Address","地址"},
                {"Remark","备注" },
            };
            var fileName = "供应商信息.xlsx";
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
                result.Content.Headers.ContentDisposition.FileName = $"供应商信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// 导入供应商信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "供应商导入")]
        [HttpPost]
        public HttpResponseMessage DoUpLoadSupplyInfo()
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
                //int i = 0;
                foreach (DataRow item in tb.Rows)
                {
                    Bussiness.Entitys.Supply entity = new Bussiness.Entitys.Supply();
                    
                    if (string.IsNullOrEmpty(item["供应商编码"].ToString()) || string.IsNullOrEmpty(item["供应商名称"].ToString()))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            DataProcess.Failure("导入的文件中含有”供应商编码“或”供应商名称“为空的数据，请先确保供应商的编码或名称不为空，再进行导入！"));
                    }
                    entity.Code = item["供应商编码"].ToString();
                    var list = SupplyContract.Supplys.Where(a => a.Code == entity.Code);
                    if (list.Count()>0)
                    {
                        if (list.Any(a =>a.IsDeleted == false))
                        {
                            string result = "供应商编码：" + item["供应商编码"].ToString() + "已存在";
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(string.Format(result)));
                        }
                    }
                   
                    entity.Name = item["供应商名称"].ToString();
                    entity.Linkman = item["联系人"].ToString();
                    entity.Phone = item["联系方式"].ToString();
                    entity.Address = item["地址"].ToString();
                    entity.IsDeleted = false;
                    SupplyContract.SupplyRepository.Insert(entity);
                }
                return Request.CreateResponse(HttpStatusCode.OK,DataProcess.Success());
            }
            catch (System.Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
            }

        }

    }
}

