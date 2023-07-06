using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Enums.SMT
{
    public enum ShelfSortStatusEnum
    {
        [Description("未分配")]
        Unallocated = 0,
        [Description("已分配")]
        Allocated = 1,
    }
}