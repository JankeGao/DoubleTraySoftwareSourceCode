using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;


namespace Bussiness.Entitys
{
    [Description("模具信息")]
    [Table("TB_WMS_MouldInformation")]
    public class MouldInformation : ServiceEntityBase<int>
    {
        /// <summary>
        /// 模具条码
        /// </summary>
        public string MaterialLabel { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? MouldState { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }


        /// <summary>
        /// 上次领用人
        /// </summary>
        public  string LastTimeReceiveName { get; set; }

        /// <summary>
        /// 上次领用时间
        /// </summary>
        public DateTime? LastTimeReceiveDatetime { get; set; }

        /// <summary>
        ///领用时长
        /// </summary>
        public int? ReceiveTime { get; set; }

  
        /// <summary>
        /// 上次归还人
        /// </summary>
        public string LastTimeReturnName { get; set; }

        /// <summary>
        /// 上次归还时间
        /// </summary>
        public DateTime? LastTimeReturnDatetime { get; set; }


        /// <summary>
        /// 领用单号
        /// </summary>
        public string RecipientsCode { get; set; }


        /// <summary>
        ///领用类型
        /// </summary>
        public int? ReceiveType { get; set; }
        public virtual string ReceiveTypeDescription
        {
            get
            {
                if (ReceiveType != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.ReceiveTypeEnum), ReceiveType.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        [NotMapped]
        public virtual string StateDescription
        {
            get
            {
                if (MouldState != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MouldInformationEnum), MouldState.Value);
                }
                return "";
            }
        }

        [NotMapped]
        public List<Bussiness.Entitys.MouldInformation> AddMouldInformation { get; set; }
    }
}
