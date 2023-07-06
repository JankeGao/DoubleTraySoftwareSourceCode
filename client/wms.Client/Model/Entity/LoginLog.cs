using System;

namespace wms.Client.Model.Entity
{
    [Serializable]
    public class LoginLog : BaseEntity
    {
        public string Account { get; set; }

        public DateTime CurrentTime { get; set; }
    }
}
