using System.ComponentModel;

namespace Bussiness.Enums
{
    /// <summary>
    /// 中间表数据枚举
    /// </summary>
    public enum InterFaceBCaption
    {
        /// <summary>
        /// 待下发
        /// </summary>
        [Description("初始状态")]
        Waiting = 0,
        /// <summary>
        /// 部分下发
        /// </summary>
        [Description("已接收")]
        Get = 1,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Finished = 2,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Cancel = 3,
    }
}
