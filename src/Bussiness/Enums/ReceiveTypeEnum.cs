using System.ComponentModel;

namespace Bussiness.Enums
{
    public  enum ReceiveTypeEnum
    {
        /// <summary>
        /// 生产
        /// </summary>
        [Description("生产")]
        Production = 0,
        /// <summary>
        /// 修模
        /// </summary>
        [Description("修模")]
        MoldRepair = 1,
        /// <summary>
        /// 注销
        /// </summary>
        [Description("注销")]
        Logout = 2,
    }
}
