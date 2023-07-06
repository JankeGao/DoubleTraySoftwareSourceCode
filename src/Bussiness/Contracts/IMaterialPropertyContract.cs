using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IMaterialPropertyContract : IScopeDependency
    {
        IRepository<Entitys.MaterialProperty, int> MaterialPropertyRepository { get; }

        IQuery<Entitys.MaterialProperty> MaterialPropertys { get; }

        DataResult CreateMaterialProperty(Entitys.MaterialProperty entity);

        DataResult EditMaterialProperty(Entitys.MaterialProperty entity);

        DataResult DeleteMaterialProperty(int id);

    }
}
