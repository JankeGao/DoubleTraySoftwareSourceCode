using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Data.Orm.Entity;
using SqlSugar;

namespace Bussiness.Entitys
{
    [Description("物料主数据")]
    [Table("TB_WMS_MATERIAL")]
    [SugarTable("TB_WMS_MATERIAL")] // 客户端使用
    public class Material: ServiceEntityBase<int>
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 物料简称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 物料类别
        /// </summary>
        public int? MaterialType { get; set; }

        [NotMapped]
        public virtual string MaterialTypeDescription
        {
            get
            {
                if (MaterialType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MaterialTypeEnum), MaterialType.Value);
                }
                return "";
            }
        }
        /// <summary>
        /// 物料类别
        /// </summary>
        public string CategoryDict { get; set; }
        /// <summary>
        /// 最低库存
        /// </summary>
        public decimal? MinNum { get; set; }
        /// <summary>
        /// 最大库存
        /// </summary>
        public decimal? MaxNum { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackageUnit { get; set; }
        /// <summary>
        /// 包装数量
        /// </summary>
        public decimal? PackageQuantity { get; set; }

        /// <summary>
        /// 是否为批物料
        /// </summary>
        public bool IsBatch { get; set; }
        /// <summary>
        /// 备注1  载具类别编号
        /// </summary>
        public string Remark1 { get; set; }
        /// <summary>
        /// 备注2  价格
        /// </summary>
        public string Remark2 { get; set; }
        /// <summary>
        /// 备注3
        /// </summary>
        public string Remark3 { get; set; }
        /// <summary>
        /// 备注4
        /// </summary>
        public string Remark4 { get; set; }
        /// <summary>
        /// 备注5
        /// </summary>
        public string Remark5 { get; set; }


        public bool IsDeleted { get; set; }

        /// <summary>
        /// 库存有效期
        /// </summary>
        public int ValidityPeriod { get; set; }

        public bool IsNeedSplit { get; set; }

        public string Material_Level { get; set; }
        /// <summary>
        /// 电子料
        /// </summary>
        public bool IsElectronicMateria { get; set; }

        /// <summary>
        /// 超发比例 0 或者NULL 不允许超发  拆盘电子料不允许超发
        /// </summary>
        public decimal? OverRatio { get; set; }

        [NotMapped]
        public string OverRatioDescription
        {
            get
            {
                switch (OverRatio)
                {
                    case null:
                        return "0%";
                    default:
                        return OverRatio + "%";
                }
            }
        }

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
        /// 是否单包
        /// </summary>
        public bool IsPackage { get; set; }
        

        /// <summary>
        /// 单位重量
        /// </summary>
        public decimal UnitWeight { get; set; }


        /// <summary>
        /// 成本中心
        /// </summary>
        public string CostCenter { get; set; }


        /// <summary>
        /// 图片地址
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// 载具箱类别图片
        /// </summary>
        public int? FileID { get; set; }

    }
}
