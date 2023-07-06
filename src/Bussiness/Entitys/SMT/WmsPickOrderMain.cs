using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    /// <summary>
    /// 拣货单主表
    /// </summary>
    [Table("TB_WMS_PICK_ORDER_MAIN")]
    public class WmsPickOrderMain : ServiceEntityBase<int>
    {
        public string PickOrderCode { get; set; }
        public int? Status { get; set; }

        [NotMapped]
        public string StatusCaption
        {
            get {
                if (Status!=null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.PickOrderStatusEnum), Status.GetValueOrDefault(0));
                }
                return "";
            }
        }

        public bool? IsNeedSplit { get; set; }

        public string SplitNo { get; set; }

        public string WareHouseCode { get; set; }
        /// <summary>
        /// 工单号
        /// </summary>
        public string Wo_No { get; set; }
        /// <summary>
        /// 默认取第一个领料单号
        /// </summary>
        public string Issue_No { get; set; }
        /// <summary>
        ///  0 出库拣货单 1 出库调拨单
        /// </summary>
        public int? OrderType { get; set; }

        [NotMapped]
        public int Index { get; set; }
        /// <summary>
        /// 调入仓库
        /// </summary>
        public string InWareHouseCode { get; set; }

        
    }
}