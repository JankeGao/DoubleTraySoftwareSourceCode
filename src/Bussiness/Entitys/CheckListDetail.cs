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
    [Description("盘点单明细")]
    [Table("TB_WMS_CheckList_Detail")]
    public class CheckListDetail : ServiceEntityBase<int>
    {
        /// <summary>
        /// 盘点编码
        /// </summary>
        public string CheckCode { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// 盘点数量
        /// </summary>
        public decimal? CheckedQuantity { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }

        /// <summary>
        /// 盘点区域编码
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 托盘编码
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 物料条码
        /// </summary>
        public string MaterialLabel { get; set; }

        public string BatchCode { get; set; }

        /// <summary>
        /// 盘点人
        /// </summary>
        public string Checker { get; set; }

        /// <summary>
        /// 盘点时间
        /// </summary>
        public DateTime? CheckedTime { get; set; }

        /// <summary>
        /// 盘点状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public string ManufactureDate { get; set; }
        [NotMapped]
        public virtual string StatusDetailDescription
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
        /// <summary>
        /// 供应商编码
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 托盘编码
        /// </summary>
        public int? TrayId { get; set; }


        
    }
    /// <summary>
    /// 盘点的托盘
    /// </summary>
    public class CheckTray 
    {
        //1 盘点明细改成盘点托盘 不然托盘上的物料数量为0是 不分配盘点任务
        /// <summary>
        /// 
        /// </summary>
        public int TrayId { get;set; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { get; set; }


        public string CheckCode { get; set; }
        /// <summary>
        ///仓库编码
        /// </summary>
        public string WareHourseCode { get; set; }

        
    }
}
