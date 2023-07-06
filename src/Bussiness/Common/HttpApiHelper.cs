using HP.Utility.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Common
{
    public class HttpApiHelper
    {
        public static DataResult InvokeWebapiApi(string url, string api, string type,string json)
        {
            ApiResult apiResult = new ApiResult() { Code = "" };
            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("authorization", "Basic YWRtaW46cGFzc3dvcmRAcmljZW50LmNvbQ==");//basic编码后授权码

            //client.DefaultRequestHeaders.Add();

            //request.Accept = "application/json";//"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //request.Headers["Accept-Language"] = "zh-CN,zh;q=0.9";
            ////   request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
            //request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36";
            //request.KeepAlive = true;
            ////上面的http头看情况而定，但是下面俩必须加 
            //request.ContentType = "application/json";
            //request.Headers["X-Locale"] = "chs";
            //request.Headers["X-User"] = "admin"; //登陆用户名
            //request.Headers["X-StayLoggedIn"] = "yes";//保持登陆状态
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("ContentType", "application/json");
            //client.DefaultRequestHeaders.Add("X-User", "admin");
            //client.DefaultRequestHeaders.Add("X-StayLoggedIn", "yes");

            client.BaseAddress = new Uri(url);

            client.Timeout = TimeSpan.FromSeconds(510);
            string result = "";
            if (type.ToLower() == "post")
            {
                // var content = new System.Net.Http.StringContent(json);
               // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var httpRequestMessage = new HttpRequestMessage();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                httpRequestMessage.Content = content;

                httpRequestMessage.RequestUri = new Uri(url+api);
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.Headers.Add("ContentType", "application/json");
                httpRequestMessage.Headers.Add("Accept", "application/json");

                var aaa =  httpRequestMessage.Content.ReadAsStringAsync();
                HttpResponseMessage response = client.SendAsync(httpRequestMessage).Result;
                apiResult.Code = ((int)response.StatusCode).ToString();
                apiResult.Content =  response.Content.ReadAsStringAsync().Result;
                if (apiResult.Code!="200")
                {
                    return DataProcess.Failure(apiResult.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<DataResult>(apiResult.Content);
                }
  
            }
            else if (type.ToLower() == "get")
            {
                HttpResponseMessage response = client.GetAsync(api).Result;
                result =  response.Content.ReadAsStringAsync().Result;
                apiResult.Code = ((int)response.StatusCode).ToString();
                apiResult.Content = result;
                if (apiResult.Code != "200")
                {
                    return DataProcess.Failure(apiResult.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<DataResult>(apiResult.Content);
                }
                // return apiResult;
                // return response;
                //if (response.IsSuccessStatusCode)
                //{
                //    string aa = response.Headers.GetValues("X-SID").FirstOrDefault();
                //    result = await response.Content.ReadAsStringAsync();
                //    return result;
                //}
            }
            else
            {
                return null;
            }
            return null;
        }
    }
}
