using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class OutTaskMaterialLabelDto : Entitys.OutTaskMaterialLabel
    {
        public string MaterialName { get; set; }

        public string WareHouseName { get; set; }

        public string TrayCode { get; set; }

        public override string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.OutTaskStatusCaption), Status.Value);
                }
                return "";
            }
        }

        public string MaterialUnit { get; set; }

        public string SupplierName { get; set; }


        public string OutDict { get; set; }

        public decimal UnitWeight { get; set; }


        /// <summary>
        /// 本次出库数量
        /// </summary>
        public decimal OutTaskMaterialQuantity { get; set; }

        public string MaterialUrl { get; set; }
        public string BoxName { get; set; }
        public string BoxUrl { get; set; }
    }
}
