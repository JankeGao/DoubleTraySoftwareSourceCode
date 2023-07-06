using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Bussiness.Contracts;
using Bussiness.Entitys;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 货柜控制
    /// </summary>
    [Description("货柜控制")]
    public class ContrainerRunController : BaseApiController
    {
        /// <summary>
        /// 货柜控制
        /// </summary>
        public IContrainerRunContract ContrainerRunContract { set; get; }


        /// <summary>
        /// 运行货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [LogFilter(Type = LogType.Operate, Name = "运行货柜")]
        public HttpResponseMessage StartRunningContainer(RunningContainer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ContrainerRunContract.StartRunningContainer(entity).ToMvcJson());
        }

        /// <summary>
        /// 路径行程设定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [LogFilter(Type = LogType.Operate, Name = "路径行程设定")]
        public HttpResponseMessage HopperSetting(RunningContainer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ContrainerRunContract.HopperSetting(entity).ToMvcJson());
        }

        /// <summary>
        /// 安全门行程设定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [LogFilter(Type = LogType.Operate, Name = "安全门行程设定")]
        public HttpResponseMessage EmergencyDoorSetting(RunningContainer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ContrainerRunContract.EmergencyDoorSetting(entity).ToMvcJson());
        }

        /// <summary>
        /// 复位全部报警
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [LogFilter(Type = LogType.Operate, Name = "复位全部报警")]
        public HttpResponseMessage ResetAlarm(RunningContainer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ContrainerRunContract.ResetAlarm(entity).ToMvcJson());
        }

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet]
        [LogFilter(Type = LogType.Operate, Name = "获取设备状态")]
        public HttpResponseMessage GetPlcDeivceStatus([FromUri]RunningContainer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ContrainerRunContract.GetPlcDeivceStatus(entity).ToMvcJson());
        }

        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet]
        [LogFilter(Type = LogType.Operate, Name = "获取报警信息")]
        public HttpResponseMessage GetAlarmInformation([FromUri]RunningContainer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ContrainerRunContract.GetAlarmInformation(entity).ToMvcJson());
        }
        /// <summary>
        /// 扫描传递物料单重
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet]
        [LogFilter(Type = LogType.Operate, Name = "获取报警信息")]
        public HttpResponseMessage StartScanBarcodeKeyDown([FromUri] RunningContainer entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ContrainerRunContract.StartScanBarcodeKeyDown(entity).ToMvcJson());
        }

    }
}

