using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys.InterFace
{
    [Description("入库单中间表")]
    [Table("TB_WMS_IF_IN")]
    public class InIF: ServiceEntityBase<int>
    {
        /// <summary>
        /// 单据号
        /// </summary>
        public string BillCode { get; set; }
        /// <summary>
        /// 入库仓库
        /// </summary>
        public string WareHouseCode { get; set; }
        /// <summary>
        /// 入库类型
        /// </summary>
        public string InDict { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        public string InDate { get; set; }
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
