using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Contracts
{
    public interface IIdentityContract : IScopeDependency
    {
        #region 用户

        IQuery<User> Users { get; }
        IQuery<User> WholeUsers { get; }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateUser(UserInputDto entity);

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditUser(UserInputDto entity);

        /// <summary>
        /// 编辑用户个人中心信息
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        DataResult EditUserCenter(UserInputDto entityDto);

        /// <summary>
        /// 编辑用户联系方式
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditUserContract(User entity);

        /// <summary>
        /// 编辑用户头像
        /// </summary>
        /// <returns></returns>
        DataResult EditUserHeader(User entity);

        /// <summary>
        /// 移除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveUser(int id);

        /// <summary>
        /// 还原用户类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RestoryUser(int id);

        /// <summary>
        /// 头像
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        string GetHeader(string header);

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        DataResult Login(LoginInfo inputDto);

        /// <summary>
        /// 设置用户密码
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        DataResult SetUserPassword(UserPasswordInputDto inputDto);

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        DataResult ResetUserPassword(UserPasswordInputDto inputDto);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        DataResult GetUserInfo(string token);


        DataResult UserLogin(LoginInfo inputDto);


        #endregion

        #region 角色

        IQuery<Role> Roles { get; }
        IQuery<RoleOutputDto> RoleDtos { get; }
        /// <summary>
        /// 查询启用用户
        /// </summary>
        /// <returns></returns>
        DataResult GetRole();

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateRole(Role entity);

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditRole(Role entity);

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveRole(int id);

        #endregion

        #region 角色用户组映射

        IQuery<RoleUserMap> RoleUsersMaps { get; }

        /// <summary>
        /// 设置角色用户
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        DataResult SetRoleUsers(RoleUsersMapInputDto inputDto);

        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        DataResult SetUserRoles(UserRolesMapInputDto inputDto);

        #endregion

        #region 用户组


        #endregion

        #region 用户组用户映射

        IQuery<UserGroupUserMap> UserGroupUsersMaps { get; }

        /// <summary>
        /// 设置用户组与用户映射
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        DataResult SetUserGroupUsersMap(UserGroupUsersMapInputDto inputDto);

        /// <summary>
        /// 设置用户与用户组映射
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        DataResult SetUserUserGroupsMap(UserUserGroupsMapInputDto inputDto);

        /// <summary>
        /// 移除用户组
        /// </summary>
        /// <param name="userGroupCode"></param>
        /// <returns></returns>
        DataResult RemoveUserGroupUsersMap(string userGroupCode);

        #endregion


    }
}
