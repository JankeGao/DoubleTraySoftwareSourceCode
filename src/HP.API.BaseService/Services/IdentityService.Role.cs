using System;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class IdentityService
    {

        /// <summary>
        /// 部门契约
        /// </summary>
        public IDepartmentContract DepartmentContract { set; get; }

        public IQuery<Role> Roles
        {
            get { return RoleRepository.Query(); }
        }


        public IQuery<RoleOutputDto> RoleDtos => Roles.LeftJoin(DepartmentContract.Departments, (role, department) => role.DepartmentCode == department.Code)
            .Select((role, department) => new Dtos.RoleOutputDto()
            {
                Id = role.Id,
                Code = role.Code,
                Remark = role.Remark,
                Name= role.Name,
                IsSystem= role.IsSystem,
                Enabled= role.Enabled,
                DepartmentCode= department.Code,
                DepartmentName= department.Name,
                ParentCode= department.ParentCode
            });

        /// <summary>
        /// 查询已启用角色且不是系统用户
        /// </summary>
        /// <returns></returns>
        public DataResult GetRole()
        {
            var roles = Roles
                .Where(a => a.Enabled)
                .Where(a => !a.IsSystem)
                .Select(a => new Role
                {
                    Id = a.Id,
                    Code = a.Code,
                    Name = a.Name,
                    Enabled = a.Enabled
                })
                .OrderBy(a => a.Id)
                .ToList();
            return DataProcess.Success(roles);
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateRole(Role entity)
        {
            entity.CheckNotNull("entity");
            DataResult result = ValidateRole(entity);
            if (!result.Success) return result;
            entity.Code = SequenceContract.Create(entity.GetType());
            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("角色编码不能为空！");
            }

            //验证角色编码是否存在
            if (Roles.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure("角色编码({0})已经存在！".FormatWith(entity.Code));
            }

            //插入角色
            if (!RoleRepository.Insert(entity))
            {
                return DataProcess.Failure("角色({0})创建失败！".FormatWith(entity.Code));
            }

            return DataProcess.Success("角色({0})创建成功！".FormatWith(entity.Code),entity.Code);
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditRole(Role entity)
        {
            entity.CheckNotNull("entity");
            entity.Id.CheckGreaterThan("Id", 0);

            DataResult result = ValidateRole(entity);
            if (!result.Success) return result;

            var oriEntity = Roles.FirstOrDefault(a => a.Id == entity.Id);
            oriEntity.CheckNotNull("oriEntity");

            //如果是系统角色
            if (oriEntity.IsSystem)
            {
                if (!entity.Enabled)
                {
                    return DataProcess.Failure("角色({0})无法禁用！");
                }
            }

            if (RoleRepository.Update(a => new Role()
            {
                Name = entity.Name,
                Enabled = entity.Enabled,
                Remark = entity.Remark
            }, a => a.Id == entity.Id) == 0)
            {
                return DataProcess.Failure("角色({0})编辑失败！".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("角色({0})编辑成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveRole(int id)
        {
            id.CheckGreaterThan("id", 0);

            var oriEntity = Roles.FirstOrDefault(a => a.Id == id);
            oriEntity.CheckNotNull("oriEntity");

            if (oriEntity.IsSystem)
            {
                return DataProcess.Failure("角色({0})是系统角色，无法移除！");
            }

            if (RoleRepository.Delete(id) == 0)
            {
                return DataProcess.Failure("角色({0})移除失败".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("角色({0})移除成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 验证数据合法性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private DataResult ValidateRole(Role entity)
        {
            if (entity.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("角色名称不能为空！");
            }
            return DataProcess.Success();
        }
    }
}
