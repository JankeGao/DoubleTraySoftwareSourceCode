using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
   public enum OutTaskStatusCaption
    {
        /// <summary>
        /// 待下发
        /// </summary>
        [Description("待执行")]
        WaitingForPicking = 0,
        [Description("执行中")]
        Picking = 1,
        [Description("已完成")]
        Finished = 2,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Cancel = 3,
    }
}
