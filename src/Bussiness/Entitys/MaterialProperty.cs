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
    [Description("物料主数据属性组")]
    [Table("TB_WMS_MATERIALPROPERTY")]
    public class MaterialProperty : ServiceEntityBase<int>
    {
        /// <summary>
        /// 属性组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 最低库存
        /// </summary>
        public decimal? MinNum { get; set; }
        /// <summary>
        /// 最大库存
        /// </summary>
        public decimal? MaxNum { get; set; }
        

        /// <summary>
        /// 是否为批物料
        /// </summary>
        public bool IsBatch { get; set; }


        /// <summary>
        /// 库存有效期
        /// </summary>
        public int ValidityPeriod { get; set; }


        /// <summary>
        /// 老化时间
        /// </summary>
        public int AgeingPeriod { get; set; }


        /// <summary>
        /// 先进先出类别
        /// </summary>
        public int? FIFOType { get; set; }

        [NotMapped]
        public virtual string FIFOTypeCaption
        {
            get
            {
                if (FIFOType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.FIFOEnum), FIFOType.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 先进先出精度
        /// </summary>
        public int? FIFOAccuracy { get; set; }

        [NotMapped]
        public virtual string FIFOAccuracyCaption
        {
            get
            {
                if (FIFOAccuracy != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.FIFOAccuracyEnum), FIFOAccuracy.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 存储锁定
        /// </summary>
        public bool IsNeedBlock{ get; set; }

        /// <summary>
        /// 是否混批
        /// </summary>
        public bool IsMaxBatch { get; set; }

        /// <summary>
        /// 是否启用单包条码
        /// </summary>
        public bool IsPackage{ get; set; }

        /// <summary>
        /// 成本中心
        /// </summary>
        public string CostCenter { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

    }
}
