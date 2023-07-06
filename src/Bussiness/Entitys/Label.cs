using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("标签管理")]
    [Table("TB_WMS_LABEL")]
    public class Label: ServiceEntityBase<int>
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 供应商编码， 
        /// </summary>
        public string SupplierCode { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ManufactrueDate { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string BatchCode { get; set; }
        /// <summary>
        /// 删除标志
        /// </summary>
        public bool? IsDeleted { get; set; }   
        /// <summary>
        /// 条码数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 库位地址
        /// </summary>
        [NotMapped]
        public string LocationCode { get; set; }
        [NotMapped]
        public int ShelfDetailId { get; set; }
        [NotMapped]
        public string SplitNo { get; set; }

        [NotMapped] 
        public string ReplenishCode { get; set; }
        [NotMapped]
        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaCode { get; set; }
        [NotMapped]
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }
        [NotMapped]
        public string NewLocationCode { get; set; }

    }
}
