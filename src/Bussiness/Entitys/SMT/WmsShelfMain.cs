using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
     [Table("TB_WMS_SHELF_MAIN")]
    public class WmsShelfMain : ServiceEntityBase<int>
    {
         /// <summary>
         /// 补货单号
         /// </summary>
         public string ReplenishCode { get; set; }
         /// <summary>
         /// 货架编码
         /// </summary>
         public string ShelfCode { get; set; }
         /// <summary>
         /// 结束时间
         /// </summary>
         public string EndTime { get; set; }

         public int? Status { get; set; }
         ///// <summary>
         ///// 拆盘单号
         ///// </summary>
         //public string SplitNo { get; set; }

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
         /// 区域编码
         /// </summary>
         public string AreaId { get; set; }
         /// <summary>
         /// 仓库编码
         /// </summary>
         public string WareHouseCode { get; set; }
         /// <summary>
         /// 拆盘单号
         /// </summary>
         public string SplitNo { get; set; }

      

    }

    [Table("VIEW_WMS_SHELF_MAIN")]
    public class WmsShelfMainVM : WmsShelfMain
    {
        public string WareHouseName { get; set; }

    }


}