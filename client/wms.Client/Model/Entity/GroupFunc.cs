using System;
using System.ComponentModel;

namespace wms.Client.Model.Entity
{
    [Serializable]
    public class GroupFunc : BaseEntity
    {
        public string GroupCode { get; set; }

        public string MenuCode { get; set; }

        [DefaultValue(0)]
        public int Authorities { get; set; }
    }
}
