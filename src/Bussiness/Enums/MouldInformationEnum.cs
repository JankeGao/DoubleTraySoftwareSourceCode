using System.ComponentModel;

namespace Bussiness.Enums
{
    /// <summary>
    /// 模具信息枚举
    /// </summary>
    public  enum MouldInformationEnum
    {
        /// <summary>
        /// 生产
        /// </summary>
        [Description("生产")]
        Employ  = 0,
        /// <summary>
        /// 修模
        /// </summary>
        [Description("修模")]
        MoldRepair  = 1,
        /// <summary>
        /// 注销
        /// </summary>
        [Description("注销")]
        WriteOff  = 2,
        /// <summary>
        /// 领用锁定
        /// </summary>
        [Description("领用锁定")]
        MouldLock   = 3,
        /// <summary>
        /// 在库中
        /// </summary>
        [Description("在库中")]
        InWarehouse = 4,



    }
}
