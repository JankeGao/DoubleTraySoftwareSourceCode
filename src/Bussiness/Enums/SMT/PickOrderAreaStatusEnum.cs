using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum PickOrderAreaStatusEnum
    {
        [Description("待启动")]
        WaitingStart = 0,
        [Description("已启动")]
        Starting = 1,
        [Description("下架完成")]
        Finished = 2,
    }
}