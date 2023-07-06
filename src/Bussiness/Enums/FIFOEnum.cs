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
    public enum FIFOEnum
    {
        [Description("无")]
        NoFIFO = 0,
        [Description("入库时间")]
        InTime = 1,
        [Description("生产日期")]
        ProDuctionTime = 2,
        [Description("库存保质期")]
        ValidityTime = 3,
    }
}
