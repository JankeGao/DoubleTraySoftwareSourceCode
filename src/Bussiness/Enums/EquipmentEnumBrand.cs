using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
    /// <summary>
    /// 设别品牌
    /// </summary>
   public enum EquipmentEnumBrand
    {
        /// <summary>
        /// 朗杰
        /// </summary>
        [Description("朗杰")]
        NamgyaiVoluntarily = 0,
        /// <summary>
        /// 卡迪斯
        /// </summary>
        [Description("卡迪斯")]
        Kardex = 1,
        /// <summary>
        /// 享乃尔
        /// </summary>
        [Description("享乃尔")]
        Hanel = 2,
  
    }
}
