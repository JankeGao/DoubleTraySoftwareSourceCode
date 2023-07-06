using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Core.Security;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    /// <summary>
    /// 数据表实体类：User 
    /// </summary>
    [Description("用户信息")]
    [Table("Base_User")]
    public class User : UserBase ,ILogicDelete
    {
        /// <summary>
        /// Sex
        /// </summary>                       
        public int Sex { get; set; }

        /// <summary>
        /// Birthdate
        /// </summary>                       
        public DateTime? Birthdate { get; set; }

        /// <summary>
        /// Telephone
        /// </summary>                       
        public string Telephone { get; set; }

        /// <summary>
        /// Mobilephone
        /// </summary>                       
        public string Mobilephone { get; set; }

        /// <summary>
        /// Email
        /// </summary>                       
        public string Email { get; set; }

        /// <summary>
        /// WeXin
        /// </summary>                       
        public string WeXin { get; set; }

        /// <summary>
        /// Qq
        /// </summary>                       
        public string WeChatOpenId { get; set; }

        /// <summary>
        /// Remark
        /// </summary>                       
        public string Remark { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { set; get; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// 载具箱类别图片
        /// </summary>
        public int? FileID { get; set; }


        /// <summary>
        /// RFIDCode
        /// </summary>
        public string RFIDCode { get; set; }
        
    }
}