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
    [Description("盘点明细")]
    [Table("TB_WMS_Inventory_Detail")]
    public class CheckDetail : ServiceEntityBase<int>, ILogicDelete
    {
        public string CheckCode { get; set; }

        public bool IsDeleted { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 盘点数量
        /// </summary>
        public decimal? CheckedQuantity { get; set; }

        public string MaterialCode { get; set; }

        public string WareHouseCode { get; set; }

        public string AreaCode { get; set; }

        public string LocationCode { get; set; }

        public string MaterialLabel { get; set; }

        public string BatchCode { get; set; }

        public string Checker { get; set; }

        public DateTime? CheckedTime { get; set; }

        public int Status { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ManufactureDate { get; set; }
        [NotMapped]
        public virtual string StatusCaption => HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.CheckStatusCaption), Status);

        public string SupplierCode { get; set; }
    }
}
