using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
   public class StockDto:Entitys.Stock
    {
        public string MaterialName { get; set; }

        public string SupplierName { get; set; }

        public string WareHouseName { get; set; }

        public string AreaName { get; set; }

        public string MaterialUnit { get; set; }

        public decimal PickedQuantity { get; set; }

        public bool Checked { get; set; }

        public decimal MaxStockNum { get; set; }

        public string TrayCode { get; set; }

        public decimal MinStockNum { get; set; }

        public int ValidityPeriod { get; set; }
        public int? MaterialType { get; set; }

        /// <summary>
        /// 有效期到期时间
        /// </summary>
        public string LimitDate
        {
            get
            {
                if (ManufactureDate != null)
                {
                    return Convert.ToDateTime(ManufactureDate).AddDays(ValidityPeriod).ToString("yyyy-MM-dd");
                }
                return "";
            }
        }
        public  string MaterialStatusDescription
        {
            get
            {
                return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MaterialStatusCaption), MaterialStatus);
            }
        }
        public string StockAge { get; set; }

        public string LayoutId { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string BoxCode { get; set; }

        /// <summary>
        /// 当前载具最多可存放
        /// </summary>
        public decimal? BoxMaxCount { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string BoxName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string BoxUrl { get; set; }


        /// <summary>
        /// 老化周期
        /// </summary>
        public int AgeingPeriod { get; set; }

        /// <summary>
        /// X灯号
        /// </summary>
        public int XLight { set; get; }
        /// <summary>
        /// Y灯号
        /// </summary>
        public int YLight { set; get; }
        /// <summary>
        /// 价格
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string Use { get; set; }

        //public string LocationCodeses { get; set; }

        //public string ManufactureDate { get; set; }

    }
}
