using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("仓库平面图")]
    [Table("Plan")]
    public class Plan : ServiceEntityBase<int>
    {
        public string WareHouseCode { set; get; }
        public string content { set; get; }
    }
}