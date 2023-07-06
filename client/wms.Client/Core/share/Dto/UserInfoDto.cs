using System.Collections.Generic;
using wms.Client.Core.share.DataModel;

namespace wms.Client.Core.share.Dto
{
    public class UserInfoDto
    {
        public User User { get; set; }

        public List<Menu> Menus { get; set; }
    }
}
