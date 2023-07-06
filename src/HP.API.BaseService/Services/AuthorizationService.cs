using HP.Core.Data;
using HP.Core.Functions;
using HP.Core.Mapping;
using HP.Core.Security;
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class AuthorizationService : IAuthorizationContract
    {
        /// <summary>
        /// 模块仓储
        /// </summary>
        public IRepository<Models.Module, int> ModuleRepository { set; get; }
        /// <summary>
        /// 功能仓储
        /// </summary>
        public IRepository<Function, int> FunctionRepository { set; get; }
        /// <summary>
        /// 模块授权仓储
        /// </summary>
        public IRepository<ModuleAuth, int> ModuleAuthRepository { set; get; }
        /// <summary>
        /// 功能授权仓储
        /// </summary>
        public IRepository<FunctionAuth, int> FunctionAuthRepository { set; get; }
        /// <summary>
        /// 数据规则仓储
        /// </summary>
        public IRepository<DataRule, int> DataRuleRepository { set; get; }
        /// <summary>
        /// 模块实体映射
        /// </summary>
        public IRepository<ModuleEntityMap,int> ModuleEntityMapRepository { set; get; }
        /// <summary>
        /// 映射
        /// </summary>
        public IMapper Mapper { set; get; }
    }
}
