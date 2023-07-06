using System;
using System.Collections.Generic;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys.InterFace
{
    [Description("出库单中间表")]
    [Table("TB_WMS_IF_OUT")]
    public class OutIF: ServiceEntityBase<int>
    {
        /// <summary>
        /// 单据号
        /// </summary>
        public string BillCode { get; set; }
        /// <summary>
        /// 出库仓库
        /// </summary>
        public string WareHouseCode { get; set; }
        /// <summary>
        /// 出库类型
        /// </summary>
        public string OutDict { get; set; }
        /// <summary>
        /// 出库日期
        /// </summary>
        public string OutDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; } 
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
