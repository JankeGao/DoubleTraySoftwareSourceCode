using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys.SMT
{
   [Table("TB_WMS_SHELF_DOWN_LOG")]
    public class ShelfDownLog : ServiceEntityBase<int>
    {
       /// <summary>
       /// 容器码
       /// </summary>
       public string ContainerCode { get; set; }
       /// <summary>
       /// 库位码
       /// </summary>
       public string LocationCode { get; set; }
       /// <summary>
       /// 物料编码
       /// </summary>
       public string MaterialCode { get; set; }
       /// <summary>
       /// 物料数量
       /// </summary>
       public int? Quantity { get; set; }


       /// <summary>
       /// 批次号
       /// </summary>
       public string BatchCode { get; set; }
       /// <summary>
       /// 供应商编码
       /// </summary>
       public string SupplierCode { get; set; }

    }
    [Table("VIEW_WMS_SHELF_DOWN_LOG")]
   public class ShelfDownLogVM : ShelfDownLog
   {
       public string MaterialName { get; set; }

       public string SupplierName { get; set; }
   }
}