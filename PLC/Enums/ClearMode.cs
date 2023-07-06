using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCServer.Enums
{
    public enum ClearMode
    {
        ClearAllOnce,//将全部熄灭，自检或复位时
        ClearOneByOne//逐条显示熄灭，当备料单备料完成、指引或发料时
    }
}
