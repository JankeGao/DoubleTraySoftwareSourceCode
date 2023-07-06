using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class HistoryInDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string InCode { get; set; }

        /// <summary>
        /// 入库单行号
        /// </summary>
        public string InWarehouseOneRow { get; set; }

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
        /// 仓库编码
        /// </summary>
        public string WarehouseCode { get; set; }

        public string TrayCode { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string ContainerCode { get; set; }
        /// <summary>
        /// 上架储位
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string BoxName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string BoxUrl { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? InWarehouseTime { get; set; }

        /// <summary>
        /// 删除
        /// </summary>
        public bool IsDeleted { get; set; }

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
        public string Unit { get; set; }

        /// <summary>
        /// 库存类型值
        /// </summary>
        public string Operator { get; set; }


        /// <summary>
        /// 库存类型编码
        /// </summary>
        public string WarehouseTypeCode { get; set; }

        /// <summary>
        /// 库存类型值
        /// </summary>
        public int WarehouseTypeNum { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatorName { get; set; }

        public string Remark2 { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
