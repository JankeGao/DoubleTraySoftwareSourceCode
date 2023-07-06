using System.Threading.Tasks;
using wms.Client.Core.share.Common.Query;
using wms.Client.Core.share.Dto;
using wms.Client.Core.share.HttpContact;

namespace wms.Client.Core.share.DataInterfaces
{
    public interface IConsumptionRepository<T>
    {
        Task<BaseResponse> GetAllListAsync(QueryParameters parameters);

        Task<BaseResponse> GetAsync(int id);

        Task<BaseResponse> SaveAsync(T model);

        Task<BaseResponse> AddAsync(T model);

        Task<BaseResponse> DeleteAsync(int id);

        Task<BaseResponse> UpdateAsync(T model);
    }

    public interface IUserRepository : IConsumptionRepository<UserDto>
    {
        Task<DataResult> LoginAsync(string account, string passWord);

        /// <summary>
        /// 获取用户的所属权限信息
        /// </summary>
        Task<BaseResponse> GetUserPermByAccountAsync(string account);

        Task<BaseResponse> GetAuthListAsync();
    }

    public interface IGroupRepository : IConsumptionRepository<GroupDto>
    {
        /// <summary>
        /// 获取菜单模块列表(包含每个菜单拥有的一些功能)
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse> GetMenuModuleListAsync();

        /// <summary>
        /// 根据ID获取用户组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResponse> GetGroupAsync(int id);

        /// <summary>
        /// 保存组数据
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<BaseResponse> SaveGroupAsync(GroupDataDto group);
    }

    public interface IMenuRepository : IConsumptionRepository<MenuDto>
    {

    }

    public interface IBasicRepository : IConsumptionRepository<BasicDto>
    {

    }

}
