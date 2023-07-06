using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IMobileLocationContract
    {
        IRepository<Bussiness.Entitys.MobileLocation,int> MobileLocationRepository {  get; }

        IQuery<Entitys.MobileLocation> MobileLocations { get; }

        IQuery<MobileLocationDto> MobileLocationDtos { get; }

        DataResult CreateMobileLocation(MobileLocation moblie);
    }
}