using System;
using System.IO;
using System.Net;
using System.Text;
using Bussiness.Contracts;
using Bussiness.Entitys;
using HP.Utility.Data;
using HP.Utility.Extensions;
using Newtonsoft.Json;

namespace Bussiness.Services
{
    public class ContrainerRunServer : IContrainerRunContract
    {
        public IWareHouseContract WareHouseContract { set; get; }


        /// <summary>
        /// 货柜自动运行--Post
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult StartRunningContainer(RunningContainer entity)
        {
            DataResult vresult = Validate(entity);
            if (!vresult.Success) return vresult;

            DataResult resultData = new DataResult();
            var container = WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode);

            // 货柜运行控制API
            var url = "http://"+container.Ip+":"+container.Port + "/" + "StartRunningContainer";

            string strData = entity.ToJsonString();
            string result = "";
            //将实时数据转发到一级平台
            HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
            wbRequest.Method = "POST";
            wbRequest.ContentType = "application/json";
            string paramData = strData;
            byte[] byteArray = Encoding.UTF8.GetBytes(paramData);
            wbRequest.ContentLength = byteArray.Length;

            try
            {
                using (Stream requestStream = wbRequest.GetRequestStream())
                {
                    using (StreamWriter swrite = new StreamWriter(requestStream))
                    {
                        swrite.Write(paramData);
                    }
                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                Stream stream = wbResponse.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                resultData = JsonConvert.DeserializeObject<DataResult>(result);
            }
            catch (Exception e)
            {
            }
            return resultData;
        }

        /// <summary>
        /// 路径行程设定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult HopperSetting(RunningContainer entity)
        {
            DataResult vresult = Validate(entity);
            if (!vresult.Success) return vresult;

            DataResult resultData = new DataResult();
            var container = WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode);

            // 报警复位
            var url = "http://" + container.Ip + ":" + container.Port + "/" + "HopperSetting";
            string result = "";
            //将实时数据转发到一级平台
            HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
            wbRequest.Method = "POST";
            wbRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                using (Stream requestStream = wbRequest.GetRequestStream())
                {

                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                Stream stream = wbResponse.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                resultData = JsonConvert.DeserializeObject<DataResult>(result);
            }
            catch (Exception e)
            {
            }
            return resultData;
        }

        /// <summary>
        /// 安全门行程设定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EmergencyDoorSetting(RunningContainer entity)
        {
            DataResult vresult = Validate(entity);
            if (!vresult.Success) return vresult;

            DataResult resultData = new DataResult();
            var container = WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode);

            // 报警复位
            var url = "http://" + container.Ip + ":" + container.Port + "/" + "EmergencyDoorSetting";
            string result = "";
            //将实时数据转发到一级平台
            HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
            wbRequest.Method = "POST";
            wbRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                using (Stream requestStream = wbRequest.GetRequestStream())
                {

                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                Stream stream = wbResponse.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                resultData = JsonConvert.DeserializeObject<DataResult>(result);
            }
            catch (Exception e)
            {
            }
            return resultData;
        }

        /// <summary>
        /// 获取PLC通讯状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult GetPlcDeivceStatus(RunningContainer entity)
        {
            DataResult vresult = Validate(entity);
            if (!vresult.Success) return vresult;

            DataResult resultData = new DataResult();
            var container = WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode);

            // 报警复位
            var url = "http://" + container.Ip + ":" + container.Port + "/" + "GetPlcDeivceStatus";
            string result = "";


            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            resultData = JsonConvert.DeserializeObject<DataResult>(result);

            return resultData;
        }


        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult GetAlarmInformation(RunningContainer entity)
        {
            DataResult vresult = Validate(entity);
            if (!vresult.Success) return vresult;

            DataResult resultData = new DataResult();
            var container = WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode);

            // 报警复位
            var url = "http://" + container.Ip + ":" + container.Port + "/" + "GetAlarmInformation";
            string result = "";


            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            resultData = JsonConvert.DeserializeObject<DataResult>(result);

            return resultData;
        }


        /// <summary>
        /// 报警复位-Post
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult ResetAlarm(RunningContainer entity)
        {
            DataResult vresult = Validate(entity);
            if (!vresult.Success) return vresult;

            DataResult resultData= new DataResult();
            var container = WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode);

            // 报警复位
            var url = "http://" + container.Ip + ":" + container.Port+"/"+ "ResetAlarm";
            string result = "";
            //将实时数据转发到一级平台
            HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
            wbRequest.Method = "POST";
            wbRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                using (Stream requestStream = wbRequest.GetRequestStream())
                {

                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                Stream stream = wbResponse.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                resultData = JsonConvert.DeserializeObject<DataResult>(result);
            }
            catch (Exception e)
            {
            }
            return resultData;
        }
        /// <summary>
        /// 扫描传递物料单重-Post
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult StartScanBarcodeKeyDown(RunningContainer entity)
        {
            DataResult vresult = Validate(entity);
            if (!vresult.Success) return vresult;

            DataResult resultData = new DataResult();
            var container = WareHouseContract.Containers.FirstOrDefault(a => a.Code == entity.ContainerCode);

            // 报警复位
            var url = "http://" + container.Ip + ":" + container.Port + "/" + "StartScanBarcodeKeyDown";
            string result = "";
            //将实时数据转发到一级平台
            HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
            wbRequest.Method = "POST";
            wbRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                using (Stream requestStream = wbRequest.GetRequestStream())
                {

                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                Stream stream = wbResponse.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                resultData = JsonConvert.DeserializeObject<DataResult>(result);
            }
            catch (Exception e)
            {
            }
            return resultData;
        }

        /// <summary>
        /// 验证数据合法性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private DataResult Validate(RunningContainer entity)
        {
            if (entity.ContainerCode.IsNullOrEmpty())
            {
                return DataProcess.Failure("货柜编码不能为空！");
            }
            return DataProcess.Success();
        }

    }


}
