using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    /// <summary>
    /// 拣货单明细表
    /// </summary>
    [Table("TB_WMS_PICK_ORDER_DETAIL")]
    public class WmsPickOrderDetail : ServiceEntityBase<int>
    {
        /// <summary>
        /// 拣选任务Code
        /// </summary>
        public string PickOrderCode { get; set; }

        public string MaterialCode { get; set; }

        public string Station_Id { get; set; }
        /// <summary>
        /// 需求数量
        /// </summary>
        public int?  Quantity { get; set; }

        public string Material_Level { get; set; }
        public bool? MaterialIsNeedSplit { get; set; }
        /// <summary>
        /// 是否为电子料
        /// </summary>
        public bool? IsElectronicMateria { get; set; }
        /// <summary>
        /// 用于关联拣货ReelId详细的唯一键
        /// </summary>
        public string DetailId { get; set; }

        [NotMapped]
        public int? IsAssigned { get; set; }
        /// <summary>
        ///已分配数量 当已分配数量大于或等于需求数量时  此时不允许再次分配
        /// </summary>
        public int? CurQuantity { get; set; }

        public int? Status { get; set; }

        [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.PickOrderDetailStatusEnum), Status.GetValueOrDefault(0));
                }
                return "";
            }
        }
        /// <summary>
        /// 站台信息
        /// </summary>
        public string Fseqno { get; set; }
        /// <summary>
        /// 已确认数量
        /// </summary>
        public int? ConfirmQuantity { get; set; }
        /// <summary>
        /// 已作废数量
        /// </summary>
        public int? CancelQuantity { get; set; }
        /// <summary>
        /// 线别
        /// </summary>
        public string Line_Id { get; set; }
        /// <summary>
        /// 原始需求数量
        /// </summary>
        public int? OrgNeedQuantity { get; set; }
        /// <summary>
        /// 单位用量
        /// </summary>
       // public decimal? Unit_Qty { get; set; }
        [NotMapped]
        public int? SortIndex { get; set; }

        [NotMapped]
        public int? OverQuantity { get {

            return ConfirmQuantity.GetValueOrDefault(0) - OrgNeedQuantity.GetValueOrDefault(0);
        } }


        [NotMapped]
        public string ShouZiMu
        { get; set; }
        [NotMapped]
        public int? SortNumber
        { get; set; }
    }
    [Table("VIEW_WMS_PICK_ORDER_DETAIL")]
    public class WmsPickOrderDetailVM : WmsPickOrderDetail
    {
        public string MaterialName { get; set; }
       // public string Material_Level { get; set; }
       // public int? IsNeedSplit { get; set; }
    }
}