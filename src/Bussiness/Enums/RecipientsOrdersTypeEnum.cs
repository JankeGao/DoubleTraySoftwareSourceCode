using System.ComponentModel;

namespace Bussiness.Enums
{
    public  enum RecipientsOrdersTypeEnum
    {
        [Description("生产")]
        Production = 0,
        [Description("修模")]
        MoldRepair = 1,
        [Description("注销")]
        Logout = 2,
    }
}
