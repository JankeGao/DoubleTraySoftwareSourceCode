using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using HP.Core.Logging;
using HP.Data.Entity.Pagination;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    public class ContainerInitializationController : ApiController
    {
         //System.Configuration.ConfigurationSettings.AppSettings["PLCServerAddress"].ToString();
        public  string serverAddress = System.Configuration.ConfigurationManager.AppSettings["PLCServerAddress"].ToString();
        #region 垂直学习
        /// <summary>
        /// 开启垂直学习
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "开启垂直学习")]
        [HttpPost]
        public HttpResponseMessage StartM300()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM300", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 监视学习状态M340  true时学习结束
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "监视学习状态M340  true时学习结束")]
        [HttpGet]
        public HttpResponseMessage GetM340()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM340", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 读取前部托盘数
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "读取前部托盘数")]
        [HttpGet]
        public HttpResponseMessage GetD300()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetD300", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 读取后部托盘数
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "读取后部托盘数")]
        [HttpGet]
        public HttpResponseMessage GetD301()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetD301", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 结束垂直学习
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "结束垂直学习")]
        [HttpPost]
        public HttpResponseMessage FinishM341()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "FinishM341", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        #endregion

        #region 水平学习
        /// <summary>
        /// 开启水平学习
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "开启水平学习")]
        [HttpPost]
        public HttpResponseMessage StartM400()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM400", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 监视学习状态M340  true时学习结束
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "监视学习状态M340  true时学习结束")]
        [HttpGet]
        public HttpResponseMessage GetM440()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM440", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 结束水平学习
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "结束水平学习")]
        [HttpPost]
        public HttpResponseMessage FinishM441()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "FinishM441", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        #endregion

        #region 自动门学习
        /// <summary>
        /// 开启自动门学习
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "开启自动门学习")]
        [HttpPost]
        public HttpResponseMessage StartM450()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM450", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 监视学习状态M340  true时学习结束
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "监视学习状态M340  true时学习结束")]
        [HttpGet]
        public HttpResponseMessage GetM490()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM490", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 结束自动门学习
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "结束自动学习")]
        [HttpPost]
        public HttpResponseMessage FinishM491()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "FinishM491", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        #endregion

        #region 托盘扫描
        /// <summary>
        /// 开启托盘扫描
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "开启托盘扫描")]
        [HttpPost]
        public HttpResponseMessage StartM350()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM350", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取托盘扫描状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取托盘扫描状态")]
        [HttpGet]
        public HttpResponseMessage GetM390()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM390", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 读取托盘扫描前部托盘数
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "读取托盘扫描前部托盘数")]
        [HttpGet]
        public HttpResponseMessage GetD390()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetD390", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 读取托盘扫描后部托盘数
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "读取托盘扫描后部托盘数")]
        [HttpGet]
        public HttpResponseMessage GetD391()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetD391", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 开启托盘扫描  开始定义
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "开启托盘扫描  开始定义")]
        [HttpPost]
        public HttpResponseMessage StartM391()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM391", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取开始定义 后状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取开始定义 后状态")]
        [HttpGet]
        public HttpResponseMessage GetM392()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM392", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "写入托盘号")]
        [HttpPost]
        public HttpResponseMessage WriteD392(Bussiness.Common.RunningContainer runningContainer)
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "WriteD392", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 确认 写入托盘号完毕
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "确认 写入托盘号完毕")]
        [HttpPost]
        public HttpResponseMessage ConfirmM393()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "ConfirmM393", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        ///  确认完毕后 下一个
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "开启托盘扫描")]
        [HttpPost]
        public HttpResponseMessage ConfirmM394()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "ConfirmM394", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        ///  获取下一个 状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = " 获取下一个 状态")]
        [HttpGet]
        public HttpResponseMessage GetM395()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM395", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 结束托盘扫描
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "开启托盘扫描")]
        [HttpPost]
        public HttpResponseMessage ConfirmM396()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "ConfirmM396", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        #endregion

        #region 自动存取托盘
        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "写入托盘号")]
        [HttpPost]
        public HttpResponseMessage WriteD650(Bussiness.Common.RunningContainer runningContainer)
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "WriteD650", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取M650状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取M650状态")]
        [HttpGet]
        public HttpResponseMessage GetM650()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM650", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取D651 托盘所在托架号
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取D651 托盘所在托架号")]
        [HttpGet]
        public HttpResponseMessage GetD651()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetD651", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 启动M651
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "启动M651")]
        [HttpPost]
        public HttpResponseMessage StartM651()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM651", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取物料高度
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取物料高度")]
        [HttpGet]
        public HttpResponseMessage GetD652()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetD652", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        #endregion

        #region 添加托盘
        /// <summary>
        /// 添加托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "添加托盘")]
        [HttpPost]
        public HttpResponseMessage WriteD700(Bussiness.Common.RunningContainer runningContainer)
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "WriteD700", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 启动M700
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "启动M700")]
        [HttpPost]
        public HttpResponseMessage StartM700()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM700", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取M701状态 true 空间足够 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取M701状态 true 空间足够 ")]
        [HttpGet]
        public HttpResponseMessage GetM701()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM701", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 确认存入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "确认存入")]
        [HttpPost]
        public HttpResponseMessage ConfirmM702()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "ConfirmM702", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        #endregion

        #region 删除托盘
        /// <summary>
        /// 写入需要删除的托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "写入需要删除的托盘")]
        [HttpPost]
        public HttpResponseMessage WriteD750(Bussiness.Common.RunningContainer runningContainer)
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "WriteD750", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 启动M750
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "启动M750")]
        [HttpPost]
        public HttpResponseMessage StartM750()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM750", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取托盘所在货架号
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取托盘所在货架号")]
        [HttpGet]
        public HttpResponseMessage GetD751()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetD751", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 启动M751
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "启动M751")]
        [HttpPost]
        public HttpResponseMessage StartM751()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM751", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 监视托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "监视托盘")]
        [HttpGet]
        public HttpResponseMessage GetM752()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM752", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 确认删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "确认删除")]
        [HttpPost]
        public HttpResponseMessage ConfirmM753()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "ConfirmM753", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 取消删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "取消删除")]
        [HttpPost]
        public HttpResponseMessage ConfirmM754()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "ConfirmM754", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        #endregion

        #region 整理存储空间
        /// <summary>
        /// 开始整理
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "开始整理")]
        [HttpPost]
        public HttpResponseMessage StartM800()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartM800", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 监视整理是否完成
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "监视整理是否完成")]
        [HttpGet]
        public HttpResponseMessage GetM801()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetM801", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 获取空间利用率
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取空间利用率")]
        [HttpGet]
        public HttpResponseMessage GetD800()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "GetD800", "get", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        /// <summary>
        /// 写入需要删除的托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "确认整理完毕")]
        [HttpPost]
        public HttpResponseMessage ConfirmM802()
        {
            var result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "ConfirmM802", "post", "");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());
            return response;
        }
        #endregion
    }
}
