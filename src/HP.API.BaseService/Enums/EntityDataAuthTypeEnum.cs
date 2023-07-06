using System.ComponentModel;

namespace HPC.BaseService.Enums
{
    public enum EntityDataAuthTypeEnum
    {
        [Description("未配置")]
        None=0,
        [Description("无限制")]
        All =1,
        [Description("仅限本人")]
        Self,
        [Description("仅限本人及下属")]
        SelfAndSubordinate,
        [Description("所在部门")]
        Department,
        [Description("所在公司")]
        Organization,
        [Description("自定义")]
        Custom
    }
}
