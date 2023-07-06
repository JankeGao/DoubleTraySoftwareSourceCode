using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class LabelDto:Entitys.Label
    {
        public string MaterialName { get; set; }

        public string MaterialUrl { get; set; }
        public string SupplyName { get; set; }
        public string ManufactrueDateFormat
        {
            get
            {
                if (ManufactrueDate.HasValue)
                {
                    return ManufactrueDate.Value.ToString("yyyy-MM-dd");
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 是否为电子料
        /// </summary>
        public bool IsElectronicMateria { get; set; }


        /// <summary>
        /// 单包数量
        /// </summary>
        public decimal PackageQuantity { get; set; }
        /// <summary>
        /// 条码打印数量
        /// </summary>
        public int LabelCount { get; set; }

    }
}
