using System.EnterpriseServices;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    [Description("客户管理")]//描述标签
    [Table("TB_WMS_CUSTOMER")]//ORM数据库与实体类映射标签
    public class Customer : ServiceEntityBase<int>
    {
        ///<summary>
        ///客户编码
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 客户联系人
        /// </summary>
        public string Linkman { set; get; }

        /// <summary>
        /// 客户电话
        /// </summary>
        public string Phone { set; get; }

        /// <summary>
        /// 客户住址
        /// </summary>
        public string Address { set; get; }

        /// <summary>
        /// 客户描述备注
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
        /// 客户删除标志
        /// </summary>
        public bool IsDeleted { set; get; }
    }
}