using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    [Table("TB_WMS_SPLIT_AREA_REEL_DETAIL")]
    public class WmsSplitAreaReelDetail : ServiceEntityBase<int>
    {
        /// <summary>
        /// 拆分单号
        /// </summary>
        public string SplitNo { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaId { get; set; }
        /// <summary>
        /// 原始ReelId
        /// </summary>
        public string OrgReelId { get; set; }
        /// <summary>
        /// Reel原始数量
        /// </summary>
        public int ?OrgQuantity { get; set; }
        /// <summary>
        /// 要拆分的数量
        /// </summary>
        public int ?SplitQuantity { get; set; }
        /// <summary>
        /// 拆分后的ReelId
        /// </summary>
        public string SplitReelId { get; set; }
        /// <summary>
        /// 拣货单主表ID
        /// </summary>
        public  string PickOrderCode { get; set; }
        /// <summary>
        /// 拣货单明细关联ID
        /// </summary>
        public string  PickDetailId { get; set; }



        /// <summary>
        /// 标志位 0初始 1表示Wms已获取
        /// </summary>
        public int? Status { get; set; }

       [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.SplitStatusEnum), Status.GetValueOrDefault(0));
                }
                return "";
            }
        }

        public string MaterialCode { get; set; }
        /// <summary>
        /// 是否分配
        /// </summary>
        [NotMapped]
        public string IsAppoint { get {
            if (!string.IsNullOrEmpty(PickDetailId))
            {
                return "已分配";
            }
            else
            {
                return "未分配";
            }
        } }

        [NotMapped]
        public string LocationCode { get; set; }
    }
}