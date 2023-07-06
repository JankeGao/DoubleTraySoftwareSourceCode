using System;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Table("Base_Register")]
    public class Register : EntityBase<int> 
    {
        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreatedTime { set; get; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobilephone { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 店铺地址
        /// </summary>
        public string Address { get; set; }
    }
}
