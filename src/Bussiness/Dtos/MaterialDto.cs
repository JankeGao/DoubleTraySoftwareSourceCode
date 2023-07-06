using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class MaterialDto:Entitys.Material
    {
        /// <summary>
        /// 载具箱类别
        /// </summary>
        public string BoxCode { get; set; }

        /// <summary>
        /// 载具箱类别
        /// </summary>
        public string BoxName { get; set; }
        /// <summary>
        /// 载具箱存放数量
        /// </summary>
        public decimal? BoxCount { get; set; }

        /// <summary>
        /// 载具箱存放数量
        /// </summary>
        public decimal? BoxLength { get; set; }


        /// <summary>
        /// 载具箱存放数量
        /// </summary>
        public decimal? BoxWidth { get; set; }

        /// <summary>
        /// 载具箱类别图片
        /// </summary>
        public string BoxPictureUrl { get; set; }

        /// <summary>
        /// 载具箱类别图片
        /// </summary>
        public int? FileID { get; set; }
    }
}
