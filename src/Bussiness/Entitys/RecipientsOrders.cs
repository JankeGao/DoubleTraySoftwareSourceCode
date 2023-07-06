using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("领用订单")]
    [Table("TB_WMS_RECEIVEORDERS")]
    public class RecipientsOrders : ServiceEntityBase<int>
    {
        /// <summary>
        /// 领用单号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 领用数量
        /// </summary>
        public int RecipientsOrdersQuantity { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string RecipientsOrdersRemarks { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 明细单编码
        /// </summary>
        public string InCode { get; set; }


        /// <summary>
        /// 领用人姓名
        /// </summary>
        public string LastTimeReceiveName { get; set; }

        /// <summary>
        /// 领用时间
        /// </summary>
        public string LastTimeReceiveDatetime { get; set; }


        /// <summary>
        /// 预计归还时间
        /// </summary>
        public string PredictReturnTime { get; set; }

        /// <summary>
        /// 领用时长
        /// </summary>
        public int ReceiveTime { get; set; }

        /// <summary>
        /// 归还人姓名
        /// </summary>
        public string LastTimeReturnName { get; set; } 

        /// <summary>
        /// 归还时间
        /// </summary>
        public string LastTimeReturnDatetime { get; set; }


        [NotMapped]
        public List<Bussiness.Entitys.RecipientsOrders> AddMaterial { get; set; }
        /// <summary>
        /// 领用状态
        /// </summary>
        public int? RecipientsOrdersState { get; set; }
        [NotMapped]
        public virtual string RecipientsOrdersDescription
        {
            get
            {
                if (RecipientsOrdersState != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.RecipientsOrdersEnum), RecipientsOrdersState.Value);
                }
                return "";
            }
        }

        /// <summary>
        ///领用类型
        /// </summary>
        public int? RecipientsOrdersType{get;set;}
        [NotMapped]
        public virtual string RecipientsOrdersTypeEnum
        {
            get
            {
                if (RecipientsOrdersType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.RecipientsOrdersTypeEnum), RecipientsOrdersType.Value);
                }
                return "";
            }
        }
    }
}
