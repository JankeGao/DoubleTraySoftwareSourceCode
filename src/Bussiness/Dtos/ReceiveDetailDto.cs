using Bussiness.Entitys;
using System;

namespace Bussiness.Dtos
{
    public class ReceiveDetailDto : ReceiveDetail
    {
 
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 模具名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 物料种类
        /// </summary>
        public int? MaterialType { get; set; }
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
        /// 模具描述
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// X灯号
        /// </summary>
        public int XLight { set; get; }
        /// <summary>
        /// Y灯号
        /// </summary>
        public int YLight { set; get; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 领用备注
        /// </summary>
        public string ReceiveRemarks { get; set; }

        /// <summary>
        /// 归还用户姓名
        /// </summary>
        public string ReturnUserName { get; set; }
    }
}