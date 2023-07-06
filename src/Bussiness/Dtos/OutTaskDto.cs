using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class OutTaskDto : Entitys.OutTask
    {
        public string OutDictDescription { get; set; }
        public string WareHouseName { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string PictureUrl { get; set; }

        public string OutTaskMaterialList { get; set; }
        
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
    }
}
