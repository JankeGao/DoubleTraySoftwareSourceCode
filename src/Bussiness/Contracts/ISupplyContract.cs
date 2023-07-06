using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface ISupplyContract : IScopeDependency
    {
        IRepository<Entitys.Supply, int> SupplyRepository { get; }

        IQuery<Entitys.Supply> Supplys { get; }

        DataResult CreateSupply(Entitys.Supply entity);

        DataResult EditSupply(Entitys.Supply entity);

        DataResult DeleteSupply(int id);

    }
}
