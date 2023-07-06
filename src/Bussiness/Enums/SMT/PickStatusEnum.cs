using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum PickStatusEnum
    {
        [Description("待生成拣货单")]
        WaitingCheck = 0,
        [Description("已生成拣货单")]
        CreatedPickOrder = 1,
        [Description("拣选完毕")]
        Picked= 2,
        [Description("已完成")]
        Finished= 3,
        [Description("缺料")]
        Lack = 4,
        [Description("发料完毕待上架")]
        WaitingForShelf = 5,
    }
}