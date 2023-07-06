using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Entitys
{
    [Description("巷道信息")]
    [Table("TB_WMS_CHANNEL")]
    public class Channel: ServiceEntityBase<int>
    {
        public string WareHouseCode { get; set; }

        public string Code { get; set; }

        public string AreaCode { get; set; }
        /// <summary>
        /// 是否参与库位规则
        /// </summary>
        public bool IsLocationRule { get; set; }
        /// <summary>
        /// 是否添加了扫描枪
        /// </summary>
        public bool IsScanned { get; set; }
        /// <summary>
        /// M3IP
        /// </summary>
        public string M3XgateIp { get; set; }
        /// <summary>
        /// M3地址
        /// </summary>
        public int? M3DeviceAddress { get; set; }
        /// <summary>
        /// 亮灯索引
        /// </summary>
        public int? M3Index { get; set; }

        public string Remark { get; set; }
    }
}
