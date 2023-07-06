using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("领用清单")]
    [Table("TB_WMS_RECEIVE_DETAIL")]
   public class ReceiveDetail : ServiceEntityBase<int>
    {
        /// <summary>
        /// 领用单编码
        /// </summary>
        public string ReceiveCode { get; set; }

        /// <summary>
        /// 来源单据号
        /// </summary>
        public string BillCode { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 模具条码信息
        /// </summary>
        public string MaterialLabel { get; set; }

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
        [NotMapped]
        public virtual string StateDescription
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.ReceiveEnum), Status.Value);
                }
                return "";
            }
        }


        public int? ReceiveType { get; set; }
        public virtual string ReceiveTypeDescription
        {
            get
            {
                if (ReceiveType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.ReceiveTypeEnum), ReceiveType.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 货柜储位
        /// </summary>
        public string LocationCode { get; set; }
    }
}
