

using System.ComponentModel;

namespace wms.Client.Core.share.Dto
{
    public class GroupDto : BaseDto
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
