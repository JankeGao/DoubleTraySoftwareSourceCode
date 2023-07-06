using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;
using SqlSugar;
//using SqlSugar;

namespace Bussiness.Entitys
{
    [Description("货柜实体")]
    [Table("TB_WMS_Container")]
    [SugarTable("TB_WMS_Container")]
    public class Container : ServiceEntityBase<int>
    {
        /// <summary>
        /// 货柜编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 货柜型号
        /// </summary>
        public string EquipmentCode { set; get; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { set; get; }

        /// <summary>
        /// Ip 地址
        /// </summary>
        public string Ip { set; get; }

        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { set; get; }

        /// <summary>
        /// 货柜唯一码与客户端确认
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        public int Status { get; set; }


        /// <summary>
        /// 状态描述
        /// </summary>
        [NotMapped]
        [SugarColumn(IsIgnore = true)]
        public string StatusCaption
        {
            get
            {
                return HP.Utility.EnumHelper.GetCaption(typeof(DeviceStatusEnum), Status);
            }
        }
        

        /// <summary>
        /// 报警状态
        /// </summary>
        public int AlarmStatus { get; set; }


        /// <summary>
        /// 状态描述
        /// </summary>
        [NotMapped]
        [SugarColumn(IsIgnore = true)]
        public string ALarmStatusCaption
        {
            get
            {
                return HP.Utility.EnumHelper.GetCaption(typeof(DeviceAlarmStateEnum), AlarmStatus);
            }
        }

        /// <summary>
        /// 货柜型号
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 是否虚拟货柜
        /// </summary>
        public bool IsVirtual { get; set; }
        /// <summary>
        /// 货柜类型  0 朗杰回转柜  1 卡迪斯货柜 2 亨乃尔货柜 3 朗杰升降柜
        /// </summary>
        public int ContainerType { get; set; }
    }
}

