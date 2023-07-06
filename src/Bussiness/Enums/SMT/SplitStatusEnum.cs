using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum SplitStatusEnum
    {
        [Description("初始")]
        Initial = 0,
        [Description("正在下架")]
        Picking = 1,
        [Description("下架完成待拆盘")]
        Spliting = 2,
        [Description("待上架")]
        WaitingShelf = 3,
        [Description("上架完毕")]
        Shelfield = 4,
        [Description("作废")]
        Cancel = 5,
    }
}