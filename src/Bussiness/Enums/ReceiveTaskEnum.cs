using System.ComponentModel;

namespace Bussiness.Enums
{
    public  enum ReceiveTaskEnum
    {
        /// <summary>
        /// 待执行
        /// </summary>
        [Description("待领用")]
        Wait = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("使用中")]
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
