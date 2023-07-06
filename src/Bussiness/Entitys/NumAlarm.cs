using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("库存上下限预警信息")]
    [Table("TB_WMS_NumAlarm")]
    public class NumAlarm : ServiceEntityBase<int>
    {
        
        ///<summary>
        ///物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 上下限预警状态
        /// </summary>
        public int? Status { get; set; }

        [NotMapped]
        public virtual string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MaterialNumStatusCaption),
                        Status.Value);
                }
                return "";
            }
        }
    }
}