using System.ComponentModel;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys.Tower
{
    [Description("料塔实体信息")]
    [Table("tower_info")]
    public class Tower 
    {
        /// <summary>
        /// id
        /// </summary>
        public int towerid { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string tower { get; set; }
        /// <summary>
        /// 设备类别
        /// </summary>
        public string type { get; set; }
    }
}
