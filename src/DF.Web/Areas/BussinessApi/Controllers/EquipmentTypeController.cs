using System;
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
using HP.Utility.Extensions;
using HP.Utility.Files;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Resources;
using Bussiness.Enums;
using NPOI.Util;
using System.Collections.Generic;
using Bussiness.Resources;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 主要的api借口
    /// </summary>
    [Description("设备型号的管理")]
    public class EquipmentTypeController : BaseApiController
    {
        /// <summary>
        /// 设备型号的信息
        /// </summary>
        public Bussiness.Contracts.IEquipmentTypeContract EquipmentTypeContract { set; get; }

        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取设备型号信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var query = EquipmentTypeContract.EquipmentType.Where(a=>a.IsDeleted==false);
            // 查询条件，根据编码和品牌查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var filterRuleType = pageCondition.FilterRuleCondition.Find(a => a.Field == "Type");
            if (filterRuleType != null)
            {
                int value = Convert.ToInt32(filterRuleType.Value.ToString());
                query = query.Where(p => p.Type == value);
                pageCondition.FilterRuleCondition.Remove(filterRuleType);

            }
            var filterRuleBrand = pageCondition.FilterRuleCondition.Find(a => a.Field == "Brand");
            if (filterRuleBrand != null)
            {
                int value = Convert.ToInt32(filterRuleBrand.Value.ToString());
                query = query.Where(p => p.Brand == value);
                pageCondition.FilterRuleCondition.Remove(filterRuleBrand);

            }
            //以倒叙方式查询显示
            var list = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK,list.ToMvcJson());

            Console.WriteLine(list);

            return response;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "添加数据时调用")]
        [HttpPost]
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.EquipmentType entity)
        {
            
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, EquipmentTypeContract.CreateEquipmentType(entity).ToMvcJson());
            return response;
        }
        
        /// <summary>
        /// 编辑信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑设备型号信息")]
        [HttpPost]
        public HttpResponseMessage PostDoEdit(Bussiness.Entitys.EquipmentType entity)
        {
            Console.WriteLine(entity);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, EquipmentTypeContract.EditEquipmentType(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除设备信息")] 
        [HttpPost]            
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.EquipmentType entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, EquipmentTypeContract.DeleteEquipmentType(entity.Id).ToMvcJson());
            return response;
        }

        #endregion

        /// <summary>
        /// 从WebAPI下载文件，设备型号模板的下载
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTemp()     
        {
            try
            {
                string path = "/Assets/themes/Excel/EquipmentType.xlsx";
                string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                var stream = new FileStream(filePath, FileMode.Open);             
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "设备型号的导入模版.xlsx"
                };
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }


        /// <summary>
        /// 导入设备型号的信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "设备型号信息的导入")]
        [HttpPost]
        public HttpResponseMessage DoUpLoadInfo()
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
                    Bussiness.Entitys.EquipmentType entity = new Bussiness.Entitys.EquipmentType();

                    Console.WriteLine(item);
                    
                    if (string.IsNullOrEmpty(item["设备型号"].ToString()) || string.IsNullOrEmpty(item["型号描述"].ToString()))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            DataProcess.Failure("导入的文件中含有”设备型号“或”型号描述“为空的数据，请先确保设备型号或型号描述不为空，再进行导入！"));
                    }
                    entity.Code = item["设备型号"].ToString();
                    var list = EquipmentTypeContract.EquipmentType.Where(a => a.Code == entity.Code);
                    if (list.Count()>0)
                    {
                        if (list.Any(a =>a.IsDeleted == false))
                        {
                            string result = "设备型号：" + item["型号描述"].ToString() + "已存在";
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(string.Format(result)));
                        }
                    }
                   
                    entity.Code = item["设备型号"].ToString(); 
                    entity.Remark = item["型号描述"].ToString();
                    entity.Brand = item["品牌"].ToInt();                               
                    entity.Type = item["类型"].ToInt();                                                           
                    entity.IsDeleted = false;                   
                    EquipmentTypeContract.EquipmentTypeRepository.Insert(entity);

                }
                return Request.CreateResponse(HttpStatusCode.OK,DataProcess.Success());
            }
            catch (System.Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
            }

        }

        /// <summary>
        /// 导出设备型号信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage ExportInformation()
        {
            var list = EquipmentTypeContract.EquipmentType.Where(a => !a.IsDeleted).ToList();
            var divFields = new Dictionary<string,string>//显示的字段与名称
            {
                {"Code","设备型号"},
                {"Brand","品牌"},
                {"Remark","型号描述"},
                {"Type","种类"},
                {"CreatedUserName" ,"添加信息"},
                {"CreatedTime","添加日期" }
            };

            var fileName = "设备型号信息.xlsx";
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
                result.Content.Headers.ContentDisposition.FileName = $"设备型号基本信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// 设备型号图片文件上传
        /// </summary>
        /// <returns></returns>
        [LogFilter(Name = "图片上传")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage PostDoFileLibraryUpload()
        {

            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            if (files.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请选择要上传的文件！").ToMvcJson());
            }

            var file = files[0];
            var fileContentType = file.ContentType;
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtensionName = Path.GetExtension(file.FileName);
            var fileSize = file.ContentLength;


            if (fileSize / 1024.0 / 1024 > 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("文件大小不能超过5MB！").ToMvcJson());
            }

            var fileclass = string.Empty;
            var inputStream = file.InputStream;   //准备读写文件内容
            byte[] bytes = new byte[inputStream.Length];
            inputStream.Read(bytes, 0, bytes.Length);

            if (bytes.Count() > 2)
            {
                fileclass = bytes[0].ToString() + bytes[1].ToString();
            }

            string filePath = string.Empty;

            if (fileclass != ((int)FileExtensionEnum.JPG).ToString()
                && fileclass != ((int)FileExtensionEnum.PNG).ToString()
                && fileclass != ((int)FileExtensionEnum.GIF).ToString())
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("仅支持JPG、PNG、GIF格式的图片！").ToMvcJson());
            }
            var FileLibrary_Picture = "/Assets/uploads/FileLibrary/Picture/{0}/{1}{2}";
            filePath = Bussiness.Resources.FileResource.FileLibrary_Box
                 .FormatWith( Guid.NewGuid().ToString(), fileExtensionName);

            var fileAbsolutePath = FileHelper.GetAbsolutePath(filePath);
            var path = Path.GetDirectoryName(fileAbsolutePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

#if DEBUG
            var FilePath = UploadURLResource.BASE_URL_DEV + filePath;
#endif
#if !DEBUG

            var FilePath = UploadURLResource.BASE_URL_PRO + filePath;
#endif
            //  FilePath = filePath;
            file.SaveAs(fileAbsolutePath);
            //返回保存路径
            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success(filePath).ToMvcJson());
        }

    }
}

