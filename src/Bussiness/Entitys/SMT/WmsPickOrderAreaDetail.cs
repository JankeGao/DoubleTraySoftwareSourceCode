using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    [Table("TB_WMS_PICK_ORDER_AREA_DETAIL")]
    public class WmsPickOrderAreaDetail : ServiceEntityBase<int>
    {
        /// <summary>
        /// 拣货任务单号
        /// </summary>
        public string PickOrderCode { get; set; }
        /// <summary>
        /// 拣货ReelId
        /// </summary>
        public string ReelId { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaId { get; set; }
        /// <summary>
        /// 库位编码
        /// </summary>
        public string LocationCode { get; set; }
        /// <summary>
        /// Reel原始数量
        /// </summary>
        public int? OrgQuantity { get; set; }
        /// <summary>
        /// 拣货任务明细ID
        /// </summary>
        public string  PickOrderDetailId { get; set; }
        /// <summary>
        /// 站位信息
        /// </summary>
        public string Station_Id { get; set; }

        /// 标志位 
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 实际需要数量
        /// </summary>
        public int? NeedQuantity { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.PickOrderAreaDetailStatusEnum), Status.GetValueOrDefault(0));
                }
                return "";
            }
        }

        public bool? IsNeedSplit { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchCode { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string ReelCreateCode { get; set; }
        /// <summary>
        /// 站台信息
        /// </summary>
        public string Fseqno { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        public string ConfirmDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int RealPickedQuantity { get; set; }

        public string SupplierCode { get; set; }

        [NotMapped]
        public string ShouZiMu
        { get; set; }
        [NotMapped]
        public int? SortNumber
        { get; set; }
    }
    [Table("VIEW_PICK_ORDER_AREA_DETAIL")]
    public class WmsPickOrderAreaDetailVM : WmsPickOrderAreaDetail
    {
        public string Issue_No { get; set; }
        public string Wo_No { get; set; }
        public string Line_Id { get; set; }

        public int? OrgNeedQuantity { get; set; }

        public string MaterialName { get; set; }


    }
}