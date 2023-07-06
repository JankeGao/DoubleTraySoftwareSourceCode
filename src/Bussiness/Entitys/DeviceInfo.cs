using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Entitys
{
    [Description("设备信息")]
    [Table("TB_WMS_DeviceInfo")]
    public class DeviceInfo : ServiceEntityBase<int>
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
        public int AreaId{ get; set; }
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
       
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

    }
}
