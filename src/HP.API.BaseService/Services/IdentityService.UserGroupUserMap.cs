using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class IdentityService
    {
        public IQuery<UserGroupUserMap> UserGroupUsersMaps
        {
            get { return UserGroupUserMapRepository.Query(); }
        }

        /// <summary>
        /// 设置用户组用户映射
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public DataResult SetUserGroupUsersMap(UserGroupUsersMapInputDto inputDto)
        {
            inputDto.CheckNotNull("inputDto");

            UserGroupUserMapRepository.UnitOfWork.TransactionEnabled = true;

            //如果有数据
            if (UserGroupUsersMaps.Any(p => p.UserGroupCode == inputDto.UserGroupCode))
            {
                if (UserGroupUserMapRepository.Delete(p => p.UserGroupCode == inputDto.UserGroupCode) == 0)
                {
                    return DataProcess.Failure("用户组({0})原始用户映射数据删除失败！".FormatWith(inputDto.UserGroupCode));
                }
            }

            //添加用户组用户映射数据
            if (!inputDto.UserCodes.IsNullOrEmpty())
            {
                foreach (string userCode in inputDto.UserCodes.FromJsonString<string[]>())
                {
                    if (!UserGroupUserMapRepository.Insert(new UserGroupUserMap()
                    {
                        UserCode = userCode,
                        UserGroupCode = inputDto.UserGroupCode
                    }))
                    {
                        return DataProcess.Failure("用户组({0})用户映射数据创建失败！".FormatWith(inputDto.UserGroupCode));
                    }
                }
            }

            UserGroupUserMapRepository.UnitOfWork.Commit();

            return DataProcess.Success("用户组({0})用户映射关系更新成功！".FormatWith(inputDto.UserGroupCode));
        }

        /// <summary>
        /// 设置用户用户组映射
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public DataResult SetUserUserGroupsMap(UserUserGroupsMapInputDto inputDto)
        {
            inputDto.CheckNotNull("inputDto");

            UserGroupUserMapRepository.UnitOfWork.TransactionEnabled = true;

            //如果有数据
            if (UserGroupUsersMaps.Any(p => p.UserCode == inputDto.UserCode))
            {
                if (UserGroupUserMapRepository.Delete(p => p.UserCode == inputDto.UserCode) == 0)
                {
                    return DataProcess.Failure("用户({0})原始用户组数据映射删除失败！".FormatWith(inputDto.UserCode));
                }
            }

            //添加用户用户组映射数据
            if (!inputDto.UserGroupCodes.IsNullOrEmpty())
            {
                foreach (string userGroupCode in inputDto.UserGroupCodes.FromJsonString<string[]>())
                {
                    if (!UserGroupUserMapRepository.Insert(new UserGroupUserMap()
                    {
                        UserCode = inputDto.UserCode,
                        UserGroupCode = userGroupCode
                    }))
                    {
                        return DataProcess.Failure("用户({0})用户组数据映射创建失败！".FormatWith(inputDto.UserCode));
                    }
                }
            }

            UserGroupUserMapRepository.UnitOfWork.Commit();

            return DataProcess.Success("用户({0})用户组数据映射更新成功！".FormatWith(inputDto.UserCode));
        }

        /// <summary>
        /// 移除用户组
        /// </summary>
        /// <param name="userGroupCode"></param>
        /// <returns></returns>
        public DataResult RemoveUserGroupUsersMap(string userGroupCode)
        {
            if (!userGroupCode.IsNullOrEmpty())
            {
                if (UserGroupUsersMaps.Any(a => a.UserGroupCode == userGroupCode))
                {
                    if (UserGroupUserMapRepository.Delete(a => a.UserGroupCode == userGroupCode) == 0)
                    {
                        return DataProcess.Failure("用户组({0})移除失败！".FormatWith(userGroupCode));
                    }
                }
            }

            return DataProcess.Success("用户组({0})移除成功！".FormatWith(userGroupCode));
        }
    }
}
