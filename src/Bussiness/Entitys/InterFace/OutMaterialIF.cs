using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys.InterFace
{
    [Description("出库物料清单-中间表")]
    [Table("TB_WMS_IF_OUT_MATERIAL")]
   public class OutMaterialIF: ServiceEntityBase<int>
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; } 
        /// <summary>
        /// 批次
        /// </summary>
        public string BatchCode { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string OutDict { get; set; }
        
        /// <summary>
        /// 客户编码
        /// </summary>
        public string CustomCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public string BillCode { get; set; }

        /// <summary>
        /// 行项目号
        /// </summary>
        public string ItemNo { get; set; }
        /// <summary>
        /// 已下架数量
        /// </summary>
        public decimal? RealPickedQuantity { get; set; }

        public DateTime? PickedTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
