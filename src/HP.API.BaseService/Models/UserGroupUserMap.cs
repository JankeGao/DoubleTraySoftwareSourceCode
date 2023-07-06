using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("用户组用户映射")]
    [Table("Base_UserGroupUserMap")]
    public class UserGroupUserMap : EntityBase<int>
    {
        [Sequence("Seq_UserGroupUserMap")]
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
        /// 用户组编码
        /// </summary>
        public string UserGroupCode { set; get; }
    }
}
