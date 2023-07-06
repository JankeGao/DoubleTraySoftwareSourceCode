using System;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Mapping;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;

namespace Bussiness.Services
{
    public class MaterialServer : Contracts.IMaterialContract
    {
        /// <summary>
        /// 物料仓储
        /// </summary>
        public IRepository<Material, int> MaterialRepository { get; set; }
        /// <summary>
        /// 库位仓储
        /// </summary>
        public IRepository<Entitys.Location, int> LocationRepository { get; set; }
        /// <summary>
        /// 载具箱仓储
        /// </summary>
        public IRepository<Box, int> BoxRepository { get; set; }
        /// <summary>
        /// 库存仓储
        /// </summary>
        public IRepository<Entitys.Stock, int> StockRepository { get; set; }
        /// <summary>
        /// 物料载具映射表仓储
        /// </summary>
        public IRepository<MaterialBoxMap, int> MaterialBoxMapRepository { get; set; }

        /// <summary>
        /// 编码规则契约
        /// </summary>
        public ISequenceContract SequenceContract { set; get; }
        /// <summary>
        /// 映射
        /// </summary>
        public IMapper Mapper { set; get; }


        public IQuery<Material> Materials {
            get
            {
                return MaterialRepository.Query();
            }
        }

        public IQuery<MaterialBoxMap> MaterialBoxMaps
        {
            get
            {
                return MaterialBoxMapRepository.Query();
            }
        }
        /// <summary>
        /// 物料数据DTO
        /// </summary>
        public IQuery<MaterialDto> MaterialDtos => Materials.LeftJoin(MaterialBoxMapRepository.Query(), ( material,map) => material.Code == map.MaterialCode)
            .LeftJoin(BoxRepository.Query(), (material, map, box) => map.BoxCode == box.Code)
            .Select((material, map, box) => new MaterialDto
            {
                Id = material.Id,
                FileID = material.FileID,
                BoxCount = map.BoxCount,
                BoxCode=box.Code,
                BoxName=box.Name,
                BoxPictureUrl =box.PictureUrl,
                BoxLength=box.BoxLength,
                BoxWidth=box.BoxWidth,
                Name =material.Name,
                AgeingPeriod=material.AgeingPeriod,
                PackageQuantity= material.PackageQuantity,
                Unit =material.Unit,
                ShortName=material.ShortName,
                MaterialType=material.MaterialType,
                IsNeedBlock=material.IsNeedBlock,
                IsMaxBatch=material.IsMaxBatch,
                IsPackage=material.IsPackage,
                IsBatch =material.IsBatch,
                MaxNum=material.MaxNum,
                MinNum=material.MinNum,
                FIFOType=material.FIFOType,
                Code=material.Code,
                PictureUrl=material.PictureUrl,
                IsDeleted=material.IsDeleted,
                Remark1=material.Remark1,
                Remark2=material.Remark2,
                Remark3=material.Remark3,
                Remark4=material.Remark4,
                Remark5=material.Remark5,
                UnitWeight=material.UnitWeight,
                ValidityPeriod=material.ValidityPeriod,
                CostCenter=material.CostCenter,
                CreatedUserCode = material.CreatedUserCode, 
                CreatedUserName = material.CreatedUserName, 
                CreatedTime = material.CreatedTime, 
                UpdatedUserCode = material.UpdatedUserCode, 
                UpdatedUserName = material.UpdatedUserName, 
                UpdatedTime = material.UpdatedTime,
            });

        /// <summary>
        /// 创建物料
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult CreateMaterial(MaterialDto entityDto)
        {
            MaterialRepository.UnitOfWork.TransactionEnabled = true;
            // 物料实体映射
            Material entity = Mapper.MapTo<Material>(entityDto);
            if (Materials.Any(a=>a.Code==entity.Code))
            {
                return DataProcess.Failure(string.Format("物料编码{0}已存在", entity.Code));
            }

            // 判断是否有物料编码，没有则生成
            if (String.IsNullOrEmpty(entity.Code))
            {
                entity.Code = SequenceContract.Create(entity.GetType());
            }
            // 判断绑定载具编码
            if (!string.IsNullOrEmpty(entityDto.BoxCode))
            {
                var boxCode = entityDto.BoxCode;

                // 查找是否存在此类别的载具箱
                if (BoxRepository.Query().Any(a => a.Code == boxCode))
                {
                    //var boxMaterailMap = new MaterialBoxMap()
                    //{
                    //    MaterialCode = entity.Code,
                    //    BoxCount= entityDto.BoxCount,
                    //    BoxCode = boxCode
                    //};
                    //if (!MaterialBoxMapRepository.Insert(boxMaterailMap))
                    //{
                    //    return DataProcess.Failure(string.Format("物料载具箱{0}映射失败", boxCode));
                    //}
                    // 查找是否存在此类别的载具箱与物料的关联关系
                    if (MaterialBoxMapRepository.Query().Any(a => a.MaterialCode == entity.Code))
                    {
                        var boxEntity = MaterialBoxMapRepository.GetEntity(a => a.MaterialCode == entity.Code);
                        boxEntity.BoxCode = boxCode;
                        boxEntity.BoxCount = entityDto.BoxCount;
                        if (MaterialBoxMapRepository.Update(boxEntity) < 0)
                        {
                            return DataProcess.Failure(string.Format("物料载具箱{0}映射失败", boxCode));
                        }
                    }
                    else
                    {
                        var boxMaterailMap = new MaterialBoxMap()
                        {
                            MaterialCode = entity.Code,
                            BoxCount = entityDto.BoxCount,
                            BoxCode = boxCode
                        };
                        if (!MaterialBoxMapRepository.Insert(boxMaterailMap))
                        {
                            return DataProcess.Failure(string.Format("物料载具箱{0}映射失败", boxCode));
                        }
                    }
                }
                else
                {
                    return DataProcess.Failure(string.Format("载具箱{0}不存在", boxCode));
                }
            }
            if (MaterialRepository.Insert(entity))
            {
                MaterialRepository.UnitOfWork.Commit();
                return DataProcess.Success(string.Format("物料编码{0}创建成功", entity.Code));
            }

            return DataProcess.Failure();
        }

        /// <summary>
        /// 删除物料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult DeleteMaterial(int id)
        {
            MaterialRepository.UnitOfWork.TransactionEnabled = true;

            var entity = MaterialRepository.GetEntity(id);
            if (StockRepository.Query().Any(a => a.MaterialCode == entity.Code))
            {
                return DataProcess.Failure(string.Format("仓库中物料{0}尚有库存,无法删除", entity.Code));
            }

            var locationList = LocationRepository.Query().Where(a => a.SuggestMaterialCode == entity.Code).ToList();
            if (locationList != null && locationList.Count > 0)
            {
                foreach (var item in locationList)
                {
                    item.SuggestMaterialCode = "";
                    LocationRepository.Update(item);
                }
            }
            if (MaterialRepository.Delete(id)<0)
            {
                return DataProcess.Failure();
            }
            var boxMaterial = MaterialBoxMapRepository.GetEntity(a=>a.MaterialCode== entity.Code);

            if (boxMaterial!=null)
            {
                if (MaterialBoxMapRepository.Delete(boxMaterial.Id) < 0)
                {
                    return DataProcess.Failure(string.Format("物料载具箱{0}映射失败", boxMaterial.BoxCode));
                }
            }
            MaterialRepository.UnitOfWork.Commit();
            return DataProcess.Success("删除成功");
        }

        /// <summary>
        /// 编辑物料
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult EditMaterial(MaterialDto entityDto)
        {
            MaterialRepository.UnitOfWork.TransactionEnabled = true;
            // 物料实体映射
            Material entity = Mapper.MapTo<Material>(entityDto);
            // 判断绑定载具编码
            if (!string.IsNullOrEmpty(entityDto.BoxCode))
            {
                var boxCode = entityDto.BoxCode;

                // 查找是否存在此类别的载具箱与物料的关联关系
                if (MaterialBoxMapRepository.Query().Any(a => a.MaterialCode== entityDto.Code))
                {
                    var boxEntity = MaterialBoxMapRepository.GetEntity(a => a.MaterialCode == entityDto.Code);
                    boxEntity.BoxCode = boxCode;
                    boxEntity.BoxCount = entityDto.BoxCount;
                    if (MaterialBoxMapRepository.Update(boxEntity) < 0)
                    {
                        return DataProcess.Failure(string.Format("物料载具箱{0}映射失败", boxCode));
                    }
                }
                else
                {
                    var boxMaterailMap = new MaterialBoxMap()
                    {
                        MaterialCode = entity.Code,
                        BoxCount = entityDto.BoxCount,
                        BoxCode = boxCode
                    };
                    if (!MaterialBoxMapRepository.Insert(boxMaterailMap))
                    {
                        return DataProcess.Failure(string.Format("物料载具箱{0}映射失败", boxCode));
                    }
                }
            }
            if (MaterialRepository.Update(entity)>0)
            {
                MaterialRepository.UnitOfWork.Commit();
                return DataProcess.Success(string.Format("物料{0}编辑成功", entity.Code, entity.FileID));
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 导入维护物料
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditMateraiInfo(MaterialDto entity)
        {
            try
            {
                entity.CheckNotNull("entity");

                var oriEntity = Materials.FirstOrDefault(a => a.Code == entity.Code);
                oriEntity.CheckNotNull("oriEntity");

                if (MaterialRepository.Update(a => new Material()
                {
                    Name = entity.Name,
                    Unit = entity.Unit,
                    MinNum = entity.MinNum,
                    MaxNum = entity.MaxNum,
                    ValidityPeriod = entity.ValidityPeriod,
                    UnitWeight=entity.UnitWeight,
                    MaterialType= entity.MaterialType,
                    PackageQuantity=entity.PackageQuantity,
                    IsPackage=entity.IsPackage,
                    IsBatch=entity.IsBatch,
                    IsMaxBatch= entity.IsMaxBatch,
                    IsNeedBlock=entity.IsNeedBlock,
                    CostCenter=entity.CostCenter,
                    Remark1 = entity.Remark1,
                    Remark2 = entity.Remark2,
                    Remark3 = entity.Remark3,
                    Remark4 = entity.Remark4,
                    Remark5 = entity.Remark5,
                }, a => a.Code == entity.Code) == 0)
                {
                    return DataProcess.Failure("物料({0})维护失败！".FormatWith(oriEntity.Code));
                }
                if (!string.IsNullOrEmpty(entity.BoxCode))
                {
                    var boxCode = entity.BoxCode;
                    if (BoxRepository.Query().Any(a => a.Code == boxCode))
                    {
                        // 查找是否存在此类别的载具箱与物料的关联关系
                        if (MaterialBoxMapRepository.Query().Any(a => a.MaterialCode == entity.Code))
                        {
                            var boxEntity = MaterialBoxMapRepository.GetEntity(a => a.MaterialCode == entity.Code);
                            boxEntity.BoxCode = boxCode;
                            boxEntity.BoxCount = entity.BoxCount;
                            if (MaterialBoxMapRepository.Update(boxEntity) < 0)
                            {
                                return DataProcess.Failure(string.Format("物料载具箱{0}映射失败", boxCode));
                            }
                        }
                        else
                        {
                            var boxMaterailMap = new MaterialBoxMap()
                            {
                                MaterialCode = entity.Code,
                                BoxCount = entity.BoxCount,
                                BoxCode = boxCode
                            };
                            if (!MaterialBoxMapRepository.Insert(boxMaterailMap))
                            {
                                return DataProcess.Failure(string.Format("物料载具箱{0}映射失败", boxCode));
                            }
                        }
                    }
                    else
                    {
                        return DataProcess.Failure(string.Format("载具箱{0}不存在", boxCode));
                    }
           
                }
                return DataProcess.Success("物料({0})维护成功！".FormatWith(oriEntity.Code));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
