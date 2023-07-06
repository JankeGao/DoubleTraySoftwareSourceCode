using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class InTaskDto : Entitys.InTask
    {
        public string InDictDescription { get; set; }
        public string WareHouseName { get; set; }
        /// <summary>
        /// 手动入库明细数据
        /// </summary>
        public string InTaskMaterialList { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string PictureUrl { get; set; }

        public override string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.InTaskStatusCaption), Status.Value);
                }
                return "";
            }
        }
    }
}
