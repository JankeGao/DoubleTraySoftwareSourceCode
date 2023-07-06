using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
    public class InServer : Contracts.IInContract
    {
        
        /// <summary>
        /// 入库单中间表
        /// </summary>
        public IRepository<InIF, int> InIFRepository { get; set; }
        /// <summary>
        /// 入库单契约
        /// </summary>
        public IInContract InContract { set; get; }
        public IRepository<InTaskMaterial, int> InTaskMaterialRepository { get; set; }
        public IRepository<InTask, int> InTaskRepository { get; set; }
        public IRepository<InMaterialIF, int> InMaterialIFRepository { get; set; }
        public IRepository<Material, int> MaterialRepository { get; set; }
        public IRepository<In, int> InRepository { get; set; } 
        public IRepository<InMaterial, int> InMaterialRepository { get; set; }
        public IRepository<InMaterialLabel, int> InMaterialLabelRepository { get; set; }
        public IRepository<Entitys.Stock, int> StockRepository { get; set; }
        public IRepository<Entitys.Location, int> LocationRepository { get; set; }

        public IRepository<HPC.BaseService.Models.Dictionary,int> DictionaryRepository { get; set; }
        public IQuery<In> Ins => InRepository.Query();

        public IQuery<InMaterial> InMaterials => InMaterialRepository.Query();

        public ISequenceContract SequenceContract { set; get; }

        public ILabelContract LabelContract { set; get; }

        public ISupplyContract SupplyContract { set; get; }

        public ICheckContract CheckContract { get; set; }

        public IInTaskContract InTaskContract { get; set; }

        public IMaterialContract MaterialContract { get; set; }

        public IMapper Mapper { set; get; }

        public IQuery<Material> Materials
        {
            get
            {
                return MaterialRepository.Query();
            }
        }

        /// <summary>
        /// 物料载具关联
        /// </summary>

        public IQuery<InMaterialDto> InMaterialDtos => InMaterials.InnerJoin(MaterialRepository.Query(), (inMaterial, material) => inMaterial.MaterialCode == material.Code)
            .InnerJoin(Ins,(inMaterial, material,ins)=>inMaterial.InCode==ins.Code)
            .LeftJoin(WareHouseContract.WareHouses, (inMaterial, material, ins, warehouse) => ins.WareHouseCode == warehouse.Code)
            .LeftJoin(SupplyContract.Supplys, (inMaterial, material, ins, warehouse,supply)=> inMaterial.SupplierCode==supply.Code)
            .Select((inMaterial, material, ins, warehouse, supply) => new Dtos.InMaterialDto
            {
            Id = inMaterial.Id,
            InCode = inMaterial.InCode,
            MaterialCode = inMaterial.MaterialCode,
            Quantity = inMaterial.Quantity,
            ManufactrueDate = inMaterial.ManufactrueDate,
            BatchCode = inMaterial.BatchCode,
            MaterialType= material.MaterialType,
            SupplierCode = inMaterial.SupplierCode,
            SupplierName = supply.Name,
            Status = inMaterial.Status,
            IsDeleted = inMaterial.IsDeleted,
            BillCode = inMaterial.BillCode,
            CustomCode = inMaterial.CustomCode,
            PackageQuantity= material.PackageQuantity,
            SendInQuantity= inMaterial.SendInQuantity,
            CustomName = inMaterial.CustomName,
            MaterialLabel = inMaterial.MaterialLabel,
            SuggestLocation = inMaterial.SuggestLocation,
            LocationCode = inMaterial.LocationCode,
            ShelfTime = inMaterial.ShelfTime,
            CreatedUserCode = inMaterial.CreatedUserCode,
            CreatedUserName = inMaterial.CreatedUserName,
            CreatedTime = inMaterial.CreatedTime,
            UpdatedUserCode = inMaterial.UpdatedUserCode,
            UpdatedUserName = inMaterial.UpdatedUserName,
            UpdatedTime = inMaterial.UpdatedTime,
            MaterialName = material.Name,
            MaterialUnit = material.Unit,
            ItemNo = inMaterial.ItemNo,
            WareHouseCode = ins.WareHouseCode,
            WareHouseName=warehouse.Name,
            RealInQuantity =inMaterial.RealInQuantity,
            ValidityDate=inMaterial.ValidityDate,
            });

        public IRepository<Entitys.DpsInterface, int> DpsInterfaceRepository { get; set; }
        public IRepository<Entitys.DpsInterfaceMain, int> DpsInterfaceMainRepository { get; set; }
        public IQuery<InDto> InDtos {
            get {
               return Ins
                    .LeftJoin(DictionaryRepository.Query(), (inentity, dictionary) => inentity.InDict == dictionary.Code)
                    .LeftJoin(WareHouseContract.WareHouses, (inentity, dictionary, warehouse) => inentity.WareHouseCode == warehouse.Code)
                    .Select((inentity, dictionary, warehouse) => new Dtos.InDto()
                    {
                       Id=inentity.Id,
                       Code=inentity.Code,
                       BillCode=inentity.BillCode,
                       WareHouseCode= inentity.WareHouseCode,
                       WareHouseName=warehouse.Name,
                       InDict= inentity.InDict,
                       Status= inentity.Status,
                       Remark = inentity.Remark,
                       IsDeleted= inentity.IsDeleted,
                       BillFields= inentity.BillFields,
                       ShelfStartTime=inentity.ShelfStartTime,
                       ShelfEndTime= inentity.ShelfEndTime,
                       CreatedUserCode = inentity.CreatedUserCode,
                       CreatedUserName = inentity.CreatedUserName,
                       CreatedTime = inentity.CreatedTime,
                       UpdatedUserCode = inentity.UpdatedUserCode,
                       UpdatedUserName = inentity.UpdatedUserName,
                       UpdatedTime = inentity.UpdatedTime,
                       InDictDescription = dictionary.Name,
                       InDate = inentity.InDate,
                       PickOrderCode = inentity.PickOrderCode,
                       OrderType = inentity.OrderType
                    });
            }

        }

        public IWareHouseContract WareHouseContract { get; set; }


        /// <summary>
        /// 轮训接口--创建WMS入库单
        /// </summary>
        /// <param name="entity"></param>
        public DataResult CreateInEntityInterFace()
        {
            #region Oracle数据获取处理
            int count = 0;
            In entity = new In(); // 创建In类型的实体entity
            List<In> entityList = new List<In>(); // 定义一个集合，用于存储遍历得到的实体entityes
            InMaterial inMaterialObj = null;
            string currentBillCode = null;
            var connectionString = "Data Source=192.168.3.168:1521/orcl;User ID=zcdx;Password=Oracle#gCsb#2023";
            var query = "SELECT * FROM V_WK_WMS_EN_WAREH_BILL";
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
                if (resultList.Count > 0)
                {

                    foreach (var dict in resultList)
                    {
                        In entityes = new In
                        {
                            Id = 0,
                            Code = dict["BILL_CODE"].ToString(),
                            Remark = "",
                            WareHouseCode = "01",
                            CreatedTime = (DateTime)dict["BILL_DATE"],
                            AddMaterial = new List<InMaterial>(),
                        };

                        inMaterialObj = new InMaterial
                        {
                            MaterialCode = dict["MATERIALCODE"].ToString(),
                            Quantity = (decimal)dict["QTY"],
                            ManufactrueDate = (DateTime?)dict["BILL_DATE"],
                            CRRCID = dict["ID"].ToString(),
                            Status = 0,
                            RealInQuantity = 0,
                            BatchCode = "",

                        };

                        if (!Ins.Any(a => a.Code == entityes.Code))
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
                                entity = new In
                                {
                                    Id = 0,
                                    Code = dict["BILL_CODE"].ToString(),
                                    Remark = "",
                                    WareHouseCode = "01",
                                    CreatedTime = (DateTime)dict["BILL_DATE"],
                                    AddMaterial = new List<InMaterial>(),
                                };
                            }
                        }
                    }

                    foreach (var tempEntity in entityList)
                    {
                        entity.AddMaterial.AddRange(tempEntity.AddMaterial); // 将集合中的每个entityes的AddMaterial加入到entity中
                    }

                    if(entityList.Count <= 0)
                    {
                        return DataProcess.Failure("暂无新增入库单");
                    }

                    InRepository.UnitOfWork.TransactionEnabled = true;
                    {
                        // 判断是否有入库单号
                        if (String.IsNullOrEmpty(entity.Code))
                        {
                            entity.Code = SequenceContract.Create(entity.GetType());
                        }
                        if (Ins.Any(a => a.Code == entity.Code))
                        {
                            return DataProcess.Failure("该入库单号已存在");
                        }
                        foreach (var inMaterial in entity.AddMaterial)
                        {
                            if (!Regex.IsMatch(inMaterial.Quantity.ToString(), @"^[0-9]*(\.[0-9]{1,2})?$"))
                            {
                                return DataProcess.Failure("请输入正确的数字格式（包含两位小数的数字或者不包含小数的数字）");
                            }
                        }
                        entity.Status = entity.Status == null ? 0 : entity.Status; // 手动入库，状态为已完成
                        entity.OrderType = entity.OrderType == null ? 0 : entity.OrderType; // 单据来源

                        if (!InRepository.Insert(entity))
                        {
                            return DataProcess.Failure(string.Format("入库单{0}新增失败", entity.Code));
                        }

                        if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
                        {
                            foreach (InMaterial item in entity.AddMaterial)
                            {
                                item.InCode = entity.Code;
                                item.BillCode = entity.BillCode;
                                item.SendInQuantity = item.SendInQuantity.GetValueOrDefault(0) == 0 ? 0 : item.SendInQuantity;
                                item.Status = item.Status == null ? 0 : item.Status;
                                item.InDict = entity.InDict;
                                item.ItemNo = (entity.AddMaterial.IndexOf(item) + 1).ToString();
                                if (Materials.Any(a => a.Code == inMaterialObj.MaterialCode))
                                {
                                    DataResult result = CreateInMaterialEntity(item);
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

                            var queryes = WareHouseContract.LocationVIEWs.Where(a => a.WareHouseCode == entity.WareHouseCode);

                            // 查询可存放库位
                            // 本身已经了存放该物料-MaterialCode是中关联的MaterialCode
                            // 不存放该物料，但是不是存储锁定的
                            // 建议物料是该物料-SuggestMaterialCode是储位绑定的物料
                            queryes = queryes.Where(a => (a.MaterialCode == item.MaterialCode));
                            var a1 = queryes.ToList();
                            // 入库是存储锁定，则必须存放在所绑定的载具中,及该载具维护的物料即为待入库物料
                            if (mateialEntity.IsNeedBlock)
                            {
                                queryes = queryes.Where(a => a.SuggestMaterialCode == item.MaterialCode);
                            }
                            var a2 = queryes.ToList();
                            //如果不允许混批
                            if (!mateialEntity.IsMaxBatch)
                            {
                                // 批次相等，或者库存中没有存放物料
                                queryes = queryes.Where(a => a.BatchCode == item.BatchCode || string.IsNullOrEmpty(a.MaterialLabel));
                                var a4 = queryes.ToList();
                                // 并且不是锁定批次的盒子
                                queryes = queryes.Where(a => !a.IsLocked);
                            }
                            var a3 = queryes.ToList();
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
                            var locationList = queryes.ToList();

                            var stockLocatonList = queryes.Where(a => a.Quantity > 0).OrderBy(a => a.Code).ToList();//有库存的库位

                            var NoStockLocationList = queryes.Where(a => a.Quantity == null && (a.LockMaterialCode == item.MaterialCode || string.IsNullOrEmpty(a.LockMaterialCode))).OrderBy(a => a.Code).ToList();//没有库存的库位;


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

                    InRepository.UnitOfWork.Commit();

                }

                return DataProcess.Success(string.Format("入库单同步成功,共有{0}条增加", count));
                #endregion

                #region ADO.NET 数据获取处理
                //int count = 0;
                //In entity = null; // 创建In类型的实体entity
                //List<In> entityList = new List<In>(); // 定义一个集合，用于存储遍历得到的实体entityes
                //InMaterial inMaterialObjes = null;
                //string currentBillCode = null;
                //var createdTime = Ins.Max(a => a.CreatedTime);
                //var connectionString = "Data Source=DESKTOP-71I0RDA;Initial Catalog=SMDB;User ID=sa;Password=123456";
                //var query = "SELECT * FROM View_ConnectionTest";

                //using (var connection = new SqlConnection(connectionString))
                //{
                //    var command = new SqlCommand(query, connection);
                //    connection.Open();
                //    var reader = command.ExecuteReader();
                //    var resultList = new List<Dictionary<string, object>>();
                //    while (reader.Read())
                //    {
                //        var row = new Dictionary<string, object>();
                //        for (int i = 0; i < reader.FieldCount; i++)
                //        {
                //            row.Add(reader.GetName(i), reader.GetValue(i));
                //        }
                //        resultList.Add(row);
                //    }
                //    reader.Close();
                //    if (resultList.Count > 0)
                //    {

                //        foreach (var dict in resultList)
                //        {
                //            entity = new In
                //            {
                //                Id = 0,
                //                Code = dict["BILL_CODE"].ToString(),
                //                Remark = "",
                //                WareHouseCode = "01",
                //                CreatedTime = (DateTime)dict["BILL_DATE"],
                //                AddMaterial = new List<InMaterial>(),
                //            };

                //            In entityes = new In
                //            {
                //                Id = 0,
                //                Code = dict["BILL_CODE"].ToString(),
                //                Remark = "",
                //                WareHouseCode = "01",
                //                CreatedTime = (DateTime)dict["BILL_DATE"],
                //                AddMaterial = new List<InMaterial>(),
                //            };

                //            inMaterialObjes = new InMaterial
                //            {
                //                MaterialCode = dict["MATERIALCODE"].ToString(),
                //                Quantity = (decimal)dict["QTY"],
                //                ManufactrueDate = (DateTime?)dict["BILL_DATE"],
                //                CRRCID = dict["ID"].ToString(),
                //                Status = 0,
                //                RealInQuantity = 0,
                //                BatchCode = "",

                //            };

                //            if (!Ins.Any(a => a.Code == entityes.Code))
                //            {

                //                if (currentBillCode != null && dict["BILL_CODE"].ToString() != currentBillCode)
                //                {
                //                    continue;
                //                }
                //                else
                //                {
                //                    currentBillCode = dict["BILL_CODE"].ToString();
                //                    entityes.AddMaterial.Add(inMaterialObjes);
                //                    entityList.Add(entityes); // 将entityes加入到集合中
                //                }
                //            }

                //        }

                //        foreach (var tempEntity in entityList)
                //        {
                //            entity.AddMaterial.AddRange(tempEntity.AddMaterial); // 将集合中的每个entityes的AddMaterial加入到entity中
                //        }

                //        InRepository.UnitOfWork.TransactionEnabled = true;
                //        {
                //            // 判断是否有入库单号
                //            if (String.IsNullOrEmpty(entity.Code))
                //            {
                //                entity.Code = SequenceContract.Create(entity.GetType());
                //            }
                //            if (Ins.Any(a => a.Code == entity.Code))
                //            {
                //                return DataProcess.Failure("该入库单号已存在");
                //            }
                //            foreach (var inMaterial in entity.AddMaterial)
                //            {
                //                if (!Regex.IsMatch(inMaterial.Quantity.ToString(), @"^[0-9]*(\.[0-9]{1,2})?$"))
                //                {
                //                    return DataProcess.Failure("请输入正确的数字格式（包含两位小数的数字或者不包含小数的数字）");
                //                }
                //            }
                //            entity.Status = entity.Status == null ? 0 : entity.Status; // 手动入库，状态为已完成
                //            entity.OrderType = entity.OrderType == null ? 0 : entity.OrderType; // 单据来源

                //            if (!InRepository.Insert(entity))
                //            {
                //                return DataProcess.Failure(string.Format("入库单{0}新增失败", entity.Code));
                //            }

                //            if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
                //            {
                //                foreach (InMaterial item in entity.AddMaterial)
                //                {
                //                    item.InCode = entity.Code;
                //                    item.BillCode = entity.BillCode;
                //                    item.SendInQuantity = item.SendInQuantity.GetValueOrDefault(0) == 0 ? 0 : item.SendInQuantity;
                //                    item.Status = item.Status == null ? 0 : item.Status;
                //                    item.InDict = entity.InDict;
                //                    item.ItemNo = (entity.AddMaterial.IndexOf(item) + 1).ToString();
                //                    DataResult result = CreateInMaterialEntity(item);
                //                    if (!result.Success)
                //                    {
                //                        return DataProcess.Failure(result.Message);
                //                    }
                //                }
                //            }
                //        }
                //        count++;

                //        InRepository.UnitOfWork.Commit();

                //    }

                //    return DataProcess.Success(string.Format("入库单同步成功,共有{0}条增加", count));

                #endregion

                #region 原同步代码
                //try
                //{
                //    InIFRepository.UnitOfWork.TransactionEnabled = true;
                //    var list = InIFRepository.Query().Where(a => a.Status == (int)InterFaceBCaption.Waiting).ToList();
                //    int count = 0;
                //    foreach (var item in list)
                //    {
                //        // 判断该来源单据号是否已存在入库单
                //        if (Ins.Any(a => a.BillCode == item.BillCode))
                //        {
                //            item.Status = (int)OrderEnum.Error;
                //            item.Remark = "来源单据号" + item.BillCode + "已存在";
                //            InIFRepository.Update(item);
                //            break;
                //        }
                //        int errorflag = 0;
                //        var inEnity = new In()
                //        {
                //            BillCode = item.BillCode,
                //            WareHouseCode = item.WareHouseCode,
                //            InDict = item.InDict,
                //            InDate = item.InDate,
                //            Status = (int)InStatusCaption.WaitingForShelf,
                //            AddMaterial = new List<Bussiness.Entitys.InMaterial>(),
                //            OrderType = (int)OrderTypeEnum.Other,
                //            //Remark = item.Remark
                //        };
                //        var materialList = InMaterialIFRepository.Query().Where(a => a.BillCode == item.BillCode).ToList();
                //        foreach (var inMaterial in materialList)
                //        {
                //            inMaterial.Status = (int)OrderEnum.Wait;
                //            if (MaterialContract.Materials.FirstOrDefault(a => a.Code == inMaterial.MaterialCode) == null)
                //            {
                //                item.Status = (int)OrderEnum.Error;
                //                errorflag = 1;
                //                inMaterial.Status = (int)OrderEnum.Error;
                //                inMaterial.Remark = "物料编码" + inMaterial.MaterialCode + "不存在!";
                //                InMaterialIFRepository.Update(inMaterial);
                //                break;
                //            }

                //            var inMaterialEntity = new InMaterial()
                //            {
                //                BillCode = inMaterial.BillCode,
                //                Status = 0,
                //                InDict = inMaterial.InDict,
                //                SendInQuantity = 0,
                //                MaterialCode = inMaterial.MaterialCode,
                //                Quantity = inMaterial.Quantity,
                //                ManufactrueDate = inMaterial.ManufactrueDate,
                //                BatchCode = inMaterial.BatchCode,
                //                SupplierCode = inMaterial.SupplierCode,
                //                SupplierName = inMaterial.SupplierName,
                //                ItemNo = inMaterial.ItemNo
                //            };
                //            inEnity.AddMaterial.Add(inMaterialEntity);
                //            InMaterialIFRepository.Update(inMaterial);
                //        }

                //        if (errorflag == 1)
                //        {
                //            InIFRepository.Update(item);
                //        }
                //        else
                //        {
                //            if (!CreateInEntity(inEnity).Success)
                //            {
                //                return DataProcess.Failure("该入库单号已存在");
                //            }
                //            item.Status = (int)OrderEnum.Wait;
                //            InIFRepository.Update(item);
                //            count += 0;
                //        }
                //    }
                //    InIFRepository.UnitOfWork.Commit();
                //    return DataProcess.Success(string.Format("入库单同步成功,共有{0}条增加", count));
                //}
                //catch (Exception ex)
                //{
                //    return null;
                //}
                #endregion

            }
        }

        public DataResult CreateInEntity(In entity)
        {
            InRepository.UnitOfWork.TransactionEnabled = true;
            {
                // 判断是否有入库单号
                if (String.IsNullOrEmpty(entity.Code))
                {
                    entity.Code = SequenceContract.Create(entity.GetType());
                }
                if (Ins.Any(a => a.Code == entity.Code))
                {
                    return DataProcess.Failure("该入库单号已存在");
                }
                foreach (var inMaterial in entity.AddMaterial)
                {
                    if (!Regex.IsMatch(inMaterial.Quantity.ToString(), @"^[0-9]*(\.[0-9]{1,2})?$"))
                    {
                        return DataProcess.Failure("请输入正确的数字格式（包含两位小数的数字或者不包含小数的数字）");
                    }
                }
                entity.Status = entity.Status == null ? 0 : entity.Status; // 手动入库，状态为已完成
                entity.OrderType= entity.OrderType == null ? 0 : entity.OrderType; // 单据来源

                if (!InRepository.Insert(entity))
                {
                    return DataProcess.Failure(string.Format("入库单{0}新增失败", entity.Code));
                }

                if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
                {
                    foreach (InMaterial item in entity.AddMaterial)
                    {
                        item.InCode = entity.Code;
                        item.BillCode = entity.BillCode;
                        item.SendInQuantity = item.SendInQuantity.GetValueOrDefault(0)==0? 0 : item.SendInQuantity;
                        item.Status = item.Status == null ? 0 : item.Status;
                        item.InDict = entity.InDict;
                        item.ItemNo =(entity.AddMaterial.IndexOf(item)+1).ToString();
                        DataResult result = CreateInMaterialEntity(item);
                        if (!result.Success)
                        {
                            return DataProcess.Failure(result.Message);
                        }
                    }
                }
            }
            InRepository.UnitOfWork.Commit();

            return DataProcess.Success(string.Format("入库单{0}新增成功", entity.Code));
        }

        public DataResult CreateInMaterialEntity(InMaterial entity)
        {

            if (!string.IsNullOrEmpty(entity.ManufactrueDate.ToString()))
            {
                // 计算该物料到期日期
                if (MaterialContract.MaterialRepository.GetEntity(a => a.Code == entity.MaterialCode).ValidityPeriod > 0)
                {
                    var days = MaterialContract.MaterialRepository.GetEntity(a => a.Code == entity.MaterialCode)
                        .ValidityPeriod;
                    
                    entity.ValidityDate = entity.ManufactrueDate.Value.AddDays(days);
                }
            }

            try
            {
                if (InMaterialRepository.Insert(entity))
                {
                    return DataProcess.Success(string.Format("入库物料{0}新增成功", entity.MaterialLabel));
                }
            }
            catch (System.Exception ex)
            {
                return DataProcess.Failure("操作失败");
            }
            return DataProcess.Failure("操作失败");
        }

        public DataResult RemoveInMaterial(int id)
        {
            InMaterial entity = InMaterialRepository.GetEntity(id);
            if (entity.Status != (int)Enums.InStatusCaption.WaitingForShelf)
            {
                return DataProcess.Failure("该入库物料条码执行中或已完成");
            }
            if (InMaterialRepository.Delete(id) > 0)
            {
                return DataProcess.Success(string.Format("入库条码{0}删除成功", entity.MaterialLabel));
            }
            return DataProcess.Failure("操作失败");
        }

        public DataResult RemoveIn(int id)
        {
            In entity = InRepository.GetEntity(id);
            if (entity.Status != (int)Enums.InStatusCaption.WaitingForShelf)
            {
                return DataProcess.Failure("该入库单执行中或已完成");
            }

            InRepository.UnitOfWork.TransactionEnabled = true;
            if (entity.Status != (int)Enums.InStatusCaption.WaitingForShelf)
            {
                return DataProcess.Failure("该入库单执行中或已完成");
            }
            if (InRepository.Delete(id) <= 0)
            {
                return DataProcess.Failure(string.Format("入库单{0}删除失败", entity.Code));
            }
            List<InMaterial> list = InMaterials.Where(a => a.InCode == entity.Code).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (InMaterial item in list)
                {
                    DataResult result = RemoveInMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }
            InRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }


        public DataResult CancelInMaterial(int id)
        {
            InMaterial entity = InMaterialRepository.GetEntity(id);
            if (entity.Status != (int)Enums.InStatusCaption.WaitingForShelf)
            {
                return DataProcess.Failure("该入库物料条码执行中或已完成");
            }
            if (InMaterialRepository.Delete(id) > 0)
            {
                return DataProcess.Success(string.Format("入库条码{0}删除成功", entity.MaterialLabel));
            }
            return DataProcess.Failure("操作失败");
        }

        /// <summary>
        /// 作废入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CancelIn(In entity)
        {

            if (InTaskContract.InTasks.Any(a => a.InCode == entity.Code && a.Status != (int)InTaskStatusCaption.WaitingForShelf))
            {
                return DataProcess.Failure(string.Format("该入库单存在执行中或已完成的入库任务，无法作废"));
            }

            entity.Status = (int) InStatusCaption.Cancel;
            InRepository.UnitOfWork.TransactionEnabled = true;

            //作废入库单
            if (InRepository.Update(entity) <= 0)
            {
                return DataProcess.Failure(string.Format("入库单{0}作废失败", entity.Code));
            }

            //作废入库物料
            List<InMaterial> list = InMaterials.Where(a => a.InCode == entity.Code).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (InMaterial item in list)
                {
                    item.Status = (int)InStatusCaption.Cancel;
                    if (InMaterialRepository.Update(item) <= 0)
                    {
                        return DataProcess.Failure(string.Format("入库单{0}作废失败", entity.Code));
                    }
                }
            }

            //作废入库任务单
            List<InTask> tasklist = InTaskContract.InTasks.Where(a => a.InCode == entity.Code).ToList();
            if (tasklist != null && tasklist.Count > 0)
            {
                foreach (InTask item in tasklist)
                {
                    item.Status = (int)InTaskStatusCaption.Cancel;
                    if (InTaskContract.InTaskRepository.Update(item) <= 0)
                    {
                        return DataProcess.Failure(string.Format("入库任务单{0}作废失败", entity.Code));
                    }
                }
            }

            //作废入库任务明细
            List<InTaskMaterialDto> taskMaterallist = InTaskContract.InTaskMaterialDtos.Where(a => a.InCode == entity.Code).ToList();
            if (taskMaterallist != null && taskMaterallist.Count > 0)
            {
                foreach (InTaskMaterialDto item in taskMaterallist)
                {
                    item.Status = (int)InTaskStatusCaption.Cancel;
                    // 物料实体映射
                    InTaskMaterial inTaskEntity = Mapper.MapTo<InTaskMaterial>(item);
                    if (InTaskContract.InTaskMaterialRepository.Update(inTaskEntity) <= 0)
                    {
                        return DataProcess.Failure(string.Format("入库任务单明细{0}作废失败", item.MaterialCode));
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

                    // 释放储位锁定数量
                    var locationEntity = WareHouseContract.Locations
                        .Where(a => a.Code == item.SuggestLocation)
                        .FirstOrDefault();

                    // 释放该储位的数量
                    locationEntity.LockQuantity = locationEntity.LockQuantity - item.Quantity;
                    if (locationEntity.LockQuantity==0)
                    {
                        locationEntity.LockMaterialCode = "";
                    }
                    locationEntity.IsLocked = false;
                    // 更新托盘储位推荐锁定的重量
                    if (WareHouseContract.LocationRepository.Update(locationEntity) <= 0)
                    {
                        return DataProcess.Failure(string.Format("储位数量锁定失败"));
                    }
                }
            }


            if (entity.OrderType == (int)OrderTypeEnum.Other)
            {
                var ifIn = InIFRepository.Query().FirstOrDefault(a => a.BillCode == entity.BillCode);
                if (ifIn != null)
                {
                    ifIn.Status = 3;//作废
                    InIFRepository.Update(ifIn);
                    InMaterialIFRepository.Update(a => new InMaterialIF() { Status = 3 }, p => p.BillCode == entity.BillCode);
                }
            }

            //作废时更新  接口状态
            InRepository.UnitOfWork.Commit();
            return DataProcess.Success("作废成功");
        }

        public DataResult EditIn(In entity)
        {
            InRepository.UnitOfWork.TransactionEnabled = true;
            if (InRepository.Update(entity) <= 0)
            {
                return DataProcess.Failure(string.Format("入库单{0}编辑失败", entity.Code));
            }

            List<InMaterial> list = InMaterials.Where(a => a.InCode == entity.Code).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (InMaterial item in list)
                {
                    DataResult result = RemoveInMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }

            if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
            {
                foreach (InMaterial item in entity.AddMaterial)
                {
                    item.InCode = entity.Code;
                    item.BillCode = entity.BillCode;
                    item.Status = 0;
                    item.SendInQuantity = 0;
                    item.InDict = entity.InDict;
                    DataResult result = CreateInMaterialEntity(item);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }
            InRepository.UnitOfWork.Commit();
            return DataProcess.Success("编辑成功");
        }

        /// <summary>
        /// 强制中止
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult ForceFinish(In entity)
        {
            return DataProcess.Success("强制中止成功！");
        }

        /// <summary>
        /// 确认入库物料
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult HandShelf(InMaterial entity)
        {

            if (entity.Status != (int)Bussiness.Enums.InStatusCaption.WaitingForShelf&& entity.Status != (int)Bussiness.Enums.InStatusCaption.Sheling)
            {
                return DataProcess.Failure("该物料条码状态不为待上架或执行中");
            }
            In inEntity = Ins.FirstOrDefault(a => a.Code == entity.InCode);
            //if (inEntity.Status != (int)Bussiness.Enums.InStatusCaption.WaitingForShelf && inEntity.Status != (int)Bussiness.Enums.InStatusCaption.HandShelf)
            //{
            //    return DataProcess.Failure("入库单状态不为待上架或者手动执行中");
            //}
            if (inEntity.Status != (int)Bussiness.Enums.InStatusCaption.WaitingForShelf && inEntity.Status != (int)Bussiness.Enums.InStatusCaption.HandShelf && inEntity.Status != (int)Bussiness.Enums.InStatusCaption.Sheling)
            {
                return DataProcess.Failure("入库单状态不为待上架或者手动执行中");
            }
            if (string.IsNullOrEmpty(entity.LocationCode))
            {
                return DataProcess.Failure("尚未选择上架库位");
            }
            Location location = LocationRepository.Query().FirstOrDefault(a => a.Code == entity.LocationCode);
            if (location == null)
            {
                return DataProcess.Failure(string.Format("系统不存在该上架库位{0}", entity.LocationCode));
            }
            //if (StockRepository.Query().Any(a=>a.MaterialLabel==entity.MaterialLabel))
            //{
            //    return DataProcess.Failure(string.Format("库存已存在该物料条码{0}", entity.MaterialLabel));
            //}

            var stock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel && a.LocationCode == location.Code);
            bool IsExistStock = false;
            entity.Status = (int)Bussiness.Enums.InStatusCaption.Finished;
            entity.ShelfTime = DateTime.Now;

            if (stock==null)
            {
                stock = new Stock
                {
                    AreaCode = location.ContainerCode,
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
                    MaterialCode = entity.MaterialCode,
                    MaterialLabel = entity.MaterialLabel,
                    SupplierCode=entity.SupplierCode,
                    MaterialStatus = 0,
                    Quantity = entity.Quantity,
                    SaleBillItemNo = "",
                    SaleBillNo = "",
                    StockStatus = 0,
                    LockedQuantity = 0
                };
            }
            else
            {
                IsExistStock = true;
                stock.Quantity = stock.Quantity + entity.Quantity;
            }
         

            if (inEntity.Status== (int)Bussiness.Enums.InStatusCaption.WaitingForShelf)
            {
                inEntity.ShelfStartTime = DateTime.Now;
                inEntity.Status = (int)Bussiness.Enums.InStatusCaption.HandShelf;
            }
            InMaterialRepository.UnitOfWork.TransactionEnabled = true;

            if (!InMaterialDtos.Where(a=>a.InCode==inEntity.Code && a.Id!=entity.Id).Any(a=>a.Status!=(int)Bussiness.Enums.InStatusCaption.Finished))
            {
                inEntity.Status = (int)Bussiness.Enums.InStatusCaption.Finished;
                inEntity.ShelfEndTime = DateTime.Now;

            }

            InMaterialRepository.Update(entity);
            InRepository.Update(inEntity);
            if (IsExistStock)
            {
                StockRepository.Update(stock);
            }
            else
            {
                StockRepository.Insert(stock);
            }
            InMaterialRepository.UnitOfWork.Commit();
            return DataProcess.Success("上架成功");
        }

        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult SendOrderToPTL(In entity)
        {
            try
            {
                //存在盘点计划 不允许发送

                if (CheckContract.Checks.Any(a=>a.Status<6 && a.WareHouseCode==entity.WareHouseCode))
                {
                    return DataProcess.Failure("该仓库尚有盘点计划,此时不允许发送PTL任务");
                }

                if (entity.Status != (int)(Bussiness.Enums.InStatusCaption.WaitingForShelf))
                {
                    return DataProcess.Failure("入库单状态不对,应为待上架状态");
                }

                //if (Ins.Any(a => a.Status == (int)Bussiness.Enums.InStatusCaption.Sheling))
                //{
                //    return DataProcess.Failure("入库单已发送至PTL");
                //}
                InRepository.UnitOfWork.TransactionEnabled = true;
                entity.Status = (int)Bussiness.Enums.InStatusCaption.Sheling;
                List<InMaterialDto> list = InMaterialDtos.Where(a => a.InCode == entity.Code && a.Status == (int)Bussiness.Enums.InStatusCaption.WaitingForShelf).ToList();
                List<InMaterial> list1 = InMaterials.Where(a => a.InCode == entity.Code && a.Status == (int)Bussiness.Enums.InStatusCaption.WaitingForShelf).ToList();

                DpsInterfaceMain main = new DpsInterfaceMain();
                main.ProofId = Guid.NewGuid().ToString();
                main.CreateDate = DateTime.Now;
                main.Status = 0;
                main.OrderType = 0;
                main.OrderCode = entity.Code;
                if (InRepository.Update(entity) < 0)
                {
                    return DataProcess.Failure("更新失败");
                }
                foreach (InMaterialDto item in list)
                {
                    InMaterial inMaterial = list1.FirstOrDefault(a => a.Id == item.Id);
                    inMaterial.Status = (int)Bussiness.Enums.InStatusCaption.Sheling;

                    DpsInterface dpsInterface = new DpsInterface();
                    dpsInterface.BatchNO = item.BatchCode;
                    dpsInterface.CreateDate = DateTime.Now;
                    dpsInterface.GoodsName = item.MaterialName;
                    dpsInterface.LocationId = item.LocationCode;
                    dpsInterface.MakerName = item.SupplierName;
                    dpsInterface.MaterialLabelId = 0;
                    dpsInterface.ProofId = main.ProofId;
                    dpsInterface.Quantity = Convert.ToInt32(item.Quantity);
                    dpsInterface.RealQuantity = 0;
                    dpsInterface.RelationId = item.Id;
                    dpsInterface.Spec = item.MaterialUnit;
                    dpsInterface.Status = 0;
                    dpsInterface.OrderCode = item.InCode;
                    dpsInterface.ToteId = item.MaterialLabel;
                    if (DpsInterfaceRepository.Insert(dpsInterface) == false)
                    {
                        return DataProcess.Failure("发送任务至PTL失败");
                    }
                    if (InMaterialRepository.Update(inMaterial) < 0)
                    {
                        return DataProcess.Failure("更新失败");
                    }
                }
                if (DpsInterfaceMainRepository.Insert(main)==false)
                {
                    return DataProcess.Failure("发送任务至PTL失败");
                }
                InRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {

                return DataProcess.Failure("发送任务至PTL失败:"+ex.Message);
            }

            return DataProcess.Success("发送PTL成功");

        }
    }
}
