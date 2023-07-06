using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class AlarmDto : Entitys.Alarm
    {
        public string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MaterialStatusCaption), Status);
                }
                return "";
            }
        }

        /// <summary>
        /// 库位地址
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public int ValidityPeriod { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        public string ManufactureDateFormat
        {
            get
            {
                if (ManufactureDate.HasValue)
                {
                    return ManufactureDate.Value.ToString("yyyy-MM-dd");
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 批次
        /// </summary>
        public string BatchCode { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseName { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public string TrayCode { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        public string LimitDate { get; set; }
        
    }
}
