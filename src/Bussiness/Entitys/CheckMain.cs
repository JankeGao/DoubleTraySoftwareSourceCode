using System;
using System.Collections.Generic;
using System.ComponentModel;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    /// <summary>
    ///  盘点任务
    /// </summary>
    [Description("盘点单任务")]
    [Table("TB_WMS_Inventory")]
    public class CheckMain : ServiceEntityBase<int>, ILogicDelete
    {
        public string Code { get; set; }
        /// <summary>
        /// 盘点类型
        /// </summary>
        public string CheckDict { get; set; }
        /// <summary>
        /// 盘点仓库
        /// </summary>
        public string WareHouseCode { get; set; }

        /// <summary>
        /// 盘点货柜
        /// </summary>
        public string ContainerCode { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int Status { get; set; }

        public string Remark { get; set; }

        [NotMapped]
        public virtual string StatusCaption => HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.CheckStatusCaption), Status);

        [NotMapped]
        public List<Bussiness.Entitys.Area> AreaCodes { get; set; }

        /// <summary>
        /// 盘点类型
        /// </summary>
        public int? CheckType{ get; set; }
        [NotMapped]
        public virtual string CheckTypeDescription
        {
            get
            {
                if (CheckType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.CheckListEnum), CheckType.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 盘点单编码
        /// </summary>
        public string CheckListCode { get; set; }
    }
}
