using Bussiness.Entitys;
using System;

namespace Bussiness.Dtos
{
    public class ReceiveTaskDetailDto : ReceiveTaskDetail
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 模具名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 领用数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 货柜储位
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 模具描述
        /// </summary>
        public string MouldRemarks { get; set; }

        /// <summary>
        /// 归还用户名
        /// </summary>
        public string ReturnUserName { get; set; }

        /// <summary>
        /// X灯号
        /// </summary>
        public int XLight { set; get; }
        /// <summary>
        /// Y灯号
        /// </summary>
        public int YLight { set; get; }

        /// <summary>
        /// 模具种类信息
        /// </summary>
        public int? MaterialType { get; set; }

        /// <summary>
        /// 托盘编码
        /// </summary>
        public string TrayCode { get; set; }

        /// <summary>
        /// 托盘编码
        /// </summary>
        public int? TrayId { get; set; }

        public string MaterialUrl { get; set; }
        public string BoxName { get; set; }
        public string BoxUrl { get; set; }
    }
}