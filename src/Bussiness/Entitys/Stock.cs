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
    [Description("库存信息")]
    [Table("TB_WMS_STOCK")]
    public class Stock: ServiceEntityBase<int>
    {
        /// <summary>
        /// 物料条码
        /// </summary>
        public string MaterialLabel { get; set; }
        /// <summary>
        /// 库位地址
        /// </summary>
        public string LocationCode { get; set; }
        /// <summary>
        /// 物料状态
        /// </summary>
        public int MaterialStatus { get; set; }

        /// <summary>
        /// 库存状态
        /// </summary>
        public int StockStatus { get; set; }
        /// <summary>
        /// 来源单据号
        /// </summary>
        public string BillCode { get; set; }
        /// <summary>
        /// 入库单号
        /// </summary>
        public string InCode { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// 上架日期
        /// </summary>
        public DateTime? ShelfTime { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string BatchCode { get; set; }
        /// <summary>
        /// 供应商编码
        /// </summary>
        public string SupplierCode { get; set; }
        /// <summary>
        /// 推荐库位
        /// </summary>
        public string SuggestLocationCode { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        public string InDate { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        /// 销售订单号
        /// </summary>
        public string SaleBillNo { get; set; }
        /// <summary>
        /// 销售订单号行号
        /// </summary>
        public string SaleBillItemNo { get; set; }
        /// <summary>
        /// 生产订单号
        /// </summary>
        public string ManufactureBillNo { get; set; }
        /// <summary>
        /// 客户编码
        /// </summary>
        public string CustomCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomName { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }
        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 托盘Id
        /// </summary>
        public int? TrayId { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 锁定数量
        /// </summary>
        public decimal LockedQuantity { get; set; }
        /// <summary>
        /// 盘点锁定
        /// </summary>
        public bool? IsCheckLocked { get; set; }

        [NotMapped]
        public string NewLocationCode { get; set; }

    }
    [Table("VIEW_WMS_STOCK")]
    public class StockVM:Stock
    {
        public string MaterialName { get; set; }

        //public string ShelfCode { get; set; }

        public bool IsElectronicMateria { get; set; }

        public string WareHouseName { get; set; }

        public int? MaterialType { get; set; }
        public virtual string MaterialTypeDescription
        {
            get
            {
                if (MaterialType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MaterialTypeEnum), MaterialType.Value);
                }
                return "";
            }
        }
        //public string SupplierName { get; set; }
    }

    [Description("呆滞料信息")]
    [Table("VIEW_WMS_INACTIVESTOCK")]
    public class InactiveStockVM : EntityBase<int>
    {

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }
        
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WareHouseName { get; set; }
        
        /// <summary>
        /// 物料最早一次的入库 日期 
        /// </summary>
        public DateTime? InTime { get; set; }

        /// <summary>
        /// 物料最后一次出库日期
        /// </summary>
        public DateTime? OutTime { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 查询日期距离OutTime或者InTime(当物料还没有出库日期时)的天数
        /// </summary>
        public int? Days { get; set; }
        
    }

    [Description("物料状态信息")]
    [Table("VIEW_WMS_MATERIALSTATUS")]
    public class MaterialStatusVM : EntityBase<int>
    {

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 物料单位
        /// </summary>
        public string MaterialUnit { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 库存锁定数量
        /// </summary>
        public decimal LockedQuantity { get; set; }


    }

    [Description("库存状态信息")]
    [Table("VIEW_WMS_INVENTORYSTATUS")]
    public class InventoryStatusVM : EntityBase<int>
    {

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 物料单位
        /// </summary>
        public string MaterialUnit { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 库存锁定数量
        /// </summary>
        public decimal LockedQuantity { get; set; }

        public decimal? NotShelfQuantity { get; set; }

    }

}
