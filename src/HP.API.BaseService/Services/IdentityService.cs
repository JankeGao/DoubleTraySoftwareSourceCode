using HP.Core.Data;
using HP.Core.Security;
using HP.Core.Sequence;
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class IdentityService : IIdentityContract
    {
        /// <summary>
        /// 用户组仓储
        /// </summary>
        //public IRepository<UserGroup, int> UserGroupRepository { set; get; }

        /// <summary>
        /// 角色仓储
        /// </summary>
        public IRepository<Role, int> RoleRepository { set; get; }

        /// <summary>
        /// 用户仓储
        /// </summary>
        public IRepository<User, int> UserRepository { set; get; }

        /// <summary>
        /// 角色用户映射仓储
        /// </summary>
        public IRepository<RoleUserMap, int> RoleUserMapRepository { set; get; }

        /// <summary>
        /// 角色用户映射仓储
        /// </summary>
        public IRepository<UserGroupUserMap, int> UserGroupUserMapRepository { set; get; }

        /// <summary>
        /// 授权
        /// </summary>
        public IAuthorization Authorization { set; get; }

        /// <summary>
        /// 编码序列契约
        /// </summary>
        public ISequenceContract SequenceContract { set; get; }

    }
}
