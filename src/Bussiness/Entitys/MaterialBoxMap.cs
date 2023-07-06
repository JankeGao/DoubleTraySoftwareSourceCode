using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Data.Orm.Entity;


namespace Bussiness.Entitys
{
    [Description("物料载具映射表")]
    [Table("TB_WMS_MATERIAL_BOX")]

    public class MaterialBoxMap : EntityBase<int>
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 载具类别编码
        /// </summary>
        public string BoxCode { get; set; }

        /// <summary>
        /// 存放数量
        /// </summary>
        public decimal? BoxCount { get; set; }

    }
}
