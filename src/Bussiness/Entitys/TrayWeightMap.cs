using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("托盘重量映射")]
    [Table("TB_WMS_Tray_Weight")]
    public class TrayWeightMap : EntityBase<int>
    {
        /// <summary>
        /// 最大存放重量
        /// </summary>
        public decimal? MaxWeight { set; get; }

        /// <summary>
        /// 已存放重量
        /// </summary>
        public decimal? LockWeight { set; get; }

        /// <summary>
        /// 托盘Id
        /// </summary>
        public int TrayId { set; get; }
        /// <summary>
        /// 储位推荐存放的重量
        /// </summary>
        public decimal? TempLockWeight { set; get; }

    }
}

