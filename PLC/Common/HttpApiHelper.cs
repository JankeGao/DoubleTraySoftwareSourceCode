using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PLCServer.Common
{
    public class HttpApiHelper
    {
      //  public static string HttpApiLoginBegin(string url, string jsonstr, string type)
      //  {
            //：/BFC/Auth/LoginBegin 
            //Encoding encoding = Encoding.UTF8;
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//webrequest请求api地址  
            //request.Accept = "text/html,application/xhtml+xml,*/*";
            //request.ContentType = "application/json";
            //request.Method = type.ToUpper().ToString();//get或者post  
            //byte[] buffer = encoding.GetBytes(jsonstr);
            //request.ContentLength = buffer.Length;
            //request.GetRequestStream().Write(buffer, 0, buffer.Length);
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            //{
            //    return reader.ReadToEnd();
            //}

            //return "";
      //  }
      /// <summary>
      /// 初始登陆
      /// </summary>
      /// <param name="url"></param>
      /// <param name="jsonstr"></param>
      /// <param name="type"></param>
      /// <returns></returns>
        public static string HttpApiLoginBegin(string url, string jsonstr, string type)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); //--需要封装的参数
              //  request.CookieContainer = new CookieContainer();
              //  CookieContainer cookie = request.CookieContainer;//如果用不到Cookie，删去即可 
                                                                 //以下是发送的http头
               // request.Referer = "";
            
                request.Accept = "application/json";//"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers["Accept-Language"] = "zh-CN,zh;q=0.9";
             //   request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36";
                request.KeepAlive = true;
                //上面的http头看情况而定，但是下面俩必须加 
                request.ContentType = "application/json";
                request.Headers["X-Locale"] = "chs";
                request.Headers["X-User"] = "admin"; //登陆用户名
                request.Headers["X-StayLoggedIn"] = "yes";//保持登陆状态

                Encoding encoding = Encoding.UTF8;//根据网站的编码自定义
                request.Method = type;  //--需要将get改为post才可行
                string postDataStr;
                Stream requestStream = request.GetRequestStream();
                if (jsonstr != "")
                {
                    postDataStr = jsonstr;//--需要封装,形式如“arg=arg1&arg2=arg2”
                    byte[] postData = encoding.GetBytes(postDataStr);//postDataStr即为发送的数据，
                    request.ContentLength = postData.Length;
                    requestStream.Write(postData, 0, postData.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();


                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string retString = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                return retString;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        /// <summary>
        /// 登陆授权
        /// </summary>
        /// <param name="url"></param>
        /// <param name="jsonstr"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string HttpApiLoginAnswer(string url,string jsonstr,string type, string SSID, string password,string user)
        {
            var HashedPass = SHA1("IoT-ES:" + password);
            string answer = HMACSHA1Text(HashedPass, SSID);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); //--需要封装的参数
            request.CookieContainer = new CookieContainer();
            CookieContainer cookie = request.CookieContainer;//如果用不到Cookie，删去即可 
            //以下是发送的http头
            request.Referer = "";
            request.Accept = "application/json";//"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
            request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
            request.UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
            request.KeepAlive = true;
            //上面的http头看情况而定，但是下面俩必须加 
            request.ContentType = "application/json";//"application/x-www-form-urlencoded";
            request.Headers["X-Locale"] = "chs";
            request.Headers["X-User"] = user;
            request.Headers["X-Answer"] = answer;
            Encoding encoding = Encoding.UTF8;//根据网站的编码自定义
            request.Method = type;  //--需要将get改为post才可行
            string postDataStr;
            Stream requestStream = request.GetRequestStream();
            if (jsonstr != "")
            {
                postDataStr = jsonstr;//--需要封装,形式如“arg=arg1&arg2=arg2”
                byte[] postData = encoding.GetBytes(postDataStr);//postDataStr即为发送的数据，
                request.ContentLength = postData.Length;
                requestStream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();


            StreamReader streamReader = new StreamReader(responseStream, encoding);
            string retString = streamReader.ReadToEnd();

            streamReader.Close();
            responseStream.Close();
            return retString;
        }


        public static string HttpApiLpWaitMessage(string url, string jsonstr, string type)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); //--需要封装的参数
            request.CookieContainer = new CookieContainer();
            CookieContainer cookie = request.CookieContainer;//如果用不到Cookie，删去即可 
            //以下是发送的http头
            request.Referer = "";
            request.Accept = "application/json";//"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
            request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
            request.UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
            request.KeepAlive = true;
            //上面的http头看情况而定，但是下面俩必须加 
            request.ContentType = "application/json";

            Encoding encoding = Encoding.UTF8;//根据网站的编码自定义
            request.Method = type;  //--需要将get改为post才可行
            string postDataStr;
            Stream requestStream = request.GetRequestStream();
            if (jsonstr != "")
            {
                postDataStr = jsonstr;//--需要封装,形式如“arg=arg1&arg2=arg2”
                byte[] postData = encoding.GetBytes(postDataStr);//postDataStr即为发送的数据，
                request.ContentLength = postData.Length;
                requestStream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();


            StreamReader streamReader = new StreamReader(responseStream, encoding);
            string retString = streamReader.ReadToEnd();

            streamReader.Close();
            responseStream.Close();
            return retString;
        }



        public static string HttpApiCallNode(string url, string jsonstr, string type)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); //--需要封装的参数
            request.CookieContainer = new CookieContainer();
            CookieContainer cookie = request.CookieContainer;//如果用不到Cookie，删去即可 
            //以下是发送的http头
            request.Referer = "";
            request.Accept = "application/json";//"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
            request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
            request.UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
            request.KeepAlive = true;
            //上面的http头看情况而定，但是下面俩必须加 
            request.ContentType = "application/json";

            Encoding encoding = Encoding.UTF8;//根据网站的编码自定义
            request.Method = type;  //--需要将get改为post才可行
            string postDataStr;
            Stream requestStream = request.GetRequestStream();
            if (jsonstr != "")
            {
                postDataStr = jsonstr;//--需要封装,形式如“arg=arg1&arg2=arg2”
                byte[] postData = encoding.GetBytes(postDataStr);//postDataStr即为发送的数据，
                request.ContentLength = postData.Length;
                requestStream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();


            StreamReader streamReader = new StreamReader(responseStream, encoding);
            string retString = streamReader.ReadToEnd();

            streamReader.Close();
            responseStream.Close();
            return retString;
        }


        /// <summary>
        /// SHA1 加密，返回大写字符串
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <returns>返回40位UTF8 大写</returns>
        public static byte[] SHA1(string content)
        {
            return SHA1(content, Encoding.UTF8);
        }
        public static string SHA2(string content)
        {
            return SHA2(content, Encoding.UTF8);
        }
        /// <summary>
        /// SHA1 加密，返回大写字符串
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <param name="encode">指定加密编码</param>
        /// <returns>返回40位大写字符串</returns>
        public static byte[] SHA1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                //string result = BitConverter.ToString(bytes_out);
                //result = result.Replace("-", "");
                //return result;
                return bytes_out;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }

        public static string SHA2(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }


        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="text">要加密的原串</param>
        ///<param name="key">私钥</param>
        /// <returns></returns>
        public static string HMACSHA1Text(string text, string key)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(key);

            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            var enText = new StringBuilder();
            foreach (byte iByte in hashBytes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();
            //return Convert.ToBase64String(hashBytes);

        }
        public static string HMACSHA1Text(byte[] text, string key)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(key);

           // byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = hmacsha1.ComputeHash(text);

            var enText = new StringBuilder();
            foreach (byte iByte in hashBytes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();

        }
        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="text">要加密的原串</param>
        ///<param name="key">私钥</param>
        /// <returns></returns>
        //public static string HMACSHA1Text(string text, string key)
        //{
        //    //HMACSHA1加密
        //    HMACSHA1 hmacsha1 = new HMACSHA1();
        //    hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(key);

        //    byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(text);
        //    byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

        //    return Convert.ToBase64String(hashBytes);

        //}


        public static async Task<string> InvokeWebapiLoginBegin(string url, string api, string type, Dictionary<string, string> dics)
        {
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
            client.DefaultRequestHeaders.Add("X-Locale", "chs");
            client.DefaultRequestHeaders.Add("X-User", "admin");
            client.DefaultRequestHeaders.Add("X-StayLoggedIn", "yes");
            client.BaseAddress = new Uri(url);

            client.Timeout = TimeSpan.FromSeconds(510);
            string result = "";
            if (type.ToLower() == "put")
            {
                HttpResponseMessage response;
                //包含复杂类型
                if (dics.Keys.Contains("input"))
                {
                    if (dics != null)
                    {
                        foreach (var item in dics.Keys)
                        {
                            api = api.Replace(item, dics[item]).Replace("{", "").Replace("}", "");
                        }
                    }
                    var contents = new StringContent(dics["input"], Encoding.UTF8, "application/json");
                    response = client.PutAsync(api, contents).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                    return "";
                }

                var content = new FormUrlEncodedContent(dics);
                response = client.PutAsync(api, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    return "";
                }
            }
            else if (type.ToLower() == "post")
            {
                var content = new FormUrlEncodedContent(dics);

                HttpResponseMessage response = client.PostAsync(api, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            else if (type.ToLower() == "get")
            {
                HttpResponseMessage response = client.GetAsync(api).Result;

                if (response.IsSuccessStatusCode)
                {
                    string aa = response.Headers.GetValues("X-SID").FirstOrDefault();
                    result = await response.Content.ReadAsStringAsync();
                    return aa;
                }
            }
            else
            {
                return "";
            }
            return "";
        }


        public static async Task<string> InvokeWebapiApiLoginAnswer(string url, string api, string type, Dictionary<string, string> dics,string password,string SSID,string user)
        {
            HttpClient client = new HttpClient();
            var HashedPass = SHA2("IoT-ES:" + password);
            string answer = HMACSHA1Text(HashedPass, SSID);
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
            client.DefaultRequestHeaders.Add("X-Locale", "chs");
            client.DefaultRequestHeaders.Add("X-User", user);
            client.DefaultRequestHeaders.Add("X-Answer", answer);
            //client.DefaultRequestHeaders.Add("X-User", "admin");
            //client.DefaultRequestHeaders.Add("X-StayLoggedIn", "yes");

            client.BaseAddress = new Uri(url);

            client.Timeout = TimeSpan.FromSeconds(510);
            string result = "";
            if (type.ToLower() == "put")
            {
                HttpResponseMessage response;
                //包含复杂类型
                if (dics.Keys.Contains("input"))
                {
                    if (dics != null)
                    {
                        foreach (var item in dics.Keys)
                        {
                            api = api.Replace(item, dics[item]).Replace("{", "").Replace("}", "");
                        }
                    }
                    var contents = new StringContent(dics["input"], Encoding.UTF8, "application/json");
                    response = client.PutAsync(api, contents).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                    return "";
                }

                var content = new FormUrlEncodedContent(dics);
                response = client.PutAsync(api, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    return "";
                }
            }
            else if (type.ToLower() == "post")
            {
                var content = new FormUrlEncodedContent(dics);

                HttpResponseMessage response = client.PostAsync(api, content).Result;
                if (response.IsSuccessStatusCode && response.StatusCode== HttpStatusCode.OK)
                {
                    result = await response.Content.ReadAsStringAsync();
                    return "登陆成功";
                }
            }
            else if (type.ToLower() == "get")
            {
                HttpResponseMessage response = client.GetAsync(api).Result;

                if (response.IsSuccessStatusCode)
                {
                    string aa = response.Headers.GetValues("X-SID").FirstOrDefault();
                    result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            else
            {
                return "";
            }
            return "";
        }


        public static async Task<string> InvokeWebapiApiLoginOut(string url, string api, string type, Dictionary<string, string> dics, string password, string SSID, string user)
        {
            HttpClient client = new HttpClient();
            var HashedPass = SHA1("IoT-ES:" + password);
            string answer = HMACSHA1Text(HashedPass, SSID);
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
            client.DefaultRequestHeaders.Add("X-Locale", "chs");
            client.DefaultRequestHeaders.Add("X-User", user);
            client.DefaultRequestHeaders.Add("X-Answer", answer);
            //client.DefaultRequestHeaders.Add("X-User", "admin");
            //client.DefaultRequestHeaders.Add("X-StayLoggedIn", "yes");

            client.BaseAddress = new Uri(url);

            client.Timeout = TimeSpan.FromSeconds(510);
            string result = "";
            if (type.ToLower() == "put")
            {
                HttpResponseMessage response;
                //包含复杂类型
                if (dics.Keys.Contains("input"))
                {
                    if (dics != null)
                    {
                        foreach (var item in dics.Keys)
                        {
                            api = api.Replace(item, dics[item]).Replace("{", "").Replace("}", "");
                        }
                    }
                    var contents = new StringContent(dics["input"], Encoding.UTF8, "application/json");
                    response = client.PutAsync(api, contents).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                    return "";
                }

                var content = new FormUrlEncodedContent(dics);
                response = client.PutAsync(api, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    return "";
                }
            }
            else if (type.ToLower() == "post")
            {
                var content = new FormUrlEncodedContent(dics);

                HttpResponseMessage response = client.PostAsync(api, content).Result;
                if (response.IsSuccessStatusCode && response.StatusCode== HttpStatusCode.OK)
                {
                    result = await response.Content.ReadAsStringAsync();
                    return "退出成功";
                }
            }
            else if (type.ToLower() == "get")
            {
                HttpResponseMessage response = client.GetAsync(api).Result;

                if (response.IsSuccessStatusCode)
                {
                    string aa = response.Headers.GetValues("X-SID").FirstOrDefault();
                    result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            else
            {
                return "";
            }
            return "";
        }


        public static async Task<HttpResponseMessage> InvokeWebapiApiLpWaitMessage(string url, string api, string type, Dictionary<string, string> dics, string password, string SSID, string user,string cookie="")
        {
         

            var handler = new HttpClientHandler() { UseCookies = false };
            HttpClient client = new HttpClient(handler);
            //var HashedPass = SHA1("IoT-ES:" + password);
            //string answer = HMACSHA1Text(HashedPass, SSID);
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

            //client.DefaultRequestHeaders.Add("Accept", "application/json");
            //client.DefaultRequestHeaders.Add("ContentType", "application/json");
            //client.DefaultRequestHeaders.Add("X-Locale", "chs");
            //if (!string.IsNullOrEmpty(cookie))
            //{
            //    client.DefaultRequestHeaders.Add("Cookie", cookie);

            //}


            //client.DefaultRequestHeaders.Add("X-User", user);
            //client.DefaultRequestHeaders.Add("X-Answer", answer);
            //client.DefaultRequestHeaders.Add("X-User", "admin");
            //client.DefaultRequestHeaders.Add("X-StayLoggedIn", "yes");

            client.BaseAddress = new Uri(url);

            client.Timeout = TimeSpan.FromSeconds(510);
            //string result = "";
            if (type.ToLower() == "put")
            {
                HttpResponseMessage response;
                //包含复杂类型
                if (dics.Keys.Contains("input"))
                {
                    if (dics != null)
                    {
                        foreach (var item in dics.Keys)
                        {
                            api = api.Replace(item, dics[item]).Replace("{", "").Replace("}", "");
                        }
                    }
                    var contents = new StringContent(dics["input"], Encoding.UTF8, "application/json");
                    response = client.PutAsync(api, contents).Result;
                    //if (response.IsSuccessStatusCode)
                    //{
                    //    result = await response.Content.ReadAsStringAsync();
                    //    return result;
                    //}
                    //return "";
                    return response;
                }

                var content = new FormUrlEncodedContent(dics);
                response = client.PutAsync(api, content).Result;
                return response;
                //if (response.IsSuccessStatusCode)
                //{
                //    result = await response.Content.ReadAsStringAsync();
                //    return "";
                //}
            }
            else if (type.ToLower() == "post")
            {
                var content = new FormUrlEncodedContent(dics);

                HttpResponseMessage response = client.PostAsync(api, content).Result;
                //if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                //{
                //    result = await response.Content.ReadAsStringAsync();
                //    return "退出成功";
                //}
                return response;
            }
            else if (type.ToLower() == "get")
            {
                var message = new HttpRequestMessage(HttpMethod.Get, url+api);
                message.Headers.Add("Accept", "application/json");
                message.Headers.Add("ContentType", "application/json");
                message.Headers.Add("X-Locale", "chs");
                if (!string.IsNullOrEmpty(cookie))
                {
                    message.Headers.Add("Cookie",cookie);
                }
                HttpResponseMessage response = await client.SendAsync(message);
                // HttpResponseMessage response = client.GetAsync(api).Result;

                //if (response.IsSuccessStatusCode)
                //{
                //  //  string aa = response.Headers.GetValues("X-SID").FirstOrDefault();
                //    result = await response.Content.ReadAsStringAsync();
                //    return result;
                //}
                return response;
            }
            else
            {
                return null;
            }
            //return null;
        }
        /// <summary>
        /// 向货架发送命令
        /// </summary>
        public static async Task<ApiResult> InvokeWebApiCallNode(string url, string type, Dictionary<string, string> dics, string OperatorCode, string Rack, int FromQuantity = 0, int ToQuantity = 0, string LocationCode = "", string api = @"/BFC/Rack/CallNode")
        {
            ApiResult apiResult = new ApiResult() { Code = "" };
            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Accept", "text/ini");//application/json
            //client.DefaultRequestHeaders.Add("ContentType", "text/ini");
            //client.DefaultRequestHeaders.Add("X-Locale", "chs");
            //client.DefaultRequestHeaders.Add("X-User", user);
            //client.DefaultRequestHeaders.Add("X-Answer", answer);
            //client.DefaultRequestHeaders.Add("X-User", "admin");
            //client.DefaultRequestHeaders.Add("X-StayLoggedIn", "yes");
            //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");
            client.BaseAddress = new Uri(url);
            
            client.Timeout = TimeSpan.FromSeconds(510);
            string result = "";
            if (type.ToLower() == "put")
            {
                HttpResponseMessage response;
                //包含复杂类型
                if (dics.Keys.Contains("input"))
                {
                    if (dics != null)
                    {
                        foreach (var item in dics.Keys)
                        {
                            api = api.Replace(item, dics[item]).Replace("{", "").Replace("}", "");
                        }
                    }
                    var contents = new StringContent(dics["input"], Encoding.UTF8, "application/json");
                    response = client.PutAsync(api, contents).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                        apiResult.Code = "200";
                        apiResult.Content = result;
                        return apiResult;
                    }
                    return apiResult;
                }

                var content = new FormUrlEncodedContent(dics);
                response = client.PutAsync(api, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    apiResult.Code = "200";
                    apiResult.Content = result;
                    return apiResult;
                }
                return apiResult;
            }
            else if (type.ToLower() == "post")
            {


                //PostMethod post = new PostMethod(url);
                //  var config = GetCConfig(OperatorCode, Rack, FromQuantity, ToQuantity, LocationCode);
                //dto.Args = "";

                //cn.baiy.byConfig byConfig = new cn.baiy.byConfig();
                //byConfig.WriteString("", "Rack", "A002");
                //byConfig.WriteString("", "api", "Pllet/RetriveStatusList");
                //result = HttpApiCallNode("http://127.0.0.1:50001/BFC/Rack/CallNode", JsonConvert.SerializeObject(dics), "post");
                //  var content = new StringContent(JsonConvert.SerializeObject(dics)); //FormUrlEncodedContent(dics);
                //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                //cn.baiy.byConfig byArgsConfig = new cn.baiy.byConfig();
                //byArgsConfig.WriteString("", "Number", "B01/01");
                //byArgsConfig.WriteDWord("", "Current", (uint)1);
                //byArgsConfig.WriteDWord("", "Justness", (uint)1);

                ////cn.baiy.byConfig byArgsConfig = new cn.baiy.byConfig();
                ////byArgsConfig.WriteString("", "Number", "B01/01");
                ////byArgsConfig.WriteDWord("", "From", (uint)1);
                ////byArgsConfig.WriteDWord("", "To", (uint)1);

                //cn.baiy.byConfig byConfig = new cn.baiy.byConfig();
                //byConfig.WriteString("", "Rack", "A002");
                ////  byConfig.WriteString("", "api", "Pallet/RetriveStatusList");
                //byConfig.WriteString("", "api", "Pallet/SetCorrectQuantity");// SetPreAuthDelta //SetCorrectQuantity
                //byConfig.Insert("Args", byArgsConfig);
                byte[] data = new byte[6];//config.SaveJson();
                //string data = JsonConvert.SerializeObject();
                //  string data = JsonConvert.SerializeObject(dto);
                //public Stream BytesToStream(byte[] bytes)
                //{
                //    Stream stream = new MemoryStream(bytes);
                //    return stream;
                //}

                //var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var httpRequestMessage = new HttpRequestMessage();
                var byteContent = new StreamContent(new MemoryStream(data));
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                httpRequestMessage.Content = byteContent;

                httpRequestMessage.RequestUri = new Uri("http://localhost:50001/BFC/Rack/CallNode");
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.Headers.Add("ContentType", "application/json");
                httpRequestMessage.Headers.Add("Accept", "application/json");
                httpRequestMessage.Headers.Add("X-Locale", "chs");
              //  var aaa = await httpRequestMessage.Content.ReadAsStringAsync();
                HttpResponseMessage response = client.SendAsync(httpRequestMessage).Result;
                //HttpResponseMessage response = client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:50001/BFC/Rack/CallNode") { Content = content }).Result; 
                result = await response.Content.ReadAsStringAsync();
                apiResult.Code = ((int)response.StatusCode).ToString();
                apiResult.Content = result;
                return apiResult;
                //HttpResponseMessage response = client.PostAsync(api, content).Result;
                if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            else if (type.ToLower() == "get")
            {
                HttpResponseMessage response = client.GetAsync(api).Result;
                result = await response.Content.ReadAsStringAsync();
                apiResult.Code = ((int)response.StatusCode).ToString();
                apiResult.Content = result;
                return apiResult;
                //if (response.IsSuccessStatusCode)
                //{
                //    //  string aa = response.Headers.GetValues("X-SID").FirstOrDefault();
                //    result = await response.Content.ReadAsStringAsync();
                //    return result;
                //}
            }
            else
            {
                return apiResult;
            }
            return apiResult;
        }


        public static Dictionary<string, string> GetKeyValuePairs(string OperatorCode, string Rack, int FromQuantity = 0, int ToQuantity = 0, string LocationCode = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (OperatorCode== "RetriveStatusList")
            {
                dic.Add("Rack", Rack);
                dic.Add("api ", "Pallet/RetriveStatusList");
            }
            if (OperatorCode== "SetPreAuthDelta")
            {
                dic.Add("Rack", Rack);
                dic.Add("api ", "Pallet/SetPreAuthDelta");
                ApiSetPreAuthDeltaEntity apiSetPreAuthDeltaEntity = new ApiSetPreAuthDeltaEntity();
                apiSetPreAuthDeltaEntity.Number = LocationCode;
                apiSetPreAuthDeltaEntity.From = FromQuantity;
                apiSetPreAuthDeltaEntity.To = ToQuantity;
                dic.Add("Args", JsonConvert.SerializeObject(apiSetPreAuthDeltaEntity));
            }
            if (OperatorCode== "SetCorrectQuantity")
            {
                dic.Add("Rack", Rack);
                dic.Add("api ", "Pallet/SetCorrectQuantity");
                ApiSetCorrectQuantityEntity  apiSetCorrectQuantityEntity = new ApiSetCorrectQuantityEntity();
                apiSetCorrectQuantityEntity.Number = LocationCode;
                apiSetCorrectQuantityEntity.Current = FromQuantity;
                apiSetCorrectQuantityEntity.Justness = ToQuantity;
                dic.Add("Args", JsonConvert.SerializeObject(apiSetCorrectQuantityEntity));
            }
            return dic;
        }

        public static Common.ApiResult InvokeWebapiApi(string url, string api, string type, string json)
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
            client.DefaultRequestHeaders.Add("Accept", "text/plain");
            client.DefaultRequestHeaders.Add("ContentType", "text/plain");
            //client.DefaultRequestHeaders.Add("X-User", "admin");
            //client.DefaultRequestHeaders.Add("X-StayLoggedIn", "yes");

            client.BaseAddress = new Uri(url);

            client.Timeout = TimeSpan.FromSeconds(510);
            string result = "";
            if (type.ToLower() == "post")
            {
                // var content = new System.Net.Http.StringContent(json);

                var httpRequestMessage = new HttpRequestMessage();
                var content = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
                httpRequestMessage.Content = content;

                httpRequestMessage.RequestUri = new Uri(url + api);
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.Headers.Add("ContentType", "application/x-www-form-urlencoded");
                httpRequestMessage.Headers.Add("Accept", "application/x-www-form-urlencoded");

                var aaa = httpRequestMessage.Content.ReadAsStringAsync();
                HttpResponseMessage response = client.SendAsync(httpRequestMessage).Result;
                apiResult.Code = ((int)response.StatusCode).ToString();
                apiResult.Content = response.Content.ReadAsStringAsync().Result;
                // HttpResponseMessage response = client.PostAsync(api, content).Result;
                return apiResult;
                if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            else if (type.ToLower() == "get")
            {
                //HttpResponseMessage response = client.GetAsync(api).Result;
                //result = response.Content.ReadAsStringAsync().Result;
                //apiResult.Code = ((int)response.StatusCode).ToString();
                //apiResult.Content = result;
                //return apiResult;
                // return response;
                //if (response.IsSuccessStatusCode)
                //{
                //    string aa = response.Headers.GetValues("X-SID").FirstOrDefault();
                //    result = await response.Content.ReadAsStringAsync();
                //    return result;
                //}

                //var httpRequestMessage = new HttpRequestMessage();
                //var content = new StringContent(json, Encoding.UTF8, "text/plain");
                //httpRequestMessage.Content = content;

                //httpRequestMessage.RequestUri = new Uri(url);
                //httpRequestMessage.Method = HttpMethod.Get;
                //httpRequestMessage.Headers.Add("ContentType", "text/plain");
                //httpRequestMessage.Headers.Add("Accept", "text/plain");
                //httpRequestMessage.Version = HttpVersion.Version11;
                //// var aaa = httpRequestMessage.Content.ReadAsStringAsync();
                //HttpResponseMessage response = client.SendAsync(httpRequestMessage).Result;
                //apiResult.Code = ((int)response.StatusCode).ToString();
                //apiResult.Content = response.Content.ReadAsStringAsync().Result;
                //// HttpResponseMessage response = client.PostAsync(api, content).Result;
                //return apiResult;
                //if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                //{
                //    result = response.Content.ReadAsStringAsync().Result;
                //}

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (json == "" ? "" : "?") + json);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                apiResult.Code = ((int)response.StatusCode).ToString();
                apiResult.Content = retString;
                // HttpResponseMessage response = client.PostAsync(api, content).Result;
                return apiResult;
            }
            else
            {
                return null;
            }
            return null;
        }



    }
}
