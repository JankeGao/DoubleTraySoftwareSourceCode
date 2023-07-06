using Bussiness.Entitys;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Services
{
    public class CustomerServer : Contracts.ICustomerContract
    {
        /// <summary>
        /// 将CUSTOMER添加到仓储中
        /// </summary>
        public IRepository<Customer, int> CustomerRepository { set; get; }

        /// <summary>
        /// 查询获取 CUSTOMER
        /// </summary>
        public IQuery<Customer> Customers
        {
            get
            {
                return CustomerRepository.Query();
            }
        }

        /// <summary>
        /// 创建客户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateCustomer(Customer entity)
        {
            if (Customers.Any(a=>a.Code==entity.Code))
            {
                return DataProcess.Failure(string.Format("客户编码{0}已存在！", entity.Code));
            }
            if (CustomerRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("客户{0}创建成功！",entity.Code));
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 根据ID删除客户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult DeleteCustomer(int id)
        {
            if (CustomerRepository.LogicDelete(id)>0)
            {
                return DataProcess.Success("删除成功！");
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 更新客户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditCustomer(Customer entity)
        {
            if (CustomerRepository.Update(entity)>0)
            {
                return DataProcess.Success("客户{0}编辑成功！",entity.Code);
            }
            return DataProcess.Failure();
        }
    }
}