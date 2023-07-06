using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("标签映射表")]
    [Table("Base_LabelMap")]
    public class LabelMap : EntityBase<int>
    {

        /// <summary>
        /// 编码
        /// </summary>
        public string BCode { set; get; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Name { set; get; }
    }
}
