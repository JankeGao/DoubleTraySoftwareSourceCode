using HP.Core.Dependency;
using HP.Core.Logging;
using HP.Data.Orm;
using HPC.BaseService.Dtos;

namespace HPC.BaseService.Contracts
{
    public interface IOperationLogContract : IScopeDependency
    {
        IQuery<OperationLog> OperationLogs { get; }
        IQuery<LogsOutputDto> LogsOutputDtos { get; }
    }
}