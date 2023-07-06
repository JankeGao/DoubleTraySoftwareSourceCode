using System.ComponentModel;

namespace Bussiness.Enums
{
    public enum MaterialStatusCaption
    {
        [Description("正常")]
        Normal = 0,
        [Description("已超期")]
        Alam = 1,
        [Description("已处理")]
        Done = 2,
        [Description("作废")]
        Cancel = 3,
        [Description("老化")]
        Old = 4,
    }
}
