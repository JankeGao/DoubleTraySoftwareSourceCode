using System;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class IdentityService
    {
        public IQuery<RoleUserMap> RoleUsersMaps
        {
            get { return RoleUserMapRepository.Query(); }
        }

        /// <summary>
        /// 设置角色用户
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public DataResult SetRoleUsers(RoleUsersMapInputDto inputDto)
        {
            inputDto.CheckNotNull("inputDto");

            RoleUserMapRepository.UnitOfWork.TransactionEnabled = true;

            //如果有数据
            if (RoleUsersMaps.Any(p => p.RoleCode == inputDto.RoleCode))
            {
                if (RoleUserMapRepository.Delete(p => p.RoleCode == inputDto.RoleCode) == 0)
                {
                    return DataProcess.Failure("角色({0})原始用户映射数据删除失败！".FormatWith(inputDto.RoleCode));
                }
            }

            //添加角色用户映射数据
            if (!inputDto.UserCodes.IsNullOrEmpty())
            {
                foreach (string userCode in inputDto.UserCodes.FromJsonString<string[]>())
                {
                    if (!RoleUserMapRepository.Insert(new RoleUserMap()
                    {
                        UserCode = userCode,
                        RoleCode = inputDto.RoleCode
                    }))
                    {
                        return DataProcess.Failure("角色({0})用户映射数据创建失败！".FormatWith(inputDto.RoleCode));
                    }
                }
            }

            RoleUserMapRepository.UnitOfWork.Commit();

            return DataProcess.Success("角色用户映射关系更新成功！");
        }

        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public DataResult SetUserRoles(UserRolesMapInputDto inputDto)
        {
            inputDto.CheckNotNull("inputDto");

            RoleUserMapRepository.UnitOfWork.TransactionEnabled = true;

            //如果有数据
            if (RoleUsersMaps.Any(p => p.UserCode == inputDto.UserCode))
            {
                if (RoleUserMapRepository.Delete(p => p.UserCode == inputDto.UserCode) == 0)
                {
                    return DataProcess.Failure("用户({0})原始角色映射数据删除失败！".FormatWith(inputDto.UserCode));
                }
            }
            try
            {
                //添加用户角色映射数据
                if (!inputDto.RoleCodes.IsNullOrEmpty())
                {
                    foreach (string roleCode in inputDto.RoleCodes.FromJsonString<string[]>())
                    {
                        if (!RoleUserMapRepository.Insert(new RoleUserMap()
                        {
                            UserCode = inputDto.UserCode,
                            RoleCode = roleCode
                        }))
                        {
                            return DataProcess.Failure("用户({0})角色映射数据创建失败！".FormatWith(inputDto.UserCode));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
            }


            RoleUserMapRepository.UnitOfWork.Commit();

            return DataProcess.Success("角色用户映射关系更新成功！");
        }
    }
}
