using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("数据字典类型")]
    [Table("Base_DictionaryType")]
    public class DictionaryType : ServiceEntityBase<int>
    {
        [Sequence("Seq_DictionaryType")]
        public override int Id
        {
            set { base.Id = value; }
            get { return base.Id; }
        }
        public string Code { set; get; }
        /// <summary>
        /// 上级编码
        /// </summary>
        public string ParentCode { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public short Sort { set; get; }
    }
}

