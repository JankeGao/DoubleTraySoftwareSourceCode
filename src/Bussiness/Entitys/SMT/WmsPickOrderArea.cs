using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    [Table("TB_WMS_PICK_ORDER_AREA")]
    public class WmsPickOrderArea : ServiceEntityBase<int>
    {
        /// <summary>
        /// 拣货任务单号
        /// </summary>
        public string PickOrderCode { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaId { get; set; }

        /// 标志位 
        /// </summary>
        public int? Status { get; set; }

       [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.PickOrderAreaStatusEnum), Status.GetValueOrDefault(0));
                }
                return "";
            }
        }
        /// <summary>
        /// 当前区域当前的ProofID
        /// </summary>
        public string ProofId { get; set; }
        /// <summary>
        /// 是否可以启动任务
        /// </summary>
        [NotMapped]
        public bool IsCanStart { get; set; }
        /// <summary>
        /// 当前区域有需要拆盘的条码
        /// </summary>
        [NotMapped]
        public bool IsNeedSplit { get; set; }
        /// <summary>
        /// 是否需要灭队
        /// </summary>
        [NotMapped]
        public bool IsNeedOffLight { get; set; }
    }
    [Table("VIEW_PICK_ORDER_AREA")]
    public class WmsPickOrderAreaVM : WmsPickOrderArea
    {
        public string Issue_No { get; set; }

        public string Wo_No { get; set; }
    }

}