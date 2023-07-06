using System.ComponentModel;

namespace HPC.BaseService.Enums
{
    public enum ValidateTypeEnum
    {
        [Description("非空")]
        NotEmpty=1,
        [Description("大于零")]
        GreaterThan0,
        [Description("大于等于零")]
        GreaterEqualsThan0,
        [Description("邮箱")]
        Email
    }
}
