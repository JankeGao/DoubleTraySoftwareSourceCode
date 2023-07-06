using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Core.Mapping;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;
using NPOI.SS.Util;

namespace Bussiness.Services
{
    public class StockServer : Contracts.IStockContract
    {

        public IRepository<InactiveStockVM, int> InactiveStockVMRepository { get; set; }

        public IQuery<Entitys.InactiveStockVM> InactiveStockVMs
        {
            get
            {
                return InactiveStockVMRepository.Query();
            }
        }

        public IRepository<MaterialStatusVM, int> MaterialStatusRepository { get; set; }

        public IQuery<Entitys.MaterialStatusVM> MaterialStatusS
        {
            get
            {
                return MaterialStatusRepository.Query();
            }
        }
        public IMapper Mapper { set; get; }
        public IRepository<InventoryStatusVM, int> InventoryStatusRepository { get; set; }

        public ISequenceContract SequenceContract { set; get; }

        public IRepository<Label,int> LabelRepository { get; set; }

        public IQuery<InventoryStatusVM> InventoryStatus
        {
            get
            {
                return InventoryStatusRepository.Query();

            }
        }

        public IRepository<Entitys.Stock, int> StockRepository { get; set; }

        public IRepository<Entitys.StockVM, int> StockVMRepository { get; set; }
        public IRepository<Entitys.Material, int> MaterialRepository { get; set; }

        public IRepository<Entitys.WareHouse, int> WareHouseRepository { get; set; }

        public IRepository<Entitys.Area, int> AreaRepository { get; set; }


        public IRepository<Entitys.Location, int> LocationRepository { get; set; }

        public IRepository<Entitys.MobileLocation,int> MobileRepository { get; set; }




        public ISupplyContract SupplyContract { set; get; }

        public IWareHouseContract WareHouseContract { set; get; }

        public IQuery<Entitys.Stock> Stocks {
            get
            {
                return StockRepository.Query();
            }
        }

        public IQuery<StockDto> StockDtos {
            get
            {
                return Stocks
                    .InnerJoin(MaterialRepository.Query(), (stocks, materials) => stocks.MaterialCode == materials.Code)
                    .InnerJoin(WareHouseRepository.Query(), (stocks, materials, warehouses) => stocks.WareHouseCode == warehouses.Code)
                    .InnerJoin(WareHouseContract.LocationVMs, (stocks, materials, warehouses,location) => stocks.LocationCode == location.Code)
                    .LeftJoin(SupplyContract.Supplys, (stocks, materials, warehouses, location,supply) => stocks.SupplierCode == supply.Code)
                    .Select((stocks, materials, warehouses, location, supply) => new Dtos.StockDto()
                    {
                        Id = stocks.Id,
                        MaterialLabel = stocks.MaterialLabel,
                        LocationCode = stocks.LocationCode,
                        MaterialStatus = stocks.MaterialStatus,
                        StockStatus = stocks.StockStatus,
                        ContainerCode= stocks.ContainerCode,
                        ShelfTime= stocks.ShelfTime,
                        TrayId =stocks.TrayId,
                        BillCode = stocks.BillCode,
                        InCode = stocks.InCode,
                        MaterialCode = stocks.MaterialCode,
                        Quantity = stocks.Quantity,
                        ManufactureDate = stocks.ManufactureDate,
                        BatchCode = stocks.BatchCode,
                        SupplierCode = stocks.SupplierCode,
                        SuggestLocationCode = stocks.SuggestLocationCode,
                        InDate = stocks.InDate,
                        IsLocked = stocks.IsLocked,
                        SaleBillNo = stocks.SaleBillNo,
                        SaleBillItemNo = stocks.SaleBillItemNo,
                        ManufactureBillNo = stocks.ManufactureBillNo,
                        CustomCode = stocks.CustomCode,
                        CustomName = stocks.CustomName,
                        WareHouseCode = stocks.WareHouseCode,
                        MaterialName = materials.Name,
                        MaterialType = materials.MaterialType,
                        CreatedTime = stocks.CreatedTime,
                        CreatedUserCode = stocks.CreatedUserCode,
                        CreatedUserName = stocks.CreatedUserName,
                        UpdatedTime = stocks.UpdatedTime,
                        UpdatedUserCode = stocks.UpdatedUserCode,
                        UpdatedUserName = stocks.UpdatedUserName,
                        WareHouseName = warehouses.Name,
                        AreaCode = stocks.AreaCode,
                        MaterialUnit = materials.Unit,
                        LockedQuantity = stocks.LockedQuantity,
                        IsCheckLocked =stocks.IsCheckLocked,
                        ValidityPeriod= materials.ValidityPeriod,
                        SupplierName=supply.Name,
                        LayoutId=location.LayoutId,
                        PictureUrl= materials.PictureUrl,
                        BoxCode= location.BoxCode,
                        BoxName = location.BoxName, 
                        TrayCode = location.TrayCode,
                        BoxUrl=location.BoxUrl,
                        AgeingPeriod= materials.AgeingPeriod,
                        XLight = location.XLight,
                        YLight = location.YLight,
                        Price = materials.Remark2,
                        Use = materials.Remark3,
                    });
            }
        }

        /// <summary>
        /// 定时任务--更改老化时间
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CheckOldStock()
        {
            var list = StockDtos.Where(a => a.MaterialStatus == (int)MaterialStatusCaption.Old).ToList();

            foreach (var item in list)
            {
                // 是否启用老化时间
                //此刻的时间应大于入库时间+老化时间
                var valtime = DateTime.Now.AddDays(-item.AgeingPeriod);
                if (valtime >= item.ShelfTime)
                {
                    item.MaterialStatus = (int)MaterialStatusCaption.Normal;
                    // 物料实体映射
                    Stock stockItem = Mapper.MapTo<Stock>(item);
                    StockRepository.Update(stockItem);
                }
            }
            return DataProcess.Success();
        }

        public DataResult CreateStockEntity(Stock entity)
        {
            //if (Stocks.Any(a => a.MaterialLabel == entity.MaterialLabel))
            //{
            //    return DataProcess.Failure(string.Format("物料条码{0}已存在", entity.MaterialLabel));
            //}

            var existingEntity = Stocks.FirstOrDefault(e => e.MaterialCode == entity.MaterialCode);
            if (existingEntity != null)
            {
                existingEntity.Quantity += entity.Quantity;
                existingEntity.ShelfTime = DateTime.Now;
                StockRepository.Update(existingEntity);
                return DataProcess.Success();
            }
            else if (StockRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("库存{0}新增成功", entity.MaterialLabel));
            }

            return DataProcess.Failure();
        }

        public DataResult RemoveStock(int id)
        {
            if (StockRepository.Delete(id) > 0)
            {
                return DataProcess.Success("删除成功");
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 根据库位码或者物料条码获取库存
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        public DataResult GetStockByMaterialLabel(string MaterialLabel, string LocationCode)
        {
            DataResult dataResult = DataProcess.Failure();

            try
            {
                if (!string.IsNullOrEmpty(MaterialLabel))
                {
                    var entity = this.StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == MaterialLabel);
                    if (entity != null)
                    {
                        return DataProcess.Success("成功", entity);

                    }
                    else
                    {
                        dataResult.Success = false;
                        dataResult.Message = "未在货架上找到该库存";
                    }
                }
                else if (!string.IsNullOrEmpty(LocationCode))
                {
                    var entity = this.StockRepository.Query().FirstOrDefault(a => a.LocationCode == LocationCode);
                    if (entity != null)
                    {
                        return DataProcess.Success("成功", entity);

                    }
                    else
                    {
                        dataResult.Success = false;
                        dataResult.Message = "未在货架上找到该库存";
                    }
                }

            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
            return dataResult;
        }


        /// <summary>
        /// 移库
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        public DataResult StockMoveLocationCode(string MaterialLabel, string LocationCode)
        {
            try
            {
                var entity = this.StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == MaterialLabel);
                if (entity != null)
                {
                    if (entity.IsLocked == true)
                    {
                        return DataProcess.Failure("该ReelId已被锁定(发料单或拆盘单占用)");
                    }
                    //1验证库位码合法性

                    var locationEntity = this.LocationRepository.Query().FirstOrDefault(a => a.Code == LocationCode);
                    if (locationEntity == null)
                    {
                        return DataProcess.Failure("库位码错误");
                    }
                    bool IsExistStock = this.StockRepository.GetCount(a => a.LocationCode == LocationCode) > 0;
                    if (IsExistStock)
                    {
                        return DataProcess.Failure("该库位上已有条码");
                    }

                    var mobile = MobileRepository;
                    MobileLocation mo = new MobileLocation();
                    mo.Code = SequenceContract.Create("MobileLocation");
                    mo.OldLocationCode = StockDtos.FirstOrDefault(a => a.MaterialLabel == MaterialLabel).LocationCode;
                    mo.NewLocationCode = LocationCode;
                    string materialCode = LabelRepository.Query().FirstOrDefault(a => a.Code == MaterialLabel).MaterialCode;
                    mo.MaterialName = MaterialRepository.Query().FirstOrDefault(a => a.Code == materialCode).Name;
                    mo.MaterialLable = MaterialLabel;
                    mo.MaterialCode = materialCode;
                    mo.Status = 0;
                    mo.WareHouseCode = StockDtos.FirstOrDefault(a => a.MaterialLabel == MaterialLabel).WareHouseCode;
                    mo.WareHouseName = StockDtos.FirstOrDefault(a => a.MaterialLabel == MaterialLabel).WareHouseName;
                    if (MobileRepository.Query().Any(a => a.Code == mo.Code))
                    {
                        return
                            DataProcess.Failure(string.Format("库位移动单{0}已存在，请勿重复创建！", mo.Code));
                    }
                    MobileRepository.Insert(mo);
                    entity.LocationCode = LocationCode;
                    this.StockRepository.Update(entity);
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("未在货架上找到该库存");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
    }
}
