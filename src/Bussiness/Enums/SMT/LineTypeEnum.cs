using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum LineTypeEnum
    {
        [Description("总装线")]
        Assembly = 0,
        [Description("轴串线")]
        Axis = 1,
        [Description("实验线")]
        Experiment = 2,
    }
}