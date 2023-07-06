using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Enums
{
   public enum DeviceCaption
    {
        [Description("600U")]
        Ptl600U = 0,
        [Description("900U")]
        Ptl900U = 1,
        [Description("XIOPort")]
        PtlXIOPort = 2,
        [Description("PtlXMatrixPort")]
        PtlXMatrixPort = 3,
        [Description("PtlMXP1O5")]
        PtlMXP1O5 = 4,
        [Description("PtlXDO10RFID")]
        PtlXDO10RFID = 5,
        [Description("PtlTera")]
        PtlTera = 6,
        [Description("PtlIBS")]
        PtlIBS = 7,
        [Description("SMT电子料塔")]
        Tower = 8
    }
}
