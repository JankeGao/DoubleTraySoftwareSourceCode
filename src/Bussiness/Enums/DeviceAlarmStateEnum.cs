using System.ComponentModel;

namespace Bussiness.Enums
{
    public  enum DeviceAlarmStateEnum
    {
        /// <summary>
        /// 报警中
        /// </summary>
        [Description("报警")]
        Urgencye = 0,
        /// <summary>
        /// 已复位
        /// </summary>
        [Description("已复位")]
        Reach = 1,
    }
}
