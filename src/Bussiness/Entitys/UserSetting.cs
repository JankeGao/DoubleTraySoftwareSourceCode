using HP.Core.Data.Infrastructure;
using HP.Core.Security;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Entitys
{
    [Description("用户信息")]
    [Table("Base_User")]
    public class UserSetting : UserBase, ILogicDelete
    {

        public int Sex { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Telephone { get; set; }
        public string Mobilephone { get; set; }
        public string Email { get; set; }
        public string WeXin { get; set; }
        public string WeChatOpenId { get; set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; }
        public string PictureUrl { get; set; }
        public int? FileID { get; set; }
        public string RFIDCode { get; set; }
    }
}
