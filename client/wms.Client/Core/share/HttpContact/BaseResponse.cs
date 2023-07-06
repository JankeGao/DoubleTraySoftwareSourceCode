

namespace wms.Client.Core.share.HttpContact
{
    public class BaseResponse
    {
        /// <summary>
        /// 后台消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// //返回状态
        /// </summary>
        public int StatusCode { get; set; }

        public object Result { get; set; }
    }

    /// <summary>
    /// 数据操作结果
    /// </summary>
    public class DataResult : DataResult<object>
    {
        ///// <summary>
        ///// 是否成功
        ///// </summary>
        //public bool Success { set; get; }
        ///// <summary>
        ///// 返回消息
        ///// </summary>
        //public string Message { set; get; }
        /////// <summary>
        /////// 返回数据1
        /////// </summary>
        //public object Data { set; get; }
    }

    public class DataResult<TData>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { set; get; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { set; get; }
        ///// <summary>
        ///// 返回数据1
        ///// </summary>
        public TData Data { set; get; }
        /// <summary>
        /// 返回结果类型
        /// </summary>
        public int ResultType { set; get; }
    }
}
