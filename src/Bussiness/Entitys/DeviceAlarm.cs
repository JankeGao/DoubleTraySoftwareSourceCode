using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;
using SqlSugar;


namespace Bussiness.Entitys
{
    [Description("报警信息")]
    [Table("TB_WMS_DeviceAlarm")]
    [SugarTable("TB_WMS_DeviceAlarm")]
    public class DeviceAlarm : ServiceEntityBase<int>
    {
        /// <summary>
        /// 报警编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WarehouseCode { get; set; }
        
        /// <summary>
        /// 延续时间
        /// </summary>
        public int ContinueTime { get; set; }


        /// <summary>
        /// 报警描述
        /// </summary>
        public int? AlarmStatus { get; set; }

        /// <summary>
        /// 报警描述
        /// </summary>
        [NotMapped]
        [SugarColumn(IsIgnore = true)]
        public virtual string AlarmStatusDescription
        {
            get
            {
                if (Code != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.DeviceAlarmEnum), AlarmStatus.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        [NotMapped]
        [SugarColumn(IsIgnore = true)]
        public virtual string StatusDescription
        {
            get
            {
                
                if (Code != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.DeviceAlarmStateEnum), Status.Value);
                }
                return "";
            }
        }

        public int? AlarmDescribe { get; set; }

        public int? IsDeleted { get; set; }
    }
}
