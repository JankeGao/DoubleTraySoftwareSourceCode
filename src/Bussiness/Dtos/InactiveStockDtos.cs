using System;

namespace Bussiness.Dtos
{
   public class InactiveStockDto:Entitys.Stock
    {
        
        public string MaterialName { get; set; }

        public string WareHouseName { get; set; }

        /// <summary>
        /// 最后一次出库日期
        /// </summary>
        public string OutTime { get; set; }


       
    }
}
