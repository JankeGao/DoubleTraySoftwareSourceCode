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
    [Table("TB_WMS_RECEIVE")]
    public class Receive : ServiceEntityBase<int>
    {
        /// <summary>
        /// 领用单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 来源单据号
        /// </summary>
        public string BillCode { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 领用人姓名
        /// </summary>
        public string LastTimeReceiveName { get; set; }

        /// <summary>
        /// 领用时间
        /// </summary>
        public DateTime? LastTimeReceiveDatetime { get; set; }

        /// <summary>
        /// 预计归还时间
        /// </summary>
        public DateTime? PredictReturnTime { get; set; }

        /// <summary>
        /// 领用时长
        /// </summary>
        public int ReceiveTime { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }


        [NotMapped]
        public List<Bussiness.Entitys.ReceiveDetail> AddMaterial { get; set; }
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

        /// <summary>
        ///领用类型
        /// </summary>
        public int? ReceiveType{get;set;}
        [NotMapped]
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
        /// 0  普通单据 1 调拨生成
        /// </summary>
        public int? OrderType { get; set; }

        public string DesignCode { get; set; }
    }
}
