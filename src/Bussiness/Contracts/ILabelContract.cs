using Bussiness.Dtos;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface ILabelContract : IScopeDependency
    {
        IRepository<Entitys.Label, int> LabelRepository { get; }

        IQuery<Entitys.Label> Labels { get; }
        IQuery<LabelDto> LabelDtos { get; }
        DataResult CreateLabel(Entitys.Label entity);

        DataResult EditLabel(Entitys.Label entity);

        DataResult DeleteLabel(int id);

        DataResult CreateBatchLabel(LabelDto entityDto);

    }
}
