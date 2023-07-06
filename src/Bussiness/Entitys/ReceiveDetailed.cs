using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("领用清单")]
    [Table("TB_WMS_RECEIVEORDERS_DETAIL")]
   public class ReceiveDetailed : ServiceEntityBase<int>
    {
        /// <summary>
        /// 领用清单编码
        /// </summary>
        public string InCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        //public string SupplierName { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 模具信息编码
        /// </summary>
        public string MouldCode { get; set; }

        /// 行项目号
        /// </summary>
        //public string ItemNo { get; set; }
        /// <summary>
        /// 实际入库数量
        /// </summary>
        // public decimal RealInQuantity { get; set; }
    }
}
