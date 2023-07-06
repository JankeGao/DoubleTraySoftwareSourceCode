using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Entitys;
using HP.Core.Data;

namespace Bussiness.Services
{
    public class InterfaceServer : Bussiness.Contracts.IInterfaceContract
    {
        public IRepository<DpsInterface, int> DpsInterfaceRepository { get; set; }

        public IRepository<DpsInterfaceMain, int> DpsInterfaceMainRepository { get; set; }
    }
}
