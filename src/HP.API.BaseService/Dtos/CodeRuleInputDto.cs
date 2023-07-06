
using HP.Core.Data;

namespace HPC.BaseService.Dtos
{
    public class CodeRuleInputDto:IInputDto<string>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 分割符
        /// </summary>
        public string Delimiter { set; get; }
        /// <summary>
        /// 重置
        /// </summary>
        public string Reset { set; get; }
        /// <summary>
        /// 步长
        /// </summary>
        public int Step { set; get; }
        /// <summary>
        /// 当前序号
        /// </summary>
        public int CurrentNo { set; get; }
        /// <summary>
        /// 当前编码
        /// </summary>
        public string CurrentCode { set; get; }
        /// <summary>
        /// 当前重置
        /// </summary>
        public string CurrentReset { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 映射实体名
        /// </summary>
        public string EntityFullName { set; get; }
        /// <summary>
        /// 规则字符串
        /// </summary>
        public string RuleJson { set; get; }
    }
}