using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class OutDto:Entitys.Out
    {
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseName { get; set; }
        public string OutDictDescription { get; set; }

        public override string StatusCaption
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
    }
}
