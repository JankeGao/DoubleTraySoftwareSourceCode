using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class InTaskMaterialDto : Entitys.InTaskMaterial
    {
        public string MaterialName { get; set; }

        public string SuggestTrayCode { get; set; }
        public string MaterialUnit { get; set; }

        public string MaterialUrl { get; set; }
        public string BoxName { get; set; }
        public string BoxUrl { get; set; }

        public string WareHouseName { get; set; }

        /// <summary>
        /// 老化时间
        /// </summary>
        public int AgeingPeriod { get; set; }

        /// <summary>
        /// 是否启用单包
        /// </summary>
        public bool IsPackage { get; set; }

        /// <summary>
        /// 是否混批
        /// </summary>
        public bool IsMaxBatch { get; set; }

        /// <summary>
        /// 是否批次管理
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 本次入库数量
        /// </summary>
        public decimal InTaskMaterialQuantity { get; set; }

        public decimal UnitWeight { get; set; }
        
        /// <summary>
        /// 物料单包数量
        /// </summary>
        public decimal? PackageQuantity { get; set; }

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

        public int ContainerType { get; set; }

        /// <summary>
        /// 托架号 默认为一
        /// </summary>
        public int BracketNumber { get; set; }

        /// <summary>
        /// 托架下托盘号
        /// </summary>
        public int BracketTrayNumber { get; set; }
    }
}
