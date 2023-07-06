using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum PickOrderStatusEnum
    {
        [Description("待启动")]
        WaitingStart = 0,
        [Description("已启动")]
        Starting = 1,
        [Description("捡料中")]
        Sending = 2,
        [Description("已复核")]
        Ready = 3,
        [Description("已过账")]
        Finished = 4,
        [Description("作废")]
        Cancel = 5,
    }
}