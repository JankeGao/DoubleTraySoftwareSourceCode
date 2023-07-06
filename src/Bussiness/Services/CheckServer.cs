using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bussiness.Services
{
    public class CheckServer : ICheckContract
    {
        public IRepository<CheckMain, int> CheckRepository { get; set; }

        public IRepository<CheckDetail, int> CheckDetailRepository { get; set; }

        public IRepository<CheckArea, int> CheckAreaRepository { get; set; }

        public IQuery<CheckMain> Checks => CheckRepository.Query();

        public IRepository<HPC.BaseService.Models.Dictionary, int> DictionaryRepository { get; set; }

        public IRepository<Bussiness.Entitys.WareHouse, int> WareHouseRepository { get; set; }
        public IRepository<Bussiness.Entitys.Material, int> MaterialRepository { get; set; }
        public IRepository<Bussiness.Entitys.Stock, int> StockRepository { get; set; }
        public IRepository<Bussiness.Entitys.Out, int> OutRepository { get; set; }

        public IRepository<Bussiness.Entitys.In, int> InRepository { get; set; }
        public IRepository<Bussiness.Entitys.Location, int> LocationRepository { get; set; }
        public ISupplyContract SupplyContract { set; get; }
        public IWareHouseContract WareHouseContract { get; set; }
        public ISequenceContract SequenceContract { set; get; }

        /// <summary>
        /// 标签契约
        /// </summary>
        public ILabelContract LabelContract { set; get; }
        public IMaterialContract MaterialContract { get; set; }
        public IRepository<CheckListDetail, int> CheckListDetailRepository { get; set; }
        public IRepository<CheckList, int> CheckListRepository { get; set; }
        public ICheckListContract CheckListContract { set; get; }

        public Bussiness.Contracts.IEquipmentTypeContract EquipmentTypeContract { set; get; }
        public IMapper Mapper { set; get; }

        public IQuery<CheckDto> CheckDtos => 
            Checks.LeftJoin(DictionaryRepository.Query(), (check, dic) => check.CheckDict == dic.Code).InnerJoin(WareHouseRepository.Query(), (check, dic, warehouse) => check.WareHouseCode == warehouse.Code).InnerJoin(WareHouseContract.Containers, (check, dic, warehouse, containers) => check.ContainerCode == containers.Code).InnerJoin(EquipmentTypeContract.EquipmentType, (check, dic, warehouse, containers, equipmentType) => containers.EquipmentCode == equipmentType.Code).Select((check, dic, warehouse, containers, equipmentType) => new CheckDto
        {
            Id = check.Id,
            Code = check.Code,
            CheckListCode = check.CheckListCode,
            CheckDict = check.CheckDict,
            CheckType = check.CheckType,
            CheckDictDescription = dic.Name,
            ContainerCode = check.ContainerCode,
            WareHouseCode = check.WareHouseCode,
            WareHouseName = warehouse.Name,
            IsDeleted = check.IsDeleted,
            StartTime = check.StartTime,
            EndTime = check.EndTime,
            CreatedTime = check.CreatedTime,
            CreatedUserCode = check.CreatedUserCode,
            CreatedUserName = check.CreatedUserName,
            UpdatedTime = check.UpdatedTime,
            UpdatedUserCode = check.UpdatedUserCode,
            UpdatedUserName = check.UpdatedUserName,
            PictureUrl = equipmentType.PictureUrl,
            CheckLocationCode = null,
            Status = check.Status,
            Remark = check.Remark
        });

        public IQuery<CheckDetail> CheckDetails => CheckDetailRepository.Query();

        public IQuery<CheckListDetail> CheckListDetails => CheckListDetailRepository.Query();

        public IQuery<CheckDetailDto> CheckDetailDtos => CheckDetails.InnerJoin(MaterialRepository.Query(), (detail, material) => detail.MaterialCode == material.Code)
            .LeftJoin(WareHouseContract.WareHouses, (detail, material, warehouse) => detail.WareHouseCode == warehouse.Code)
            .LeftJoin(SupplyContract.Supplys, (detail, material, warehouse, supply) => detail.SupplierCode == supply.Code)
            .LeftJoin(WareHouseContract.LocationVMs, (detail, material, warehouse, supply, Locations) => detail.LocationCode == Locations.Code)
            .Select((detail, material, warehouse, supply, Locations) => new CheckDetailDto
            {
                Id= detail.Id,
                CheckCode = detail.CheckCode,
                IsDeleted = detail.IsDeleted,
                Quantity = detail.Quantity,
                CheckedQuantity = detail.CheckedQuantity,
                MaterialCode = detail.MaterialCode,
                WareHouseCode = detail.WareHouseCode,
                AreaCode= detail.AreaCode,
                LocationCode = detail.LocationCode,
                MaterialLabel = detail.MaterialLabel,
                MaterialName = material.Name,
                BatchCode = detail.BatchCode,
                CreatedTime = detail.CreatedTime,
                CreatedUserCode = detail.CreatedUserCode,
                CreatedUserName =detail.CreatedUserName,
                UpdatedTime = detail.UpdatedTime,
                UpdatedUserCode = detail.UpdatedUserCode,
                UpdatedUserName=detail.UpdatedUserName,
                Status = detail.Status,
                CheckedTime = detail.CheckedTime,
                Checker =detail.Checker,
                ContainerCode = Locations.ContainerCode,
                ManufactureDate =detail.ManufactureDate,
                MaterialUnit =material.Unit,
                WareHouseName = warehouse.Name,
                SupplierName =supply.Name,
                TrayId = Locations.TrayId,
                TrayCode= Locations.TrayCode
            });
        /// <summary>
        /// 复盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CheckAgain(CheckMain entity)
        {
            CheckRepository.UnitOfWork.TransactionEnabled = true;
            if (entity.Status!=(int)Bussiness.Enums.CheckStatusCaption.Checked)
            {
                return DataProcess.Failure("该盘点单尚未盘点完成");
            }

            entity.Status = (int)Bussiness.Enums.CheckStatusCaption.CheckAgagin;

            //var list = CheckDetails.Where(a => a.CheckCode == entity.Code).ToList();
            //foreach (var item in list)
            //{
            //    item.Status = (int)Bussiness.Enums.CheckStatusCaption.CheckAgagin;
            //    CheckDetailRepository.Update(item);
            //}
            var areaList = this.CheckAreaRepository.Query().Where(a => a.CheckCode == entity.Code).ToList();
            foreach (var item in areaList)
            {
   
            }
            CheckAreaRepository.Update(a => new Bussiness.Entitys.CheckArea() { Status = (int)Bussiness.Enums.CheckStatusCaption.CheckAgagin }, p => p.CheckCode == entity.Code);
            CheckDetailRepository.Update(a => new Bussiness.Entitys.CheckDetail() { Status = (int)Bussiness.Enums.CheckStatusCaption.CheckAgagin,CheckedQuantity=0 }, p => p.CheckCode == entity.Code);
            CheckRepository.Update(entity);


            // 更改盘点单的状态
            var checkLists = CheckListContract.CheckLists.Where(a => a.Code == entity.CheckListCode).ToList();
            var checkListDetails = CheckListContract.CheckListDetails.Where(a => a.CheckCode == entity.CheckListCode).ToList();
            foreach (var item in checkLists)  // 更改盘点单状态
            {
                item.Status = (int)CheckListStatusEnum.AnewCheck;
                CheckListRepository.Update(item);
            }
            foreach(var item in checkListDetails)  // 更改盘点单明细状态
            {
                item.Status = (int)CheckListStatusEnum.AnewCheck;
                CheckListDetailRepository.Update(item);
            }

            CheckRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");

        }

        public DataResult CreateCheckDetailEntity(CheckDetail entity)
        {
            if (!CheckDetailRepository.Insert(entity))
            {
                return DataProcess.Failure("新增盘点条码失败");
            }
            return DataProcess.Success();
        }

        public DataResult CreateCheckEntity(CheckMain entity)
        {

            //1判断盘点单号是否存在 存在则提示
            if (string.IsNullOrEmpty(entity.Code))
            {
                entity.Code = SequenceContract.Create("InventoryCode");
            }
            if (Checks.Any(a=>a.Code==entity.Code))
            {
                return DataProcess.Failure("盘点任务号已存在");
            }
            if (string.IsNullOrEmpty(entity.WareHouseCode))
            {
                return DataProcess.Failure("未选择盘点仓库");
            }
            //if (entity.AreaCodes==null || entity.AreaCodes.Count==0)
            //{
            //    return DataProcess.Failure("未选择盘点区域");
            //}
            //2查找是否有正在作业的出库单
            List<CheckArea> areas = new List<CheckArea>();
            foreach (var item in entity.AreaCodes)
            {
                if (CheckAreaRepository.Query().FirstOrDefault(a=>(a.Status<6) && a.WareHouseCode==entity.WareHouseCode && a.AreaCode== item.Code) !=null)
                {
                    return DataProcess.Failure("区域" + item + "尚有正在作业的盘点单");
                }
                CheckArea area = new CheckArea();
                area.AreaCode = item.Code;
                area.CheckCode = entity.Code;
                area.Status = 0;
                area.WareHouseCode = entity.WareHouseCode;
                areas.Add(area);
            }
            //if (OutRepository.Query().Any(a=>a.WareHouseCode==entity.WareHouseCode && a.Status!=(int)Bussiness.Enums.OutStatusCaption.WaitingForPicking && a.Status!=(int)Bussiness.Enums.OutStatusCaption.Finished))
            //{
            //    return DataProcess.Failure("该仓库有正在作业的出库单");
            //}
            //3 查找 是否有正在作业的 入库单

            if (InRepository.Query().Any(a => a.WareHouseCode == entity.WareHouseCode && a.Status != (int)Bussiness.Enums.InStatusCaption.WaitingForShelf && a.Status != (int)Bussiness.Enums.InStatusCaption.Finished))
            {
                return DataProcess.Failure("该仓库有正在作业的入库单");
            }
            CheckRepository.UnitOfWork.TransactionEnabled = true;
            //查找该仓库库存
            var list = StockRepository.Query().Where(a => a.WareHouseCode == entity.WareHouseCode).ToList();
            var insertList = new List<Bussiness.Entitys.CheckDetail>();
            foreach (var item in list)
            {
                item.IsCheckLocked = true;

                Entitys.CheckDetail detail = new CheckDetail();
                detail.WareHouseCode = item.WareHouseCode;
                detail.AreaCode = item.AreaCode;
                detail.BatchCode = item.BatchCode;
                detail.CheckCode = entity.Code;
                detail.CheckedQuantity = 0;
                detail.IsDeleted = false;
                detail.LocationCode = item.LocationCode;
                detail.MaterialCode = item.MaterialCode;
                detail.MaterialLabel = item.MaterialLabel;
                detail.Quantity = Convert.ToInt32(item.Quantity);
                detail.Status = 0;
                detail.ManufactureDate = item.ManufactureDate;
                detail.SupplierCode = item.SupplierCode;
                insertList.Add(detail);
                StockRepository.Update(item);
            }
            if (insertList.Count()==0)
            {
                return DataProcess.Failure("该仓库没有库存");
            }

            if (!CheckRepository.Insert(entity))
            {
                return DataProcess.Failure("新增盘点单失败");
            }

            foreach (var item in areas)
            {
                var isexistList = insertList.FindAll(a => a.AreaCode == item.AreaCode);
                if (isexistList == null || isexistList.Count()==0)
                {
                    item.Status = 5;//盘点完成
                }
                if (!CreateCheckArea(item).Success)
                {
                    return DataProcess.Failure("新增盘点单失败");
                }
            }
            foreach (var item in insertList)
            {
                if (!CreateCheckDetailEntity(item).Success)
                {
                    return DataProcess.Failure("新增盘点单失败");
                }
            }
            CheckRepository.UnitOfWork.Commit();
            return DataProcess.Success("新增盘点单成功");
        }

        
        /// <summary>
        /// 新增盘点单任务下发--新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateCheckListEntity(CheckList entity)
        {
            //1判断盘点单号是否存在 存在则提示
            if (string.IsNullOrEmpty(entity.WareHouseCode))
            {
                return DataProcess.Failure("未选择盘点仓库");
            }

            // 盘点任务实体
            var checkItem = new CheckMain();

            // 获取盘点单下的明细
            var CheckListDetailList = CheckListDetailRepository.Query().Where(a => a.CheckCode == entity.Code).ToList();

            // 判断盘点的没有内容--创建空的盘点任务
            if(CheckListDetailList.Count <= 0)
            {
                checkItem.IsDeleted = false;
                checkItem.CheckType = entity.CheckDict;
                checkItem.WareHouseCode = entity.WareHouseCode;
                checkItem.StartTime = entity.StartTime;
                checkItem.EndTime = entity.EndTime;
                checkItem.Status = (int)CheckListStatusEnum.WaitingForCheck;
                checkItem.Remark = entity.Remark;
                checkItem.CheckListCode = entity.Code;
                // 生成编码
                checkItem.Code = SequenceContract.Create(checkItem.GetType());
                if (!CheckRepository.Insert(checkItem))
                {
                    return DataProcess.Failure("新增盘点单任务失败");
                }
                // 更新盘点单状态
                entity.Status = (int)CheckListStatusEnum.Issue;
                CheckListRepository.Update(entity);
            }
            else  // 盘点的有内容
            {
                // 根据仓库下的货柜分组
                var groupList = CheckListDetailList.GroupBy(a => new { a.WareHouseCode, a.ContainerCode });

                CheckRepository.UnitOfWork.TransactionEnabled = true;
                foreach (var item in groupList)
                {
                    // 盘点单明细
                    CheckListDetail temp = item.FirstOrDefault();
                    // 盘点单任务
                    var checKEntity = new CheckMain()
                    {
                        IsDeleted = false,
                        CheckType = entity.CheckDict,
                        WareHouseCode = temp.WareHouseCode,
                        ContainerCode = temp.ContainerCode,
                        StartTime = entity.StartTime,
                        EndTime = entity.EndTime,
                        Status = (int)CheckStatusCaption.WaitingForCheck,
                        Remark = entity.Remark,
                        CheckListCode = entity.Code,
                    };
                    // 生成编码
                    checKEntity.Code = SequenceContract.Create(checKEntity.GetType());

                    // 添加盘点任务明细
                    foreach (CheckListDetail CheckListMaterial in item)
                    {
                        var checKDetailEntity = new CheckDetail()
                        {
                            CheckCode = checKEntity.Code,
                            IsDeleted = false,
                            WareHouseCode = temp.WareHouseCode,
                            Status = (int)CheckStatusCaption.WaitingForCheck,
                            Quantity = Convert.ToInt32(CheckListMaterial.Quantity),
                            CheckedQuantity = 0,
                            MaterialCode = CheckListMaterial.MaterialCode,
                            MaterialLabel = CheckListMaterial.MaterialLabel,
                            LocationCode = CheckListMaterial.LocationCode,
                            CheckedTime = CheckListMaterial.CheckedTime,
                        };

                        if (!CheckDetailRepository.Insert(checKDetailEntity))
                        {
                            return DataProcess.Failure("新增盘点明细失败");
                        }

                        // 更新盘点明细状态
                        CheckListMaterial.Status = (int)CheckListStatusEnum.Issue;
                        CheckListDetailRepository.Update(CheckListMaterial);

                    }
                    if (!CheckRepository.Insert(checKEntity))
                    {
                        return DataProcess.Failure("新增盘点单任务失败");
                    }

                    // 更新盘点单状态
                    entity.Status = (int)CheckListStatusEnum.Issue;
                    entity.StartTime = DateTime.Now;
                    CheckListRepository.Update(entity);
                }
            }                     
            CheckRepository.UnitOfWork.Commit();
            return DataProcess.Success("新增盘点单任务成功");
        }

        
        public DataResult EditCheck(CheckMain entity)
        {
            if (CheckRepository.Update(entity)>0)
            {
                return DataProcess.Success("编辑成功");
            }
            return DataProcess.Failure("编辑失败");
        }

        /// <summary>
        /// 手动盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult HandCheckDetail(CheckDetail entity)
        {
            CheckDetailRepository.UnitOfWork.TransactionEnabled = true;
            // 盘点任务实体
            var checkEntity = Checks.FirstOrDefault(a => a.Code == entity.CheckCode);
            if (checkEntity.Status!=(int)CheckStatusCaption.WaitingForCheck && checkEntity.Status!=(int)CheckStatusCaption.HandChecking && entity.Status!=(int)CheckStatusCaption.CheckAgagin)
            {
                return DataProcess.Failure("盘点单状态不为待盘(复盘)点或者手动盘点");
            }
            entity.Status = (int)CheckStatusCaption.Checked;
            if (string.IsNullOrEmpty(entity.Checker))
            {
                entity.Checker = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
            }
            entity.CheckedTime = DateTime.Now;
            if (checkEntity.StartTime==null)
            {
                checkEntity.StartTime = DateTime.Now;
            }
            // 更改盘点任务明细
            CheckDetailRepository.Update(entity);
            //CheckArea checkArea = this.CheckAreaRepository.Query().FirstOrDefault(a => a.CheckCode == entity.CheckCode && a.AreaCode == entity.AreaCode);
            //if (!CheckDetails.Any(a=>a.CheckCode==entity.CheckCode && a.Status!=(int)Bussiness.Enums.CheckStatusCaption.Checked && a.Id!=entity.Id))
            //{
            //    checkEntity.Status = (int)Bussiness.Enums.CheckStatusCaption.Checked;
            //    checkEntity.EndTime = DateTime.Now;
            //}

            //if (!CheckDetails.Any(a => a.CheckCode == entity.CheckCode && a.Status != (int)Bussiness.Enums.CheckStatusCaption.Checked && a.Id != entity.Id && a.AreaCode==entity.AreaCode))
            //{
            //    checkArea.Status= (int)Bussiness.Enums.CheckStatusCaption.Checked;
            //    CheckAreaRepository.Update(checkArea);
            //}
            // 是否都是盘点完成
            if (CheckDetailDtos.Where(a => a.CheckCode == checkEntity.Code).Any(a => a.Status != (int)CheckStatusCaption.Checked))
            {
                checkEntity.Status = (int)CheckStatusCaption.HandChecking;
            }
            else
            {
                checkEntity.Status = (int)CheckStatusCaption.Checked;
            }
            
            CheckRepository.Update(checkEntity);

            // 盘点单
            var checkListEntity = CheckListRepository.Query().FirstOrDefault(a => a.Code == checkEntity.CheckListCode);
            // 更改盘点单详情信息
            CheckListDetail checkListDetailEntity;
            //var checkListDetailEntity;
            // 没有条码时，则是盘点的空储位
            if (string.IsNullOrEmpty(entity.MaterialLabel))
            {
                 checkListDetailEntity = CheckListDetailRepository.Query().FirstOrDefault(a => a.CheckCode == checkListEntity.Code && a.LocationCode == entity.LocationCode);
            }
            else  //有条码根据加条码
            {
                 checkListDetailEntity = CheckListDetailRepository.Query().FirstOrDefault(a => a.CheckCode == checkListEntity.Code && a.LocationCode == entity.LocationCode && a.MaterialLabel == entity.MaterialLabel);
            }           
            checkListDetailEntity.Status = (int)CheckListStatusEnum.Accomplish;
            checkListDetailEntity.Checker = entity.Checker;
            checkListDetailEntity.CheckedQuantity = entity.CheckedQuantity;
            if (CheckListDetailRepository.Update(checkListDetailEntity) <= 0)
            {
                return DataProcess.Failure("盘点单详情更新失败。");
            }       
            // 盘点详情是否都已经完成
            if (CheckListContract.CheckListDetailDtos.Where(a=>a.CheckCode == checkListEntity.Code).Any(a=>a.Status != (int)CheckListStatusEnum.Accomplish))
            {               
                checkListEntity.Status = (int)CheckListStatusEnum.Proceed;
            }
            else
            {
                checkListEntity.Status = (int)CheckListStatusEnum.Accomplish;
            }
            if(CheckListRepository.Update(checkListEntity)<=0)
            {
                return DataProcess.Failure("盘点单更新失败。");
            }
            CheckDetailRepository.UnitOfWork.Commit();
            return DataProcess.Success("盘点成功");
        }


        /// <summary>
        /// 货柜盘点--客户端
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult HandCheckDetailClient(CheckDto entityDto)
        {
            //添加用户角色映射数据
            if (!entityDto.CheckDetailMaterialList.IsNullOrEmpty())
            {
                CheckDetailRepository.UnitOfWork.TransactionEnabled = true;
                var checkDetailList = entityDto.CheckDetailMaterialList.FromJsonString<List<CheckDetailDto>>();

                // 盘点明细
                var tempEntity= checkDetailList.FirstOrDefault();

                // 盘点任务实体
                var checkEntity = Checks.FirstOrDefault(a => a.Code == entityDto.Code);

                //盘点没有数据时 -单个盘点储位
                if(checkDetailList.Count == 0)
                {
                    // 获取盘点单明细
                    //var CheckListDetailedItem = CheckListDetailRepository.Query().FirstOrDefault(a=>a.CheckCode == checkListEntity.Code && a.LocationCode ==)

                    //盘点任务明细盘点数量置为0
                    var CheckDetaileds = CheckDetailRepository.Query().Where(a => a.CheckCode == entityDto.Code && a.LocationCode == entityDto.CheckLocationCode).ToList();
                    foreach(var CheckDetailedItem in CheckDetaileds)
                    {
                        CheckDetailedItem.CheckedQuantity = 0;
                        CheckDetailedItem.Status = (int)CheckStatusCaption.Checked;
                        CheckDetailRepository.Update(CheckDetailedItem);
                    }
                }
                else   // 有盘点任务明细
                {
                    // 盘点任务明细
                    foreach (var entity in checkDetailList)
                    {
                        #region 盘点任务明细

                        entity.Status = (int)CheckStatusCaption.Checked;
                        if (string.IsNullOrEmpty(entity.Checker))
                        {
                            entity.Checker = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
                        }
                        entity.CheckedTime = DateTime.Now;

                        // 查询原始盘点任务明细
                        var mateialEntity = MaterialContract.MaterialRepository.GetEntity(a => a.Code == entity.MaterialCode);

                        // 单包
                        if (mateialEntity.IsPackage)
                        {
                            var orCheckDetail = CheckDetailRepository.Query()
                                .FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel
                                                     && a.LocationCode == entity.LocationCode
                                                     && a.CheckCode == checkEntity.Code);
                            try
                            {

                                if (orCheckDetail == null)
                                {
                                    CheckDetail inEnity = Mapper.MapTo<CheckDetail>(entity);
                                    inEnity.Quantity = 0;
                                    inEnity.WareHouseCode = checkEntity.WareHouseCode;
                                    if (string.IsNullOrEmpty(entity.Checker))
                                    {
                                        inEnity.Checker = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
                                    }
                                    CheckDetailRepository.Insert(inEnity);
                                }
                                else // 存在则更新
                                {
                                    orCheckDetail.CheckedQuantity = entity.CheckedQuantity;
                                    orCheckDetail.CheckedTime = entity.CheckedTime;
                                    orCheckDetail.Status = entity.Status;
                                    // 不存在则新增
                                    if (string.IsNullOrEmpty(entity.Checker))
                                    {
                                        orCheckDetail.Checker = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
                                    }
                                    CheckDetailRepository.Update(orCheckDetail);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            // 非单包
                            var orUnPackageCheckDetail = CheckDetailRepository.Query()
                                .FirstOrDefault(a => a.MaterialCode == entity.MaterialCode
                                                     && a.LocationCode == entity.LocationCode
                                                     && a.CheckCode == checkEntity.Code);
                            try
                            {
                 
                                // 不存在则新增
                                if (orUnPackageCheckDetail == null)
                                {
                                    CheckDetail inEnity = Mapper.MapTo<CheckDetail>(entity);
                                    inEnity.Quantity = 0;
                                    inEnity.WareHouseCode = checkEntity.WareHouseCode;
                                    inEnity.Checker = entity.Checker;
                                    CheckDetailRepository.Insert(inEnity);
                                }
                                else // 存在则更新
                                {
                                    orUnPackageCheckDetail.CheckedQuantity = entity.CheckedQuantity;
                                    orUnPackageCheckDetail.CheckedTime = entity.CheckedTime;
                                    orUnPackageCheckDetail.Status = entity.Status;
                                    orUnPackageCheckDetail.Checker = entity.Checker;
                                    CheckDetailRepository.Update(orUnPackageCheckDetail);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        
                        #endregion
                    }                                            
                }
                // 更改盘点任务状态
                checkEntity.Status = (int)CheckStatusCaption.HandChecking;
                CheckRepository.Update(checkEntity);
                CheckDetailRepository.UnitOfWork.Commit();
            }      
            return DataProcess.Success("盘点成功");
        }

        /// <summary>
        /// 盘点完成-更改状态
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult ConfirmCheck(CheckDto entityDto)
        {
            // 盘点任务实体
            var checkEntity = Checks.FirstOrDefault(a => a.Code == entityDto.Code);

            CheckDetailRepository.UnitOfWork.TransactionEnabled = true;

            if (checkEntity.Status != (int)CheckStatusCaption.WaitingForCheck && checkEntity.Status != (int)CheckStatusCaption.HandChecking && checkEntity.Status != (int)CheckStatusCaption.CheckAgagin)
            {
                return DataProcess.Failure("盘点任务状态不为待盘(复盘)点或者手动盘点");
            }

            #region 盘点任务明细-更改盘点任务明细盘点数量(盘亏情况)
            //判断有哪些系统有，盘点没有
            if (CheckDetails.Any(a => a.Status != (int)CheckStatusCaption.Checked && a.CheckCode == entityDto.Code))
            {
                var unCheckDetailList = CheckDetails.Where(a => a.Status != (int)CheckStatusCaption.Checked && a.CheckCode == entityDto.Code).ToList();
                
                //将盘点数量置为0
                foreach ( var item in unCheckDetailList)
                {
                    item.CheckedQuantity = 0;
                    item.Status = (int)CheckStatusCaption.Checked;
                    CheckDetailRepository.Update(item);
                }
            }
            #endregion

           
            #region 盘点任务-更改盘点任务状态
            if (checkEntity.StartTime == null)
            {
                checkEntity.StartTime = DateTime.Now;
            }
            // 是否都是盘点完成
            if (CheckDetailDtos.Where(a => a.CheckCode == checkEntity.Code).Any(a => a.Status != (int)CheckStatusCaption.Checked))
            {
                checkEntity.Status = (int)CheckStatusCaption.HandChecking;
            }
            else
            {
                checkEntity.Status = (int)CheckStatusCaption.Checked;
            }
            // 更新盘点任务
            if (CheckRepository.Update(checkEntity) <= 0)
            {
                return DataProcess.Failure("盘点单更新失败。");
            }
            #endregion

            // 盘点单
            var checkListEntity = CheckListRepository.Query().FirstOrDefault(a => a.Code == checkEntity.CheckListCode);

            #region 盘点单明细-有更新无添加

            // 盘点任务明细数据
            var CheckDetaileds = CheckDetails.Where(a => a.CheckCode == entityDto.Code).ToList();

            try
            {
                // 循环盘点任务明细
                foreach (var entity in CheckDetaileds)
                {
                    //原始盘点单明细
                    var checkListDetailEntity = CheckListDetailRepository.Query().FirstOrDefault(a => a.CheckCode == checkListEntity.Code && a.MaterialLabel == entity.MaterialLabel && a.LocationCode == entity.LocationCode);

                    //当前所放储位
                    var LocationItem = LocationRepository.Query().FirstOrDefault(a => a.Code == entity.LocationCode);
                    //不存在则添加
                    if (checkListDetailEntity == null)
                    {
                        //盘点单明细
                        var inEnity = new CheckListDetail()
                        {
                            CheckCode = checkListEntity.Code,
                            IsDeleted = false,
                            Quantity = entity.CheckedQuantity,
                            CheckedQuantity = entity.CheckedQuantity,
                            BatchCode = entity.BatchCode,
                            MaterialCode = entity.MaterialCode,
                            MaterialLabel = entity.MaterialLabel,
                            WareHouseCode = entity.WareHouseCode,
                            LocationCode = entity.LocationCode,
                            ContainerCode = LocationItem.ContainerCode,
                            CheckedTime = entity.CheckedTime,
                            Checker = entity.Checker,
                            Status = (int)CheckListStatusEnum.Accomplish,
                            TrayId = LocationItem.TrayId
                        };

                        CheckListDetailRepository.Insert(inEnity);
                    }
                    else // 存在则更新
                    {
                        checkListDetailEntity.Status = (int)CheckListStatusEnum.Accomplish;
                        checkListDetailEntity.Checker = entity.Checker;
                        checkListDetailEntity.CheckedQuantity = entity.CheckedQuantity;
                        if (CheckListDetailRepository.Update(checkListDetailEntity) <= 0)
                        {
                            return DataProcess.Failure("盘点单详情更新失败。");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            #endregion

            #region 盘点单
            // 盘点详情是否都已经完成
            if (CheckListContract.CheckListDetailDtos.Where(a => a.CheckCode == checkListEntity.Code).Any(a => a.Status != (int)CheckListStatusEnum.Accomplish))
            {
                checkListEntity.Status = (int)CheckListStatusEnum.Proceed;
            }
            else
            {
                checkListEntity.Status = (int)CheckListStatusEnum.Accomplish;
            }
            // 更新盘点单
            if (CheckListRepository.Update(checkListEntity) <= 0)
            {
                return DataProcess.Failure("盘点单更新失败。");
            }
            #endregion
            CheckDetailRepository.UnitOfWork.Commit();
            return DataProcess.Success("盘点完成");
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult CancelCheck(int id)
        {
            var checkEntity = CheckRepository.GetEntity(id);
            if (checkEntity.Status==(int)Bussiness.Enums.CheckStatusCaption.Sended || checkEntity.Status==(int)Bussiness.Enums.CheckStatusCaption.PTLChecking)
            {
                return DataProcess.Failure("该盘点单已发送至PTL,此时不允许作废");
            }
            checkEntity.Status = (int)Bussiness.Enums.CheckStatusCaption.Cancel;
            var list = CheckDetails.Where(a => a.CheckCode == checkEntity.Code).ToList();
            foreach (var item in list)
            {
                item.Status=(int)Bussiness.Enums.CheckStatusCaption.Cancel;
                //查找库存 解锁
                var stock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == item.MaterialLabel && a.LocationCode == item.LocationCode);
                if (stock!=null)
                {
                    stock.IsCheckLocked = false;
                    StockRepository.Update(stock);
                }
                // 不存在则新增
              
                CheckDetailRepository.Update(item);
            }
            var areaList = CheckAreaRepository.Query().Where(a => a.CheckCode == checkEntity.Code).ToList();
            foreach (var item in areaList)
            {

            }

            // 作废盘点单详情
            var CheckListDetailList = CheckListDetailRepository.Query().Where(a => a.CheckCode == checkEntity.CheckListCode).ToList();
            foreach(var CheckListDetailItem in CheckListDetailList)
            {
                CheckListDetailItem.Status = (int)CheckListStatusEnum.Cancellation;
                CheckListDetailRepository.Update(CheckListDetailItem);
            }
            //作废盘点单
            var CheckListlEntity = CheckListRepository.Query().FirstOrDefault(a => a.Code == checkEntity.CheckListCode);
            CheckListlEntity.Status = (int)CheckListStatusEnum.Cancellation;
            CheckListRepository.Update(CheckListlEntity);

            CheckAreaRepository.Update(a => new CheckArea() { Status = (int)Bussiness.Enums.CheckStatusCaption.Cancel }, p => p.CheckCode == checkEntity.Code);
            CheckRepository.Update(checkEntity);
            return DataProcess.Success("作废成功");
        }

        public DataResult CancelCheckDetail(int id)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// 提交盘点 并更新库存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult SubmitCheckResult(CheckMain entity)
        {
            try
            {
                //var checkEntity = CheckRepository.GetEntity(id);
                if (entity.Status != (int)Bussiness.Enums.CheckStatusCaption.Checked)
                {
                    return DataProcess.Failure("该盘点单尚未盘点完成");
                }
                CheckRepository.UnitOfWork.TransactionEnabled = true;
                entity.Status = (int)Bussiness.Enums.CheckStatusCaption.Finished;
                entity.EndTime = DateTime.Now;
                var list = CheckDetails.Where(a => a.CheckCode == entity.Code).ToList();
                foreach (var item in list)
                {
                    item.Status = (int)Bussiness.Enums.CheckStatusCaption.Finished;
                    //查找库存 解锁
                    var stock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == item.MaterialLabel && a.LocationCode == item.LocationCode);
                    if (stock != null)
                    {
                        stock.IsCheckLocked = false;
                        stock.Quantity = Convert.ToDecimal(item.CheckedQuantity);
                        if (stock.Quantity == 0)
                        {
                            StockRepository.Delete(stock);
                        }
                        else
                        {
                            StockRepository.Update(stock);
                        }
                    }
                    else // 如果在当前储位下没有该物料条码
                    {
                        if (Convert.ToDecimal(item.CheckedQuantity) > 0)
                        {
                            if (String.IsNullOrEmpty(item.MaterialLabel))
                            {
                                var materialLabel = new Label()
                                {
                                    MaterialCode = item.MaterialCode,
                                    Quantity = Convert.ToDecimal(item.CheckedQuantity),
                                    SupplierCode = item.SupplierCode,
                                    ManufactrueDate = DateTime.Now,
                                    BatchCode = item.BatchCode,
                                };
                                var creatLabel = LabelContract.CreateLabel(materialLabel);
                                if (!creatLabel.Success)
                                {
                                    return DataProcess.Failure(string.Format("创建物料条码失败{0}", item.MaterialCode));
                                }

                                item.MaterialLabel = creatLabel.Message;
                            }


                            var checkListEntity = CheckListRepository.Query().FirstOrDefault(a => a.Code == entity.CheckListCode);
                            var LocationEntity = LocationRepository.Query().FirstOrDefault(a => a.Code == item.LocationCode);
                            stock = new Stock();
                            //  stock.ContainerCode = item.AreaCode;
                            stock.BatchCode = item.BatchCode;
                            stock.BillCode = entity.Code;
                            stock.InDate = DateTime.Now.ToString("yyyy-MM-dd");
                            stock.WareHouseCode = item.WareHouseCode;
                            stock.IsCheckLocked = false;
                            stock.LocationCode = item.LocationCode;
                            stock.LockedQuantity = 0;
                            stock.ManufactureDate = item.ManufactureDate;
                            stock.MaterialCode = item.MaterialCode;
                            stock.MaterialLabel = item.MaterialLabel;
                            stock.MaterialStatus = 0;
                            stock.Quantity = Convert.ToDecimal(item.CheckedQuantity);
                            stock.StockStatus = 0;
                            stock.ContainerCode = LocationEntity.ContainerCode;
                            stock.TrayId = LocationEntity.TrayId;
                            StockRepository.Insert(stock);
                        }
                    }

                    CheckDetailRepository.Update(item);
                }
                //CheckAreaRepository.Update(a => new Entitys.CheckArea() { Status = 6 }, p => p.CheckCode == entity.Code);

                CheckRepository.Update(entity);
                //更新盘点单
                var checkList = CheckListRepository.Query().FirstOrDefault(a => a.Code == entity.CheckListCode);
                // 更新盘点单明细
                if (!CheckRepository.Query().Any(a=>a.Status<6))
                {
                    checkList.Status = (int)CheckListStatusEnum.Submit;
                    checkList.EndTime = DateTime.Now;
                    CheckListRepository.Update(checkList);
                    var checkListDetails = CheckListDetailRepository.Query().Where(a => a.CheckCode == checkList.Code).ToList();
                    foreach (var item in checkListDetails)
                    {
                        item.Status = (int)CheckListStatusEnum.Submit;
                        item.Checker = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
                        CheckListDetailRepository.Update(item);
                    }
                }

                #region 登录获取token及进行任务反馈
                //try
                //{
                //    CheckListDetail checkListDetailses = CheckListDetails.FirstOrDefault(a => a.CheckCode == entity.CheckListCode);
                //    // 定义登录接口url和请求参数
                //    string loginUrl = "http://192.168.3.224:8181/api/sys/v1/sso/login";
                //    string username = "MOM";
                //    string password = "CRRc@23#";
                //    // 发送登录请求并获取返回值
                //    var clientes = new HttpClient();
                //    var data = new Dictionary<string, string>
                //    {
                //        { "username", username },
                //        { "password", password }
                //    };
                //    var contentes = new FormUrlEncodedContent(data);
                //    var response = clientes.PostAsync(loginUrl, contentes).Result;
                //    if (response.IsSuccessStatusCode)
                //    {
                //        var responseString = response.Content.ReadAsStringAsync().Result;
                //        // 解析返回值并获取token
                //        var responseObject = JObject.Parse(responseString);
                //        var code = (int)responseObject["code"];
                //        var token = (string)responseObject["data"];
                //        var message = (string)responseObject["message"];
                //        if (code == 1)
                //        {
                //            // 登录成功，拼接token到下架任务反馈接口url和请求参数
                //            string url = "http://192.168.3.224:8181/api/zcdx-MOM/v1/MOM/MOMStockCheck/?access-token=" + token;
                //            // 构造请求参数
                //            List<EnWarehBillShelfModel> models = new List<EnWarehBillShelfModel>();
                //            EnWarehBillShelfModel model = new EnWarehBillShelfModel
                //            {
                //                Id = checkListDetailses.MaterialCode,
                //                Num = (int)checkListDetailses.Quantity,
                //                //WarehBinId = entity.LocationCode,
                //                //MatProduceBatch = entity.BatchCode,
                //            };
                //            models.Add(model);
                //            string json = JsonConvert.SerializeObject(models);
                //            // 构造请求内容
                //            StringContent contented = new StringContent(json, Encoding.UTF8, "application/json");
                //            // 构造HTTP请求
                //            using (var cliented = new HttpClient())
                //            {
                //                cliented.BaseAddress = new Uri(url);
                //                cliented.DefaultRequestHeaders.Accept.Clear();
                //                cliented.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //                // 发送POST请求
                //                HttpResponseMessage responsees = cliented.PostAsync(url, contented).Result;
                //                // 处理响应结果
                //                if (responsees.IsSuccessStatusCode)
                //                {
                //                    string result = responsees.Content.ReadAsStringAsync().Result;

                //                    // 解析JSON数据
                //                    var resultes = JsonConvert.DeserializeObject<dynamic>(result);
                //                    // 获取接口返回的各个字段值
                //                    var codees = (int)resultes.code;
                //                    var dataes = (string)resultes.data;
                //                    var messagees = (string)resultes.message;
                //                    var successes = (bool)resultes.success;

                //                    if (codees == 0)
                //                    {
                //                        return DataProcess.Failure(dataes, messagees);
                //                    }
                //                    else if (codees == 1)
                //                    {
                //                        return DataProcess.Success(dataes, messagees);
                //                    }
                //                    else if (codees == 2)
                //                    {
                //                        return DataProcess.Failure(dataes, messagees);
                //                    }

                //                    //return DataProcess.Success("数据传输响应成功！", result);
                //                }
                //                else
                //                {
                //                    return DataProcess.Failure("数据传输响应失败！", responsees.StatusCode);
                //                }
                //            }
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{

                //    return DataProcess.Failure("请求登录接口失败！", ex.Message);
                //}
                #endregion

                CheckRepository.UnitOfWork.Commit();
                return DataProcess.Success("提交成功");
            }
            catch (Exception ex)
            {
                return DataProcess.Failure("提交失败",ex.Message);
            }
        }

        // 上架任务模型
        public class EnWarehBillShelfModel
        {
            //单据id
            [JsonProperty("id")]
            public string Id { get; set; }
            //数量
            [JsonProperty("actualQty")]
            public int Num { get; set; }
            //仓位id(物料位置)
            [JsonProperty("warehBinId")]
            public string WarehBinId { get; set; }
            //生产批次
            [JsonProperty("matProduceBatch")]
            public object MatProduceBatch { get; set; }
        }


        /// <summary>
        /// 新增盘点条码 新增表示已盘点完成
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateCheckDetailForHand(CheckDetail entity)
        {          
            var location = LocationRepository.Query().FirstOrDefault(a => a.Code == entity.LocationCode);
            if (location==null)
            {
                return DataProcess.Failure("该库位码不存在");
            }
            if (CheckDetails.Any(a => a.CheckCode == entity.CheckCode && a.LocationCode == entity.LocationCode && a.MaterialLabel == entity.MaterialLabel))
            {
                return DataProcess.Failure("此条码盘点库位上已存在");
            }
            //entity.AreaCode = location.AreaCode;
            entity.WareHouseCode = location.WareHouseCode;
            entity.CheckedQuantity = entity.Quantity;
            entity.Checker = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
            entity.CheckedTime = DateTime.Now;
            entity.Status = (int)Bussiness.Enums.CheckStatusCaption.Checked;

            this.CheckDetailRepository.UnitOfWork.TransactionEnabled = true;
            if (CheckAreaRepository.Query().FirstOrDefault(a=>a.CheckCode==entity.CheckCode && a.AreaCode==entity.AreaCode)==null)
            {
                CheckArea checkArea = new CheckArea();
                checkArea.AreaCode = entity.AreaCode;
                checkArea.CheckCode = entity.CheckCode;
                checkArea.Status = entity.Status = (int)Bussiness.Enums.CheckStatusCaption.Checked;
                checkArea.WareHouseCode = entity.WareHouseCode;
                CheckAreaRepository.Insert(checkArea);
            }
            if (!CheckDetailRepository.Insert(entity))
            {
                return DataProcess.Failure("新增盘点条码失败");
            }

            //获取盘点任务
            var checkEntity = CheckRepository.Query().FirstOrDefault(a => a.Code == entity.CheckCode);
            // 更改盘点任务状态
            if (CheckDetailDtos.Where(a => a.CheckCode == checkEntity.Code).Any(a => a.Status != (int)CheckStatusCaption.Checked))
            {
                checkEntity.Status = checkEntity.Status;
            }
            else
            {
                checkEntity.Status = (int)CheckStatusCaption.Checked;
            }
            CheckRepository.Update(checkEntity);
             // 获取盘点单
             var checkListEntity = CheckListRepository.Query().FirstOrDefault(a => a.Code == checkEntity.CheckListCode);
            //添加盘点明细
            var checkListDetailEntity = new CheckListDetail()
            {
                IsDeleted = false,
                CheckCode = checkListEntity.Code,
                Quantity = entity.Quantity,
                CheckedQuantity = entity.Quantity,
                MaterialCode = entity.MaterialCode,
                MaterialLabel = entity.MaterialLabel,
                WareHouseCode = entity.WareHouseCode,
                LocationCode = entity.LocationCode,
                ContainerCode = location.ContainerCode,
                Status = (int)CheckListStatusEnum.Accomplish,
                TrayId = location.TrayId,
                CreatedTime = DateTime.Now,
                CheckedTime = DateTime.Now
            };
            // 修改盘点单明细状态
            if(!CheckListDetailRepository.Insert(checkListDetailEntity))
            {
                return DataProcess.Failure("新增盘点单明细失败");
            }
            // 修改盘点单状态
            if (CheckListContract.CheckListDetailDtos.Where(a => a.CheckCode == checkListEntity.Code).Any(a => a.Status != (int)CheckListStatusEnum.Accomplish))
            {
                checkListEntity.Status = checkListEntity.Status;
            }
            else
            {
                checkListEntity.Status = (int)CheckListStatusEnum.Accomplish;
            }
            if(CheckListRepository.Update(checkListEntity) <= 0)
            {
                return DataProcess.Failure("盘点单状态修改失败");
            }
            this.CheckDetailRepository.UnitOfWork.Commit();
            return DataProcess.Success();          
        }
        
        /// <summary>
        /// 创建盘点区域
        /// </summary>
        /// <returns></returns>
        public DataResult CreateCheckArea(CheckArea entity)
        {
            if (!CheckAreaRepository.Insert(entity))
            {
                return DataProcess.Failure("新增盘点区域失败");
            }
            return DataProcess.Success();
        }

    }
}
