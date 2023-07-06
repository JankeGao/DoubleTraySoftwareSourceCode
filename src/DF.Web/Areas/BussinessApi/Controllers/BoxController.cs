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
using HP.Utility.Data;
using HP.Utility.Extensions;
using HP.Utility.Files;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using Bussiness.Enums;
using System.Collections.Generic;
using Bussiness.Dtos;
using Bussiness.Resources;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 载具箱
    /// </summary>
    [Description("载具箱的管理")]
    public class BoxController : BaseApiController
    {
        /// <summary>
        /// 载具箱的信息
        /// </summary>
        public Bussiness.Contracts.IBoxContract BoxContract { set; get; }

        /// <summary>
        /// 物料主数据契约
        /// </summary>
        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }

   
        #region 首页

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取箱子信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        {
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "MaterialCode");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                var query1 = BoxContract.Box.InnerJoin(MaterialContract.MaterialBoxMapRepository.Query(), (box, map) => box.Code == map.BoxCode)
                    .InnerJoin(MaterialContract.Materials, (box, map, materail) => materail.Code == map.MaterialCode)
                    .Select((box, map, materail) => new BoxDto()
                    {
                        Id = box.Id,
                        Code = box.Code,
                        BoxColour = box.BoxColour,
                        BoxLength = box.BoxLength,
                        BoxWidth = box.BoxWidth,
                        FileID = box.FileID,
                        IsVirtual = box.IsVirtual,
                        IntroduceBox = box.IntroduceBox,
                        MaterialCode = materail.Code,
                        MaterialName = materail.Name,
                        Name = box.Name,
                        PictureUrl = box.PictureUrl,
                        Type = box.Type,
                        IsDeleted = box.IsDeleted,
                        CreatedUserCode = box.CreatedUserCode,
                        CreatedUserName = box.CreatedUserName,
                        CreatedTime = box.CreatedTime,
                        UpdatedUserCode = box.UpdatedUserCode,
                        UpdatedUserName = box.UpdatedUserName,
                        UpdatedTime = box.UpdatedTime,
                    });
                query1 = query1.Where(p => p.MaterialCode.Contains(value) || p.MaterialName.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

                // 查询条件，根据用户名称查询
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
                if (filterRule != null)
                {
                    string value1 = filterRule.Value.ToString();
                    query1 = query1.Where(p => p.Code.Contains(value1) || p.Name.Contains(value1));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }
                //以倒叙方式查询显示
                var proList = query1.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, proList.ToMvcJson());
                return response;
            }
            else
            {
                var query = BoxContract.Box.Where(a => a.IsDeleted == false);
                // 查询条件，根据用户名称查询
                filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.Code.Contains(value) || p.Name.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);
                }
                //以倒叙方式查询显示
                var proList = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, proList.ToMvcJson());
                return response;
            }
        }


        /// <summary>
        /// 获取单个箱子信息
        /// </summary>
        /// <param name="boxCode"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetBoxByCode(string boxCode)
        {
            var entity = BoxContract.Box.Where(a => a.IsDeleted == false&&a.Code==boxCode).FirstOrDefault();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, entity.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取载具绑定的物料
        /// </summary>
        /// <param name="boxCode"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetBoxMaterialMapByCode(string boxCode)
        {
            var l = MaterialContract.MaterialDtos.ToList();
            var list = MaterialContract.MaterialDtos.Where(a => a.BoxCode == boxCode).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());
            return response;
        }



        /// <summary>
        /// post方式获取箱子数据-添加数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public HttpResponseMessage PostDoCreate(Bussiness.Entitys.Box entity)  
        {
            
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, BoxContract.CreateBox(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 更改数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑箱子信息")] 
        [HttpPost]
        public HttpResponseMessage PostDoEdit(Bussiness.Entitys.Box entity)
        {        
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, BoxContract.EditBox(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 删除数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除箱子信息")] 
        [HttpPost]     
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.Box entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, BoxContract.DeleteBox(entity.Id).ToMvcJson());
            return response;
        }

        /// <summary>
        /// get方式获取数据
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取箱子信息")]
        [HttpGet]
        public HttpResponseMessage GetBoxlierList(string KeyValue)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, BoxContract.Box.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue)).Take(20).ToList().ToMvcJson());
            return response;
        }
        
        #endregion

        /// <summary>
        /// 从WebAPI下载文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTempBox()     //载具箱模板的下载
        {
            try
            {
                string path = "/Assets/themes/Excel/载具导入模版.xlsx";
                string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                var stream = new FileStream(filePath, FileMode.Open);             
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "载具箱的导入模版.xlsx"
                };
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }


        /// <summary>
        /// 导入载具箱的信息
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "载具箱信息的导入")]
        [HttpPost]
        public HttpResponseMessage DoUpLoadBoxInfo()
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
                    Bussiness.Entitys.Box entity = new Bussiness.Entitys.Box();

                    Console.WriteLine(item);
                    
                    if (string.IsNullOrEmpty(item["载具箱编码"].ToString()) || string.IsNullOrEmpty(item["载具箱名称"].ToString()))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            DataProcess.Failure("导入的文件中含有”载具箱编码“或”载具箱名称“为空的数据，请先确保载具箱的编码或名称不为空，再进行导入！"));
                    }
                    entity.Code = item["载具箱编码"].ToString();
                    var list = BoxContract.Box.Where(a => a.Code == entity.Code);
                    if (list.Count()>0)
                    {
                        if (list.Any(a =>a.IsDeleted == false))
                        {
                            string result = "载具箱编码：" + item["载具箱编码"].ToString() + "已存在";
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(string.Format(result)));
                        }
                    }
                   
                    entity.Name = item["载具箱名称"].ToString();
                    entity.Type = item["种类"].ToString();
                    entity.IntroduceBox= item["介绍"].ToString();
                    entity.BoxWidth = item["宽度"].ToInt();
                    entity.BoxLength = item["长度"].ToInt();
                    entity.IsVirtual = false;
                    entity.IsDeleted = false;                   
                    entity.BoxColour = item["颜色"].ToString();               
                    BoxContract.BoxRepository.Insert(entity);

                }
                return Request.CreateResponse(HttpStatusCode.OK,DataProcess.Success());
            }
            catch (System.Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
            }
         }

        /// <summary>
        /// 导出载具箱信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTemp()
        {
            var list = BoxContract.Box.Where(a => !a.IsDeleted).ToList();
            var divFields = new Dictionary<string,string>//显示的字段与名称
            {
                {"Code","载具箱编码"},
                {"Name","载具箱名称"},
                {"Type","种类"},
                {"IntroduceBox","介绍"},
                {"BoxWidth","宽度"},
                { "BoxLength","长度"},
                {"BoxColour","颜色" },
                {"CreatedTime","添加日期" }

            };

            var fileName = "载具箱信息.xlsx";
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
                result.Content.Headers.ContentDisposition.FileName = $"载具箱基本信息{System.DateTime.Now.ToString("yyyyMMdd")}.xls";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// 载具箱图片文件上传
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
            filePath = Bussiness.Resources. FileResource.FileLibrary_Box.FormatWith(Guid.NewGuid().ToString(), fileExtensionName);

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

