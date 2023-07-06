namespace HPC.BaseService.Models
{
    /// <summary>
    /// 登录信息
    /// </summary>
    public class UserLogin
    {
        /// <summary>
        /// WeChat
        /// </summary>
        public string WeChatCode { get; set; }
        /// <summary>
        /// 店铺编码
        /// </summary>
        public string StoreCode { get; set; }
        /// <summary>
        /// 获取或设置 用户名
        /// </summary>
        public string MemberCode { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobilephone { get; set; }

        /// <summary>
        /// 获取或设置 用户密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 获取或设置 验证码
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 获取或设置 记住登录
        /// </summary>
        public bool Remember { get; set; }

        /// <summary>
        /// 获取或设置 返回地址
        /// </summary>
        public string ReturnUrl { get; set; }
        public string SessionId { set; get; }
        public string ClientIp { set; get; }
    }
}