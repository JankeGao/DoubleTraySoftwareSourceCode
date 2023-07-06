using HP.Core.Dependency;
using HP.Core.Sequence.Entitys;
using HP.Data.Orm;
using HP.Utility.Data;
using HPC.BaseService.Dtos;

namespace HPC.BaseService.Contracts
{
    public interface ICodeRuleContract: IScopeDependency
    {
        IQuery<CodeRule> CodeRules { get; }
        IQuery<CodeRuleItem> CodeRuleItems { get; }

        /// <summary>
        /// 创建字典
        /// </summary>
        /// <returns></returns>
        DataResult CreateCodeRule(CodeRuleInputDto dto);

        /// <summary>
        /// 编辑字典类型
        /// </summary>
        /// <returns></returns>
        DataResult EditCodeRule(CodeRuleInputDto dto);

        /// <summary>
        /// 移除字典类型
        /// </summary>
        /// <returns></returns>
        DataResult RemoveCodeRule(string id);
    }
}