using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Core.Mapping;
using HP.Core.Security;
using HP.Core.Security.Permissions;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bussiness.Services
{
    public class OutTaskServer : Contracts.IOutTaskContract
    {
        public IRepository<Material, int> MaterialRepository { get; set; }
        public IRepository<OutMaterial, int> OutMaterialRepository { get; set; }
        public IRepository<Entitys.Tray, int> TrayRepository { get; set; }
        public IRepository<OutTask, int> OutTaskRepository { get; set; }

        public IRepository<StockLockMap, int> StockLockMapRepository { get; set; }

        public IRepository<OutTaskMaterial, int> OutTaskMaterialRepository { get; set; }

        public IRepository<OutMaterialLabel, int> OutMaterialLabelRepository { get; set; }

        public IRepository<Entitys.Stock, int> StockRepository { get; set; }

        public IRepository<CheckMain, int> CheckRepository { get; set; }
        public IRepository<Entitys.Location, int> LocationRepository { get; set; }

        public IRepository<HPC.BaseService.Models.Dictionary,int> DictionaryRepository { get; set; }

        public IRepository<Out, int> OutRepository { get; set; }
        public IQuery<Out> Outs => OutRepository.Query();

        public IQuery<OutMaterial> OutMaterials => OutMaterialRepository.Query();
        public ISequenceContract SequenceContract { set; get; }

        /// <summary>
        /// 出库单契约
        /// </summary>
        public IOutContract OutContract { set; get; }

        public IStockContract StockContract { get; set; }
        /// <summary>
        /// 标签契约
        /// </summary>
        public ILabelContract LabelContract { set; get; }

        public ISupplyContract SupplyContract { set; get; }

        public ICheckContract CheckContract { get; set; }

        public IMaterialContract MaterialContract { get; set; }

        public IEquipmentTypeContract EquipmentTypeContract { set; get; }

        public IMapper Mapper { set; get; }

        public IQuery<OutTask> OutTasks => OutTaskRepository.Query();
        public IQuery<OutTaskMaterial> OutTaskMaterials => OutTaskMaterialRepository.Query();
        public IRepository<WareHouse, int> WareHouseRepository { get; set; }
        public IRepository<OutTaskMaterialLabel, int> OutTaskMaterialLabelRepository { get; set; }
        public IQuery<OutTaskMaterialLabel> OutTaskMaterialLabels => OutTaskMaterialLabelRepository.Query();
        public IWareHouseContract WareHouseContract { get; set; }
        public IRepository<Container, int> ContainerRepository { get; set; }
        //public Bussiness.Contracts.IOutContract OutContract { set; get; }
        public IQuery<OutTaskDto> OutTaskDtos
        {
            get
            {
                return OutTasks
                    .LeftJoin(DictionaryRepository.Query(),(outentity, dictionary) => outentity.OutDict == dictionary.Code)
                    .LeftJoin(WareHouseContract.WareHouses,
                        (outentity, dictionary, warehouse) => outentity.WareHouseCode == warehouse.Code)
                    .InnerJoin(WareHouseContract.Containers, (outentity, dictionary, warehouse, containers) => outentity.ContainerCode == containers.Code)
                    .InnerJoin(EquipmentTypeContract.EquipmentType, (outentity, dictionary, warehouse, containers, equipmentType) => containers.EquipmentCode == equipmentType.Code)
                    .Select((outentity, dictionary, warehouse, containers, equipmentType) => new Dtos.OutTaskDto()
                    {
                        Id = outentity.Id,
                        Code = outentity.Code,
                        OutCode = outentity.OutCode,
                        WareHouseCode = outentity.WareHouseCode,
                        WareHouseName = warehouse.Name,
                        ContainerCode = outentity.ContainerCode,
                        BillCode= outentity.BillCode,
                        OutDict = outentity.OutDict,
                        Status = outentity.Status,
                        Remark = outentity.Remark,
                        IsDeleted = outentity.IsDeleted,
                        ShelfStartTime = outentity.ShelfStartTime,
                        ShelfEndTime = outentity.ShelfEndTime,
                        CreatedUserCode = outentity.CreatedUserCode,
                        CreatedUserName = outentity.CreatedUserName,
                        CreatedTime = outentity.CreatedTime,
                        UpdatedUserCode = outentity.UpdatedUserCode,
                        UpdatedUserName = outentity.UpdatedUserName,
                        UpdatedTime = outentity.UpdatedTime,
                        OutDictDescription = dictionary.Name,
                        PictureUrl = equipmentType.PictureUrl,
                        OutDate = outentity.OutDate,
                        //CRRCID = outentity.CRRCID,
                    });
            }
        }

        public IQuery<OutTaskMaterialDto> OutTaskMaterialDtos => OutTaskMaterials
                  .InnerJoin(MaterialRepository.Query(), (inMaterial, material) => inMaterial.MaterialCode == material.Code)
                  .InnerJoin(OutTasks, (outTaskMaterial, material, inTasks) => outTaskMaterial.OutTaskCode == inTasks.Code)
                  .InnerJoin(WareHouseContract.LocationVMs, (outTaskMaterial, material, inTasks, location) => outTaskMaterial.SuggestLocation == location.Code)
                  .LeftJoin(SupplyContract.Supplys, (outTaskMaterial, material, inTasks, location, supply) => outTaskMaterial.SupplierCode == supply.Code)
                  .Select((outTaskMaterial, material, inTasks, location, supply) => new Dtos.OutTaskMaterialDto
                  {
                      Id = outTaskMaterial.Id,
                      OutCode = outTaskMaterial.OutCode,
                      MaterialCode = outTaskMaterial.MaterialCode,
                      SuggestContainerCode = outTaskMaterial.SuggestContainerCode,
                      SuggestTrayId = outTaskMaterial.SuggestTrayId,
                      SuggestTrayCode = location.TrayCode,
                      OutTaskCode = outTaskMaterial.OutTaskCode,
                      MaterialType = material.MaterialType,
                      Quantity = outTaskMaterial.Quantity,
                      UnitWeight = material.UnitWeight,
                      ManufactrueDate = outTaskMaterial.ManufactrueDate,
                      OutDict = outTaskMaterial.OutDict,
                      BatchCode = outTaskMaterial.BatchCode,
                      SupplierCode = outTaskMaterial.SupplierCode,
                      IsPackage = material.IsPackage,
                      IsMaxBatch = material.IsMaxBatch,
                      IsBatch = material.IsBatch,
                      SupplierName = supply.Name,
                      Status = outTaskMaterial.Status,
                      IsDeleted = outTaskMaterial.IsDeleted,
                      BillCode = outTaskMaterial.BillCode,
                      CustomCode = outTaskMaterial.CustomCode,
                      CustomName = outTaskMaterial.CustomName,
                      MaterialLabel = outTaskMaterial.MaterialLabel,
                      SuggestLocation = outTaskMaterial.SuggestLocation,
                      LocationCode = outTaskMaterial.LocationCode,
                      CreatedUserCode = outTaskMaterial.CreatedUserCode,
                      CreatedUserName = outTaskMaterial.CreatedUserName,
                      CreatedTime = outTaskMaterial.CreatedTime,
                      UpdatedUserCode = outTaskMaterial.UpdatedUserCode,
                      UpdatedUserName = outTaskMaterial.UpdatedUserName,
                      UpdatedTime = outTaskMaterial.UpdatedTime,
                      MaterialName = material.Name,
                      MaterialUrl = material.PictureUrl,
                      MaterialUnit = material.Unit,
                      ItemNo = outTaskMaterial.ItemNo,
                      WareHouseCode = inTasks.WareHouseCode,
                      RealPickedQuantity = outTaskMaterial.RealPickedQuantity,
                      XLight = location.XLight,
                      YLight = location.YLight,
                      BoxName = location.BoxName,
                      BoxUrl = location.BoxUrl,
                      ContainerType = location.ContainerType,
                      BracketNumber = location.BracketNumber,
                      BracketTrayNumber = location.BracketTrayNumber,
                  });


        public IQuery<OutTaskMaterialLabelDto> OutTaskMaterialLabelDtos => OutTaskMaterialLabels
            .InnerJoin(MaterialRepository.Query(),
                (outMaterialLabel, material) => outMaterialLabel.MaterialCode == material.Code)
            .InnerJoin(WareHouseContract.LocationVMs,
                (outMaterialLabel, material, location) => outMaterialLabel.LocationCode == location.Code)
            .InnerJoin(WareHouseContract.Trays,
                (outMaterialLabel, material, location, tray) => outMaterialLabel.TrayId == tray.Id)
            .LeftJoin(SupplyContract.Supplys,
                (outMaterialLabel, material, location, tray, supplier) =>
                    outMaterialLabel.SupplierCode == supplier.Code)
            .Select((outMaterialLabel, material, location, tray, supplier) => new OutTaskMaterialLabelDto
            {
                Id = outMaterialLabel.Id,
                OutCode = outMaterialLabel.OutCode,
                MaterialCode = outMaterialLabel.MaterialCode,
                TrayId = outMaterialLabel.TrayId,
                ContainerCode = outMaterialLabel.ContainerCode,
                TaskCode = outMaterialLabel.TaskCode,
                Quantity = outMaterialLabel.Quantity,
                BatchCode = outMaterialLabel.BatchCode,
                Status = outMaterialLabel.Status,
                IsDeleted = outMaterialLabel.IsDeleted,
                BillCode = outMaterialLabel.BillCode,
                LocationCode = outMaterialLabel.LocationCode,
                OutMaterialId = outMaterialLabel.OutMaterialId,
                MaterialLabel = outMaterialLabel.MaterialLabel,
                OriginalQuantity= outMaterialLabel.OriginalQuantity,
                SupplierCode= outMaterialLabel.SupplierCode,
                AreaCode = outMaterialLabel.AreaCode,
                WareHouseCode = outMaterialLabel.WareHouseCode,
                TrayCode = tray.Code,
                MaterialName = material.Name,
                MaterialUnit = material.Unit,
                PickedTime = outMaterialLabel.PickedTime,
                RealPickedQuantity = outMaterialLabel.RealPickedQuantity,
                Operator = outMaterialLabel.Operator,
                CheckedTime = outMaterialLabel.CheckedTime,
                Checker = outMaterialLabel.Checker,
                CreatedTime = outMaterialLabel.CreatedTime,
                CreatedUserCode = outMaterialLabel.CreatedUserCode,
                CreatedUserName = outMaterialLabel.CreatedUserName,
                CheckedQuantity = outMaterialLabel.CheckedQuantity,
                SupplierName = supplier.Name,
                XLight= location.XLight,
                YLight= location.YLight,
                BoxName = location.BoxName,
                BoxUrl = location.BoxUrl,
                MaterialUrl=material.PictureUrl,

            });



        /// <summary>
        /// 生成出库任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateOutTaskEntity(Out entity)
        {
            try
            {
                if (CheckRepository.Query().Any(a => a.WareHouseCode == entity.WareHouseCode && a.Status != (int)Bussiness.Enums.CheckStatusCaption.Finished && a.Status != (int)Bussiness.Enums.CheckStatusCaption.Cancel))
                {
                    return DataProcess.Failure("该仓库尚有盘点单未完成");
                }
                //查找可用库存是否满足-出库单明细表
                List<OutMaterialDto> list = OutContract.OutMaterialDtos.Where(a => a.OutCode == entity.Code).ToList();

                // 按照物料编码分组
                IEnumerable<IGrouping<string, OutMaterialDto>> group = list.GroupBy(a => a.MaterialCode);

                // 出库任务明细表
                List<OutTaskMaterialLabel> labelList = new List<OutTaskMaterialLabel>();

                OutContract.OutRepository.UnitOfWork.TransactionEnabled = true;

                foreach (IGrouping<string, OutMaterialDto> item in group)
                {
                    // 该物料下发数量
                    decimal sendQuantity = 0;

                    // 获取物料实体
                    var materialEntity = MaterialContract.MaterialDtos.FirstOrDefault(a => a.Code == item.Key);

                    var query = StockRepository.Query()
                        .Where(a => a.MaterialCode == item.Key &&
                                    (a.IsCheckLocked == false || a.IsCheckLocked == null) &&
                                    a.WareHouseCode == entity.WareHouseCode);
                    // 是否启用老化时间
                    if (materialEntity.AgeingPeriod > 0)
                    {
                        //此刻的时间应大于入库时间+老化时间
                        var valtime = DateTime.Now.AddDays(-materialEntity.AgeingPeriod);
                        query = query.Where(a => valtime > a.ShelfTime);
                    }


                    // 先进先出策略选择
                    switch (materialEntity.FIFOType)
                    {
                        // 无先进先出，根据创建时间
                        case 0:
                            query.OrderBy(a => a.CreatedTime);
                            break;
                        // 入库时间
                        case 1:
                            if (materialEntity.FIFOAccuracy == 0 || materialEntity.FIFOAccuracy == 1) //无，秒
                            {
                                query.OrderBy(a => a.ShelfTime);
                            }
                            else if (materialEntity.FIFOAccuracy == 2) // 分钟
                            {
                                query.OrderBy(a => ((DateTime)a.ShelfTime).ToString("g"));//2016/5/9 13:09 短日期 短时间
                                query.OrderBy(a => DateTime.Parse(((DateTime)a.ShelfTime).ToString()).ToString("yyyy-MM-dd HH:mm"));//2016/5/9 13:09 短日期 短时间
                            }
                            else if (materialEntity.FIFOAccuracy == 3) // 小时
                            {
                                query.OrderBy(a => DateTime.Parse(((DateTime)a.ShelfTime).ToString()).ToString("yyyy-MM-dd HH"));//2016/5/9 13:09 短日期 短时间
                            }
                            else // 天
                            {
                                query.OrderBy(a => DateTime.Parse(((DateTime)a.ShelfTime).ToString()).ToString("yyyy-MM-dd"));//2016/5/9 13:09 短日期
                            }
                            break;
                        // 生产日期
                        case 2:
                            if (materialEntity.FIFOAccuracy == 0 || materialEntity.FIFOAccuracy == 1) //无，秒
                            {
                                query.OrderBy(a => a.ManufactureDate);
                            }
                            else if (materialEntity.FIFOAccuracy == 2) // 分钟
                            {
                                query.OrderBy(a => ((DateTime)a.ManufactureDate).ToString("g"));//2016/5/9 13:09 短日期 短时间
                                query.OrderBy(a => DateTime.Parse(((DateTime)a.ManufactureDate).ToString()).ToString("yyyy-MM-dd HH:mm"));//2016/5/9 13:09 短日期 短时间
                            }
                            else if (materialEntity.FIFOAccuracy == 3) // 小时
                            {
                                query.OrderBy(a => DateTime.Parse(((DateTime)a.ManufactureDate).ToString()).ToString("yyyy-MM-dd HH"));//2016/5/9 13:09 短日期 短时间
                            }
                            else // 天
                            {
                                query.OrderBy(a => DateTime.Parse(((DateTime)a.ManufactureDate).ToString()).ToString("yyyy-MM-dd"));//2016/5/9 13:09 短日期
                            }
                            break;
                        // 保质期日期——生产日期+保质期-当前时间
                        case 3:
                            if (materialEntity.FIFOAccuracy == 0 || materialEntity.FIFOAccuracy == 1) //无，秒
                            {
                                query.OrderBy(a => (((DateTime)a.ManufactureDate).AddDays(materialEntity.ValidityPeriod) - DateTime.Now));
                            }
                            else if (materialEntity.FIFOAccuracy == 2) // 分钟
                            {
                                query.OrderBy(a => ((DateTime)a.ManufactureDate).ToString("g"));//2016/5/9 13:09 短日期 短时间
                                query.OrderBy(a => DateTime.Parse(((((DateTime)a.ManufactureDate).AddDays(materialEntity.ValidityPeriod) - DateTime.Now)).ToString()).ToString("yyyy-MM-dd HH:mm"));//2016/5/9 13:09 短日期 短时间
                            }
                            else if (materialEntity.FIFOAccuracy == 3) // 小时
                            {
                                query.OrderBy(a => DateTime.Parse(((((DateTime)a.ManufactureDate).AddDays(materialEntity.ValidityPeriod) - DateTime.Now)).ToString()).ToString("yyyy-MM-dd HH"));//2016/5/9 13:09 短日期 短时间
                            }
                            else // 天
                            {
                                query.OrderBy(a => DateTime.Parse(((((DateTime)a.ManufactureDate).AddDays(materialEntity.ValidityPeriod) - DateTime.Now)).ToString()).ToString("yyyy-MM-dd"));//2016/5/9 13:09 短日期
                            }
                            break;
                    }

                    List<Stock> AvailableStock = query.ToList();

                    if (AvailableStock.Sum(a => a.Quantity - a.LockedQuantity) < item.Sum(a => a.Quantity))
                    {
                        return DataProcess.Failure("物料" + item.Key + "库存不足");
                    }

                    // 本次待出库的数量
                    decimal needQuantity = item.Sum(a => a.Quantity);


                    // 分配库存
                    foreach (Stock stock in AvailableStock)
                    {
                        // 库存条码可用数量
                        decimal aviQuantiy = stock.Quantity - stock.LockedQuantity;


                        if (aviQuantiy <= 0)
                        {
                            continue;
                        }

                        // 储位信息
                        var locationEntity =
                            WareHouseContract.LocationRepository.GetEntity(a => a.Code == stock.LocationCode);

                        // 如果库存条码大于总共的数量
                        if (aviQuantiy >= needQuantity)
                        {
                            OutTaskMaterialLabel label = new OutTaskMaterialLabel
                            {
                                BatchCode = stock.BatchCode,
                                BillCode = entity.BillCode,
                                IsDeleted = false,
                                LocationCode = stock.LocationCode,
                                OriginalQuantity = stock.Quantity,
                                MaterialCode = item.Key,
                                MaterialLabel = stock.MaterialLabel,
                                SupplierCode = stock.SupplierCode,
                                OutCode = entity.Code,
                                ContainerCode = stock.ContainerCode,
                                TrayId = stock.TrayId,
                                OutMaterialId = 0,
                                Quantity = needQuantity,
                                Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending,
                                WareHouseCode = stock.WareHouseCode,
                                AreaCode = stock.AreaCode,
                                XLight = locationEntity.XLight,
                                YLight = locationEntity.YLight,

                            };
                            labelList.Add(label);
                            // taskLabelList.Add(label);
                            stock.LockedQuantity = stock.LockedQuantity + needQuantity;
                            stock.IsLocked = true;
                            StockRepository.Update(stock);
                            // 本次分配的数量
                            sendQuantity = sendQuantity + label.Quantity;
                            break;

                        }
                        else
                        {
                            // 如果出库的物料数量小于库存条码数量
                            OutTaskMaterialLabel label = new OutTaskMaterialLabel
                            {
                                BatchCode = stock.BatchCode,
                                BillCode = entity.BillCode,
                                ContainerCode = stock.ContainerCode,
                                OriginalQuantity = stock.Quantity,
                                TrayId = stock.TrayId,
                                IsDeleted = false,
                                LocationCode = stock.LocationCode,
                                MaterialCode = item.Key,
                                OutCode = entity.Code,
                                MaterialLabel = stock.MaterialLabel,
                                OutMaterialId = 0,
                                Quantity = aviQuantiy,
                                Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending,
                                WareHouseCode = stock.WareHouseCode,
                                AreaCode = stock.AreaCode,
                                SupplierCode = stock.SupplierCode,
                                XLight = locationEntity.XLight,
                                YLight = locationEntity.YLight,

                            };
                            needQuantity = needQuantity - aviQuantiy;
                            stock.LockedQuantity = stock.LockedQuantity + aviQuantiy;
                            stock.IsLocked = true;//锁定住 不允许移库。。
                            StockRepository.Update(stock);
                            // 本次分配的数量
                            sendQuantity = sendQuantity + label.Quantity;
                            labelList.Add(label);
                        }
                    }

                    // 出库单明细
                    foreach (OutMaterialDto outMaterial in item)
                    {
                        outMaterial.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
                        // 计算下发数量
                        if (sendQuantity > outMaterial.Quantity)
                        {
                            // 如果分配数量还大于出库单行项目数量
                            outMaterial.SendInQuantity = outMaterial.Quantity;
                            sendQuantity = sendQuantity - outMaterial.Quantity;
                        }
                        else
                        {
                            // 如果分配数量还已不足出单行项目数量
                            outMaterial.SendInQuantity = sendQuantity;
                            sendQuantity = 0;
                        }

                        // 物料实体映射
                        OutMaterial outMaterialEntity = Mapper.MapTo<OutMaterial>(outMaterial);

                        OutContract.OutMaterialRepository.Update(outMaterialEntity);
                    }
                }
                entity.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;

                OutContract.OutRepository.Update(entity);

                if (labelList.Count > 0)
                {

                    // 生成出库任务单
                    var groupList = labelList.GroupBy(a => new { a.WareHouseCode, a.ContainerCode });

                    var outEntity = OutContract.OutDtos.FirstOrDefault(a => a.Code == entity.Code);


                    foreach (var item in groupList)
                    {
                        OutTaskMaterialLabel temp = item.FirstOrDefault();
                        var outTask = new OutTask()
                        {
                            Status = (int)OutTaskStatusCaption.WaitingForPicking,
                            WareHouseCode = temp.WareHouseCode,
                            ContainerCode = temp.ContainerCode,
                            IsDeleted=false,
                            OutCode = entity.Code,
                            OutDict = outEntity.OutDict,
                            //CRRCID = entity.CRRCID,
                        };
                        outTask.Code = SequenceContract.Create(outTask.GetType());


                        var materialList = item.GroupBy(a => new
                            {a.WareHouseCode,a.TrayId, a.ContainerCode, a.LocationCode, a.MaterialCode, a.BatchCode}).ToList();

                        foreach (var OutMaterial in materialList)
                        {
                            decimal pickQuantity = OutMaterial.Sum(a => a.Quantity);
                            var OutMaterialEntity = new OutTaskMaterial()
                            {
                                Status = (int)OutTaskStatusCaption.WaitingForPicking,
                                WareHouseCode = temp.WareHouseCode,
                                ContainerCode = temp.ContainerCode,
                                OutCode = entity.Code,
                                OutDict = outEntity.OutDict,
                                Quantity= pickQuantity,
                                BatchCode = OutMaterial.Key.BatchCode,
                                SuggestLocation = OutMaterial.Key.LocationCode,
                                SuggestContainerCode= OutMaterial.Key.ContainerCode,
                                SuggestTrayId= OutMaterial.Key.TrayId,
                                RealPickedQuantity=0,
                                MaterialCode= OutMaterial.Key.MaterialCode
                            };
                            OutMaterialEntity.OutTaskCode = outTask.Code;
                             
                            if (!OutTaskMaterialRepository.Insert(OutMaterialEntity))
                            {
                                return DataProcess.Failure(string.Format("出库库任务明细{0}新增失败", entity.Code));
                            }
                        }

                        foreach (OutTaskMaterialLabel OutMaterialLabel in item)
                        {
                            OutMaterialLabel.TaskCode = outTask.Code;
                            if (!OutTaskMaterialLabelRepository.Insert(OutMaterialLabel))
                            {
                                return DataProcess.Failure(string.Format("出库库任务明细{0}新增失败", entity.Code));
                            }
                        }

                        if (!OutTaskRepository.Insert(outTask))
                        {
                            return DataProcess.Failure(string.Format("入库任务单{0}下发失败", entity.Code));
                        }
                    }

                    // 更新出库物料单
                    foreach (var item in list)
                    {
                        if (item.SendInQuantity >= item.Quantity)
                        {
                            item.Status = (int)OutStatusCaption.SendedPickOrder;
                        }
                        else if (item.SendInQuantity > 0)
                        {
                            item.Status = (int)OutStatusCaption.PartSending;
                        }
                    }
                    entity.Status = (int)OutStatusCaption.PartSending;

                    if (OutContract.OutMaterials.Any(a => a.OutCode == entity.Code && a.Status != (int)OutStatusCaption.SendedPickOrder))
                    {
                        entity.Status = (int)OutStatusCaption.SendedPickOrder;
                    }
                    if (OutContract.OutRepository.Update(entity) < 0)
                    {
                        return DataProcess.Failure("任务下发，出库单更新失败");
                    }

                    foreach (var item in list)
                    {
                        // 物料实体映射
                        OutMaterial outMaterialEntity = Mapper.MapTo<OutMaterial>(item);
                        if (OutContract.OutMaterialRepository.Update(outMaterialEntity) < 0)
                        {
                            return DataProcess.Failure("任务下发，出库物料明细更新失败");
                        }
                    }

                }
                OutContract.OutRepository.UnitOfWork.Commit();

                return DataProcess.Success("拣货任务生成成功");
            }
            catch (Exception ex)
            {

                return DataProcess.Failure("拣货任务生成失败:" + ex.Message);
            }
        }

        

        /// <summary>
        /// 删除出库任务单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveOutTask(int id)
        {
            OutTask entity = OutTaskRepository.GetEntity(id);
            if (entity.Status != (int)Enums.OutTaskStatusCaption.WaitingForPicking)
            {
                return DataProcess.Failure("该出库任务单执行中或已完成");
            }

            OutTaskRepository.UnitOfWork.TransactionEnabled = true;

            // 更新单据状态
            // 获取入库任务明细单
            List<OutTaskMaterialLabelDto> list = OutTaskMaterialLabelDtos.Where(a => a.TaskCode == entity.Code).ToList();

            // 获取入库单
            var outEntity = OutContract.OutRepository.GetEntity(a => a.Code == entity.OutCode);

            bool inFinishFlag = false;
            foreach (var item in list)
            {
                //// 行项目实体
                //var outMaterialList = OutContract.OutMaterials.Where(a => a.OutCode == item.OutCode).ToList();

                //foreach (var outMaterial in outMaterialList)
                //{
                //    if (outMaterial.SendInQuantity> item.Quantity)
                //    {
                //        outMaterial.SendInQuantity = outMaterial.SendInQuantity - item.Quantity;
                //    }
                //}

                //inMaterialEntity.SendInQuantity = inMaterialEntity.SendInQuantity - item.Quantity;

                ////如果未存在下发情况
                //if (inMaterialEntity.SendInQuantity == 0)
                //{
                //    inFinishFlag = true;
                //    //      inMaterialEntity.Status = (int)OutStatusCaption.SendInQuantity;
                //}
                //else
                //{
                //    inFinishFlag = false;
                //}
                //// 更新入库单行项目
                //if (OutContract.OutMaterialRepository.Update(inMaterialEntity) < 0)
                //{
                //    return DataProcess.Failure("任务删除，入库单行项目更新失败");
                //}

                //// 解除托盘的重量锁定
                ////  托盘实体
                //var trayEntity = WareHouseContract.TrayWeightMapRepository.GetEntity(a => a.TrayId == item.TrayId);
                //trayEntity.TempLockWeight = trayEntity.TempLockWeight - item.Quantity * item.UnitWeight;

                //// 更新托盘储位推荐锁定的重量
                //if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
                //{
                //    return DataProcess.Failure(string.Format("托盘重量锁定失败"));
                //}
            }


            if (inFinishFlag)
            {
                //if (!OutContract.OutMaterials.Any(a=>a.OutCode==entity.OutCode&&a.Status!= (int)OutStatusCaption.WaitingForShelf))
                //{
                //    inEntity.Status = (int)OutStatusCaption.WaitingForShelf;
                //    if (OutContract.OutRepository.Update(inEntity) < 0)
                //    {
                //        return DataProcess.Failure("任务下发，入库单更新失败");
                //    }
                //}
            }

            // 删除入库任务
            if (OutTaskRepository.Delete(id) <= 0)
            {
                return DataProcess.Failure(string.Format("入库单{0}删除失败", entity.Code));
            }

            if (list != null && list.Count > 0)
            {
                foreach (OutTaskMaterialLabel item in list)
                {
                    DataResult result = RemoveOutTaskMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }

            OutTaskRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }


        public DataResult RemoveOutTaskMaterial(int id)
        {
            OutTaskMaterialLabel entity = OutTaskMaterialLabelRepository.GetEntity(id);
            //if (entity.Status != (int)Enums.OutStatusCaption.WaitingForShelf)
            //{
            //    return DataProcess.Failure("该入库物料条码执行中或已完成");
            //}
            if (OutTaskMaterialLabelRepository.Delete(id) > 0)
            {
                return DataProcess.Success(string.Format("入库条码{0}删除成功", entity.MaterialLabel));
            }
            return DataProcess.Failure("操作失败");
        }

        /// <summary>
        /// 客户端手动出库-核查可使用库存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CheckAvailableStock(OutTaskMaterialLabel entity)
        {
            // 获取物料实体
            var materialEntity = MaterialContract.MaterialDtos.FirstOrDefault(a => a.Code == entity.MaterialCode);

            // 查询该货柜下的可用物料
            var query = StockContract.StockDtos
                .Where(a => a.MaterialCode == entity.MaterialCode &&
                            (a.IsCheckLocked == false || a.IsCheckLocked == null) && a.ContainerCode== entity.ContainerCode);

            // 是否启用老化时间
            if (materialEntity.AgeingPeriod > 0)
            {
                //ShelfTime+AgeingPeriod>datatime.now
                //此刻的时间应大于入库时间+老化时间
                var valtime = DateTime.Now.AddDays(-materialEntity.AgeingPeriod);
                query = query.Where(a => valtime <a.ShelfTime);
            }

            var outStockList = query.ToList();
            return DataProcess.Success(string.Format("获取可存放库存成功"), outStockList);
        }


        /// <summary>
        /// 手动下架条码 减少库存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult ConfirmHandPicked(OutTaskMaterialDto entityDto)
        {

            // 物料实体映射
            OutTaskMaterialDto entity = OutTaskMaterialDtos.FirstOrDefault(
                a => a.OutTaskCode == entityDto.OutTaskCode
                   && a.MaterialCode == entityDto.MaterialCode
                   && a.SuggestLocation == entityDto.LocationCode
                   && a.BatchCode == entityDto.BatchCode);

            if (entity.Status != (int)Bussiness.Enums.InTaskStatusCaption.WaitingForShelf &&
                entity.Status != (int)Bussiness.Enums.InTaskStatusCaption.InProgress)
            {
                return DataProcess.Failure("该出库任务明细单状态不为待拣货或执行中");
            }

            OutTask inTaskEntity = OutTasks.FirstOrDefault(a => a.Code == entityDto.OutTaskCode);
            if (inTaskEntity.Status != (int)Bussiness.Enums.OutTaskStatusCaption.WaitingForPicking &&
                inTaskEntity.Status != (int)Bussiness.Enums.OutTaskStatusCaption.Picking)
            {
                return DataProcess.Failure("出库任务单状态不为待拣货或者手动执行中");
            }

            // 启用了单包管理，且被分配到了其他出库任务
            if (entity.IsPackage&&OutTaskMaterialLabels.Any(a => a.MaterialLabel == entityDto.MaterialLabel && a.TaskCode != entityDto.OutTaskCode&&a.Status==(int)OutTaskStatusCaption.WaitingForPicking))
            {
                return DataProcess.Failure("该物料已被其他出库任务锁定，无法出库！");
            }

            if (string.IsNullOrEmpty(entityDto.LocationCode))
            {
                return DataProcess.Failure("尚未选择出库库位");
            }

            Location location = LocationRepository.Query().FirstOrDefault(a => a.Code == entityDto.LocationCode);
            if (location == null)
            {
                return DataProcess.Failure(string.Format("系统不存在该库位{0}", entityDto.LocationCode));
            }

            OutTaskMaterialLabelRepository.UnitOfWork.TransactionEnabled = true;

            //本出库任务单--全部
            IQuery<OutTaskMaterial> outTaskMaterialList = OutTaskMaterials.Where(a => a.OutTaskCode == entity.OutTaskCode);

            // 本储位，本物料--不包含批次
            var outMaterialEntity = outTaskMaterialList.FirstOrDefault(a => a.MaterialCode == entityDto.MaterialCode&&a.SuggestLocation== entityDto.LocationCode&&a.BatchCode==entityDto.BatchCode);

            // 该任务已拣选的物料数量

            decimal? pickedQuantity = outMaterialEntity.RealPickedQuantity.GetValueOrDefault(0);
            decimal? needQuantity = outMaterialEntity.Quantity;

            // 如果本次拣选的条码的数量小于需求数量
            entity.RealPickedQuantity = entity.RealPickedQuantity.GetValueOrDefault(0) + entityDto.OutTaskMaterialQuantity;
            if (entity.RealPickedQuantity >= entity.Quantity)
            {
                entity.Status = (int)Bussiness.Enums.OutTaskStatusCaption.Finished;
                entity.PickedTime = DateTime.Now;
            }
            else
            {
                entity.Status = (int)Bussiness.Enums.OutTaskStatusCaption.Picking;
            }


            Stock stock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == entityDto.MaterialLabel);
            if (stock == null)
            {
                return DataProcess.Failure("未找到该条码库存！");
            }

            // 如果在推荐的库存中
            var suggestStock = OutTaskMaterialLabels.FirstOrDefault(a => a.MaterialLabel == entityDto.MaterialLabel && a.TaskCode == entityDto.OutTaskCode);
            if (suggestStock != null)
            {
                suggestStock.Status = (int)OutTaskStatusCaption.Picking;
                suggestStock.PickedTime = DateTime.Now;
                suggestStock.RealPickedQuantity = suggestStock.RealPickedQuantity.GetValueOrDefault(0) + entityDto.OutTaskMaterialQuantity;
                if (suggestStock.RealPickedQuantity >= suggestStock.Quantity)
                {
                    suggestStock.Status = (int)OutTaskStatusCaption.Finished;
                }
                OutTaskMaterialLabelRepository.Update(suggestStock);
            }
            else    // 如果不在，判断下批次是否对的
            {
                if (entityDto.IsBatch && stock.BatchCode != entityDto.BatchCode)
                {
                    return DataProcess.Failure("该物料已开启批次管理，所选出库物料不属于本出库批次");
                }
            }

            // 如果没有启用了单包管理
            if (!entityDto.IsPackage)
            {
                stock.Quantity = stock.Quantity - entityDto.OutTaskMaterialQuantity;
                if (stock.Quantity < 0)
                {
                    return DataProcess.Failure(string.Format("出库数量{0}大于库存可用数量{1}!", entityDto.OutTaskMaterialQuantity, stock.Quantity));
                }
                stock.LockedQuantity = stock.LockedQuantity - entityDto.OutTaskMaterialQuantity;
                if (stock.LockedQuantity == 0)
                {
                    stock.IsLocked = false;
                }

                if (stock.Quantity == 0)
                {
                    StockRepository.Delete(stock);

                }
                else
                {
                    StockRepository.Update(stock);
                }

            }
            else // 如果启用了单包管理
            {
                // 直接删除该库存
                StockRepository.Delete(stock);
            }


            // 增加实际出库物料
            if (string.IsNullOrEmpty(entityDto.Operator))
            {
                entityDto.Operator = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
            }
            var outMaterialLabel = new OutMaterialLabel()
            {
                WareHouseCode= stock.WareHouseCode,
                BatchCode = stock.BatchCode,
                Status = (int)OutTaskStatusCaption.Picking,
                PickedTime = DateTime.Now,
                TaskCode = entity.OutTaskCode,
                OutCode = entity.OutCode,
                Quantity = entityDto.OutTaskMaterialQuantity,
                Operator = entityDto.Operator,
                MaterialLabel = entityDto.MaterialLabel,
                MaterialCode = entityDto.MaterialCode,
                LocationCode = entityDto.LocationCode,
                BillCode = entity.BillCode,


            };
            OutMaterialLabelRepository.Insert(outMaterialLabel);

            #region 核查储位状态--解锁储位
            //  托盘实体
            if (!StockContract.Stocks.Any(a=>a.LocationCode== entityDto.LocationCode))
            {
                location.LockMaterialCode = "";
                // 更新托盘储位推荐锁定的重量
                if (WareHouseContract.LocationRepository.Update(location) <= 0)
                {
                    return DataProcess.Failure(string.Format("储位解锁失败"));
                }
            }
            #endregion

            #region 更新托盘的重量锁定
            //  托盘实体
            var trayEntity = WareHouseContract.TrayWeightMapRepository.GetEntity(a => a.TrayId == stock.TrayId);

            trayEntity.LockWeight = trayEntity.LockWeight - entityDto.OutTaskMaterialQuantity * entityDto.UnitWeight;

            // 更新托盘储位推荐锁定的重量
            if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
            {
                return DataProcess.Failure(string.Format("托盘重量锁定失败"));
            }
            #endregion


            #region 更新出库物料明细状态

            OutTaskMaterial outMaterial = Mapper.MapTo<OutTaskMaterial>(entity);
            OutTaskMaterialRepository.Update(outMaterial);

            #endregion


            #region 更新出库任务状态

            OutTask outTaskEntity = OutTasks.FirstOrDefault(a => a.Code == entity.OutTaskCode);

            if (OutTaskMaterials.Where(a => a.OutTaskCode == entity.OutTaskCode).Any(a => a.Status != (int)Bussiness.Enums.OutTaskStatusCaption.Finished))
            {
                outTaskEntity.Status = (int)Bussiness.Enums.OutTaskStatusCaption.Picking;
            }
            else
            {
                outTaskEntity.Status = (int)Bussiness.Enums.OutTaskStatusCaption.Finished;

                // 释放所有未拣选的库存锁定
                var list = OutTaskMaterialLabels.Where(a =>
                        a.TaskCode == entity.OutTaskCode &&
                        a.Status == (int)OutTaskStatusCaption.WaitingForPicking)
                    .ToList();
                foreach (var stockupdate in list)
                {
                    Stock stockup = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == stockupdate.MaterialLabel);
                    stockup.LockedQuantity = stockup.LockedQuantity - stockupdate.Quantity;
                    stockup.IsLocked = false;
                    StockRepository.Update(stockup);
                    stockupdate.Status = (int)OutTaskStatusCaption.Finished;
                    OutTaskMaterialLabelRepository.Update(stockupdate);
                }
            }

            // 更新出库任务状态
            OutTaskRepository.Update(outTaskEntity);

            #endregion


            #region 更新出库单明细状态
            Out outEntity = OutContract.Outs.FirstOrDefault(a => a.Code == entity.OutCode);
            var outMaterialList = OutContract.OutMaterials
                .Where(a => a.OutCode == outEntity.Code && a.MaterialCode == entity.MaterialCode).ToList();
            var needShareQuantity = entityDto.OutTaskMaterialQuantity;

            foreach (var item in outMaterialList)
            {
                if (needShareQuantity > 0)
                {
                    // 如果行项目的已拣货数量小于需拣货数量
                    if (item.PickedQuantity.GetValueOrDefault(0) < item.Quantity)
                    {
                        // 如果本次出库数量小数
                        if (needShareQuantity <= (item.Quantity - item.PickedQuantity.GetValueOrDefault(0)))
                        {
                            item.PickedQuantity = item.PickedQuantity.GetValueOrDefault(0) + needShareQuantity;
                        }
                        else
                        {
                            item.PickedQuantity = item.Quantity;
                            needShareQuantity = needShareQuantity - (item.Quantity - item.PickedQuantity.GetValueOrDefault(0));
                            item.Status = (int)Bussiness.Enums.OutStatusCaption.Finished;
                        }
                    }
                }

                if (item.PickedQuantity.GetValueOrDefault(0) >= item.Quantity)
                {
                    item.Status = (int)Bussiness.Enums.OutStatusCaption.Finished;
                    // 如果为三方系统同步创建
                    if (outEntity.OrderType == (int)OrderTypeEnum.Other )
                    {
                        // 查询该中间表实体
                        var outMaterialIFEntity = OutContract.OutMaterialIFRepository.Query().FirstOrDefault(a => a.BillCode == outEntity.BillCode && a.ItemNo== item.ItemNo);
                        if (outMaterialIFEntity!=null)
                        {
                            if (outMaterialIFEntity.Status == (int)OrderEnum.Wait)
                            {
                                outMaterialIFEntity.Status = (int)OrderEnum.Finish;
                                outMaterialIFEntity.RealPickedQuantity = item.PickedQuantity.GetValueOrDefault(0);
                                OutContract.OutMaterialIFRepository.Update(outMaterialIFEntity);
                            }
                        }
                    }
                }
                OutContract.OutMaterialRepository.Update(item);
            }

            #endregion

            #region 更新出库单状态
            if (outMaterialList.Any(a => a.Status != (int)Bussiness.Enums.OutStatusCaption.Finished))
            {
                outEntity.Status = outEntity.Status;
            }
            else
            {
                outEntity.Status = (int)Bussiness.Enums.OutStatusCaption.Finished;
                // 如果为三方系统同步创建
                if (outEntity.OrderType == (int)OrderTypeEnum.Other )
                {
                    // 查询该中间表实体
                    var outIfEntity = OutContract.OutIFRepository.Query().FirstOrDefault(a => a.BillCode == outEntity.BillCode);
                    if (outIfEntity!=null)
                    {
                        if (outIfEntity.Status == (int)OrderEnum.Wait)
                        {
                            outIfEntity.Status = (int)OrderEnum.Finish;
                            OutContract.OutIFRepository.Update(outIfEntity);
                        }
                    }
                }
            }
            // 更新出库单状态
            OutContract.OutRepository.Update(outEntity);
            #endregion

            OutMaterial outEntityes = OutMaterials.FirstOrDefault(a => a.OutCode == entity.OutCode);
            OutTask outTaskEntityes = OutTaskDtos.FirstOrDefault(a => a.Code == entity.OutCode);

            #region 登录获取token及进行任务反馈
            //try
            //{
            //    // 登录成功，拼接token到下架任务反馈接口url和请求参数
            //    string url = "http://192.168.3.224:8181/api/zcdx-wms/v1/wms/wmsExWareBill/unShelveWK";
            //    // 构造请求参数
            //    List<EnWarehBillShelfModel> models = new List<EnWarehBillShelfModel>();
            //    EnWarehBillShelfModel model = new EnWarehBillShelfModel
            //    {
            //        MaterialId = outEntityes.CRRCID,
            //        Num = (int)entity.RealPickedQuantity,
            //        WorkStationId = outEntityes.WORKSTATIONID,
            //        WarehBinId = outEntityes.WAREHBINID,
            //        MergeId = outEntityes.MERGEID
            //    };
            //    models.Add(model);
            //    string json = JsonConvert.SerializeObject(models);

            //    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //    httpWebRequest.ContentType = "application/json; charset=UTF-8";
            //    httpWebRequest.Method = "POST";
            //    httpWebRequest.Timeout = 50000;
            //    httpWebRequest.ProtocolVersion = HttpVersion.Version10;
            //    byte[] btBodys = Encoding.UTF8.GetBytes(json);
            //    httpWebRequest.ContentLength = btBodys.Length;
            //    httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);
            //    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            //    string responseContent = streamReader.ReadToEnd();
            //    httpWebResponse.Close();
            //    streamReader.Close();
            //    if (httpWebResponse != null)
            //    {
            //        httpWebResponse.Close();
            //    }
            //    if (httpWebRequest != null)
            //    {
            //        httpWebRequest.Abort();
            //    }
            //    // 处理响应结果
            //    if (responseContent != null)
            //    {
            //        // 解析JSON数据
            //        var resultes = JsonConvert.DeserializeObject<dynamic>(responseContent);
            //        // 获取接口返回的各个字段值
            //        var codees = (int)resultes.code;
            //        var dataes = (string)resultes.data;
            //        var messagees = (string)resultes.message;
            //        var successes = (bool)resultes.success;

            //        if (codees == 0)
            //        {
            //            return DataProcess.Failure(dataes, messagees);
            //        }
            //        else if (codees == 2)
            //        {
            //            return DataProcess.Failure(dataes, messagees);
            //        }
            //    }
            //    else
            //    {
            //        return DataProcess.Failure("数据传输响应失败！");
            //    }
            //}
            //catch (Exception ex)
            //{

            //    return DataProcess.Failure("请求登录接口失败！", ex.Message);
            //}
            #endregion

            OutTaskMaterialLabelRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }

        // 上架任务模型
        public class EnWarehBillShelfModel
        {
            
            [JsonProperty("materialId")]
            public string MaterialId { get; set; }
            //数量
            [JsonProperty("qty")]
            public int Num { get; set; }
            
            [JsonProperty("workStationId")]
            public string WorkStationId { get; set; }
            
            [JsonProperty("warehBinId")]
            public object WarehBinId { get; set; }

            [JsonProperty("mergeId")]
            public object MergeId { get; set; }
        }

        public DataResult HandStartContainer(OutTaskMaterialDto entityDto)
        {
            // 物料实体映射
            OutTaskMaterialDto entity = OutTaskMaterialDtos.FirstOrDefault(
                a => a.OutTaskCode == entityDto.OutTaskCode
                   && a.MaterialCode == entityDto.MaterialCode
                   && a.SuggestLocation == entityDto.LocationCode
                   && a.BatchCode == entityDto.BatchCode);
            var location = WareHouseContract.LocationVIEWs.FirstOrDefault(a => a.Code == entityDto.LocationCode);
            var container = ContainerRepository.Query().FirstOrDefault(a => a.Code == location.ContainerCode);
            if (container != null && container.IsVirtual == false)
            {
                Bussiness.Common.RunningContainer runningContainer = new Common.RunningContainer();
                runningContainer.ContainerCode = container.Code;
                runningContainer.IpAddress = container.Ip;
                runningContainer.Port = Convert.ToInt32(container.Port);
                runningContainer.TrayCode = Convert.ToInt32(location.TrayCode);
                runningContainer.XLight = location.XLight;
                runningContainer.XLenght = location.XLenght.GetValueOrDefault(0);
                runningContainer.ContainerType = container.ContainerType;
                if (container.ContainerType == 2)
                {
                    var tray = this.TrayRepository.Query().FirstOrDefault(a => a.ContainerCode == container.Code && a.Code == location.TrayCode);
                    if (tray != null)
                    {
                        runningContainer.BracketNumber = tray.BracketNumber;
                        runningContainer.TrayCode = tray.BracketTrayNumber;
                    }
                }
                var result = new DataResult { Success = false, Message = "" };
                var serverAddress = System.Configuration.ConfigurationSettings.AppSettings["PLCServerAddress"].ToString();
                //if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Kardex)
                //{

                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningC3000Container", "post",Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningC3000Container(runningContainer);
                //}
                //else if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Hanel)
                //{
                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningHanelContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningHanelContainer(runningContainer);
                //}
                //else
                //{
                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));
                //}
                result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));
                return DataProcess.SetDataResult(result.Success, result.Message, result.Data);
            }
            else
            {
                return DataProcess.Failure("未找到货柜或者货柜未虚拟货柜");
            }
        }

        public DataResult HandRestoreContainer(OutTaskMaterialDto entityDto)
        {
            // 物料实体映射
            OutTaskMaterialDto entity = OutTaskMaterialDtos.FirstOrDefault(
                a => a.OutTaskCode == entityDto.OutTaskCode
                   && a.MaterialCode == entityDto.MaterialCode
                   && a.SuggestLocation == entityDto.LocationCode
                   && a.BatchCode == entityDto.BatchCode);
            var location = WareHouseContract.LocationVIEWs.FirstOrDefault(a => a.Code == entityDto.LocationCode);
            var container = ContainerRepository.Query().FirstOrDefault(a => a.Code == location.ContainerCode);
            if (container != null && container.IsVirtual == false)
            {
                Bussiness.Common.RunningContainer runningContainer = new Common.RunningContainer();
                runningContainer.ContainerCode = container.Code;
                runningContainer.IpAddress = container.Ip;
                runningContainer.Port = Convert.ToInt32(container.Port);
                runningContainer.TrayCode = Convert.ToInt32(location.TrayCode);
                runningContainer.XLight = location.XLight;
                runningContainer.XLenght = location.XLenght.GetValueOrDefault(0);
                runningContainer.ContainerType = container.ContainerType;
                var result = new DataResult { Success = false, Message = "" };
                var serverAddress = System.Configuration.ConfigurationSettings.AppSettings["PLCServerAddress"].ToString();
                //if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Kardex)
                //{

                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningC3000Container", "post",Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningC3000Container(runningContainer);
                //}
                //else if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Hanel)
                //{
                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningHanelContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningHanelContainer(runningContainer);
                //}
                //else
                //{
                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));
                //}
                result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRestoreContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));
                return DataProcess.SetDataResult(result.Success, result.Message, result.Data);
            }
            else
            {
                return DataProcess.Failure("未找到货柜或者货柜未虚拟货柜");
            }
        }
        
        //// 找到所有本任务推荐的库存
        //   var list = OutTaskMaterialLabels.Where(a =>
        //       a.MaterialCode == entityDto.MaterialCode && a.LocationCode == entityDto.LocationCode &&
        //       a.BatchCode == entityDto.BatchCode).ToList();

        //    // 需要释放发库存数量
        //    var needFreeQuantity = entityDto.OutTaskMaterialQuantity;
        //    foreach (var lockS in list)
        //    {
        //        if (needFreeQuantity > 0)
        //        {
        //            var stockLock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == lockS.MaterialLabel);
        //            if (stockLock.LockedQuantity >= needFreeQuantity)
        //            {
        //                stockLock.LockedQuantity = stockLock.LockedQuantity - needFreeQuantity;
        //                needFreeQuantity = 0;
        //                StockRepository.Update(stockLock);
        //            }
        //            else
        //            {
        //                stockLock.LockedQuantity = 0;
        //                needFreeQuantity = needFreeQuantity - stockLock.LockedQuantity;
        //            }
        //        }
        //    }


        ///// <summary>
        ///// 手动下架条码 减少库存
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //public DataResult ConfirmHandPicked(OutTaskMaterialLabelDto entity)
        //{
        //    OutTaskMaterialLabelRepository.UnitOfWork.TransactionEnabled = true;


        //    IQuery<OutTaskMaterialLabel> labelList = OutTaskMaterialLabels.Where(a => a.TaskCode == entity.TaskCode);

        //    List<OutTaskMaterialLabel> materialLabelList = labelList.Where(a => a.MaterialCode == entity.MaterialCode && a.Id != entity.Id).ToList();


        //    if (!materialLabelList.Any(a => a.MaterialCode == entity.MaterialCode && a.Status < (int)Bussiness.Enums.OutStatusCaption.WaitSending && a.Id != entity.Id))
        //    {
        //        // 获取此出库任务的条码列表
        //        List<OutTaskMaterialLabel> list = OutTaskMaterialLabels.Where(a => a.TaskCode == entity.TaskCode && a.MaterialCode == entity.MaterialCode).ToList();

        //        // 该任务已拣选的物料数量
        //        decimal? pickedQuantity = list.Sum(a => a.RealPickedQuantity);
        //        decimal? needQuantity = list.Sum(a => a.Quantity);

        //        // 如果本次拣选的条码
        //        if (entity.Quantity <= (needQuantity - pickedQuantity))
        //        {

        //            if (entity.OutTaskMaterialQuantity == entity.Quantity)
        //            {
        //                entity.Status = (int)Bussiness.Enums.OutTaskStatusCaption.Finished;
        //                entity.PickedTime = DateTime.Now;
        //            }
        //            else
        //            {
        //                entity.Status = (int)Bussiness.Enums.OutTaskStatusCaption.Picking;
        //            }


        //            if (string.IsNullOrEmpty(entity.Operator))
        //            {
        //                entity.Operator = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
        //            }

        //            entity.RealPickedQuantity = entity.RealPickedQuantity.GetValueOrDefault(0) + entity.OutTaskMaterialQuantity;

        //            //扣减库存
        //            Stock stock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel);
        //            // stock.Quantity = stock.Quantity - entity.RealPickedQuantity.GetValueOrDefault(0);
        //            stock.LockedQuantity = stock.LockedQuantity - entity.OutTaskMaterialQuantity;
        //            stock.Quantity = stock.Quantity - entity.OutTaskMaterialQuantity;

        //            if (stock.LockedQuantity == 0)
        //            {
        //                stock.IsLocked = false;
        //            }

        //            if (stock.Quantity == 0)
        //            {
        //                StockRepository.Delete(stock);
        //            }
        //            else
        //            {
        //                StockRepository.Update(stock);
        //            }



        //            #region 更新托盘的重量锁定
        //            //  托盘实体
        //            var trayEntity = WareHouseContract.TrayWeightMapRepository.GetEntity(a => a.TrayId == stock.TrayId);

        //            trayEntity.LockWeight = trayEntity.LockWeight - stock.Quantity * entity.UnitWeight;

        //            // 更新托盘储位推荐锁定的重量
        //            if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
        //            {
        //                return DataProcess.Failure(string.Format("托盘重量锁定失败"));
        //            }
        //            #endregion


        //            #region 更新出库物料明细状态

        //            OutTaskMaterialLabel outMaterialLabelEntity = Mapper.MapTo<OutTaskMaterialLabel>(entity);
        //            OutTaskMaterialLabelRepository.Update(outMaterialLabelEntity);

        //            #endregion


        //            #region 更新出库任务状态
        //            OutTask outTaskEntity = OutTasks.FirstOrDefault(a => a.Code == entity.TaskCode);

        //            if (labelList.Any(a => a.Status != (int)Bussiness.Enums.OutStatusCaption.Finished))
        //            {
        //                outTaskEntity.Status = (int)Bussiness.Enums.OutTaskStatusCaption.Picking;
        //            }
        //            else
        //            {
        //                outTaskEntity.Status = (int)Bussiness.Enums.OutTaskStatusCaption.Finished;
        //            }

        //            // 更新出库任务状态
        //            OutTaskRepository.Update(outTaskEntity);

        //            #endregion


        //            #region 更新出库单明细状态
        //            Out outEntity = OutContract.Outs.FirstOrDefault(a => a.Code == entity.OutCode);
        //            var outMaterialList = OutContract.OutMaterials
        //                .Where(a => a.OutCode == outEntity.Code && a.MaterialCode == entity.MaterialCode).ToList();
        //            var needShareQuantity = entity.OutTaskMaterialQuantity;

        //            foreach (var item in outMaterialList)
        //            {
        //                if (needShareQuantity > 0)
        //                {
        //                    // 如果行项目的已拣货数量小于需拣货数量
        //                    if (item.PickedQuantity.GetValueOrDefault(0) < item.Quantity)
        //                    {
        //                        // 如果本次出库数量小数
        //                        if (needShareQuantity <= (item.Quantity - item.PickedQuantity.GetValueOrDefault(0)))
        //                        {
        //                            item.PickedQuantity = item.PickedQuantity.GetValueOrDefault(0) + needShareQuantity;
        //                        }
        //                        else
        //                        {
        //                            item.PickedQuantity = item.Quantity;
        //                            needShareQuantity = needShareQuantity - (item.Quantity - item.PickedQuantity.GetValueOrDefault(0));
        //                            item.Status = (int)Bussiness.Enums.OutStatusCaption.Finished;
        //                        }
        //                    }

        //                    OutContract.OutMaterialRepository.Update(item);

        //                }
        //            }

        //            #endregion

        //            #region 更新出库单状态
        //            if (outMaterialList.Any(a => a.Status != (int)Bussiness.Enums.OutStatusCaption.Finished))
        //            {
        //                outEntity.Status = outEntity.Status;
        //            }
        //            else
        //            {
        //                outEntity.Status = (int)Bussiness.Enums.OutStatusCaption.Finished;
        //            }
        //            // 更新出库单状态
        //            OutContract.OutRepository.Update(outEntity);
        //            #endregion

        //        }
        //    }


        //    OutTaskMaterialLabelRepository.UnitOfWork.Commit();
        //    return DataProcess.Success("操作成功");
        //}




        /// <summary>
        /// 客户端手动执行出库
        /// </summary>
        /// <param name="outTaskEntityDto"></param>
        /// <returns></returns>
        public DataResult HandPickClient(OutTaskDto outTaskEntityDto)
        {

            //添加用户角色映射数据
            if (!outTaskEntityDto.OutTaskMaterialList.IsNullOrEmpty())
            {

                var outTaskMaterialLabelList = outTaskEntityDto.OutTaskMaterialList.FromJsonString<List<OutTaskMaterialLabelDto>>();
                var tempEntity = outTaskMaterialLabelList.FirstOrDefault();
                var containerEntity = WareHouseContract.Containers.FirstOrDefault(a => a.Code == tempEntity.ContainerCode);

                OutTaskMaterialLabelRepository.UnitOfWork.TransactionEnabled = true;

                // 创建出库单
                var outBill = new Out()
                {
                    BillCode = tempEntity.BillCode,
                    WareHouseCode = containerEntity.WareHouseCode,
                    OutDate = DateTime.Now.ToString(),
                    Status = (int)OutStatusCaption.Finished,
                    OutDict = tempEntity.OutDict,
                    Remark = outTaskEntityDto.Remark,
                    //CRRCID = outTaskEntityDto.CRRCID,
                };
                outBill.Code = SequenceContract.Create(outBill.GetType());

                var outMaterialList = outTaskMaterialLabelList.GroupBy(a => a.MaterialCode).ToList();
                List<OutMaterial> materialList = new List<OutMaterial>();
                foreach (IGrouping<string, OutTaskMaterialLabelDto> list in outMaterialList)
                {
                    var tempInEntity = list.FirstOrDefault();
                    decimal? inQuantity = list.Sum(a => a.Quantity);
                    var inMaterial = new OutMaterial()
                    {
                        OutCode = outBill.Code,
                        PickedQuantity = inQuantity,
                        Quantity = (decimal)inQuantity,
                        SendInQuantity = (decimal)inQuantity,
                        SupplierCode = tempInEntity.SupplierCode,
                        MaterialCode = list.Key,
                        OutDict = tempEntity.OutDict,
                        Status = (int)OutStatusCaption.Finished,
                        BatchCode = tempInEntity.BatchCode,
                        PickedTime = tempInEntity.PickedTime,
                    };
                    materialList.Add(inMaterial);
                }
                outBill.AddMaterial = materialList;
                if (!OutContract.CreateOutEntity(outBill).Success)
                {
                    return DataProcess.Failure(string.Format("出库单创建失败！"));
                }

                // 创建出库任务
                var outTaskBill = new OutTask()
                {
                    OutCode = outBill.Code,
                    ContainerCode = containerEntity.Code,
                    WareHouseCode = containerEntity.WareHouseCode,
                    OutDate = DateTime.Now.ToString(),
                    Status = (int)OutTaskStatusCaption.Finished,
                    OutDict = tempEntity.OutDict,
                    Remark= outBill.Remark
                };
                outTaskBill.Code = SequenceContract.Create(outTaskBill.GetType());
                if (!OutTaskRepository.Insert(outTaskBill))
                {
                    return DataProcess.Failure(string.Format("出库任务单创建失败！"));
                }

                var outTaskMaterialGroup = outTaskMaterialLabelList.GroupBy(a => new { a.WareHouseCode, a.ContainerCode, a.TrayId,a.MaterialCode,a.LocationCode,a.BatchCode });
                foreach ( var OutMaterial in outTaskMaterialGroup)
                {
                    // 创建出库任务明细
                    decimal pickQuantity = OutMaterial.Sum(a => a.Quantity);
                    var OutMaterialEntity = new OutTaskMaterial()
                    {
                        Status = (int)OutTaskStatusCaption.Finished,
                        WareHouseCode = OutMaterial.Key.WareHouseCode,
                        ContainerCode = OutMaterial.Key.ContainerCode,
                        OutCode = outBill.Code,
                        OutDict = outBill.OutDict,
                        Quantity = pickQuantity,
                        BatchCode = OutMaterial.Key.BatchCode,
                        SuggestLocation = OutMaterial.Key.LocationCode,
                        SuggestContainerCode = OutMaterial.Key.ContainerCode,
                        SuggestTrayId = OutMaterial.Key.TrayId,
                        RealPickedQuantity = pickQuantity,
                        MaterialCode = OutMaterial.Key.MaterialCode,
                        OutTaskCode = outTaskBill.Code,
                    };

                    if (!OutTaskMaterialRepository.Insert(OutMaterialEntity))
                    {
                        return DataProcess.Failure(string.Format("出库库任务明细{0}新增失败", outTaskBill.Code));
                    }
                }


                // 创建出库任务建议物料
                foreach (OutTaskMaterialLabelDto entityDto in outTaskMaterialLabelList)
                {
                    if (string.IsNullOrEmpty(entityDto.LocationCode))
                    {
                        return DataProcess.Failure("尚未选择拣货库位");
                    }

                    Location location = LocationRepository.Query().FirstOrDefault(a => a.Code == entityDto.LocationCode);
                    if (location == null)
                    {
                        return DataProcess.Failure(string.Format("系统不存在该拣货库位{0}", entityDto.LocationCode));
                    }

                    OutTaskMaterialLabel entity = Mapper.MapTo<OutTaskMaterialLabel>(entityDto);
                    entity.OutCode = outBill.Code;
                    entity.TaskCode = outTaskBill.Code;
                    entity.WareHouseCode = containerEntity.WareHouseCode;
                    entity.Quantity = entityDto.Quantity;
                    entity.Status = (int)OutTaskStatusCaption.Finished;
                    entity.TrayId = location.TrayId;

                    if (!OutTaskMaterialLabelRepository.Insert(entity))
                    {
                        return DataProcess.Failure(string.Format("出库任务单明细创建失败！"));
                    }

                    //扣减库存
                    Stock stock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == entityDto.MaterialLabel);
                    stock.Quantity = stock.Quantity - entityDto.OutTaskMaterialQuantity;

                    if (stock.LockedQuantity == 0)
                    {
                        stock.IsLocked = false;
                    }

                    if (stock.Quantity == 0)
                    {
                        StockRepository.Delete(stock);
                    }
                    else
                    {
                        StockRepository.Update(stock);
                    }

                    if (string.IsNullOrEmpty(entityDto.Operator))
                    {
                        entityDto.Operator = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
                    }
                    var outMaterialLabel = new OutMaterialLabel()
                    {
                        BatchCode = stock.BatchCode,
                        Status = (int)OutStatusCaption.Finished,
                        PickedTime = DateTime.Now,
                        TaskCode = entity.TaskCode,
                        OutCode = entity.OutCode,
                        Quantity = entityDto.OutTaskMaterialQuantity,
                        Operator = entityDto.Operator,
                        MaterialLabel = entityDto.MaterialLabel,
                        MaterialCode = entityDto.MaterialCode,
                        LocationCode = entityDto.LocationCode,
                        BillCode = entity.BillCode,

                    };
                    OutMaterialLabelRepository.Insert(outMaterialLabel);

                    #region 核查储位状态--解锁储位
                    //  托盘实体
                    if (!StockContract.Stocks.Any(a => a.LocationCode == entityDto.LocationCode))
                    {
                        location.LockMaterialCode = "";
                        // 更新托盘储位推荐锁定的重量
                        if (WareHouseContract.LocationRepository.Update(location) <= 0)
                        {
                            return DataProcess.Failure(string.Format("储位解锁失败"));
                        }
                    }
                    #endregion

                    #region 更新托盘的重量锁定
                    //  托盘实体
                    var trayEntity = WareHouseContract.TrayWeightMapRepository.GetEntity(a => a.TrayId == stock.TrayId);

                    trayEntity.LockWeight = trayEntity.LockWeight - entityDto.OutTaskMaterialQuantity * entityDto.UnitWeight;

                    // 更新托盘储位推荐锁定的重量
                    if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
                    {
                        return DataProcess.Failure(string.Format("托盘重量锁定失败"));
                    }
                    #endregion
                }
            }

            OutTaskMaterialLabelRepository.UnitOfWork.Commit();
            return DataProcess.Success("出库成功");
        }





        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult SendOrderToPTL(OutTask entity)
        {
            try
            {
                //存在盘点计划 不允许发送

                //if (CheckContract.Checks.Any(a=>a.Status<6 && a.WareHouseCode==entity.WareHouseCode))
                //{
                //    return DataProcess.Failure("该仓库尚有盘点计划,此时不允许发送PTL任务");
                //}

                //if (entity.Status != (int)(Bussiness.Enums.OutStatusCaption.WaitingForShelf))
                //{
                //    return DataProcess.Failure("入库单状态不对,应为待上架状态");
                //}

                ////if (Outs.Any(a => a.Status == (int)Bussiness.Enums.OutStatusCaption.Sheling))
                ////{
                ////    return DataProcess.Failure("入库单已发送至PTL");
                ////}
                //OutTaskRepository.UnitOfWork.TransactionEnabled = true;
                //entity.Status = (int)Bussiness.Enums.OutStatusCaption.Sheling;
                //List<OutMaterialDto> list = OutTaskMaterialDtos.Where(a => a.OutCode == entity.Code && a.Status == (int)Bussiness.Enums.OutStatusCaption.WaitingForShelf).ToList();
                //List<OutMaterial> list1 = OutTaskMaterialDtos.Where(a => a.OutCode == entity.Code && a.Status == (int)Bussiness.Enums.OutStatusCaption.WaitingForShelf).ToList();

                //DpsOutterfaceMain main = new DpsOutterfaceMain();
                //main.ProofId = Guid.NewGuid().ToString();
                //main.CreateDate = DateTime.Now;
                //main.Status = 0;
                //main.OrderType = 0;
                //main.OrderCode = entity.Code;
                //if (OutTaskRepository.Update(entity) < 0)
                //{
                //    return DataProcess.Failure("更新失败");
                //}
                //foreach (OutMaterialDto item in list)
                //{
                //    OutMaterial inMaterial = list1.FirstOrDefault(a => a.Id == item.Id);
                //    inMaterial.Status = (int)Bussiness.Enums.OutStatusCaption.Sheling;

                //    DpsOutterface dpsOutterface = new DpsOutterface();
                //    dpsOutterface.BatchNO = item.BatchCode;
                //    dpsOutterface.CreateDate = DateTime.Now;
                //    dpsOutterface.GoodsName = item.MaterialName;
                //    dpsOutterface.LocationId = item.LocationCode;
                //    dpsOutterface.MakerName = item.SupplierName;
                //    dpsOutterface.MaterialLabelId = 0;
                //    dpsOutterface.ProofId = main.ProofId;
                //    dpsOutterface.Quantity = Convert.ToOutt32(item.Quantity);
                //    dpsOutterface.RealQuantity = 0;
                //    dpsOutterface.RelationId = item.Id;
                //    dpsOutterface.Spec = item.MaterialUnit;
                //    dpsOutterface.Status = 0;
                //    dpsOutterface.OrderCode = item.OutCode;
                //    dpsOutterface.ToteId = item.MaterialLabel;
                //    if (DpsOutterfaceRepository.Outsert(dpsOutterface) == false)
                //    {
                //        return DataProcess.Failure("发送任务至PTL失败");
                //    }
                //    if (OutMaterialRepository.Update(inMaterial) < 0)
                //    {
                //        return DataProcess.Failure("更新失败");
                //    }
                //}
                //if (DpsOutterfaceMainRepository.Outsert(main)==false)
                //{
                //    return DataProcess.Failure("发送任务至PTL失败");
                //}
                //OutTaskRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {

                return DataProcess.Failure("发送任务至PTL失败:"+ex.Message);
            }

            return DataProcess.Success("发送PTL成功");

        }
    }
}
