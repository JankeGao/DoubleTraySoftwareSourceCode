using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Data.Orm.Entity;


namespace Bussiness.Entitys
{
    [Description("设备型号管理")]
    [Table("TB_WMS_EquipmentType")]

    public class EquipmentType : ServiceEntityBase<int>
    {
        //型号
        public string Code { get; set; }

        //型号描述
        public string Remark { get; set; }

        //品牌
        public int? Brand { get; set; }
        [NotMapped]
        public virtual string BrandDescription
        {
            get
            {
                if (Type != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.EquipmentEnumBrand), Brand.Value);
                }
                return "";
            }
        }

        //类型
        public int? Type { get; set; }
        [NotMapped]
        public virtual string TypeDescription
        {
            get
            {
                if (Type != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.EquipmentTypeEnum), Type.Value);
                }
                return "";
            }
        }

        //是否是逻辑删除
        public bool IsDeleted { get; set; }

        //图片路径
        public string PictureUrl { get; set; }

        //图片ID
        public int FileID { get; set; }

        

    }
}
