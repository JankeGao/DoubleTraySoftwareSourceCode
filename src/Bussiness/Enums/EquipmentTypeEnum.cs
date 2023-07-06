using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
    /// <summary>
    /// 设备类型
    /// </summary>
   public enum EquipmentTypeEnum
    {
        /// <summary>
        /// 升降库
        /// </summary>
        [Description("升降库")]
        GoUpDecline = 0,
        /// <summary>
        /// 回转库
        /// </summary>
        [Description("回转库")]
        Rotation = 1,

  
    }
}
