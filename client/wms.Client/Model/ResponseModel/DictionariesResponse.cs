using System.Collections.Generic;
using Newtonsoft.Json;
using wms.Client.Model.Entity;

namespace wms.Client.Model.ResponseModel
{
    public class DictionariesResponse : BaseResponse
    {
        public List<Dictionaries> Dictionaries
        {
            get
            {
                if (dynamicObj == null) return null;
                return JsonConvert.DeserializeObject<List<Dictionaries>>(dynamicObj.ToString());
            }
        }
    }
}
