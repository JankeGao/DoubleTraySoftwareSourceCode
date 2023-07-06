using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("PTL任务明细表")]
    [Table("tb_ptl_interface")]
    /// <summary>
    /// 订单明细。
    /// </summary>
    public class DpsInterface: ServiceEntityBase<int>
    {
        /// <summary>
        /// 获取或设置主键。
        /// </summary>

        /// <summary>
        /// 获取或设置所属单号。
        /// </summary>
        public string ProofId { get; set; }

        /// <summary>
        /// 获取或设置库位码。
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// 获取或设置批号。
        /// </summary>
        public string BatchNO { get; set; }

        /// <summary>
        /// 获取或设置品名。
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 获取或设置产地。
        /// </summary>
        public string MakerName { get; set; }

        /// <summary>
        /// 获取或设置规格。
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 获取或设置应拣数量。
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 获取或设置实拣数量。
        /// </summary>
        public int? RealQuantity { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 获取或设置下发时间。
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 获取或设置周转箱号。
        /// </summary>
        public string ToteId { get; set; }

        /// <summary>
        /// 获取或设置拣货员工牌。
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 获取或设置拣选时间。
        /// </summary>
        public DateTime? PickedDate { get; set; }

        public int RelationId { get; set; }//关联的物料Id

        public int MaterialLabelId { get; set; }//关联的 出库Label 或者盘点Label Id

        public string OrderCode { get; set; }

    }
}