using Bussiness.Entitys;
using System;

namespace Bussiness.Dtos
{
    public class RecipientsOrdersDto : RecipientsOrders
    {
 
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 货柜储位
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 模具编码
        /// </summary>
        public string MouldCode { get; set; }

        /// <summary>
        /// 模具名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 物料种类
        /// </summary>
        public int? MaterialType { get; set; }

        /// <summary>
        /// 模具条码
        /// </summary>
        public string MaterialLabel { get; set; }

        /// <summary>
        /// 模具描述
        /// </summary>
        public string Remarks { get; set; }





    }
}