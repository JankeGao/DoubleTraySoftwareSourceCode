using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class CheckDto:Bussiness.Entitys.CheckMain
    {
        public string CheckDictDescription { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// 储位编码
        /// </summary>
        public string CheckLocationCode { get; set; }

        public string WareHouseName { get; set; }

        /// <summary>
        /// 客户端盘点
        /// </summary>
        public string CheckDetailMaterialList { get; set; }

        public override string StatusCaption => HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.CheckStatusCaption), Status);

    }
}
