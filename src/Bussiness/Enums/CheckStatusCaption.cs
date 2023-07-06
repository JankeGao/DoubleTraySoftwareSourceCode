using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
   public enum CheckStatusCaption
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        [Description("待执行")]
        WaitingForCheck = 0,
        /// <summary>
        /// 手动执行中
        /// </summary>
        [Description("执行中")]
        HandChecking = 1,
        /// <summary>
        /// 已发送PTL
        /// </summary>
        [Description("已发送PTL")]
        Sended = 2,
        /// <summary>
        /// PTL执行中
        /// </summary>
        [Description("PTL执行中")]
        PTLChecking = 3,
        /// <summary>
        /// 盘点完成
        /// </summary>
        [Description("盘点完成")]
        Checked = 4,
        /// <summary>
        /// 复盘
        /// </summary>
        [Description("复盘")]
        CheckAgagin = 5,
        /// <summary>
        /// 结果已提交
        /// </summary>
        [Description("结果已提交")]
        Finished = 6,
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Cancel = 7,
    }
}
