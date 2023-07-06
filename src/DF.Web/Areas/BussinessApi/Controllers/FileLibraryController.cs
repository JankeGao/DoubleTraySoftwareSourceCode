using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HP.Utility.Files;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Resources;
using Bussiness.Enums;
using Bussiness.Entitys;
using HP.Data.Orm.Extensions;
using Bussiness.Contracts;
using Bussiness.Resources;
using System.ComponentModel;
using System.Data;
using System.Net.Http.Headers;
using HP.Data.Orm;
using HP.Web.Mvc.Interceptor;
using NPOI.Util;
using System.Collections.Generic;
using HPC.BaseService.Dtos;
using HPC.BaseService.Contracts;
using HP.Core.Data;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class FileLibraryController : BaseApiController
    {
        #region 契约

        /// <summary>
        /// 文件库契约
        /// </summary>
        public IFileLibraryContract FileLibraryContract { set; get; }

        #endregion


        #region 查询

        /// <summary>
        /// 获取文件库List或分页
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        public HttpResponseMessage GetFileLibraryListOrPages([FromUri]MvcPageCondition pageCondition)
        {
            try
            {
                // 获取产品图片信息
                var query = FileLibraryContract.FileLibrarys.Where(a => a.CategoryId == 0);
                // 查询条件，根据用户名称查询
                var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "FileName");
                if (filterRule != null)
                {
                    string value = filterRule.Value.ToString();
                    query = query.Where(p => p.Code.Contains(value) || p.FileName.Contains(value));
                    pageCondition.FilterRuleCondition.Remove(filterRule);

                }

                var proList = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, proList.ToMvcJson());
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
        public IIdentityContract IdentityContract { get; set; }


        public IRepository<Bussiness.Entitys.UserSetting, int> UserSettingRepository { get; set; }
        #region 文件上传

        /// <summary>
        /// 文件库文件上传
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        //[LogFilte(Name = "文件库文件上传")]
        [HttpPost]
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


            if (fileSize / 1024.0 / 1024 / 5> 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("文件大小不能超过5MB！").ToMvcJson());
            }

            var fileclass = string.Empty;
            var inputStream = file.InputStream;
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
            filePath = Bussiness.Resources.FileResource.FileLibrary_Picture
                .FormatWith( Guid.NewGuid().ToString(), fileExtensionName);
            var fileAbsolutePath = FileHelper.GetAbsolutePath(filePath);
            var path = Path.GetDirectoryName(fileAbsolutePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

#if DEBUG
            var fileLibraryEntity = new FileLibrary
            {
                CategoryId = 0, // 产品
                Code = fileName,
                FileName = fileName,
                ExtensionName = fileExtensionName,
                ContentType = fileContentType,
                Size = fileSize,
                FilePath = filePath
                //UploadURLResource.BASE_URL_DEV  服务器访问地址
            };
#endif
#if !DEBUG
            var fileLibraryEntity = new FileLibrary
            {
                CategoryId = 0,// 产品
                Code = fileName,
                FileName = fileName,
                ExtensionName = fileExtensionName,
                ContentType = fileContentType,
                Size = fileSize,
                FilePath = UploadURLResource.BASE_URL_PRO + filePath
            };
#endif

            FileLibrary filelibraryEntity = new FileLibrary
            {
                CategoryId = 0,// 产品
                Code = fileName,
                FileName = fileName,
                ExtensionName = fileExtensionName,
                ContentType = fileContentType,
                Size = fileSize,
                FilePath = filePath
            };

            var result = FileLibraryContract.FileLibraryUpload(filelibraryEntity);
            if (result.Success)
            {
                file.SaveAs(fileAbsolutePath);
            }

            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success(filelibraryEntity).ToMvcJson());
        } 
        
        
        /// <summary>
          /// 人脸照片上传
          /// </summary>
          /// <returns></returns>
        [AllowAnonymous]
        //[LogFilte(Name = "文件库文件上传")]
        [HttpPost]
        public HttpResponseMessage PostDoFaceLibraryUpload()
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


            if (fileSize / 1024.0 / 1024 / 5.21 > 1) //  200K
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("文件大小不能超过200K！").ToMvcJson());
            }

            var fileclass = string.Empty;
            var inputStream = file.InputStream;
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
            filePath = Bussiness.Resources.FileResource.FileLibrary_Face
                .FormatWith(Guid.NewGuid().ToString(), fileExtensionName);
            var fileAbsolutePath = FileHelper.GetAbsolutePath(filePath);
            var path = Path.GetDirectoryName(fileAbsolutePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

#if DEBUG
            var fileLibraryEntity = new FileLibrary
            {
                CategoryId = 0, // 产品
                Code = fileName,
                FileName = fileName,
                ExtensionName = fileExtensionName,
                ContentType = fileContentType,
                Size = fileSize,
                FilePath = filePath
                //UploadURLResource.BASE_URL_DEV  服务器访问地址
            };
#endif
#if !DEBUG
            var fileLibraryEntity = new FileLibrary
            {
                CategoryId = 0,// 产品
                Code = fileName,
                FileName = fileName,
                ExtensionName = fileExtensionName,
                ContentType = fileContentType,
                Size = fileSize,
                FilePath = UploadURLResource.BASE_URL_PRO + filePath
            };
#endif

            FileLibrary filelibraryEntity = new FileLibrary
            {
                CategoryId = 0,// 产品
                Code = fileName,
                FileName = fileName,
                ExtensionName = fileExtensionName,
                ContentType = fileContentType,
                Size = fileSize,
                FilePath = filePath
            };

            var result = FileLibraryContract.FileLibraryUpload(filelibraryEntity);
            if (result.Success)
            {
                file.SaveAs(fileAbsolutePath);
            }

            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success(filelibraryEntity).ToMvcJson());
        }






        /// <summary>
        /// 人脸照片上传
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        //[LogFilte(Name = "文件库文件上传")]
        [HttpPost]
        public HttpResponseMessage PostDoFaceLibraryUpload1()
        {
            int Id = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["Id"].ToString());


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


            if (fileSize / 1024.0 / 1024 / 5.21 > 1) //  200K
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("文件大小不能超过200K！").ToMvcJson());
            }

            var fileclass = string.Empty;
            var inputStream = file.InputStream;
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
            filePath = Bussiness.Resources.FileResource.FileLibrary_Face
                .FormatWith(Guid.NewGuid().ToString(), fileExtensionName);

            string imageStr = Convert.ToBase64String(bytes);
            string Remark = imageStr;
            var entity = UserSettingRepository.GetEntity(Id);
            entity.Remark = imageStr;

            UserSettingRepository.Update(entity);
            var fileAbsolutePath = FileHelper.GetAbsolutePath(filePath);
            var path = Path.GetDirectoryName(fileAbsolutePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

#if DEBUG
            var fileLibraryEntity = new FileLibrary
            {
                CategoryId = 0, // 产品
                Code = fileName,
                FileName = fileName,
                ExtensionName = fileExtensionName,
                ContentType = fileContentType,
                Size = fileSize,
                FilePath = filePath
                //UploadURLResource.BASE_URL_DEV  服务器访问地址
            };
#endif
#if !DEBUG
            var fileLibraryEntity = new FileLibrary
            {
                CategoryId = 0,// 产品
                Code = fileName,
                FileName = fileName,
                ExtensionName = fileExtensionName,
                ContentType = fileContentType,
                Size = fileSize,
                FilePath = UploadURLResource.BASE_URL_PRO + filePath
            };
#endif

            FileLibrary filelibraryEntity = new FileLibrary
            {
                CategoryId = 0,// 产品
                Code = fileName,
                FileName = fileName,
                ExtensionName = fileExtensionName,
                ContentType = fileContentType,
                Size = fileSize,
                FilePath = filePath
            };

            var result = FileLibraryContract.FileLibraryUpload(filelibraryEntity);

    
            if (result.Success)
            {
                file.SaveAs(fileAbsolutePath);
            }
            filelibraryEntity.Remark = imageStr;
            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success(filelibraryEntity).ToMvcJson());
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogApiFilter(Type = LogType.Operate, Name = "文件信息移除")]     
        [HttpPost]
        public HttpResponseMessage PostDoRemove(FileLibrary entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, FileLibraryContract.RemoveFileLibrary(entity.Id).ToMvcJson());
            return response;
        }

        #endregion
    }
}
