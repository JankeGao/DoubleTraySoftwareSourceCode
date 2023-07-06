using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("模块权限")]
    [Table("Base_ModuleAuth")]
    public class ModuleAuth : EntityBase<int>
    {
        [Sequence("Seq_ModuleAuth")]
        public override int Id
        {
            set { base.Id = value; }
            get { return base.Id; }
        }


        /// <summary>
        /// 类别
        /// </summary>
        public int Type { set; get; }
        //类别ID
        public string TypeCode { set; get; }
        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleCode { set; get; }
    }
}
