using wms.Client.Model.Query;

namespace wms.Client.Model.RequestModel
{
    /// <summary>
    /// 字典查询
    /// </summary>
    public class DictionariesRequest : BaseRequest
    {
        public override string route { get { return ServerIP + "api/Dictionary/GetDictionaries"; } }

        public DictionariesParameters parameters { get; set; }
    }
}
