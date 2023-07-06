using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Entitys
{
    [Table("TB_WMS_Inventory_Area")]
   public class CheckArea : ServiceEntityBase<int>
    {
        public string CheckCode { get; set; }

        public string AreaCode { get; set; }

        public string WareHouseCode { get; set; }

        public string ProofId { get; set; }

        public int Status { get; set; }
        
        [NotMapped]
        public bool IsNeedOffLight { get; set; }

        [NotMapped]
        public virtual string StatusCaption => HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.CheckStatusCaption), Status);
    }
}
