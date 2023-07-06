using Bussiness.Entitys;
using Bussiness.Entitys.SMT;
using HP.Core.Data;
using HP.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Sequence;
using HP.Data.Orm;

namespace Bussiness.Services.SMT
{
    public class ShelfServer : Bussiness.Contracts.SMT.IShelfContract
    {
        public IRepository<WmsShelfMain, int> WmsShelfMainRepository { get; set; }

        public IRepository<WmsShelfDetail, int> WmsShelfDetailRepository { get; set; }

        public IRepository<WmsShelfSort, int> WmsShelfSortRepository { get; set; }
        public IRepository<Entitys.Location, int> LocaitonRepository { get; set; }

        public IRepository<Entitys.Stock, int> StockRepository { get; set; }

        public IRepository<Entitys.StockVM, int> StockVMRepository { get; set; }

        public Contracts.ILabelContract LabelContract { get; set; }

        public Bussiness.Contracts.IWareHouseContract WareHouseContract { get; set; }

        public Bussiness.Contracts.SMT.IPickContract PickContract { get; set; }

        public Bussiness.Contracts.SMT.IStockLightContract StockLightContract { get; set; }

        public IRepository<Bussiness.Entitys.PTL.DpsInterfaceMain, int> DpsInterfaceMainRepository { get; set; }

        public IRepository<Bussiness.Entitys.PTL.DpsInterface, int> DpsInterfaceRepository { get; set; }



        public IRepository<WmsShelfMainVM, int> WmsShelfMainVMRepository { get; set; }

        public IRepository<WmsShelfDetailVM, int> WmsShelfDetailVMRepository { get; set; }


        public HP.Core.Sequence.ISequenceContract SequenceContract { get; set; }
        public HPC.BaseService.Contracts.IDictionaryContract DictionaryContract { get; set; }

        public Bussiness.Contracts.IInContract InContract { get; set; }

        public IQuery<WmsShelfMain> WmsShelfMains
        {
            get
            {
                return WmsShelfMainRepository.Query();
            }
        }


        /// <summary>
        /// 强制完成上架亮灯任务
        /// </summary>
        /// <param name="ReplenishCode"></param>
        /// <returns></returns>
        public DataResult CompelFinishedReplenishOrder(string ReplenishCode)
        {
            var entity = this.WmsShelfMainRepository.Query().FirstOrDefault(a => a.ReplenishCode == ReplenishCode);
            if (entity.Status != 0)
            {
                return DataProcess.Failure("上架任务状态不对");
            }

            if (string.IsNullOrEmpty(entity.SplitNo))
            {
                List<Bussiness.Entitys.SMT.WmsShelfDetail> detailList = this.WmsShelfDetailRepository.Query()
                    .Where(a => a.ReplenishCode == ReplenishCode && a.Status == 0).ToList();
                List<Bussiness.Entitys.Stock> stockList = new List<Entitys.Stock>();
                this.WmsShelfMainRepository.UnitOfWork.TransactionEnabled = true;

                if (detailList != null && detailList.Count > 0)
                {
                    foreach (var shelfDetail in detailList)
                    {
                        Bussiness.Entitys.Stock stock = new Bussiness.Entitys.Stock();
                        var LocationEntity = LocaitonRepository.Query()
                            .FirstOrDefault(a => a.Code == shelfDetail.LocationCode);


                        var inMaterialEntity = this.InContract.InMaterials.FirstOrDefault(a => a.Status == 0 && a.MaterialLabel == shelfDetail.ReelId);
                        if (inMaterialEntity != null)
                        {
                            inMaterialEntity.LocationCode = LocationEntity.Code;
                            var result = InContract.HandShelf(inMaterialEntity);
                            if (!result.Success)
                            {
                                return DataProcess.Failure(result.Message);
                            }
                        }
                        else
                        {
                            //stock.AreaCode = LocationEntity.AreaCode;
                            stock.BatchCode = shelfDetail.BatchCode;
                            stock.BillCode = "";
                            stock.CustomCode = "";
                            stock.CustomName = "";
                            stock.InCode = shelfDetail.ReplenishCode;
                            stock.InDate = DateTime.Now.ToString("yyyy-MM-dd");
                            stock.WareHouseCode = LocationEntity.WareHouseCode;
                            stock.IsCheckLocked = false;
                            stock.IsLocked = false;
                            stock.LocationCode = shelfDetail.LocationCode;
                            stock.LockedQuantity = 0;
                            stock.ManufactureBillNo = "";
                            stock.ManufactureDate = shelfDetail.ReelCreateCode;
                            stock.MaterialCode = shelfDetail.MaterialCode;
                            stock.MaterialLabel = shelfDetail.ReelId;
                            stock.MaterialStatus = 0;
                            stock.Quantity = Convert.ToDecimal(shelfDetail.Quantity);
                            stock.SaleBillItemNo = "";
                            stock.SaleBillNo = "";
                            stock.StockStatus = 0;
                            stock.SupplierCode = shelfDetail.SupplierCode;
                            stock.WareHouseCode = LocationEntity.WareHouseCode;
                            shelfDetail.Status = (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Finished;
                            shelfDetail.FinishedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            var entityStock = StockRepository.Query()
                                .FirstOrDefault(a => a.MaterialLabel == shelfDetail.ReelId && a.LocationCode == shelfDetail.LocationCode);

                            if (entityStock == null)
                            {
                                stockList.Add(stock);
                            }
                            else
                            {
                                //shelfDetail.Status = (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Cancel;
                                entityStock.Quantity = entityStock.Quantity + stock.Quantity;
                                StockRepository.Update(entityStock);
                            }
                        }


                    }
                }

                entity.Status = 3;
                entity.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //using (this.UnitOfWork)
                {
                    this.WmsShelfMainRepository.Update(entity);
                    if (detailList != null && detailList.Count > 0)
                    {
                        foreach (var item in detailList)
                        {
                            this.WmsShelfDetailRepository.Update(item);
                        }
                    }

                    if (stockList.Count > 0)
                    {
                        foreach (var item in stockList)
                        {
                            StockRepository.Insert(item);
                        }
                    }

                    this.WmsShelfMainRepository.UnitOfWork.Commit();
                }
                //  WmsToPTLServer.ForWmsService wmsToPtlServer = new WmsToPTLServer.ForWmsService();
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                if (detailList != null && detailList.Count > 0)
                {
                    //wmsToPtlServer.Finish(
                    foreach (var shelfDetail in detailList)
                    {
                        //  wmsToPtlServer.Finish(entity.AreaId, shelfDetail.ProofId);
                        ptlServer.FinishOrder(entity.AreaId, shelfDetail.ProofId,false);
                    }
                }
            }
            else
            {
                // Bussiness.ShiYiTongServices.WmsSplitServer splitServer = new WmsSplitServer(this.UnitOfWork);
                //  Bussiness.Services.StockServer stockServer = new Services.StockServer(this.UnitOfWork);
                // Bussiness.ShiYiTongServices.WmsPickOrderServer pickServer = new WmsPickOrderServer(this.UnitOfWork);
                string SplitNo = entity.SplitNo;
                //var SplitMain = splitServer.SplitMainRepository.GetEntity<Bussiness.Entitys.SMT.WmsSplitMain>("SELECT * FROM TB_WMS_SPLIT_MAIN WHERE SPLITNO='" + SplitNo + "'");
                List<Bussiness.Entitys.SMT.WmsShelfDetail> detailList = this.WmsShelfDetailRepository.Query()
                    .Where(a => a.ReplenishCode == ReplenishCode && a.Status == 0).ToList();


                foreach (var shelfDetail in detailList)
                {
                    shelfDetail.Status = 4;
                    //#region 3库位码的合法性
                    //bool IsRightLocation = true;
                    //WmsService.BaseSettingService baseServer = new WmsService.BaseSettingService(this.UnitOfWork);
                    //var LocationEntity = baseServer.LocationRepository.GetEntity<WmsService.Entitys.BaseSetting.LocationEntity>("SELECT * FROM TB_WMS_LOCATION WHERE ID ='" + shelfDetail.LocationCode + "'");
                    //#endregion


                    //var splitStock = stockServer.StockRepository.GetEntity<Bussiness.Entitys.Stock>("SELECT * FROM TB_WMS_REELID_STOCK WHERE REELID='" + shelfDetail.ReelId + "'");
                    //splitStock.LocationCode = shelfDetail.LocationCode;
                    ////找到拆盘对应reelId
                    //var SplitReel = splitServer.SplitAreaReelDetailRepository.GetEntity<Bussiness.Entitys.SMT.WmsSplitAreaReelDetail>("SELECT * FROM TB_WMS_SPLIT_AREA_REEL_DETAIL WHERE SPLITNO='" + SplitNo + "' AND SPLITREELID ='" + shelfDetail.ReelId + "'");
                    //SplitReel.Status = 4;


                    //var TotalCount = splitServer.SplitAreaReelDetailRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL_DETAIL WHERE SPLITNO='" + SplitNo + "'");
                    //var ShelfieldReelCount = splitServer.SplitAreaReelDetailRepository.GetEntity<int>("SELECT  COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL_DETAIL WHERE SPLITNO='" + SplitNo + "' AND STATUS = 4");
                    //using (this.UnitOfWork)
                    //{
                    //    UnitOfWork.TransactionEnabled = true;
                    //    stockServer.StockRepository.Update(splitStock);
                    //    splitServer.SplitAreaReelDetailRepository.Update(SplitReel);
                    //    shelfDetail.Status = (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Finished;
                    //    shelfDetail.FinishedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //    this.ShelfDetailRepository.Update(shelfDetail);
                    //    //是否更新区域状态
                    //    if (SplitReel.PickDetailId != "" && SplitReel.PickOrderCode != "")
                    //    {
                    //        //更新单据对应的拣货任务详情

                    //        var pickAreaList = pickServer.WmsPickOrderAreaRepository.Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA WHERE PICKORDERCODE='" + SplitReel.PickOrderCode + "'");
                    //        if (pickAreaList.Find(a => a.AreaId == LocationEntity.AreaId) == null)
                    //        {
                    //            var newPickArea = new Bussiness.Entitys.SMT.WmsPickOrderArea();
                    //            newPickArea.AreaId = LocationEntity.AreaId;
                    //            newPickArea.PickOrderCode = SplitReel.PickOrderCode;
                    //            newPickArea.ProofId = Guid.NewGuid().ToString();
                    //            newPickArea.Status = 0;//待启动
                    //            newPickArea.WareHouseCode = LocationEntity.WarehouseId;
                    //            pickServer.WmsPickOrderAreaRepository.Insert(newPickArea);
                    //        }
                    //        else
                    //        {
                    //            foreach (var item in pickAreaList)
                    //            {
                    //                if (item.AreaId == LocationEntity.AreaId)
                    //                {
                    //                    item.Status = 0;
                    //                    pickServer.WmsPickOrderAreaRepository.Update(item);
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //        var pickAreaReel = pickServer.WmsPickOrderAreaDetailRepository.GetEntity<Bussiness.Entitys.SMT.WmsPickOrderAreaDetail>("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE REELID='" + SplitReel.SplitReelId + "'AND PICKORDERCODE='" + SplitReel.PickOrderCode + "' AND PICKORDERDETAILID='" + SplitReel.PickDetailId + "'");
                    //        pickAreaReel.LocationCode = shelfDetail.LocationCode;
                    //        pickAreaReel.Status = 0;
                    //        pickAreaReel.AreaId = LocationEntity.AreaId;
                    //        pickAreaReel.WareHouseCode = LocationEntity.WarehouseId;
                    //        pickAreaReel.IsNeedSplit = 0;
                    //        pickServer.WmsPickOrderAreaDetailRepository.Update(pickAreaReel);

                    //    }
                    //    if (TotalCount - ShelfieldReelCount == 1)
                    //    {
                    //        splitServer.SplitMainRepository.Execute("UPDATE TB_WMS_SPLIT_MAIN SET STATUS =4 WHERE SPLITNO='" + SplitNo + "'");
                    //        splitServer.SplitMainRepository.Execute("UPDATE TB_WMS_SPLIT_DETAIL SET STATUS =4 WHERE SPLITNO='" + SplitNo + "'");
                    //        splitServer.SplitMainRepository.Execute("UPDATE TB_WMS_SPLIT_AREA SET STATUS =4 WHERE SPLITNO='" + SplitNo + "'");
                    //        splitServer.SplitMainRepository.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL SET STATUS =4 WHERE SPLITNO='" + SplitNo + "'");

                    //    }
                    //    UnitOfWork.Commit();
                    //}
                    //  WmsToPTLServer.ForWmsService wmsToPtlServer = new WmsToPTLServer.ForWmsService();
                    //wmsToPtlServer.Finish(
                    //   wmsToPtlServer.Finish(entity.AreaId, shelfDetail.ProofId);
                }

                // using (this.UnitOfWork)
                {
                    entity.Status = 3;
                    WmsShelfMainRepository.UnitOfWork.TransactionEnabled = true;
                    this.WmsShelfMainRepository.Update(entity);
                    if (detailList != null && detailList.Count > 0)
                    {
                        foreach (var item in detailList)
                        {
                            this.WmsShelfDetailRepository.Update(item);
                        }
                    }

                    WmsShelfMainRepository.UnitOfWork.Commit();
                }
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                if (detailList != null && detailList.Count > 0)
                {
                    //wmsToPtlServer.Finish(
                    foreach (var shelfDetail in detailList)
                    {
                        //  wmsToPtlServer.Finish(entity.AreaId, shelfDetail.ProofId);
                        ptlServer.FinishOrder(entity.AreaId, shelfDetail.ProofId,false);
                    }
                }
            }

            return DataProcess.Success();
        }

        /// <summary>
        /// WEB确认上架
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public DataResult WebConfirmShelf(Bussiness.Entitys.Label label)
        {
            #region 1验证条码信息
            //从条码表中获取信息
            var ReelIdInfo = this.LabelContract.LabelDtos.FirstOrDefault(a => a.Code == label.Code);
            if (ReelIdInfo == null)
            {
                return DataProcess.Failure("条码信息表中不存在此条码信息");
            }
            #endregion
            #region 3库位码的合法性

            bool IsRightLocation = true;
            var LocationEntity = LocaitonRepository.Query().FirstOrDefault(a => a.Code == label.LocationCode);
            if (LocationEntity == null)
            {
                IsRightLocation = false;
            }

            if (!IsRightLocation)
            {
                return DataProcess.Failure("库位码不存在");
            }

            #endregion

            #region 1判断次ReelId是否合法(已在货架上不允许重复上架)

            var isReelIdExistStock = StockVMRepository.Query().FirstOrDefault(a => a.MaterialLabel == label.Code);
            if (isReelIdExistStock != null)
            {
                if (ReelIdInfo.IsElectronicMateria == true)//电子料处理方式
                {
                    return DataProcess.Failure("该条码在库存中已存在");
                }
                else //非电子料处理方式
                {
                    //库存条码必须唯一
                    if (isReelIdExistStock.LocationCode != LocationEntity.Code)
                    {
                        return DataProcess.Failure(string.Format("库存中库位{0}存在该条码,若要上架请选择该库位", isReelIdExistStock.LocationCode));
                    }
                }
            }

            #endregion

            #region 4判断此库位是否合法 即是否为空库位

            var IsCurrentLocationStock =
                StockVMRepository.Query().FirstOrDefault(a => a.LocationCode == label.LocationCode);
            if (IsCurrentLocationStock != null)
            {
                if (ReelIdInfo.IsElectronicMateria)
                {
                    return DataProcess.Failure("此库位已被其他条码占用,用选择其他库位");
                }
                else
                {
                    //if (IsCurrentLocationStock.MaterialLabel!= label.Code)
                    //{
                    //    return DataProcess.Failure(string.Format("该库位已存在其他条码的库存");
                    //}
                }
            }

            #endregion


            Entitys.SMT.WmsShelfMain main = new Entitys.SMT.WmsShelfMain();
            //var rule = new RT.BaseService.Sequence.Sequence("LabelRule");
            // main.CreateUserName = RT.BaseService.Identity.Account.UserName; ;
            //Random dom = new Random();
            main.ReplenishCode = main.ReplenishCode = SequenceContract.Create("ShelfOrder");
            //main.ReplenishCode = Guid.NewGuid().ToString();
            //main.ShelfCode = LocationEntity.ShelfCode;
            main.Status = 3;
           // main.AreaId = LocationEntity.AreaCode;
            main.AreaId = LocationEntity.ContainerCode;
            main.WareHouseCode = LocationEntity.WareHouseCode;
            main.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //从mes视图中获取到ReelId信息
            Entitys.SMT.WmsShelfDetail detail = new Entitys.SMT.WmsShelfDetail();
            detail.ReelId = label.Code;
            detail.BatchCode = label.BatchCode;
            detail.LocationCode = label.LocationCode;
            detail.ReelCreateCode = label.ManufactrueDate.GetValueOrDefault(DateTime.Now);
            detail.MaterialCode = label.MaterialCode;
            detail.OrgQuantity = (int)label.Quantity;
            detail.Quantity = (int)label.Quantity;
            detail.ReplenishCode = main.ReplenishCode;
            detail.SupplierCode = label.SupplierCode;
            detail.ShelfSortNo = 1;
            detail.Status = 3;
            detail.Status = (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Finished;
            detail.FinishedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //增加库存或累加库存
            WmsShelfMainRepository.UnitOfWork.TransactionEnabled = true;
            this.WmsShelfMainRepository.Insert(main);
            this.WmsShelfDetailRepository.Insert(detail);

            var inMaterialEntity = this.InContract.InMaterials.FirstOrDefault(a => a.Status == 0 && a.MaterialLabel == label.Code);
            if (inMaterialEntity != null)
            {
                inMaterialEntity.LocationCode = label.LocationCode;
                var result = InContract.HandShelf(inMaterialEntity);
                if (!result.Success)
                {
                    return DataProcess.Failure(result.Message);
                }
            }
            else
            {

                if (isReelIdExistStock != null)
                {
                    decimal stockQuantity = isReelIdExistStock.Quantity + label.Quantity;
                    StockRepository.Update(a => new Bussiness.Entitys.Stock() { Quantity = stockQuantity }, p => p.Id == isReelIdExistStock.Id);
                }
                else
                {
                    Bussiness.Entitys.Stock stock = new Bussiness.Entitys.Stock();
                    stock.AreaCode = LocationEntity.ContainerCode;
                    stock.BatchCode = label.BatchCode;
                    stock.BillCode = "";
                    stock.CustomCode = "";
                    stock.CustomName = "";
                    stock.InCode = main.ReplenishCode;
                    stock.InDate = DateTime.Now.ToString("yyyy-MM-dd");
                    stock.WareHouseCode = LocationEntity.WareHouseCode;
                    stock.IsCheckLocked = false;
                    stock.IsLocked = false;
                    stock.LocationCode = label.LocationCode;
                    stock.LockedQuantity = 0;
                    stock.ManufactureBillNo = "";
                    stock.ManufactureDate = detail.ReelCreateCode;
                    stock.MaterialCode = label.MaterialCode;
                    stock.MaterialLabel = label.Code;
                    stock.MaterialStatus = 0;
                    stock.Quantity = label.Quantity;
                    stock.SaleBillItemNo = "";
                    stock.SaleBillNo = "";
                    stock.StockStatus = 0;
                    stock.SupplierCode = label.SupplierCode;
                    stock.WareHouseCode = LocationEntity.WareHouseCode;

                    if (LabelContract.Labels.FirstOrDefault(A => A.Code == label.Code) == null)
                    {
                        LabelContract.CreateLabel(label);
                    }

                    StockRepository.Insert(stock);
                }
            }


            WmsShelfMainRepository.UnitOfWork.Commit();

            return DataProcess.Success();
        }

        #region  手持端操作逻辑

        /// <summary>
        /// 首次上架
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        public DataResult FirstShelf(Bussiness.Entitys.Label label)
        {
            try
            {
                string DetailId = "";

                #region 1验证条码信息
                //从条码表中获取信息
                var ReelIdInfo = this.LabelContract.LabelDtos.FirstOrDefault(a => a.Code == label.Code);
                if (ReelIdInfo == null)
                {
                    return DataProcess.Failure("条码信息表中不存在此条码信息");
                }
                #endregion

                #region 获取到上架规则
                //上架分配原则
                var shelfRule = DictionaryContract.Dictionaries.Where(a => a.TypeCode == "ShelfRule" && a.Enabled == true).OrderBy(a => a.Sort).FirstOrDefault();
                if (shelfRule == null)
                {
                    return DataProcess.Failure("尚未分配上架规则");
                }
                 if (shelfRule.Value == "Manual")//人工绑定分配
                {
                    // 若果扫描进来的条码,物料未绑定库位 则系统推荐一个库位 若系统推荐不了库位 则手工再扫描库位
                    var firstLocation = WareHouseContract.Locations.FirstOrDefault(a => a.SuggestMaterialCode == ReelIdInfo.MaterialCode && a.Enabled == true);
                    if (firstLocation != null)
                    {
                        label.LocationCode = firstLocation.Code;
                    }
                    else
                    {
                        return DataProcess.Failure("尚未维护库位与物料绑定关系");
                    }
                }
                else if (shelfRule.Value == "System")//系统分配
                {

                }
                else //手动选择库位 
                {

                }
                #endregion

                #region 3库位码的合法性

                bool IsRightLocation = true;
                var LocationEntity = this.WareHouseContract.LocationRepository.Query()
                    .FirstOrDefault(a =>
                        a.Code == label
                            .LocationCode);
                if (LocationEntity == null)
                {
                    IsRightLocation = false;
                }

                if (!IsRightLocation)
                {
                    return DataProcess.Failure("不存在此库位码,请检查扫描是否正确。");
                }

                if (LocationEntity.Enabled == false)
                {
                    return DataProcess.Failure("库位码尚未启用");
                }

                #endregion

                if (!string.IsNullOrEmpty(label.SplitNo))
                {
                    #region 1判断次ReelId是否合法(已在货架上不允许重复上架)

                    bool isReelIdExist = this.StockRepository.Query().FirstOrDefault(a =>
                                             a.MaterialLabel == label.Code && a.LocationCode ==
                                             LocationEntity.WareHouseCode + "00000000") == null
                        ? false
                        : true; /*.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_REELID_STOCK WHERE REELID='" + ReelId + "' AND LOCATIONCODE='" + LocationEntity.WarehouseId + "00000000" + "'") > 0;*/
                    if (!isReelIdExist)
                    {
                        return DataProcess.Failure("该拆盘条码不在缓存区");
                    }

                    bool IsExistSplitTask = ReelIsInSplitPlan(label.Code, label.SplitNo);
                    ;
                    if (!IsExistSplitTask)
                    {
                        return DataProcess.Failure("此条码不在此拆盘单上或状态不对");
                    }

                    #endregion
                }
                else
                {
                    #region 1判断次ReelId是否合法(已在货架上不允许重复上架)
                    var isReelIdExistStock = StockVMRepository.Query().FirstOrDefault(a => a.MaterialLabel == label.Code);
                    if (isReelIdExistStock != null)
                    {
                        if (ReelIdInfo.IsElectronicMateria == true)//电子料处理方式
                        {
                            return DataProcess.Failure("该条码在库存中已存在");
                        }
                        else //非电子料处理方式
                        {
                            //库存条码必须唯一
                            if (isReelIdExistStock.LocationCode != LocationEntity.Code)
                            {
                                return DataProcess.Failure(string.Format("库存中库位{0}存在该条码,若要上架请选择该库位", isReelIdExistStock.LocationCode));
                            }
                        }
                    }

                    #endregion
                }

                #region  2 判断该区域是否正在进行拣货任务或者拆盘任务
                bool IsExistReplenishTask = true;
                IsExistReplenishTask = StockLightContract.IsCurrentAreaShelfTasking(LocationEntity.WareHouseCode, LocationEntity.ContainerCode);
                if (IsExistReplenishTask)
                {
                    return DataProcess.Failure("区域" + LocationEntity.ContainerCode + "尚有上架任务正在进行");
                }
                bool IsAreaSplitTasking =
                    StockLightContract.IsCurrentAreaSplitTasking(LocationEntity.WareHouseCode, LocationEntity.ContainerCode);
                if (IsAreaSplitTasking)
                {
                    return DataProcess.Failure("该区域存在拆盘任务,无法在此货架上补货,请等待此区域拣货任务结束后在上架");
                }

                // IsCurrentAreaSplitTasking = splitServer.SplitAreaRepository.GetEntity<int>("SELECT COUNT(*) FROM TB")
                bool IsAreaPickTasking =
                    StockLightContract.IsCurrentAreaPickTasking(LocationEntity.WareHouseCode, LocationEntity.ContainerCode);
                if (IsAreaPickTasking)
                {
                    return DataProcess.Failure("该区域存在拣货任务,无法在此货架上补货,请等待此区域拣货任务结束后在上架");
                }
                bool IsExistCheckTask = StockLightContract.IsCurrentAreaCheckTasking(LocationEntity.WareHouseCode, LocationEntity.ContainerCode);
                if (IsExistCheckTask)
                {
                    return DataProcess.Failure("区域:" + LocationEntity.ContainerCode + "尚有盘点任务正在进行");
                }
                #endregion

                #region 4判断此库位是否合法 即是否为空库位

                var IsCurrentLocationStock = StockVMRepository.Query().FirstOrDefault(a => a.LocationCode == label.LocationCode);
                if (IsCurrentLocationStock != null)
                {
                    if (ReelIdInfo.IsElectronicMateria)
                    {
                        return DataProcess.Failure("此库位已被其他条码占用,用选择其他库位");
                    }
                    else
                    {
                        //if (IsCurrentLocationStock.MaterialLabel!= label.Code)
                        //{
                        //    return DataProcess.Failure(string.Format("该库位已存在其他条码的库存");
                        //}
                    }
                }

                #endregion

                #region 5 创建上架任务主表以及  第一条任务明细 并计算好下面的库位顺序

                Bussiness.Entitys.SMT.WmsShelfMain main = new Bussiness.Entitys.SMT.WmsShelfMain();
                //var rule = new RT.BaseService.Sequence.Sequence("LabelRule");
                Random dom = new Random();
                main.ReplenishCode = SequenceContract.Create("ShelfOrder"); //"I" + DateTime.Now.ToString("yyyyMMddHHmmss") + dom.Next(1, 10000).ToString();
                //main.ShelfCode = LocationEntity.ShelfCode;
                main.Status = 0;
                main.AreaId = LocationEntity.ContainerCode;
                main.WareHouseCode = LocationEntity.WareHouseCode;
                main.SplitNo = label.SplitNo;
                //从mes视图中获取到ReelId信息
                //UnitOfWork mesUnitOfWork = new DataAccess.UnitOfWork();
                //MesBusiness.MesServices.ReelIdServer reelIdServer = new MesBusiness.MesServices.ReelIdServer(mesUnitOfWork);
                ////SELECT * FROM MES.IVMAT_REEL@MES WHERE REEL_ID='LCR13050592001085'
                ////var ReelIdInfo = reelIdServer.ReelIdRepository.GetEntity<MesBusiness.MesEntitys.ReelIdInfo>("SELECT * FROM IVMAT_Reel WHERE REEL_ID='" + ReelId + "'");
                //var ReelIdInfo = new MesBusiness.MesEntitys.ReelIdInfo();


                //var Connection = new System.Data.OracleClient.OracleConnection(); ;//Oracle.DataAccess.Client.OracleConnection();
                //Connection.ConnectionString = ConnectionString;
                ////Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand();
                //var cmd = new System.Data.OracleClient.OracleCommand();
                //cmd.Connection = Connection;
                //cmd.CommandText = "SELECT * FROM MES.IVMAT_REEL@MES WHERE REEL_ID='" + ReelId + "'";
                //// cmd.CommandText = "SELECT * FROM IVMAT_REEL WHERE REEL_ID='" + ReelId + "'";
                ////  Oracle.DataAccess.Client.OracleDataAdapter dr1 = new Oracle.DataAccess.Client.OracleDataAdapter();
                //System.Data.OracleClient.OracleDataAdapter dr1 = new System.Data.OracleClient.OracleDataAdapter();
                //dr1.SelectCommand = cmd;
                //DataTable top1Dt = new DataTable();
                //dr1.Fill(top1Dt);

                //if (top1Dt.Rows.Count == 0)
                //{
                //    ReelIdInfo = null;
                //}
                //else
                //{
                //    ReelIdInfo = Common.ConvertDataTableToList<MesBusiness.MesEntitys.ReelIdInfo>.GetEntityByDataRow(top1Dt.Rows[0]);
                //}




                Bussiness.Entitys.SMT.WmsShelfDetail detail = new Bussiness.Entitys.SMT.WmsShelfDetail();
                detail.ReelId = ReelIdInfo.Code;
                detail.BatchCode = ReelIdInfo.BatchCode;
                detail.LocationCode = label.LocationCode;
                detail.ReelCreateCode = ReelIdInfo.ManufactrueDate.GetValueOrDefault(DateTime.Now);
                detail.MaterialCode = ReelIdInfo.MaterialCode;
                detail.OrgQuantity =
                    Convert.ToInt32(ReelIdInfo.Quantity); //ReelIdInfo.Org_Quantity.GetValueOrDefault(0)
                detail.Quantity = Convert.ToInt32(ReelIdInfo.Quantity); // ReelIdInfo.Cur_Quantity.GetValueOrDefault(0)
                detail.ReplenishCode = main.ReplenishCode;
                detail.SupplierCode = ReelIdInfo.SupplierCode;
                detail.ShelfSortNo = 1;
                detail.UniqueValue = Guid.NewGuid().ToString();
                detail.Status = 0;
                detail.ProofId = Guid.NewGuid().ToString();
                // detail.UserId = ReelIdInfo.User_Id;
                int count = this.StockRepository.GetCount(a =>
                    a.MaterialCode ==
                    detail.MaterialCode); //.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_REELID_STOCK WHERE MATERIALCODE='" + detail.MaterialCode + "'");
                detail.CurrentMaterialReelIdCount = count;
                //if (detail.Quantity == 0)
                //{
                //                        result.Success = false;
                //                        result.Message = "ReelId实际数量为0";
                //    return RT.Utility.JsonHelper.SerializeObject(result);
                //}
                //创建库位序号
                //1查出次货架所有空库存的库位码



                List<Bussiness.Entitys.SMT.WmsShelfSort> shelfSortList = new List<Bussiness.Entitys.SMT.WmsShelfSort>();
                if (shelfRule.Value == "System")
                {
                    #region 系统分配库位
    //                List<Bussiness.Entitys.Location> locationList = this.WareHouseContract.LocationRepository.SqlQuery(
    //"SELECT * FROM TB_WMS_LOCATION WHERE ShelfCode='" + LocationEntity.ShelfCode +
    //"'AND Enabled =1  AND CODE NOT IN (SELECT LOCATIONCODE FROM VIEW_WMS_STOCK WHERE ShelfCode='" +
    //LocationEntity.ShelfCode + "')").ToList();
                    List<Bussiness.Entitys.Location> sortList = new List<Bussiness.Entitys.Location>();

                    Bussiness.Entitys.SMT.WmsShelfSort sortEntity = new Bussiness.Entitys.SMT.WmsShelfSort();
                    sortEntity.ShelfSortNo = 1;
                    sortEntity.ReplenishCode = main.ReplenishCode;
                    sortEntity.Status = 1;
                    sortEntity.LocationCode = detail.LocationCode;
                    shelfSortList.Insert(0, sortEntity);
                    //2按照从左到右 从上到下的算法 排序空库位码
                    //1先按照层数分组

                    //string locationCode = LocationEntity.Code.Substring(LocationEntity.Row.Length - 4, 4);
                    //LocationEntity.Row = locationCode.Substring(0, 1);
                    //if (locationCode.Substring(1, 3) == "100")
                    //{
                    //    LocationEntity.Column = "100";
                    //}
                    //else
                    //{
                    //    LocationEntity.Column = locationCode.Substring(2, 2);
                    //}
                    //var rowGroup = locationList.GroupBy(a => a.Row);
                    //int CurrentRow = int.Parse(LocationEntity.Row);
                    //int CurrentColumn = int.Parse(LocationEntity.Column);
                    //bool IsCurrentCase = false; //如果正序
                    //bool IsCase = true;
                    //var lastEntity = locationList.OrderBy(a => int.Parse(a.Row)).ThenBy(a => int.Parse(a.Column)).Last();
                    //// 第一组 正序
                    //var FirstGroup = locationList
                    //    .Where(a => a.Row == LocationEntity.Row && int.Parse(a.Column) > CurrentColumn).ToList()
                    //    .OrderBy(a => int.Parse(a.Column));
                    //if (FirstGroup != null && FirstGroup.Count() > 0)
                    //{
                    //    sortList.AddRange(FirstGroup);
                    //}

                    //for (int i = CurrentRow + 1; i < int.Parse(lastEntity.Row) + 1; i++)
                    //{
                    //    if (IsCurrentCase)
                    //    {
                    //        var list = locationList.Where(a => int.Parse(a.Row) == i).OrderBy(a => int.Parse(a.Column))
                    //            .ToList();
                    //        if (list.Count != null && list.Count > 0)
                    //        {
                    //            sortList.AddRange(list);
                    //            IsCurrentCase = false;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var list = locationList.Where(a => int.Parse(a.Row) == i)
                    //            .OrderByDescending(a => int.Parse(a.Column)).ToList();
                    //        if (list != null && list.Count > 0)
                    //        {
                    //            sortList.AddRange(list);
                    //            IsCurrentCase = true;
                    //        }
                    //    }
                    //}

                    //for (int i = 1; i < CurrentRow; i++)
                    //{
                    //    if (IsCase)
                    //    {
                    //        var list = locationList.Where(a => int.Parse(a.Row) == i).OrderBy(a => int.Parse(a.Column))
                    //            .ToList();
                    //        if (list != null && list.Count > 0)
                    //        {
                    //            sortList.AddRange(list);
                    //            IsCase = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var list = locationList.Where(a => int.Parse(a.Row) == i)
                    //            .OrderByDescending(a => int.Parse(a.Column)).ToList();
                    //        if (list != null && list.Count > 0)
                    //        {
                    //            sortList.AddRange(list);
                    //            IsCase = false;
                    //        }
                    //    }
                    //}

                    ////最后一组
                    //var LastGroup = locationList
                    //    .Where(a => a.Row == LocationEntity.Row && int.Parse(a.Column) < CurrentColumn).ToList();
                    //if (LastGroup != null && LastGroup.Count() > 0)
                    //{
                    //    sortList.AddRange(LastGroup);
                    //}

                    ////3插入库位排序表
                    //bool IsCreateShelfMainSuccess = false;
                    //for (int i = 0; i < sortList.Count; i++)
                    //{
                    //    Bussiness.Entitys.SMT.WmsShelfSort sort = new Bussiness.Entitys.SMT.WmsShelfSort();
                    //    sort.LocationCode = sortList[i].Code;
                    //    sort.ReplenishCode = main.ReplenishCode;
                    //    sort.ShelfSortNo = i + 2;
                    //    //if (i == 0)
                    //    //{
                    //    //    sort.Status = 1;
                    //    //}
                    //    //else
                    //    //{
                    //    //    sort.Status = 0;
                    //    //}
                    //    sort.Status = 0;
                    //    shelfSortList.Add(sort);
                    //}
                    #endregion
                }


                #endregion

                //7 向PTL发送亮灯命令请求 参数 任务编号 任务区域 任务仓库编码
                bool IsPTLExecuteSuccess = false;
                Bussiness.Entitys.PTL.DpsInterfaceMain ptlMain = new Bussiness.Entitys.PTL.DpsInterfaceMain();
                Bussiness.Entitys.PTL.DpsInterface ptlInterface = new Bussiness.Entitys.PTL.DpsInterface();
                CreateDpsInterface(ref ptlMain, ref ptlInterface, detail, main);
                //  using (newWork)
                {
                    this.WmsShelfMainRepository.UnitOfWork.TransactionEnabled = true;
                    WmsShelfMainRepository.Insert(main);
                    this.WmsShelfDetailRepository.Insert(detail);
                    foreach (var item in shelfSortList)
                    {
                        this.WmsShelfSortRepository.Insert(item);
                    }

                    ptlInterface.RelationId = detail.Id;
                    ptlInterface.MaterialLabel = detail.ReelId;
                    DpsInterfaceMainRepository.Insert(ptlMain);
                    DpsInterfaceRepository.Insert(ptlInterface);
                    WmsShelfMainRepository.UnitOfWork.Commit();
                    // IsCreateShelfMainSuccess = true;
                }

                var entity = this.WmsShelfDetailRepository.Query().FirstOrDefault(a =>
                    a.ReplenishCode == main.ReplenishCode &&
                    a.UniqueValue ==
                    detail.UniqueValue); //.GetEntity<Bussiness.Entitys.SMT.WmsShelfDetail>("SELECT * FROM TB_WMS_SHELF_DETAIL WHERE REPLENISHCODE='" + main.ReplenishCode + "' AND UNIQUEVALUE='" + detail.UniqueValue + "'");
                detail.Id = entity.Id;
                //亮灯
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                ptlServer.StartOrder(LocationEntity.ContainerCode, detail.ProofId,false);

                ReelIdInfo.LocationCode = label.LocationCode;
                ReelIdInfo.SplitNo = label.SplitNo;
                ReelIdInfo.ShelfDetailId = detail.Id;
                ReelIdInfo.ReplenishCode = main.ReplenishCode;
                return DataProcess.Success("上架任务启动成功", ReelIdInfo);
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// pda确认上架
        /// </summary>
        /// <param name="ReplenishCode">补货单号</param>
        /// <param name="DetailId">明细ID</param>
        /// <returns></returns>
        public DataResult PdaConfirmShelf(Bussiness.Entitys.Label label)
        {
            try
            {
                //1根据是否亮灯成功,是否向PTL发送熄灭命令 参数(DetailId)
                bool IsPTLExecuteSuccess = true;
                bool IsWmsExecuteSuccess = false;

                //2 PTL执行成功 wms将执行上架操作 增加库存并更新 明細狀態
                if (IsPTLExecuteSuccess)
                {
                    var shelfDetail = this.WmsShelfDetailRepository.GetEntity(label.ShelfDetailId);
                    if (shelfDetail.Status >= 3)
                    {
                        Bussiness.Entitys.Label label2 = new Label();
                        label2.ShelfDetailId = label.ShelfDetailId;
                        label2.SplitNo = label.SplitNo;
                        label2.ReplenishCode = label.ReplenishCode;
                        label2.Code = label.Code;
                        return DataProcess.Success("上架成功", label2);
                    }

                    shelfDetail.Quantity = (int)label.Quantity;
                    var shelfMain = this.WmsShelfMainRepository.Query()
                        .FirstOrDefault(a => a.ReplenishCode == label.ReplenishCode);

                    #region 3库位码的合法性

                    bool IsRightLocation = true;
                    var LocationEntity = this.WareHouseContract.LocationRepository.Query()
                        .FirstOrDefault(a => a.Code == label.LocationCode);
                    if (LocationEntity == null)
                    {
                        IsRightLocation = false;
                    }

                    if (!IsRightLocation)
                    {
                        return DataProcess.Failure("库位码错误");
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(label.SplitNo))
                    {
                        //找到拆盘对应reelId
                        var SplitReel = this.PickContract.WmsSplitAreaReelDetailRepository.Query()
                            .FirstOrDefault(a =>
                                a.SplitNo == label.SplitNo &&
                                a.SplitReelId ==
                                label.Code); //.GetEntity<Business.ShiYiTongEntitys.WmsSplitAreaReelDetail>("SELECT * FROM TB_WMS_SPLIT_AREA_REEL_DETAIL WHERE SPLITNO='" + SplitNo + "' AND SPLITREELID ='" + shelfDetail.ReelId + "'");
                        SplitReel.Status = 4;
                        var SplitMain = this.PickContract.WmsSplitMainRepository.Query()
                            .FirstOrDefault(a =>
                                a.SplitNo ==
                                label.SplitNo); /*splitServer.SplitMainRepository.GetEntity<Business.ShiYiTongEntitys.WmsSplitMain>("SELECT * FROM TB_WMS_SPLIT_MAIN WHERE SPLITNO='" + SplitNo + "'");*/
                        var splitStock = this.StockRepository.Query()
                            .FirstOrDefault(a =>
                                a.MaterialLabel ==
                                label.Code); //stockServer.StockRepository.GetEntity<Business.Entitys.Stock>("SELECT * FROM TB_WMS_REELID_STOCK WHERE REELID='" + shelfDetail.ReelId + "'");
                        if (splitStock != null)
                        {
                            splitStock.LocationCode = shelfDetail.LocationCode;
                            if (string.IsNullOrEmpty(SplitReel.PickOrderCode) ||
                                string.IsNullOrEmpty(SplitReel.PickDetailId))
                            {
                                splitStock.IsLocked = false;
                            }
                        }


                        var TotalCount =
                            this.PickContract.WmsSplitAreaReelDetailRepository.GetCount(a =>
                                a.SplitNo == label.SplitNo);
                        var ShelfieldReelCount =
                            this.PickContract.WmsSplitAreaReelDetailRepository.GetCount(a =>
                                a.SplitNo == label.SplitNo &&
                                a.Status >=
                                4); //.GetEntity<int>("SELECT  COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL_DETAIL WHERE SPLITNO='" + SplitNo + "' AND STATUS >= 4");

                        // using (work)
                        {
                            this.WmsShelfDetailRepository.UnitOfWork.TransactionEnabled = true;
                            this.StockRepository.Update(splitStock);
                            this.PickContract.WmsSplitAreaReelDetailRepository.Update(SplitReel);
                            shelfDetail.Status = (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Finished;
                            shelfDetail.FinishedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            this.WmsShelfDetailRepository.Update(shelfDetail);
                            //是否更新区域状态
                            if (!string.IsNullOrEmpty(SplitReel.PickDetailId) &&
                                !string.IsNullOrEmpty(SplitReel.PickOrderCode)
                            ) //SplitReel.PickDetailId != "" && SplitReel.PickOrderCode != ""
                            {
                                //更新单据对应的拣货任务详情

                                var pickAreaList = this.PickContract.WmsPickOrderAreaRepository.Query()
                                    .Where(a => a.PickOrderCode == SplitReel.PickOrderCode)
                                    .ToList(); //Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA WHERE PICKORDERCODE='" + SplitReel.PickOrderCode + "'");
                                if (pickAreaList.Find(a => a.AreaId == LocationEntity.ContainerCode) == null)
                                {
                                    var newPickArea = new Bussiness.Entitys.SMT.WmsPickOrderArea();
                                    newPickArea.AreaId = LocationEntity.ContainerCode;
                                    newPickArea.PickOrderCode = SplitReel.PickOrderCode;
                                    newPickArea.ProofId = Guid.NewGuid().ToString();
                                    newPickArea.Status = 0; //待启动
                                    newPickArea.WareHouseCode = LocationEntity.WareHouseCode;
                                    this.PickContract.WmsPickOrderAreaRepository.Insert(newPickArea);
                                }
                                else
                                {
                                    foreach (var item in pickAreaList)
                                    {
                                        if (item.AreaId == LocationEntity.ContainerCode)
                                        {
                                            item.Status = 0;
                                            this.PickContract.WmsPickOrderAreaRepository.Update(item);
                                            break;
                                        }
                                    }
                                }

                                var pickAreaReel = this.PickContract.WmsPickOrderAreaDetailRepository.Query()
                                    .FirstOrDefault(a =>
                                        a.ReelId == SplitReel.SplitReelId &&
                                        a.PickOrderCode == SplitReel.PickOrderCode &&
                                        a.PickOrderDetailId ==
                                        SplitReel
                                            .PickDetailId); //.GetEntity<Business.ShiYiTongEntitys.WmsPickOrderAreaDetail>("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE REELID='" + SplitReel.SplitReelId + "'AND PICKORDERCODE='" + SplitReel.PickOrderCode + "' AND PICKORDERDETAILID='" + SplitReel.PickDetailId + "'");
                                pickAreaReel.LocationCode = shelfDetail.LocationCode;
                                pickAreaReel.Status = 0;
                                pickAreaReel.AreaId = LocationEntity.ContainerCode;
                                pickAreaReel.WareHouseCode = LocationEntity.WareHouseCode;
                                pickAreaReel.IsNeedSplit = false;
                                this.PickContract.WmsPickOrderAreaDetailRepository.Update(pickAreaReel);
                            }

                            if (TotalCount - ShelfieldReelCount == 1)
                            {
                                this.PickContract.WmsSplitMainRepository.Update(
                                    a => new Bussiness.Entitys.SMT.WmsSplitMain() { Status = 4 },
                                    p => p.SplitNo ==
                                         label
                                             .SplitNo); //Execute("UPDATE TB_WMS_SPLIT_MAIN SET STATUS =4 WHERE SPLITNO='" + SplitNo + "'");
                                this.PickContract.WmsSplitAreaRepository.Update(
                                    a => new Bussiness.Entitys.SMT.WmsSplitArea() { Status = 4 },
                                    p => p.SplitNo == label.SplitNo);
                                this.PickContract.WmsSplitAreaReelRepository.Update(
                                    a => new Bussiness.Entitys.SMT.WmsSplitAreaReel() { Status = 4 },
                                    p => p.SplitNo == label.SplitNo);
                            }

                            this.WmsShelfDetailRepository.UnitOfWork.Commit();
                        }

                        //WmsToPTLServer.ForWmsService wmsToPtlServer = new WmsToPTLServer.ForWmsService();
                        ////wmsToPtlServer.Finish(
                        //wmsToPtlServer.Finish(shelfMain.AreaId, shelfDetail.ProofId);
                        PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                        ptlServer.FinishOrder(shelfMain.AreaId, shelfDetail.ProofId,false);
                    }
                    else
                    {
                        Bussiness.Entitys.Stock stock = new Bussiness.Entitys.Stock();
                        stock.BatchCode = shelfDetail.BatchCode;
                        stock.AreaCode = LocationEntity.ContainerCode;
                        stock.BillCode = "";
                        stock.CustomCode = "";
                        stock.CustomName = "";
                        stock.InCode = shelfMain.ReplenishCode;
                        stock.InDate = DateTime.Now.ToString("yyyy-MM-dd");
                        stock.IsCheckLocked = false;
                        stock.IsLocked = false;
                        stock.LocationCode = LocationEntity.Code;
                        stock.LockedQuantity = 0;
                        stock.ManufactureBillNo = "";
                        stock.ManufactureDate = shelfDetail.ReelCreateCode;
                        stock.MaterialCode = shelfDetail.MaterialCode;
                        stock.MaterialLabel = shelfDetail.ReelId;
                        stock.MaterialStatus = 0;
                        stock.Quantity = Convert.ToInt32(shelfDetail.Quantity);
                        stock.SaleBillItemNo = "";
                        stock.SaleBillNo = "";
                        stock.StockStatus = 0;
                        stock.SupplierCode = shelfDetail.SupplierCode;
                        stock.WareHouseCode = LocationEntity.WareHouseCode;
                        shelfDetail.Status = (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Finished;
                        shelfDetail.FinishedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                        
                        this.WmsShelfDetailRepository.UnitOfWork.TransactionEnabled = true;

                        var inMaterialEntity = this.InContract.InMaterials.FirstOrDefault(a => a.Status == 0 && a.MaterialLabel == label.Code);
                        if (inMaterialEntity != null)
                        {
                            inMaterialEntity.LocationCode = label.LocationCode;
                            var result = InContract.HandShelf(inMaterialEntity);
                            if (!result.Success)
                            {
                                return DataProcess.Failure(result.Message);
                            }
                        }
                        else
                        {
                            var entity = this.StockRepository.Query().FirstOrDefault(a =>a.MaterialLabel == stock.MaterialLabel && a.LocationCode == LocationEntity.Code);
                            if (entity == null)
                            {
                                this.StockRepository.Insert(stock);
                            }
                            else
                            {
                                entity.Quantity = entity.Quantity + stock.Quantity;
                                StockRepository.Update(entity);
                                // shelfDetail.Status = (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Cancel;
                            }
                        }


                        this.WmsShelfDetailRepository.Update(shelfDetail);
                        this.WmsShelfDetailRepository.UnitOfWork.Commit();
                        IsWmsExecuteSuccess = true;


                        PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                        ptlServer.FinishOrder(shelfMain.AreaId, shelfDetail.ProofId,false);
                    }
                }

                //  Business.Entitys.PdaResult result = new Business.Entitys.PdaResult();
                Bussiness.Entitys.Label label1 = new Label();
                label1.ShelfDetailId = label.ShelfDetailId;
                label1.SplitNo = label.SplitNo;
                label1.ReplenishCode = label.ReplenishCode;
                label1.Code = label.Code;
                return DataProcess.Success("上架成功", label1);
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 第二次扫描或者......
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="RelenishCode">补货单号</param>
        /// <returns></returns>
        public DataResult NextShelf(Bussiness.Entitys.Label label)
        {
            try
            {
                int DetailId = 0;
                int sortNo = 0;
                //3创建上架任务明细 并返回ID
                //从mes视图中获取到ReelId信息
                var ReelIdInfo = this.LabelContract.LabelDtos.FirstOrDefault(a => a.Code == label.Code);
                if (ReelIdInfo == null)
                {
                    return DataProcess.Failure("未能从条码信息表中获取到该条码信息");
                }
                var shelfMain = this.WmsShelfMainRepository.Query()
                    .FirstOrDefault(a =>
                        a.ReplenishCode ==
                        label.ReplenishCode); //.GetEntity<Business.ShiYiTongEntitys.WmsShelfMain>("SELECT * FROM TB_WMS_SHELF_MAIN WHERE REPLENISHCODE='" + ReplenishCode + "'");
                var lastDetail = this.WmsShelfDetailRepository.GetEntity(label.ShelfDetailId);
                #region 获取到上架规则
                //上架分配原则
                var shelfRule = DictionaryContract.Dictionaries.Where(a => a.TypeCode == "ShelfRule" && a.Enabled == true).OrderBy(a => a.Sort).FirstOrDefault();
                if (shelfRule == null)
                {
                    return DataProcess.Failure("尚未分配上架规则");
                }
                var sortEntity = new Bussiness.Entitys.SMT.WmsShelfSort();

                if (shelfRule.Value == "Manual")//人工绑定分配
                {
                    // 若果扫描进来的条码,物料未绑定库位 则系统推荐一个库位 若系统推荐不了库位 则手工再扫描库位
                    //if (label.LocationCode==lastDetail.LocationCode)
                    //{
                    //    //获取一个推荐库位 获取不到 则等待扫描的库位 假如有这个条码的库存 则默认推荐到该库位上,
                    //    // 假如 都为空  则随意取一个空库位

                    //    var stockEntity = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == label.Code);
                    //    if (stockEntity != null && stockEntity.LocationCode != stockEntity.WareHouseCode + "00000000")
                    //    {
                    //        label.LocationCode = stockEntity.LocationCode;//相同条码推荐到一个库位上
                    //    }
                    //    else //没有找到 则在库位表查找第一个推荐该物料的库位
                    //    {
                    //        var firstLocation = WareHouseContract.Locations.FirstOrDefault(a => a.SuggestMaterialCode == ReelIdInfo.MaterialCode && a.Enabled == true);
                    //        if (firstLocation != null)
                    //        {
                    //            label.LocationCode = firstLocation.Code;
                    //        }
                    //        else
                    //        {
                    //            return DataProcess.Failure("未找到分配库位,请先扫描库位码");
                    //        }
                    //    }
                    //}
                    var firstLocation = WareHouseContract.Locations.FirstOrDefault(a => a.SuggestMaterialCode == ReelIdInfo.MaterialCode && a.Enabled == true);
                    if (firstLocation != null)
                    {
                        label.LocationCode = firstLocation.Code;
                    }
                    else
                    {
                        return DataProcess.Failure("尚未维护库位与物料绑定关系");
                    }
                }
                else if (shelfRule.Value == "System")//系统分配
                {
                    #region 2 从上架库位序号表中取出相对应的库位码,如若没有空库位了 提示此货架已放满


                    //if (lastDetail.Status == (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Initial)
                    //{
                    //    Business.Entitys.PdaResult lastResult = RT.Utility.JsonHelper.DeSerializeObject<Business.Entitys.PdaResult>(PdaConfirmShelf(ReplenishCode, LastDetailId, SplitNo));
                    //     if (lastResult.Success ==false)
                    //    {
                    //        result = lastResult;
                    //        result.Message = "上一次扫描上架执行失败:" + lastResult.Message;
                    //        return RT.Utility.JsonHelper.SerializeObject(result);
                    //    }
                    //}
                    if (lastDetail.Status != (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Finished &&
                        lastDetail.Status != (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Cancel)
                    {
                        return DataProcess.Failure("当前条码还未确认上架");
                    }

                    sortEntity = this.WmsShelfSortRepository.Query()
                       .Where(a => a.ReplenishCode == label.ReplenishCode && a.Status == 0).OrderBy(a => a.ShelfSortNo)
                       .Take(1)
                       .FirstOrDefault(); //.GetEntity<Business.ShiYiTongEntitys.WmsShelfSort>("SELECT * FROM ( SELECT * FROM TB_WMS_SHELF_SORT WHERE REPLENISHCODE='" + ReplenishCode + "' AND STATUS=0 ORDER BY SHELFSORTNO ) WHERE ROWNUM =1");
                    if (sortEntity == null || sortEntity.Id == 0)
                    {
                        return DataProcess.Failure("该货架已放满或需要重新计算库位,重新开始上架之前,请先结束本次上架任务!");
                    }
                    label.LocationCode = sortEntity.LocationCode;
                    sortNo = sortEntity.ShelfSortNo.GetValueOrDefault(0);
                    #endregion
                }
                else //人工 扫描库位码
                {
                    if (string.IsNullOrEmpty(label.LocationCode))
                    {
                        //ReelIdInfo.LocationCode = label.LocationCode;
                        //ReelIdInfo.SplitNo = label.SplitNo;
                        //ReelIdInfo.ShelfDetailId = 0;
                        //ReelIdInfo.ReplenishCode = shelfMain.ReplenishCode;
                        return DataProcess.Failure("请先扫描库位码");
                    }
                }
                #endregion




                #region 3库位码的合法性

                bool IsRightLocation = true;
                var LocationEntity = this.WareHouseContract.LocationRepository.Query()
                    .FirstOrDefault(a =>
                        a.Code == label
                            .LocationCode);
                if (LocationEntity == null)
                {
                    IsRightLocation = false;
                }

                if (!IsRightLocation)
                {
                    return DataProcess.Failure("不存在此库位码,请检查扫描是否正确。");
                }

                if (LocationEntity.Enabled == false)
                {
                    return DataProcess.Failure("库位码尚未启用");
                }

                #endregion

                #region 4判断此库位是否合法 即是否为空库位

                var IsCurrentLocationStock = StockVMRepository.Query().FirstOrDefault(a => a.LocationCode == label.LocationCode);
                if (IsCurrentLocationStock != null)
                {
                    if (ReelIdInfo.IsElectronicMateria)
                    {
                        return DataProcess.Failure("此库位已被其他条码占用,用选择其他库位");
                    }
                    else
                    {
                        //if (IsCurrentLocationStock.MaterialLabel!= label.Code)
                        //{
                        //    return DataProcess.Failure(string.Format("该库位已存在其他条码的库存");
                        //}
                    }
                }

                #endregion

                #region 1判断次ReelId是否合法(已在货架上不允许重复上架)

                if (!string.IsNullOrEmpty(label.SplitNo))
                {
                    #region 1判断次ReelId是否合法(已在货架上不允许重复上架)

                    string virLocation = LocationEntity.WareHouseCode + "00000000";
                    bool isReelIdExist = this.StockRepository.GetCount(a =>
                                             a.MaterialLabel == label.Code && a.LocationCode == virLocation) >
                                         0; //.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_REELID_STOCK WHERE REELID='" + ReelId + "' AND LOCATIONCODE='" + LocationEntity.WarehouseId + "00000000" + "'") > 0;
                    if (!isReelIdExist)
                    {
                        return DataProcess.Failure("该条码不在缓存区");
                    }

                    bool IsExistSplitTask = ReelIsInSplitPlan(label.Code, label.SplitNo);
                    if (!IsExistSplitTask)
                    {
                        return DataProcess.Failure("该条码不在此拆盘计划中");
                    }

                    #endregion
                }
                else
                {
                    #region 1判断次ReelId是否合法(已在货架上不允许重复上架)


                    var isReelIdExistStock = StockVMRepository.Query().FirstOrDefault(a => a.MaterialLabel == label.Code);
                    if (isReelIdExistStock != null)
                    {
                        if (ReelIdInfo.IsElectronicMateria == true)//电子料处理方式
                        {
                            return DataProcess.Failure("该条码在库存中已存在");
                        }
                        else //非电子料处理方式
                        {
                            //库存条码必须唯一
                            if (isReelIdExistStock.LocationCode != LocationEntity.Code)
                            {
                                return DataProcess.Failure(string.Format("库存中库位{0}存在该条码,若要上架请选择该库位", isReelIdExistStock.LocationCode));
                            }
                        }
                    }

                    #endregion
                }



                #endregion



                Bussiness.Entitys.SMT.WmsShelfDetail detail = new Bussiness.Entitys.SMT.WmsShelfDetail();
                detail.ReelId = ReelIdInfo.Code;
                detail.BatchCode = ReelIdInfo.BatchCode;
                detail.LocationCode = label.LocationCode;
                detail.ReelCreateCode = ReelIdInfo.ManufactrueDate.GetValueOrDefault(DateTime.Now);
                detail.MaterialCode = ReelIdInfo.MaterialCode;
                detail.OrgQuantity = Convert.ToInt32(ReelIdInfo.Quantity);
                detail.Quantity = Convert.ToInt32(ReelIdInfo.Quantity);
                detail.ReplenishCode = lastDetail.ReplenishCode;
                detail.SupplierCode = ReelIdInfo.SupplierCode;
                detail.ShelfSortNo = sortNo;
                detail.Status = 0;
                detail.UniqueValue = Guid.NewGuid().ToString();
                detail.ProofId = Guid.NewGuid().ToString();
                //int count = this.StockRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_REELID_STOCK WHERE MATERIALCODE='" + detail.MaterialCode + "'");
                //detail.CurrentMaterialReelIdCount = count;
                //if (detail.Quantity == 0)
                //{
                //    result.Success = false;
                //    result.Message = "ReelId实际数量为0";
                //    return RT.Utility.JsonHelper.SerializeObject(result);
                //}
                bool IsWmsCreateDetailSuccess = false;

                //更新Sort表状态
                sortEntity.Status = 1;


                Bussiness.Entitys.PTL.DpsInterfaceMain ptlMain = new Bussiness.Entitys.PTL.DpsInterfaceMain();
                Bussiness.Entitys.PTL.DpsInterface ptlInterface = new Bussiness.Entitys.PTL.DpsInterface();
                CreateDpsInterface(ref ptlMain, ref ptlInterface, detail, shelfMain);
                // Business.PTLServices.PTLServer ptlServer = new Business.PTLServices.PTLServer(newWork);
                //   using (newWork)
                {
                    WmsShelfDetailRepository.UnitOfWork.TransactionEnabled = true;
                    this.WmsShelfDetailRepository.Insert(detail);
                    ptlInterface.RelationId = detail.Id;
                    ptlInterface.MaterialLabel = detail.ReelId;
                    if (sortEntity != null && sortEntity.Id != 0)
                    {
                        this.WmsShelfSortRepository.Update(sortEntity);
                    }
                    this.DpsInterfaceRepository.Insert(ptlInterface);
                    this.DpsInterfaceMainRepository.Insert(ptlMain);
                    WmsShelfDetailRepository.UnitOfWork.Commit();
                    IsWmsCreateDetailSuccess = true;
                }
                // 4 向PTL发送亮灯请求 

                var entity = this.WmsShelfDetailRepository.Query().FirstOrDefault(a =>
                    a.ReplenishCode == shelfMain.ReplenishCode &&
                    a.UniqueValue ==
                    detail.UniqueValue); //.GetEntity<Bussiness.Entitys.SMT.WmsShelfDetail>("SELECT * FROM TB_WMS_SHELF_DETAIL WHERE REPLENISHCODE='" + main.ReplenishCode + "' AND UNIQUEVALUE='" + detail.UniqueValue + "'");
                detail.Id = entity.Id;
                //亮灯
                //WmsToPTLServer.ForWmsService wmsToPtlServer = new WmsToPTLServer.ForWmsService();
                //wmsToPtlServer.Start(LocationEntity.WareHouseCode+LocationEntity.AreaCode, detail.ProofId);

                ReelIdInfo.LocationCode = label.LocationCode;
                ReelIdInfo.SplitNo = label.SplitNo;
                ReelIdInfo.ShelfDetailId = detail.Id;
                ReelIdInfo.ReplenishCode = shelfMain.ReplenishCode;

                //WmsToPTLServer.ForWmsService wmsToPtlServer = new WmsToPTLServer.ForWmsService();
                //wmsToPtlServer.Start(LocationEntity.AreaId, detail.ProofId);
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                ptlServer.StartOrder(LocationEntity.ContainerCode, detail.ProofId,false);
                return DataProcess.Success("扫描成功", ReelIdInfo);
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 完成本次上架
        /// </summary>
        /// <param name="ReplenishCode"></param>
        /// <returns></returns>
        public DataResult FinishCurrentReplenish(Entitys.Label label)
        {
            //1根据补货单号  完成此次上架 更新上架主表完成时间以及状态
            try
            {
                var main = this.WmsShelfMainRepository.Query()
                    .FirstOrDefault(a => a.ReplenishCode == label.ReplenishCode);
                main.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                main.Status = (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Finished;
                var detailList = this.WmsShelfDetailRepository.Query()
                    .Where(a => a.ReplenishCode == label.ReplenishCode && a.Status == 0)
                    .ToList(); //.Query<Business.ShiYiTongEntitys.WmsShelfDetail>("SELECT * FROM TB_WMS_SHELF_DETAIL WHERE REPLENISHCODE='" + ReplenishCode + "' AND STATUS=0");
                foreach (var item in detailList)
                {
                    item.Status = 3;
                    item.FinishedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    label.ShelfDetailId = item.Id;
                    var result = PdaConfirmShelf(label);
                    if (!result.Success)
                    {
                        return result;
                    }
                }

                //  using (work)
                {
                    this.WmsShelfMainRepository.UnitOfWork.TransactionEnabled = true;
                    WmsShelfMainRepository.Update(main);
                    foreach (var item in detailList)
                    {
                        this.WmsShelfDetailRepository.Update(item);
                    }

                    WmsShelfMainRepository.UnitOfWork.Commit();
                }
                Bussiness.Entitys.Label label1 = new Label();
                label1.ShelfDetailId = 0;
                label1.SplitNo = label.SplitNo;
                label1.ReplenishCode = "";
                return DataProcess.Success("上架成功", label1);
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 跳过当前亮灯位置,亮灯下一个位置
        /// </summary>
        /// <returns></returns>
        public DataResult SkipCurrentLightLocation(Bussiness.Entitys.Label label)
        {
            try
            {
                int DetailId = 0;

                //上架分配原则
                var shelfRule = DictionaryContract.Dictionaries.Where(a => a.TypeCode == "ShelfRule" && a.Enabled == true).OrderBy(a => a.Sort).FirstOrDefault();
                if (shelfRule == null)
                {
                    return DataProcess.Failure("尚未分配上架规则");
                }


                if (shelfRule.Value == "System")//人工绑定分配
                {

                    var shelfMain = this.WmsShelfMainRepository.Query()
                        .FirstOrDefault(a => a.ReplenishCode == label.ReplenishCode);

                    #region 2 从上架库位序号表中取出相对应的库位码,如若没有空库位了 提示此货架已放满

                    var lastDetail = this.WmsShelfDetailRepository.GetEntity(label.ShelfDetailId);
                    if (lastDetail.Status != (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Initial)
                    {
                        return DataProcess.Failure("当前条码已上架");
                    }

                    var sortEntity = this.WmsShelfSortRepository.Query()
                        .Where(a => a.ReplenishCode == label.ReplenishCode && a.Status == 0).OrderBy(a => a.ShelfSortNo)
                        .Take(1)
                        .FirstOrDefault(); /*this.WmsShelfSortRepository.GetEntity<Business.ShiYiTongEntitys.WmsShelfSort>("SELECT * FROM ( SELECT * FROM TB_WMS_SHELF_SORT WHERE REPLENISHCODE='" + ReplenishCode + "' AND STATUS=0 ORDER BY SHELFSORTNO ) WHERE ROWNUM =1");*/
                    if (sortEntity == null)
                    {
                        return DataProcess.Failure("该货架已放满或需要重新计算库位,重新开始上架之前,请先结束本次上架任务!");
                    }

                    string LastProofId = lastDetail.ProofId;
                    lastDetail.LocationCode = sortEntity.LocationCode;
                    lastDetail.ShelfSortNo = sortEntity.ShelfSortNo;
                    lastDetail.ProofId = Guid.NewGuid().ToString();
                    sortEntity.Status = 1;
                    Bussiness.Entitys.PTL.DpsInterfaceMain ptlMain = new Bussiness.Entitys.PTL.DpsInterfaceMain();
                    Bussiness.Entitys.PTL.DpsInterface ptlInterface = new Bussiness.Entitys.PTL.DpsInterface();
                    CreateDpsInterface(ref ptlMain, ref ptlInterface, lastDetail, shelfMain);

                    //using (newWork)
                    {
                        this.WmsShelfDetailRepository.UnitOfWork.TransactionEnabled = true;
                        WmsShelfDetailRepository.Update(lastDetail);
                        this.WmsShelfSortRepository.Update(sortEntity);
                        this.DpsInterfaceRepository.Insert(ptlInterface);
                        this.DpsInterfaceMainRepository.Insert(ptlMain);
                        this.WmsShelfDetailRepository.UnitOfWork.Commit();
                    }

                    //WmsToPTLServer.ForWmsService wmsToPtlServer = new WmsToPTLServer.ForWmsService();
                    ////wmsToPtlServer.Finish(
                    //wmsToPtlServer.Finish(shelfMain.AreaId, LastProofId);
                    ////System.Threading.Thread.Sleep(1000);
                    //wmsToPtlServer.Start(shelfMain.AreaId, lastDetail.ProofId);

                    PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                    ptlServer.FinishOrder(shelfMain.AreaId, LastProofId,false);
                    ptlServer.StartOrder(shelfMain.AreaId, lastDetail.ProofId,false);
                    label.LocationCode = lastDetail.LocationCode;
                    return DataProcess.Success("操作成功", label);
                    //发送上一个的灭灯指令(根据上一次亮灯是否成功)  和新的亮灯指令(需和滨哥讨论)

                    #endregion
                }
                else
                {
                    return DataProcess.Failure("系统分配原则才允许跳过当前库位");
                }

            }
            catch (Exception ex)
            {
                return DataProcess.Failure();
            }
        }

        public bool ReelIsInSplitPlan(string ReelId, string SplitNo)
        {
            try
            {
                return this.PickContract.WmsSplitAreaReelDetailRepository.GetCount(a =>
                           a.SplitReelId == ReelId && a.SplitNo == SplitNo) >
                       0; //.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL_DETAIL WHERE SplitReelId ='" + ReelId + "' AND STATUS =3") > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool CreateDpsInterface(ref Bussiness.Entitys.PTL.DpsInterfaceMain main,
            ref Bussiness.Entitys.PTL.DpsInterface detail, Bussiness.Entitys.SMT.WmsShelfDetail shelfDetail,
            Entitys.SMT.WmsShelfMain shelfMain)
        {
            detail.BatchNO = shelfDetail.BatchCode;
            detail.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            detail.GoodsName = shelfDetail.MaterialCode;
            detail.LocationId = shelfDetail.LocationCode;
            detail.MakerName = "";
            detail.OperateId = Guid.NewGuid().ToString();

            detail.ProofId = shelfDetail.ProofId;
            detail.Quantity = shelfDetail.Quantity;
            detail.Spec = "";
            detail.Status = 0;
            detail.ToteId = "";
            detail.UserId = shelfMain.CreatedUserName;
            detail.RelationId = shelfDetail.Id;
            detail.MaterialLabel = shelfDetail.ReelId;
            main.CreateDate = detail.CreateDate;
            main.OrderType = 2;
            main.ProofId = shelfDetail.ProofId;
            main.OrderCode = shelfDetail.ReplenishCode;
            main.Status = 0;
            main.WareHouseCode = shelfMain.WareHouseCode;
            main.AreaCode = shelfMain.AreaId;
            return true;
        }

        #endregion
    }
}