using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class CheckDetailDto:Bussiness.Entitys.CheckDetail
    {
        public string MaterialName { get; set; }

        public string MaterialUnit { get; set; }

        public string SupplierName { get; set; }

        public string ContainerCode { get; set; }
        public int? TrayId { get; set; }

        public string TrayCode { get; set; }
        public string WareHouseName { get; set; }

        public string AreaName { get; set; }
        public override string StatusCaption => HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.CheckStatusCaption), Status);

    }
}
