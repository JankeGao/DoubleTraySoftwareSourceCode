using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Contracts;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;


namespace Bussiness.Entitys
{
    [Description("储位信息")]
    [Table("TB_WMS_LOCATION")]

    public class Location : ServiceEntityBase<int>
    {
        /// <summary>
        /// 库位编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// X灯号
        /// </summary>
        public int XLight { set; get; }
        /// <summary>
        /// Y灯号
        /// </summary>
        public int YLight { set; get; }

        /// <summary>
        /// 载具编码
        /// </summary>
        public string BoxCode { set; get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Enabled { set; get; }
        /// <summary>
        /// 是否为拣货位
        /// </summary>
        public bool IsLocked { set; get; }
        /// <summary>
        /// 建议物料编码
        /// </summary>
        public string SuggestMaterialCode { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { set; get; }

        /// <summary>
        /// 托盘编码
        /// </summary>
        public int? TrayId { get; set; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { set; get; }

        /// <summary>
        /// 图形可视化Id
        /// </summary>
        public string LayoutId { set; get; }


        /// <summary>
        /// 图形可视化Id
        /// </summary>
        public decimal? LockQuantity { set; get; }
        /// <summary>
        /// 锁定物料
        /// </summary>
        public string LockMaterialCode { get; set; }
        /// <summary>
        /// 托盘号
        /// </summary>
        [NotMapped]
        public string TrayCode { get; set; }
        /// <summary>
        /// X轴灯号长度
        /// </summary>
        public int? XLenght { get; set; }

    }

    /// <summary>
    /// 储位分配逻辑主要应用视图，以储位表为主核查该储位所存放的载具，关联至载具-物料映射表
    /// </summary>
    [Table("VIEW_WMS_LOCATION")]
    //[SugarTable("VIEW_WMS_LOCATION")] // 客户端使用
    public class LocationVIEW : Location
    {
        /// <summary>
        /// 该储位载具可存放的物料
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 该储位存放的物料类别
        /// </summary>
        public int MaterialType{ get; set; }

        /// <summary>
        /// 存储锁定
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 该库位可存储的物料是否存储锁定
        /// </summary>
        public bool IsNeedBlock { get; set; }

        /// <summary>
        /// 该库位可存储的物料是否混批
        /// </summary>
        public bool IsMaxBatch { get; set; }

        /// <summary>
        /// 该库位可存储的物料单位重量
        /// </summary>
        public decimal? UnitWeight { get; set; }


        /// <summary>
        /// 该库位当前存储的物料
        /// </summary>
        public string MaterialLabel { get; set; }

        /// <summary>
        /// 该库位当前存储的物料数量
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// 该库位当前存储的批次
        /// </summary>
        public string BatchCode { get; set; }

        /// <summary>
        /// 该库位所在托盘最大称重
        /// </summary>
        public decimal? MaxWeight { get; set; }

        /// <summary>
        /// 该储位可最多存放的数量
        /// </summary>
        public decimal? BoxCount { get; set; }


        /// <summary>
        /// 该储位可最多存放的数量
        /// </summary>
        public string MaterialName { get; set; }


        /// <summary>
        /// 该储位可最多存放的数量
        /// </summary>
        public string TrayCode { get; set; }

        
    }

    public class LocationVM : Location
    {
        /// <summary>
        /// 托盘编码
        /// </summary>
        public string TrayCode { set; get; }
        /// <summary>
        /// 建议物料名称
        /// </summary>
        public string SuggestMaterialName { set; get; }

        /// <summary>
        /// 建议物料单位
        /// </summary>
        public string SuggestMaterialUnit { set; get; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string BoxName { set; get; }

        /// <summary>
        /// 载具宽度
        /// </summary>
        public int BoxWidth { get; set; }

        /// <summary>
        /// 载具长度
        /// </summary>
        public int BoxLength { get; set; }

        /// <summary>
        /// 库位当前存储的物料
        /// </summary>
        public string StockMaterialLabel { get; set; }

        /// <summary>
        /// 库位当前存储的物料
        /// </summary>
        public decimal? StockLabelQuantity{ get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string StockMaterialBatch { get; set; }


        /// <summary>
        /// 图片地址
        /// </summary>
        public string BoxUrl { get; set; }


        /// <summary>
        /// 可存放的数量
        /// </summary>
        public decimal? AviQuantity { get; set; }


        /// <summary>
        /// 库位当前存储的物料
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 托架号 默认为一
        /// </summary>
        public int BracketNumber { get; set; }

        /// <summary>
        /// 托架下托盘号
        /// </summary>
        public int BracketTrayNumber { get; set; }

        public int ContainerType { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal? StockQuantity { get; set; }

        public decimal? MinStockQuantity { get; set; }

        public decimal? MaxStockQuantity { get; set; }

    }
}

