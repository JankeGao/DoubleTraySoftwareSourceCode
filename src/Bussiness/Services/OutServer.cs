using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Entitys.InterFace;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Core.Mapping;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;

namespace Bussiness.Services
{
    public class OutServer : Contracts.IOutContract
    {
        public IRepository<OutTask, int> OutTaskRepository { get; set; }
        public IRepository<OutTaskMaterialLabel, int> OutTaskMaterialLabelRepository { get; set; }
        public IRepository<OutTaskMaterial, int> OutTaskMaterialRepository { get; set; }
        public IOutContract OutContract { set; get; }
        public IRepository<OutIF, int> OutIFRepository { get; set; }
        public IRepository<OutMaterialIF, int> OutMaterialIFRepository { get; set; }
        public IRepository<WareHouse, int> WareHouseRepository { get; set; }
        public IRepository<Area, int> AreaRepository { get; set; }
        public IRepository<Material, int> MaterialRepository { get; set; }
        public IRepository<Out, int> OutRepository { get; set; }
        public IRepository<OutMaterial, int> OutMaterialRepository { get; set; }

      //  public IRepository<OutMaterialLabel, int> OutMaterialLabelRepository { get; set; }

        public IRepository<Entitys.Stock, int> StockRepository { get; set; }

        public IRepository<Entitys.Location, int> LocationRepository { get; set; }

        public IRepository<Entitys.CheckMain, int> CheckRepository { get; set; }

        public IRepository<HPC.BaseService.Models.Dictionary, int> DictionaryRepository { get; set; }
        public IQuery<Out> Outs => OutRepository.Query();

        public IQuery<OutMaterial> OutMaterials => OutMaterialRepository.Query();

        public IMaterialContract MaterialContract { set; get; }

        public ISequenceContract SequenceContract { set; get; }

        public IWareHouseContract WareHouseContract { set; get; }
        public ISupplyContract SupplyContract { set; get; }

        public IOutTaskContract OutTaskContract { set; get; }

        public IStockContract StockContract { set; get; }

        public IMapper Mapper { set; get; }

        public IQuery<Material> Materials
        {
            get
            {
                return MaterialRepository.Query();
            }
        }
        public IRepository<Entitys.DpsInterface, int> DpsInterfaceRepository { get; set; }
        public IRepository<Entitys.DpsInterfaceMain, int> DpsInterfaceMainRepository { get; set; }
        public IQuery<OutMaterialDto> OutMaterialDtos => OutMaterials
            .InnerJoin(MaterialRepository.Query(), (outMaterial, material) => outMaterial.MaterialCode == material.Code)
            .InnerJoin(Outs, (outMaterial, material, outs) => outMaterial.OutCode == outs.Code)
            .Select((outMaterial, material, outs) => new Dtos.OutMaterialDto
        {
            Id = outMaterial.Id,
            OutCode = outMaterial.OutCode,
            MaterialCode = outMaterial.MaterialCode,
            Quantity = outMaterial.Quantity,
            BatchCode = outMaterial.BatchCode,
            Status = outMaterial.Status,
            IsDeleted = outMaterial.IsDeleted,
            BillCode = outMaterial.BillCode,
            SuggestLocation = outMaterial.SuggestLocation,
            FIFOType=material.FIFOType,
            FIFOAccuracy=material.FIFOAccuracy,
            CreatedUserCode = outMaterial.CreatedUserCode,
            CreatedUserName = outMaterial.CreatedUserName,
            CreatedTime = outMaterial.CreatedTime,
            UpdatedUserCode = outMaterial.UpdatedUserCode,
            UpdatedUserName = outMaterial.UpdatedUserName,
            UpdatedTime = outMaterial.UpdatedTime,
            MaterialName = material.Name,
            SendInQuantity= outMaterial.SendInQuantity,
            MaterialUnit = material.Unit,
            ItemNo = outMaterial.ItemNo,
            WareHouseCode = outs.WareHouseCode,
            AvailableStock = StockRepository.Query().Where(a => a.MaterialCode == outMaterial.MaterialCode && a.WareHouseCode == outs.WareHouseCode).Sum(a => a.Quantity - a.LockedQuantity),
            PickedQuantity = outMaterial.PickedQuantity,
            PickedTime = outMaterial.PickedTime,
            CheckedQuantity = outMaterial.CheckedQuantity
        });

        public IQuery<OutDto> OutDtos => Outs
            .LeftJoin(DictionaryRepository.Query(), (outentity, dictionary) => outentity.OutDict == dictionary.Code)
            .LeftJoin(WareHouseContract.WareHouses, (outentity, dictionary,warehouse)=>outentity.WareHouseCode==warehouse.Code)
            .Select((outentity, dictionary, warehouse) => new Dtos.OutDto()
        {
            Id = outentity.Id,
            Code = outentity.Code,
            BillCode = outentity.BillCode,
            WareHouseCode = outentity.WareHouseCode,
            WareHouseName= warehouse.Name,
            OutDict = outentity.OutDict,
            Status = outentity.Status,
            Remark = outentity.Remark,
            IsDeleted = outentity.IsDeleted,
            BillFields = outentity.BillFields,
            ShelfStartTime = outentity.ShelfStartTime,
            ShelfEndTime = outentity.ShelfEndTime,
            CreatedUserCode = outentity.CreatedUserCode,
            CreatedUserName = outentity.CreatedUserName,
            CreatedTime = outentity.CreatedTime,
            UpdatedUserCode = outentity.UpdatedUserCode,
            UpdatedUserName = outentity.UpdatedUserName,
            UpdatedTime = outentity.UpdatedTime,
            OutDictDescription = dictionary.Name,
            OrderType= outentity.OrderType,
            //CRRCID= outentity.CRRCID,
        });

        public IRepository<OutMaterialLabel, int> OutMaterialLabelRepository { get; set; }
        public IQuery<OutMaterialLabel> OutMaterialLabels => OutMaterialLabelRepository.Query();
        public IQuery<OutMaterialLabelDto> OutMaterialLabelDtos => OutMaterialLabels
           .InnerJoin(MaterialRepository.Query(), (outMaterialLabel, material) => outMaterialLabel.MaterialCode == material.Code)
           .InnerJoin(WareHouseRepository.Query(), (outMaterialLabel, material, wareHouse) => outMaterialLabel.WareHouseCode == wareHouse.Code)
           .InnerJoin(AreaRepository.Query(), (outMaterialLabel, material, wareHouse, area) => outMaterialLabel.AreaCode == area.Code && outMaterialLabel.WareHouseCode == area.WareHouseCode)
           .LeftJoin(SupplyContract.Supplys, (outMaterialLabel, material, wareHouse, area, supplier) => outMaterialLabel.SupplierCode == supplier.Code)
           .Select((outMaterialLabel, material, wareHouse, area, supplier) => new OutMaterialLabelDto
           {
               Id = outMaterialLabel.Id,
               OutCode = outMaterialLabel.OutCode,
               MaterialCode = outMaterialLabel.MaterialCode,
               Quantity = outMaterialLabel.Quantity,
               BatchCode = outMaterialLabel.BatchCode,
               Status = outMaterialLabel.Status,
               IsDeleted = outMaterialLabel.IsDeleted,
               BillCode = outMaterialLabel.BillCode,
               LocationCode = outMaterialLabel.LocationCode,
               OutMaterialId = outMaterialLabel.OutMaterialId,
               MaterialLabel = outMaterialLabel.MaterialLabel,
               AreaCode = outMaterialLabel.AreaCode,
               WareHouseCode = outMaterialLabel.WareHouseCode,
               MaterialName = material.Name,
               MaterialUnit = material.Unit,
               WareHouseName = wareHouse.Name,
               AreaName = area.Name,
               PickedTime = outMaterialLabel.PickedTime,
               RealPickedQuantity = outMaterialLabel.RealPickedQuantity,
               Operator = outMaterialLabel.Operator,
               CheckedTime = outMaterialLabel.CheckedTime,
               Checker = outMaterialLabel.Checker,
               CreatedTime = outMaterialLabel.CreatedTime,
               CreatedUserCode = outMaterialLabel.CreatedUserCode,
               CreatedUserName = outMaterialLabel.CreatedUserName,
               CheckedQuantity = outMaterialLabel.CheckedQuantity,
               SupplierName = supplier.Name
           });

        // public IQuery<OutMaterialLabel> OutMaterialLabels => OutMaterialLabelRepository.Query();

        /// <summary>
        /// 轮训接口--创建WMS出库单
        /// </summary>
        public DataResult CreateOutEntityInterFace()
        {
            #region Oracle数据获取处理
            int count = 0;
            Out entity = new Out(); // 创建In类型的实体entity
            List<Out> entityList = new List<Out>(); // 定义一个集合，用于存储遍历得到的实体entityes
            OutMaterial inMaterialObj = null;
            string currentBillCode = null;
            var connectionString = "Data Source=192.168.3.168:1521/orcl;User ID=zcdx;Password=Oracle#gCsb#2023";
            var query = "SELECT * FROM V_WK_WMS_EX_WAREH_BILL";
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
                    Out entityes = new Out
                    {
                        Id = 0,
                        Code = dict["BILL_CODE"].ToString(),
                        Remark = "",
                        WareHouseCode = "01",
                        CreatedTime = DateTime.Now,
                        AddMaterial = new List<OutMaterial>(),
                    };

                    inMaterialObj = new OutMaterial
                    {
                        MaterialCode = dict["MATERIALCODE"].ToString(),
                        Quantity = (decimal)dict["QTY"],
                        CRRCID = dict["MATERIAL_ID"].ToString(),
                        WORKSTATIONID = dict["WORK_STATION_ID"].ToString(),
                        MERGEID = dict["MERGE_ID"].ToString(),
                        WAREHBINID = dict["WAREH_BIN_ID"].ToString(),
                        Status = 0,
                        BatchCode = "",

                    };

                    if (!Outs.Any(a => a.Code == entityes.Code))
                    {

                        if (currentBillCode != null && dict["BILL_CODE"].ToString() != currentBillCode)
                        {
                            continue;
                        }
                        else
                        {
                            currentBillCode = dict["BILL_CODE"].ToString();
                            entityes.AddMaterial.Add(inMaterialObj);
                            entityList.Add(entityes); // 将entityes加入到集合中
                            entity = new Out
                            {
                                Id = 0,
                                Code = dict["BILL_CODE"].ToString(),
                                Remark = "",
                                WareHouseCode = "01",
                                CreatedTime = DateTime.Now,
                                AddMaterial = new List<OutMaterial>(),
                            };
                        }
                    }
                }

                foreach (var tempEntity in entityList)
                {
                    entity.AddMaterial.AddRange(tempEntity.AddMaterial); // 将集合中的每个entityes的AddMaterial加入到entity中
                }

                if (entityList.Count <= 0)
                {
                    return DataProcess.Failure("暂无新增出库单");
                }

                OutRepository.UnitOfWork.TransactionEnabled = true;
                {
                    // 判断是否有出库单号
                    if (String.IsNullOrEmpty(entity.Code))
                    {
                        entity.Code = SequenceContract.Create(entity.GetType());
                    }
                    if (Outs.Any(a => a.Code == entity.Code))
                    {
                        return DataProcess.Failure("该出库单号已存在");
                    }
                    entity.Status = entity.Status == null ? 0 : entity.Status;
                    entity.OrderType = entity.OrderType == null ? 0 : entity.OrderType; // 单据来源

                    if (!OutRepository.Insert(entity))
                    {
                        return DataProcess.Failure(string.Format("出库单{0}新增失败", entity.Code));
                    }
                    if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
                    {

                        foreach (OutMaterial item in entity.AddMaterial)
                        {
                            item.OutCode = entity.Code;
                            item.BillCode = entity.BillCode;
                            item.Status = item.Status == null ? 0 : item.Status;
                            if (Materials.Any(a => a.Code == inMaterialObj.MaterialCode))
                            {
                                DataResult result = CreateOutMaterialEntity(item);
                                if (!result.Success)
                                {
                                    return DataProcess.Failure(result.Message);
                                }
                            }
                            else
                            {
                                return DataProcess.Failure(string.Format("物料编码{0}系统中不存在，请维护！", inMaterialObj.MaterialCode));
                            }
                            if (inMaterialObj.Quantity <= 0)
                            {
                                return DataProcess.Failure(string.Format("编码为{0}的物料传入数量需大于零，请维护！", inMaterialObj.MaterialCode));
                            }
                        }

                    }
                }
                count++;

                Thread.Sleep(500);

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

                        var queryes = StockRepository.Query()
                            .Where(a => a.MaterialCode == item.Key &&
                                        (a.IsCheckLocked == false || a.IsCheckLocked == null) &&
                                        a.WareHouseCode == entity.WareHouseCode);
                        // 是否启用老化时间
                        if (materialEntity.AgeingPeriod > 0)
                        {
                            //此刻的时间应大于入库时间+老化时间
                            var valtime = DateTime.Now.AddDays(-materialEntity.AgeingPeriod);
                            queryes = queryes.Where(a => valtime > a.ShelfTime);
                        }


                        // 先进先出策略选择
                        switch (materialEntity.FIFOType)
                        {
                            // 无先进先出，根据创建时间
                            case 0:
                                queryes.OrderBy(a => a.CreatedTime);
                                break;
                            // 入库时间
                            case 1:
                                if (materialEntity.FIFOAccuracy == 0 || materialEntity.FIFOAccuracy == 1) //无，秒
                                {
                                    queryes.OrderBy(a => a.ShelfTime);
                                }
                                else if (materialEntity.FIFOAccuracy == 2) // 分钟
                                {
                                    queryes.OrderBy(a => ((DateTime)a.ShelfTime).ToString("g"));//2016/5/9 13:09 短日期 短时间
                                    queryes.OrderBy(a => DateTime.Parse(((DateTime)a.ShelfTime).ToString()).ToString("yyyy-MM-dd HH:mm"));//2016/5/9 13:09 短日期 短时间
                                }
                                else if (materialEntity.FIFOAccuracy == 3) // 小时
                                {
                                    queryes.OrderBy(a => DateTime.Parse(((DateTime)a.ShelfTime).ToString()).ToString("yyyy-MM-dd HH"));//2016/5/9 13:09 短日期 短时间
                                }
                                else // 天
                                {
                                    queryes.OrderBy(a => DateTime.Parse(((DateTime)a.ShelfTime).ToString()).ToString("yyyy-MM-dd"));//2016/5/9 13:09 短日期
                                }
                                break;
                            // 生产日期
                            case 2:
                                if (materialEntity.FIFOAccuracy == 0 || materialEntity.FIFOAccuracy == 1) //无，秒
                                {
                                    queryes.OrderBy(a => a.ManufactureDate);
                                }
                                else if (materialEntity.FIFOAccuracy == 2) // 分钟
                                {
                                    queryes.OrderBy(a => ((DateTime)a.ManufactureDate).ToString("g"));//2016/5/9 13:09 短日期 短时间
                                    queryes.OrderBy(a => DateTime.Parse(((DateTime)a.ManufactureDate).ToString()).ToString("yyyy-MM-dd HH:mm"));//2016/5/9 13:09 短日期 短时间
                                }
                                else if (materialEntity.FIFOAccuracy == 3) // 小时
                                {
                                    queryes.OrderBy(a => DateTime.Parse(((DateTime)a.ManufactureDate).ToString()).ToString("yyyy-MM-dd HH"));//2016/5/9 13:09 短日期 短时间
                                }
                                else // 天
                                {
                                    queryes.OrderBy(a => DateTime.Parse(((DateTime)a.ManufactureDate).ToString()).ToString("yyyy-MM-dd"));//2016/5/9 13:09 短日期
                                }
                                break;
                            // 保质期日期——生产日期+保质期-当前时间
                            case 3:
                                if (materialEntity.FIFOAccuracy == 0 || materialEntity.FIFOAccuracy == 1) //无，秒
                                {
                                    queryes.OrderBy(a => (((DateTime)a.ManufactureDate).AddDays(materialEntity.ValidityPeriod) - DateTime.Now));
                                }
                                else if (materialEntity.FIFOAccuracy == 2) // 分钟
                                {
                                    queryes.OrderBy(a => ((DateTime)a.ManufactureDate).ToString("g"));//2016/5/9 13:09 短日期 短时间
                                    queryes.OrderBy(a => DateTime.Parse(((((DateTime)a.ManufactureDate).AddDays(materialEntity.ValidityPeriod) - DateTime.Now)).ToString()).ToString("yyyy-MM-dd HH:mm"));//2016/5/9 13:09 短日期 短时间
                                }
                                else if (materialEntity.FIFOAccuracy == 3) // 小时
                                {
                                    queryes.OrderBy(a => DateTime.Parse(((((DateTime)a.ManufactureDate).AddDays(materialEntity.ValidityPeriod) - DateTime.Now)).ToString()).ToString("yyyy-MM-dd HH"));//2016/5/9 13:09 短日期 短时间
                                }
                                else // 天
                                {
                                    queryes.OrderBy(a => DateTime.Parse(((((DateTime)a.ManufactureDate).AddDays(materialEntity.ValidityPeriod) - DateTime.Now)).ToString()).ToString("yyyy-MM-dd"));//2016/5/9 13:09 短日期
                                }
                                break;
                        }

                        List<Stock> AvailableStock = queryes.ToList();

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
                                IsDeleted = false,
                                OutCode = entity.Code,
                                OutDict = outEntity.OutDict,
                                //CRRCID = entity.CRRCID,
                            };
                            outTask.Code = SequenceContract.Create(outTask.GetType());


                            var materialList = item.GroupBy(a => new
                            { a.WareHouseCode, a.TrayId, a.ContainerCode, a.LocationCode, a.MaterialCode, a.BatchCode }).ToList();

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
                                    Quantity = pickQuantity,
                                    BatchCode = OutMaterial.Key.BatchCode,
                                    SuggestLocation = OutMaterial.Key.LocationCode,
                                    SuggestContainerCode = OutMaterial.Key.ContainerCode,
                                    SuggestTrayId = OutMaterial.Key.TrayId,
                                    RealPickedQuantity = 0,
                                    MaterialCode = OutMaterial.Key.MaterialCode
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

                    OutRepository.UnitOfWork.Commit();

                    return DataProcess.Success(string.Format("出库单同步成功,共有{0}条增加", count));
                }
                catch (Exception ex)
                {

                    return DataProcess.Failure("拣货任务生成失败:" + ex.Message);
                }
            }
            #endregion


            #region 原同步代码
            //    try
            //{
            //    OutIFRepository.UnitOfWork.TransactionEnabled = true;
            //    var list = OutIFRepository.Query().Where(a => a.Status == (int)InterFaceBCaption.Waiting).ToList();
            //    int count = 0;
            //    foreach (var item in list)
            //    {
            //        // 判断该来源单据号是否已存在出库单
            //        if (Outs.Any(a => a.BillCode == item.BillCode))
            //        {
            //            item.Status = (int)OrderEnum.Error;
            //            item.Remark = "来源单据号" + item.BillCode + "已存在";
            //            OutIFRepository.Update(item);
            //            break;
            //        }
            //        int errorflag = 0;
            //        var outEnity = new Out()
            //        {
            //            BillCode = item.BillCode,
            //            WareHouseCode = item.WareHouseCode,
            //            OutDict = item.OutDict,
            //            OutDate = item.OutDate,
            //            Status = (int)OutStatusCaption.WaitSending,
            //            AddMaterial = new List<Bussiness.Entitys.OutMaterial>(),
            //            OrderType = (int)OrderTypeEnum.Other,
            //          //  Remark = item.Remark
            //        };
            //        var materialList = OutMaterialIFRepository.Query().Where(a => a.BillCode == item.BillCode).ToList();
            //        foreach (var outMaterial in materialList)
            //        {
            //            outMaterial.Status = (int)OrderEnum.Wait;
            //            if (MaterialContract.Materials.FirstOrDefault(a => a.Code == outMaterial.MaterialCode) == null)
            //            {
            //                item.Status = (int)OrderEnum.Error;
            //                errorflag = 1;
            //                outMaterial.Status = (int)OrderEnum.Error;
            //                outMaterial.Remark = "物料编码" + outMaterial.MaterialCode + "不存在!";
            //                OutMaterialIFRepository.Update(outMaterial);
            //                break;
            //            }

            //            var inMaterialEntity = new OutMaterial()
            //            {
            //                BillCode = outMaterial.BillCode,
            //                Status = 0,
            //                OutDict = outMaterial.OutDict,
            //                SendInQuantity = 0,
            //                MaterialCode = outMaterial.MaterialCode,
            //                Quantity = outMaterial.Quantity,
            //                BatchCode = outMaterial.BatchCode,
            //                ItemNo = outMaterial.ItemNo
            //            };
            //            outEnity.AddMaterial.Add(inMaterialEntity);
            //            OutMaterialIFRepository.Update(outMaterial);
            //        }

            //        if (errorflag == 1)
            //        {
            //            OutIFRepository.Update(item);
            //        }
            //        else
            //        {
            //            if (!CreateOutEntity(outEnity).Success)
            //            {
            //                return DataProcess.Failure("该出库单号已存在");
            //            }
            //            item.Status = (int)OrderEnum.Wait;
            //            OutIFRepository.Update(item);
            //            count = count + 1;
            //        }
            //    }
            //    OutIFRepository.UnitOfWork.Commit();
            //    return DataProcess.Success(string.Format("出库单同步成功,共有{0}条增加", count));
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            #endregion
        }

        public DataResult CreateOutEntity(Out entity)
        {
            OutRepository.UnitOfWork.TransactionEnabled = true;
            {
                // 判断是否有出库单号
                if (String.IsNullOrEmpty(entity.Code))
                {
                    entity.Code = SequenceContract.Create(entity.GetType());
                }
                if (Outs.Any(a => a.Code == entity.Code))
                {
                    return DataProcess.Failure("该出库单号已存在");
                }
                entity.Status = entity.Status == null ? 0 : entity.Status;
                entity.OrderType = entity.OrderType == null ? 0 : entity.OrderType; // 单据来源

                if (!OutRepository.Insert(entity))
                {
                    return DataProcess.Failure(string.Format("出库单{0}新增失败", entity.Code));
                }
                if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
                {

                    foreach (OutMaterial item in entity.AddMaterial)
                    {
                        item.OutCode = entity.Code;
                        item.BillCode = entity.BillCode;
                        item.Status = item.Status == null ? 0 : item.Status;
                        DataResult result = CreateOutMaterialEntity(item);
                        if (!result.Success)
                        {
                            return DataProcess.Failure(result.Message);
                        }
                    }
                    // 如果为料仓退料，直接出库下架
                    //if (entity.OutDict == "GetReturn")
                    //{
                    //    foreach (OutMaterial item in entity.AddMaterial)
                    //    {
                    //        item.OutCode = entity.Code;
                    //        item.BillCode = entity.BillCode;
                    //        item.Status = 0;

                    //        DataResult result = CreateOutMaterialEntity(item);
                    //        if (!result.Success)
                    //        {
                    //            return DataProcess.Failure(result.Message);
                    //        }
                    //    }
                    //}
                }
            }
            OutRepository.UnitOfWork.Commit();
            return DataProcess.Success(string.Format("出库单{0}新增成功", entity.Code), entity);
        }

        /// <summary>
        /// 核查可用库存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CheckAvailableStock(string MaterialCode, string WareHouseCode)
        {
            // 获取物料实体
            var materialEntity = MaterialContract.MaterialDtos.FirstOrDefault(a => a.Code == MaterialCode);

            var query = StockRepository.Query()
                .Where(a => a.MaterialCode == MaterialCode &&
                            (a.IsCheckLocked == false || a.IsCheckLocked == null) &&
                            a.WareHouseCode == WareHouseCode);

            // 是否启用老化时间
            if (materialEntity.AgeingPeriod > 0)
            {
                //此刻的时间应大于出库时间+老化时间
                var valtime = DateTime.Now.AddDays(-materialEntity.AgeingPeriod);
                query = query.Where(a => valtime > a.ShelfTime);
            }

            var AvailableStock = query.Sum(a => a.Quantity - a.LockedQuantity);
            return DataProcess.Success("核算成功",AvailableStock);
        }

        public DataResult CreateOutMaterialEntity(OutMaterial entity)
        {
            //if (OutMaterials.Any(a => a.MaterialLabel == entity.MaterialLabel))
            //{
            //    return DataProcess.Failure("该出库条码已存在");
            //}
            if (OutMaterialRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("出库物料{0}新增成功", entity.MaterialCode));
            }
            return DataProcess.Failure("操作失败");
        }

        public DataResult RemoveOutMaterial(int id)
        {
            OutMaterial entity = OutMaterialRepository.GetEntity(id);
            if (entity.Status != (int)Enums.OutStatusCaption.WaitSending)
            {
                return DataProcess.Failure("该出库物料条码执行中或已完成");
            }
            if (OutMaterialRepository.Delete(id) > 0)
            {
                return DataProcess.Success(string.Format("出库物料{0}删除成功", entity.MaterialCode));
            }
            return DataProcess.Failure("操作失败");
        }

        public DataResult RemoveOut(int id)
        {
            Out entity = OutRepository.GetEntity(id);
            if (entity.Status != (int)Enums.OutStatusCaption.WaitSending)
            {
                return DataProcess.Failure("该出库单执行中或已完成");
            }

            OutRepository.UnitOfWork.TransactionEnabled = true;
            if (entity.Status != (int)Enums.OutStatusCaption.WaitSending)
            {
                return DataProcess.Failure("该出库单执行中或已完成");
            }
            if (OutRepository.Delete(id) <= 0)
            {
                return DataProcess.Failure(string.Format("出库单{0}删除失败", entity.Code));
            }
            List<OutMaterial> list = OutMaterials.Where(a => a.OutCode == entity.Code).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (OutMaterial item in list)
                {
                    DataResult result = RemoveOutMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }
            OutRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }

        public DataResult EditOut(Out entity)
        {
            OutRepository.UnitOfWork.TransactionEnabled = true;
            if (OutRepository.Update(entity) <= 0)
            {
                return DataProcess.Failure(string.Format("出库单{0}编辑失败", entity.Code));
            }

            List<OutMaterial> list = OutMaterials.Where(a => a.OutCode == entity.Code).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (OutMaterial item in list)
                {
                    DataResult result = RemoveOutMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }

            if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
            {
                foreach (OutMaterial item in entity.AddMaterial)
                {
                    item.OutCode = entity.Code;
                    item.BillCode = entity.BillCode;
                    item.Status = 0;
                    DataResult result = CreateOutMaterialEntity(item);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }
            OutRepository.UnitOfWork.Commit();
            return DataProcess.Success("编辑成功");
        }


        /// <summary>
        /// 作废出库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CancelOut(Out entity)
        {

            if (OutTaskContract.OutTasks.Any(a => a.OutCode == entity.Code && a.Status != (int)OutTaskStatusCaption.WaitingForPicking))
            {
                return DataProcess.Failure(string.Format("该出库单存在执行中或已完成的出库任务，无法作废"));
            }

            entity.Status = (int)OutStatusCaption.Cancel;
            OutRepository.UnitOfWork.TransactionEnabled = true;

            //作废出库单
            if (OutRepository.Update(entity) <= 0)
            {
                return DataProcess.Failure(string.Format("出库单{0}作废失败", entity.Code));
            }

            //作废出库物料
            List<OutMaterial> list = OutMaterials.Where(a => a.OutCode == entity.Code).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (OutMaterial item in list)
                {
                    item.Status = (int)OutStatusCaption.Cancel;
                    if (OutMaterialRepository.Update(item) <= 0)
                    {
                        return DataProcess.Failure(string.Format("出库单{0}作废失败", entity.Code));
                    }
                }
            }

            //作废出库任务单
            List<OutTask> tasklist = OutTaskContract.OutTasks.Where(a => a.OutCode == entity.Code).ToList();
            if (tasklist != null && tasklist.Count > 0)
            {
                foreach (OutTask item in tasklist)
                {
                    item.Status = (int)OutTaskStatusCaption.Cancel;
                    if (OutTaskContract.OutTaskRepository.Update(item) <= 0)
                    {
                        return DataProcess.Failure(string.Format("出库任务单{0}作废失败", entity.Code));
                    }
                }
            }

            //作废出库任务单
            List<OutTaskMaterial> taskMateriallist = OutTaskContract.OutTaskMaterials.Where(a => a.OutCode == entity.Code).ToList();
            if (taskMateriallist != null && taskMateriallist.Count > 0)
            {
                foreach (OutTaskMaterial item in taskMateriallist)
                {
                    item.Status = (int)OutTaskStatusCaption.Cancel;
                    if (OutTaskContract.OutTaskMaterialRepository.Update(item) <= 0)
                    {
                        return DataProcess.Failure(string.Format("出库任务单{0}作废失败", entity.Code));
                    }
                }
            }

            //作废出库任务明细
            List<OutTaskMaterialLabelDto> taskMaterallist = OutTaskContract.OutTaskMaterialLabelDtos.Where(a => a.OutCode == entity.Code).ToList();
            if (taskMaterallist != null && taskMaterallist.Count > 0)
            {
                foreach (OutTaskMaterialLabelDto item in taskMaterallist)
                {
                    item.Status = (int)OutTaskStatusCaption.Cancel;
                    // 物料实体映射
                    OutTaskMaterialLabel outTaskMaterialLabel = Mapper.MapTo<OutTaskMaterialLabel>(item);
                    if (OutTaskContract.OutTaskMaterialLabelRepository.Update(outTaskMaterialLabel) <= 0)
                    {
                        return DataProcess.Failure(string.Format("出库任务单明细{0}作废失败", item.MaterialCode));
                    }

                    // 解除库存锁定
                    //  库存实体
                    var stock = StockContract.StockRepository.GetEntity(a => a.MaterialLabel == item.MaterialLabel);

                    stock.LockedQuantity = stock.LockedQuantity - item.Quantity;
                    stock.IsLocked = false;//锁定住 不允许移库。。
                    StockRepository.Update(stock);

                    // 更新托盘储位推荐锁定的重量
                    if (StockRepository.Update(stock) <= 0)
                    {
                        return DataProcess.Failure(string.Format("释放库存失败"));
                    }
                }
            }

            OutRepository.UnitOfWork.Commit();
            return DataProcess.Success("编辑成功");
        }

        //public DataResult HandGenerateOutLabel(Out entity)
        //{
        //    try
        //    {
        //        if (CheckRepository.Query().Any(a=>a.WareHouseCode==entity.WareHouseCode && a.Status!=(int)Bussiness.Enums.CheckStatusCaption.Finished && a.Status!=(int)Bussiness.Enums.CheckStatusCaption.Cancel))
        //        {
        //            return DataProcess.Failure("该仓库尚有盘点单未完成");
        //        }
        //        //查找可用库存是否满足
        //        List<OutMaterial> list = OutMaterials.Where(a => a.OutCode == entity.Code).ToList();
        //        IEnumerable<IGrouping<string, OutMaterial>> group = list.GroupBy(a => a.MaterialCode);
        //        List<OutMaterialLabel> labelList = new List<OutMaterialLabel>();
        //        OutRepository.UnitOfWork.TransactionEnabled = true;

        //        foreach (IGrouping<string, OutMaterial> item in group)
        //        {
        //            List<Stock> AvailableStock = StockRepository.Query().Where(a => a.MaterialCode == item.Key && (a.IsCheckLocked == false || a.IsCheckLocked==null) && a.WareHouseCode == entity.WareHouseCode).OrderBy(a => a.ManufactureDate).ToList();//先进先出
        //            if (AvailableStock.Sum(a => a.Quantity - a.LockedQuantity) < item.Sum(a => a.Quantity))
        //            {
        //                return DataProcess.Failure("物料" + item.Key + "库存不足");
        //            }
        //            decimal quantity = item.Sum(a => a.Quantity);
        //            foreach (Stock stock in AvailableStock)
        //            {
        //                decimal needQuantiy = stock.Quantity - stock.LockedQuantity;
        //                if (needQuantiy<=0)
        //                {
        //                    continue;
        //                }
        //                if (needQuantiy >= quantity)
        //                {
        //                    OutMaterialLabel label = new OutMaterialLabel
        //                    {
        //                        BatchCode = stock.BatchCode,
        //                        BillCode = entity.BillCode,
        //                        IsDeleted = false,
        //                        LocationCode = stock.LocationCode,
        //                        MaterialCode = item.Key,
        //                        MaterialLabel = stock.MaterialLabel,
        //                        SupplierCode = stock.SupplierCode,
        //                        OutCode = entity.Code,
        //                        OutMaterialId = 0,
        //                        Quantity = quantity,
        //                        Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending,
        //                        WareHouseCode = stock.WareHouseCode,
        //                        AreaCode = stock.AreaCode
        //                    };
        //                    labelList.Add(label);
        //                    stock.LockedQuantity = stock.LockedQuantity + quantity;
        //                    stock.IsLocked = true;
        //                    StockRepository.Update(stock);
        //                    break;

        //                }
        //                else
        //                {
        //                    OutMaterialLabel label = new OutMaterialLabel
        //                    {
        //                        BatchCode = stock.BatchCode,
        //                        BillCode = entity.BillCode,
        //                        IsDeleted = false,
        //                        LocationCode = stock.LocationCode,
        //                        MaterialCode = item.Key,
        //                        OutCode = entity.Code,
        //                        MaterialLabel =stock.MaterialLabel,
        //                        OutMaterialId = 0,
        //                        Quantity = needQuantiy,
        //                        Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending,
        //                        WareHouseCode = stock.WareHouseCode,
        //                        AreaCode = stock.AreaCode,
        //                        SupplierCode = stock.SupplierCode,
        //                    };
        //                    quantity = quantity - needQuantiy;
        //                    stock.LockedQuantity = stock.LockedQuantity + needQuantiy;
        //                    stock.IsLocked = true;//锁定住 不允许移库。。
        //                    StockRepository.Update(stock);
        //                    labelList.Add(label);
        //                }
        //            }

        //            foreach (OutMaterial outMaterial in item)
        //            {
        //                outMaterial.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //                OutMaterialRepository.Update(outMaterial);
        //            }
        //        }
        //        entity.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;

        //        OutRepository.Update(entity);

        //        if (labelList.Count > 0)
        //        {
        //            foreach (OutMaterialLabel item in labelList)
        //            {
        //                if (!OutMaterialLabelRepository.Insert(item))
        //                {
        //                    return DataProcess.Failure("操作失败");
        //                }
        //            }
        //        }
        //        OutRepository.UnitOfWork.Commit();

        //        return DataProcess.Success("亮灯计划生成成功");
        //    }
        //    catch (Exception ex)
        //    {

        //        return DataProcess.Failure("亮灯计划生成失败:" + ex.Message);
        //    }

        //}


        /// <summary>
        /// 手动下架条码 减少库存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //public DataResult ExcuteHandShelfDown(OutMaterialLabel entity)
        //{
        //    OutMaterialLabelRepository.UnitOfWork.TransactionEnabled = true;
        //    //foreach (var item in list)
        //    //{
        //    //    if (pickedQuantity>=item.Quantity && pickedQuantity>0)
        //    //    {
        //    //        item.PickedQuantity = item.Quantity;
        //    //        item.Status = (int)Bussiness.Enums.OutStatusCaption.Picked;
        //    //        item.PickedTime = DateTime.Now;
        //    //        OutMaterialRepository.Update(item);
        //    //        pickedQuantity = pickedQuantity - item.Quantity;
        //    //    }
        //    //    else
        //    //    {
        //    //        item.PickedQuantity = pickedQuantity;
        //    //        //item.Status = (int)Bussiness.Enums.OutStatusCaption.HandPicking;
        //    //        item.PickedTime = DateTime.Now;
        //    //    }
        //    //}

        //    IQuery<OutMaterialLabel> labelList = OutMaterialLabels.Where(a => a.OutCode == entity.OutCode);
        //    List<OutMaterialLabel> materialLabelList = labelList.Where(a => a.MaterialCode == entity.MaterialCode && a.Id != entity.Id).ToList();
        //    entity.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //    if (!materialLabelList.Any(a => a.MaterialCode == entity.MaterialCode && a.Status < (int)Bussiness.Enums.OutStatusCaption.WaitSending && a.Id != entity.Id))
        //    {
        //        List<OutMaterial> list = OutMaterials.Where(a => a.OutCode == entity.OutCode && a.MaterialCode == entity.MaterialCode).ToList();
        //        decimal? pickedQuantity = materialLabelList.Sum(a => a.RealPickedQuantity) + entity.RealPickedQuantity;
        //        foreach (OutMaterial item in list)
        //        {
        //            if (pickedQuantity >= item.Quantity && pickedQuantity > 0)
        //            {
        //                item.PickedQuantity = item.Quantity;
        //                item.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //                item.PickedTime = DateTime.Now;
        //                OutMaterialRepository.Update(item);
        //                pickedQuantity = pickedQuantity - item.Quantity;
        //            }
        //            else
        //            {
        //                item.PickedQuantity = pickedQuantity;
        //                item.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //                item.PickedTime = DateTime.Now;
        //                OutMaterialRepository.Update(item);
        //                pickedQuantity = 0;
        //            }
        //        }

        //    }


        //    if (!labelList.Any(a => a.Status < (int)Bussiness.Enums.OutStatusCaption.WaitSending && a.Id != entity.Id))
        //    {
        //        Out outEntity = Outs.FirstOrDefault(a => a.Code == entity.OutCode);
        //        outEntity.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //        outEntity.ShelfEndTime = DateTime.Now;
        //        OutRepository.Update(outEntity);
        //    }

        //    entity.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //    entity.PickedTime = DateTime.Now;
        //    if (string.IsNullOrEmpty(entity.Operator))
        //    {
        //        entity.Operator = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
        //    }

        //    //扣减库存
        //    Stock stock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel);
        //    // stock.Quantity = stock.Quantity - entity.RealPickedQuantity.GetValueOrDefault(0);
        //    stock.LockedQuantity = stock.LockedQuantity - entity.Quantity + entity.RealPickedQuantity.GetValueOrDefault(0);
        //    StockRepository.Update(stock);//更新锁定数量
        //    //if (stock.LockedQuantity == 0)
        //    //{
        //    //    stock.IsLocked = false;
        //    //}
        //    //if (stock.Quantity == 0)
        //    //{
        //    //    StockRepository.Delete(stock);
        //    //}
        //    //else
        //    //{
        //    //    StockRepository.Update(stock);
        //    //}
        //    OutMaterialLabelRepository.Update(entity);
        //    OutMaterialLabelRepository.UnitOfWork.Commit();
        //    return DataProcess.Success("操作成功");
        //}
        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //public DataResult SendOrderToPTL(Out entity)
        //{
        //    try
        //    {
        //        if (entity.Status != (int)(Bussiness.Enums.OutStatusCaption.WaitSending))
        //        {
        //            return DataProcess.Failure("出库单状态不对,应为任务待发送状态");
        //        }
        //        OutRepository.UnitOfWork.TransactionEnabled = true;
        //        entity.Status = (int)Bussiness.Enums.OutStatusCaption.SendedPickOrder;
        //        List<OutMaterial> list = OutMaterials.Where(a => a.OutCode == entity.Code && a.Status == (int)Bussiness.Enums.OutStatusCaption.WaitSending).ToList();
        //        list.ForEach(a => a.Status = (int)Bussiness.Enums.OutStatusCaption.SendedPickOrder);
        //     //   List<OutMaterialLabelDto> labelList = OutMaterialLabelDtos.Where(a => a.OutCode == entity.Code && a.Status == (int)Bussiness.Enums.OutStatusCaption.WaitSending).ToList();
        //        // labelList.ForEach(a => a.Status = (int)Bussiness.Enums.OutStatusCaption.SendedPickOrder);
        //        List<OutMaterialLabel> labelList1 = OutMaterialLabels.Where(a => a.OutCode == entity.Code && a.Status == (int)Bussiness.Enums.OutStatusCaption.WaitSending).ToList();



        //        DpsInterfaceMain main = new DpsInterfaceMain();
        //        main.ProofId = Guid.NewGuid().ToString();
        //        main.CreateDate = DateTime.Now;
        //        main.Status = 0;
        //        main.OrderType = 1;
        //        main.OrderCode = entity.Code;
        //        if (OutRepository.Update(entity) < 0)
        //        {
        //            return DataProcess.Failure("更新失败");
        //        }
        //        foreach (OutMaterial item in list)
        //        {
        //            if (OutMaterialRepository.Update(item) < 0)
        //            {
        //                return DataProcess.Failure("更新失败");
        //            }
        //        }
        //        //foreach (OutMaterialLabelDto item in labelList)
        //        //{
        //        //    OutMaterialLabel outLabel = labelList1.FirstOrDefault(a => a.Id == item.Id);
        //        //    outLabel.Status = (int)Bussiness.Enums.OutStatusCaption.SendedPickOrder;
        //        //    DpsInterface dpsInterface = new DpsInterface();
        //        //    dpsInterface.BatchNO = item.BatchCode;
        //        //    dpsInterface.CreateDate = DateTime.Now;
        //        //    dpsInterface.GoodsName = item.MaterialName;
        //        //    dpsInterface.LocationId = item.LocationCode;
        //        //    dpsInterface.MakerName = item.SupplierName;
        //        //    dpsInterface.MaterialLabelId = item.Id;
        //        //    dpsInterface.ProofId = main.ProofId;
        //        //    dpsInterface.Quantity = Convert.ToInt32(item.Quantity);
        //        //    dpsInterface.RealQuantity = 0;
        //        //    dpsInterface.RelationId = item.Id;
        //        //    dpsInterface.Spec = item.MaterialUnit;
        //        //    dpsInterface.Status = 0;
        //        //    dpsInterface.ToteId = item.MaterialLabel;
        //        //    dpsInterface.OrderCode = main.OrderCode;
        //        //    if (!DpsInterfaceRepository.Insert(dpsInterface))
        //        //    {
        //        //        return DataProcess.Failure("发送PTL任务失败");
        //        //    }

        //        //    if (OutMaterialLabelRepository.Update(outLabel) < 0)
        //        //    {
        //        //        return DataProcess.Failure("更新失败");
        //        //    }
        //        //}
        //        if (!DpsInterfaceMainRepository.Insert(main))
        //        {
        //            return DataProcess.Failure("发送PTL任务失败");
        //        }
        //        OutRepository.UnitOfWork.Commit();
        //    }
        //    catch (Exception ex)
        //    {

        //        return DataProcess.Failure("发送PTL任务失败:" + ex.Message);
        //    }
        //    return DataProcess.Success("发送PTL成功");

        //}
        /// <summary>
        /// 人工手动选择拣货
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //public DataResult ConfirmHandPicked(Out entity)
        //{

        //    if (entity.Status != (int)Bussiness.Enums.OutStatusCaption.PartSending && entity.Status != (int)Bussiness.Enums.OutStatusCaption.WaitSending)
        //    {
        //        return DataProcess.Failure("出库单状态不对,应为任务待发送或者手动执行中状态");
        //    }
        //    if (entity.PickedStockList == null || entity.PickedStockList.Count() <= 0)
        //    {
        //        return DataProcess.Failure("未选择出库条码");
        //    }
        //    OutMaterialLabelRepository.UnitOfWork.TransactionEnabled = true;
        //    List<string> labelList = entity.PickedStockList.Select(a => a.MaterialLabel).ToList();
        //    List<OutMaterial> outMaterialList = OutMaterials.Where(a => a.OutCode == entity.Code && (a.Status == (int)Bussiness.Enums.OutStatusCaption.WaitSending || a.Status == (int)Bussiness.Enums.OutStatusCaption.PartSending)).ToList();
        //    List<OutMaterialLabel> outLabelList = new List<Bussiness.Entitys.OutMaterialLabel>();
        //    List<OutMaterialLabel> outLabelListCopy = new List<Bussiness.Entitys.OutMaterialLabel>();
        //    foreach (StockDto item in entity.PickedStockList)
        //    {
        //        Stock stock = StockRepository.GetEntity(item.Id);
        //        if (stock == null)
        //        {
        //            return DataProcess.Failure("物料条码:" + item.MaterialLabel + "已不再库存");
        //        }
        //        if (stock.Quantity - item.PickedQuantity - stock.LockedQuantity < 0)
        //        {
        //            return DataProcess.Failure("物料条码:" + item.MaterialLabel + "可用库存不足");
        //        }

        //        stock.LockedQuantity = stock.LockedQuantity + item.PickedQuantity;
        //        stock.IsLocked = true;
        //        StockRepository.Update(stock);
        //        //if (stock.Quantity == 0)
        //        //{
        //        //    StockRepository.Delete(stock);
        //        //}
        //        //else
        //        //{
        //        //    StockRepository.Update(stock);
        //        //}
        //        OutMaterialLabel label = new OutMaterialLabel
        //        {
        //            BatchCode = stock.BatchCode,
        //            BillCode = entity.BillCode,
        //            IsDeleted = false,
        //            LocationCode = stock.LocationCode,
        //            MaterialCode = item.MaterialCode,
        //            MaterialLabel = stock.MaterialLabel,
        //            OutCode = entity.Code,
        //            OutMaterialId = 0,
        //            Quantity = item.PickedQuantity,
        //            Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending,
        //            WareHouseCode = stock.WareHouseCode,
        //            AreaCode = stock.AreaCode,
        //            RealPickedQuantity = item.PickedQuantity,
        //            PickedTime = DateTime.Now,
        //            Operator = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code,
        //    };
        //        outLabelList.Add(label);
        //        outLabelListCopy.Add(label);

        //    }

        //    var group = outMaterialList.GroupBy(a => a.MaterialCode);
        //    foreach (var item in group)
        //    {
        //        var list = outLabelListCopy.Where(a => a.MaterialCode == item.Key).ToList();
        //        foreach (var OutMaterial in item)
        //        {
        //            var outmaterial = outMaterialList.FirstOrDefault(a => a.Id == OutMaterial.Id);
        //            foreach (var label in list)
        //            {
        //                //var label = list.FirstOrDefault(a=>a.MaterialLabel==outlabel.MaterialLabel);
        //                if (label.RealPickedQuantity<=0)
        //                {
        //                    continue;
        //                }
        //                if (label.RealPickedQuantity.GetValueOrDefault(0) >= outmaterial.Quantity - outmaterial.PickedQuantity.GetValueOrDefault(0))
        //                {

        //                    label.RealPickedQuantity = label.RealPickedQuantity - (outmaterial.Quantity - outmaterial.PickedQuantity.GetValueOrDefault(0));
        //                    outmaterial.PickedQuantity = outmaterial.Quantity;
        //                    outmaterial.PickedTime = DateTime.Now;
        //                    outmaterial.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //                    OutMaterialRepository.Update(outmaterial);
        //                    break;
        //                }
        //                else
        //                {
        //                    outmaterial.PickedQuantity = outmaterial.PickedQuantity.GetValueOrDefault(0) + label.RealPickedQuantity;
        //                    outmaterial.PickedTime = DateTime.Now;
        //                    outmaterial.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //                    OutMaterialRepository.Update(outmaterial);
        //                    label.RealPickedQuantity = 0;
        //                }
        //            }
        //        }
        //    }

        //    if (!outMaterialList.Any(a=>a.Status<(int)Bussiness.Enums.OutStatusCaption.WaitSending))
        //    {
        //        Out outEntity = Outs.FirstOrDefault(a => a.Code == entity.Code);
        //        outEntity.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //        outEntity.ShelfEndTime = DateTime.Now;
        //        OutRepository.Update(outEntity);
        //    }
        //    else
        //    {
        //        Out outEntity = Outs.FirstOrDefault(a => a.Code == entity.Code);
        //        outEntity.Status = (int)Bussiness.Enums.OutStatusCaption.WaitSending;
        //        if (outEntity.ShelfStartTime==null)
        //        {
        //            outEntity.ShelfStartTime = DateTime.Now;
        //        }
        //        OutRepository.Update(outEntity);
        //    }

        //    outLabelList.ForEach(a => a.RealPickedQuantity = a.Quantity);
        //    foreach (var item in outLabelList)
        //    {
        //        OutMaterialLabelRepository.Insert(item);
        //    }
        //    OutMaterialLabelRepository.UnitOfWork.Commit();

        //    return DataProcess.Success("操作成功");



        //}


        //public DataResult ConfirmCheckLabel(OutMaterialLabel searchEntity)
        //{
        //    OutMaterialLabelRepository.UnitOfWork.TransactionEnabled = true;
        //    //foreach (var item in list)
        //    //{
        //    //    if (pickedQuantity>=item.Quantity && pickedQuantity>0)
        //    //    {
        //    //        item.PickedQuantity = item.Quantity;
        //    //        item.Status = (int)Bussiness.Enums.OutStatusCaption.Picked;
        //    //        item.PickedTime = DateTime.Now;
        //    //        OutMaterialRepository.Update(item);
        //    //        pickedQuantity = pickedQuantity - item.Quantity;
        //    //    }
        //    //    else
        //    //    {
        //    //        item.PickedQuantity = pickedQuantity;
        //    //        //item.Status = (int)Bussiness.Enums.OutStatusCaption.HandPicking;
        //    //        item.PickedTime = DateTime.Now;
        //    //    }
        //    //}
        //    //判断此条码是否属于该出库单号
        //    //var entity = OutMaterialLabels.FirstOrDefault(a => a.MaterialLabel == searchEntity.MaterialLabel && a.OutCode == searchEntity.OutCode && a.Id == searchEntity.Id);
        //    var entity = OutMaterialLabels.FirstOrDefault(a => a.MaterialLabel == searchEntity.MaterialLabel && a.OutCode == searchEntity.OutCode);
        //    if (entity==null)
        //    {
        //        return DataProcess.Failure("该条码不属于出库单:"+searchEntity.OutCode);
        //    }

        //    if (entity.Status!=(int)Bussiness.Enums.OutStatusCaption.WaitSending)
        //    {
        //        return DataProcess.Failure("该条码尚未拣货完成");
        //    }
        //    IQuery<OutMaterialLabel> labelList = OutMaterialLabels.Where(a => a.OutCode == entity.OutCode);
        //    List<OutMaterialLabel> materialLabelList = labelList.Where(a => a.MaterialCode == entity.MaterialCode && a.Id != entity.Id).ToList();
        //    entity.Status = (int)Bussiness.Enums.OutStatusCaption.Finished;
        //    entity.CheckedTime = DateTime.Now;
        //    entity.CheckedQuantity = searchEntity.CheckedQuantity;
        //    if (string.IsNullOrEmpty(entity.Checker))
        //    {
        //        entity.Checker = HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
        //    }

        //    //扣减库存
        //    Stock stock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel);
        //    stock.Quantity = stock.Quantity - entity.CheckedQuantity.GetValueOrDefault(0);
        //    stock.LockedQuantity = stock.LockedQuantity - entity.RealPickedQuantity.GetValueOrDefault(0);
        //    if (stock.LockedQuantity == 0)
        //    {
        //        stock.IsLocked = false;
        //    }
        //    if (stock.Quantity == 0)
        //    {
        //        StockRepository.Delete(stock);
        //    }
        //    else
        //    {
        //        StockRepository.Update(stock);
        //    }
        //    //判断是否全部复核完毕或者 复核部分
        //    //区分 手动拣货 还是自动分配。。
        //    var outEntity = Outs.FirstOrDefault(a => a.Code == entity.OutCode);
        //    //判断是否拣选完毕
        //    if (outEntity.Status==(int)Bussiness.Enums.OutStatusCaption.WaitSending)
        //    {
        //        if (!labelList.Any(a => a.Status < (int)Bussiness.Enums.OutStatusCaption.Finished && a.Id != entity.Id))
        //        {
        //            var outMaterialList = OutMaterials.Where(a => a.OutCode == entity.OutCode).ToList();
        //            var group = outMaterialList.GroupBy(a => a.MaterialCode);
        //            foreach (var item in group)
        //            {
        //                var checkedQuantity = labelList.Where(a => a.MaterialCode == item.Key).Sum(a => a.CheckedQuantity);
        //                if (item.Key==entity.MaterialCode)
        //                {
        //                    checkedQuantity = checkedQuantity.GetValueOrDefault(0) + entity.CheckedQuantity;
        //                }
        //                foreach (var obj in item)
        //                {
        //                    var outmaterial = outMaterialList.FirstOrDefault(a => a.Id == obj.Id);
        //                    if (checkedQuantity>=outmaterial.PickedQuantity)
        //                    {
        //                        outmaterial.CheckedQuantity = outmaterial.PickedQuantity;
        //                        outmaterial.Status= (int)Bussiness.Enums.OutStatusCaption.Finished;
        //                        OutMaterialRepository.Update(outmaterial);
        //                        checkedQuantity = checkedQuantity - outmaterial.PickedQuantity;
        //                    }
        //                    else
        //                    {
        //                        outmaterial.CheckedQuantity = checkedQuantity;
        //                        outmaterial.Status = (int)Bussiness.Enums.OutStatusCaption.Finished;
        //                        OutMaterialRepository.Update(outmaterial);
        //                        checkedQuantity = 0;
        //                    }
        //                }
        //            }
        //            outEntity.Status = (int)Bussiness.Enums.OutStatusCaption.Finished;
        //            outEntity.ShelfEndTime = DateTime.Now;
        //            OutRepository.Update(outEntity);
        //        }
        //    }

        //    OutMaterialLabelRepository.Update(entity);
        //    OutMaterialLabelRepository.UnitOfWork.Commit();
        //    return DataProcess.Success("操作成功");
        //}




    }
}
