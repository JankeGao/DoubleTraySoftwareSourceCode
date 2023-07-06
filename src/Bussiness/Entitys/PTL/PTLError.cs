using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Entitys.PTL
{
    [Description("PTL错误日志")]
    [Table("TB_PTL_ERROR")]
    public class PTLError : EntityBase<int>
    {
        public string XGateIP { get; set; }

        public int BusIndex { get; set; }

        public int DeviceAddress { get; set; }
        public string ErrorMessage { get; set; }

        public string DeviceTypeName { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
