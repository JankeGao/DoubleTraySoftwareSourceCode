


using wms.Core;

namespace Consumption.ViewModel
{
    using Consumption.Shared.Common;
    using wms.Core.share.Dto;
    using Consumption.ViewModel.Interfaces;

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserViewModel : BaseRepository<UserDto>, IUserViewModel
    {
        public UserViewModel(IUserRepository repository) : base(repository)
        {

        }
    }
}
