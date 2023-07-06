using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Entitys.PTL
{
    [Description("PTL执行错误日志")]
    [Table("TB_PTL_EXCUTE_ERROR")]
    public class PTLExcuteError : EntityBase<int>
    {
        public string WareHouseCode { get; set; }
        /// <summary>
        /// PTL任务单号
        /// </summary>
         public string ProofId { get; set; }
        /// <summary>
        /// 执行区域
        /// </summary>
        public string AreraCode { get; set; }
        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime? HappenedTime { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 异常描述
        /// </summary>
        public string ErrorDescription { get; set; }
        /// <summary>
        /// 异常状态 0 未处理  1已处理  2 已超过处理次数
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 异常处理次数
        /// </summary>
        public int HandleTimes { get; set; }
        /// <summary>
        /// 已执行处理次数
        /// </summary>
        public int HandledTimes { get; set; }
        /// <summary>
        /// 执行出错方法
        /// </summary>
        public string ErrorFunction { get; set; }
        /// <summary>
        /// 执行出错参数
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// 任务单号
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 单号类型
        /// </summary>
        public int OrderType { get; set; }
        [NotMapped]
        public string OrderTypeDescription
        {
            get
            {
                return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.PTL.OrderType), OrderType);
            }
        }

        [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status==0)
                {
                    return "未处理";
                }
                else if (Status==1)
                {
                    return "已处理";
                }
                else
                {
                    return "处理中";
                }
            }
        }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? HandledDate { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public string Handler { get; set; }
    }
}
