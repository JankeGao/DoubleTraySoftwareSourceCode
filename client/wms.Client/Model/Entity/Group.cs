using System;

namespace wms.Client.Model.Entity
{
    [Serializable]
    public class Group : BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

    }
}
