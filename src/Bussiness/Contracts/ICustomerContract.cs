using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface ICustomerContract  : IScopeDependency
    {
        IRepository<Entitys.Customer, int> CustomerRepository { get; }

        IQuery<Entitys.Customer> Customers { get; }

        /// <summary>
        /// 创建客户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateCustomer(Entitys.Customer entity);

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditCustomer(Entitys.Customer entity);

        /// <summary>
        /// 根据ID删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult DeleteCustomer(int id);
    }
}