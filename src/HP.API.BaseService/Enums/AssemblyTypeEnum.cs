using System.ComponentModel;

namespace HPC.BaseService.Enums
{
    public enum AssemblyTypeEnum
    {
        [Description("文本")]
        TextBox=1,
        [Description("数字")]
        NumberBox,
        [Description("下拉")]
        ComboBox,
        [Description("下拉树")]
        ComboTree,
        [Description("下拉表格")]
        ComboGrid,
        [Description("日期")]
        DateBox,
        [Description("日期时间")]
        DateTimeBox,
        [Description("复选框")]
        CheckBox,
    }
}
