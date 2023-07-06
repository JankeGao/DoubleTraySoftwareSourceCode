using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys.InterFace
{
    [Description("入库物料清单-中间表")]
    [Table("TB_WMS_IF_IN_MATERIAL")]
   public class InMaterialIF : ServiceEntityBase<int>
    {

        public string InDict { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; } 
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ManufactrueDate { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string BatchCode { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierCode { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string BillCode { get; set; }

        /// <summary>
        /// 上架库位
        /// </summary>
        public string LocationCode { get; set; }
        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime? ShelfTime { get; set; }

        /// <summary>
        /// 行项目号
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// 实际入库数量
        /// </summary>
        public decimal? RealInQuantity { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
