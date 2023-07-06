using System;

namespace wms.Client.Model.Entity
{
    [Serializable]
    public class Authorithitem : BaseEntity
    {
        public string AuthorityName { get; set; }

        public string AuthorityValue { get; set; }
    }
}
