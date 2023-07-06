using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class OutMaterialDto : Entitys.OutMaterial
    {
        public string MaterialName { get; set; }

        public string MaterialUnit { get; set; }
        /// <summary>
        /// 可用库存
        /// </summary>
        public decimal? AvailableStock { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }


        /// <summary>
        /// 先进先出类别
        /// </summary>
        public int? FIFOType { get; set; }
        public virtual string FIFOTypeCaption
        {
            get
            {
                if (FIFOType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.FIFOEnum), FIFOType.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 先进先出精度
        /// </summary>
        public int? FIFOAccuracy { get; set; }

        public virtual string FIFOAccuracyCaption
        {
            get
            {
                if (FIFOAccuracy != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.FIFOAccuracyEnum), FIFOAccuracy.Value);
                }
                return "";
            }
        }



    }
}
