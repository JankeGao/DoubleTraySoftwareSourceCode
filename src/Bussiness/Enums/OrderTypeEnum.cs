using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
    /// <summary>
    /// 单据来源
    /// </summary>
    public enum OrderTypeEnum
    {
        /// <summary>
        /// WMS创建
        /// </summary>
        [Description("WMS创建")]
        Self = 0,
        /// <summary>
        /// 三方同步
        /// </summary>
        [Description("调拨单")]
        Move = 1,
        /// <summary>
        /// 三方同步
        /// </summary>
        [Description("三方同步")]
        Other = 2
    }
}
