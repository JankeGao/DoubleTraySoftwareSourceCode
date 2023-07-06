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
   public enum CheckListEnum
    {
        /// <summary>
        /// 月度盘点
        /// </summary>
        [Description("月度盘点")]
        MonthlyCheck= 0,
        /// <summary>
        /// 年度盘点
        /// </summary>
        [Description("年度盘点")]
        YearCheck = 1,
        /// <summary>
        /// 周期盘点
        /// </summary>
        [Description("周期盘点")]
        PeriodCheck = 2,
  
    }
}
