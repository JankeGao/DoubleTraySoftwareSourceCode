using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.SMT
{
    [Table("TB_WMS_REEL_LIGHT_AREA_DETAIL")]
    public class WmsReelLightAreaDetail : ServiceEntityBase<int>
    {
        public string LightCode { get; set; }
        /// <summary>
        /// 标志位 0初始 1表示Wms已获取
        /// </summary>
        public int? Status { get; set; }

        [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.ReelLightEnum), Status.GetValueOrDefault(0));
                }
                return "";
            }
        }

        public string AreaId { get; set; }

        public string ReelId { get; set; }

        public string LocationCode { get; set; }

        public string MaterialCode { get; set; }
        public int? OrgQuantity { get; set; }

        public string BatchCode { get; set; }

    }
}