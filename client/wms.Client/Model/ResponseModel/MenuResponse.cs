using System.Collections.Generic;
using Newtonsoft.Json;
using wms.Client.Model.Entity;

namespace wms.Client.Model.ResponseModel
{
    public class MenuResponse : BaseResponse
    {
        public List<Menu> menus
        {
            get
            {
                if (dynamicObj == null) return null;
                return JsonConvert.DeserializeObject<List<Menu>>(dynamicObj.ToString());
            }
        }
    }
}
