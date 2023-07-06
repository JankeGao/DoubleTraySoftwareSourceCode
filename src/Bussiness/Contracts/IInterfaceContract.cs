using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IInterfaceContract : IScopeDependency
    {
        IRepository<Entitys.DpsInterface, int> DpsInterfaceRepository { get; }
        IRepository<Entitys.DpsInterfaceMain, int> DpsInterfaceMainRepository { get; }
    }
}
