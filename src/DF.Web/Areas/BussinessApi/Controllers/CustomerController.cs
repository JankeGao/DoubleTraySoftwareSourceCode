using System;
using System.Collections.Generic;
using System.Data;
using System.EnterpriseServices;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    [Description("客户管理")]//描述标签
    public class CustomerController : BaseApiController
    {
        /// <summary>
        /// 客户信息
        /// </summary>
        public Bussiness.Contracts.ICustomerContract CustomerContract { set; get; }

        #region 首页
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "客户信息")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = CustomerContract.Customers.Where(a =>a.IsDeleted == false);
            //查询条件，根据客户名称查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value) || p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);
            }

            var list = query.ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 创建新客户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "创建新客户信息 ")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.Customer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, CustomerContract.CreateCustomer(entity).ToMvcJson());
        }

        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑客户信息")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(Bussiness.Entitys.Customer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, CustomerContract.EditCustomer(entity).ToMvcJson());
        }

        /// <summary>
        /// 根据ID删除客户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除客户信息")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.Customer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, CustomerContract.DeleteCustomer(entity.Id).ToMvcJson());
        }

        /// <summary>
        /// 根据查询条件获取客户信息
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取客户信息")]
        [HttpGet]
        public HttpResponseMessage GetCustomersList(string KeyValue)
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                CustomerContract.Customers.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue)).Take(20)
                    .ToList().ToMvcJson());
        }
        #endregion

        /// <summary>
        /// 下载客户表
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
                result.Content.Headers.ContentDisposition.FileName = $"客户导入模板.xlsx";
                return result;
            }
            catch 
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }


        /// <summary>
        /// 导入客户的信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "客户信息的导入")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage DoUpLoadCustomerInfo()
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
                foreach (DataRow item in tb.Rows)
                {
                    Bussiness.Entitys.Customer entity = new Bussiness.Entitys.Customer();

                    Console.WriteLine(item);

                    if (string.IsNullOrEmpty(item["客户编码"].ToString()) || string.IsNullOrEmpty(item["客户名称"].ToString()))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            DataProcess.Failure("导入的文件中含有”客户编码“或”客户名称“为空的数据，请先确保客户的编码或名称不为空，再进行导入！"));
                    }
                    entity.Code = item["客户编码"].ToString();
                    var list = CustomerContract.Customers.Where(a => a.Code == entity.Code);
                    if (list.Count() > 0)
                    {
                        if (list.Any(a => a.IsDeleted == false))
                        {
                            string result = "客户编码：" + item["客户编码"].ToString() + "已存在";
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(string.Format(result)));
                        }
                    }

                    entity.Code = item["客户编码"].ToString();
                    entity.Name = item["客户名称"].ToString();
                    entity.Linkman = item["联系人"].ToString();
                    entity.Phone = item["联系方式"].ToString();
                    entity.Address = item["地址"].ToString();
                    entity.Remark = item["备注"].ToString();
                    
                    CustomerContract.CustomerRepository.Insert(entity);

                }
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success());
            }
            catch (System.Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
            }

        }

        /// <summary>
        /// 导出客户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage ExportClientData([FromUri] MvcPageCondition pageCondition)
        {
            var query = CustomerContract.Customers.Where(a => a.IsDeleted == false);
            //查询条件，根据客户名称查询
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
                {"Code","客户编码"},
                {"Name","客户名称"},
                {"Linkman","客户联系人"},
                {"Phone","客户电话"},
                {"Address","客户住址"},
                { "Remark","客户描述备注"},

            };

            var fileName = "客户信息.xlsx";
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
                result.Content.Headers.ContentDisposition.FileName = $"客户基本信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }





    }
}