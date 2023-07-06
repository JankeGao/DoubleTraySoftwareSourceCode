using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bussiness.Dtos
{
    public class DeviceAlarmDto : Entitys.DeviceAlarm
    {
        /// <summary>
        /// 设备品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string EquipmentType { get; set; }

        /// <summary>
        /// 设备描述
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }
    }
}
