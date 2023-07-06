using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class OutMaterialLabelDto:Entitys.OutMaterialLabel
    {
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WareHouseName { get; set; }

        public string AreaName { get; set; }

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

        /// <summary>
        /// 物料单位
        /// </summary>
        public string MaterialUnit { get; set; }

        public string SupplierName { get; set; }

        public string ItemNo { get; set; }
        public string TrayCode { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string ContainerCode { get; set; }
        /// <summary>
        /// 上架储位
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string BoxName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string BoxUrl { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatorName { get; set; }

        public string Remark2 { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
