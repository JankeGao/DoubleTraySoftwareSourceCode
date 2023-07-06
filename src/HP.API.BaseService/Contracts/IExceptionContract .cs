using HP.Core.Dependency;
using HP.Core.Exceptions;
using HP.Data.Orm;

namespace HPC.BaseService.Contracts
{
    public interface IExceptionContract : IScopeDependency
    {
        IQuery<ExceptionInfo> Exceptions { get; }
    }
}