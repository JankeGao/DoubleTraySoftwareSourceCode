using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("区域信息")]
    [Table("TB_WMS_AREA")]
    public class Area : ServiceEntityBase<int>
    {
        /// <summary>
        /// 区域编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 区域类别
        /// </summary>
        public string CategoryDict { set; get; }
       [NotMapped]
        public string CategoryDictName
        {
            get
            {
                //if (!string.IsNullOrEmpty(CategoryDict))
                //{
                //    return Wms.Business.Dictionary.GetStorageAreaCategoryList().Find(p => p.Code == CategoryDict).Name;
                //}
                //else
                //{
                //    return null;
                //}
                return "";
            }
        }
        /// <summary>
        /// 库位数量
        /// </summary>
        public int? Quantity { set; get; }
        /// <summary>
        /// 仓库编号,关联仓库信息表
        /// </summary>
        public string WareHouseCode { set; get; }

        public string Remark { get; set; }

        [NotMapped]
        public string OrderCode { get; set; }
        [NotMapped]
        public string OrderTypeName { get; set; }
        [NotMapped]
        public int OrderCount { get; set; }
        [NotMapped]
        public int FinishedOrderCount { get; set; }

        [NotMapped]
        public int OrderType { get; set; }

        [NotMapped]
        public bool IsExistOrder { get; set; }

        [NotMapped]
        public int OrderProgress { get; set; }
        [NotMapped]
        public int? Status { get; set; }
    }
}

