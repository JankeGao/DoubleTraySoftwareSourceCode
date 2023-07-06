using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Services
{
    public class MobileLocationServer :IMobileLocationContract
    {
        public IRepository<MobileLocation, int> MobileLocationRepository { get; }
        public IQuery<MobileLocation> MobileLocations { get; }

        public IQuery<MobileLocationDto> MobileLocationDtos { get; }


        public DataResult CreateMobileLocation(MobileLocation moblie)
        {
            if (MobileLocationRepository.Insert(moblie))
            {
                return DataProcess.Success();
            }

            return DataProcess.Failure();
        }
    }
}