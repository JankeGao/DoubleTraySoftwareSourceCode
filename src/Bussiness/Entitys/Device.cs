using System.ComponentModel;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("设备信息")]
    [Table("TB_WMS_Device")]
    public class Device : ServiceEntityBase<int>
    {
        public string Name { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 设备地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 设备端口号
        /// </summary>
        public  int Port { get; set; }
        /// <summary>
        /// 所属区域Id
        /// </summary>
        public int AreaCode{ get; set; }
        /// <summary>
        /// 设备服务器地址
        /// </summary>
        public string ServerAddress { get; set; }
        /// <summary>
        /// 通道
        /// </summary>
        public int Channel { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public int Status { get; set; }

        [NotMapped]
        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatusCaption
        {
            get
            {
                return HP.Utility.EnumHelper.GetCaption(typeof(DeviceStatusEnum), Status);
            }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WarehouseCode { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 数据库地址
        /// </summary>
        public string SqlIp { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string SqlDatabase { get; set; }

        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string SqlUserName { get; set; }

        /// <summary>
        /// 数据库密码
        /// </summary>
        public string SqlPassword { get; set; }
    }
}
