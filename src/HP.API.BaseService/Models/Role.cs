using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("角色信息")]
    [Table("Base_Role")]
    public class Role : ServiceEntityBase<int>
    {

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { set; get; }
        /// <summary>
        /// 系统角色
        /// </summary>
        public bool IsSystem { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }


        /// <summary>
        /// 备注
        /// </summary>
        public string DepartmentCode { set; get; }
        
    }
}

