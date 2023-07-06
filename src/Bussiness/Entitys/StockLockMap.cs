using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;
using SqlSugar;

namespace Bussiness.Entitys
{
    [Description("库存锁定表")]
    [Table("TB_WMS_STOCK_Lock")]
    public class StockLockMap : EntityBase<int>
    {
        /// <summary>
        /// 托盘Id
        /// </summary>
        public string WarehouseCode { set; get; }
        /// <summary>
        /// 托盘Id
        /// </summary>
        public string LocationCode { set; get; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string MaterialCode { set; get; }

        /// <summary>
        /// 托盘Id
        /// </summary>
        public decimal LockQuantity { set; get; }
        /// <summary>
        /// 用户编码
        /// </summary>
        public string OutTaskCode { set; get; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string BatchCode { set; get; }
        
    }
}

