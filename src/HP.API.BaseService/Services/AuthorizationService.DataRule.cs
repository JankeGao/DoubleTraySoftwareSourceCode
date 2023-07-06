using System.Collections.Generic;
using System.Linq;
using HP.Core.Security;
using HP.Data.Orm;
using HP.Utility;
using HP.Utility.Data;
using HP.Utility.Extensions;

namespace HPC.BaseService.Services
{
    public partial class AuthorizationService
    {
        public IQuery<DataRule> DataRules
        {
            get { return DataRuleRepository.Query(); }
        }

        /// <summary>
        /// 数据授权
        /// </summary>
        /// <param name="type">类别</param>
        /// <param name="typeCode">类别编码</param>
        /// <param name="auths">数据规则列表</param>
        /// <returns></returns>
        private DataResult DataRuleAuthorization(int type, string typeCode, List<DataRule> auths)
        {
            //原始数据规则授权
            var oriDataRuleEntityInfos =
                DataRules.Where(a => a.Type == type && a.TypeCode == typeCode)
                    .Select(a => new { a.EntityInfoId, a.DataFilterRule })
                    .ToList();

            var oriDataRuleEntityInfoIds = oriDataRuleEntityInfos.Select(a => a.EntityInfoId);

            //当前数据规则授权
            List<string> dataRuleEntityInfoIds = auths.Select(a => a.EntityInfoId).ToList();

            //待插入
            var dataRuleEntityInfoIdsForInsert = dataRuleEntityInfoIds.Except(oriDataRuleEntityInfoIds);
            foreach (string dataRuleEntityInfoId in dataRuleEntityInfoIdsForInsert)
            {
                if (!DataRuleRepository.Insert(new DataRule()
                {
                    Type = type,
                    TypeCode = typeCode,
                    EntityInfoId = dataRuleEntityInfoId,
                    DataFilterRule = auths.Find(a => a.EntityInfoId == dataRuleEntityInfoId).DataFilterRule
                }))
                {
                    return
                        DataProcess.Failure(
                            "{0}({1})数据规则创建失败！".FormatWith(EnumHelper.GetCaption(typeof(AuthorizationType),
                                type), typeCode));
                }
            }

            //待更新
            var dataRuleEntityInfoIdsForUpdate = dataRuleEntityInfoIds.Intersect(oriDataRuleEntityInfoIds);
            foreach (string dataRuleEntityInfoId in dataRuleEntityInfoIdsForUpdate)
            {
                string dataFilterRule = auths.Find(b => b.EntityInfoId == dataRuleEntityInfoId).DataFilterRule;
                string oriDataFilterRule = oriDataRuleEntityInfos.Find(a => a.EntityInfoId == dataRuleEntityInfoId).DataFilterRule;
                if (dataFilterRule != oriDataFilterRule)
                {
                    if (
                    DataRuleRepository.Update(a => new DataRule
                    {
                        DataFilterRule = dataFilterRule
                    }, a => a.Type == type && a.TypeCode == typeCode && a.EntityInfoId == dataRuleEntityInfoId) == 0)
                    {
                        return
                            DataProcess.Failure(
                                "{0}({1})数据规则更新失败！".FormatWith(EnumHelper.GetCaption(typeof(AuthorizationType),
                                    type), typeCode));
                    }
                }
            }

            return DataProcess.Success();
        }
    }
}
