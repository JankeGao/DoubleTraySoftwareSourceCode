using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bussiness.Contracts;
using Bussiness.Entitys;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Api.Interceptor;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    public class DeviceController : BaseApiController
    {
       // public IRepository<HPC.Video.Models.Device, int> VideoRepository { set; get; }

       /// <summary>
       /// 设备契约
       /// </summary>
        public IDeviceContract DeviceContract { get; set; }


        [LogApiFilter(Type = LogType.Login, Name = "获取设备信息")]
        [HttpGet]
        public HttpResponseMessage GetVideoByAreaId(int AreaId, [FromUri]MvcPageCondition pageCondition)
        {
            var query = DeviceContract.Devices;
            var pageResult = query.ToPage(pageCondition).ToMvcJson();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, pageResult);
            return response;
        }


        #region  SMT 电子料塔 - 盛来
        /// <summary>
        /// 同步设备信息
        /// </summary>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        [LogApiFilter(Type = LogType.Login, Name = "获取设备信息")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PostSynchronizationDevice(Device entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        # endregion


        [HttpPost]
        [LogApiFilter(Type = LogType.Operate, Name = "创建设备信息")]
        public HttpResponseMessage PostDoCreate(Device entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DeviceContract.CreateDevice(entity).ToMvcJson());
            return response;
        }

        //[LogApiFilter(Type = LogType.Login, Name = "获取设备信息")]
        //[HttpGet]
        //public HttpResponseMessage GetPageRecords([FromUri]MvcPageCondition pageCondition)
        //{
        //    try
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, DeviceContract.DeviceVMs.ToPage(pageCondition).ToMvcJson());
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}
        [System.Web.Http.HttpPost]
        [LogApiFilter(Type = LogType.Operate, Name = "删除设备信息")]
        public HttpResponseMessage PostDoEdit(Device entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DeviceContract.EditDevice(entity).ToMvcJson());
            return response;
        }
        [System.Web.Http.HttpPost]
        [LogApiFilter(Type = LogType.Operate, Name = "删除设备信息")]
        public HttpResponseMessage PostDoDelete(Device entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DeviceContract.RemoveDevice(entity.Id).ToMvcJson());
            return response;
        }



    }
}