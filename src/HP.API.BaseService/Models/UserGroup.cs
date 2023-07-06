using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("用户组")]
    [Table("Base_UserGroup")]
    public class UserGroup : ServiceEntityBase<int>
    {
        /// <summary>
        /// 组编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 组名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
    }
}
