using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace PLCServer.Interceptor
{
    //IClientMessageInspector
    public class MessageIntercept : IClientMessageInspector,IDispatchMessageInspector
    {
        /// <summary>
        /// 客户端接收到的恢复
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // throw new NotImplementedException();
        }
        /// <summary>
        /// 服务器端接收到的请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            Console.WriteLine("服务器端接收到的请求:{0}\n", request);
            Console.WriteLine("服务器端：接收到的请求：\n{0}", request);
            //  (服务端)栓查验证信息
            string un = request.Headers.GetHeader<string>("user", "myTest");
            string ps = request.Headers.GetHeader<string>("pw", "myTest");
            if (un == "admin" && ps == "123")
            {
                Console.WriteLine("用户名和密码正确.");
            }
            else
            {
                throw new Exception("验证失败!");
            }

            Stopwatch stw = Stopwatch.StartNew();





            //日志
            return "";

        }
        /// <summary>
        /// 服务器端的回复
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var watch = (Stopwatch)correlationState;
            watch.Stop();
            //日志
            Console.WriteLine("服务器将作出以下回复:{0}\n", reply);
        }
        /// <summary>
        /// 客户端发送前的请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // throw new NotImplementedException();
            System.ServiceModel.Channels.MessageHeader hdUserName = System.ServiceModel.Channels.MessageHeader.CreateHeader("user", "myTest", "admin");
            System.ServiceModel.Channels.MessageHeader hdPassWord = System.ServiceModel.Channels.MessageHeader.CreateHeader("pw", "myTest", "123");
            request.Headers.Add(hdUserName);
            request.Headers.Add(hdPassWord);
            return null;
        }
    }
}
