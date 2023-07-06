using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;


namespace Bussiness.Entitys
{
    [Description("库存信息")]
    [Table("TB_WMS_WAREHOUSE")]
    public class WareHouse : ServiceEntityBase<int>
    {
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 仓库地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public string CategoryDict { set; get; }
        [NotMapped]
        public string CategoryDictName
        {
            get
            {
                return "";
            }
        }
        /// <summary>
        /// 是否为虚拟库
        /// </summary>
        public bool? IsVirtual { set; get; }
        /// <summary>
        /// 是否允许管理库位
        /// </summary>
        public bool? AllowManage { set; get; }

        public string Remark { get; set; }

        [NotMapped]
        /// <summary>
        ///  货柜信息
        /// </summary>
        public System.Collections.Generic.List<Dtos.ContainerDto> children { get; set; }
    }
}

