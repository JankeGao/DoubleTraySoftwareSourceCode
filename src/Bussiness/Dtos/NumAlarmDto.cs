namespace Bussiness.Dtos
{
    public class NumAlarmDto : Entitys.NumAlarm
    {
        /// <summary>
        /// 库存上下限预警状态信息
        /// </summary>
        public override string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MaterialNumStatusCaption), Status.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 库存下限
        /// </summary>
        public decimal? MinNum { get; set; }

        /// <summary>
        /// 库存上限
        /// </summary>
        public decimal? MaxNum { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 库存导出数量
        /// </summary>

        public string Quantityes { get; set; }

        /// <summary>
        /// 已超上下限数量
        /// </summary>
        public decimal OverNum { get; set; }

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
        /// 区域编码
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string TrayCode { get; set; }

        /// <summary>
        /// 库位地址
        /// </summary>
        public string LocationCode { get; set; }

    }
}