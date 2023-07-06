using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;

namespace Bussiness.Services
{
    public class CheckListServer : ICheckListContract
    {
        /// <summary>
        /// 盘点单Checklist
        /// </summary>
        public IRepository<CheckList, int> CheckListRepository { get; set; }
        public IQuery<CheckList> CheckLists => CheckListRepository.Query();

        /// <summary>
        /// 盘点单明细
        /// </summary>
        public IRepository<CheckListDetail, int> CheckListDetailRepository { get; set; }
        public IQuery<CheckListDetail> CheckListDetails => CheckListDetailRepository.Query();

        /// <summary>
        /// 仓库
        /// </summary>
        public IRepository<Bussiness.Entitys.WareHouse, int> WareHouseRepository { get; set; }
        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }

        /// <summary>
        /// 物料
        /// </summary>
        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }

        /// <summary>
        /// 托盘
        /// </summary>
        public IRepository<Tray, int> TrayRepository { get; set; }

        /// <summary>
        /// 货柜
        /// </summary>
        public IRepository<Bussiness.Entitys.Container, int> ContainerRepository { get; set; }
        public IQuery<Container> Containers => ContainerRepository.Query();

        /// <summary>
        /// 盘点类别
        /// </summary>
        public IRepository<HPC.BaseService.Models.Dictionary, int> DictionaryRepository { get; set; }

        /// <summary>
        /// 自动生成编码
        /// </summary>
        public ISequenceContract SequenceContract { set; get; }

        /// <summary>
        /// 自动生成编码
        /// </summary>
        public IInContract InContract { set; get; }

        /// <summary>
        /// 自动生成编码
        /// </summary>
        public IOutTaskContract OutTaskContract { set; get; }
        /// <summary>
        /// 自动生成编码
        /// </summary>
        public IInTaskContract InTaskContract { set; get; }

        /// <summary>
        /// 自动生成编码
        /// </summary>
        public IOutContract OutContract { set; get; }


        /// <summary>
        /// 储位
        /// </summary>
        public IRepository<Bussiness.Entitys.Location, int> LocationRepository { get; set; }
        public IQuery<Location> Locations => LocationRepository.Query();

        /// <summary>
        /// 盘点任务
        /// </summary>
        public Bussiness.Contracts.ICheckContract CheckContract { set; get; }

        /// <summary>
        /// 盘点任务明细
        /// </summary>
        public IRepository<Entitys.CheckDetail, int> CheckDetailRepository { get; set; }

        public IRepository<CheckMain, int> CheckRepository { get; set; }
        /// <summary>
        ///视图
        /// </summary>
        public IRepository<Entitys.StockVM, int> StockVMRepository { get; set; }
        public IQuery<StockVM> StockVMs => StockVMRepository.Query();

        /// <summary>
        /// 库存
        /// </summary>
        public IRepository<Bussiness.Entitys.Stock, int> StockRepository { get; set; }

        /// <summary>
        /// 获取盘点单信息
        /// </summary>
        public IQuery<CheckListDto> CheckListDtos => CheckLists.LeftJoin(WareHouseContract.WareHouses,(checkList, wareHouses) => checkList.WareHouseCode == wareHouses.Code).Select((checkList, warehouse) => new CheckListDto
        {
            Id = checkList.Id,
            Code = checkList.Code,          
            WareHouseCode = checkList.WareHouseCode,
            WareHouseName = warehouse.Name,
            IsDeleted = checkList.IsDeleted,
            StartTime = checkList.StartTime,
            EndTime = checkList.EndTime,
            CreatedTime = checkList.CreatedTime,
            CreatedUserCode = checkList.CreatedUserCode,
            CreatedUserName = checkList.CreatedUserName,
            UpdatedTime = checkList.UpdatedTime,
            UpdatedUserCode = checkList.UpdatedUserCode,
            UpdatedUserName = checkList.UpdatedUserName,
            Status = checkList.Status,
            Remark = checkList.Remark,
            CheckDict = checkList.CheckDict
        });

        /// <summary>
        /// 盘点明细
        /// </summary>
        public IQuery<CheckListDetailDto> CheckListDetailDtos
        {
            get
            {
                return CheckListDetails.InnerJoin(WareHouseContract.WareHouses,(checkListDetails ,wareHousers) => checkListDetails.WareHouseCode == wareHousers.Code)
                    .InnerJoin(MaterialContract.Materials,(checkListDetails, wareHousers,materials) => checkListDetails.MaterialCode == materials.Code)
                    .Select((checkListDetails, wareHousers, materials) => new CheckListDetailDto()
                    {
                        Id = checkListDetails.Id,
                        CheckCode = checkListDetails.CheckCode,
                        TrayId= checkListDetails.TrayId,
                        Code = checkListDetails.CheckCode,
                        Status = checkListDetails.Status,
                        IsDeleted = checkListDetails.IsDeleted,
                        WareHouseCode = checkListDetails.WareHouseCode,
                        WarehouseName = wareHousers.Name,
                        CreatedTime = checkListDetails.CreatedTime,
                        MaterialCode = checkListDetails.MaterialCode,
                        MaterialLabel = checkListDetails.MaterialLabel,
                        ContainerCode = checkListDetails.ContainerCode,
                        CreatedUserName = checkListDetails.CreatedUserName,
                        CheckedQuantity = checkListDetails.CheckedQuantity,
                        LocationCode = checkListDetails.LocationCode,
                        MaterialName = materials.Name,
                        CheckedTime = checkListDetails.CheckedTime,
                        Quantity = checkListDetails.Quantity,
                        Checker = checkListDetails.Checker,
                        MaterialUnit = materials.Unit                      
                    });
            }
        }


        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //public DataResult CreateCheckListEntity(CheckList entity)
        //{
        //    // 事务
        //    CheckListRepository.UnitOfWork.TransactionEnabled = true;
        //    {

        //        // 选中的托盘信息-明细
        //        var CheckListDetailList = new List<CheckListDetail>();
        //        // 盘点单实例
        //        var CheckListEntity = new CheckList();

        //        foreach (var item in entity.AddCheckListDetails)
        //        {
        //            var trayEntity = TrayRepository.Query().FirstOrDefault(a => a.Id == item.TrayId);

        //            if (OutTaskContract.OutTaskMaterialDtos.Any(a => (a.Status == (int)OutTaskStatusCaption.Picking || a.Status == (int)OutTaskStatusCaption.WaitingForPicking) && a.SuggestTrayId == trayEntity.Id))
        //            {
        //                return DataProcess.Failure("货柜" + trayEntity.ContainerCode + "托盘" + trayEntity.Code + "尚有正在作业的出库任务");
        //            }
        //            if (InTaskContract.InTaskMaterials.Any(a => (a.Status == (int)InTaskStatusCaption.InProgress || a.Status == (int)InTaskStatusCaption.WaitingForShelf) && a.SuggestTrayId == trayEntity.Id))
        //            {
        //                return DataProcess.Failure("货柜" + trayEntity.ContainerCode + "托盘" + trayEntity.Code + "尚有正在作业的入库任务");
        //            }
        //            if (CheckListDetailDtos.Any(a => (a.Status == (int)CheckListStatusEnum.WaitingForCheck || a.Status == (int)CheckListStatusEnum.Accomplish) && a.TrayId == trayEntity.Id))
        //            {
        //                return DataProcess.Failure("货柜" + trayEntity.ContainerCode + "托盘" + trayEntity.Code + "尚有正在作业的盘点单");
        //            }
        //        }


        //        foreach (var item  in entity.AddCheckListDetails )
        //        {
        //            var trayEntity = TrayRepository.Query().FirstOrDefault(a => a.Id == item.TrayId);

        //            var stockList = StockRepository.Query().Where(a => a.TrayId == trayEntity.Id).ToList();

        //            // 盘点内容没有物料
        //            if (stockList.Count <= 0)  
        //            {
        //                CheckListEntity.WareHouseCode = trayEntity.WareHouseCode;
        //                CheckListEntity.IsDeleted = false;
        //                CheckListEntity.StartTime = DateTime.Now;
        //                CheckListEntity.Remark = entity.Remark;
        //                CheckListEntity.CreatedTime = DateTime.Now;
        //                CheckListEntity.CheckDict = entity.CheckDict;
        //                CheckListEntity.Status = (int)CheckListStatusEnum.WaitingForCheck;
        //                // 生成编码
        //                CheckListEntity.Code = SequenceContract.Create(CheckListEntity.GetType());

        //                var LocationList = LocationRepository.Query().Where(a => a.TrayId == trayEntity.Id).ToList();
        //                foreach(var LocationEntity in LocationList)
        //                {
        //                    // 盘点单明细
        //                    var checkListDetailEntity = new CheckListDetail()
        //                    {
        //                        CheckCode = CheckListEntity.Code,
        //                        IsDeleted = false,
        //                        Quantity = 0,
        //                        MaterialCode = LocationEntity.SuggestMaterialCode,
        //                        MaterialLabel =null,
        //                        WareHouseCode = LocationEntity.WareHouseCode,
        //                        LocationCode = LocationEntity.Code,
        //                        ContainerCode = LocationEntity.ContainerCode,
        //                        Status = (int)CheckListStatusEnum.WaitingForCheck,
        //                        TrayId = LocationEntity.TrayId
        //                    };
        //                    CheckListDetailList.Add(checkListDetailEntity);
        //                }
        //            }
        //            else
        //            {
        //                //var stockLists = stockList.GroupBy(a => new { a.LocationCode });
        //                //foreach(var groutbyStok in stockLists)
        //                //{
        //                    foreach (var stockEntity in stockList)
        //                    {
        //                        // 盘点单明细
        //                        var checkListDetailEntity = new CheckListDetail()
        //                        {
        //                            IsDeleted = false,
        //                            Quantity = stockEntity.Quantity,
        //                            MaterialCode = stockEntity.MaterialCode,
        //                            MaterialLabel = stockEntity.MaterialLabel,
        //                            WareHouseCode = stockEntity.WareHouseCode,
        //                            LocationCode = stockEntity.LocationCode,
        //                            ContainerCode = stockEntity.ContainerCode,
        //                            Status = (int)CheckListStatusEnum.WaitingForCheck,
        //                            TrayId = stockEntity.TrayId
        //                        };
        //                        CheckListDetailList.Add(checkListDetailEntity);
        //                    }

        //                //}                     
        //            }                                   
        //        }                
                              
        //        // 根据仓库分组
        //        var groupList = CheckListDetailList.GroupBy(a => new { a.WareHouseCode });


        //        if (CheckListDetailList.Count <= 0)
        //        {
        //            if (!CheckListRepository.Insert(CheckListEntity))
        //            {
        //                return DataProcess.Failure(string.Format("盘点单{0}添加失败", entity.Code));
        //            }
        //        }

        //        // 循环创建盘点单
        //        foreach (var item in groupList)
        //        {
        //            // 3 查找 是否有正在作业的 入库单
        //            if (InContract.InDtos.Any(a => a.WareHouseCode == entity.WareHouseCode && a.Status != (int)Bussiness.Enums.InStatusCaption.WaitingForShelf && a.Status != (int)Bussiness.Enums.InStatusCaption.Finished))
        //            {
        //                return DataProcess.Failure("该仓库有正在作业的入库单，请先执行完成。");
        //            }

        //            // 3 查找 是否有正在作业的 入库单
        //            if (OutContract.OutDtos.Any(a => a.WareHouseCode == entity.WareHouseCode && a.Status != (int)Bussiness.Enums.OutStatusCaption.WaitSending && a.Status != (int)Bussiness.Enums.OutStatusCaption.Finished))
        //            {
        //                return DataProcess.Failure("该仓库有正在作业的出库单，请先执行完成。");
        //            }

        //            CheckListDetail temp = item.FirstOrDefault();
        //            var CheckListItem = new CheckList()
        //            {
        //                WareHouseCode = temp.WareHouseCode,
        //                IsDeleted = false,
        //                Remark = entity.Remark,
        //                CheckDict = entity.CheckDict,
        //                Status = (int)CheckListStatusEnum.WaitingForCheck,
        //            };

        //            // 生成编码
        //            CheckListItem.Code = SequenceContract.Create(CheckListItem.GetType());
        //            // 增加盘点单明细
        //            foreach (CheckListDetail CheckListMaterial in item)
        //            {
        //                // 创建盘点明细对象
        //                CheckListMaterial.CheckCode = CheckListItem.Code;
        //                // 添加盘点单明细
        //                if (!CheckListDetailRepository.Insert(CheckListMaterial))
        //                {
        //                    return DataProcess.Failure(string.Format("盘点单明细{0}新增失败", entity.Code));
        //                }
        //            }
        //            if (!CheckListRepository.Insert(CheckListItem))
        //            {
        //                return DataProcess.Failure(string.Format("盘点单{0}添加失败", entity.Code));
        //            }
        //        }
        //    }
        //    CheckListRepository.UnitOfWork.Commit();
        //    return DataProcess.Success(string.Format("盘点单{0}添加成功", entity.Code));
        //}

       
        /// <summary>
        ///  作废盘点单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CancellationeCheckList(CheckList entity)
        {

            CheckListRepository.UnitOfWork.TransactionEnabled = true;
            
            // 获取盘点单明细
            var CheckListDetailList = CheckListDetailRepository.Query().Where(a => a.CheckCode == entity.Code).ToList();
     
            // 循环判断盘点单明细状态
            foreach (var item in CheckListDetailList)
            {
                if (item.Status == (int)CheckListStatusEnum.Accomplish || item.Status == (int)CheckListStatusEnum.Cancellation)
                {
                    return DataProcess.Failure(string.Format("盘点明细{0}作废失败", item.CheckCode));
                }
                item.Status = (int)CheckListStatusEnum.Cancellation;
                
                if (CheckListDetailRepository.Update(item) <= 0)
                {
                    return DataProcess.Failure(string.Format("领用明细{0}作废失败", item.CheckCode));
                }
            }

            // 获取盘点单任务
            var CheckLists = CheckContract.Checks.Where(a => a.CheckListCode == entity.Code).ToList();

            //循环获取任务判断更改状态
            foreach (var item in CheckLists)
            {                
                if (item.Status == (int)CheckStatusCaption.WaitingForCheck)
                {                    
                    // 作废任务明细
                    var CheckDetailList = CheckDetailRepository.Query().Where(a => a.CheckCode == item.Code).ToList();
                    foreach (var checkDetailEntity in CheckDetailList)
                    {
                        checkDetailEntity.Status = (int)CheckStatusCaption.Cancel;
                        if(CheckDetailRepository.Update(checkDetailEntity) <= 0)
                        {
                            return DataProcess.Failure(string.Format("盘点任务明细{0}作废失败", checkDetailEntity.CheckCode));
                        }
                    }
                    item.Status = (int)CheckStatusCaption.Cancel;
                    if (CheckRepository.Update(item) <= 0)
                    {
                        return DataProcess.Failure(string.Format("盘点任务{0}作废失败", item.Code));
                    }
                }
                else
                {
                    if (item.Status == (int)CheckStatusCaption.Checked)
                    {
                        return DataProcess.Failure(string.Format("盘点任务{0}作废失败，盘点任务已完成。", item.Code));
                    }
                    if (item.Status == (int)CheckStatusCaption.Finished)
                    {
                        return DataProcess.Failure(string.Format("盘点任务{0}作废失败，盘点任务结果已提交。", item.Code));
                    }
                    if (item.Status == (int)CheckStatusCaption.CheckAgagin)
                    {
                        return DataProcess.Failure(string.Format("盘点任务{0}作废失败，盘点任务正在复盘。", item.Code));
                    }
                    if (item.Status == (int)CheckStatusCaption.HandChecking)
                    {
                        return DataProcess.Failure(string.Format("盘点任务{0}作废失败，盘点任务手动执行中。", item.Code));
                    }
                }
            }
            // 改变盘点单的状态
            entity.Status = (int)CheckListStatusEnum.Cancellation;
            if (CheckListRepository.Update(entity) <= 0)
            {
                return DataProcess.Failure(string.Format("盘点单{0}作废失败", entity.Code));
            }
            CheckListRepository.UnitOfWork.Commit();
            return DataProcess.Success("作废成功");
        }


        /// <summary>
        /// 每隔30秒触发一次获取视图数据
        /// </summary>
        /// <returns></returns>
        public async Task StartCreateOutEntityInterFace()
        {
            while (true)
            {

                #region Oracle数据获取处理
                var connectionString = "Data Source=192.168.3.224:1521/orcl;User ID=zcdx;Password=Oracle#gCsb#2023";
                var query = "SELECT * FROM V_WK_WMS_STOCK_CHECK";
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    var command = new OracleCommand(query, connection);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    var resultList = new List<Dictionary<string, object>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        resultList.Add(row);
                    }
                    reader.Close();
                    foreach (var dict in resultList)
                    {
                        CheckList entity = new CheckList
                        {
                            Id = 0,
                            CheckDict = 2,
                            Code = dict["BILL_CODE"].ToString(),
                            CreatedUserCode = "",
                            CreatedUserName = "",
                            Remark = "",
                            WareHouseCode = "",
                            CreatedTime = (DateTime)dict["Date"],
                            AddCheckListDetails = new List<CheckListDetail>(),
                        };

                        CheckListDetail inMaterialObj = new CheckListDetail
                        {
                            Id = 0,
                            IsDeleted = false,
                            TrayId = (int)dict["TrayId"],
                        };
                        if (!CheckLists.Any(a => a.Code == entity.Code))
                        {
                            entity.AddCheckListDetails.Add(inMaterialObj);
                            CreateCheckListEntityView(entity); // 执行需要定时执行的方法
                        }
                    }
                }
                #endregion

                await Task.Delay(30000); // 设置执行间隔时间为30秒
            }
        }
        /// <summary>
        /// 视图创建盘点单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateCheckListEntityView(CheckList entity)
        {
            // 事务
            CheckListRepository.UnitOfWork.TransactionEnabled = true;
            {

                // 选中的托盘信息-明细
                var CheckListDetailList = new List<CheckListDetail>();
                // 盘点单实例
                var CheckListEntity = new CheckList();

                foreach (var item in entity.AddCheckListDetails)
                {
                    var trayEntity = TrayRepository.Query().FirstOrDefault(a => a.Id == item.TrayId);

                    if (OutTaskContract.OutTaskMaterialDtos.Any(a => (a.Status == (int)OutTaskStatusCaption.Picking || a.Status == (int)OutTaskStatusCaption.WaitingForPicking) && a.SuggestTrayId == trayEntity.Id))
                    {
                        return DataProcess.Failure("货柜" + trayEntity.ContainerCode + "托盘" + trayEntity.Code + "尚有正在作业的出库任务");
                    }
                    if (InTaskContract.InTaskMaterials.Any(a => (a.Status == (int)InTaskStatusCaption.InProgress || a.Status == (int)InTaskStatusCaption.WaitingForShelf) && a.SuggestTrayId == trayEntity.Id))
                    {
                        return DataProcess.Failure("货柜" + trayEntity.ContainerCode + "托盘" + trayEntity.Code + "尚有正在作业的入库任务");
                    }
                    if (CheckListDetailDtos.Any(a => (a.Status == (int)CheckListStatusEnum.WaitingForCheck || a.Status == (int)CheckListStatusEnum.Accomplish) && a.TrayId == trayEntity.Id))
                    {
                        return DataProcess.Failure("货柜" + trayEntity.ContainerCode + "托盘" + trayEntity.Code + "尚有正在作业的盘点单");
                    }
                }


                foreach (var item in entity.AddCheckListDetails)
                {
                    var trayEntity = TrayRepository.Query().FirstOrDefault(a => a.Id == item.TrayId);

                    var stockList = StockRepository.Query().Where(a => a.TrayId == trayEntity.Id).ToList();

                    // 盘点内容没有物料
                    if (stockList.Count <= 0)
                    {
                        CheckListEntity.WareHouseCode = trayEntity.WareHouseCode;
                        CheckListEntity.IsDeleted = false;
                        CheckListEntity.StartTime = DateTime.Now;
                        CheckListEntity.Remark = entity.Remark;
                        CheckListEntity.CreatedTime = DateTime.Now;
                        CheckListEntity.CheckDict = entity.CheckDict;
                        CheckListEntity.Status = (int)CheckListStatusEnum.WaitingForCheck;
                        // 生成编码
                        CheckListEntity.Code = SequenceContract.Create(CheckListEntity.GetType());

                        var LocationList = LocationRepository.Query().Where(a => a.TrayId == trayEntity.Id).ToList();
                        foreach (var LocationEntity in LocationList)
                        {
                            // 盘点单明细
                            var checkListDetailEntity = new CheckListDetail()
                            {
                                CheckCode = CheckListEntity.Code,
                                IsDeleted = false,
                                Quantity = 0,
                                MaterialCode = LocationEntity.SuggestMaterialCode,
                                MaterialLabel = null,
                                WareHouseCode = LocationEntity.WareHouseCode,
                                LocationCode = LocationEntity.Code,
                                ContainerCode = LocationEntity.ContainerCode,
                                Status = (int)CheckListStatusEnum.WaitingForCheck,
                                TrayId = LocationEntity.TrayId
                            };
                            CheckListDetailList.Add(checkListDetailEntity);
                        }
                    }
                    else
                    {
                        //var stockLists = stockList.GroupBy(a => new { a.LocationCode });
                        //foreach(var groutbyStok in stockLists)
                        //{
                        foreach (var stockEntity in stockList)
                        {
                            // 盘点单明细
                            var checkListDetailEntity = new CheckListDetail()
                            {
                                IsDeleted = false,
                                Quantity = stockEntity.Quantity,
                                MaterialCode = stockEntity.MaterialCode,
                                MaterialLabel = stockEntity.MaterialLabel,
                                WareHouseCode = stockEntity.WareHouseCode,
                                LocationCode = stockEntity.LocationCode,
                                ContainerCode = stockEntity.ContainerCode,
                                Status = (int)CheckListStatusEnum.WaitingForCheck,
                                TrayId = stockEntity.TrayId
                            };
                            CheckListDetailList.Add(checkListDetailEntity);
                        }

                        //}                     
                    }
                }

                // 根据仓库分组
                var groupList = CheckListDetailList.GroupBy(a => new { a.WareHouseCode });


                if (CheckListDetailList.Count <= 0)
                {
                    if (!CheckListRepository.Insert(CheckListEntity))
                    {
                        return DataProcess.Failure(string.Format("盘点单{0}添加失败", entity.Code));
                    }
                }

                // 循环创建盘点单
                foreach (var item in groupList)
                {
                    // 3 查找 是否有正在作业的 入库单
                    if (InContract.InDtos.Any(a => a.WareHouseCode == entity.WareHouseCode && a.Status != (int)Bussiness.Enums.InStatusCaption.WaitingForShelf && a.Status != (int)Bussiness.Enums.InStatusCaption.Finished))
                    {
                        return DataProcess.Failure("该仓库有正在作业的入库单，请先执行完成。");
                    }

                    // 3 查找 是否有正在作业的 入库单
                    if (OutContract.OutDtos.Any(a => a.WareHouseCode == entity.WareHouseCode && a.Status != (int)Bussiness.Enums.OutStatusCaption.WaitSending && a.Status != (int)Bussiness.Enums.OutStatusCaption.Finished))
                    {
                        return DataProcess.Failure("该仓库有正在作业的出库单，请先执行完成。");
                    }

                    CheckListDetail temp = item.FirstOrDefault();
                    var CheckListItem = new CheckList()
                    {
                        WareHouseCode = temp.WareHouseCode,
                        IsDeleted = false,
                        Remark = entity.Remark,
                        CheckDict = entity.CheckDict,
                        Status = (int)CheckListStatusEnum.WaitingForCheck,
                    };

                    // 生成编码
                    CheckListItem.Code = SequenceContract.Create(CheckListItem.GetType());
                    // 增加盘点单明细
                    foreach (CheckListDetail CheckListMaterial in item)
                    {
                        // 创建盘点明细对象
                        CheckListMaterial.CheckCode = CheckListItem.Code;
                        // 添加盘点单明细
                        if (!CheckListDetailRepository.Insert(CheckListMaterial))
                        {
                            return DataProcess.Failure(string.Format("盘点单明细{0}新增失败", entity.Code));
                        }
                    }
                    if (!CheckListRepository.Insert(CheckListItem))
                    {
                        return DataProcess.Failure(string.Format("盘点单{0}添加失败", entity.Code));
                    }
                }
            }
            CheckListRepository.UnitOfWork.Commit();
            return DataProcess.Success(string.Format("盘点单{0}添加成功", entity.Code));
        }


        #region 盘点逻辑变更
        //创建盘点单时  只根据创建 盘点单 盘点单明细==> 托盘信息
        //下发盘点单时  根据盘点回归分组  创建盘点任务==> 盘点托盘==>盘点明细(系统存在的物料库存信息,若不存在 则事先没有明细)


        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateCheckListEntity(CheckList entity)
        {
            // 事务
            CheckListRepository.UnitOfWork.TransactionEnabled = true;
            {

                // 选中的托盘信息-明细
                var CheckListDetailList = new List<CheckListDetail>();
                // 盘点单实例
                var CheckListEntity = new CheckList();

                foreach (var item in entity.AddCheckListDetails)
                {
                    var trayEntity = TrayRepository.Query().FirstOrDefault(a => a.Id == item.TrayId);

                    if (OutTaskContract.OutTaskMaterialDtos.Any(a => (a.Status == (int)OutTaskStatusCaption.Picking || a.Status == (int)OutTaskStatusCaption.WaitingForPicking) && a.SuggestTrayId == trayEntity.Id))
                    {
                        return DataProcess.Failure("货柜" + trayEntity.ContainerCode + "托盘" + trayEntity.Code + "尚有正在作业的出库任务");
                    }
                    if (InTaskContract.InTaskMaterials.Any(a => (a.Status == (int)InTaskStatusCaption.InProgress || a.Status == (int)InTaskStatusCaption.WaitingForShelf) && a.SuggestTrayId == trayEntity.Id))
                    {
                        return DataProcess.Failure("货柜" + trayEntity.ContainerCode + "托盘" + trayEntity.Code + "尚有正在作业的入库任务");
                    }
                    if (CheckListDetailDtos.Any(a => (a.Status == (int)CheckListStatusEnum.WaitingForCheck || a.Status == (int)CheckListStatusEnum.Accomplish) && a.TrayId == trayEntity.Id))
                    {
                        return DataProcess.Failure("货柜" + trayEntity.ContainerCode + "托盘" + trayEntity.Code + "尚有正在作业的盘点单");
                    }
                }


                foreach (var item in entity.AddCheckListDetails)
                {
                    var trayEntity = TrayRepository.Query().FirstOrDefault(a => a.Id == item.TrayId);

                    var stockList = StockRepository.Query().Where(a => a.TrayId == trayEntity.Id).ToList();

                    // 盘点内容没有物料
                    if (stockList.Count <= 0)
                    {
                        CheckListEntity.WareHouseCode = trayEntity.WareHouseCode;
                        CheckListEntity.IsDeleted = false;
                        CheckListEntity.StartTime = DateTime.Now;
                        CheckListEntity.Remark = entity.Remark;
                        CheckListEntity.CreatedTime = DateTime.Now;
                        CheckListEntity.CheckDict = entity.CheckDict;
                        CheckListEntity.Status = (int)CheckListStatusEnum.WaitingForCheck;
                        // 生成编码
                        CheckListEntity.Code = SequenceContract.Create(CheckListEntity.GetType());

                        var LocationList = LocationRepository.Query().Where(a => a.TrayId == trayEntity.Id).ToList();
                        foreach (var LocationEntity in LocationList)
                        {
                            // 盘点单明细
                            var checkListDetailEntity = new CheckListDetail()
                            {
                                CheckCode = CheckListEntity.Code,
                                IsDeleted = false,
                                Quantity = 0,
                                MaterialCode = LocationEntity.SuggestMaterialCode,
                                MaterialLabel = null,
                                WareHouseCode = LocationEntity.WareHouseCode,
                                LocationCode = LocationEntity.Code,
                                ContainerCode = LocationEntity.ContainerCode,
                                Status = (int)CheckListStatusEnum.WaitingForCheck,
                                TrayId = LocationEntity.TrayId
                            };
                            CheckListDetailList.Add(checkListDetailEntity);
                        }
                    }
                    else
                    {
                        //var stockLists = stockList.GroupBy(a => new { a.LocationCode });
                        //foreach(var groutbyStok in stockLists)
                        //{
                        foreach (var stockEntity in stockList)
                        {
                            // 盘点单明细
                            var checkListDetailEntity = new CheckListDetail()
                            {
                                IsDeleted = false,
                                Quantity = stockEntity.Quantity,
                                MaterialCode = stockEntity.MaterialCode,
                                MaterialLabel = stockEntity.MaterialLabel,
                                WareHouseCode = stockEntity.WareHouseCode,
                                LocationCode = stockEntity.LocationCode,
                                ContainerCode = stockEntity.ContainerCode,
                                Status = (int)CheckListStatusEnum.WaitingForCheck,
                                TrayId = stockEntity.TrayId
                            };
                            CheckListDetailList.Add(checkListDetailEntity);
                        }

                        //}                     
                    }
                }

                // 根据仓库分组
                var groupList = CheckListDetailList.GroupBy(a => new { a.WareHouseCode });


                if (CheckListDetailList.Count <= 0)
                {
                    if (!CheckListRepository.Insert(CheckListEntity))
                    {
                        return DataProcess.Failure(string.Format("盘点单{0}添加失败", entity.Code));
                    }
                }

                // 循环创建盘点单
                foreach (var item in groupList)
                {
                    // 3 查找 是否有正在作业的 入库单
                    if (InContract.InDtos.Any(a => a.WareHouseCode == entity.WareHouseCode && a.Status != (int)Bussiness.Enums.InStatusCaption.WaitingForShelf && a.Status != (int)Bussiness.Enums.InStatusCaption.Finished))
                    {
                        return DataProcess.Failure("该仓库有正在作业的入库单，请先执行完成。");
                    }

                    // 3 查找 是否有正在作业的 入库单
                    if (OutContract.OutDtos.Any(a => a.WareHouseCode == entity.WareHouseCode && a.Status != (int)Bussiness.Enums.OutStatusCaption.WaitSending && a.Status != (int)Bussiness.Enums.OutStatusCaption.Finished))
                    {
                        return DataProcess.Failure("该仓库有正在作业的出库单，请先执行完成。");
                    }

                    CheckListDetail temp = item.FirstOrDefault();
                    var CheckListItem = new CheckList()
                    {
                        WareHouseCode = temp.WareHouseCode,
                        IsDeleted = false,
                        Remark = entity.Remark,
                        CheckDict = entity.CheckDict,
                        Status = (int)CheckListStatusEnum.WaitingForCheck,
                    };

                    // 生成编码
                    CheckListItem.Code = SequenceContract.Create(CheckListItem.GetType());
                    // 增加盘点单明细
                    foreach (CheckListDetail CheckListMaterial in item)
                    {
                        // 创建盘点明细对象
                        CheckListMaterial.CheckCode = CheckListItem.Code;
                        // 添加盘点单明细
                        if (!CheckListDetailRepository.Insert(CheckListMaterial))
                        {
                            return DataProcess.Failure(string.Format("盘点单明细{0}新增失败", entity.Code));
                        }
                    }
                    if (!CheckListRepository.Insert(CheckListItem))
                    {
                        return DataProcess.Failure(string.Format("盘点单{0}添加失败", entity.Code));
                    }
                }
            }
            CheckListRepository.UnitOfWork.Commit();
            return DataProcess.Success(string.Format("盘点单{0}添加成功", entity.Code));
        }
        #endregion
    }
}
