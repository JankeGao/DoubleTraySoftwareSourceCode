using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum ReplenishStatusEnum
    {
        [Description("初始状态")]
        Initial = 0,
        [Description("已完成")]
         Finished = 3,
        [Description("作废")]
        Cancel = 4,
        [Description("WEB端创建")]
        WebInitial = 5,
    }
}