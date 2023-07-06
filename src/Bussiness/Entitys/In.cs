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
    [Description("入库单")]
    [Table("TB_WMS_IN")]
    public class In: ServiceEntityBase<int>, ILogicDelete
    {
        /// <summary>
        /// 入库单号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 来源单据号
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

        public bool IsDeleted { get; set; }

        public string BillFields { get; set; }
        /// <summary>
        /// 上架开始时间
        /// </summary>
        public DateTime? ShelfStartTime { get; set; }
        /// <summary>
        /// 上架结束时间
        /// </summary>
        public DateTime? ShelfEndTime { get; set; }

        [NotMapped]
        public List<Bussiness.Entitys.InMaterial> AddMaterial { get; set; }

        [NotMapped]
        public virtual string StatusCaption
        {
            get
            {
                if (Status!=null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.InStatusCaption), Status.Value);
                }
                return "";
            }
        }
        /// <summary>
        /// 捡货单号 调拨单有
        /// </summary>
        public string PickOrderCode { get; set; }
        /// <summary>
        /// 0  普通单据 1 调拨生成
        /// </summary>
        public int? OrderType { get; set; }

        /// <summary>
        /// MOM传入ID
        /// </summary>
        public string CRRCID { get; set; }
    }
}
