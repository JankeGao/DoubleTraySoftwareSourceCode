using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("系统标签管理")]
    [Table("Base_Label")]
    public class Label : ServiceEntityBase<int>
    {

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
    }
}
