using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("托盘实体")]
    [Table("TB_WMS_Tray")]
    public class Tray : ServiceEntityBase<int>
    {
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string Code { set; get; }

        public override int  Id { set; get; }

        /// <summary>
        /// 货柜编号
        /// </summary>
        public string ContainerCode { set; get; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WareHouseCode { set; get; }


        /// <summary>
        /// 托盘承重
        /// </summary>
        public decimal? MaxWeight { set; get; }

        /// <summary>
        /// 托盘宽度
        /// </summary>
        public int TrayWidth { set; get; }

        /// <summary>
        /// 托盘宽度
        /// </summary>
        public int TrayLength { set; get; }


        /// <summary>
        /// X 方向灯数量
        /// </summary>
        public int XNumber { set; get; }

        /// <summary>
        /// Y 方向灯数量
        /// </summary>
        public int YNumber { set; get; }


        /// <summary>
        /// 布局视图Json
        /// </summary>
        public string LayoutJson { set; get; }

        /// <summary>
        /// 布局视图Json
        /// </summary>
        [NotMapped]
        public string LocationList { set; get; }

        /// <summary>
        /// 布局视图Json
        /// </summary>
        public decimal? LockWeight { set; get; }
        /// <summary>
        /// 托架号 默认为一
        /// </summary>
        public int BracketNumber { get; set; }

        /// <summary>
        /// 托架下托盘号
        /// </summary>
        public int BracketTrayNumber { get; set; }



    }
}

