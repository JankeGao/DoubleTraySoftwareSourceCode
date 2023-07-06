using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;
using SqlSugar;

namespace Bussiness.Entitys
{
    [Description("托盘操作权限映射")]
    [Table("TB_WMS_Tray_User")]
    [SugarTable("TB_WMS_Tray_User")] // 客户端使用
    public class TrayUserMap : EntityBase<int>
    {
        /// <summary>
        /// 托盘Id
        /// </summary>
        public string WareHouseCode { set; get; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { set; get; }

        /// <summary>
        /// 托盘Id
        /// </summary>
        public int TrayId { set; get; }
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { set; get; }

    }
}

