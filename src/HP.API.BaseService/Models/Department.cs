using System.Collections.Generic;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("部门")]
    [Table("Base_Department")]
    public class Department : ServiceEntityBase<int>
    {
        public string Code { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { set; get; }
        /// <summary>
        /// 部门性质
        /// </summary>
        public string NatureDict { set; get; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { set; get; }
        /// <summary>
        /// 分机号
        /// </summary>
        public string ENumber { set; get; }
        /// <summary>
        /// 上级编码
        /// </summary>
        public string ParentCode { set; get; }
        /// <summary>
        /// 机构编码
        /// </summary>
        public string OrganizationCode { set; get; }
        /// <summary>
        /// 标志
        /// </summary>
        public string Flag { set; get; }
        /// <summary>
        /// 部门负责人
        /// </summary>
        public string Manager { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
        /// 管理的仓库列表
        /// </summary>
        public string WareHouseManageCodes { get; set; }
        [NotMapped]
        public List<string> WareHouseManageCodeList { get; set; }
    }
}

