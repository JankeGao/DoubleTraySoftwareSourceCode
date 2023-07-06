using System;
using System.Collections.Generic;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys.InterFace
{
    [Description("领用订单-中间表")]
    [Table("TB_WMS_IF_RECEIVE")]
    public class ReceiveIF : ServiceEntityBase<int>
    {


        /// <summary>
        /// 单据号
        /// </summary>
        public string BillCode { get; set; }


        /// <summary>
        /// 领用时长
        /// </summary>
        public int ReceiveTime { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }

        /// <summary>
        /// 预计归还时间
        /// </summary>
        public DateTime? PredictReturnTime { get; set; }
        
        /// <summary>
        /// 领用状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        ///领用类型
        /// </summary>
        public int? ReceiveType{get;set;}

        /// <summary>
        /// 领用人姓名
        /// </summary>
        public string LastTimeReceiveName { get; set; }

        /// <summary>
        /// 领用时间
        /// </summary>
        public DateTime? LastTimeReceiveDatetime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
