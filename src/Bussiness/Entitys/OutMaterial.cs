using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("出库物料清单")]
    [Table("TB_WMS_OUT_MATERIAL")]
   public class OutMaterial: ServiceEntityBase<int>
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutCode { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; } 
        /// <summary>
        /// 批次
        /// </summary>
        public string BatchCode { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string OutDict { get; set; }
        
        /// <summary>
        /// 客户编码
        /// </summary>
        public string CustomCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? IsDeleted { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string BillCode { get; set; }
        /// <summary>
        /// 推荐库位
        /// </summary>
        public string SuggestLocation { get; set; }


        [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.OutStatusCaption), Status.Value);
                }
                return "";
            }
        }
        /// <summary>
        /// 行项目号
        /// </summary>
        public string ItemNo { get; set; }
        /// <summary>
        /// 已下架数量
        /// </summary>
        public decimal? PickedQuantity { get; set; }

        public DateTime? PickedTime { get; set; }
        /// <summary>
        /// 复核数量
        /// </summary>
        public decimal? CheckedQuantity { get; set; }


        /// <summary>
        /// 下发任务数量
        /// </summary>
        public decimal? SendInQuantity { get; set; }

        /// <summary>
        /// MOM传入ID
        /// </summary>
        public string CRRCID { get; set; }
        public string WORKSTATIONID { get; set; }
        public string MERGEID { get; set; }
        public string WAREHBINID { get; set; }

    }
}
