using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
     [Table("TB_WMS_SHELF_DETAIL")]
    public class WmsShelfDetail : ServiceEntityBase<int>
    {
         /// <summary>
         /// 补货单号
         /// </summary>
         public string ReplenishCode { get; set; }
         /// <summary>
         /// ReelId
         /// </summary>
         public string ReelId { get; set; }
         /// <summary>
         /// 物料编码
         /// </summary>
         public string MaterialCode { get; set; }
         /// <summary>
         /// 供应商编码
         /// </summary>
         public string SupplierCode { get; set; }
         /// <summary>
         /// 批次号
         /// </summary>
         public string BatchCode { get; set; }
         /// <summary>
         /// 库位码
         /// </summary>
         public string LocationCode { get; set; }
         /// <summary>
         /// 实际数量
         /// </summary>
         public int? Quantity { get; set; }
         /// <summary>
         /// 原始数量
         /// </summary>
         public int? OrgQuantity { get; set; }
         /// <summary>
         /// 生产日期
         /// </summary>
         public DateTime? ReelCreateCode { get; set; }
         public int? Status { get; set; }

         [NotMapped]
         public string StatusCaption
         {
             get {
                 if (Status!=null)
                 {
                     return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.ReplenishStatusEnum), Status.GetValueOrDefault(0));
                 }
                 return "";
             }

         }
         /// <summary>
         /// 完成时间
         /// </summary>
         public string FinishedTime { get; set; }
         /// <summary>
         /// 货架排序号
         /// </summary>
         public int? ShelfSortNo { get; set; }
         /// <summary>
         /// 返回上架明细唯一值
         /// </summary>
         public string UniqueValue { get; set; }
         /// <summary>
         /// 关联PTL
         /// </summary>
         public string ProofId { get; set; }

         [NotMapped]
         public int? CurrentMaterialReelIdCount
         {
             get;
             set;
         }
    }

    [Table("VIEW_WMS_SHELF_DETAIL")]
    public class WmsShelfDetailVM : WmsShelfDetail
    {
        public string SupplierName { get; set; }
    }
}