using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class IdentityService
    {
        //public IQuery<UserGroup> UserGroups
        //{
        //    get { return UserGroupRepository.Query(); }
        //}

        ///// <summary>
        ///// 创建用户组
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //public DataResult CreateUserGroup(UserGroup entity)
        //{
        //    DataResult result = ValidateUserGroup(entity);
        //    if (!result.Success) return result;

        //    if (entity.Code.IsNullOrEmpty())
        //    {
        //        return DataProcess.Failure("用户组编码不能为空！");
        //    }

        //    //验证用户组编码是否存在
        //    if (UserGroups.Any(a => a.Code == entity.Code))
        //    {
        //        return DataProcess.Failure("用户组编码({0})已经存在！".FormatWith(entity.Code));
        //    }

        //    //插入用户组
        //    if (!UserGroupRepository.Insert(entity))
        //    {
        //        return DataProcess.Failure("用户组({0})创建失败！".FormatWith(entity.Code));
        //    }

        //    return DataProcess.Success("用户组({0})创建成功！".FormatWith(entity.Code));
        //}

        ///// <summary>
        ///// 编辑用户组
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //public DataResult EditUserGroup(UserGroup entity)
        //{
        //    DataResult result = ValidateUserGroup(entity);
        //    if (!result.Success) return result;

        //    entity.Id.CheckGreaterThan("Id", 0);

        //    var oriEntity = UserGroups.Where(a => a.Id == entity.Id).Select(a => new {a.Code}).FirstOrDefault();
        //    oriEntity.CheckNotNull("oriEntity");

        //    if (UserGroupRepository.Update(a => new UserGroup
        //    {
        //        Name = entity.Name,
        //        Enabled = entity.Enabled,
        //        Remark = entity.Remark
        //    }, a => a.Id == entity.Id) == 0)
        //    {
        //        return DataProcess.Failure("用户组({0})编辑失败！".FormatWith(oriEntity.Code));
        //    }

        //    return DataProcess.Success("用户组({0})编辑成功！".FormatWith(oriEntity.Code));
        //}

        ///// <summary>
        ///// 移除用户组
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public DataResult RemoveUserGroup(int id)
        //{
        //    id.CheckGreaterThan("id", 0);

        //    var oriEntity = UserGroups.Where(a => a.Id == id).Select(a => new {a.Code}).FirstOrDefault();
        //    oriEntity.CheckNotNull("oriEntity");

        //    UserGroupRepository.UnitOfWork.TransactionEnabled = true;

        //    //移除用户组
        //    if (UserGroupRepository.Delete(id) == 0)
        //    {
        //        return DataProcess.Failure("用户组({0})移除失败".FormatWith(oriEntity.Code));
        //    }

        //    //移除用户组用户映射
        //    DataResult result = RemoveUserGroupUsersMap(oriEntity.Code);
        //    if (!result.Success) return result;

        //    UserGroupRepository.UnitOfWork.Commit();

        //    return DataProcess.Success("用户组({0})移除成功！".FormatWith(oriEntity.Code));
        //}

        ///// <summary>
        ///// 验证数据合法性
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //private DataResult ValidateUserGroup(UserGroup entity)
        //{
        //    entity.CheckNotNull("entity");

        //    if (entity.Name.IsNullOrEmpty())
        //    {
        //        return DataProcess.Failure("用户组名称不能为空！");
        //    }

        //    return DataProcess.Success();
        //}
    }
}
