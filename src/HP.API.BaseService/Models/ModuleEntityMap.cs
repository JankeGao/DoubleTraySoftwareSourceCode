using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("模块实体映射")]
    [Table("Base_ModuleEntityMap")]
    public class ModuleEntityMap : EntityBase<int>
    {
        [Sequence("Seq_ModuleEntityMap")]
        public override int Id
        {
            set { base.Id = value; }
            get { return base.Id; }
        }

        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleId { set; get; }

        /// <summary>
        /// 实体编码
        /// </summary>
        public string EntityId { set; get; }
    }
}
