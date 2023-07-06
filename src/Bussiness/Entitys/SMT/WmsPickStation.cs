using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    /// <summary>
    /// 初始单据站位表信息
    /// </summary>
    [Table("TB_WMS_PICK_STATION")]
    public class WmsPickStation : ServiceEntityBase<int>
    {
        /// <summary>
        /// 主表ID
        /// </summary>
        public int Issue_HId { get; set; }
        /// <summary>
        /// 明细ID
        /// </summary>
        public int? Issue_LId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string Material_Id { get; set; }
        /// <summary>
        /// 接收数量
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// 站位
        /// </summary>
        public string Station_Id { get; set; }
        /// <summary>
        /// 线别
        /// </summary>
        public string Line_Id { get; set; }
        public string Remark
        {
            get;
            set;
        }
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
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.PickStatusEnum), Status.GetValueOrDefault(0));
                }
                return "";
            }
        }
        /// <summary>
        /// 指定的ReelId
        /// </summary>
        public string AppointReelId { get; set; }
        /// <summary>
        /// 指定的数量
        /// </summary>
        public int? AppointQuantity { get; set; }
        /// <summary>
        /// 是否已分配完毕
        /// </summary>
        [NotMapped]
        public int? IsAssigned { get; set; }
        /// <summary>
        /// 站台序号
        /// </summary>
        public string Fseqno { get; set; }
        /// <summary>
        /// 超发比例
        /// </summary>
        public decimal? OverRatio { get; set; }
    }
    [Table("VIEW_WMS_PICK_STATION")]
    public class WmsPickStationVM : WmsPickStation
    {
        public string MaterialName { get; set; }
        public string Material_Level { get; set; }
    }
}