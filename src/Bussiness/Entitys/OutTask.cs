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
    [Description("出库任务单")]
    [Table("TB_WMS_Out_Task")]
    public class OutTask : ServiceEntityBase<int>
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string OutCode { get; set; }


        public string BillCode { get; set; }
        /// <summary>
        /// 出库仓库
        /// </summary>
        public string WareHouseCode { get; set; }

        /// <summary>
        /// 货柜
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 出库类型
        /// </summary>
        public string OutDict { get; set; }
        /// <summary>
        /// 出库日期
        /// </summary>
        public string OutDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; } 
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// 上架开始时间
        /// </summary>
        public DateTime? ShelfStartTime { get; set; }
        /// <summary>
        /// 上架结束时间
        /// </summary>
        public DateTime? ShelfEndTime { get; set; }

        [NotMapped]
        public virtual string StatusCaption
        {
            get
            {
                if (Status!=null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.InTaskStatusCaption), Status.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// MOM传入ID
        /// </summary>
        public string CRRCID { get; set; }

    }
}
