using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Bussiness.Contracts.SMT
{
    public interface IPTLErrorContract: IScopeDependency
    {
        IRepository<Bussiness.Entitys.PTL.PTLError, int> PTLErrorRepository { get; }

        IRepository<Bussiness.Entitys.PTL.PTLExcuteError, int> PTLExcuteErrorRepository { get; }

        IRepository<Bussiness.Entitys.PTL.PTLInterfaceLog, int> PTLInterfaceLogRepository { get; }

        IQuery<Bussiness.Entitys.PTL.PTLError> PTLErrors { get; }

        IQuery<Bussiness.Entitys.PTL.PTLExcuteError> PTLExcuteErrors { get; }

        IQuery<Bussiness.Entitys.PTL.PTLInterfaceLog> PTLInterfaceLogs { get; }

        DataResult HandleExcuteError(Bussiness.Entitys.PTL.PTLExcuteError error);
    }
}
