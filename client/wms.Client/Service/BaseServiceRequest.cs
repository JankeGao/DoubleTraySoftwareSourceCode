using System;
using System.Threading.Tasks;
using HP.Utility;
using Newtonsoft.Json;
using RestSharp;
using wms.Client.Model.RequestModel;

namespace wms.Client.Service
{
    public class BaseServiceRequest<T>
    {
        /// <summary>
        /// 带证书的http请求
        /// </summary>
        public HttpCertificateMethod certHttp = new HttpCertificateMethod();


        /// <summary>
        /// restSharp实例
        /// </summary>
        public RestSharpCertificateMethod restSharp = new RestSharpCertificateMethod();


        /// <summary>
        /// 获取证书获取字符串请求
        /// </summary> 
        /// <param name="dto"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        //public async Task<T> GetRequest(BaseRequest br, Method method = Method.GET)
        //{
        //    string pms = br.GetPropertiesObject();
        //    string url = br.route;
        //    if (!string.IsNullOrWhiteSpace(br.getParameter))
        //        url = br.route + br.getParameter;
        //    string resultString = await certHttp.RequestBehavior(method, url, pms);

        //    T result = JsonHelper.DeserializeObject<T>(resultString);//JsonConvert.DeserializeObject<T>(resultString);
        //    return result;
        //}


        /// <summary>
        /// T请求
        /// </summary>
        /// <param name="request">请求参数</param>
        /// <param name="method">方法类型</param>
        /// <returns></returns>
        public async Task<Response> GetRequest<Response>(BaseRequest request, RestSharp.Method method) where Response : class
        {
            string pms = request.GetPropertiesObject();
            string url = request.route;
       
            if (!string.IsNullOrWhiteSpace(request.getParameter))
                url += request.getParameter;
            Response result = await restSharp.RequestBehavior<Response>(url, method, pms);
            return result;
        }


        /// <summary>
        /// 请求
        /// </summary>
        /// <typeparam name="Response"></typeparam>
        /// <param name="url">地址</param>
        /// <param name="pms">参数</param>
        /// <param name="method">方法类型</param>
        /// <returns></returns>
        public async Task<Response> GetRequest<Response>(BaseRequest request, object obj, RestSharp.Method method) where Response : class
        {
            try
            {
                string pms = string.Empty;
                string url = request.route;
                if (!string.IsNullOrWhiteSpace(obj?.ToString())) pms = JsonConvert.SerializeObject(obj);
                Response result = await restSharp.RequestBehavior<Response>(url, method, pms,true);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }


}
