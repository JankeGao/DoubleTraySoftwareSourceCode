using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class InMaterialDto:Entitys.InMaterial
    {
        public string MaterialName { get; set; }

        public int? MaterialType { get; set; }
        public virtual string MaterialTypeDescription
        {
            get
            {
                if (MaterialType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MaterialTypeEnum), MaterialType.Value);
                }
                return "";
            }
        }

        public string MaterialUnit { get; set; }

        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }

        /// <summary>
        /// 物料单包数量
        /// </summary>
        public decimal? PackageQuantity { get; set; }
    }
}
