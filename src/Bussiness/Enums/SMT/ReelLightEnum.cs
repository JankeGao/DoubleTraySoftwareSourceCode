using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum ReelLightEnum
    {
        [Description("开始")]
        Start = 0,
        [Description("结束")]
        Finished = 1,
    }
}