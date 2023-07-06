using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
   public enum InStatusCaption
    {
        /// <summary>
        /// 待下发
        /// </summary>
        [Description("待下发")]
        WaitingForShelf = 0,
        /// <summary>
        /// 部分下发
        /// </summary>
        [Description("部分下发")]
        Sheling = 1,
        /// <summary>
        /// 全部下发
        /// </summary>
        [Description("全部下发")]
        HandShelf = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Finished = 3,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Cancel = 4,
    }
}
