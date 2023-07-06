using Bussiness.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Data.Orm.Entity;

namespace Bussiness.Dtos
{
    public class MouldInformationDto : MouldInformation
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 货柜储位编码
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 货柜储位编码
        /// </summary>
        public decimal Quantity { get; set; }
        

        /// <summary>
        /// 模具名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public  string WareHouseCode { get; set; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 托盘编码
        /// </summary>
        public string TrayCode { get; set; }

        /// <summary>
        /// 物料种类
        /// </summary>
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


        /// <summary>
        /// 领用备注
        /// </summary>
        public string RecipientsOrdersRemarks { get; set; }

    }
}
