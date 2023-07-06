using System.ComponentModel;

namespace wms.Client.Core.share.DataModel
{
    /// <summary>
    /// 组
    /// </summary>
    public class Group : BaseEntity
    {
        /// <summary>
        /// 组代码
        /// </summary>
        [Description("组代码")]
        public string GroupCode { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        [Description("组名称")]
        public string GroupName { get; set; }

    }
}
