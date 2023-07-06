using Bussiness.Entitys.PTL;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Services.SMT
{
    public class PTLErrorServer : Bussiness.Contracts.SMT.IPTLErrorContract
    {
        public IRepository<PTLError, int> PTLErrorRepository { get; set; }

        public IQuery<PTLError> PTLErrors { get {
                return PTLErrorRepository.Query();
            } }

        public IRepository<PTLExcuteError, int> PTLExcuteErrorRepository { get; set; }

        public IQuery<PTLExcuteError> PTLExcuteErrors { get {
                return PTLExcuteErrorRepository.Query();
            } }

        public IRepository<PTLInterfaceLog, int> PTLInterfaceLogRepository { get; set; }

        public IQuery<PTLInterfaceLog> PTLInterfaceLogs
        {
            get
            {
                return PTLInterfaceLogRepository.Query();
            }
        }

        public DataResult HandleExcuteError(PTLExcuteError error)
        {
            var entity = this.PTLExcuteErrorRepository.GetEntity(error.Id);
            entity.HandledDate = DateTime.Now;
            entity.Handler = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
            entity.Status = 1;
            if (PTLExcuteErrorRepository.Update(entity)>0)
            {
                return DataProcess.Success();
            }
            else
            {
                return DataProcess.Failure("操作失败");
            }

        }
    }
}
