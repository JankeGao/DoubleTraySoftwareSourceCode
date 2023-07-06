using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Core.Mapping;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Contracts;

namespace Bussiness.Services
{
    public class WareHouseServer : Contracts.IWareHouseContract
    {
        public IIdentityContract IdentityContract { get; set; }
        public IMaterialContract MaterialContract { set; get; }

        public IRepository<Entitys.Channel, int> ChannelRepository { get; set; }

        public IBoxContract BoxContract { set; get; }
        public IRepository<Plan, int> WareHousePlanRepository { get; set; }
        public IEquipmentTypeContract EquipmentTypeContract { set; get; }
        public IRepository<WareHouse, int> WareHouseRepository { get; set; }

        public IRepository<TrayUserMap, int> TrayUserMapRepository { get; set; }

        public IRepository<TrayWeightMap, int> TrayWeightMapRepository { get; set; }

        public IQuery<TrayWeightMap> TrayWeightMaps => TrayWeightMapRepository.Query();

        public IRepository<Area, int> AreaRepository { get; set; }

        public IRepository<Container, int> ContainerRepository { get; set; }

        public IRepository<Tray, int> TrayRepository { get; set; }
        public IRepository<Location, int> LocationRepository { get; set; }

        public IQuery<WareHouse> WareHouses => WareHouseRepository.Query();

        public IQuery<Area> Areas => AreaRepository.Query();

        public IQuery<TrayUserMap> TrayUserMaps => TrayUserMapRepository.Query();

        public IQuery<Container> Containers => ContainerRepository.Query();


        public IQuery<Tray> Trays => TrayRepository.Query();

        public IQuery<Location> Locations => LocationRepository.Query();

        public IRepository<Entitys.LocationVIEW, int> LocationVIEWRepository { get; set; }
        public IQuery<LocationVIEW> LocationVIEWs => LocationVIEWRepository.Query();

        public IMapper Mapper { set; get; }

        public ISequenceContract SequenceContract { set; get; }

        public IDepartmentContract DepartmentContract { set; get; }
        public IQuery<Dtos.TrayDto> TrayDtos
        {
            get
            {
                return Trays.InnerJoin(Containers, (tray, container) => tray.ContainerCode == container.Code).Select((tray, container) => new TrayDto()
                {
                    Code = tray.Code,
                    BracketNumber = tray.BracketNumber,
                    BracketTrayNumber = tray.BracketTrayNumber,
                    ContainerType = container.ContainerType,
                    ContainerCode = tray .ContainerCode,
                    WareHouseCode= tray.WareHouseCode,
                    MaxWeight = tray.MaxWeight,
                    TrayWidth =tray.TrayWidth,
                    TrayLength =tray.TrayLength,
                    XNumber = tray.XNumber,
                    YNumber =tray.YNumber,
                    LayoutJson = tray.LayoutJson,
                    LockWeight =tray.LockWeight,
                    Id = tray.Id

                });
            }
        }

        /// <summary>
        /// 根据用户管理的权限，获取库存信息
        /// </summary>
        public IQuery<WareHouse> WareHouseAuthDtos
        {
            get
            {
                // 获取人员权限
                var query = WareHouses;
                var identity = HP.Core.Security.Permissions.IdentityManager.Identity.UserData;
                var currentUser = IdentityContract.Users.FirstOrDefault(a => a.Code == identity.Code);
                var userPower = DepartmentContract.Departments.Where(a => a.Code == identity.DeptId).FirstOrDefault();
                if (userPower != null && !identity.IsSystem)
                {
                    if (!string.IsNullOrEmpty(userPower.WareHouseManageCodes))
                    {
                        var wareHouseManage = userPower.WareHouseManageCodes.Split(',').ToList();
                        query = query.Where(a => wareHouseManage.Contains(a.Code));
                    }
                }

                return query;
            }
        }

        public IQuery<LocationVM> LocationVMs
        {
            get
            {
                return Locations.LeftJoin(MaterialContract.Materials, (location, material) => location.SuggestMaterialCode == material.Code)
                    .InnerJoin(WareHouses, (location, material, warehouse) => warehouse.Code == location.WareHouseCode)
                    .InnerJoin(TrayDtos, (location, material, warehouse, tray) => location.TrayId == tray.Id)
                    .LeftJoin(BoxContract.Box, (location, material, warehouse, tray, box) => location.BoxCode == box.Code)
                    .Select((location, material, warehouse, tray, box) => new LocationVM()
                    {
                        Id = location.Id,
                        IsLocked = location.IsLocked,
                        WareHouseCode = location.WareHouseCode,
                        TrayCode = tray.Code,
                        SuggestMaterialName = material.Name,
                        SuggestMaterialUnit = material.Unit,
                        Code = location.Code,
                        SuggestMaterialCode = location.SuggestMaterialCode,
                        LockMaterialCode = location.LockMaterialCode,
                        XLight = location.XLight,
                        Remark = location.Remark,
                        YLight = location.YLight,
                        LayoutId = location.LayoutId,
                        Enabled = location.Enabled,
                        CreatedTime = location.CreatedTime,
                        CreatedUserCode = location.CreatedUserCode,
                        CreatedUserName = location.CreatedUserName,
                        UpdatedTime = location.UpdatedTime,
                        UpdatedUserCode = location.UpdatedUserCode,
                        UpdatedUserName = location.UpdatedUserName,
                        ContainerCode = location.ContainerCode,
                        TrayId = location.TrayId,
                        BoxCode = location.BoxCode,
                        BoxName = box.Name,
                        BoxLength = box.BoxLength,
                        BoxUrl = box.PictureUrl,
                        BoxWidth = box.BoxWidth,
                        WarehouseName = warehouse.Name,
                        BracketNumber = tray.BracketNumber,
                        BracketTrayNumber = tray.BracketTrayNumber,
                        ContainerType = tray.ContainerType,
                        MinStockQuantity = material.MinNum,
                        MaxStockQuantity = material.MaxNum,
                        XLenght = location.XLenght
                    });
                    //.LeftJoin(StockRepository.Query(), (vm, stock) => vm.Code == stock.LocationCode)  .Select((vm, stock) => new LocationVM()
                    //{
                    //    Id = vm.Id,
                    //    IsLocked = vm.IsLocked,
                    //    WareHouseCode = vm.WareHouseCode,
                    //    TrayCode = vm.TrayCode,
                    //    SuggestMaterialName = vm.SuggestMaterialName,
                    //    SuggestMaterialUnit = vm.SuggestMaterialUnit,
                    //    Code = vm.Code,
                    //    SuggestMaterialCode = vm.SuggestMaterialCode,
                    //    LockMaterialCode = vm.LockMaterialCode,
                    //    XLight = vm.XLight,
                    //    Remark = vm.Remark,
                    //    YLight = vm.YLight,
                    //    LayoutId = vm.LayoutId,
                    //    Enabled = vm.Enabled,
                    //    CreatedTime = vm.CreatedTime,
                    //    CreatedUserCode = vm.CreatedUserCode,
                    //    CreatedUserName = vm.CreatedUserName,
                    //    UpdatedTime = vm.UpdatedTime,
                    //    UpdatedUserCode = vm.UpdatedUserCode,
                    //    UpdatedUserName = vm.UpdatedUserName,
                    //    ContainerCode = vm.ContainerCode,
                    //    TrayId = vm.TrayId,
                    //    BoxCode = vm.BoxCode,
                    //    BoxName = vm.BoxName,
                    //    BoxLength = vm.BoxLength,
                    //    BoxUrl = vm.BoxUrl,
                    //    BoxWidth = vm.BoxWidth,
                    //    WarehouseName = vm.WarehouseName,
                    //    BracketNumber = vm.BracketNumber,
                    //    BracketTrayNumber = vm.BracketTrayNumber,
                    //    ContainerType = vm.ContainerType,
                    //    MinStockQuantity = vm.MinStockQuantity,
                    //    MaxStockQuantity = vm.MaxStockQuantity,
                    //    StockQuantity = stock.Quantity
                    //});
            }
        }

        /// <summary>
        /// 货柜数据
        /// </summary>
        public IQuery<ContainerDto> ContainerDtos
        {
            get
            {
                return Containers.LeftJoin(EquipmentTypeContract.EquipmentType, (container, equipmentType) => container.EquipmentCode == equipmentType.Code)
                    .Select((container, equipmentType) => new ContainerDto()
                    {
                        Id = container.Id,
                        Code=container.Code,
                        EquipmentCode=container.EquipmentCode,
                        WareHouseCode=container.WareHouseCode,
                        Brand=equipmentType.Brand,
                        EquipmentType=equipmentType.Type,
                        Ip=container.Ip,
                        Port=container.Port,
                        UID=container.UID,
                        Status= container.Status,
                        Remark= container.Remark,
                        AlarmStatus = container.AlarmStatus,
                        PictureUrl =equipmentType.PictureUrl,
                        CreatedTime = container.CreatedTime,
                        CreatedUserCode = container.CreatedUserCode,
                        CreatedUserName = container.CreatedUserName,
                        UpdatedTime = container.UpdatedTime,
                        UpdatedUserCode = container.UpdatedUserCode,
                        UpdatedUserName = container.UpdatedUserName,
                    });
            }
        }
        /// <summary>
        /// 获取货柜权限
        /// </summary>
        public IQuery<TrayUserDto> TrayUserDtos
        {
            get
            {
                return TrayUserMaps.InnerJoin(IdentityContract.Users, (map, user) => map.UserCode == user.Code)
                    .Select((map, user) => new TrayUserDto()
                    {
                        Id = map.Id,
                        UserCode = map.UserCode,
                        UserName= user.Name,
                        TrayId=map.TrayId,
                        WareHouseCode=map.WareHouseCode,
                        ContainerCode=map.ContainerCode
                    });
            }
        }

        public IRepository<Entitys.Stock, int> StockRepository { get; set; }


        #region 仓库管理

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateWareHouse(WareHouse entity)
        {
            if (WareHouses.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure(string.Format("仓库{0}已存在", entity.Code));
            }
            if (WareHouseRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("仓库{0}创建成功", entity.Code));
            }
            return DataProcess.Failure("操作失败");
        }

        /// <summary>
        /// 编辑仓库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditWareHouse(WareHouse entity)
        {
            // 根据id获取仓库
            var warehouse = WareHouseRepository.GetEntity(entity.Id);
            if (warehouse == null)
            {
                return DataProcess.Failure($"仓库：{entity.Code}不存在！");
            }
            if (string.IsNullOrEmpty(entity.Code) || string.IsNullOrEmpty(entity.Name))
            {
                return DataProcess.Failure("请检查数据，仓库编码、名称不能为空！");
            }
            if (!warehouse.Code.Equals(entity.Code))
            {
                return DataProcess.Failure("仓库编码不可修改");
            }
            // 判断是否修改编码，且修改后的编码已在数据库中存在
            //if (!warehouse.Code.Equals(entity.Code) && WareHouses.Any(a => a.Id != warehouse.Id && a.Code == entity.Code))
            //{
            //    return DataProcess.Failure($"仓库编码{entity.Code}已存在，请勿重复设置！");
            //}
            // 更新仓库信息
            if (WareHouseRepository.Update(entity) > 0)
            {
                return DataProcess.Success();
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 移除仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveWareHouse(int id)
        {
            WareHouse entity = WareHouseRepository.GetEntity(id);
            if (StockRepository.Query().Any(a => a.WareHouseCode == entity.Code))
            {
                return DataProcess.Success(string.Format("仓库{0}尚有库存,无法删除", entity.Code));
            }
            WareHouseRepository.UnitOfWork.TransactionEnabled = true;
            List<Container> containerList = Containers.Where(a => a.WareHouseCode == entity.Code).ToList();
            if (containerList != null && containerList.Count() > 0)
            {
                foreach (Container item in containerList)
                {
                    DataResult result = RemoveContainer(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }

                }
            }
            if (WareHouseRepository.Delete(id) > 0)
            {
                WareHouseRepository.UnitOfWork.Commit();
                return DataProcess.Success(string.Format("仓库{0}删除成功", entity.Code));
            }
            return DataProcess.Failure("操作失败");
        }

        #endregion


        #region 货柜管理

        /// <summary>
        /// 创建货柜
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult CreateContainer(ContainerDto entityDto)
        {
            ContainerRepository.UnitOfWork.TransactionEnabled = true;
            // 添加货柜
            // 实体映射
            Container entity = Mapper.MapTo<Container>(entityDto);
            entity.UID = Guid.NewGuid().ToString();
            entity.Status = (int) DeviceStatusEnum.None;
            entity.AlarmStatus=(int)DeviceAlarmStateEnum.Reach;

            var EquipmentType = this.EquipmentTypeContract.EquipmentTypeRepository.Query().FirstOrDefault(a => a.Code == entity.EquipmentCode);
            if (EquipmentType==null)
            {
                return DataProcess.Failure("未选择设备型号");
            }
            if (EquipmentType.Brand==(int)Bussiness.Enums.EquipmentEnumBrand.NamgyaiVoluntarily)
            {
                if (EquipmentType.Type==(int)Bussiness.Enums.EquipmentTypeEnum.GoUpDecline)
                {
                    entity.ContainerType = 3;
                }
                else
                {
                    entity.ContainerType = 0;
                }
            }
            if (EquipmentType.Brand == (int)Bussiness.Enums.EquipmentEnumBrand.Hanel)
            {
                entity.ContainerType = 2;
            }
            if (EquipmentType.Brand == (int)Bussiness.Enums.EquipmentEnumBrand.Kardex)
            {
                entity.ContainerType = 1;
            }
            if (Containers.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure(string.Format("货柜编号{0}在仓库中已存在", entity.Code));
            }
            if (!ContainerRepository.Insert(entity))
            {
                return DataProcess.Failure(string.Format("货柜{0}创建失败", entity.Code));
            }

            // 添加托盘
            if (entityDto.TrayNumber > 0)
            {
                for (int i = 0; i < entityDto.TrayNumber; i++)
                {
                    var trayEntity = new Tray
                    {
                        Code = (i + 1).ToString(),
                        ContainerCode = entityDto.Code,
                        TrayLength = entityDto.TrayLength,
                        TrayWidth = entityDto.TrayWidth,
                        WareHouseCode = entityDto.WareHouseCode,
                        XNumber = entityDto.XNumber,
                        YNumber = entityDto.YNumber,
                        MaxWeight = entityDto.MaxWeight,
                        BracketNumber = 1,
                        BracketTrayNumber = i + 1
                    };
                    if (!TrayRepository.Insert(trayEntity))
                    {
                        return DataProcess.Failure(string.Format("托盘{0}创建失败", trayEntity.Code));
                    }

                    var temp = TrayRepository.GetEntity(a =>
                        a.WareHouseCode == trayEntity.WareHouseCode && a.ContainerCode == trayEntity.ContainerCode && a.Code ==
                            trayEntity.Code);
                    // 添加托盘称重映射
                    var trayWeight = new TrayWeightMap()
                    {
                        TrayId = temp.Id,
                        MaxWeight = temp.MaxWeight,
                        LockWeight = 0,
                        TempLockWeight = 0
                    };
                    if (!TrayWeightMapRepository.Insert(trayWeight))
                    {
                        return DataProcess.Failure(string.Format("添加托盘重量锁定失败"));
                    }
                }
            }

            ContainerRepository.UnitOfWork.Commit();
            return DataProcess.Success(string.Format("货柜{0}创建成功", entity.Code));

        }


        /// <summary>
        /// 编辑货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditContainer(Container entity)
        {
            var current = this.ContainerRepository.GetEntity(entity.Id);
            entity.ContainerType = current.ContainerType;
            if (ContainerRepository.Update(entity) > 0)
            {
                return DataProcess.Success(string.Format("货柜{0}编辑成功", entity.Code));
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 移除货柜
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveContainer(int id)
        {
            Container entity = ContainerRepository.GetEntity(id);
            if (StockRepository.Query().Any(a => a.ContainerCode == entity.Code))
            {
                return DataProcess.Success(string.Format("货柜{0}尚有库存,无法删除", entity.Code));
            }
            ContainerRepository.UnitOfWork.TransactionEnabled = true;


            List<Tray> trays = TrayRepository.Query().Where(a => a.WareHouseCode == entity.WareHouseCode && a.ContainerCode == entity.Code).ToList();
            if (trays != null)
            {
                foreach (var item in trays)
                {
                    DataResult result = RemoveTray(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }
            if (ContainerRepository.Delete(id) > 0)
            {
                ContainerRepository.UnitOfWork.Commit();
                return DataProcess.Success(string.Format("货柜{0}删除成功", entity.Code));
            }
            return DataProcess.Failure("操作失败");
        }

        #endregion


        #region 托盘管理

        /// <summary>
        /// 创建托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateTray(Tray entity)
        {
            TrayRepository.UnitOfWork.TransactionEnabled = true;
            //货柜编码
            if (entity.ContainerCode.IsNullOrEmpty())
            {
                return DataProcess.Failure(string.Format("未获取到该货柜编码"));
            }

            
            if (entity.ContainerCode.IsNullOrEmpty())
            {
                if (Trays.Any(a => a.ContainerCode == entity.ContainerCode))
                {
                    var trayCode = Trays.Where(a => a.ContainerCode == entity.ContainerCode).OrderByDesc(a => a.Code).FirstOrDefault().Code;
                    entity.Code = (Convert.ToInt32(trayCode) + 1).ToString();
                }
                else
                {
                    entity.Code = 1.ToString();
                }
            }
            if (Trays.Any(a => a.ContainerCode == entity.ContainerCode&&a.Code==entity.Code))
            {
                return DataProcess.Failure(string.Format("该货柜下已存在该编号托盘！"));
            }

            if (Trays.Any(a => a.Code == entity.Code && a.ContainerCode == entity.ContainerCode))
            {
                return DataProcess.Failure(string.Format("托盘{0}已存在", entity.Code));
            }
            if (!TrayRepository.Insert(entity))
            {
                return DataProcess.Failure("托盘创建失败");
            }

            var temp = TrayRepository.GetEntity(a => a.WareHouseCode == entity.WareHouseCode && a.ContainerCode == entity.ContainerCode && a.Code == entity.Code);
            // 添加托盘称重映射
            var trayWeight = new TrayWeightMap()
            {
                TrayId = temp.Id,
                MaxWeight = temp.MaxWeight,
                LockWeight = 0,
                TempLockWeight = 0
            };
            if (!TrayWeightMapRepository.Insert(trayWeight))
            {
                return DataProcess.Failure(string.Format("添加托盘重量锁定失败"));
            }
            TrayRepository.UnitOfWork.Commit();
            return DataProcess.Success(string.Format("托盘{0}创建成功", entity.Code));
        }

        /// <summary>
        /// 编辑托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditTray(Tray entity)
        {
            TrayRepository.UnitOfWork.TransactionEnabled = true;

            var temp = TrayWeightMapRepository.GetEntity(a => a.TrayId == entity.Id);
            entity.MaxWeight = entity.MaxWeight * 1000;  // g与kg的单位转换
            // 添加托盘称重映射
            var trayWeight = new TrayWeightMap()
            {
                TrayId = temp.Id,
                MaxWeight = entity.MaxWeight,
            };
            if (TrayWeightMapRepository.Update(trayWeight)<0)
            {
                return DataProcess.Failure(string.Format("更新托盘重量锁定失败"));
            }
            if (TrayRepository.Update(entity) < 0)
            {
                return DataProcess.Failure(string.Format("托盘{0}编辑失败", entity.Code));
            }

            TrayRepository.UnitOfWork.Commit();
            return DataProcess.Success(string.Format("托盘{0}编辑成功", entity.Code));
        }

        /// <summary>
        /// 移除托盘
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveTray(int id)
        {
            Tray entity = TrayRepository.GetEntity(id);
            TrayRepository.UnitOfWork.TransactionEnabled = true;
            List<Location> locationList = Locations.Where(a => a.ContainerCode == entity.ContainerCode && a.WareHouseCode == entity.WareHouseCode && a.TrayId == id).ToList();

            if (locationList != null && locationList.Count() > 0)
            {
                foreach (Location item in locationList)
                {
                    DataResult result = RemoveLocation(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }

            var trayWeights = TrayWeightMaps.FirstOrDefault(a => a.TrayId == id);
            if (TrayWeightMapRepository.Delete(trayWeights.Id) < 0)
            {
                TrayRepository.UnitOfWork.Commit();
                return DataProcess.Failure(string.Format("托盘称重{0}删除失败", entity.Code));
            }

            // 删除托盘人员权限
            List<TrayUserMap> trayUserMapList = TrayUserMaps.Where(a => a.ContainerCode == entity.ContainerCode && a.WareHouseCode == entity.WareHouseCode && a.TrayId == id).ToList();
            if (trayUserMapList != null && trayUserMapList.Count() > 0)
            {
                foreach (TrayUserMap item in trayUserMapList)
                {
                    if (TrayUserMapRepository.Delete(item.Id) < 0)
                    {
                        return DataProcess.Failure(string.Format("托盘人员权限{0}删除失败", entity.Code));
                    }
                }
            }

            if (TrayRepository.Delete(id) > 0)
            {
                TrayRepository.UnitOfWork.Commit();
                return DataProcess.Success(string.Format("托盘{0}删除成功", entity.Code));
            }
            return DataProcess.Failure("操作失败");
        }

        /// <summary>
        /// 批量维护托盘权限
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult AddBatchTrayUserMap(TrayUserMap entity)
        {
            try
            {
                TrayUserMapRepository.UnitOfWork.TransactionEnabled = true;

                // 货柜批量维护，全部托盘
                if (entity.TrayId <= 0)
                {
                    if (TrayUserMaps.Any(p => p.ContainerCode == entity.ContainerCode))
                    {
                        if (TrayUserMapRepository.Delete(p => p.ContainerCode == entity.ContainerCode) == 0)
                        {
                            return DataProcess.Failure("货柜({0})权限更新失败！".FormatWith(entity.ContainerCode));
                        }
                    }
                    var trayList = Trays.Where(a => a.WareHouseCode == entity.WareHouseCode && a.ContainerCode == entity.ContainerCode).ToList();
                    foreach (var item in trayList)
                    {                //添加托盘用户映射数据
                        if (!entity.UserCode.IsNullOrEmpty())
                        {
                            foreach (string userCode in entity.UserCode.FromJsonString<string[]>())
                            {
                                if (!TrayUserMapRepository.Insert(new TrayUserMap()
                                {
                                    UserCode = userCode,
                                    ContainerCode = entity.ContainerCode,
                                    WareHouseCode = entity.WareHouseCode,
                                    TrayId = item.Id
                                }))
                                {
                                    return DataProcess.Failure("区域({0})白名单设置失败！".FormatWith(entity.ContainerCode));
                                }
                            }
                        }
                    }
                }
                else // 单个托盘
                {
                    if (TrayUserMaps.Any(p => p.ContainerCode == entity.ContainerCode && p.TrayId == entity.TrayId))
                    {
                        if (TrayUserMapRepository.Delete(p => p.ContainerCode == entity.ContainerCode && p.TrayId == entity.TrayId) == 0)
                        {
                            return DataProcess.Failure("货柜({0})权限更新失败！".FormatWith(entity.ContainerCode));
                        }
                    }
                    if (!entity.UserCode.IsNullOrEmpty())
                    {
                        foreach (string userCode in entity.UserCode.FromJsonString<string[]>())
                        {
                            if (!TrayUserMapRepository.Insert(new TrayUserMap()
                            {
                                UserCode = userCode,
                                ContainerCode = entity.ContainerCode,
                                WareHouseCode = entity.WareHouseCode,
                                TrayId = entity.TrayId,
                            }))
                            {
                                return DataProcess.Failure("区域({0})白名单设置失败！".FormatWith(entity.ContainerCode));
                            }
                        }
                    }
                }


                TrayUserMapRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
            }


            return DataProcess.Success("白名单设置成功！");
        }

        #endregion


        #region 储位管理

        /// <summary>
        /// 维护托盘储位信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditTrayLocation(Tray entity)
        {
            List<Bussiness.Entitys.Location> list = entity.LocationList.FromJsonString<List<Location>>();

            if (list.Count > 0)
            {
                LocationRepository.UnitOfWork.TransactionEnabled = true;
                foreach (var item in list)
                {
                    item.IsLocked = false;
                    item.TrayCode = entity.Code;
                    var result = CreateLocation(item);
                    if (!result.Success)
                    {
                        return result;
                    }
                }
                if (TrayRepository.Update(entity) < 0)
                {
                    return DataProcess.Failure(string.Format("托盘{0}编辑成功", entity.Code));
                }
                LocationRepository.UnitOfWork.Commit();
            }
            return DataProcess.Success("批量添加库位成功");

        }


        /// <summary>
        /// 维护储位
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateLocation(Location entity)
        {
            entity.XLight = entity.XLight == 0 ? 1 : entity.XLight;
            entity.YLight = entity.YLight == 0 ? 1 : entity.YLight;
            entity.XLenght = entity.XLenght.GetValueOrDefault(0) == 0 ? 1 : entity.XLenght;

            LocationRepository.Delete(a => a.LayoutId == entity.LayoutId);
            // 如果已存在当前库位
            if (Locations.Any(a => a.LayoutId == entity.LayoutId))
            {
                var updateEntity = Locations.FirstOrDefault(a => a.LayoutId == entity.LayoutId);
                updateEntity.XLight = entity.XLight - entity.XLenght.GetValueOrDefault(0)+1;
                updateEntity.BoxCode = entity.BoxCode;
                updateEntity.SuggestMaterialCode = entity.SuggestMaterialCode;
                updateEntity.WareHouseCode = entity.WareHouseCode;
                updateEntity.TrayId = entity.TrayId;
                updateEntity.IsLocked = entity.IsLocked;
                updateEntity.ContainerCode = entity.ContainerCode;
                updateEntity.Enabled = entity.Enabled;
                updateEntity.XLenght = entity.XLenght;


         //       updateEntity.Code = /*"S" + DateTime.Now.ToString("yyyyMMdd") + "-"+*/ /*"LJ"+entity.ContainerCode+"-" +*/ entity.ContainerCode + "-" + entity.TrayCode.PadLeft(2, '0') + "-" + entity.XLight.ToString().PadLeft(2, '0') + "-" + entity.YLight.ToString().PadLeft(2, '0');
         // 更新库位信息
                if (LocationRepository.Update(updateEntity) < 0)
                {
                    return DataProcess.Failure(string.Format("库位{0}维护失败", entity.Code));
                }
            }
            else
            {
                // 创建库位码
                //entity.Code = /*"S" + DateTime.Now.ToString("yyyyMMdd") + "-"+*/entity.TrayCode.PadLeft(2, '0') + "-"+entity.XLight.ToString().PadLeft(2, '0') + "-"+entity.YLight.ToString().PadLeft(2, '0');  //SequenceContract.Create(entity.GetType());
                entity.XLight= entity.XLight - entity.XLenght.GetValueOrDefault(0) + 1;
                // entity.Code = SequenceContract.Create(entity.GetType());
               
                entity.Code = /*"S" + DateTime.Now.ToString("yyyyMMdd") + "-"+*/ /*"LJ"+entity.ContainerCode+"-" +*/ entity.ContainerCode + "-" + entity.TrayCode.PadLeft(2, '0') + "-" + entity.XLight.ToString().PadLeft(2, '0') + "-" + entity.YLight.ToString().PadLeft(2, '0');  //SequenceContract.Create(entity.GetType());
                // 插入库位信息
                if (!LocationRepository.Insert(entity))
                {
                    return DataProcess.Failure(string.Format("库位{0}维护失败", entity.Code));
                }
            }

            return DataProcess.Success(string.Format("库位{0}维护成功", entity.Code));
        }


        /// <summary>
        /// 移除库位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveLocation(int id)
        {
            Location entity = LocationRepository.GetEntity(id);
            if (StockRepository.Query().Any(a => a.LocationCode == entity.Code))
            {
                return DataProcess.Success(string.Format("库位{0}尚有库存,无法删除", entity.Code));
            }
            if (LocationRepository.Delete(id) > 0)
            {
                return DataProcess.Success(string.Format("库位{0}删除成功", entity.Code));
            }

            return DataProcess.Failure("操作失败");
        }


        /// <summary>
        /// 移除库位-根据layoutId
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        public DataResult RemoveLocationByLayoutId(string layoutId)
        {
            Location entity = Locations.FirstOrDefault(a => a.LayoutId == layoutId);
            if (entity == null)
            {
                return DataProcess.Success(string.Format("库位{0}未维护图像视图", layoutId));
            }
            if (StockRepository.Query().Any(a => a.LocationCode == entity.Code))
            {
                return DataProcess.Success(string.Format("库位{0}尚有库存,无法删除", entity.Code));
            }
            if (LocationRepository.Delete(entity.Id) > 0)
            {
                return DataProcess.Success(string.Format("库位{0}删除成功", entity.Code));
            }

            return DataProcess.Failure("操作失败");
        }

        public DataResult RemoveLocationByTrayId(Tray entity)
        {
            if (entity == null)
            {
                return DataProcess.Failure("请选择一个托盘！");
            }
            var tray = TrayRepository.GetEntity(entity.Id);
            if (tray == null)
            {
                return DataProcess.Failure($"托盘：{entity.Code}不存在！");
            }
            var deleteLocations = Locations.Where(a => a.TrayId == tray.Id).ToList();
            if (deleteLocations.Count < 1)
            {
                return DataProcess.Failure("当前托盘暂未维护库位，无法删除！");
            }
            var deleteLocationCodes = deleteLocations.Select(a => a.Code).Distinct().ToList();
            // 判断需要删除的库位中是否还有库存
            if (StockRepository.Query().Any(a => deleteLocationCodes.Contains(a.LocationCode)))
            {
                return DataProcess.Failure("托盘中的库位还有库存，暂时无法删除！");
            }
            LocationRepository.UnitOfWork.TransactionEnabled = true;
            if (LocationRepository.Delete(a => a.TrayId == tray.Id) < 1)
            {
                return DataProcess.Failure("删除失败！");
            }
            tray.LayoutJson = "";
            TrayRepository.Update(tray);
            LocationRepository.UnitOfWork.Commit();
            return DataProcess.Success();
        }
        #endregion

        #region 客户端方法

        /// <summary>
        /// 移除库位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult CheckUID(string code,string uid)
        {
            if (Containers.Any(a => a.UID == uid && a.Code == code))
            {
                return DataProcess.Success();
            }

            return DataProcess.Failure("未生成该货柜客户端");
        }

        #endregion
    }
}
