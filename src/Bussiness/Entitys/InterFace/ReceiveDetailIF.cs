using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys.InterFace
{
    [Description("领用清单-中间表")]
    [Table("TB_WMS_IF_RECEIVE_DETAIL")]
   public class ReceiveDetailIF : ServiceEntityBase<int>
    {

        /// <summary>
        /// 模具条码信息
        /// </summary>
        public string MaterialLabel { get; set; }


        /// <summary>
        /// 单据号
        /// </summary>
        public string BillCode { get; set; }


        /// <summary>
        /// 领用数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 领用人姓名
        /// </summary>
        public string LastTimeReceiveName { get; set; }

        /// <summary>
        /// 领用时间
        /// </summary>
        public DateTime? LastTimeReceiveDatetime { get; set; }

        /// <summary>
        /// 领用时长
        /// </summary>
        public int ReceiveTime { get; set; }

        /// <summary>
        /// 预计归还时间
        /// </summary>
        public DateTime? PredictReturnTime { get; set; }

        /// <summary>
        /// 归还人姓名
        /// </summary>
        public string LastTimeReturnName { get; set; }

        /// <summary>
        /// 归还时间
        /// </summary>
        public DateTime? LastTimeReturnDatetime { get; set; }


        /// <summary>
        /// 领用状态
        /// </summary>
        public int? Status { get; set; }

        public int? ReceiveType { get; set; }

        /// <summary>
        /// 货柜储位
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
