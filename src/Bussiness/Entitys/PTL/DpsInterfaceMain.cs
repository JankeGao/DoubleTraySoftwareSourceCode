using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Bussiness.Entitys.PTL
{
    [Table("DPSINTERFACEMAIN")]
   public class DpsInterfaceMain:EntityBase<int>
    {
        /// <summary>
        /// 获取或设置单号。
        /// </summary>
        public string ProofId { get; set; }

        /// <summary>
        /// 获取或设置订单类型。0拣货单 1 拆盘单 2 上架任务灯 3 库存点亮
        /// </summary>
        public int? OrderType { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 获取或设置下单时间。
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 获取或设置完成时间。
        /// </summary>
        public string FinishedDate { get; set; }

        public string OrderCode { get; set; }

        public string WareHouseCode { get; set; }

        public string AreaCode { get; set; }

    }
}