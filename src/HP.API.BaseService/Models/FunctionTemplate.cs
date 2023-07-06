using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("功能模版")]
    [Table("Base_FunctionTemplate")]
    public class FunctionTemplate : ServiceEntityBase<int>
    {
        [Sequence("Seq_FunctionTemplate")]
        public override int Id
        {
            set { base.Id = value; }
            get { return base.Id; }
        }

        public string Code { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public short Sort { set; get; }
    }
}
