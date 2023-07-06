using System.ComponentModel;

namespace HPC.BaseService.Enums
{
    public enum DataRuleSpecialWordEnum
    {
        [Description("{=CurrentUserCode}")]
        CurrentUserCode=1,
        [Description("{=CurrentLeader}")]
        CurrentLeader,
        [Description("{=CurrentOrgId}")]
        CurrentOrgId,
        [Description("{=CurrentDeptId}")]
        CurrentDeptId
    }
}