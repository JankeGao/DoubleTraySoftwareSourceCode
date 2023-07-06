

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using wms.Client.Core.share.HttpContact;
using wms.Client.LogicCore.Configuration;
using DataResult = HP.Utility.Data.DataResult;

namespace wms.Client.Service
{
    /// <summary>
    /// RestSharp Client
    /// </summary>
    public class RestSharpCertificateMethod
    {
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求类型</param>
        /// <param name="pms">参数</param>
        /// <param name="isToken">是否Token</param>
        /// <param name="isJson">是否Json</param>
        /// <returns></returns>
        public async Task<Response> RequestBehavior<Response>(string url, RestSharp.Method method, string pms,
            bool isToken = true, bool isJson = true) where Response : class
        {
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(method);
            if (isToken)
            {
                client.AddDefaultHeader("token", Loginer.LoginerUser.Token);
            }
            switch (method)
            {
                case RestSharp.Method.GET:
                    request.AddHeader("Content-Type", "application/json");
                    break;
                case RestSharp.Method.POST:
                    if (isJson)
                    {
                        request.AddHeader("Content-Type", "application/json;charset=UTF-8");
                        request.AddJsonBody(pms);
                    }
                    else
                    {
                        request.AddHeader("Content-Type", "application/json");
                        request.AddParameter("application/x-www-form-urlencoded",
                            pms, ParameterType.RequestBody);
                    }
                    break;
                case RestSharp.Method.PUT:
                    request.AddHeader("Content-Type", "application/json");
                    break;
                case RestSharp.Method.DELETE:
                    request.AddHeader("Content-Type", "application/json");
                    break;
                default:
                    request.AddHeader("Content-Type", "application/json");
                    break;
            }

            try
            {
              //  var response = await client.ExecuteAsync(request);
                var response =  client.ExecuteAsync(request).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<responstContent>(response.Content);
                    if (result.Content != null)
                    {
                        return JsonConvert.DeserializeObject<Response>(result.Content.ToString());
                    }
                    else
                    {
                        var resultDevice = JsonConvert.DeserializeObject<DataResult>(response.Content);
                        return JsonConvert.DeserializeObject<Response>(response.Content);
                    }

                }
                else
                    return new DataResult
                    {
                        Success=false,
                        ResultType = (int)response.StatusCode,
                        Message = string.IsNullOrWhiteSpace(response.StatusDescription) ? $"Error:{response.ErrorMessage}"
                            : response.StatusDescription
                    } as Response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public class responstContent
        {
            public string Content { get; set; }

            public string ContentType { get; set; }
        }
    }
}
