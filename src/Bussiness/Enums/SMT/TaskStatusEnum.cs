using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum TaskStatusEnum
    {
        [Description("待启动")]
        WaitStart = 0,
        [Description("进行中")]
        Picking = 1,
        [Description("冻结")]
        Frozen = 2,
        [Description("暂停")]
        Stop = 3,
        [Description("作废")]
        Cancel = 4,
        [Description("已完成")]
        Finished = 6,
    }
}