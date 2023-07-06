using System.ComponentModel;

namespace Bussiness.Enums
{
    [Description("库存上下限预警状态")]
    public enum MaterialNumStatusCaption
    {
        [Description("已达到最小库存")]
        ReachedMin = 0,

        [Description("已达到最大库存")]
        ReachedMax = 1,

        [Description("小于最小库存")]
        OverMin = 2,

        [Description("大于最大库存")]
        OverMax = 3,
    }
}