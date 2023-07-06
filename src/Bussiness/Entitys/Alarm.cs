using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;


namespace Bussiness.Entitys
{
    [Description("预警信息")]
    [Table("TB_WMS_Alarm")]
    public class Alarm : ServiceEntityBase<int>
    {
        /// <summary>
        /// 物料条码
        /// </summary>
        public string MaterialLabel { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

    }
}
