using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Services
{
    public class LabelServer : Contracts.ILabelContract
    {

        public IRepository<Label, int> LabelRepository { get; set; }
        public ISequenceContract SequenceContract { set; get; }
        public IMaterialContract MaterialContract { set; get; }
        public ISupplyContract SupplyContract { set; get; }

        public IQuery<Label> Labels {
            get
            {
                return LabelRepository.Query();
            }
        }
        public IQuery<LabelDto> LabelDtos
        {
            get
            {
                return Labels.InnerJoin(MaterialContract.Materials, (labels, materials) => labels.MaterialCode == materials.Code)
                    .LeftJoin(SupplyContract.Supplys, (labels, materials, suppliers) => labels.SupplierCode == suppliers.Code)
                    .Select((labels, materials, suppliers)
                    => new Dtos.LabelDto()
                {
                    Id = labels.Id,
                    Code=labels.Code,
                    MaterialCode = labels.MaterialCode,
                    MaterialName = materials.Name,
                    SupplierCode = labels.SupplierCode,
                    SupplyName=suppliers.Name,
                    BatchCode = labels.BatchCode,
                    ManufactrueDate = labels.ManufactrueDate,
                    CreatedTime = labels.CreatedTime,
                    CreatedUserCode = labels.CreatedUserCode,
                    CreatedUserName = labels.CreatedUserName,
                    UpdatedTime = labels.UpdatedTime,
                    UpdatedUserCode = labels.UpdatedUserCode,
                    UpdatedUserName = labels.UpdatedUserName,
                    Quantity = labels.Quantity,
                    IsElectronicMateria = materials.IsElectronicMateria,
                    MaterialUrl= materials.PictureUrl
                    });
            }
        }
        public DataResult CreateLabel(Label entity)
        {
            if (entity.Code==null)
            {
                entity.Code = SequenceContract.Create("Label");
            }
            if (Labels.Any(a=>a.Code==entity.Code))
            {
                return DataProcess.Failure(string.Format("标签编码{0}已存在", entity.Code));
            }
            if (LabelRepository.Insert(entity))
            {
                var result = LabelDtos.Where(a => a.Code == entity.Code).FirstOrDefault();
                return DataProcess.Success( entity.Code, result);
            }
            return DataProcess.Failure();
        }

        public DataResult DeleteLabel(int id)
        {
            if (LabelRepository.LogicDelete(id)>0)
            {
                return DataProcess.Success("删除成功");
            }
            return DataProcess.Failure();
        }

        public DataResult EditLabel(Label entity)
        {
            if (LabelRepository.Update(entity)>0)
            {
                return DataProcess.Success(string.Format("标签{0}编辑成功", entity.Code));
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 批量创建标签
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult CreateBatchLabel(LabelDto entityDto)
        {
            LabelRepository.UnitOfWork.TransactionEnabled = true;
            var tmpQuantity = entityDto.Quantity;
            for (var i = 0; i < entityDto.LabelCount; i++)
            {
                var labelEntity = new Label()
                {
                    MaterialCode = entityDto.MaterialCode,
                    ManufactrueDate = entityDto.ManufactrueDate,
                    SupplierCode = entityDto.SupplierCode,
                    BatchCode = entityDto.BatchCode,
                };
                if (tmpQuantity >= entityDto.PackageQuantity)
                {
                    labelEntity.Quantity = entityDto.PackageQuantity;
                }
                else
                {
                    labelEntity.Quantity = tmpQuantity;
                }
                labelEntity.Code = SequenceContract.Create(labelEntity.GetType());


                tmpQuantity = tmpQuantity - entityDto.PackageQuantity;

                if (!LabelRepository.Insert(labelEntity))
                {
                    return DataProcess.Failure(string.Format("入库条码{0}创建失败", entityDto.MaterialCode));
                }
            }
            LabelRepository.UnitOfWork.Commit();
            return DataProcess.Success("入库条码创建成功");
        }
    }
}
