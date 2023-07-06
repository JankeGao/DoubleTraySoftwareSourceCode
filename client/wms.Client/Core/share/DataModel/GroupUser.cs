

namespace wms.Client.Core.share.DataModel
{
    /// <summary>
    /// 组用户
    /// </summary>
    public partial class GroupUser : BaseEntity
    {
        /// <summary>
        /// 组代码
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

    }

}
