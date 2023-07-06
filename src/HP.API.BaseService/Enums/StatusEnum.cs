using System.ComponentModel;

namespace HPC.BaseService.Enums
{
    public enum StatusEnum
    {
        [Description("已创建")]
        Created = 1,
        [Description("已提交")]
        Submited,
        [Description("已作废")]
        Canceled,
        [Description("已完成")]
        Completed
    }
}
