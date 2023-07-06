using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum PickOrderAreaDetailStatusEnum
    {
        [Description("待捡选")]
        WaitingPick = 0,
        [Description("拣选中")]
        Picking = 1,
        [Description("待拆盘上架")]
        WatingSplit = 2,
        [Description("拣货完成待复核")]
        WaitingSend = 3,
        [Description("已复核")]
        Finished = 4,
        [Description("作废")]
        Canceled = 5,
        [Description("已过帐")]
        ConfirmToMes = 6,
    }
}