using System;

namespace wms.Client.Model.Entity
{
    [Serializable]
    public class DictionaryType : BaseEntity
    {
        public string TypeCode { get; set; }

        public string TypeName { get; set; }
    }
}
