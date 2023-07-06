using HPC.BaseService.Contracts;
using HP.Core.Data;
using HP.Core.Exceptions;
using HP.Data.Orm;

namespace HPC.BaseService.Services
{
    public class ExceptionService : IExceptionContract
    {
        /// <summary>
        /// 操作日志仓储
        /// </summary>
        public IRepository<ExceptionInfo, string> ExceptionRepository { set; get; }

        public IQuery<ExceptionInfo> Exceptions
        {
            get { return ExceptionRepository.Query(); }
        }

    }
}
