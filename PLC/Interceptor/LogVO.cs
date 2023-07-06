using System;

namespace PLCServer.Interceptor
{
    public class LogVO
    {
        /// <summary>
        /// 契约名称
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 契约名称
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 契约名称
        /// </summary>
        public string ServiceUri { get; set; }
        /// <summary>
        /// 契约名称
        /// </summary>
        public string ClientIp { get; set; }
        /// <summary>
        /// 契约名称
        /// </summary>
        public int ClientPort { get; set; }


        /// <summary>
        /// 契约名称
        /// </summary>
        public string ContractName { get; set; }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// 输入（序列化成json字符串）
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// 输出（序列化成json字符串）
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// 耗时(ms)
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// 执行开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 执行结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

    }
}
