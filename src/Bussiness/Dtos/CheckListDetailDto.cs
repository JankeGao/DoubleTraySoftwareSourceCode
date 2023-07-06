using Bussiness.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    /// <summary>
    /// 盘点明细Dto
    /// </summary>
    public class CheckListDetailDto :CheckListDetail
    {
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 物料单位
        /// </summary>
        public string MaterialUnit { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }    
        
        public string AreaName { get; set; }


        public string Code { get; set; }
  
    }
}
