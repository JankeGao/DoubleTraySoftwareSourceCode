using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("角色用户映射")]
    [Table("Base_RoleUserMap")]
    public class RoleUserMap : EntityBase<int>
    {
        [Sequence("Seq_RoleUserMap")]
        public override int Id
        {
            set { base.Id = value; }
            get { return base.Id; }
        }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { set; get; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleCode { set; get; }
    }
}
