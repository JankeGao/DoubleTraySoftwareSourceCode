using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum PickOrderDetailStatusEnum
    {
        [Description("未核查")]
        WaitingCheck = 0,
        [Description("缺料")]
        Lack = 1,
        [Description("部分分配")]
        AppointPart = 2,
        [Description("完全分配")]
        Appionted = 3,
        [Description("已复核")]
        Checked = 4,
        [Description("过账完成")]
        FinishedPart = 5,
    }
}