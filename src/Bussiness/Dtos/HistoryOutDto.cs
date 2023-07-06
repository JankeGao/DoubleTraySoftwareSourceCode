using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class HistoryOutDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutCode{ get; set; }

        /// <summary>
        /// 出库单行号
        /// </summary>
        public string OutWarehouseOneRow { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 条码信息
        /// </summary>
        public string BarCodeMessage { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 库位码
        /// </summary>
        public string WarehouseCode { get; set; }

        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? OutWareHouseTime { get; set; }

        /// <summary>
        /// 删除
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        ///  材料标签
        /// </summary>
        public string MaterialLabel { get; set; }

        /// <summary>
        /// 行项目号
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string MaterialUnit { get; set; }

        /// <summary>
        /// 库存类型编码
        /// </summary>
        public string WarehouseTypeCode { get; set; }

        /// <summary>
        /// 库存类型值
        /// </summary>
        public int WarehouseTypeNum { get; set; }

        /// <summary>
        /// 出库人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
       

    }
}
