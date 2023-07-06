using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class InDto:Entitys.In
    {
        public string InDictDescription { get; set; }
        public string WareHouseName { get; set; }
        //public string Remark { get; set; }

        public override string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.InStatusCaption), Status.Value);
                }
                return "";
            }
        }
    }
}
