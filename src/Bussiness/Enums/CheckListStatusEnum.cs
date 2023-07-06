using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
    /// <summary>
    /// 盘点单
    /// </summary>
   public enum CheckListStatusEnum
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        [Description("待下发")]
        WaitingForCheck = 0,
        /// <summary>
        /// 已下发
        /// </summary>
        [Description("全部下发")]
        Issue = 1,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        Proceed = 2,
        /// <summary>
        /// 盘点完成
        /// </summary>
        [Description("盘点完成")]
        Accomplish = 3 ,
        /// <summary>
        /// 复盘
        /// </summary>
        [Description("复盘")]
        AnewCheck = 4,
        /// <summary>
        /// 已提交
        /// </summary>
        [Description("已提交")]
        Submit = 5,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Cancellation = 6,
    }
}
