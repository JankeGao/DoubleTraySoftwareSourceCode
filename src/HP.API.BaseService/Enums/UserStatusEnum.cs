using System.ComponentModel;

namespace HPC.BaseService.Enums
{
    public enum UserStatusEnum
    {
        [Description("正常")]
        Normal=1,
        [Description("离职")]
        Leave =2
    }
}
