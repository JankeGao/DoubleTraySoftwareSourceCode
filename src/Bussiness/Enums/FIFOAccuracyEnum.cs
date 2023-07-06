using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
    /// <summary>
    /// 先进先出原则
    /// </summary>
    public enum FIFOAccuracyEnum
    {
        [Description("无")]
        No = 0,
        [Description("秒")]
        Second = 1,
        [Description("分钟")]
        Minute = 2,
        [Description("小时")]
        Hour = 3,
        [Description("天")]
        Day =4,
    }
}
