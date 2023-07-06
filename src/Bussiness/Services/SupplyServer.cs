using Bussiness.Entitys;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Services
{
    public class SupplyServer : Contracts.ISupplyContract
    {
        public IRepository<Supply, int> SupplyRepository { get; set; }

        public IQuery<Supply> Supplys {
            get
            {
                return SupplyRepository.Query();
            }
        }

        public DataResult CreateSupply(Supply entity)
        {
            if (Supplys.Any(a=>a.Code==entity.Code))
            {
                return DataProcess.Failure(string.Format("供应商编码{0}已存在", entity.Code));
            }
            if (SupplyRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("供应商编码{0}创建成功", entity.Code));
            }
            return DataProcess.Failure();
        }

        public DataResult DeleteSupply(int id)
        {
            if (SupplyRepository.LogicDelete(id)>0)
            {
                return DataProcess.Success("删除成功");
            }
            return DataProcess.Failure();
        }

        public DataResult EditSupply(Supply entity)
        {
            // 根据id获取供应商
            var supply = SupplyRepository.GetEntity(entity.Id);
            if (supply == null)
            {
                return DataProcess.Failure($"供应商：{entity.Code}不存在！");
            }
            if (string.IsNullOrEmpty(entity.Code) || string.IsNullOrEmpty(entity.Name))
            {
                return DataProcess.Failure("请检查数据，供应商编码、名称不能为空！");
            }
            if (!supply.Code.Equals(entity.Code))
            {
                return DataProcess.Failure("供应商编码不可修改");
            }
            // 判断是否修改编码，且修改后的编码已在数据库中存在
            //if (!supply.Code.Equals(entity.Code) && Supplys.Any(a => a.Id != supply.Id && a.Code == entity.Code))
            //{
            //    return DataProcess.Failure($"供应商编码{entity.Code}已存在，请勿重复设置！");
            //}
            // 更新供应商信息
            if (SupplyRepository.Update(entity) > 0)
            {
                return DataProcess.Success();
            }
            return DataProcess.Failure();
        }
    }
}
