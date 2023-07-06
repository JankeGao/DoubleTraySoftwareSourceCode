using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Bussiness.Entitys.SMT
{
    [Table("TB_WMS_SPLIT_ISSUE")]
    public class WmsSplitIssue : ServiceEntityBase<int>
    {
        /// <summary>
        /// 拆分单号
        /// </summary>
        public string SplitNo { get; set; }

        public string PickOrderCode { get; set; }

        public string Issue_No { get; set; }

        public string Wo_No { get; set; }
    }

    [Table("VIEW_WMS_SPLIT_ISSUE")]
    public class WmsSplitIssueVM : WmsSplitIssue
    {
        public int Status { get; set; }

        [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.PickOrderStatusEnum), Status);
                }
                return "";
            }
        }
    }
}