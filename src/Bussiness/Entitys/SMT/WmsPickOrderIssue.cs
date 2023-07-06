using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    /// <summary>
    /// 拣货单 关联初始单据表  一个拣货单可对应多个
    /// </summary>
    [Table("TB_WMS_PICK_ORDER_ISSUE")]
    public class WmsPickOrderIssue : ServiceEntityBase<int>
    {
        /// <summary>
        /// 拆分单号
        /// </summary>
        public string PickOrderCode { get; set; }
        /// <summary>
        /// 领料单编号
        /// </summary>
        public int ?Issue_HId { get; set; }
        /// <summary>
        /// 领料单号
        /// </summary>
        public string Issue_No { get; set; }
        /// <summary>
        /// 工单号
        /// </summary>
        public string Wo_No { get; set; }

        public int? Status { get; set; }

        [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.PickOrderStatusEnum), Status.GetValueOrDefault(0));
                }
                return "";
            }
        }
        /// <summary>
        /// 0 出库单 1 调拨单
        /// </summary>
        public int? OrderType { get; set; }

        public string IssueType { get; set; }
    }
}