using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("行政区域")]
    [Table(Name ="Base_Region")]
    public class Region : EntityBase<int>
    {
        [Sequence("Seq_Region")]
        public override int Id
        {
            set { base.Id = value; }
            get { return base.Id; }
        }
        public string Code { set; get; }
        /// <summary>
        /// 上级
        /// </summary>
        public string ParentCode { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { set; get; }
        /// <summary>
        /// 经度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Latitude { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Lvl { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
    }
}
