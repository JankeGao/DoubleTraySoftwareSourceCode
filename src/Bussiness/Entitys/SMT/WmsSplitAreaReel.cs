using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    [Table("TB_WMS_SPLIT_AREA_REEL")]
    public class WmsSplitAreaReel : ServiceEntityBase<int>
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
        /// 库位码
        /// </summary>
        public string LocationCode { get; set; }
        /// <summary>
        /// ReelId
        /// </summary>
        public string ReelId { get; set; }
        /// <summary>
        /// Reel原始数量
        /// </summary>
        public int ?OrgQuantity { get; set; }
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

        public string PickOrderCode { get; set; }
        /// <summary>
        /// 存放MES拆盘主表ID
        /// </summary>
        public int HeadId { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchCode { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string ReelCreateCode { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierCode { get; set; }
    }
}