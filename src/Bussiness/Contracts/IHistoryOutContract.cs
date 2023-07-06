using HP.Core.Dependency;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;
using Bussiness.Entitys;
using Bussiness.Dtos;

namespace Bussiness.Contracts
{
    public interface IHistoryOutContract : IScopeDependency
        {
        /// <summary>
        /// 保存获取的联合数据
        /// </summary>
        IQuery<OutMaterialLabelDto> OutMaterialLabelDtos { get; }   
    }
    
}
