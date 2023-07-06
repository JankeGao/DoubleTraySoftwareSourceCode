using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    /// <summary>
    /// 初始单据明细表
    /// </summary>
    [Table("TB_WMS_PICK_DETAIL")]
    public class WmsPickDetail : ServiceEntityBase<int>
    {
        /// <summary>
        /// 备料单号
        /// </summary>
        public string Issue_No { get; set; }
        /// <summary>
        /// 账本ID
        /// </summary>
        public int? Org_Id { get; set; }
        /// <summary>
        /// 主表ID
        /// </summary>
        public int Issue_HId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 接收数量
        /// </summary>
        public int?  Quantity { get; set; }
        /// <summary>
        ///出库仓库编码
        /// </summary>
        public string  Out_WareHouse_No { get; set; }

        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 调入仓库
        /// </summary>
        public string In_WareHouse_No { get; set; }
        /// <summary>
        /// 单位用量
        /// </summary>
        public decimal? Unit_Qty { get; set; }
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
    }
    [Table("VIEW_WMS_PICK_DETAIL")]
    public class WmsPickDetailVM : WmsPickDetail
    {
        public string MaterialName { get; set; }
        public string Material_Level { get; set; }
        public bool? IsNeedSplit { get; set; }

        public bool? IsElectronicMateria { get; set; }
    }
}