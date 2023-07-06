using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
    public enum ContainerTypeEnum
    {
        [Description("三菱")]
        Mitsubishi = 0,
        [Description("卡迪斯")]
        Kardex = 1,
        [Description("亨乃尔")]
        Hanel = 2,
        [Description("三菱垂直回转柜")]
        MitsubishiRotation = 3,
        [Description("虚拟货柜")]
        Virtual = 4,
    }
}
