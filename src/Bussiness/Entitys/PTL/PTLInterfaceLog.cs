
using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Entitys.PTL
{
    [Table("TB_PTL_INTERFACE_LOG")]
    /// <summary>
    /// PTL请求日志
    /// </summary>
    public class PTLInterfaceLog: EntityBase<int>
    {

        public DateTime ? RequertTime { get; set; }

        public DateTime ? ReposnseTime { get; set; }
        /// <summary>
        /// 调用方法
        /// </summary>
        public string Action { get; set; }

        public string SereverHost { get; set; }

        public string ServerPort { get; set; }

        public string ServiceUri { get; set; }

        public string ClientIp { get; set; }

        public string ClientPort { get; set; }

        public bool IsSuccess { get; set; }

        public string RequestBody { get; set; }

        public string ReponseBody { get; set; }

       // public string 
    }
}
