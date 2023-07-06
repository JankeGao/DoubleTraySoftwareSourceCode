using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
     [Table("TB_WMS_SHELF_SORT")]
    public class WmsShelfSort : ServiceEntityBase<int>
    {
         /// <summary>
         /// 补货单号
         /// </summary>
         public string ReplenishCode { get; set; }
         /// <summary>
         /// 库位码
         /// </summary>
         public string LocationCode { get; set; }
         /// <summary>
         /// 库位序号
         /// </summary>
         public int? ShelfSortNo { get; set; }
         /// <summary>
         /// 状态
         /// </summary>
         public int? Status { get; set; }

    }
}