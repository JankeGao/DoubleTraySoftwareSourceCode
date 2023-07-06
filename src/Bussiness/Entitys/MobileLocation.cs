using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("库位移动")]
    [Table("TB_WMS_MobileLocation")]
    public class MobileLocation : ServiceEntityBase<int>
    {
        public  string Code { set; get; }

        public int? Status { set; get; }

        public string WareHouseCode { set; get; }

        public string WareHouseName { set; get; }

        public string MaterialLable { set; get; }

        public string MaterialCode { set; get; }

        public string MaterialName { set; get; }

        public string NewLocationCode { set; get; }

        public  string OldLocationCode { set; get; }

        public string Remark { set; get; }

        [NotMapped]
        public virtual string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MobileLocationStatusEnum),Status.Value);
                }
                return "";
            }
        }


    }
}