using System.ComponentModel;
using DeltaRT.Core.Data;
using DeltaRT.Data.Orm.Entity;

namespace DeltaRT.BaseService.Models
{
    [Description("功能权限")]
    [Table("Base_FunctionAuth")]
    public class FunctionAuth : EntityBase<int>
    {
        [Sequence("Seq_FunctionAuth")]
        public override int Id
        {
            set { base.Id = value; }
            get { return base.Id; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public int Type { set; get; }
        /// <summary>
        /// 类型编码
        /// </summary>
        public string TypeCode { set; get; }
        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleId { set; get; }
        /// <summary>
        /// 功能编码
        /// </summary>
        public string FunctionCode { set; get; }
    }
}
