using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace DF.Web.Models
{
    [Table("Base_ToDo")]
    public class ToDo:ServiceEntityBase<int>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Lvl { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { set; get; }
    }
}
