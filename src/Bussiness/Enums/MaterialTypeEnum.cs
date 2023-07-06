using System.ComponentModel;

namespace Bussiness.Enums
{
    [Description("物料类别")]
    public enum MaterialTypeEnum
    {
        [Description("物料")]
        Nomarl = 0,

        [Description("模具")]
        Mould = 1,
    }
}