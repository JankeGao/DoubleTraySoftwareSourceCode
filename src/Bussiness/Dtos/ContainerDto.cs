using Bussiness.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class ContainerDto : Entitys.Container
    {

        /// <summary>
        /// 设备品牌
        /// </summary>
        public int? Brand { get; set; }

        public string BrandDescription
        {
            get
            {
                if (Brand != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.EquipmentEnumBrand), Brand.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        public int? EquipmentType { get; set; }

        public string EquipmentTypeDescription
        {
            get
            {
                if (EquipmentType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.EquipmentTypeEnum), EquipmentType.Value);
                }
                return "";
            }
        }


        public string PictureUrl { get; set; }

        /// <summary>
        /// 托盘最大承重
        /// </summary>
        public int? MaxWeight { set; get; }
        
        /// <summary>
        /// 托盘宽度
        /// </summary>
        public int TrayWidth { set; get; }

        /// <summary>
        /// 托盘宽度
        /// </summary>
        public int TrayLength { set; get; }


        /// <summary>
        /// X 方向灯数量
        /// </summary>
        public int XNumber { set; get; }

        /// <summary>
        /// Y 方向灯数量
        /// </summary>
        public int YNumber { set; get; }


        /// <summary>
        /// 托盘数量
        /// </summary>
        public int TrayNumber { set; get; }
        
        /// <summary>
        /// 设备描述
        /// </summary>
        public string Remark { get; set; }

        public string ContainerCode { get; set; }
        public bool IsDelete { get; set; }


        /// <summary>
        /// 库位信息
        /// </summary>
        public List<Tray> children { get; set; }
    }

}
