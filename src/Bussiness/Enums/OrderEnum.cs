using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
    /// <summary>
    /// 三方单据状态
    /// </summary>
    public enum OrderEnum
    {
        /// <summary>
        /// 待执行
        /// </summary>
        [Description("初始状态")]
        Created = 0,
        /// <summary>
        /// 待执行
        /// </summary>
        [Description("待执行")]
        Wait = 1,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Finish = 2,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Cancel = 3,
        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        Error = 4,
    }
}
