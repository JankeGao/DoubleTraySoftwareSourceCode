using Bussiness.Entitys;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Services
{
    public class MaterialPropertyServer : Contracts.IMaterialPropertyContract
    {
        public IRepository<MaterialProperty, int> MaterialPropertyRepository { get; set; }

        public IQuery<MaterialProperty> MaterialPropertys {
            get
            {
                return MaterialPropertyRepository.Query();
            }
        }

        public DataResult CreateMaterialProperty(MaterialProperty entity)
        {
            if (MaterialPropertys.Any(a=>a.Name == entity.Name))
            {
                return DataProcess.Failure(string.Format("物料属性组名称{0}已存在", entity.Name));
            }
            if (MaterialPropertyRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("物料属性组名称{0}创建成功", entity.Name));
            }
            return DataProcess.Failure();
        }

        public DataResult DeleteMaterialProperty(int id)
        {
            if (MaterialPropertyRepository.Delete(id)>0)
            {
                return DataProcess.Success("删除成功");
            }
            return DataProcess.Failure();
        }

        public DataResult EditMaterialProperty(MaterialProperty entity)
        {
            if (MaterialPropertyRepository.Update(entity)>0)
            {
                return DataProcess.Success(string.Format("物料属性组{0}编辑成功", entity.Name));
            }
            return DataProcess.Failure();
        }
    }
}
