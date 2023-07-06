using System;

namespace wms.Client.Model.Entity
{
    [Serializable]
    public class GroupUser : BaseEntity
    {
        public string GroupCode { get; set; }

        public string Account { get; set; }
    }
}
