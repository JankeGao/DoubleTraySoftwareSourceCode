using System.Collections.Generic;
using Newtonsoft.Json;
using wms.Client.Model.Entity;

namespace wms.Client.Model.ResponseModel
{
    public class GroupResponse : BaseResponse
    {
        public List<Group> groups
        {
            get
            {
                if (dynamicObj == null) return null;
                return JsonConvert.DeserializeObject<List<Group>>(dynamicObj.ToString());
            }
        }
    }
}
