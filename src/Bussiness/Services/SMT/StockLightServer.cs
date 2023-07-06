using Bussiness.Contracts.SMT;
using Bussiness.Entitys.SMT;
using HP.Core.Data;
using HP.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Services.SMT
{
    public class StockLightServer : IStockLightContract
    {
        public IRepository<WmsReelLightMain, int> WmsReelLightMainRepository { get; set; }

        public IRepository<WmsReelLightArea, int> WmsReelLightAreaRepository { get; set; }

        public IRepository<WmsReelLightAreaDetail, int> WmsReelLightAreaDetailRepository { get; set; }

        public IRepository<Bussiness.Entitys.Stock, int> StockRepository { get; set; }

        public IRepository<Bussiness.Entitys.SMT.WmsShelfMain, int> WmsShelfMainRepository { get; set; }

        public IRepository<Bussiness.Entitys.SMT.WmsPickOrderArea, int> WmsPickOrderAreaRepository { get; set; }

        public IRepository<Bussiness.Entitys.SMT.WmsSplitArea, int> WmsSplitAreaRepository { get; set; }

        public IRepository<Bussiness.Entitys.PTL.DpsInterfaceMain, int> DpsInterfaceMainRepository { get; set; }

        public IRepository<Bussiness.Entitys.PTL.DpsInterface, int> DpsInterfaceRepository { get; set; }

        public HP.Core.Sequence.ISequenceContract SequenceContract { set; get; }

        public Bussiness.Contracts.ICheckContract CheckContract { get; set; }
        public DataResult LightReelArray(List<Bussiness.Entitys.Stock> stockList)
        {
            // MobileService.MoblieServer mobileServer = new MobileService.MoblieServer();
            //  Business.Services.StockServer stockServer = new Services.StockServer(this.UnitOfWork);
            // PTLServices.PTLServer ptlServer = new PTLServices.PTLServer();
            //var StockList = stockServer.StockVMRepository.Query("SELECT * FROM VIEW_WMS_REELID_STOCK WHERE ID IN @IdList", new { IdList = StockIdList });
            //if (StockList.Any(a=>a.IsLocked==true))
            //{
            //    return DataProcess.Failure("存在正在被锁定的库存");
            //}

            List<int> stockIdList = new List<int>();
            foreach (var item in stockList)
            {
                stockIdList.Add(item.Id);
            }
            var StockList = StockRepository.Query().Where(a => stockIdList.Contains(a.Id)).ToList();

            Entitys.SMT.WmsReelLightMain main = new Entitys.SMT.WmsReelLightMain();
            main.LightCode = SequenceContract.Create("StockLight");
            main.Status = 0;
            var areaGroup = StockList.GroupBy(a => a.AreaCode);
            List<Entitys.SMT.WmsReelLightArea> areaList = new List<Entitys.SMT.WmsReelLightArea>();
            List<Entitys.SMT.WmsReelLightAreaDetail> areaDetailList = new List<Entitys.SMT.WmsReelLightAreaDetail>();

            bool isExitsTask = this.WmsReelLightMainRepository.Query().FirstOrDefault(a => a.Status == 0) == null ? false : true;
            if (isExitsTask)
            {
                return DataProcess.Failure("上次点亮的还未熄灭,请先熄灭");
            }

            foreach (var item in areaGroup)
            {
                var isExist = IsCurrentAreaShelfTasking(item.FirstOrDefault().WareHouseCode, item.FirstOrDefault().AreaCode);
                if (isExist)
                {
                    return DataProcess.Failure("区域"+item.FirstOrDefault().AreaCode+"存在正在启动的上架任务");
                }
                isExist =IsCurrentAreaPickTasking(item.FirstOrDefault().WareHouseCode, item.FirstOrDefault().AreaCode);
                if (isExist)
                {
                    return DataProcess.Failure("区域" + item.FirstOrDefault().AreaCode + "存在正在启动的拣货任务");
                    //continue;
                }
                isExist = IsCurrentAreaSplitTasking(item.FirstOrDefault().WareHouseCode, item.FirstOrDefault().AreaCode);
                if (isExist)
                {
                    return DataProcess.Failure("区域" + item.FirstOrDefault().AreaCode + "存在正在启动的拆盘任务");
                    // continue;
                }

                Entitys.SMT.WmsReelLightArea area = new Entitys.SMT.WmsReelLightArea();
                area.AreaId = item.FirstOrDefault().AreaCode;
                area.LightCode = main.LightCode;
                area.ProofId = Guid.NewGuid().ToString();
                area.Status = 0;
                area.WareHouseCode = item.FirstOrDefault().WareHouseCode;
                areaList.Add(area);
                foreach (var areaDetail in item)
                {
                    Entitys.SMT.WmsReelLightAreaDetail reelAreaDetail = new Entitys.SMT.WmsReelLightAreaDetail();
                    reelAreaDetail.AreaId = areaDetail.AreaCode;
                    reelAreaDetail.LightCode = main.LightCode;
                    reelAreaDetail.LocationCode = areaDetail.LocationCode;
                    reelAreaDetail.ReelId = areaDetail.MaterialLabel;
                    reelAreaDetail.Status = 0;
                    reelAreaDetail.MaterialCode = areaDetail.MaterialCode;
                    reelAreaDetail.OrgQuantity = Convert.ToInt32( areaDetail.Quantity);
                    reelAreaDetail.BatchCode = areaDetail.BatchCode;
                    areaDetailList.Add(reelAreaDetail);
                }
            }
            if (areaList.Count > 0 && areaDetailList.Count > 0)
            {
                var dpsMainList = new List<Entitys.PTL.DpsInterfaceMain>();
                var dpsDetailList = new List<Entitys.PTL.DpsInterface>();
    
               // using (this.UnitOfWork)
                {
                    WmsReelLightMainRepository. UnitOfWork.TransactionEnabled = true;
                    this.WmsReelLightMainRepository.Insert(main);
                    foreach (var item in areaList)
                    {
                        this.WmsReelLightAreaRepository.Insert(item);
                    }
                    foreach (var item in areaDetailList)
                    {
                        this.WmsReelLightAreaDetailRepository.Insert(item);
                    }

                    CreateDpsinterfaceMainList(ref dpsMainList, ref dpsDetailList, areaList, areaDetailList);
                    foreach (var item in dpsDetailList)
                    {
                        this.DpsInterfaceRepository.Insert(item);
                    }
                    foreach (var item in dpsMainList)
                    {
                      this.DpsInterfaceMainRepository.Insert(item);
                    }
                    WmsReelLightMainRepository.UnitOfWork.Commit();

                }

                //发起亮灯请求
                System.Threading.Thread.Sleep(1000);
                // WmsToPTLServer.ForWmsService lightServer = new WmsToPTLServer.ForWmsService();
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
              //  ptlServerClient.Endpoint.EndpointBehaviors.Add(new Bussiness.Common.MyEndPointBehavior());
                foreach (var item in areaList)
                {
                    var result = ptlServer.StartOrder(item.AreaId, item.ProofId,false);
                    //if (!result.Success)
                    //{
                    //    return DataProcess.Failure(result.Message);
                    //}
                }

            
            }
            else
            {
                return DataProcess.Failure("指定的ReelId集合所在区域存在上架或拆盘或拣货任务");

            }
            return DataProcess.Success("亮灯成功");
        }

        public DataResult OffLight()
        {
           // MobileService.MoblieServer mobileServer = new MobileService.MoblieServer();
         //   Business.Services.StockServer stockServer = new Services.StockServer(this.UnitOfWork);
        //    PTLServices.PTLServer ptlServer = new PTLServices.PTLServer();


            Entitys.SMT.WmsReelLightMain main = new Entitys.SMT.WmsReelLightMain();
            main.LightCode = Guid.NewGuid().ToString();
            main.Status = 0;
            List<Entitys.SMT.WmsReelLightArea> areaList = new List<Entitys.SMT.WmsReelLightArea>();
            List<Entitys.SMT.WmsReelLightAreaDetail> areaDetailList = new List<Entitys.SMT.WmsReelLightAreaDetail>();

            var entity = this.WmsReelLightMainRepository.Query().FirstOrDefault(a => a.Status == 0);
            if (entity != null)
            {
                var detailList = this.WmsReelLightAreaRepository.Query().Where(a => a.LightCode == entity.LightCode && a.Status==0).ToList();
                //WmsToPTLServer.ForWmsService lightServer = new WmsToPTLServer.ForWmsService();
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                foreach (var item in detailList)
                {
                    item.Status = 1;
                    this.WmsReelLightAreaRepository.Update(item);
                    this.WmsReelLightAreaDetailRepository.Update(p => new Bussiness.Entitys.SMT.WmsReelLightAreaDetail() { Status = 1 }, a => a.LightCode == entity.LightCode && a.AreaId == item.AreaId);
                    // lightServer.Finish(item.WareHouseCode+item.AreaId, item.ProofId);
                    ptlServer.FinishOrder(item.AreaId, item.ProofId,false);
                }
                entity.Status = 1;
               this.WmsReelLightMainRepository.Update(entity);
                return DataProcess.Success("灭灯成功");
            }
            else
            {
                return DataProcess.Failure("尚未点亮任意");
            }
        }

        public DataResult OffAreaLight(string LightCode,string AreaCode)
        {

            var entity = this.WmsReelLightMainRepository.Query().FirstOrDefault(a => a.Status == 0 && a.LightCode==LightCode);
            if (entity != null)
            {
                this.WmsReelLightMainRepository.UnitOfWork.TransactionEnabled = true;
                var detailList = this.WmsReelLightAreaRepository.Query().Where(a => a.LightCode == entity.LightCode && a.AreaId==AreaCode && a.Status==0).ToList();
                //WmsToPTLServer.ForWmsService lightServer = new WmsToPTLServer.ForWmsService();
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                foreach (var item in detailList)
                {
                    item.Status = 1;
                    this.WmsReelLightAreaRepository.Update(item);
                    this.WmsReelLightAreaDetailRepository.Update(p => new Bussiness.Entitys.SMT.WmsReelLightAreaDetail() { Status = 1 },a => a.LightCode == entity.LightCode && a.AreaId==item.AreaId );

                    // lightServer.Finish(item.WareHouseCode+item.AreaId, item.ProofId);
                    ptlServer.FinishOrder(item.AreaId, item.ProofId,false);
                }
                int areaCount = WmsReelLightAreaRepository.GetCount(a => a.LightCode == entity.LightCode);
                int finishedCount = WmsReelLightAreaRepository.GetCount(a => a.LightCode == entity.LightCode && a.Status == 1);
                if (areaCount-finishedCount==0)
                {
                    entity.Status = 1;
                    this.WmsReelLightMainRepository.Update(entity);
                }
                this.WmsReelLightMainRepository.UnitOfWork.Commit();
                return DataProcess.Success("灭灯成功");
            }
            else
            {
                return DataProcess.Failure("尚未点亮任意");
            }
        }

        /// <summary>
        /// 判断当前区域是否存在上架任务
        /// </summary>
        /// <param name="WareHouseCode"></param>
        /// <param name="AreaId"></param>
        /// <param name="ShelfCode"></param>
        /// <returns></returns>
        public bool IsCurrentAreaShelfTasking(string WareHouseCode, string AreaId)
        {
            // Business.ShiYiTongServices.WmsShelfServer shelfServer = new Business.ShiYiTongServices.WmsShelfServer();
            //return shelfServer.ShelfMainRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SHELF_MAIN WHERE STATUS = 0 AND WAREHOUSECODE='" + WareHouseCode + "' AND AREAID='" + AreaId + "'") > 0;
            return WmsShelfMainRepository.Query().FirstOrDefault(a => a.AreaId == AreaId && a.WareHouseCode == WareHouseCode && a.Status == 0) == null ? false : true;
        }


        /// <summary>
        /// 判断当前区域是否有正在进行的拣货任务
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentAreaPickTasking(string WareHouseCode, string AreaId)
        {

            //var entity = pickOrderServer.WmsPickOrderAreaRepository.GetEntity<Business.ShiYiTongEntitys.WmsPickOrderArea>("SELECT * FROM TB_WMS_PICK_ORDER_AREA WHERE STATUS = 1 AND WAREHOUSECODE='" + WareHouseCode + "' AND AREAID='" + AreaId + "'");
            //if (entity != null)
            //{
            //    entity.Status = 2;
            //    pickOrderServer.WmsPickOrderAreaRepository.Update(entity);
            //    // PickTaskDoFinish(entity.PickOrderCode, entity.AreaId);
            //}
            //return pickOrderServer.WmsPickOrderAreaRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_PICK_ORDER_AREA WHERE STATUS = 1 AND WAREHOUSECODE='" + WareHouseCode + "' AND AREAID='" + AreaId + "'") > 0;

            return WmsPickOrderAreaRepository.Query().FirstOrDefault(a => a.WareHouseCode == WareHouseCode && a.AreaId == AreaId && a.Status == 1) == null ? false : true;
        }

        /// <summary>
        /// 判断当前区域是否有正在进行的拆分任务
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentAreaSplitTasking(string WareHouseCode, string AreaId)
        {

            //return splitServer.SplitAreaRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA WHERE STATUS = 1 AND WAREHOUSECODE='" + WareHouseCode + "' AND AREAID='" + AreaId + "'") > 0;
            return WmsSplitAreaRepository.Query().FirstOrDefault(a => a.AreaId == AreaId && a.WareHouseCode == WareHouseCode && a.Status == 1) == null ? false : true;
        }

        public bool IsCurrentAreaCheckTasking(string WareHouseCode, string AreaId)
        {
            return this.CheckContract.CheckAreaRepository.Query().FirstOrDefault(a=>a.AreaCode==AreaId && a.WareHouseCode==WareHouseCode && a.Status<6)==null ? false : true; 
        }

        public DataResult CreateDpsinterfaceMainList(ref List<Bussiness.Entitys.PTL.DpsInterfaceMain> mainList, ref List<Bussiness.Entitys.PTL.DpsInterface> dpsDetailList, List<Bussiness.Entitys.SMT.WmsReelLightArea> areaList, List<Bussiness.Entitys.SMT.WmsReelLightAreaDetail> areaDetailList)
        {
            foreach (var item in areaList)
            {
                Entitys.PTL.DpsInterfaceMain main = new Entitys.PTL.DpsInterfaceMain();
                main.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                main.OrderType = 3;
                main.ProofId = item.ProofId;
                main.Status = 0;
                main.OrderCode = item.LightCode;
                main.WareHouseCode = item.WareHouseCode;
                main.AreaCode = item.AreaId;
                mainList.Add(main);
                var thisAreaDetailList = areaDetailList.FindAll(a => a.AreaId == item.AreaId);
                foreach (var areadetail in thisAreaDetailList)
                {
                    Entitys.PTL.DpsInterface face = new Entitys.PTL.DpsInterface();
                    face.BatchNO = areadetail.BatchCode;
                    face.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    face.GoodsName = areadetail.MaterialCode;
                    face.LocationId = areadetail.LocationCode;
                    face.MakerName = "";
                    face.ProofId = item.ProofId;
                    face.Quantity = areadetail.OrgQuantity;
                    face.Spec = "";
                    face.Status = 0;
                    face.UserId = "";
                    face.ToteId = "";
                    face.OperateId = Guid.NewGuid().ToString();
                    face.RelationId = areadetail.Id;
                    face.MaterialLabel = areadetail.ReelId;
                    dpsDetailList.Add(face);
                }
            }
            return DataProcess.Success();
        }

    }
}
