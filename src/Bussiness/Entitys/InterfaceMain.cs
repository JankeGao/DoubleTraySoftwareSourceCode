using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("PTL任务主表")]
    [Table("tb_ptl_interfacemain")]
    /// <summary>
    /// 订单表头。
    /// </summary>
    public class DpsInterfaceMain: ServiceEntityBase<int>
    {

        /// <summary>
        /// 获取或设置单号。唯一键  出库单号或者入库单号 或者 盘点单号
        /// </summary>
        public string ProofId { get; set; }

        /// <summary>
        /// 获取或设置订单类型。 0 入库单号 1出库单号 2 盘点单号
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 获取或设置下单时间。
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 获取或设置完成时间。
        /// </summary>
        public DateTime? FinishedDate { get; set; }
        public string OrderCode { get; set; }

    }
}