using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.PTL
{
    public enum OrderType
    {
        [Description("拣货单")]
        Pick = 0,
        [Description("拆盘单")]
        Split = 1,
        [Description("上架任务")]
        Shelf = 2,
        [Description("库存点亮")]
        StockLight = 3,
        [Description("盘点")]
        Check = 4,
    }
}