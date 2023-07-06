using System.ComponentModel;

namespace Bussiness.Enums
{
    public  enum RecipientsOrdersEnum
    {
        [Description("已完成")]
        Accomplish = 0,
        [Description("进行中")]
        Proceed = 1,
        [Description("待执行")]
        Execute = 2,
    }
}
