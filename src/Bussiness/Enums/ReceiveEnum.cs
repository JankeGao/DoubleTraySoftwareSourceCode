using System.ComponentModel;

namespace Bussiness.Enums
{
    public  enum ReceiveEnum
    {
        /// <summary>
        /// 待执行
        /// </summary>
        [Description("待下发")]
        Wait = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("全部下发")]
        Proceed = 1,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已归还")]
        Finish = 2,
        /// <summary>
        /// 以作废
        /// </summary>
        [Description("已作废")]
        Cancellation = 3,
    }
}
