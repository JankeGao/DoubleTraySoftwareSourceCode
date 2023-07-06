using HPC.BaseService.Contracts;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Data.Orm;
using HPC.BaseService.Dtos;

namespace HPC.BaseService.Services
{
    public class OperationLogService : IOperationLogContract
    {
        /// <summary>
        /// 操作日志仓储
        /// </summary>
        public IRepository<OperationLog, int> OperationLogRepository { set; get; }

        public IQuery<OperationLog> OperationLogs
        {
            get { return OperationLogRepository.Query(); }
        }

        public IQuery<LogsOutputDto> LogsOutputDtos
        {
            get
            {
                return OperationLogs.Select(a => new LogsOutputDto
                {
                        Id = a.Id,
                        Name = a.Name,
                        EndTime=a.EndTime,
                        Success=a.Success,
                        BeginTime=a.BeginTime,
                        Ip=a.Ip,
                        Message=a.Message,
                        ModuleName=a.ModuleName,
                        PostData=a.PostData,
                        TotalMilliseconds=a.TotalMilliseconds,
                        Type=a.Type,
                        Url=a.Url,
                        CreatedUserCode = a.CreatedUserCode,
                        CreatedUserName = a.CreatedUserName,
                        CreatedTime = a.CreatedTime,
                    }).OrderByDesc(a => a.CreatedTime); ;
            }
        }
    }
}
