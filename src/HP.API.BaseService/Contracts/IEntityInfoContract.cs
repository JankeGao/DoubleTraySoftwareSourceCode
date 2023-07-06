using HP.Core.Dependency;
using HP.Core.Security;
using HP.Data.Orm;
using HP.Utility.Data;

namespace HPC.BaseService.Contracts
{
    public interface IEntityInfoContract : IScopeDependency
    {
        IQuery<EntityInfo> EntityInfos { get; }

        DataResult EditEntityInfo(EntityInfo entity);
    }
}