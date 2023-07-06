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
using Bussiness.Entitys.InterFace;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Core.Mapping;
using HP.Core.Security;
using HP.Core.Security.Permissions;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility;
using HP.Utility.Data;
using HP.Utility.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bussiness.Services
{
    public class InTaskServer : Contracts.IInTaskContract
    {
        public IRepository<Material, int> MaterialRepository { get; set; }
        public IRepository<InMaterial, int> InMaterialRepository { get; set; }
        public IRepository<InTask, int> InTaskRepository { get; set; }
        public IRepository<InTaskMaterial, int> InTaskMaterialRepository { get; set; }

        public IRepository<Entitys.Stock, int> StockRepository { get; set; }

        public IRepository<Entitys.Tray, int> TrayRepository { get; set; }
        public IRepository<Entitys.Location, int> LocationRepository { get; set; }

        public IRepository<HPC.BaseService.Models.Dictionary, int> DictionaryRepository { get; set; }

        public IRepository<MouldInformation, int> MouldInformationRepository { get; set; }
        public IRepository<InMaterialLabel, int> InMaterialLabelRepository { get; set; }
        public ISequenceContract SequenceContract { set; get; }

        /// <summary>
        /// 入库单契约
        /// </summary>
        public IInContract InContract { set; get; }

        /// <summary>
        /// 标签契约
        /// </summary>
        public ILabelContract LabelContract { set; get; }

        public ISupplyContract SupplyContract { set; get; }

        public ICheckContract CheckContract { get; set; }

        public IMaterialContract MaterialContract { get; set; }

        public IEquipmentTypeContract EquipmentTypeContract { set; get; }

        public IRepository<Container, int> ContainerRepository { get; set; }

        public IMapper Mapper { set; get; }

        public IRepository<In, int> InRepository { get; set; }
        public IQuery<InMaterial> InMaterials => InMaterialRepository.Query();

        public IQuery<InTask> InTasks => InTaskRepository.Query();

        public IQuery<InTaskMaterial> InTaskMaterials => InTaskMaterialRepository.Query();
        public IQuery<InTaskMaterialDto> InTaskMaterialDtos => InTaskMaterials
            .InnerJoin(MaterialRepository.Query(), (inMaterial, material) => inMaterial.MaterialCode == material.Code)
            .InnerJoin(InTasks, (inTaskMaterial, material, inTasks) => inTaskMaterial.InTaskCode == inTasks.Code)
            .InnerJoin(WareHouseContract.LocationVMs, (inTaskMaterial, material, inTasks, location) => inTaskMaterial.SuggestLocation == location.Code)
            .LeftJoin(SupplyContract.Supplys, (inTaskMaterial, material, inTasks, location, supply) => inTaskMaterial.SupplierCode == supply.Code)
            .Select((inTaskMaterial, material, inTasks, location, supply) => new Dtos.InTaskMaterialDto
            {
                Id = inTaskMaterial.Id,
                InCode = inTaskMaterial.InCode,
                MaterialCode = inTaskMaterial.MaterialCode,
                SuggestContainerCode = inTaskMaterial.SuggestContainerCode,
                SuggestTrayId = inTaskMaterial.SuggestTrayId,
                SuggestTrayCode = location.TrayCode,
                InTaskCode = inTaskMaterial.InTaskCode,
                MaterialType = material.MaterialType,
                Quantity = inTaskMaterial.Quantity,
                UnitWeight = material.UnitWeight,
                ManufactrueDate = inTaskMaterial.ManufactrueDate,
                InDict = inTaskMaterial.InDict,
                BatchCode = inTaskMaterial.BatchCode,
                SupplierCode = inTaskMaterial.SupplierCode,
                IsPackage = material.IsPackage,
                IsMaxBatch = material.IsMaxBatch,
                AgeingPeriod = material.AgeingPeriod,
                IsBatch = material.IsBatch,
                SupplierName = supply.Name,
                Status = inTaskMaterial.Status,
                IsDeleted = inTaskMaterial.IsDeleted,
                BillCode = inTaskMaterial.BillCode,
                CustomCode = inTaskMaterial.CustomCode,
                CustomName = inTaskMaterial.CustomName,
                MaterialLabel = inTaskMaterial.MaterialLabel,
                SuggestLocation = inTaskMaterial.SuggestLocation,
                LocationCode = inTaskMaterial.LocationCode,
                ShelfTime = inTaskMaterial.ShelfTime,
                CreatedUserCode = inTaskMaterial.CreatedUserCode,
                CreatedUserName = inTaskMaterial.CreatedUserName,
                CreatedTime = inTaskMaterial.CreatedTime,
                UpdatedUserCode = inTaskMaterial.UpdatedUserCode,
                UpdatedUserName = inTaskMaterial.UpdatedUserName,
                UpdatedTime = inTaskMaterial.UpdatedTime,
                MaterialName = material.Name,
                MaterialUrl = material.PictureUrl,
                MaterialUnit = material.Unit,
                ItemNo = inTaskMaterial.ItemNo,
                WareHouseCode = inTasks.WareHouseCode,
                RealInQuantity = inTaskMaterial.RealInQuantity,
                XLight = location.XLight,
                YLight = location.YLight,
                BoxName = location.BoxName,
                BoxUrl = location.BoxUrl,
                ContainerType = location.ContainerType,
                BracketTrayNumber = location.BracketTrayNumber,
                BracketNumber = location.BracketNumber
            });

        public IQuery<InTaskDto> InTaskDtos {
            get {
                return InTasks
                    .LeftJoin(DictionaryRepository.Query(), (inentity, dictionary) => inentity.InDict == dictionary.Code)
                    .LeftJoin(WareHouseContract.WareHouses, (inentity, dictionary, warehouse) => inentity.WareHouseCode == warehouse.Code)
                    .InnerJoin(WareHouseContract.Containers, (inentity, dictionary, warehouse, containers) => inentity.ContainerCode == containers.Code)
                    .InnerJoin(EquipmentTypeContract.EquipmentType, (inentity, dictionary, warehouse, containers, equipmentType) => containers.EquipmentCode == equipmentType.Code)
                    .Select((inentity, dictionary, warehouse, containers, equipmentType) => new Dtos.InTaskDto()
                    {
                        Id = inentity.Id,
                        Code = inentity.Code,
                        InCode = inentity.InCode,
                        WareHouseCode = inentity.WareHouseCode,
                        WareHouseName = warehouse.Name,
                        ContainerCode = inentity.ContainerCode,
                        InDict = inentity.InDict,
                        Status = inentity.Status,
                        Remark = inentity.Remark,
                        IsDeleted = inentity.IsDeleted,
                        ShelfStartTime = inentity.ShelfStartTime,
                        ShelfEndTime = inentity.ShelfEndTime,
                        CreatedUserCode = inentity.CreatedUserCode,
                        CreatedUserName = inentity.CreatedUserName,
                        CreatedTime = inentity.CreatedTime,
                        UpdatedUserCode = inentity.UpdatedUserCode,
                        UpdatedUserName = inentity.UpdatedUserName,
                        UpdatedTime = inentity.UpdatedTime,
                        InDictDescription = dictionary.Name,
                        PictureUrl = equipmentType.PictureUrl,
                        InDate = inentity.InDate,
                        BillCode = inentity.BillCode
                    });
            }

        }

        public IWareHouseContract WareHouseContract { get; set; }



        /// <summary>
        /// 生成入库任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateInTaskEntity(In entity)
        {
            InTaskRepository.UnitOfWork.TransactionEnabled = true;
            {
                // 获取入库仓库
                var inEntity = InContract.Ins.FirstOrDefault(a => a.Code == entity.Code);
                // 获取入库物料明细
                var inMaterailList = InContract.InMaterials.Where(a => a.InCode == inEntity.Code).ToList();//.OrderBy(a=>a.MaterialCode).
                // 入库任务物料明细表
                var inTaskMaterialList = new List<InTaskMaterial>();

                //验证入库物料收维护载具
                foreach (var item in inMaterailList)
                {
                    var inQuantity = item.Quantity - item.SendInQuantity.GetValueOrDefault(0);
                    decimal sendInQuantity = 0;
                    var mateialEntity = MaterialContract.MaterialRepository.GetEntity(a => a.Code == item.MaterialCode);

                    //验证该物料是否维护了载具信息
                    if (!MaterialContract.MaterialBoxMaps.Any(a => a.MaterialCode == item.MaterialCode))
                    {
                        return DataProcess.Failure(string.Format("物料{0}未维护存放载具，请先维护", item.MaterialCode));
                    }

                    // 验证该物料属性组是否为存储锁定，如果是存储锁定
                    if (mateialEntity.IsNeedBlock)
                    {
                        if (!WareHouseContract.Locations.Any(a =>
                            a.SuggestMaterialCode == item.MaterialCode && a.WareHouseCode == inEntity.WareHouseCode))
                        {
                            return DataProcess.Failure(string.Format("物料{0}在仓库{1}中未维护存放储位，请先维护", item.MaterialCode,
                                inEntity.WareHouseCode));
                        }
                    }

                    /* 储位分配逻辑
                     1、查找该物料可存放的载具,确定全部可存放储位
                     2、查看该物料是存储锁定
                     3、查看是否混批
                     4、分配货柜及托盘时，查看是否超重
                    */

                    // 可存放的载具列表，库位绑定载具，载具绑定物料，库位码关联库存表

                    var query = WareHouseContract.LocationVIEWs.Where(a => a.WareHouseCode == entity.WareHouseCode);

                    // 查询可存放库位
                    // 本身已经了存放该物料-MaterialCode是中关联的MaterialCode
                    // 不存放该物料，但是不是存储锁定的
                    // 建议物料是该物料-SuggestMaterialCode是储位绑定的物料
                    query = query.Where(a => (a.MaterialCode == item.MaterialCode));
                    var a1 = query.ToList();
                    // 入库是存储锁定，则必须存放在所绑定的载具中,及该载具维护的物料即为待入库物料
                    if (mateialEntity.IsNeedBlock)
                    {
                        query = query.Where(a => a.SuggestMaterialCode == item.MaterialCode);
                    }
                    var a2 = query.ToList();
                    //如果不允许混批
                    if (!mateialEntity.IsMaxBatch)
                    {
                        // 批次相等，或者库存中没有存放物料
                        query = query.Where(a => a.BatchCode == item.BatchCode || string.IsNullOrEmpty(a.MaterialLabel));
                        var a4 = query.ToList();
                        // 并且不是锁定批次的盒子
                        query = query.Where(a => !a.IsLocked);
                    }
                    var a3 = query.ToList();
                    //判断此批物料的重量，进行储位筛选
                    // 是否维护单包数量
                    decimal? packageWeight = mateialEntity.UnitWeight; // 单个的重量
                    decimal packCount = 1; // 单包数量
                    if (mateialEntity.IsPackage && mateialEntity.PackageQuantity > 0)
                    {
                        packageWeight = mateialEntity.UnitWeight * mateialEntity.PackageQuantity; // 单包数量的重量
                        packCount = (decimal)mateialEntity.PackageQuantity;
                    }

                    // 根据载具可存放的数量进行储位筛选，分配数量，明确哪个储位存放多少数量，以重量做限制
                    var locationList = query.ToList();

                    var stockLocatonList = query.Where(a => a.Quantity > 0).OrderBy(a => a.Code).ToList();//有库存的库位

                    var NoStockLocationList = query.Where(a => a.Quantity == null && (a.LockMaterialCode == item.MaterialCode || string.IsNullOrEmpty(a.LockMaterialCode))).OrderBy(a => a.Code).ToList();//没有库存的库位;


                    //1 优先分配有库存

                    foreach (var locationEntity in stockLocatonList)
                    {
                        if (inQuantity > 0)
                        {
                            /* 计算储位可存放的数量*/
                            // 储位实体

                            // 当前储位已存放的数量
                            decimal lockQuantity =
                                WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code).Sum(a => a.Quantity) ==
                                null
                                    ? 0
                                    : (decimal)WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code)
                                        .Sum(a => a.Quantity);

                            // 当前储位可存放的数量
                            var available = (decimal)locationEntity.BoxCount - lockQuantity - (decimal)locationEntity.LockQuantity;

                            // 当前储位可存放的单包数量
                            decimal aviCount = Math.Floor(available / packCount);

                            // 本次需要入库的单包数量
                            decimal inCount = Math.Floor(inQuantity / packCount);

                            // 确定本储位入库单包数量
                            if (inCount > aviCount)
                            {
                                inCount = aviCount;
                            }

                            //如果剩余不足一个单包
                            if (Math.Floor(inQuantity / packCount) == 0)
                            {
                                packageWeight = mateialEntity.UnitWeight;
                                inCount = inQuantity;
                                packCount = 1;
                            }

                            // 入库可存放一个单包
                            if (available >= packCount)
                            {
                                // 本次入库的单包总重量
                                decimal? inWeight = inCount * packageWeight;

                                /* 计算托盘是否可承重*/
                                //  托盘实体
                                var trayEntity =
                                    WareHouseContract.TrayWeightMapRepository.GetEntity(a =>
                                        a.TrayId == locationEntity.TrayId);


                                bool isFlag = false;
                                // 如果托盘称重为0 ，则默认不开启托盘承重校验
                                if (trayEntity.MaxWeight == 0)
                                {
                                    isFlag = true;
                                }
                                else
                                {
                                    var availabelTray = trayEntity.MaxWeight - trayEntity.LockWeight -
                                                        trayEntity.TempLockWeight;

                                    // 如果托盘重量可存放
                                    if (availabelTray >= inWeight)
                                    {
                                        isFlag = true;
                                    }
                                    else
                                    {
                                        // 如果可存放下一个单包重量
                                        if (availabelTray >= packageWeight)
                                        {
                                            // 计算可以存放几个单包
                                            var tempInCount =
                                                Math.Floor((decimal)availabelTray / (decimal)packageWeight);
                                            // 确保是当前载具可存放的数量
                                            if (tempInCount < inCount)
                                            {
                                                inCount = tempInCount;
                                                isFlag = true;
                                            }
                                        }
                                    }
                                }


                                // 判断是满足生成任务的条件
                                if (isFlag)
                                {
                                    var inTaskMaterialItem = new InTaskMaterial()
                                    {
                                        InCode = item.InCode,
                                        BatchCode = item.BatchCode,
                                        ItemNo = item.ItemNo,
                                        SuggestContainerCode = locationEntity.ContainerCode,
                                        InDict = item.InDict,
                                        WareHouseCode = locationEntity.WareHouseCode,
                                        SuggestLocation = locationEntity.Code, // 建议入库位置
                                        SuggestTrayId = locationEntity.TrayId,
                                        Status = (int)InTaskStatusCaption.WaitingForShelf,
                                        Quantity = inCount * packCount, // 入库数量乘以单包数量
                                        SupplierCode = item.SupplierCode,
                                        CustomCode = item.CustomCode,
                                        MaterialCode = item.MaterialCode,
                                        XLight = locationEntity.XLight,
                                        YLight = locationEntity.YLight
                                    };
                                    inTaskMaterialList.Add(inTaskMaterialItem);
                                    inQuantity = inQuantity - inTaskMaterialItem.Quantity; // 减去入库数量
                                    sendInQuantity = sendInQuantity + inTaskMaterialItem.Quantity;

                                    // 如果托盘维护的承重信息
                                    if (trayEntity.MaxWeight > 0)
                                    {
                                        trayEntity.TempLockWeight = trayEntity.TempLockWeight + inTaskMaterialItem.Quantity * mateialEntity.UnitWeight;

                                        // 更新托盘储位推荐锁定的重量
                                        if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
                                        {
                                            return DataProcess.Failure(string.Format("托盘重量锁定失败"));
                                        }
                                    }

                                    // 锁定该储位的数量
                                    locationEntity.LockQuantity = locationEntity.LockQuantity + inTaskMaterialItem.Quantity;
                                    // 如果不允许混批，则锁定该储位
                                    if (!mateialEntity.IsMaxBatch)
                                    {
                                        locationEntity.IsLocked = true;
                                    }
                                    // 物料实体映射
                                    Location LocationItem = Mapper.MapTo<Location>(locationEntity);

                                    // 更新托盘储位推荐锁定的重量
                                    if (WareHouseContract.LocationRepository.Update(LocationItem) <= 0)
                                    {
                                        return DataProcess.Failure(string.Format("储位数量锁定失败"));
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    foreach (var locationEntity in NoStockLocationList)
                    {
                        if (inQuantity > 0)
                        {
                            /* 计算储位可存放的数量*/

                            // 当前储位已存放的数量
                            decimal lockQuantity =
                                WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code).Sum(a => a.Quantity) ==
                                null
                                    ? 0
                                    : (decimal)WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code)
                                        .Sum(a => a.Quantity);

                            // 当前储位可存放的数量
                            var available = (decimal)locationEntity.BoxCount - lockQuantity - (decimal)locationEntity.LockQuantity;

                            // 当前储位可存放的单包数量
                            decimal aviCount = Math.Floor(available / packCount);

                            // 本次需要入库的单包数量
                            decimal inCount = Math.Floor(inQuantity / packCount);

                            // 确定本储位入库单包数量
                            if (inCount > aviCount)
                            {
                                inCount = aviCount;
                            }

                            //如果剩余不足一个单包
                            if (Math.Floor(inQuantity / packCount) == 0)
                            {
                                packageWeight = mateialEntity.UnitWeight;
                                inCount = inQuantity;
                                packCount = 1;
                            }

                            // 入库可存放一个单包
                            if (available >= packCount)
                            {
                                // 本次入库的单包总重量
                                decimal? inWeight = inCount * packageWeight;

                                /* 计算托盘是否可承重*/
                                //  托盘实体
                                var trayEntity =
                                    WareHouseContract.TrayWeightMapRepository.GetEntity(a =>
                                        a.TrayId == locationEntity.TrayId);


                                bool isFlag = false;
                                // 如果托盘称重为0 ，则默认不开启托盘承重校验
                                if (trayEntity.MaxWeight == 0)
                                {
                                    isFlag = true;
                                }
                                else
                                {
                                    var availabelTray = trayEntity.MaxWeight - trayEntity.LockWeight -
                                                        trayEntity.TempLockWeight;

                                    // 如果托盘重量可存放
                                    if (availabelTray >= inWeight)
                                    {
                                        isFlag = true;
                                    }
                                    else
                                    {
                                        // 如果可存放下一个单包重量
                                        if (availabelTray >= packageWeight)
                                        {
                                            // 计算可以存放几个单包
                                            var tempInCount =
                                                Math.Floor((decimal)availabelTray / (decimal)packageWeight);
                                            // 确保是当前载具可存放的数量
                                            if (tempInCount < inCount)
                                            {
                                                inCount = tempInCount;
                                                isFlag = true;
                                            }
                                        }
                                    }
                                }

                                // 判断是满足生成任务的条件
                                if (isFlag)
                                {
                                    var inTaskMaterialItem = new InTaskMaterial()
                                    {
                                        InCode = item.InCode,
                                        BatchCode = item.BatchCode,
                                        ItemNo = item.ItemNo,
                                        SuggestContainerCode = locationEntity.ContainerCode,
                                        InDict = item.InDict,
                                        WareHouseCode = locationEntity.WareHouseCode,
                                        SuggestLocation = locationEntity.Code, // 建议入库位置
                                        SuggestTrayId = locationEntity.TrayId,
                                        Status = (int)InTaskStatusCaption.WaitingForShelf,
                                        Quantity = inCount * packCount, // 入库数量乘以单包数量
                                        SupplierCode = item.SupplierCode,
                                        CustomCode = item.CustomCode,
                                        MaterialCode = item.MaterialCode,
                                        XLight = locationEntity.XLight,
                                        YLight = locationEntity.YLight
                                    };
                                    inTaskMaterialList.Add(inTaskMaterialItem);
                                    inQuantity = inQuantity - inTaskMaterialItem.Quantity; // 减去入库数量
                                    sendInQuantity = sendInQuantity + inTaskMaterialItem.Quantity;

                                    // 如果托盘维护的承重信息
                                    if (trayEntity.MaxWeight > 0)
                                    {
                                        trayEntity.TempLockWeight = trayEntity.TempLockWeight + inTaskMaterialItem.Quantity * mateialEntity.UnitWeight;

                                        // 更新托盘储位推荐锁定的重量
                                        if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
                                        {
                                            return DataProcess.Failure(string.Format("托盘重量锁定失败"));
                                        }
                                    }

                                    // 锁定该储位的数量
                                    locationEntity.LockQuantity = locationEntity.LockQuantity + inTaskMaterialItem.Quantity;
                                    locationEntity.LockMaterialCode = inTaskMaterialItem.MaterialCode;
                                    // 如果不允许混批，则锁定该储位
                                    if (!mateialEntity.IsMaxBatch)
                                    {
                                        locationEntity.IsLocked = true;
                                    }
                                    // 物料实体映射
                                    Location LocationItem = Mapper.MapTo<Location>(locationEntity);

                                    // 更新托盘储位推荐锁定的重量
                                    if (WareHouseContract.LocationRepository.Update(LocationItem) <= 0)
                                    {
                                        return DataProcess.Failure(string.Format("储位数量锁定失败"));
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    // 计算已下发数量
                    item.SendInQuantity = item.SendInQuantity + sendInQuantity;
                }

                // 生成储位任务
                if (inTaskMaterialList.Count > 0)
                {
                    var groupList = inTaskMaterialList.GroupBy(a => new { a.WareHouseCode, a.SuggestContainerCode });

                    foreach (var item in groupList)
                    {
                        InTaskMaterial temp = item.FirstOrDefault();
                        var inTask = new InTask()
                        {
                            Status = (int)InTaskStatusCaption.WaitingForShelf,
                            WareHouseCode = temp.WareHouseCode,
                            ContainerCode = temp.SuggestContainerCode,
                            InCode = temp.InCode,
                            InDict = temp.InDict,
                            BillCode = inEntity.BillCode,
                            Remark = inEntity.Remark
                        };
                        inTask.Code = SequenceContract.Create(inTask.GetType());

                        var inMaterialGroup = item.GroupBy(a => new { a.SuggestLocation, a.MaterialCode, a.BatchCode });

                        foreach (var inTaskMaterial in inMaterialGroup)
                        {
                            InTaskMaterial tempMaterial = inTaskMaterial.FirstOrDefault();

                            var quantity = inTaskMaterial.Sum(a => a.Quantity);
                            var inTaskMaterialEntity = new InTaskMaterial()
                            {
                                InTaskCode = inTask.Code,
                                Status = (int)InTaskStatusCaption.WaitingForShelf,
                                WareHouseCode = tempMaterial.WareHouseCode,
                                InCode = tempMaterial.InCode,
                                InDict = tempMaterial.InDict,
                                BillCode = tempMaterial.BillCode,
                                BatchCode = tempMaterial.BatchCode,
                                ItemNo = tempMaterial.ItemNo,
                                SuggestContainerCode = tempMaterial.SuggestContainerCode,
                                SuggestLocation = tempMaterial.SuggestLocation, // 建议入库位置
                                SuggestTrayId = tempMaterial.SuggestTrayId,
                                Quantity = quantity, // 入库数量乘以单包数量
                                SupplierCode = tempMaterial.SupplierCode,
                                CustomCode = tempMaterial.CustomCode,
                                MaterialCode = tempMaterial.MaterialCode,
                                XLight = tempMaterial.XLight,
                                YLight = tempMaterial.YLight
                            };
                            if (!InTaskMaterialRepository.Insert(inTaskMaterialEntity))
                            {
                                return DataProcess.Failure(string.Format("入库任务明细{0}新增失败", entity.Code));
                            }
                        }

                        if (!InTaskRepository.Insert(inTask))
                        {
                            return DataProcess.Failure(string.Format("入库任务单{0}下发失败", entity.Code));
                        }
                    }


                    // 更新入库物料单
                    foreach (var item in inMaterailList)
                    {
                        if (item.SendInQuantity >= item.Quantity)
                        {
                            item.Status = (int)InStatusCaption.HandShelf;
                        }
                        else if (item.SendInQuantity > 0)
                        {
                            item.Status = (int)InStatusCaption.Sheling;
                        }
                        if (InContract.InMaterialRepository.Update(item) < 0)
                        {
                            return DataProcess.Failure("任务下发，入库物料明细更新失败");
                        }
                    }

                    // 更新入库单
                    inEntity.Status = (int)InStatusCaption.Sheling;

                    if (!InContract.InMaterials.Any(a => a.InCode == inEntity.Code && a.Status != (int)InStatusCaption.HandShelf))
                    {
                        inEntity.Status = (int)InStatusCaption.HandShelf;
                    }
                    if (InContract.InRepository.Update(inEntity) < 0)
                    {
                        return DataProcess.Failure("任务下发，入库单更新失败");
                    }
                }
                else
                {
                    InTaskRepository.UnitOfWork.Commit();
                    return DataProcess.Failure("当前物料无可存放储位");
                }

            }
            InTaskRepository.UnitOfWork.Commit();

            return DataProcess.Success(string.Format("入库任务单{0}下发成功", entity.Code));
        }



        ///// <summary>
        ///// 生成入库任务
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //public DataResult CreateInTaskEntity(In entity)
        //{
        //    InTaskRepository.UnitOfWork.TransactionEnabled = true;
        //    {
        //        // 获取入库仓库
        //        var inEntity = InContract.Ins.FirstOrDefault(a => a.Code == entity.Code);
        //        // 获取入库物料明细
        //        var inMaterailList = InContract.InMaterials.Where(a => a.InCode == inEntity.Code).ToList();
        //        // 入库任务物料明细表
        //        var inTaskMaterialList = new List<InTaskMaterial>();

        //        //验证入库物料收维护载具
        //        foreach (var item in inMaterailList)
        //        {
        //            var inQuantity = item.Quantity-item.SendInQuantity.GetValueOrDefault(0);
        //            decimal sendInQuantity = 0;
        //            var mateialEntity = MaterialContract.MaterialRepository.GetEntity(a => a.Code == item.MaterialCode);

        //            //验证该物料是否维护了载具信息
        //            if (!MaterialContract.MaterialBoxMaps.Any(a => a.MaterialCode == item.MaterialCode))
        //            {
        //                return DataProcess.Failure(string.Format("物料{0}未维护存放载具，请先维护", item.MaterialCode));
        //            }

        //            // 验证该物料属性组是否为存储锁定，如果是存储锁定
        //            if (mateialEntity.IsNeedBlock)
        //            {
        //                if (!WareHouseContract.Locations.Any(a =>
        //                    a.SuggestMaterialCode == item.MaterialCode && a.WareHouseCode == inEntity.WareHouseCode))
        //                {
        //                    return DataProcess.Failure(string.Format("物料{0}在仓库{1}中未维护存放储位，请先维护", item.MaterialCode,
        //                        inEntity.WareHouseCode));
        //                }
        //            }

        //            /* 储位分配逻辑
        //             1、查找该物料可存放的载具,确定全部可存放储位
        //             2、查看该物料是存储锁定
        //             3、查看是否混批
        //             4、分配货柜及托盘时，查看是否超重
        //            */

        //            // 可存放的载具列表，库位绑定载具，载具绑定物料，库位码关联库存表

        //            var query = WareHouseContract.LocationVIEWs.Where(a => a.WareHouseCode == entity.WareHouseCode);

        //            // 查询可存放库位
        //            // 本身已经了存放该物料-MaterialCode是中关联的MaterialCode
        //            // 不存放该物料，但是不是存储锁定的
        //            // 库存中没有其他物料的
        //            // 建议物料是该物料-SuggestMaterialCode是储位绑定的物料

        //            // 可存放该入库物料的储位--并且库存为空的储位,或者是存储锁定的物料

        //            // 先查询所有可存放的的储位
        //            query = query.Where(a => (a.MaterialCode == item.MaterialCode && a.LockMaterialCode == item.MaterialCode) 
        //                                     ||(a.MaterialCode == item.MaterialCode && string.IsNullOrEmpty(a.LockMaterialCode))).OrderByDesc(a=> (a.Quantity==null?0: a.Quantity));

        //           // query = query.Where(a => (a.MaterialCode == item.MaterialCode && string.IsNullOrEmpty(a.MaterialLabel) || a.MaterialCode == a.SuggestMaterialCode));

        //           var a1 = query.ToList();
        //            // 入库是存储锁定，则必须存放在所绑定的载具中,及该载具维护的物料即为待入库物料
        //            if (mateialEntity.IsNeedBlock)
        //            {
        //                query = query.Where(a => a.SuggestMaterialCode == item.MaterialCode);
        //            }
        //            var a2 = query.ToList();
        //            //如果不允许混批
        //            if (!mateialEntity.IsMaxBatch)
        //            {
        //                // 批次相等，或者库存中没有存放物料
        //                query = query.Where(a => a.BatchCode == item.BatchCode ||  string.IsNullOrEmpty(a.MaterialLabel));
        //                // 并且不是锁定批次的盒子
        //                query = query.Where(a => !a.IsLocked);
        //            }
        //            var a3 = query.ToList();
        //            //判断此批物料的重量，进行储位筛选
        //            // 是否维护单包数量
        //            decimal? packageWeight = mateialEntity.UnitWeight; // 单个的重量
        //            decimal packCount = 1; // 单包数量
        //            if (mateialEntity.IsPackage && mateialEntity.PackageQuantity > 0 )
        //            {
        //                packageWeight = mateialEntity.UnitWeight * mateialEntity.PackageQuantity; // 单包数量的重量
        //                packCount = (decimal) mateialEntity.PackageQuantity;
        //            }

        //            // 根据载具可存放的数量进行储位筛选，分配数量，明确哪个储位存放多少数量，以重量做限制
        //            var locationList = query.ToList();
        //            // 按照储位编码分组
        //            IEnumerable<IGrouping<string, LocationVIEW>> locationCodeList = locationList.GroupBy(a => a.Code);

        //            foreach (var location in locationCodeList)
        //            {
        //                // 入库仍有数量需要分配
        //                if (inQuantity > 0)
        //                {
        //                    /* 计算储位可存放的数量*/
        //                    // 储位实体
        //                    //var locationEntity = WareHouseContract.LocationVIEWs
        //                    //    .Where(a => a.Code == code && a.MaterialCode == item.MaterialCode)
        //                    //    .FirstOrDefault();
        //                    var locationEntity = location.FirstOrDefault();

        //                    // 当前储位已存放的数量
        //                    decimal lockQuantity =
        //                        WareHouseContract.LocationVIEWs.Where(a => a.Code == location.Key).Sum(a => a.Quantity) ==
        //                        null
        //                            ? 0
        //                            : (decimal) WareHouseContract.LocationVIEWs.Where(a => a.Code == location.Key)
        //                                .Sum(a => a.Quantity);

        //                    // 当前储位可存放的数量
        //                    var available = (decimal) locationEntity.BoxCount - lockQuantity- (decimal)locationEntity.LockQuantity;

        //                    // 当前储位可存放的单包数量
        //                    decimal aviCount = Math.Floor(available / packCount);

        //                    if (aviCount<=0)
        //                    {
        //                        continue;
        //                    }

        //                    // 本次需要入库的单包数量
        //                    decimal inCount = Math.Floor(inQuantity / packCount);

        //                    // 确定本储位入库单包数量
        //                    if (inCount > aviCount)
        //                    {
        //                        inCount = aviCount;
        //                    }

        //                    //如果剩余不足一个单包
        //                    if (Math.Floor(inQuantity / packCount) == 0)
        //                    {
        //                        packageWeight = mateialEntity.UnitWeight;
        //                        inCount = inQuantity;
        //                        packCount = 1;
        //                    }

        //                    // 入库可存放一个单包
        //                    if (available > packCount)
        //                    {
        //                        // 本次入库的单包总重量
        //                        decimal? inWeight = inCount * packageWeight;

        //                        /* 计算托盘是否可承重*/
        //                        //  托盘实体
        //                        var trayEntity =
        //                            WareHouseContract.TrayWeightMapRepository.GetEntity(a =>
        //                                a.TrayId == locationEntity.TrayId);


        //                        bool isFlag = false;
        //                        // 如果托盘称重为0 ，则默认不开启托盘承重校验
        //                        if (trayEntity.MaxWeight == 0)
        //                        {
        //                            isFlag = true;
        //                        }
        //                        else
        //                        {
        //                            var availabelTray = trayEntity.MaxWeight - trayEntity.LockWeight -
        //                                                trayEntity.TempLockWeight;

        //                            // 如果托盘重量可存放
        //                            if (availabelTray >= inWeight)
        //                            {
        //                                isFlag = true;
        //                            }
        //                            else
        //                            {
        //                                // 如果可存放下一个单包重量
        //                                if (availabelTray >= packageWeight)
        //                                {
        //                                    // 计算可以存放几个单包
        //                                    var tempInCount =
        //                                        Math.Floor((decimal)availabelTray / (decimal)packageWeight);
        //                                    // 确保是当前载具可存放的数量
        //                                    if (tempInCount < inCount)
        //                                    {
        //                                        inCount = tempInCount;
        //                                        isFlag = true;
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        // 判断是满足生成任务的条件
        //                        if (isFlag)
        //                        {
        //                            var inTaskMaterialItem = new InTaskMaterial()
        //                            {
        //                                InCode = item.InCode,
        //                                BatchCode = item.BatchCode,
        //                                ItemNo= item.ItemNo,
        //                                SuggestContainerCode = locationEntity.ContainerCode,
        //                                InDict=item.InDict,
        //                                WareHouseCode=locationEntity.WareHouseCode,
        //                                SuggestLocation = locationEntity.Code, // 建议入库位置
        //                                SuggestTrayId= locationEntity.TrayId,
        //                                Status = (int)InTaskStatusCaption.WaitingForShelf,
        //                                Quantity = inCount * packCount, // 入库数量乘以单包数量
        //                                SupplierCode = item.SupplierCode,
        //                                CustomCode = item.CustomCode,
        //                                MaterialCode = item.MaterialCode,
        //                                XLight= locationEntity.XLight,
        //                                YLight= locationEntity.YLight
        //                            };
        //                            inTaskMaterialList.Add(inTaskMaterialItem);
        //                            inQuantity = inQuantity - inTaskMaterialItem.Quantity; // 减去入库数量
        //                            sendInQuantity = sendInQuantity + inTaskMaterialItem.Quantity;

        //                            // 如果托盘维护的承重信息
        //                            if (trayEntity.MaxWeight > 0)
        //                            {
        //                                trayEntity.TempLockWeight = trayEntity.TempLockWeight + inTaskMaterialItem.Quantity * mateialEntity.UnitWeight;

        //                                // 更新托盘储位推荐锁定的重量
        //                                if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
        //                                {
        //                                    return DataProcess.Failure(string.Format("托盘重量锁定失败"));
        //                                }
        //                            }

        //                            // 锁定该储位的数量
        //                            locationEntity.LockQuantity = locationEntity.LockQuantity + inTaskMaterialItem.Quantity;
        //                            // 锁定该储位的锁定物料
        //                            locationEntity.LockMaterialCode = item.MaterialCode;
        //                            // 如果不允许混批，则锁定该储位
        //                            if (!mateialEntity.IsMaxBatch)
        //                            {
        //                                locationEntity.IsLocked = true;
        //                            }
        //                            // 物料实体映射
        //                            Location LocationItem = Mapper.MapTo<Location>(locationEntity);

        //                            // 更新托盘储位推荐锁定的重量
        //                            if (WareHouseContract.LocationRepository.Update(LocationItem) <= 0)
        //                            {
        //                                return DataProcess.Failure(string.Format("储位数量锁定失败"));
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //            // 计算已下发数量
        //            item.SendInQuantity = item.SendInQuantity + sendInQuantity;
        //        }

        //        // 生成储位任务
        //        if (inTaskMaterialList.Count > 0)
        //        {
        //            var groupList = inTaskMaterialList.GroupBy(a => new { a.WareHouseCode, a.SuggestContainerCode });

        //            foreach (var item in groupList)
        //            {
        //                InTaskMaterial temp = item.FirstOrDefault();
        //                var inTask = new InTask()
        //                {
        //                    Status = (int)InTaskStatusCaption.WaitingForShelf,
        //                    WareHouseCode = temp.WareHouseCode,
        //                    ContainerCode = temp.SuggestContainerCode,
        //                    InCode = temp.InCode,
        //                    InDict = temp.InDict,
        //                    BillCode = inEntity.BillCode,
        //                    Remark= inEntity.Remark
        //                };
        //                inTask.Code = SequenceContract.Create(inTask.GetType());

        //                var inMaterialGroup = item.GroupBy(a => new {a.LocationCode, a.MaterialCode, a.BatchCode });

        //                foreach (var inTaskMaterial in inMaterialGroup)
        //                {
        //                    InTaskMaterial tempMaterial = inTaskMaterial.FirstOrDefault();

        //                    var quantity = inTaskMaterial.Sum(a => a.Quantity);
        //                    var inTaskMaterialEntity = new InTaskMaterial()
        //                    {
        //                        InTaskCode= inTask.Code,
        //                        Status = (int)InTaskStatusCaption.WaitingForShelf,
        //                        WareHouseCode = tempMaterial.WareHouseCode,
        //                        InCode = tempMaterial.InCode,
        //                        InDict = tempMaterial.InDict,
        //                        BillCode = tempMaterial.BillCode,
        //                        BatchCode = tempMaterial.BatchCode,
        //                        ItemNo = tempMaterial.ItemNo,
        //                        SuggestContainerCode = tempMaterial.SuggestContainerCode,
        //                        SuggestLocation = tempMaterial.SuggestLocation, // 建议入库位置
        //                        SuggestTrayId = tempMaterial.SuggestTrayId,
        //                        Quantity = quantity, // 入库数量乘以单包数量
        //                        SupplierCode = tempMaterial.SupplierCode,
        //                        CustomCode = tempMaterial.CustomCode,
        //                        MaterialCode = tempMaterial.MaterialCode,
        //                        XLight = tempMaterial.XLight,
        //                        YLight = tempMaterial.YLight
        //                    };
        //                    if (!InTaskMaterialRepository.Insert(inTaskMaterialEntity))
        //                    {
        //                        return DataProcess.Failure(string.Format("入库任务明细{0}新增失败", entity.Code));
        //                    }
        //                }

        //                if (!InTaskRepository.Insert(inTask))
        //                {
        //                    return DataProcess.Failure(string.Format("入库任务单{0}下发失败", entity.Code));
        //                }
        //            }


        //            // 更新入库物料单
        //            foreach (var item in inMaterailList)
        //            {
        //                if (item.SendInQuantity >= item.Quantity)
        //                {
        //                    item.Status = (int)InStatusCaption.HandShelf;
        //                }
        //                else if (item.SendInQuantity > 0)
        //                {
        //                    item.Status = (int)InStatusCaption.Sheling;
        //                }
        //                if (InContract.InMaterialRepository.Update(item) < 0)
        //                {
        //                    return DataProcess.Failure("任务下发，入库物料明细更新失败");
        //                }
        //            }

        //            // 更新入库单
        //            inEntity.Status = (int)InStatusCaption.Sheling;

        //            if (!InContract.InMaterials.Any(a => a.InCode == inEntity.Code && a.Status != (int)InStatusCaption.HandShelf))
        //            {
        //                inEntity.Status = (int)InStatusCaption.HandShelf;
        //            }
        //            if (InContract.InRepository.Update(inEntity) < 0)
        //            {
        //                return DataProcess.Failure("任务下发，入库单更新失败");
        //            }
        //        }
        //        else
        //        {
        //            InTaskRepository.UnitOfWork.Commit();
        //            return DataProcess.Failure("当前物料无可存放储位");
        //        }

        //    }
        //    InTaskRepository.UnitOfWork.Commit();

        //    return DataProcess.Success(string.Format("入库任务单{0}下发成功", entity.Code));
        //}


        /// <summary>
        /// 获取物料条码可存放的储位
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CheckAvailableLocation(InTaskMaterial entity)
        {
            var inLocationList = new List<LocationVM>();
            // 获取入库单
            var inEntity = InContract.Ins.FirstOrDefault(a => a.Code == entity.InCode);

            //验证入库物料收维护载具
            // 获取标签数量
            var labelEntity = LabelContract.LabelRepository.GetEntity(a => a.Code == entity.MaterialLabel);
            var inQuantity = labelEntity.Quantity;

            var mateialEntity = MaterialContract.MaterialRepository.GetEntity(a => a.Code == entity.MaterialCode);
            //验证该物料是否维护了载具信息
            if (!MaterialContract.MaterialBoxMaps.Any(a => a.MaterialCode == entity.MaterialCode))
            {
                return DataProcess.Failure(string.Format("物料{0}未维护存放载具，请先维护", entity.MaterialCode));
            }

            // 验证该物料属性组是否为存储锁定，如果是存储锁定
            if (mateialEntity.IsNeedBlock)
            {
                if (!WareHouseContract.Locations.Any(a =>
                    a.SuggestMaterialCode == entity.MaterialCode && a.WareHouseCode == inEntity.WareHouseCode))
                {
                    return DataProcess.Failure(string.Format("物料{0}在仓库{1}中未维护存放储位，请先维护", entity.MaterialCode,
                        inEntity.WareHouseCode));
                }
            }

            /* 储位分配逻辑
            1、查找该物料可存放的载具,确定全部可存放储位
            2、查看该物料是存储锁定
            3、查看是否混批
            4、分配货柜及托盘时，查看是否超重
            */
            // 可存放的载具列表，库位绑定载具，载具绑定物料，库位码关联库存表
            // 查询可存放库位，本身存放该物料，或者不存放该物料，但是不是存储锁定的
            var query = WareHouseContract.LocationVIEWs.Where(a =>
                a.WareHouseCode == entity.WareHouseCode && a.ContainerCode == entity.SuggestContainerCode);

            query = query.Where(a => a.MaterialCode == entity.MaterialCode);//f|| !a.IsNeedBlock

            // 入库是存储锁定，则必须存放在所绑定的载具中,及该载具维护的物料即为待入库物料
            if (mateialEntity.IsNeedBlock)
            {
                query = query.Where(a => a.SuggestMaterialCode == entity.MaterialCode);
            }

            //如果不允许混批
            if (!mateialEntity.IsMaxBatch)
            {
                // 批次相等，或者库存中没有存放物料
                query = query.Where(a => a.BatchCode == labelEntity.BatchCode || a.MaterialLabel == null);
            }

            //判断此批物料的重量，进行储位筛选


            // 是否维护单包数量
            decimal? packageWeight = mateialEntity.UnitWeight; // 单个的重量
            decimal packCount = 1; // 单包数量
            if (mateialEntity.IsPackage && mateialEntity.PackageQuantity > 0)
            {
                packageWeight = mateialEntity.UnitWeight * mateialEntity.PackageQuantity; // 单包数量的重量
                packCount = (decimal)mateialEntity.PackageQuantity;
            }

            // 根据载具可存放的数量进行储位筛选，分配数量，明确哪个储位存放多少数量，以重量做限制
            var locationCodeList = query.GroupBy(a => a.Code).Select(a => a.Code).ToList();

            var locationList = query.ToList();

            var stockLocatonList = query.Where(a => a.Quantity > 0).OrderBy(a => a.Code).ToList();//有库存的库位

            var NoStockLocationList = query.Where(a => a.Quantity == null && (a.LockMaterialCode == entity.MaterialCode || string.IsNullOrEmpty(a.LockMaterialCode))).OrderBy(a => a.Code).ToList();//没有库存的库位;

            foreach (var locationEntity in stockLocatonList)
            {
                // 入库仍有数量需要分配
                if (inQuantity > 0)
                {
                    /* 计算储位可存放的数量*/
                    // 储位实体
                    //var locationEntity = WareHouseContract.LocationVIEWs
                    //    .Where(a => a.Code == code && a.MaterialCode == labelEntity.MaterialCode)
                    //    .FirstOrDefault();

                    // 当前储位已存放的数量
                    decimal lockQuantity =
                        WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code).Sum(a => a.Quantity) ==
                        null
                            ? 0
                            : (decimal)WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code)
                                .Sum(a => a.Quantity);

                    // 当前储位可存放的数量
                    var available = (decimal)locationEntity.BoxCount - lockQuantity;


                    // 入库可存放一个单包
                    if (available > labelEntity.Quantity)
                    {
                        // 本次入库的单包总重量
                        decimal? inWeight = labelEntity.Quantity * mateialEntity.UnitWeight; // 本次入库的重量
                        /* 计算托盘是否可承重*/
                        //  托盘实体
                        var trayEntity =
                            WareHouseContract.TrayWeightMapRepository.GetEntity(a =>
                                a.TrayId == locationEntity.TrayId);

                        var availabelTray = trayEntity.MaxWeight - trayEntity.LockWeight - trayEntity.TempLockWeight;

                        // 判断是满足生成任务的条件
                        if (availabelTray > inWeight)
                        {
                            var avLocation = WareHouseContract.LocationVMs.FirstOrDefault(a => a.Code == locationEntity.Code);

                            // 进行人员权限的筛选
                            IdentityTicket ticket = IdentityManager.Identity;

                            // 如果不是超级管理员，则进行人员权限筛选
                            if (!ticket.UserData.IsSystem)
                            {
                                if (WareHouseContract.TrayUserDtos.Any(a => a.UserCode == ticket.UserData.Code && avLocation.TrayId == a.TrayId))
                                {
                                    inLocationList.Add(avLocation);
                                }
                            }
                            else
                            {
                                inLocationList.Add(avLocation);
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            foreach (var locationEntity in NoStockLocationList)
            {
                // 入库仍有数量需要分配
                if (inQuantity > 0)
                {
                    /* 计算储位可存放的数量*/
                    // 储位实体
                    //var locationEntity = WareHouseContract.LocationVIEWs
                    //    .Where(a => a.Code == code && a.MaterialCode == labelEntity.MaterialCode)
                    //    .FirstOrDefault();

                    // 当前储位已存放的数量
                    decimal lockQuantity =
                        WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code).Sum(a => a.Quantity) ==
                        null
                            ? 0
                            : (decimal)WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code)
                                .Sum(a => a.Quantity);

                    // 当前储位可存放的数量
                    var available = (decimal)locationEntity.BoxCount - lockQuantity;


                    // 入库可存放一个单包
                    if (available > labelEntity.Quantity)
                    {
                        // 本次入库的单包总重量
                        decimal? inWeight = labelEntity.Quantity * mateialEntity.UnitWeight; // 本次入库的重量
                        /* 计算托盘是否可承重*/
                        //  托盘实体
                        var trayEntity =
                            WareHouseContract.TrayWeightMapRepository.GetEntity(a =>
                                a.TrayId == locationEntity.TrayId);

                        var availabelTray = trayEntity.MaxWeight - trayEntity.LockWeight - trayEntity.TempLockWeight;

                        // 判断是满足生成任务的条件
                        if (availabelTray > inWeight)
                        {
                            var avLocation = WareHouseContract.LocationVMs.FirstOrDefault(a => a.Code == locationEntity.Code);

                            // 进行人员权限的筛选
                            IdentityTicket ticket = IdentityManager.Identity;

                            // 如果不是超级管理员，则进行人员权限筛选
                            if (!ticket.UserData.IsSystem)
                            {
                                if (WareHouseContract.TrayUserDtos.Any(a => a.UserCode == ticket.UserData.Code && avLocation.TrayId == a.TrayId))
                                {
                                    inLocationList.Add(avLocation);
                                }
                            }
                            else
                            {
                                inLocationList.Add(avLocation);
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return DataProcess.Success(string.Format("获取可存放库位成功"), inLocationList);
        }


        /// <summary>
        /// 获取物料条码可存放的储位--客户端-手动入库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CheckClientLocation(InTaskMaterialDto entity)
        {
            try
            {
                var inLocationList = new List<LocationVM>();

                //验证入库物料收维护载具
                // 获取标签数量


                var inQuantity = entity.InTaskMaterialQuantity; // 本次入库数量

                var mateialEntity = MaterialContract.MaterialRepository.GetEntity(a => a.Code == entity.MaterialCode);
                //验证该物料是否维护了载具信息
                if (!MaterialContract.MaterialBoxMaps.Any(a => a.MaterialCode == entity.MaterialCode))
                {
                    return DataProcess.Failure(string.Format("物料{0}未维护存放载具，请先维护", entity.MaterialCode));
                }
                //if (inQuantity<=0)
                //{
                //    return DataProcess.Failure("入库数量小于0或者未填写");
                //}
                #region 客户端之前代码

                ///* 储位分配逻辑
                //1、查找该物料可存放的载具,确定全部可存放储位
                //2、查看该物料是存储锁定
                //3、查看是否混批
                //4、分配货柜及托盘时，查看是否超重
                //*/
                //// 可存放的载具列表，库位绑定载具，载具绑定物料，库位码关联库存表
                //// 查询可存放库位，本身存放该物料，或者不存放该物料，但是不是存储锁定的
                //var query = WareHouseContract.LocationVIEWs.Where(a => a.ContainerCode == entity.SuggestContainerCode);

                //query = WareHouseContract.LocationVIEWs.Where(a => a.MaterialCode == entity.MaterialCode && (a.LockMaterialCode == entity.MaterialCode || string.IsNullOrEmpty(a.LockMaterialCode)));//|| !a.IsNeedBlock

                //// 入库是存储锁定，则必须存放在所绑定的载具中,及该载具维护的物料即为待入库物料
                //if (mateialEntity.IsNeedBlock)
                //{
                //    query = query.Where(a => a.SuggestMaterialCode == entity.MaterialCode);
                //}

                ////如果不允许混批
                //if (!mateialEntity.IsMaxBatch)
                //{
                //    // 批次相等，或者库存中没有存放物料
                //    query = query.Where(a => a.BatchCode == entity.BatchCode || a.MaterialLabel == null);
                //}

                ////判断此批物料的重量，进行储位筛选


                //// 是否维护单包数量
                //decimal? packageWeight = mateialEntity.UnitWeight; // 单个的重量
                //decimal packCount = 1; // 单包数量
                //if (mateialEntity.IsPackage && mateialEntity.PackageQuantity > 0)
                //{
                //    packageWeight = mateialEntity.UnitWeight * mateialEntity.PackageQuantity; // 单包数量的重量
                //    packCount = (decimal)mateialEntity.PackageQuantity;
                //}

                //// var queryList = query.ToList();
                //// 根据载具可存放的数量进行储位筛选，分配数量，明确哪个储位存放多少数量，以重量做限制
                //var locationCodeList = query.GroupBy(a => a.Code).Select(a => a.Code).ToList();


                //foreach (var code in locationCodeList)
                //{
                //    // 仅获取前20个可存放储位
                //    if (inLocationList.Count < 20)
                //    {
                //        /* 计算储位可存放的数量*/
                //        // 储位实体
                //        var locationEntity = WareHouseContract.LocationVIEWs
                //            .Where(a => a.Code == code && a.MaterialCode == entity.MaterialCode)
                //            .FirstOrDefault();

                //        // 当前储位已存放的数量
                //        decimal lockQuantity =
                //            WareHouseContract.LocationVIEWs.Where(a => a.Code == code).Sum(a => a.Quantity) ==
                //            null
                //                ? 0
                //                : (decimal)WareHouseContract.LocationVIEWs.Where(a => a.Code == code)
                //                    .Sum(a => a.Quantity);

                //        // 当前储位可存放的数量
                //        var available = (decimal)locationEntity.BoxCount - lockQuantity - (decimal)locationEntity.LockQuantity;

                //        if (available <= 0)
                //        {
                //            continue;
                //        }

                //        var avLocation = WareHouseContract.LocationVMs.FirstOrDefault(a => a.Code == code);
                //        avLocation.AviQuantity = available;

                //        // 进行人员权限的筛选
                //        IdentityTicket ticket = IdentityManager.Identity;

                //        // 如果不是超级管理员，则进行人员权限筛选
                //        if (!ticket.UserData.IsSystem)
                //        {
                //            if (WareHouseContract.TrayUserDtos.Any(a => a.UserCode == ticket.UserData.Code && avLocation.TrayId == a.TrayId))
                //            {
                //                inLocationList.Add(avLocation);
                //            }
                //        }
                //        else
                //        {
                //            inLocationList.Add(avLocation);
                //        }
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}

                //inLocationList = inLocationList.OrderBy(a => a.AviQuantity).Take(20).ToList();
                #endregion

                inLocationList = GetClientAvailable(entity);
                if (inLocationList.Count == 0)
                {
                    return DataProcess.Failure("未找到可存放库位");
                }
                inLocationList = inLocationList.OrderBy(a => a.AviQuantity).Take(20).ToList();
                return DataProcess.Success(string.Format("获取可存放库位成功"), inLocationList);
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
        }

        public List<LocationVM> GetClientAvailable(InTaskMaterialDto entity)
        {
            var inLocationList = new List<LocationVM>();
            var mateialEntity = MaterialContract.MaterialRepository.GetEntity(a => a.Code == entity.MaterialCode);
            //验证入库物料收维护载具
            // 获取标签数量
            // var labelEntity = LabelContract.LabelRepository.GetEntity(a => a.Code == entity.MaterialLabel);
            var container = ContainerRepository.Query().FirstOrDefault(a => a.Code == entity.SuggestContainerCode);
            entity.WareHouseCode = container.WareHouseCode;
            var inQuantity = entity.InTaskMaterialQuantity; // 本次入库数量
            var query = WareHouseContract.LocationVIEWs.Where(a =>
     a.WareHouseCode == entity.WareHouseCode && a.ContainerCode == entity.SuggestContainerCode);

            query = query.Where(a => a.MaterialCode == entity.MaterialCode);//f|| !a.IsNeedBlock

            // 入库是存储锁定，则必须存放在所绑定的载具中,及该载具维护的物料即为待入库物料
            if (mateialEntity.IsNeedBlock)
            {
                query = query.Where(a => a.SuggestMaterialCode == entity.MaterialCode);
            }

            //如果不允许混批
            if (!mateialEntity.IsMaxBatch)
            {
                // 批次相等，或者库存中没有存放物料
                query = query.Where(a => a.BatchCode == entity.BatchCode || a.MaterialLabel == null);
            }

            //判断此批物料的重量，进行储位筛选


            // 是否维护单包数量
            decimal? packageWeight = mateialEntity.UnitWeight; // 单个的重量
            decimal packCount = 1; // 单包数量
            if (mateialEntity.IsPackage && mateialEntity.PackageQuantity > 0)
            {
                packageWeight = mateialEntity.UnitWeight * mateialEntity.PackageQuantity; // 单包数量的重量
                packCount = (decimal)mateialEntity.PackageQuantity;
            }

            // 根据 具可存放的数量进行储位筛选，分配数量，明确哪个储位存放多少数量，以重量做限制
            // var locationCodeList = query.GroupBy(a => a.Code).Select(a => a.Code).ToList();

            //   var locationList = query.ToList();

            var stockLocatonList = query.Where(a => a.Quantity > 0).OrderBy(a => a.Code).ToList();//有库存的库位

            var NoStockLocationList = query.Where(a => a.Quantity == null && (a.LockMaterialCode == entity.MaterialCode || string.IsNullOrEmpty(a.LockMaterialCode))).OrderBy(a => a.Code).ToList();//没有库存的库位;

            foreach (var locationEntity in stockLocatonList)
            {
                // 入库仍有数量需要分配

                //  if (inQuantity > 0)
                if (true)
                {
                    /* 计算储位可存放的数量*/
                    // 储位实体
                    //var locationEntity = WareHouseContract.LocationVIEWs
                    //    .Where(a => a.Code == code && a.MaterialCode == labelEntity.MaterialCode)
                    //    .FirstOrDefault();

                    // 当前储位已存放的数量
                    //decimal lockQuantity =
                    //    WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code).Sum(a => a.Quantity) ==
                    //    null
                    //        ? 0
                    //        : (decimal)WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code)
                    //            .Sum(a => a.Quantity);

                    decimal lockQuantity = StockRepository.Query().Where(a => a.LocationCode == locationEntity.Code).Sum(a => a.Quantity);//WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code).Sum(a => a.Quantity).GetValueOrDefault(0);

                    // 当前储位可存放的数量
                    var available = (decimal)locationEntity.BoxCount - lockQuantity;


                    // 入库可存放一个单包
                    if (available > 0)//inQuantity
                    {
                        // 本次入库的单包总重量
                        decimal? inWeight = 0 * mateialEntity.UnitWeight; //inQuantity// 本次入库的重量
                        /* 计算托盘是否可承重*/
                        //  托盘实体
                        var trayEntity =
                            WareHouseContract.TrayWeightMapRepository.GetEntity(a =>
                                a.TrayId == locationEntity.TrayId);

                        var availabelTray = trayEntity.MaxWeight - trayEntity.LockWeight - trayEntity.TempLockWeight;

                        // 判断是满足生成任务的条件
                        if (availabelTray > inWeight)
                        {
                            var avLocation = WareHouseContract.LocationVMs.FirstOrDefault(a => a.Code == locationEntity.Code);
                            avLocation.AviQuantity = available;
                            // 进行人员权限的筛选
                            IdentityTicket ticket = IdentityManager.Identity;

                            // 如果不是超级管理员，则进行人员权限筛选
                            if (!ticket.UserData.IsSystem)
                            {
                                if (WareHouseContract.TrayUserDtos.Any(a => a.UserCode == ticket.UserData.Code && avLocation.TrayId == a.TrayId))
                                {
                                    inLocationList.Add(avLocation);
                                }
                            }
                            else
                            {
                                inLocationList.Add(avLocation);
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            foreach (var locationEntity in NoStockLocationList)
            {
                // 入库仍有数量需要分配
                // if (inQuantity > 0)
                if (true)
                {
                    /* 计算储位可存放的数量*/
                    // 储位实体
                    //var locationEntity = WareHouseContract.LocationVIEWs
                    //    .Where(a => a.Code == code && a.MaterialCode == labelEntity.MaterialCode)
                    //    .FirstOrDefault();

                    // 当前储位已存放的数量
                    //decimal lockQuantity =
                    //    WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code).Sum(a => a.Quantity) ==
                    //    null
                    //        ? 0
                    //        : (decimal)WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code)
                    //            .Sum(a => a.Quantity);
                    decimal lockQuantity = StockRepository.Query().Where(a => a.LocationCode == locationEntity.Code).Sum(a => a.Quantity); //WareHouseContract.LocationVIEWs.Where(a => a.Code == locationEntity.Code).Sum(a => a.Quantity).GetValueOrDefault(0);

                    // 当前储位可存放的数量
                    var available = (decimal)locationEntity.BoxCount - lockQuantity;


                    // 入库可存放一个单包
                    if (available > 0)//inQuantity
                    {
                        // 本次入库的单包总重量
                        decimal? inWeight = 0 * mateialEntity.UnitWeight; // inQuantity 本次入库的重量
                        /* 计算托盘是否可承重*/
                        //  托盘实体
                        var trayEntity =
                            WareHouseContract.TrayWeightMapRepository.GetEntity(a =>
                                a.TrayId == locationEntity.TrayId);

                        var availabelTray = trayEntity.MaxWeight - trayEntity.LockWeight - trayEntity.TempLockWeight;

                        // 判断是满足生成任务的条件
                        if (availabelTray > inWeight)
                        {
                            var avLocation = WareHouseContract.LocationVMs.FirstOrDefault(a => a.Code == locationEntity.Code);
                            avLocation.AviQuantity = available;
                            // 进行人员权限的筛选
                            IdentityTicket ticket = IdentityManager.Identity;

                            // 如果不是超级管理员，则进行人员权限筛选
                            if (!ticket.UserData.IsSystem)
                            {
                                if (WareHouseContract.TrayUserDtos.Any(a => a.UserCode == ticket.UserData.Code && avLocation.TrayId == a.TrayId))
                                {
                                    inLocationList.Add(avLocation);
                                }
                            }
                            else
                            {
                                inLocationList.Add(avLocation);
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return inLocationList;
        }
        /// <summary>
        /// 执行上架
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult HandShelf(InTaskMaterialDto entityDto)
        {
            // 物料实体映射
            InTaskMaterial entity = InTaskMaterials.FirstOrDefault(
                a => a.InTaskCode == entityDto.InTaskCode
                   && a.MaterialCode == entityDto.MaterialCode
                   && a.SuggestLocation == entityDto.LocationCode
                   && a.BatchCode == entityDto.BatchCode);

            if (entityDto.InTaskMaterialQuantity > (entity.Quantity - entity.RealInQuantity))
            {
                return DataProcess.Failure("入库数量大于待入库数量，请核查");
            }

            if (entity.Status != (int)Bussiness.Enums.InTaskStatusCaption.WaitingForShelf &&
                entity.Status != (int)Bussiness.Enums.InTaskStatusCaption.InProgress)
            {
                return DataProcess.Failure("该入库任务明细单状态不为待上架或执行中");
            }

            InTask inTaskEntity = InTasks.FirstOrDefault(a => a.Code == entity.InTaskCode);
            if (inTaskEntity.Status != (int)Bussiness.Enums.InTaskStatusCaption.WaitingForShelf &&
                inTaskEntity.Status != (int)Bussiness.Enums.InTaskStatusCaption.InProgress)
            {
                return DataProcess.Failure("入库任务单状态不为待上架或者手动执行中");
            }

            if (string.IsNullOrEmpty(entityDto.LocationCode))
            {
                return DataProcess.Failure("尚未选择上架库位");
            }

            Location location = LocationRepository.Query().FirstOrDefault(a => a.Code == entityDto.LocationCode);
            if (location == null)
            {
                return DataProcess.Failure(string.Format("系统不存在该上架库位{0}", entityDto.LocationCode));
            }


            // 当前是否存在
            bool IsExistStock = false;
            var stock = new Stock();

            // 如果启用的单包管理，查询库内是否有相同的条码已入库
            if (entityDto.IsPackage)
            {
                var labelEntity = LabelContract.LabelDtos.FirstOrDefault(a => a.Code == entityDto.MaterialLabel);

                // 如果不允许混批
                if (!entityDto.IsMaxBatch)
                {
                    if (labelEntity.BatchCode != entityDto.BatchCode)
                    {
                        return DataProcess.Failure(string.Format("该入库物料批次不正确，请核查{0}", entityDto.BatchCode));
                    }
                }

                if (StockRepository.Query().Any(a => a.MaterialLabel == entityDto.MaterialLabel))
                {
                    return DataProcess.Failure(string.Format("库存已存在该物料条码{0}", entityDto.MaterialLabel));
                }
                stock = new Stock
                {
                    BatchCode = entityDto.BatchCode,
                    BillCode = entity.BillCode,
                    CreatedTime = DateTime.Now,
                    CustomCode = entity.CustomCode,
                    CustomName = entity.CustomName,
                    InCode = entity.InCode,
                    IsLocked = false,
                    LocationCode = entityDto.LocationCode,
                    ManufactureBillNo = "",
                    ManufactureDate = string.IsNullOrEmpty(entity.ManufactrueDate.ToString()) ? DateTime.Now : entity.ManufactrueDate,
                    WareHouseCode = location.WareHouseCode,
                    ContainerCode = location.ContainerCode,
                    TrayId = location.TrayId,
                    MaterialCode = entityDto.MaterialCode,
                    MaterialLabel = entityDto.MaterialLabel,
                    SupplierCode = entity.SupplierCode,
                    MaterialStatus = 0,
                    Quantity = entityDto.InTaskMaterialQuantity,
                    SaleBillItemNo = "",
                    SaleBillNo = "",
                    ShelfTime = DateTime.Now,
                    StockStatus = 0,
                    LockedQuantity = 0
                };
            }
            else // 非单包管理
            {
                // 增加批次管理
                stock = StockRepository.Query().FirstOrDefault(a => a.MaterialCode == entityDto.MaterialCode && a.LocationCode == location.Code);
                //entity.Status = (int)Bussiness.Enums.InStatusCaption.Finished;

                if (stock == null)
                {
                    var materialLabel = new Label()
                    {
                        MaterialCode = entityDto.MaterialCode,
                        Quantity = entityDto.InTaskMaterialQuantity,
                        SupplierCode = entity.SupplierCode,
                        ManufactrueDate = string.IsNullOrEmpty(entity.ManufactrueDate.ToString()) ? DateTime.Now : entity.ManufactrueDate,
                        BatchCode = entityDto.BatchCode,
                    };
                    var creatLabel = LabelContract.CreateLabel(materialLabel);
                    if (!creatLabel.Success)
                    {
                        return DataProcess.Failure(string.Format("创建物料条码失败{0}", entityDto.MaterialCode));
                    }
                    stock = new Stock
                    {
                        ContainerCode = location.ContainerCode,
                        BatchCode = entityDto.BatchCode,
                        BillCode = entity.BillCode,
                        CreatedTime = DateTime.Now,
                        ShelfTime = DateTime.Now,
                        CustomCode = entity.CustomCode,
                        CustomName = entity.CustomName,
                        InCode = entity.InCode,
                        IsLocked = false,
                        LocationCode = entityDto.LocationCode,
                        ManufactureBillNo = "",
                        ManufactureDate = string.IsNullOrEmpty(entity.ManufactrueDate.ToString()) ? DateTime.Now : entity.ManufactrueDate,
                        WareHouseCode = location.WareHouseCode,
                        MaterialCode = entityDto.MaterialCode,
                        MaterialLabel = creatLabel.Message,
                        SupplierCode = entity.SupplierCode,
                        TrayId = location.TrayId,
                        MaterialStatus = 0,
                        Quantity = entityDto.InTaskMaterialQuantity,
                        SaleBillItemNo = "",
                        SaleBillNo = "",
                        StockStatus = 0,
                        LockedQuantity = 0
                    };
                }
                else
                {
                    IsExistStock = true;
                    stock.Quantity = stock.Quantity + entityDto.InTaskMaterialQuantity;
                }

            }
            InTaskMaterialRepository.UnitOfWork.TransactionEnabled = true;


            #region 入库任务明细
            // 释放储位锁定数量
            var locationEntity = WareHouseContract.Locations
                .Where(a => a.Code == entity.SuggestLocation && a.TrayId == entity.SuggestTrayId)
                .FirstOrDefault();

            // 上架时间
            entity.ShelfTime = DateTime.Now;

            entity.RealInQuantity = entity.RealInQuantity + entityDto.InTaskMaterialQuantity;
            if (entity.Status != (int)Bussiness.Enums.InTaskStatusCaption.Finished)
            {
                entity.Status = (int)Bussiness.Enums.InTaskStatusCaption.InProgress;
                if (entity.RealInQuantity >= entity.Quantity)
                {
                    entity.Status = (int)Bussiness.Enums.InTaskStatusCaption.Finished;
                    locationEntity.IsLocked = false; // 全部完成，解锁储位
                }
            }
            // 判断是否为模具
            if (entityDto.MaterialType == (int)MaterialTypeEnum.Mould)
            {
                try
                {
                    var mouldEntity = new MouldInformation()
                    {
                        MaterialLabel = entityDto.MaterialLabel, //模具编码                       
                        MouldState = (int)MouldInformationEnum.InWarehouse
                    };
                    MouldInformationRepository.Insert(mouldEntity);
                }
                catch (Exception ex)
                {

                }
            }



            // 释放该储位的数量
            locationEntity.LockQuantity = locationEntity.LockQuantity - entityDto.InTaskMaterialQuantity;

            // 更新托盘储位推荐锁定的重量

            if (WareHouseContract.LocationRepository.Update(locationEntity) <= 0)
            {
                return DataProcess.Failure(string.Format("储位数量锁定失败"));
            }

            // 更新入库任务明细
            InTaskMaterialRepository.Update(entity);

            // 增加实际出库物料
            if (string.IsNullOrEmpty(entityDto.Operator))
            {
                entityDto.Operator = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
            }


            #endregion

            #region 历史入库物料表
            // 添加入库物料表
            var inMaterialLabel = new InMaterialLabel()
            {
                WareHouseCode = stock.WareHouseCode,
                BatchCode = stock.BatchCode,
                Status = (int)OutTaskStatusCaption.Picking,
                ShelfTime = DateTime.Now,
                TaskCode = entity.InTaskCode,
                InCode = entity.InCode,
                Quantity = entityDto.InTaskMaterialQuantity,
                Operator = entityDto.Operator,
                MaterialLabel = stock.MaterialLabel,
                MaterialCode = stock.MaterialCode,
                LocationCode = stock.LocationCode,
                BillCode = entity.BillCode

            };
            InMaterialLabelRepository.Insert(inMaterialLabel);
            #endregion

            #region 入库任务单

            // 更新入库任务单状态
            if (inTaskEntity.Status == (int)Bussiness.Enums.InTaskStatusCaption.WaitingForShelf)
            {
                inTaskEntity.ShelfStartTime = DateTime.Now;
                inTaskEntity.Status = (int)Bussiness.Enums.InTaskStatusCaption.InProgress;
            }
            // 更新入库任务单
            if (InTaskMaterialDtos.Where(a => a.InTaskCode == inTaskEntity.Code).Any(a => a.Status != (int)Bussiness.Enums.InTaskStatusCaption.Finished))
            {
                inTaskEntity.Status = inTaskEntity.Status;
            }
            else
            {
                inTaskEntity.ShelfEndTime = DateTime.Now;
                inTaskEntity.Status = (int)Bussiness.Enums.InTaskStatusCaption.Finished;
            }

            // 更新入库任务明细
            InTaskRepository.Update(inTaskEntity);

            #endregion


            #region 入库单明细状态更新
            // 如果全部下发，核查是否完成
            var inEntity = InContract.Ins.FirstOrDefault(a => a.Code == entity.InCode);
            // 核查入库单状态
            var inMaterialEntity = InContract.InMaterials.FirstOrDefault(a => a.InCode == entity.InCode && a.ItemNo == entity.ItemNo);
            inMaterialEntity.RealInQuantity = inMaterialEntity.RealInQuantity + entityDto.InTaskMaterialQuantity;

            if (inMaterialEntity.RealInQuantity >= inMaterialEntity.Quantity)
            {
                inMaterialEntity.Status = (int)InStatusCaption.Finished;

                // 如果为三方系统同步创建
                if (inEntity.OrderType == (int)OrderTypeEnum.Other)
                {
                    // 查询该中间表实体
                    var inMaterialIFEntity = InContract.InMaterialIFRepository.Query().FirstOrDefault(a => a.BillCode == inEntity.BillCode && a.ItemNo == inMaterialEntity.ItemNo);
                    if (inMaterialIFEntity != null)
                    {
                        if (inMaterialIFEntity.Status == (int)OrderEnum.Wait)
                        {
                            inMaterialIFEntity.Status = (int)OrderEnum.Finish;
                            inMaterialIFEntity.RealInQuantity = inMaterialEntity.RealInQuantity;
                            InContract.InMaterialIFRepository.Update(inMaterialIFEntity);
                        }
                    }
                }
            }

            // 更新入库单明细
            InContract.InMaterialRepository.Update(inMaterialEntity);

            #endregion


            #region 入库单状态更新

            if (InContract.InMaterials.Where(a => a.InCode == inEntity.Code).Any(a => a.Status != (int)InStatusCaption.Finished))
            {
                inEntity.Status = inEntity.Status;
            }
            else // 入库单完成
            {
                inEntity.Status = (int)InStatusCaption.Finished;

                // 如果为三方系统同步创建
                if (inEntity.OrderType == (int)OrderTypeEnum.Other)
                {
                    // 查询该中间表实体
                    var inIfEntity = InContract.InIFRepository.Query().FirstOrDefault(a => a.BillCode == inEntity.BillCode);
                    if (inIfEntity.Status == (int)OrderEnum.Wait)
                    {
                        inIfEntity.Status = (int)OrderEnum.Finish;
                        InContract.InIFRepository.Update(inIfEntity);

                    }
                }
            }

            // 更新入库单
            InContract.InRepository.Update(inEntity);

            #endregion



            // 解除托盘的重量临时锁定
            //  托盘实体
            var trayEntity = WareHouseContract.TrayWeightMapRepository.GetEntity(a => a.TrayId == location.TrayId);
            //  如果维护了托盘最大称重
            if (trayEntity.MaxWeight > 0)
            {
                trayEntity.TempLockWeight = trayEntity.TempLockWeight - entityDto.InTaskMaterialQuantity * entityDto.UnitWeight;
                trayEntity.LockWeight = trayEntity.LockWeight + entityDto.InTaskMaterialQuantity * entityDto.UnitWeight;

                // 更新托盘储位推荐锁定的重量
                if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
                {
                    return DataProcess.Failure(string.Format("托盘重量锁定失败"));
                }
            }

            // 是否启用老化时间
            if (entityDto.AgeingPeriod > 0)
            {
                //此刻的时间应大于入库时间+老化时间
                stock.MaterialStatus = (int)MaterialStatusCaption.Old;
            }

            if (IsExistStock)
            {
                StockRepository.Update(stock);
            }
            else
            {
                StockRepository.Insert(stock);
            }

            InMaterial inEntityes = InMaterials.FirstOrDefault(a => a.InCode == entity.InCode);

            #region 登录获取token及进行任务反馈
            //try
            //{
            //    //上架任务反馈接口url和请求参数
            //    string url = "http://192.168.3.224:8181/api/zcdx-wms/v1/wms/wmsEnWareBill/putawayWK";
            //    // 构造请求参数
            //    List<EnWarehBillShelfModel> models = new List<EnWarehBillShelfModel>();
            //    EnWarehBillShelfModel model = new EnWarehBillShelfModel
            //    {
            //        Id = inEntityes.CRRCID,
            //        Qty = (int)entity.RealInQuantity,
            //        WarehBinCode = entity.SuggestLocation
            //    };
            //    models.Add(model);
            //    string json = JsonConvert.SerializeObject(models);

            //    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //    httpWebRequest.ContentType = "application/json; charset=UTF-8";
            //    httpWebRequest.Method = "PUT";
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
            //    return DataProcess.Failure("请求服务失败！", ex.Message);
            //}
            #endregion

            InTaskMaterialRepository.UnitOfWork.Commit();

            return DataProcess.Success("上架成功！");
        }

        
    public class EnWarehBillShelfModel
    {
        //单据id
        [JsonProperty("id")]
        public string Id { get; set; }
        //数量
        [JsonProperty("qty")]
        public int Qty { get; set; }
        //仓位id(物料位置)
        [JsonProperty("warehBinCode")]
        public string WarehBinCode { get; set; }
    }

    /// <summary>
    /// 启动货柜
    /// </summary>
    /// <returns></returns>
    public DataResult HandStartContainer(InTaskMaterialDto entityDto)
        {
            // 物料实体映射
            InTaskMaterial entity = InTaskMaterials.FirstOrDefault(
                a => a.InTaskCode == entityDto.InTaskCode
                   && a.MaterialCode == entityDto.MaterialCode
                   && a.SuggestLocation == entityDto.LocationCode
                   && a.BatchCode == entityDto.BatchCode);
            var location = WareHouseContract.LocationVIEWs.FirstOrDefault(a => a.Code == entityDto.LocationCode);
            var container = ContainerRepository.Query().FirstOrDefault(a => a.Code == location.ContainerCode);
            if (container!=null && container.IsVirtual==false)
            {
                Bussiness.Common.RunningContainer runningContainer = new Common.RunningContainer();
                runningContainer.ContainerCode = container.Code;
                runningContainer.IpAddress = container.Ip;
                runningContainer.Port = Convert.ToInt32(container.Port);
                runningContainer.TrayCode = Convert.ToInt32(location.TrayCode);
                runningContainer.XLight = location.XLight;
                runningContainer.XLenght = location.XLenght.GetValueOrDefault(0);
                runningContainer.ContainerType = container.ContainerType;
                if (container.ContainerType==2)
                {
                    var tray = this.TrayRepository.Query().FirstOrDefault(a => a.ContainerCode == container.Code && a.Code == location.TrayCode);
                    if (tray!=null)
                    {
                        runningContainer.BracketNumber = tray.BracketNumber;
                        runningContainer.TrayCode = tray.BracketTrayNumber;
                    }
                }
                var result = new DataResult { Success = false, Message = "" };
                var serverAddress = System.Configuration.ConfigurationSettings.AppSettings["PLCServerAddress"].ToString();
                //if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Kardex)
                //{

                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningC3000Container", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningC3000Container(runningContainer);
                //}
                //else  if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Hanel)
                //{
                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningHanelContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningHanelContainer(runningContainer);
                //}
                //else
                //{

                //}

                result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));


                return DataProcess.SetDataResult(result.Success, result.Message, result.Data);
            }
            else
            {
                return DataProcess.Failure("未找到货柜或者货柜未虚拟货柜");
            }
        }

        /// <summary>
        /// 存入货柜
        /// </summary>
        /// <returns></returns>
        public DataResult HandRestoreContainer(InTaskMaterialDto entityDto)
        {
            // 物料实体映射
            InTaskMaterial entity = InTaskMaterials.FirstOrDefault(
                a => a.InTaskCode == entityDto.InTaskCode
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

                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningC3000Container", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningC3000Container(runningContainer);
                //}
                //else  if (container.ContainerType == (int)Bussiness.Enums.ContainerTypeEnum.Hanel)
                //{
                //    result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRunningHanelContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer)); // plcServer.StartRunningHanelContainer(runningContainer);
                //}
                //else
                //{

                //}

                result = Bussiness.Common.HttpApiHelper.InvokeWebapiApi(serverAddress, "StartRestoreContainer", "post", Newtonsoft.Json.JsonConvert.SerializeObject(runningContainer));


                return DataProcess.SetDataResult(result.Success, result.Message, result.Data);
            }
            else
            {
                return DataProcess.Failure("未找到货柜或者货柜未虚拟货柜");
            }
        }


        /// <summary>
        /// 客户端手动执行上架
        /// </summary>
        /// <param name="inTaskEntityDto"></param>
        /// <returns></returns>
        public DataResult HandShelfClient(InTaskDto inTaskEntityDto)
        {

            //添加用户角色映射数据
            if (!inTaskEntityDto.InTaskMaterialList.IsNullOrEmpty())
            {

                var inTaskMaterialList = inTaskEntityDto.InTaskMaterialList.FromJsonString<List<InTaskMaterialDto>>();

                var tempEntity = inTaskMaterialList.FirstOrDefault();
                var containerEntity = WareHouseContract.Containers.FirstOrDefault(a => a.Code == tempEntity.SuggestContainerCode);

                InTaskMaterialRepository.UnitOfWork.TransactionEnabled = true;

                // 创建入库单
                var inBill = new In()
                {
                    BillCode = tempEntity.BillCode,
                    WareHouseCode = containerEntity.WareHouseCode,
                    InDate = DateTime.Now.ToString(),
                    Status = (int)InStatusCaption.Finished,
                    InDict = tempEntity.InDict
                };
                inBill.Code = SequenceContract.Create(inBill.GetType());

                var inMaterialList = inTaskMaterialList.GroupBy(a => a.MaterialCode).ToList();

                List<InMaterial> materialList = new List<InMaterial>();
                foreach (IGrouping<string, InTaskMaterialDto> list in inMaterialList)
                {
                    var tempInEntity = list.FirstOrDefault();
                    decimal? inQuantity = list.Sum(a => a.Quantity);
                    var inMaterial = new InMaterial()
                    {
                        InCode = inBill.Code,
                        SendInQuantity = inQuantity,
                        Quantity = (decimal)inQuantity,
                        RealInQuantity = (decimal)inQuantity,
                        SupplierCode= tempInEntity.SupplierCode,
                        ManufactrueDate= tempInEntity.ManufactrueDate,
                        MaterialCode = list.Key,
                        InDict = tempEntity.InDict,
                        Status = (int)InStatusCaption.Finished,
                        BatchCode = tempInEntity.BatchCode,
                        ShelfTime= tempInEntity.ShelfTime,
                    };
                    materialList.Add(inMaterial);
                }
                inBill.AddMaterial = materialList;
                if (!InContract.CreateInEntity(inBill).Success)
                {
                    return DataProcess.Failure(string.Format("入库单创建失败！"));
                }

                // 创建入库任务
                var inTaskBill = new InTask()
                {
                    InCode = inBill.Code,
                    ContainerCode= containerEntity.Code,
                    WareHouseCode = containerEntity.WareHouseCode,
                    InDate = DateTime.Now.ToString(),
                    Status = (int)InTaskStatusCaption.Finished,
                    InDict = tempEntity.InDict
                };
                inTaskBill.Code = SequenceContract.Create(inTaskBill.GetType());
                if (!InTaskRepository.Insert(inTaskBill))
                {
                    return DataProcess.Failure(string.Format("入库任务单创建失败！"));
                }

                // 创建入库任务明细
                foreach (InTaskMaterialDto entityDto in inTaskMaterialList)
                {
                    if (string.IsNullOrEmpty(entityDto.LocationCode))
                    {
                        return DataProcess.Failure("尚未选择上架库位");
                    }

                    Location location = LocationRepository.Query().FirstOrDefault(a => a.Code == entityDto.LocationCode);
                    if (location == null)
                    {
                        return DataProcess.Failure(string.Format("系统不存在该上架库位{0}", entityDto.LocationCode));
                    }

                    InTaskMaterial entity = Mapper.MapTo<InTaskMaterial>(entityDto);
                    entity.InCode = inBill.Code;
                    entity.InTaskCode = inTaskBill.Code;
                    entity.WareHouseCode = containerEntity.WareHouseCode;
                    entity.Status = (int) InTaskStatusCaption.Finished;
                    entity.SuggestTrayId = location.TrayId;

                    if (!InTaskMaterialRepository.Insert(entity))
                    {
                        return DataProcess.Failure(string.Format("入库任务单明细创建失败！"));
                    }

                    // 添加库存信息
                    // 当前是否存在
                    bool IsExistStock = false;
                    var stock = new Stock();

                    // 物料实体
                    var materialEntity = MaterialContract.Materials.FirstOrDefault(a => a.Code == entityDto.MaterialCode);
                    // 如果启用的单包管理，查询库内是否有相同的条码已入库
                    if (materialEntity.IsPackage)
                    {
                        //if (StockRepository.Query().Any(a => a.MaterialLabel == entityDto.MaterialLabel))
                        //{
                        //    return DataProcess.Failure(string.Format("库存已存在该物料条码{0}", entityDto.MaterialLabel));
                        //}
                        var materialLabel = new Label();
                        var materialLabelCode = SequenceContract.Create(materialLabel.GetType());
           

                        entity.MaterialLabel = materialLabelCode;

                        stock = new Stock
                        {
                            BatchCode = entity.BatchCode,
                            BillCode = entity.BillCode,
                            CreatedTime = DateTime.Now,
                            CustomCode = entity.CustomCode,
                            CustomName = entity.CustomName,
                            InCode = entity.InCode,
                            IsLocked = false,
                            LocationCode = entity.LocationCode,
                            ManufactureBillNo = "",
                            ManufactureDate = string.IsNullOrEmpty(entity.ManufactrueDate.ToString()) ? DateTime.Now : entity.ManufactrueDate,
                            WareHouseCode = location.WareHouseCode,
                            ContainerCode = location.ContainerCode,
                            TrayId = location.TrayId,
                            MaterialCode = entity.MaterialCode,
                            MaterialLabel = materialLabelCode, //entity.MaterialLabel,
                            SupplierCode = entity.SupplierCode,
                            MaterialStatus = 0,
                            Quantity = entityDto.InTaskMaterialQuantity,
                            SaleBillItemNo = "",
                            SaleBillNo = "",
                            ShelfTime = DateTime.Now,
                            StockStatus = 0,
                            LockedQuantity = 0
                        };

                        materialLabel.BatchCode = stock.BatchCode;
                        materialLabel.Code = materialLabelCode;
                        materialLabel.IsDeleted = false;
                        materialLabel.ManufactrueDate = stock.ManufactureDate;
                        materialLabel.MaterialCode = stock.MaterialCode;
                        materialLabel.Quantity = stock.Quantity;
                        materialLabel.SupplierCode = stock.SupplierCode;
                        LabelContract.CreateLabel(materialLabel);

                        // 判断是否为模具
                        if (materialEntity.MaterialType == (int)MaterialTypeEnum.Mould)
                        {
                            try
                            {
                                var mouldEntity = new MouldInformation()
                                {
                                    MaterialLabel = materialLabelCode, //模具编码                       
                                    MouldState = (int)MouldInformationEnum.InWarehouse
                                };
                                MouldInformationRepository.Insert(mouldEntity);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    else // 非单包管理
                    {

                        // 如果允许混批
                        //if (materialEntity.IsMaxBatch)
                        //{
                        //    stock = StockRepository.Query().FirstOrDefault(a => a.MaterialCode == entity.MaterialCode && a.LocationCode == location.Code);
                        //}
                        //else //如果不允许混批
                        //{
                        //    stock = StockRepository.Query().FirstOrDefault(a => a.MaterialCode == entity.MaterialCode && a.LocationCode == location.Code && a.BatchCode == entityDto.BatchCode);
                        //}

                        stock = StockRepository.Query().FirstOrDefault(a => a.MaterialCode == entity.MaterialCode && a.LocationCode == location.Code);


                        if (stock == null)
                        {
                            var materialLabel = new Label();
                            var materialLabelCode = SequenceContract.Create(materialLabel.GetType());
                            stock = new Stock
                            {
                                ContainerCode = location.ContainerCode,
                                BatchCode = entity.BatchCode,
                                BillCode = entity.BillCode,
                                CreatedTime = DateTime.Now,
                                ShelfTime = DateTime.Now,
                                CustomCode = entity.CustomCode,
                                CustomName = entity.CustomName,
                                InCode = entity.InCode,
                                IsLocked = false,
                                LocationCode = entity.LocationCode,
                                ManufactureBillNo = "",
                                ManufactureDate = string.IsNullOrEmpty(entity.ManufactrueDate.ToString()) ? DateTime.Now : entity.ManufactrueDate,
                                WareHouseCode = location.WareHouseCode,
                                MaterialCode = entity.MaterialCode,
                                MaterialLabel = materialLabelCode,
                                SupplierCode = entity.SupplierCode,
                                TrayId = location.TrayId,
                                MaterialStatus = 0,
                                Quantity = entityDto.InTaskMaterialQuantity,
                                SaleBillItemNo = "",
                                SaleBillNo = "",
                                StockStatus = 0,
                                LockedQuantity = 0
                            };

                            materialLabel.BatchCode = stock.BatchCode;
                            materialLabel.Code = materialLabelCode;
                            materialLabel.IsDeleted = false;
                            materialLabel.ManufactrueDate = stock.ManufactureDate;
                            materialLabel.MaterialCode = stock.MaterialCode;
                            materialLabel.Quantity = stock.Quantity;
                            materialLabel.SupplierCode = stock.SupplierCode;
                            LabelContract.CreateLabel(materialLabel);
                        }
                        else
                        {
                            IsExistStock = true;
                            stock.Quantity = stock.Quantity + entityDto.InTaskMaterialQuantity;
                        }
                    }
                    if (IsExistStock)
                    {
                        StockRepository.Update(stock);
                    }
                    else
                    {
                        StockRepository.Insert(stock);
                    }

                    
                    // 锁定该储位的数量
                    location.LockMaterialCode = entity.MaterialCode;
                    // 物料实体映射
                    Location LocationItem = Mapper.MapTo<Location>(location);

                    // 更新托盘储位推荐锁定的重量
                    if (WareHouseContract.LocationRepository.Update(LocationItem) <= 0)
                    {
                        return DataProcess.Failure(string.Format("储位数量锁定失败"));
                    }


                    #region 历史入库物料表
                    // 增加实际出库物料
                    if (string.IsNullOrEmpty(entityDto.Operator))
                    {
                        entityDto.Operator = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
                    }
                    // 添加入库物料表
                    var inMaterialLabel = new InMaterialLabel()
                    {
                        WareHouseCode = stock.WareHouseCode,
                        BatchCode = stock.BatchCode,
                        Status = (int)OutTaskStatusCaption.Picking,
                        ShelfTime = DateTime.Now,
                        TaskCode = entity.InTaskCode,
                        InCode = entity.InCode,
                        Quantity = entityDto.InTaskMaterialQuantity,
                        Operator = entityDto.Operator,
                        MaterialLabel = stock.MaterialLabel,
                        MaterialCode = stock.MaterialCode,
                        LocationCode = stock.LocationCode,
                        BillCode = entity.BillCode
                    };
                    InMaterialLabelRepository.Insert(inMaterialLabel);
                    #endregion
                }
            }


            InTaskMaterialRepository.UnitOfWork.Commit();
            return DataProcess.Success("上架成功");
        }

        /// <summary>
        /// 删除入库任务单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveInTask(int id)
        {
            InTask entity = InTaskRepository.GetEntity(id);
            if (entity.Status != (int)Enums.InStatusCaption.WaitingForShelf)
            {
                return DataProcess.Failure("该入库单执行中或已完成");
            }

            InTaskRepository.UnitOfWork.TransactionEnabled = true;

            // 更新单据状态
            // 获取入库任务明细单
            List<InTaskMaterialDto> list = InTaskMaterialDtos.Where(a => a.InTaskCode == entity.Code).ToList();

            // 获取入库单
            var inEntity = InContract.InRepository.GetEntity(a => a.Code == entity.InCode);

            bool inFinishFlag = false;
            foreach (var item in list)
            {
                // 入库单行项目实体
                var inMaterialEntity =
                    InContract.InMaterialRepository.GetEntity(a => a.InCode == item.InCode && a.ItemNo == item.ItemNo);
                inMaterialEntity.SendInQuantity = inMaterialEntity.SendInQuantity -item.Quantity;

                //如果未存在下发情况
                if (inMaterialEntity.SendInQuantity == 0)
                {
                    inFinishFlag = true;
                    inMaterialEntity.Status = (int)InStatusCaption.WaitingForShelf;
                }
                else
                {
                    inFinishFlag = false;
                }
                // 更新入库单行项目
                if (InContract.InMaterialRepository.Update(inMaterialEntity) < 0)
                {
                    return DataProcess.Failure("任务删除，入库单行项目更新失败");
                }

                // 解除托盘的重量锁定
                //  托盘实体
                var trayEntity = WareHouseContract.TrayWeightMapRepository.GetEntity(a => a.TrayId == item.SuggestTrayId);
                trayEntity.TempLockWeight = trayEntity.TempLockWeight - item.Quantity * item.UnitWeight;

                // 更新托盘储位推荐锁定的重量
                if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
                {
                    return DataProcess.Failure(string.Format("托盘重量锁定失败"));
                }
            }


            if (inFinishFlag)
            {
                if (!InContract.InMaterials.Any(a=>a.InCode==entity.InCode&&a.Status!= (int)InStatusCaption.WaitingForShelf))
                {
                    inEntity.Status = (int)InStatusCaption.WaitingForShelf;
                    if (InContract.InRepository.Update(inEntity) < 0)
                    {
                        return DataProcess.Failure("任务下发，入库单更新失败");
                    }
                }
            }

            // 删除入库任务
            if (InTaskRepository.Delete(id) <= 0)
            {
                return DataProcess.Failure(string.Format("入库单{0}删除失败", entity.Code));
            }

            if (list != null && list.Count > 0)
            {
                foreach (InTaskMaterial item in list)
                {
                    DataResult result = RemoveInTaskMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }

            InTaskRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }


        /// <summary>
        /// 删除入库任务单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult CancelInTask(int id)
        {
            InTask entity = InTaskRepository.GetEntity(id);


            if (entity.Status != (int)Enums.InStatusCaption.WaitingForShelf)
            {
                return DataProcess.Failure("该入库单执行中或已完成");
            }

            InTaskRepository.UnitOfWork.TransactionEnabled = true;

            // 更新单据状态
            // 获取入库任务明细单
            List<InTaskMaterialDto> list = InTaskMaterialDtos.Where(a => a.InTaskCode == entity.Code).ToList();

            // 获取入库单
            var inEntity = InContract.InRepository.GetEntity(a => a.Code == entity.InCode);

            bool inFinishFlag = false;
            foreach (var item in list)
            {
                // 入库单行项目实体
                var inMaterialEntity =
                    InContract.InMaterialRepository.GetEntity(a => a.InCode == item.InCode && a.ItemNo == item.ItemNo);
                inMaterialEntity.SendInQuantity = inMaterialEntity.SendInQuantity - item.Quantity;

                //如果未存在下发情况
                if (inMaterialEntity.SendInQuantity == 0)
                {
                    inFinishFlag = true;
                    inMaterialEntity.Status = (int)InStatusCaption.WaitingForShelf;
                }
                else
                {
                    inFinishFlag = false;
                }
                // 更新入库单行项目
                if (InContract.InMaterialRepository.Update(inMaterialEntity) < 0)
                {
                    return DataProcess.Failure("任务删除，入库单行项目更新失败");
                }

                // 解除托盘的重量锁定
                //  托盘实体
                var trayEntity = WareHouseContract.TrayWeightMapRepository.GetEntity(a => a.TrayId == item.SuggestTrayId);
                trayEntity.TempLockWeight = trayEntity.TempLockWeight - item.Quantity * item.UnitWeight;

                // 更新托盘储位推荐锁定的重量
                if (WareHouseContract.TrayWeightMapRepository.Update(trayEntity) <= 0)
                {
                    return DataProcess.Failure(string.Format("托盘重量锁定失败"));
                }
            }


            if (inFinishFlag)
            {
                if (!InContract.InMaterials.Any(a => a.InCode == entity.InCode && a.Status != (int)InStatusCaption.WaitingForShelf))
                {
                    inEntity.Status = (int)InStatusCaption.WaitingForShelf;
                    if (InContract.InRepository.Update(inEntity) < 0)
                    {
                        return DataProcess.Failure("任务下发，入库单更新失败");
                    }
                }
            }

            // 删除入库任务
            if (InTaskRepository.Delete(id) <= 0)
            {
                return DataProcess.Failure(string.Format("入库单{0}删除失败", entity.Code));
            }

            if (list != null && list.Count > 0)
            {
                foreach (InTaskMaterial item in list)
                {
                    DataResult result = RemoveInTaskMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }

            InTaskRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }



        public DataResult RemoveInTaskMaterial(int id)
        {
            InTaskMaterial entity = InTaskMaterialRepository.GetEntity(id);
            if (entity.Status != (int)Enums.InStatusCaption.WaitingForShelf)
            {
                return DataProcess.Failure("该入库物料条码执行中或已完成");
            }
            if (InTaskMaterialRepository.Delete(id) > 0)
            {
                return DataProcess.Success(string.Format("入库条码{0}删除成功", entity.MaterialLabel));
            }
            return DataProcess.Failure("操作失败");
        }
        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult SendOrderToPTL(InTask entity)
        {
            try
            {
                //存在盘点计划 不允许发送

                //if (CheckContract.Checks.Any(a=>a.Status<6 && a.WareHouseCode==entity.WareHouseCode))
                //{
                //    return DataProcess.Failure("该仓库尚有盘点计划,此时不允许发送PTL任务");
                //}

                //if (entity.Status != (int)(Bussiness.Enums.InStatusCaption.WaitingForShelf))
                //{
                //    return DataProcess.Failure("入库单状态不对,应为待上架状态");
                //}

                ////if (Ins.Any(a => a.Status == (int)Bussiness.Enums.InStatusCaption.Sheling))
                ////{
                ////    return DataProcess.Failure("入库单已发送至PTL");
                ////}
                //InTaskRepository.UnitOfWork.TransactionEnabled = true;
                //entity.Status = (int)Bussiness.Enums.InStatusCaption.Sheling;
                //List<InMaterialDto> list = InTaskMaterialDtos.Where(a => a.InCode == entity.Code && a.Status == (int)Bussiness.Enums.InStatusCaption.WaitingForShelf).ToList();
                //List<InMaterial> list1 = InTaskMaterialDtos.Where(a => a.InCode == entity.Code && a.Status == (int)Bussiness.Enums.InStatusCaption.WaitingForShelf).ToList();

                //DpsInterfaceMain main = new DpsInterfaceMain();
                //main.ProofId = Guid.NewGuid().ToString();
                //main.CreateDate = DateTime.Now;
                //main.Status = 0;
                //main.OrderType = 0;
                //main.OrderCode = entity.Code;
                //if (InTaskRepository.Update(entity) < 0)
                //{
                //    return DataProcess.Failure("更新失败");
                //}
                //foreach (InMaterialDto item in list)
                //{
                //    InMaterial inMaterial = list1.FirstOrDefault(a => a.Id == item.Id);
                //    inMaterial.Status = (int)Bussiness.Enums.InStatusCaption.Sheling;

                //    DpsInterface dpsInterface = new DpsInterface();
                //    dpsInterface.BatchNO = item.BatchCode;
                //    dpsInterface.CreateDate = DateTime.Now;
                //    dpsInterface.GoodsName = item.MaterialName;
                //    dpsInterface.LocationId = item.LocationCode;
                //    dpsInterface.MakerName = item.SupplierName;
                //    dpsInterface.MaterialLabelId = 0;
                //    dpsInterface.ProofId = main.ProofId;
                //    dpsInterface.Quantity = Convert.ToInt32(item.Quantity);
                //    dpsInterface.RealQuantity = 0;
                //    dpsInterface.RelationId = item.Id;
                //    dpsInterface.Spec = item.MaterialUnit;
                //    dpsInterface.Status = 0;
                //    dpsInterface.OrderCode = item.InCode;
                //    dpsInterface.ToteId = item.MaterialLabel;
                //    if (DpsInterfaceRepository.Insert(dpsInterface) == false)
                //    {
                //        return DataProcess.Failure("发送任务至PTL失败");
                //    }
                //    if (InMaterialRepository.Update(inMaterial) < 0)
                //    {
                //        return DataProcess.Failure("更新失败");
                //    }
                //}
                //if (DpsInterfaceMainRepository.Insert(main)==false)
                //{
                //    return DataProcess.Failure("发送任务至PTL失败");
                //}
                //InTaskRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {

                return DataProcess.Failure("发送任务至PTL失败:"+ex.Message);
            }

            return DataProcess.Success("发送PTL成功");

        }
    }
}
