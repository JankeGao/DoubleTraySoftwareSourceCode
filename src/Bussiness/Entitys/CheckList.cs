using System;
using System.Collections.Generic;
using System.ComponentModel;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("盘点单List")]
    [Table("TB_WMS_CheckList")]
    public class CheckList : ServiceEntityBase<int>
    {
        public string Code { get; set; }
        /// <summary>
        /// 盘点类型
        /// </summary>
        public int? CheckDict { get; set; }
        [NotMapped]
        public virtual string CheckDictDescription
        {
            get
            {
                if (CheckDict != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.CheckListEnum), CheckDict.Value);
                }
                return "";
            }
        }
        /// <summary>
        /// 盘点仓库
        /// </summary>
        public string WareHouseCode { get; set; }


        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 盘点备注
        /// </summary>
        public string Remark { get; set; }
        

        [NotMapped]
        public List<CheckListDetail> AddCheckListDetails { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        [NotMapped]
        public virtual string StatusDescription
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.CheckListStatusEnum), Status.Value);
                }
                return "";
            }
        }

        [NotMapped]
        public List<Bussiness.Entitys.Area> AreaCodes { get; set; }
    }
}
