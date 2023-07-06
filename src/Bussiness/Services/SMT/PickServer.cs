using Bussiness.Contracts.SMT;
using Bussiness.Entitys.SMT;
using HP.Core.Data;
using HP.Core.Sequence;
using HP.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Data.Orm.Extensions;
using HP.Utility.Extensions;

using Bussiness.Dtos;
using HP.Data.Orm;


namespace Bussiness.Services.SMT
{
    public class PickServer : Bussiness.Contracts.SMT.IPickContract
    {

        public IRepository<WmsPickDetail, int> WmsPickDetailRepository { get; set; }

        public IRepository<WmsPickMain, int> WmsPickMainRepository { get; set; }

        public IRepository<WmsPickOrderDetail, int> WmsPickOrderDetailRepository { get; set; }

        public IRepository<WmsPickOrderIssue, int> WmsPickOrderIssueRepository { get; set; }

        public IRepository<WmsPickOrderMain, int> WmsPickOrderMainRepository { get; set; }

        public IRepository<WmsPickStation, int> WmsPickStationRepository { get; set; }

        public IRepository<WmsPickDetailVM, int> WmsPickDetailVMRepository { get; set; }

        public IRepository<Entitys.StockVM, int> StockVMRepository { get; set; }

        public IRepository<Entitys.Stock, int> StockRepository { get; set; }
        public ISequenceContract SequenceContract { set; get; }

        public IRepository<WmsPickOrderArea, int> WmsPickOrderAreaRepository { get; set; }

        public IRepository<WmsPickOrderAreaDetail, int> WmsPickOrderAreaDetailRepository { get; set; }

        public IRepository<WmsPickOrderAreaDetailVM, int> WmsPickOrderAreaDetailVMRepository { get; set; }

        public IRepository<WmsSplitArea, int> WmsSplitAreaRepository { get; set; }

        public IRepository<WmsSplitAreaReel, int> WmsSplitAreaReelRepository { get; set; }

        public IRepository<WmsSplitAreaReelDetail, int> WmsSplitAreaReelDetailRepository { get; set; }

        public IRepository<WmsSplitIssue, int> WmsSplitIssueRepository { get; set; }
        public IRepository<WmsSplitIssueVM, int> WmsSplitIssueVMRepository { get; set; }

        public IRepository<WmsSplitMain, int> WmsSplitMainRepository { get; set; }
        public IQuery<PickDto> PickQuery { get; }

        public IRepository<Bussiness.Entitys.Label, int> LabelContract { get; set; }


        public Bussiness.Contracts.SMT.IStockLightContract StockLightContract { get; set; }

        public IRepository<Bussiness.Entitys.PTL.DpsInterfaceMain, int> DpsInterfaceMainRepository { get; set; }

        public IRepository<Bussiness.Entitys.PTL.DpsInterface, int> DpsInterfaceRepository { get; set; }

        public IRepository<HPC.BaseService.Models.Dictionary, int> DictionaryRepository { get; set; }

        public IQuery<WmsPickMain> WmsPickMains => WmsPickMainRepository.Query();

        public IQuery<PickDto> PickDtos => WmsPickMains.LeftJoin(DictionaryRepository.Query(), (wmspickmainentity, dictionary) => wmspickmainentity.Issue_Type == dictionary.Code)
            .LeftJoin(WareHouseContract.WareHouses, (wmspickmainentity, dictionary, warehouse) => wmspickmainentity.WareHouseCode == warehouse.Code)
            .Select((wmspickmainentity, dictionary, warehouse) => new Dtos.PickDto()
            {
                Org_Id = wmspickmainentity.Org_Id,
                Issue_No = wmspickmainentity.Issue_No,
                Parts_No = wmspickmainentity.Parts_No,
                Department_No = wmspickmainentity.Department_No,
                Wo_No = wmspickmainentity.Wo_No,
                Plan_Date = wmspickmainentity.Plan_Date,
                Custom_No = wmspickmainentity.Custom_No,
                User_Id = wmspickmainentity.User_Id,
                User_Name = wmspickmainentity.User_Name,
                Trans_Type = wmspickmainentity.Trans_Type,
                Status = wmspickmainentity.Status,
                Wms_Get_Date = wmspickmainentity.Wms_Get_Date,
                WareHouseCode = wmspickmainentity.WareHouseCode,
                WareHouseName = warehouse.Name,
                Issue_Type = wmspickmainentity.Issue_Type,
                ProofId = wmspickmainentity.ProofId,
                IsIssue = wmspickmainentity.IsIssue,
                IsCanCombine = wmspickmainentity.IsCanCombine,
                OrderType = wmspickmainentity.OrderType,
                InWareHouseCode = wmspickmainentity.InWareHouseCode,
                //  AddMaterial = wmspickmainentity.AddMaterial,
                Remark = wmspickmainentity.Remark,
                BillCode = wmspickmainentity.BillCode,
                Id = wmspickmainentity.Id,
                CreatedUserCode = wmspickmainentity.CreatedUserCode,
                CreatedUserName = wmspickmainentity.CreatedUserName,
                CreatedTime = wmspickmainentity.CreatedTime,
                UpdatedUserCode = wmspickmainentity.UpdatedUserCode,
                UpdatedUserName = wmspickmainentity.UpdatedUserName,
                UpdatedTime = wmspickmainentity.UpdatedTime,
                PickDictDescription = dictionary.Name,
            });

        public Bussiness.Contracts.IWareHouseContract WareHouseContract { get; set; }

        public Bussiness.Contracts.SMT.IShelfContract ShelfContract { get; set; }
        public HPC.BaseService.Contracts.IDictionaryContract DictionaryContract { get; set; }

        public Bussiness.Contracts.IInContract InContract { get; set; }
        /// <summary>
        /// 合并领料单
        /// </summary>
        /// <param name="Issue_HIdList"></param>
        /// <param name="IsWorkorder"></param>
        /// <returns></returns>
        public DataResult CombinePickOrder(List<int> Issue_HIdList, bool IsWorkorder = true)
        {
            try
            {
                //1判断状态是否正确
                string ids = "";
                if (Issue_HIdList.Count() > 0)
                {
                    foreach (var item in Issue_HIdList)
                    {
                        ids = ids + item.ToString() + ",";
                    }
                }
                ids = ids.Substring(0, ids.Length - 1);
                List<Entitys.SMT.WmsPickMain> pickMainList = this.WmsPickMainRepository.Query().Where(a => Issue_HIdList.Contains(a.Id)).ToList();
                foreach (var item in pickMainList)
                {
                    if (item.Status != (int)Bussiness.Enums.SMT.PickStatusEnum.WaitingCheck)
                    {
                        return DataProcess.Failure("领料单号:" + item.Issue_No + "状态不对");
                    }
                }
                List<Entitys.SMT.WmsPickDetailVM> pickDetailList = this.WmsPickDetailVMRepository.SqlQuery("SELECT * FROM VIEW_WMS_PICK_DETAIL WHERE ISSUE_HID  IN (" + ids + ")").ToList();
                List<Entitys.SMT.WmsPickDetail> pickDetailListCopy = this.WmsPickDetailRepository.SqlQuery("SELECT * FROM TB_WMS_PICK_DETAIL WHERE ISSUE_HID IN  (" + ids + ")").ToList();
                //List<Entitys.SMT.WmsPickDetail> pickDetailList = this.WmsPickDetailRepository.Query<Entitys.SMT.WmsPickDetail>("SELECT * FROM VIEW_WMS_PICK_DETAIL WHERE ISSUE_HID IN @IdList", new { IdList = Issue_HIdList });
                //2判断是否在同一仓库
                if (pickDetailList.Count != pickDetailListCopy.Count)
                {
                    return DataProcess.Failure("尚有物料信息未同步,请前往物料信息同步ERP物料");
                }
                if (pickDetailList.GroupBy(a => a.Out_WareHouse_No).Count() > 1)
                {
                    return DataProcess.Failure("所选择的领料单不在同一仓库");
                }
                //if (pickMainList.GroupBy(a => a.Wo_No).Count() > 1)
                //{
                //    return DataProcess.Failure("所选择的领料单不是同一工单");
                //}
                if (false)
                {
                    //using (this.UnitOfWork)
                    //{
                    //    UnitOfWork.TransactionEnabled = true;
                    //    string where = "";
                    //    foreach (var item in pickMainList)
                    //    {
                    //        where += "'" + item.Issue_HId.GetValueOrDefault(0).ToString() + "',";
                    //    }
                    //    where = where.Substring(0, where.Length - 1);
                    //    // foreach (var item in pickMainList)
                    //    {
                    //        var Connection = new System.Data.OracleClient.OracleConnection(); ;//Oracle.DataAccess.Client.OracleConnection();
                    //        Connection.ConnectionString = ConnectionString;
                    //        //Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand();
                    //        var cmd = new System.Data.OracleClient.OracleCommand();
                    //        cmd.Connection = Connection;
                    //        cmd.CommandText = "SELECT * from wmsuser.WMS_ISSUE_ITEM_STATION_ERP_V @ERP WHERE ISSUE_HID IN (" + where + ")";
                    //        //  cmd.CommandText = "SELECT * from WMS_ISSUE_ITEM_STATION WHERE ISSUE_HID IN (" + where + ")";
                    //        //  Oracle.DataAccess.Client.OracleDataAdapter dr1 = new Oracle.DataAccess.Client.OracleDataAdapter();
                    //        System.Data.OracleClient.OracleDataAdapter dr1 = new System.Data.OracleClient.OracleDataAdapter();
                    //        dr1.SelectCommand = cmd;
                    //        DataTable top1Dt = new DataTable();
                    //        dr1.Fill(top1Dt);
                    //        var StationList = new List<Bussiness.Entitys.SMT.WmsPickStation>();
                    //        if (top1Dt.Rows.Count > 0)
                    //        {
                    //            //StationList = Common.ConvertDataTableToList<Bussiness.Entitys.SMT.WmsPickStation>.ConvertToModel(top1Dt).ToList();
                    //            foreach (DataRow item in top1Dt.Rows)
                    //            {
                    //                Bussiness.Entitys.SMT.WmsPickStation pickStation = new Bussiness.Entitys.SMT.WmsPickStation();
                    //                pickStation.AppointQuantity = 0;
                    //                pickStation.AppointReelId = "";
                    //                pickStation.IsAssigned = 0;
                    //                pickStation.Issue_HId = Convert.ToInt32(item["Issue_HId"].ToString());
                    //                pickStation.Issue_LId = Convert.ToInt32(item["Issue_LId"].ToString());
                    //                pickStation.Line_Id = item["Line_Id"].ToString();
                    //                pickStation.Material_Id = item["Material_Id"].ToString();
                    //                pickStation.Quantity = Convert.ToInt32(Convert.ToDecimal(item["Quantity"].ToString()));
                    //                pickStation.Remark = "";// item["Remark"].ToString();
                    //                pickStation.Station_Id = item["Station_Id"].ToString();
                    //                pickStation.Status = 0;
                    //                pickStation.Fseqno = item["Fseqno"].ToString();
                    //                StationList.Add(pickStation);
                    //            }
                    //            foreach (var item in pickMainList)
                    //            {
                    //                var currentList = StationList.FindAll(a => a.Issue_HId == item.Issue_HId);
                    //                if (currentList != null && currentList.Count > 0)
                    //                {
                    //                    if (StationList != null && StationList.Count > 0)
                    //                    {
                    //                        //this.WmsPickStationRepository.Execute("DELETE FROM TB_WMS_PICK_STATION WHERE ISSUE_HID ="+item.Issue_HId);
                    //                        //   this.WmsPickStationRepository.Insert(StationList);
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    //foreach (var detial in pickDetailList.FindAll(a=>a.Issue_HId == item.Issue_HId))
                    //                    //{
                    //                    //    Bussiness.Entitys.SMT.WmsPickStation pickStation = new Bussiness.Entitys.SMT.WmsPickStation();
                    //                    //    pickStation.AppointQuantity = 0;
                    //                    //    pickStation.AppointReelId = "";
                    //                    //    pickStation.IsAssigned = 0;
                    //                    //    pickStation.Issue_HId = detial.Issue_HId;
                    //                    //    pickStation.Issue_LId = detial.Issue_LId;
                    //                    //    pickStation.Line_Id = "";
                    //                    //    pickStation.Material_Id = detial.Material_Id;
                    //                    //    pickStation.Quantity = detial.Quantity;
                    //                    //    pickStation.Remark = "";
                    //                    //    pickStation.Station_Id = "无站位信息";
                    //                    //    pickStation.Status = 0;
                    //                    //    StationList.Add(pickStation);
                    //                    //}
                    //                    return DataProcess.Failure("领料单号:" + item.Issue_No + "未找到站位表信息");
                    //                }
                    //            }
                    //            if (StationList != null && StationList.Count > 0)
                    //            {
                    //                this.WmsPickStationRepository.Execute("DELETE FROM TB_WMS_PICK_STATION WHERE ISSUE_HID IN @HIdList", new { HIdList = Issue_HIdList });
                    //                this.WmsPickStationRepository.Insert(StationList);
                    //            }

                    //        }
                    //        else
                    //        {
                    //            return DataProcess.Failure("未能获取到任意单据的站位表信息");
                    //            //foreach (var item in pickDetailList)
                    //            //{
                    //            //       Bussiness.Entitys.SMT.WmsPickStation pickStation = new Bussiness.Entitys.SMT.WmsPickStation();
                    //            //    pickStation.AppointQuantity = 0;
                    //            //    pickStation.AppointReelId = "";
                    //            //    pickStation.IsAssigned = 0;
                    //            //    pickStation.Issue_HId = item.Issue_HId;
                    //            //    pickStation.Issue_LId =item.Issue_LId;
                    //            //    pickStation.Line_Id = "";
                    //            //    pickStation.Material_Id = item.Material_Id;
                    //            //    pickStation.Quantity =item.Quantity;
                    //            //    pickStation.Remark = "";
                    //            //    pickStation.Station_Id = "无站位信息";
                    //            //    pickStation.Status = 0;
                    //            //    StationList.Add(pickStation);
                    //            //}
                    //        }
                    //    }
                    //    UnitOfWork.Commit();

                    //}

                }
                else
                {
                    var StationList = new List<Bussiness.Entitys.SMT.WmsPickStation>();
                    foreach (var item in pickDetailList)
                    {
                        Bussiness.Entitys.SMT.WmsPickStation pickStation = new Bussiness.Entitys.SMT.WmsPickStation();
                        pickStation.AppointQuantity = 0;
                        pickStation.AppointReelId = "";
                        pickStation.IsAssigned = 0;
                        pickStation.Issue_HId = item.Issue_HId;
                        pickStation.Issue_LId = item.Id;
                        pickStation.Line_Id = "";
                        pickStation.Material_Id = item.MaterialCode;
                        pickStation.Quantity = item.Quantity.GetValueOrDefault(0);
                        pickStation.Remark = "";
                        pickStation.Station_Id = "无站位信息";
                        pickStation.Status = 0;
                        pickStation.Fseqno = "无站台信息";
                        StationList.Add(pickStation);
                    }
                    // using (UnitOfWork)
                    {
                        //UnitOfWork.TransactionEnabled = true;
                        //this.WmsPickStationRepository.Execute("DELETE FROM TB_WMS_PICK_STATION WHERE ISSUE_HID IN @HIdList", new { HIdList = Issue_HIdList });
                        //this.WmsPickStationRepository.Insert(StationList);
                        //UnitOfWork.Commit();

                        WmsPickStationRepository.UnitOfWork.TransactionEnabled = true;
                        WmsPickStationRepository.Delete(a => Issue_HIdList.Contains(a.Issue_HId));

                        WmsPickStationRepository.InsertRange(StationList);
                        WmsPickStationRepository.UnitOfWork.Commit();
                    }
                }
                var AllStationList = this.WmsPickStationRepository.SqlQuery("SELECT * FROM TB_WMS_PICK_STATION WHERE ISSUE_HID IN  (" + ids + ")").ToList();
                if (AllStationList.Count == 0)
                {
                    return DataProcess.Failure("无站位信息");
                }
                var groupStationList = AllStationList.GroupBy(a => new { a.Material_Id, a.Fseqno });
                #region  创建拣货任务
                Bussiness.Entitys.SMT.WmsPickOrderMain pickOrderMain = new Entitys.SMT.WmsPickOrderMain();
                //pickOrderMain.CreateUserId =HPC.BaseService.Services.AuthorizationService.
                //pickOrderMain.CreateUserName = HP.BaseService.Identity.Account.UserName;
                pickOrderMain.IsNeedSplit = false;
                pickOrderMain.PickOrderCode = SequenceContract.Create("PickOrder");
                pickOrderMain.SplitNo = "";
                pickOrderMain.Status = 0;
                pickOrderMain.WareHouseCode = pickDetailList.First().Out_WareHouse_No;
                pickOrderMain.Wo_No = pickMainList.First().Wo_No;
                pickOrderMain.InWareHouseCode = pickMainList.First().InWareHouseCode;
                pickOrderMain.OrderType = pickMainList.First().OrderType;
                foreach (var item in pickMainList)
                {
                    pickOrderMain.Issue_No += item.Issue_No + ",";
                }
                pickOrderMain.Issue_No = pickOrderMain.Issue_No.Substring(0, pickOrderMain.Issue_No.Length - 1);
                List<Bussiness.Entitys.SMT.WmsPickOrderDetail> wmsPickOrderDetailList = new List<Entitys.SMT.WmsPickOrderDetail>();
                //List<Bussiness.Entitys.SMT.WmsPickOrderAreaDetail> wmsPickOrderAreaDetailList = new List<Entitys.SMT.WmsPickOrderAreaDetail>();
                List<Bussiness.Entitys.SMT.WmsPickOrderIssue> wmsPickOrderIssueList = new List<Entitys.SMT.WmsPickOrderIssue>();
                foreach (var item in groupStationList)
                {
                    Bussiness.Entitys.SMT.WmsPickOrderDetail wmsPickOrderDetail = new Entitys.SMT.WmsPickOrderDetail();
                    wmsPickOrderDetail.MaterialCode = item.FirstOrDefault().Material_Id;
                    wmsPickOrderDetail.PickOrderCode = pickOrderMain.PickOrderCode;
                    wmsPickOrderDetail.Station_Id = item.FirstOrDefault().Station_Id;
                    var materialInfo = pickDetailList.Find(a => a.MaterialCode == wmsPickOrderDetail.MaterialCode);
                    if (materialInfo == null)
                    {
                        continue;
                    }
                    wmsPickOrderDetail.Material_Level = materialInfo.Material_Level;
                    wmsPickOrderDetail.MaterialIsNeedSplit = materialInfo.IsNeedSplit;
                    wmsPickOrderDetail.IsElectronicMateria = materialInfo.IsElectronicMateria;
                    if (wmsPickOrderDetail.MaterialIsNeedSplit == true || IsWorkorder == false)
                    {
                        wmsPickOrderDetail.Quantity = Convert.ToInt32(item.Sum(a => a.Quantity));
                    }
                    else if(wmsPickOrderDetail.IsElectronicMateria==true) //电子获取超发比例
                    {
                        var quantity = Convert.ToInt32(item.Sum(a => a.Quantity));
                        var overQuantity = quantity * (1 + quantity * item.FirstOrDefault().OverRatio.GetValueOrDefault(0) / 100);
                        wmsPickOrderDetail.Quantity = quantity + Convert.ToInt32(overQuantity);
                    }
                    else
                    {
                        wmsPickOrderDetail.Quantity = Convert.ToInt32(item.Sum(a => a.Quantity));//+ 200;
                    }
                    wmsPickOrderDetail.OrgNeedQuantity = item.Sum(a => a.Quantity);

                    wmsPickOrderDetail.DetailId = Guid.NewGuid().ToString();
                    wmsPickOrderDetail.IsAssigned = 0;
                    wmsPickOrderDetail.CurQuantity = 0;
                    wmsPickOrderDetail.Status = 0;
                    wmsPickOrderDetail.Fseqno = item.FirstOrDefault().Fseqno;
                    wmsPickOrderDetail.Line_Id = item.FirstOrDefault().Line_Id;
                    // wmsPickOrderDetail.RealQauntity = 0;
                    wmsPickOrderDetailList.Add(wmsPickOrderDetail);

                }
                foreach (var item in pickMainList)
                {
                    Bussiness.Entitys.SMT.WmsPickOrderIssue issue = new Entitys.SMT.WmsPickOrderIssue();
                    issue.Issue_HId = item.Id;
                    issue.Issue_No = item.Issue_No;
                    issue.PickOrderCode = pickOrderMain.PickOrderCode;
                    issue.Wo_No = item.Wo_No;
                    issue.Status = 0;
                    issue.OrderType = item.OrderType;
                    issue.IssueType = item.Issue_Type;
                    wmsPickOrderIssueList.Add(issue);
                }
                #endregion
                //Bussiness.ShiYiTongServices.WmsPickOrderServer pickOrderServer = new WmsPickOrderServer(this.UnitOfWork);
                pickMainList.ForEach(a => a.Status = 1);
                pickDetailListCopy.ForEach(a => a.Status = 1);
                AllStationList.ForEach(a => a.Status = 1);
                WmsPickOrderMainRepository.UnitOfWork.TransactionEnabled = true;
                {
                    foreach (var item in pickMainList)
                    {
                        this.WmsPickMainRepository.Update(item);
                    }
                    foreach (var item in pickDetailListCopy)
                    {
                        this.WmsPickDetailRepository.Update(item);
                    }

                    foreach (var item in AllStationList)
                    {
                        this.WmsPickStationRepository.Update(item);
                    }

                    //插入拣选任务
                    this.WmsPickOrderMainRepository.Insert(pickOrderMain);
                    foreach (var item in wmsPickOrderDetailList)
                    {
                        this.WmsPickOrderDetailRepository.Insert(item);
                    }
                    foreach (var item in wmsPickOrderIssueList)
                    {
                        this.WmsPickOrderIssueRepository.Insert(item);
                    }

                }
                WmsPickOrderMainRepository.UnitOfWork.Commit();
                return DataProcess.Success();
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        #region 备料单 创建删除编辑 等操作
        public DataResult CreatePickEntity(WmsPickMain entity)
        {
            //PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
            //var list = ptlServer.GetLocations().ToList(); ;
            //return DataProcess.Failure();
            WmsPickMainRepository.UnitOfWork.TransactionEnabled = true;
            {
                // 判断是否有出库单号
                if (String.IsNullOrEmpty(entity.Issue_No))
                {
                    if (entity.OrderType==0)//出库单
                    {
                        entity.Issue_No = SequenceContract.Create("OutCode");
                    }
                    else //调拨单
                    {
                        entity.Issue_No = SequenceContract.Create("TransferOrder");
                    }
                }
                if (WmsPickMainRepository.Query().Any(a => a.Issue_No == entity.Issue_No))
                {
                    return DataProcess.Failure("该出库单号已存在");
                }
                entity.Status = 0;

                if (!WmsPickMainRepository.Insert(entity))
                {
                    return DataProcess.Failure(string.Format("出库单{0}新增失败", entity.Issue_No));
                }
                if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
                {

                    foreach (Bussiness.Entitys.SMT.WmsPickDetail item in entity.AddMaterial)
                    {
                        item.Issue_HId = entity.Id;
                        item.Out_WareHouse_No = entity.WareHouseCode;
                        item.In_WareHouse_No = entity.InWareHouseCode;
                        //item. = entity.BillCode;
                        item.Status = 0;
                        item.Issue_No = entity.Issue_No;
                        DataResult result = CreatePickMaterialEntity(item);
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
            WmsPickMainRepository.UnitOfWork.Commit();
            return DataProcess.Success(string.Format("出库单{0}新增成功", entity.Issue_No), entity);
        }

        public DataResult CreatePickMaterialEntity(WmsPickDetail entity)
        {
            //if (OutMaterials.Any(a => a.MaterialLabel == entity.MaterialLabel))
            //{
            //    return DataProcess.Failure("该入库条码已存在");
            //}
            if (WmsPickDetailRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("备料单物料{0}新增成功", entity.MaterialCode));
            }
            return DataProcess.Failure("操作失败");
        }

        public DataResult EditPickMain(WmsPickMain entity)
        {
            WmsPickMainRepository.UnitOfWork.TransactionEnabled = true;
            if (WmsPickMainRepository.Update(entity) <= 0)//
            {
                return DataProcess.Failure(string.Format("备料单{0}编辑失败", entity.Issue_No));
            }

            List<Entitys.SMT.WmsPickDetail> list = WmsPickDetailRepository.Query().Where(a => a.Issue_HId == entity.Id).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (WmsPickDetail item in list)
                {
                    DataResult result = RemovePickMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }

            if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
            {
                foreach (WmsPickDetail item in entity.AddMaterial)
                {
                    item.Issue_HId = entity.Id;
                    item.Out_WareHouse_No = entity.WareHouseCode;
                    //item. = entity.BillCode;
                    item.Status = 0;
                    item.Issue_No = entity.Issue_No;
                    DataResult result = CreatePickMaterialEntity(item);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }
            WmsPickMainRepository.UnitOfWork.Commit();
            return DataProcess.Success("编辑成功");
        }

        public DataResult RemovePickMain(int id)
        {
            WmsPickMain entity = WmsPickMainRepository.GetEntity(id);
            if (entity.Status != (int)Enums.SMT.PickStatusEnum.WaitingCheck)
            {
                return DataProcess.Failure("该备料单单执行中或已完成");
            }

            WmsPickMainRepository.UnitOfWork.TransactionEnabled = true;
            if (WmsPickMainRepository.Delete(id) <= 0)
            {
                return DataProcess.Failure(string.Format("备料单{0}删除失败", entity.Issue_No));
            }
            List<Entitys.SMT.WmsPickDetail> list = WmsPickDetailRepository.Query().Where(a => a.Issue_HId == entity.Id).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (WmsPickDetail item in list)
                {
                    DataResult result = RemovePickMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }
            WmsPickMainRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }

        public DataResult RemovePickMaterial(int id)
        {
            Entitys.SMT.WmsPickDetail entity = WmsPickDetailRepository.GetEntity(id);
            if (entity.Status != (int)Bussiness.Enums.SMT.PickStatusEnum.WaitingCheck)
            {
                return DataProcess.Failure("该备料状态不对,此时不允许删除");
            }
            if (WmsPickDetailRepository.Delete(id) > 0)
            {
                return DataProcess.Success(string.Format("备料单物料{0}删除成功", entity.MaterialCode));
            }
            return DataProcess.Failure("操作失败");
        }


        /// <summary>
        /// 作废拣选的ReelId
        /// </summary>
        /// <param name="Reel"></param>
        /// <returns></returns>
        public DataResult CancelPcikAreaDetailReel(string ReelId, string PickOrderCode)
        {
            var entity = WmsPickOrderAreaDetailRepository.Query().FirstOrDefault(a => a.ReelId == ReelId && a.PickOrderCode == PickOrderCode);
            if (entity == null)
            {
                return DataProcess.Failure("该拣货条码不存在此拣货单上");
            }//<Bussiness.Entitys.SMT.WmsPickOrderAreaDetail>("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE REELID='" + ReelId + "' AND PICKORDERCODE='" + PickOrderCode + "'");
            var pickOrderDetail = WmsPickOrderDetailRepository.Query().FirstOrDefault(a => a.DetailId == entity.PickOrderDetailId);//GetEntity("SELECT * FROM TB_WMS_PICK_ORDER_DETAIL WHERE PICKORDERCODE='" + PickOrderCode + "' AND DETAILID='" + entity.PickOrderDetailId + "'");
            var stockEntity = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == ReelId);/* stockServer.StockRepository.GetEntity<Bussiness.Entitys.Stock>("SELECT * FROM TB_WMS_REELID_STOCK WHERE REELID='" + ReelId + "'");*/
            if (entity != null)
            {
                if (entity.Status == 2)
                {
                    return DataProcess.Failure("此条码处于待拆盘状态");
                }
                if (entity.Status >= 4)
                {
                    return DataProcess.Failure("此条码已复核");
                    // pickOrderDetail.ConfirmQuantity = pickOrderDetail.ConfirmQuantity - entity.NeedQuantity;
                }
                //if (entity.Status ==5)
                //{
                //     return DataProcess.Failure("此ReelId已作废");
                //}
                // entity.Status = 5;
                pickOrderDetail.CurQuantity = pickOrderDetail.CurQuantity - entity.NeedQuantity;
                if (pickOrderDetail.CurQuantity < pickOrderDetail.Quantity)
                {
                    if (pickOrderDetail.CurQuantity == 0)
                    {
                        pickOrderDetail.Status = 1;
                    }
                    else
                    {
                        pickOrderDetail.Status = 2;
                    }
                }
                //  using (this.UnitOfWork)
                {
                    //    this.UnitOfWork.TransactionEnabled = true;
                    WmsPickOrderAreaDetailRepository.UnitOfWork.TransactionEnabled = true;
                    WmsPickOrderAreaDetailRepository.Delete(entity);
                    WmsPickOrderDetailRepository.Update(pickOrderDetail);
                    if (stockEntity != null)
                    {
                        StockRepository.Delete(stockEntity);
                    }
                    WmsPickOrderAreaDetailRepository.UnitOfWork.Commit();
                }
            }
            else
            {
                return DataProcess.Failure("此ReelId不在此次拣货单中");
            }
            return DataProcess.Success();
        }
        #endregion

        #region 备料单管理

        /// <summary>
        /// 分配拣货单拣货任务
        /// </summary>
        /// <param name="PickOrderCode"></param>
        /// <returns></returns>
        public DataResult CheckPickOrder(List<string> PickOrderCodeList)
        {
            try
            {
                //1检查单据状态
                List<Bussiness.Entitys.SMT.WmsPickOrderMain> pickOrderMainList = this.WmsPickOrderMainRepository.Query().Where(a => PickOrderCodeList.Contains(a.PickOrderCode)).ToList();
                if (pickOrderMainList.Count != PickOrderCodeList.Count)
                {
                    return DataProcess.Failure("拣货单号不存在");
                }
                foreach (var pickOrderMain in pickOrderMainList)
                {
                    if (pickOrderMain.Status != 0 && pickOrderMain.Status != 1 && pickOrderMain.Status != 2 && pickOrderMain.Status != 3)
                    {
                        return DataProcess.Failure(pickOrderMain.PickOrderCode + "该领料单已料过账或已作废");
                    }
                }
                List<Bussiness.Entitys.SMT.WmsPickOrderDetail> pickOrderDetailList = this.WmsPickOrderDetailRepository.Query().Where(a => (a.Status == 1 || a.Status == 0 || a.Status == 2) && PickOrderCodeList.Contains(a.PickOrderCode)).ToList();//("SELECT * FROM TB_WMS_PICK_ORDER_DETAIL WHERE PICKORDERCODE IN @CodeList AND STATUS IN(0,1,2)", new { CodeList = PickOrderCodeList });//
                List<Bussiness.Entitys.SMT.WmsPickOrderAreaDetail> pickOrderAreaDetailList = new List<Entitys.SMT.WmsPickOrderAreaDetail>();
                List<Bussiness.Entitys.Stock> stockIdList = new List<Entitys.Stock>();
                if (pickOrderDetailList != null && pickOrderDetailList.Count > 0)
                {
                    foreach (var item in pickOrderDetailList)
                    {
                        int index = PickOrderCodeList.IndexOf(item.PickOrderCode);
                        item.SortIndex = index;
                    }
                    var groupMaterial = pickOrderDetailList.GroupBy(a => a.MaterialCode);

                    foreach (var item in groupMaterial)
                    {
                        var aa = item.ToList();
                        var NewItemList = new List<Bussiness.Entitys.SMT.WmsPickOrderDetail>();
                        NewItemList = item.OrderBy(a => a.SortIndex).ToList();
                        //continue;
                        var AvailableStockList = GetCurrentAvailableStock(item.FirstOrDefault().MaterialCode, pickOrderMainList.FirstOrDefault().WareHouseCode);

                        if (item.FirstOrDefault().MaterialIsNeedSplit == true)
                        {
                            foreach (var station in item)
                            {
                                var stationNeedQuantiy = station.Quantity.GetValueOrDefault(0) - station.CurQuantity.GetValueOrDefault(0);
                                if (stationNeedQuantiy <= 0)
                                {
                                    continue;
                                }
                                foreach (var stock in AvailableStockList.FindAll(a => a.IsLocked == false))
                                {
                                    var AvailableQuantity = stock.Quantity - stock.LockedQuantity;
                                    if (AvailableQuantity <= 0)
                                    {
                                        continue;
                                    }
                                    if (AvailableQuantity >= stationNeedQuantiy)
                                    {
                                        Entitys.SMT.WmsPickOrderAreaDetail areaPickDetail = new Entitys.SMT.WmsPickOrderAreaDetail();
                                        areaPickDetail.AreaId = stock.AreaCode;
                                        areaPickDetail.PickOrderCode = station.PickOrderCode;
                                        areaPickDetail.PickOrderDetailId = station.DetailId;
                                        areaPickDetail.LocationCode = stock.LocationCode;
                                        areaPickDetail.OrgQuantity = Convert.ToInt32(stock.Quantity);
                                        areaPickDetail.ReelId = stock.MaterialLabel;
                                        areaPickDetail.Station_Id = station.Station_Id;
                                        areaPickDetail.Status = 0;
                                        areaPickDetail.NeedQuantity = stationNeedQuantiy;
                                        areaPickDetail.MaterialCode = stock.MaterialCode;
                                        areaPickDetail.WareHouseCode = stock.WareHouseCode;
                                        areaPickDetail.BatchCode = stock.BatchCode;
                                        areaPickDetail.ReelCreateCode = stock.ManufactureDate.ToString();
                                        areaPickDetail.Fseqno = station.Fseqno;
                                        areaPickDetail.MaterialCode = station.MaterialCode;
                                        areaPickDetail.SupplierCode = stock.SupplierCode;
                                        areaPickDetail.RealPickedQuantity = areaPickDetail.NeedQuantity.GetValueOrDefault(0);
                                        var curentStock = AvailableStockList.Find(a => a.Id == stock.Id);
                                        // curentStock.IsLocked = true;
                                        curentStock.LockedQuantity += Convert.ToInt32(stationNeedQuantiy);


                                        //  CurrentStation.AppointReelId = stock.ReelId;
                                        // CurrentStation.AppointQuantity = station.Quantity;
                                        // CurrentStation.Status = 3;
                                        pickOrderAreaDetailList.Add(areaPickDetail);
                                        //if (!stockIdList.Contains(curentStock.Id))
                                        //{
                                        //    stockIdList.Add(curentStock.Id);
                                        //}
                                        //if (stockIdList.FirstOrDefault(a=>a.Id==curentStock.Id)==null)
                                        //{

                                        //}
                                        var existStock = stockIdList.Find(a => a.Id == curentStock.Id);
                                        if (existStock == null || existStock.Id == 0)
                                        {
                                            stockIdList.Add(curentStock);
                                        }
                                        else
                                        {
                                            //  existStock.IsLocked = true;
                                            existStock.LockedQuantity = curentStock.LockedQuantity;
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        Entitys.SMT.WmsPickOrderAreaDetail areaPickDetail = new Entitys.SMT.WmsPickOrderAreaDetail();
                                        areaPickDetail.AreaId = stock.AreaCode;
                                        areaPickDetail.PickOrderCode = station.PickOrderCode;
                                        areaPickDetail.PickOrderDetailId = station.DetailId;
                                        areaPickDetail.LocationCode = stock.LocationCode;
                                        areaPickDetail.OrgQuantity = Convert.ToInt32(stock.Quantity);
                                        areaPickDetail.Station_Id = station.Station_Id;
                                        areaPickDetail.Status = 0;
                                        areaPickDetail.NeedQuantity = Convert.ToInt32(AvailableQuantity);
                                        areaPickDetail.ReelId = stock.MaterialLabel;
                                        areaPickDetail.MaterialCode = stock.MaterialCode;
                                        areaPickDetail.WareHouseCode = stock.WareHouseCode;
                                        areaPickDetail.BatchCode = stock.BatchCode;
                                        areaPickDetail.ReelCreateCode = stock.ManufactureDate.ToString();
                                        areaPickDetail.Fseqno = station.Fseqno;
                                        areaPickDetail.MaterialCode = station.MaterialCode;
                                        areaPickDetail.SupplierCode = stock.SupplierCode;
                                        areaPickDetail.RealPickedQuantity = areaPickDetail.NeedQuantity.GetValueOrDefault(0);

                                        var curentStock = AvailableStockList.Find(a => a.Id == stock.Id);
                                        // curentStock.IsLocked = true;
                                        curentStock.LockedQuantity += AvailableQuantity;
                                        stationNeedQuantiy = stationNeedQuantiy - Convert.ToInt32(AvailableQuantity);

                                        //var CurrentStation = AllStationList.Find(a => a.Station_Id == station.Station_Id && a.Issue_LId == station.Issue_LId && a.Issue_HId == station.Issue_HId);
                                        //  CurrentStation.AppointReelId = stock.ReelId;
                                        // CurrentStation.AppointQuantity = station.Quantity;
                                        //CurrentStation.Status = 3;
                                        pickOrderAreaDetailList.Add(areaPickDetail);
                                        //if (!stockIdList.Contains(curentStock.Id))
                                        //{
                                        //    stockIdList.Add(curentStock.Id);
                                        //}
                                        var existStock = stockIdList.Find(a => a.Id == curentStock.Id);
                                        if (existStock == null || existStock.Id == 0)
                                        {
                                            stockIdList.Add(curentStock);
                                        }
                                        else
                                        {
                                            //existStock.IsLocked = true;
                                            existStock.LockedQuantity = curentStock.LockedQuantity;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item.FirstOrDefault().IsElectronicMateria == true)
                            {
                                foreach (var station in item)
                                {
                                    var stationNeedQuantiy = station.Quantity.GetValueOrDefault(0) - station.CurQuantity.GetValueOrDefault(0);
                                    if (stationNeedQuantiy <= 0)
                                    {
                                        continue;
                                    }
                                    foreach (var stock in AvailableStockList.FindAll(A => A.IsLocked == false))
                                    {

                                        var AvailableQuantity = stock.Quantity - stock.LockedQuantity;
                                        if (AvailableQuantity <= 0)
                                        {
                                            continue;
                                        }
                                        var currentStockList = AvailableStockList.FindAll(a => a.IsLocked == false);
                                        //if (currentStockList.Count == 1)
                                        //{
                                        //    if (AvailableQuantity >= stationNeedQuantiy)
                                        //    {
                                        //        Entitys.SMT.WmsPickOrderAreaDetail areaPickDetail = new Entitys.SMT.WmsPickOrderAreaDetail();
                                        //        areaPickDetail.AreaId = stock.AreaId;
                                        //        areaPickDetail.PickOrderCode = station.PickOrderCode;
                                        //        areaPickDetail.PickOrderDetailId = station.DetailId;
                                        //        areaPickDetail.LocationCode = stock.LocationCode;
                                        //        areaPickDetail.OrgQuantity = stock.MaterialCount;
                                        //        areaPickDetail.ReelId = stock.ReelId;
                                        //        areaPickDetail.Station_Id = station.Station_Id;
                                        //        areaPickDetail.Status = 0;
                                        //        areaPickDetail.NeedQuantity = stationNeedQuantiy;
                                        //        areaPickDetail.MaterialCode = stock.MaterialCode;
                                        //        areaPickDetail.WareHouseCode = stock.WareHouseCode;
                                        //        areaPickDetail.BatchCode = stock.BatchCode;
                                        //        areaPickDetail.ReelCreateCode = stock.ReelCreateCode;
                                        //        areaPickDetail.Fseqno = station.Fseqno;
                                        //        areaPickDetail.MaterialCode = station.MaterialCode;

                                        //        var curentStock = AvailableStockList.Find(a => a.Id == stock.Id);
                                        //        //curentStock.IsLocked = 1;
                                        //        curentStock.LockedQuantity += Convert.ToInt32(stationNeedQuantiy);


                                        //        //  CurrentStation.AppointReelId = stock.ReelId;
                                        //        // CurrentStation.AppointQuantity = station.Quantity;
                                        //        // CurrentStation.Status = 3;
                                        //        pickOrderAreaDetailList.Add(areaPickDetail);
                                        //        if (!stockIdList.Contains(curentStock.Id))
                                        //        {
                                        //            stockIdList.Add(curentStock.Id.GetValueOrDefault(0));
                                        //        }
                                        //        break;
                                        //    }
                                        //    else
                                        //    {
                                        //        Entitys.SMT.WmsPickOrderAreaDetail areaPickDetail = new Entitys.SMT.WmsPickOrderAreaDetail();
                                        //        areaPickDetail.AreaId = stock.AreaId;
                                        //        areaPickDetail.PickOrderCode = station.PickOrderCode;
                                        //        areaPickDetail.PickOrderDetailId = station.DetailId;
                                        //        areaPickDetail.LocationCode = stock.LocationCode;
                                        //        areaPickDetail.OrgQuantity = stock.MaterialCount;
                                        //        areaPickDetail.Station_Id = station.Station_Id;
                                        //        areaPickDetail.Status = 0;
                                        //        areaPickDetail.NeedQuantity = AvailableQuantity;
                                        //        areaPickDetail.ReelId = stock.ReelId;
                                        //        areaPickDetail.MaterialCode = stock.MaterialCode;
                                        //        areaPickDetail.WareHouseCode = stock.WareHouseCode;
                                        //        areaPickDetail.BatchCode = stock.BatchCode;
                                        //        areaPickDetail.ReelCreateCode = stock.ReelCreateCode;
                                        //        areaPickDetail.Fseqno = station.Fseqno;
                                        //        areaPickDetail.MaterialCode = station.MaterialCode;

                                        //        var curentStock = AvailableStockList.Find(a => a.Id == stock.Id);
                                        //        curentStock.IsLocked = 1;
                                        //        curentStock.LockedQuantity += AvailableQuantity;
                                        //        stationNeedQuantiy = stationNeedQuantiy - AvailableQuantity;

                                        //        //var CurrentStation = AllStationList.Find(a => a.Station_Id == station.Station_Id && a.Issue_LId == station.Issue_LId && a.Issue_HId == station.Issue_HId);
                                        //        //  CurrentStation.AppointReelId = stock.ReelId;
                                        //        // CurrentStation.AppointQuantity = station.Quantity;
                                        //        //CurrentStation.Status = 3;
                                        //        pickOrderAreaDetailList.Add(areaPickDetail);
                                        //        if (!stockIdList.Contains(curentStock.Id))
                                        //        {
                                        //            stockIdList.Add(curentStock.Id.GetValueOrDefault(0));
                                        //        }
                                        //    }
                                        //}
                                        // else
                                        // {
                                        if (AvailableQuantity >= stationNeedQuantiy)
                                        {
                                            Entitys.SMT.WmsPickOrderAreaDetail areaPickDetail = new Entitys.SMT.WmsPickOrderAreaDetail();
                                            areaPickDetail.AreaId = stock.AreaCode;
                                            areaPickDetail.PickOrderCode = station.PickOrderCode;
                                            areaPickDetail.PickOrderDetailId = station.DetailId;
                                            areaPickDetail.LocationCode = stock.LocationCode;
                                            areaPickDetail.OrgQuantity = Convert.ToInt32(stock.Quantity);
                                            areaPickDetail.ReelId = stock.MaterialLabel;
                                            areaPickDetail.Station_Id = station.Station_Id;
                                            areaPickDetail.Status = 0;
                                            areaPickDetail.NeedQuantity = Convert.ToInt32(AvailableQuantity);
                                            areaPickDetail.RealPickedQuantity = areaPickDetail.NeedQuantity.GetValueOrDefault(0);
                                            areaPickDetail.MaterialCode = stock.MaterialCode;
                                            areaPickDetail.WareHouseCode = stock.WareHouseCode;
                                            areaPickDetail.BatchCode = stock.BatchCode;
                                            areaPickDetail.ReelCreateCode = stock.ManufactureDate.ToString();
                                            areaPickDetail.SupplierCode = stock.SupplierCode;
                                            areaPickDetail.Fseqno = station.Fseqno;
                                            areaPickDetail.MaterialCode = station.MaterialCode;

                                            var curentStock = AvailableStockList.Find(a => a.Id == stock.Id);
                                            curentStock.IsLocked = true;
                                            //var CurrentStation = AllStationList.Find(a => NullToEmpty(a.Station_Id) == NullToEmpty(station.Station_Id) && a.Issue_LId == station.Issue_LId && a.Issue_HId == station.Issue_HId);
                                            station.IsAssigned = 1;
                                            //  CurrentStation.AppointReelId = stock.ReelId;
                                            //  CurrentStation.AppointQuantity = stock.MaterialCount;
                                            //  CurrentStation.Status = 3;

                                            //此处判断分配给你他之后库存总量是否满足
                                            if (AvailableStockList.FindAll(a => a.IsLocked == false).Sum(a => a.Quantity) < item.ToList().FindAll(a => a.MaterialCode == station.MaterialCode && a.IsAssigned != 1).Sum(a => (a.Quantity - a.CurQuantity)))
                                            {
                                                curentStock.IsLocked = false;
                                                areaPickDetail.NeedQuantity = stationNeedQuantiy;
                                                areaPickDetail.RealPickedQuantity = areaPickDetail.NeedQuantity.GetValueOrDefault(0);
                                            }
                                            //if (stock.LockedQuantity.GetValueOrDefault(0) > 0)
                                            //{
                                            //    curentStock.IsLocked = 0;
                                            //    areaPickDetail.NeedQuantity = stationNeedQuantiy;
                                            //}
                                            curentStock.LockedQuantity += Convert.ToDecimal(areaPickDetail.NeedQuantity);
                                            pickOrderAreaDetailList.Add(areaPickDetail);
                                            //if (!stockIdList.Contains(curentStock.Id))
                                            //{
                                            //    stockIdList.Add(curentStock.Id);
                                            //}
                                            var existStock = stockIdList.Find(a => a.Id == curentStock.Id);
                                            if (existStock == null || existStock.Id == 0)
                                            {
                                                stockIdList.Add(curentStock);
                                            }
                                            else
                                            {
                                                existStock.IsLocked = true;
                                                existStock.LockedQuantity = curentStock.LockedQuantity;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            Entitys.SMT.WmsPickOrderAreaDetail areaPickDetail = new Entitys.SMT.WmsPickOrderAreaDetail();
                                            areaPickDetail.AreaId = stock.AreaCode;
                                            areaPickDetail.PickOrderCode = station.PickOrderCode;
                                            areaPickDetail.PickOrderDetailId = station.DetailId;
                                            areaPickDetail.LocationCode = stock.LocationCode;
                                            areaPickDetail.OrgQuantity = Convert.ToInt32(stock.Quantity);
                                            areaPickDetail.ReelId = stock.MaterialLabel;
                                            areaPickDetail.Station_Id = station.Station_Id;
                                            areaPickDetail.Status = 0;
                                            areaPickDetail.NeedQuantity = Convert.ToInt32(AvailableQuantity);
                                            areaPickDetail.RealPickedQuantity = areaPickDetail.NeedQuantity.GetValueOrDefault(0);
                                            areaPickDetail.MaterialCode = stock.MaterialCode;
                                            areaPickDetail.WareHouseCode = stock.WareHouseCode;
                                            areaPickDetail.BatchCode = stock.BatchCode;
                                            areaPickDetail.ReelCreateCode = stock.ManufactureDate.ToString();
                                            areaPickDetail.Fseqno = station.Fseqno;
                                            areaPickDetail.MaterialCode = station.MaterialCode;
                                            areaPickDetail.SupplierCode = stock.SupplierCode;
                                            var curentStock = AvailableStockList.Find(a => a.Id == stock.Id);
                                            curentStock.IsLocked = true;
                                            curentStock.LockedQuantity += AvailableQuantity;
                                            //  var CurrentStation = AllStationList.Find(a => NullToEmpty(a.Station_Id) == NullToEmpty(station.Station_Id) && a.Issue_LId == station.Issue_LId && a.Issue_HId== station.Issue_HId);
                                            //  CurrentStation.AppointReelId = stock.ReelId;
                                            //  CurrentStation.AppointQuantity = stock.MaterialCount;
                                            //  CurrentStation.Status = 3;
                                            stationNeedQuantiy = stationNeedQuantiy - Convert.ToInt32(AvailableQuantity);
                                            pickOrderAreaDetailList.Add(areaPickDetail);
                                            //if (!stockIdList.Contains(curentStock.Id))
                                            //{
                                            //    stockIdList.Add(curentStock.Id);
                                            //}
                                            var existStock = stockIdList.Find(a => a.Id == curentStock.Id);
                                            if (existStock == null || existStock.Id == 0)
                                            {
                                                stockIdList.Add(curentStock);
                                            }
                                            else
                                            {
                                                existStock.IsLocked = true;
                                                existStock.LockedQuantity = curentStock.LockedQuantity;
                                            }
                                            //  break;
                                        }
                                        // }

                                    }
                                }

                            }
                            else
                            {
                                foreach (var station in item)
                                {
                                    var stationNeedQuantiy = station.Quantity.GetValueOrDefault(0) - station.CurQuantity.GetValueOrDefault(0);
                                    if (stationNeedQuantiy <= 0)
                                    {
                                        continue;
                                    }
                                    foreach (var stock in AvailableStockList)
                                    {

                                        var AvailableQuantity = stock.Quantity - stock.LockedQuantity;
                                        if (AvailableQuantity <= 0)
                                        {
                                            continue;
                                        }
                                        var currentStockList = AvailableStockList;
                                        if (AvailableQuantity >= stationNeedQuantiy)
                                        {
                                            Entitys.SMT.WmsPickOrderAreaDetail areaPickDetail = new Entitys.SMT.WmsPickOrderAreaDetail();
                                            areaPickDetail.AreaId = stock.AreaCode;
                                            areaPickDetail.PickOrderCode = station.PickOrderCode;
                                            areaPickDetail.PickOrderDetailId = station.DetailId;
                                            areaPickDetail.LocationCode = stock.LocationCode;
                                            areaPickDetail.OrgQuantity = stationNeedQuantiy;// Convert.ToInt32(stock.Quantity);
                                            areaPickDetail.ReelId = stock.MaterialLabel;
                                            areaPickDetail.Station_Id = station.Station_Id;
                                            areaPickDetail.Status = 0;
                                            areaPickDetail.NeedQuantity = stationNeedQuantiy;//Convert.ToInt32(AvailableQuantity);
                                            areaPickDetail.RealPickedQuantity = areaPickDetail.NeedQuantity.GetValueOrDefault(0);
                                            areaPickDetail.MaterialCode = stock.MaterialCode;
                                            areaPickDetail.WareHouseCode = stock.WareHouseCode;
                                            areaPickDetail.BatchCode = stock.BatchCode;
                                            areaPickDetail.ReelCreateCode = stock.ManufactureDate.ToString();
                                            areaPickDetail.SupplierCode = stock.SupplierCode;

                                            areaPickDetail.Fseqno = station.Fseqno;
                                            areaPickDetail.MaterialCode = station.MaterialCode;

                                            var curentStock = AvailableStockList.Find(a => a.Id == stock.Id);
                                            curentStock.IsLocked = true;
                                            //var CurrentStation = AllStationList.Find(a => NullToEmpty(a.Station_Id) == NullToEmpty(station.Station_Id) && a.Issue_LId == station.Issue_LId && a.Issue_HId == station.Issue_HId);
                                            station.IsAssigned = 1;
                                            //  CurrentStation.AppointReelId = stock.ReelId;
                                            //  CurrentStation.AppointQuantity = stock.MaterialCount;
                                            //  CurrentStation.Status = 3;

                                            //此处判断分配给你他之后库存总量是否满足
                                            //if (AvailableStockList.FindAll(a => a.IsLocked == false).Sum(a => a.Quantity) < item.ToList().FindAll(a => a.MaterialCode == station.MaterialCode && a.IsAssigned != 1).Sum(a => (a.Quantity - a.CurQuantity)))
                                            //{
                                            //    curentStock.IsLocked = false;
                                            //    areaPickDetail.NeedQuantity = stationNeedQuantiy;
                                            //}
                                            //if (stock.LockedQuantity.GetValueOrDefault(0) > 0)
                                            //{
                                            //    curentStock.IsLocked = 0;
                                            //    areaPickDetail.NeedQuantity = stationNeedQuantiy;
                                            //}
                                            curentStock.LockedQuantity += stationNeedQuantiy;
                                            pickOrderAreaDetailList.Add(areaPickDetail);
                                            //if (!stockIdList.Contains(curentStock.Id))
                                            //{
                                            //    stockIdList.Add(curentStock.Id);
                                            //}
                                            var existStock = stockIdList.Find(a => a.Id == curentStock.Id);
                                            if (existStock == null || existStock.Id == 0)
                                            {
                                                stockIdList.Add(curentStock);
                                            }
                                            else
                                            {
                                                existStock.IsLocked = true;
                                                existStock.LockedQuantity = curentStock.LockedQuantity;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            Entitys.SMT.WmsPickOrderAreaDetail areaPickDetail = new Entitys.SMT.WmsPickOrderAreaDetail();
                                            areaPickDetail.AreaId = stock.AreaCode;
                                            areaPickDetail.PickOrderCode = station.PickOrderCode;
                                            areaPickDetail.PickOrderDetailId = station.DetailId;
                                            areaPickDetail.LocationCode = stock.LocationCode;
                                            areaPickDetail.OrgQuantity = Convert.ToInt32(AvailableQuantity);//Convert.ToInt32(stock.Quantity);
                                            areaPickDetail.ReelId = stock.MaterialLabel;
                                            areaPickDetail.Station_Id = station.Station_Id;
                                            areaPickDetail.Status = 0;
                                            areaPickDetail.NeedQuantity = Convert.ToInt32(AvailableQuantity);
                                            areaPickDetail.RealPickedQuantity = areaPickDetail.NeedQuantity.GetValueOrDefault(0);
                                            areaPickDetail.MaterialCode = stock.MaterialCode;
                                            areaPickDetail.WareHouseCode = stock.WareHouseCode;
                                            areaPickDetail.BatchCode = stock.BatchCode;
                                            areaPickDetail.ReelCreateCode = stock.ManufactureDate.ToString();
                                            areaPickDetail.Fseqno = station.Fseqno;
                                            areaPickDetail.MaterialCode = station.MaterialCode;
                                            areaPickDetail.SupplierCode = stock.SupplierCode;

                                            var curentStock = AvailableStockList.Find(a => a.Id == stock.Id);
                                            curentStock.IsLocked = true;
                                            //  var CurrentStation = AllStationList.Find(a => NullToEmpty(a.Station_Id) == NullToEmpty(station.Station_Id) && a.Issue_LId == station.Issue_LId && a.Issue_HId== station.Issue_HId);
                                            //  CurrentStation.AppointReelId = stock.ReelId;
                                            //  CurrentStation.AppointQuantity = stock.MaterialCount;
                                            //  CurrentStation.Status = 3;
                                            curentStock.LockedQuantity += AvailableQuantity;
                                            stationNeedQuantiy = stationNeedQuantiy - Convert.ToInt32(AvailableQuantity);
                                            pickOrderAreaDetailList.Add(areaPickDetail);
                                            //if (!stockIdList.Contains(curentStock.Id))
                                            //{
                                            //    stockIdList.Add(curentStock.Id);
                                            //}
                                            var existStock = stockIdList.Find(a => a.Id == curentStock.Id);
                                            if (existStock == null || existStock.Id == 0)
                                            {
                                                stockIdList.Add(curentStock);
                                            }
                                            else
                                            {
                                                existStock.IsLocked = true;
                                                existStock.LockedQuantity = curentStock.LockedQuantity;
                                            }
                                            //  break;
                                        }
                                        // }

                                    }
                                }

                            }
                        }
                    }
                    //  return DataProcess.Success();
                    if (pickOrderAreaDetailList.Count == 0)
                    {
                        return DataProcess.Failure("目前没有任意物料可以分配");
                    }
                    //是否需要拆盘
                    var ReelGroup = pickOrderAreaDetailList.GroupBy(a => a.ReelId);
                    bool IsNeedSplit = pickOrderAreaDetailList.Find(a => a.NeedQuantity != a.OrgQuantity) == null ? false : true;// ReelGroup.Count()== pickAreaDetailList.Count()?false:true;

                    //Bussiness.PTLServices.PTLServer ptlServer = new PTLServices.PTLServer(UnitOfWork);
                    //Bussiness.ShiYiTongServices.WmsSplitServer splitServer = new WmsSplitServer(UnitOfWork);
                    //Bussiness.Services.StockServer stockServer = new Services.StockServer(UnitOfWork);
                    //Bussiness.ShiYiTongServices.WmsPickOrderServer pickOrderServer = new WmsPickOrderServer(UnitOfWork);
                    //foreach (var item in pickOrderAreaDetailList)
                    //{
                    //    item.CurQuantity = wmsPickOrderAreaDetailList.FindAll(a => a.PickOrderDetailId == item.DetailId).ToList().Sum(a => a.NeedQuantity);
                    //}

                    //更新已分配数量
                    foreach (var item in pickOrderDetailList)
                    {
                        //var ReadyPickAreaDetailList = WmsPickOrderAreaDetailRepository.SqlQuery("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE PICKORDERCODE IN @CodeList AND PICKORDERDETAILID ='" + item.DetailId + "' AND STATUS <5", new { CodeList = PickOrderCodeList });

                        var ReadyPickAreaDetailList = WmsPickOrderAreaDetailRepository.Query().Where(a => a.PickOrderDetailId == item.DetailId && a.Status < 5 && PickOrderCodeList.Contains(a.PickOrderCode)).ToList();
                        var ReadyQauntiy = 0;
                        ReadyQauntiy = ReadyPickAreaDetailList.Sum(a => a.RealPickedQuantity) + pickOrderAreaDetailList.FindAll(a => a.PickOrderCode == item.PickOrderCode && a.PickOrderDetailId == item.DetailId && a.MaterialCode == item.MaterialCode).Sum(a => a.RealPickedQuantity);
                        if (ReadyQauntiy >= item.Quantity)
                        {
                            item.CurQuantity = ReadyQauntiy;
                            item.Status = 3;
                        }
                        else if (ReadyQauntiy > 0 && ReadyQauntiy < item.Quantity)
                        {
                            item.CurQuantity = ReadyQauntiy;
                            item.Status = 2;
                        }
                        else
                        {
                            item.CurQuantity = 0;
                            item.Status = 1;
                        }

                    }

                    if (IsNeedSplit)
                    {

                        Entitys.SMT.WmsSplitMain splitMain = new Entitys.SMT.WmsSplitMain();
                        //splitMain.Id = 111111;
                        //splitMain.CreateUserId = RT.BaseService.Identity.Account.UserId;
                        //splitMain.CreateUserName = RT.BaseService.Identity.Account.UserName;
                        //  splitMain.PickOrderCode = pickOrderMain.PickOrderCode;
                        foreach (var item in pickOrderMainList)
                        {
                            splitMain.PickOrderCode += item.PickOrderCode + ",";
                        }
                        splitMain.PickOrderCode = splitMain.PickOrderCode.Substring(0, splitMain.PickOrderCode.Length - 1);
                        //pickOrderMainList.FirstOrDefault().Wo_No;//工单号
                        // Random dom = new Random();
                        splitMain.SplitNo = SequenceContract.Create("SplitOrder");//"S" + DateTime.Now.ToString("yyyyMMddhhmmss") + dom.Next(1, 10000).ToString();
                        splitMain.Status = 0;
                        splitMain.WareHouseCode = pickOrderMainList.FirstOrDefault().WareHouseCode;
                        splitMain.ProofId = Guid.NewGuid().ToString();
                        //pickOrderMain.IsNeedSplit = 1;
                        // pickOrderMain.SplitNo = splitMain.SplitNo;

                        List<Entitys.SMT.WmsSplitAreaReel> splitReelList = new List<Entitys.SMT.WmsSplitAreaReel>(); List<Entitys.SMT.WmsSplitAreaReelDetail> splitReelDetailList = new List<Entitys.SMT.WmsSplitAreaReelDetail>();
                        List<Entitys.SMT.WmsSplitArea> splitAreaList = new List<Entitys.SMT.WmsSplitArea>();
                        List<Entitys.SMT.WmsSplitIssue> splitIssueList = new List<Entitys.SMT.WmsSplitIssue>();
                        foreach (var item in ReelGroup)
                        {

                            if (item.ToList().Find(a => a.NeedQuantity != a.OrgQuantity) != null)
                            {
                                pickOrderAreaDetailList.FindAll(a => a.ReelId == item.FirstOrDefault().ReelId).ForEach(a => { a.Status = 2; a.IsNeedSplit = true; });
                                Entitys.SMT.WmsSplitAreaReel splitReel = new Entitys.SMT.WmsSplitAreaReel();
                                splitReel.AreaId = item.FirstOrDefault().AreaId;
                                splitReel.LocationCode = item.FirstOrDefault().LocationCode;
                                splitReel.OrgQuantity = item.FirstOrDefault().OrgQuantity;
                                splitReel.ReelId = item.FirstOrDefault().ReelId;
                                splitReel.SplitNo = splitMain.SplitNo;
                                splitReel.Status = 0;
                                splitReel.WareHouseCode = item.FirstOrDefault().WareHouseCode;
                                splitReel.MaterialCode = item.FirstOrDefault().MaterialCode;
                                splitReel.PickOrderCode = item.FirstOrDefault().PickOrderCode;
                                splitReel.BatchCode = item.FirstOrDefault().BatchCode;
                                splitReel.ReelCreateCode = item.FirstOrDefault().ReelCreateCode;
                                splitReelList.Add(splitReel);
                                for (int i = 0; i < item.Count(); i++)
                                {
                                    var splitItem = item.ToList()[i];
                                    Entitys.SMT.WmsSplitAreaReelDetail reelDetail = new Entitys.SMT.WmsSplitAreaReelDetail();
                                    reelDetail.AreaId = splitItem.AreaId;
                                    reelDetail.PickOrderCode = splitItem.PickOrderCode;
                                    reelDetail.PickDetailId = splitItem.PickOrderDetailId;
                                    reelDetail.OrgQuantity = splitItem.OrgQuantity;
                                    reelDetail.OrgReelId = splitItem.ReelId;
                                    reelDetail.SplitReelId = "";
                                    reelDetail.SplitNo = splitMain.SplitNo;
                                    reelDetail.SplitQuantity = splitItem.NeedQuantity;
                                    reelDetail.Status = 0;
                                    reelDetail.WareHouseCode = splitItem.WareHouseCode;
                                    reelDetail.MaterialCode = splitItem.MaterialCode;
                                    if (i == item.Count() - 1)
                                    {
                                        if (item.Sum(a => a.NeedQuantity) >= item.FirstOrDefault().OrgQuantity)
                                        {
                                            reelDetail.SplitReelId = item.FirstOrDefault().ReelId;
                                        }
                                    }
                                    splitReelDetailList.Add(reelDetail);


                                }
                                if (item.Sum(a => a.NeedQuantity) < item.FirstOrDefault().OrgQuantity)
                                {
                                    Entitys.SMT.WmsSplitAreaReelDetail reelDetail = new Entitys.SMT.WmsSplitAreaReelDetail();
                                    reelDetail.AreaId = item.FirstOrDefault().AreaId;
                                    reelDetail.PickOrderCode = "";
                                    reelDetail.PickDetailId = "";
                                    reelDetail.OrgQuantity = item.FirstOrDefault().OrgQuantity;
                                    reelDetail.OrgReelId = item.FirstOrDefault().ReelId;
                                    reelDetail.SplitReelId = item.FirstOrDefault().ReelId;
                                    reelDetail.SplitNo = splitMain.SplitNo;
                                    reelDetail.SplitQuantity = item.FirstOrDefault().OrgQuantity - item.Sum(a => a.NeedQuantity);
                                    reelDetail.Status = 0;
                                    reelDetail.WareHouseCode = item.FirstOrDefault().WareHouseCode;
                                    reelDetail.MaterialCode = item.FirstOrDefault().MaterialCode;
                                    splitReelDetailList.Add(reelDetail);
                                }
                            }
                            else
                            {
                                pickOrderAreaDetailList.FindAll(a => a.ReelId == item.FirstOrDefault().ReelId).ForEach(a => { a.IsNeedSplit = false; a.Status = 0; });
                            }
                        }


                        foreach (var item in pickOrderMainList)
                        {
                            //var onePickDetailList = pickDetailListCopy.FindAll(a => a.Issue_HId == item.Issue_HId);
                            //foreach (var pickDetail in onePickDetailList)
                            //{
                            //    var list = pickAreaDetailList.FindAll(a => a.Issue_HId == item.Issue_HId && a.Issue_LId == pickDetail.Issue_LId);
                            //}
                            // item.Status = 4;
                            var list = pickOrderAreaDetailList.FindAll(a => a.PickOrderCode == item.PickOrderCode);
                            if (list.Find(a => a.IsNeedSplit == true) != null)
                            {
                                //   item.Status = 2;
                                Bussiness.Entitys.SMT.WmsSplitIssue issue = new Entitys.SMT.WmsSplitIssue();
                                issue.SplitNo = splitMain.SplitNo;
                                issue.PickOrderCode = item.PickOrderCode;
                                issue.Issue_No = item.Issue_No;
                                issue.Wo_No = item.Wo_No;
                                item.IsNeedSplit = true;
                                splitIssueList.Add(issue);
                            }
                        }
                        var splitAreaGroup = splitReelList.GroupBy(a => a.AreaId);
                        foreach (var item in splitAreaGroup)
                        {
                            Entitys.SMT.WmsSplitArea splitArea = new Entitys.SMT.WmsSplitArea();
                            splitArea.AreaId = item.FirstOrDefault().AreaId;
                            splitArea.SplitNo = splitMain.SplitNo;
                            splitArea.Status = 0;
                            splitArea.WareHouseCode = item.FirstOrDefault().WareHouseCode;
                            // splitArea.ProofId = Guid.NewGuid().ToString();
                            splitAreaList.Add(splitArea);
                        }


                        //  using (this.UnitOfWork)
                        {
                            this.WmsSplitMainRepository.UnitOfWork.TransactionEnabled = true;
                            foreach (var pickOrderMain in pickOrderMainList)
                            {
                                pickOrderMain.Status = 1;
                            }


                            //1插入拆盘单
                            this.WmsSplitMainRepository.Insert(splitMain);
                            foreach (var item in splitAreaList)
                            {
                                WmsSplitAreaRepository.Insert(item);
                            }
                            foreach (var item in splitReelList)
                            {
                                WmsSplitAreaReelRepository.Insert(item);
                            }
                            foreach (var item in splitReelDetailList)
                            {
                                WmsSplitAreaReelDetailRepository.Insert(item);
                            }
                            foreach (var item in splitIssueList)
                            {
                                WmsSplitIssueRepository.Insert(item);
                            }
                            //生成拣货区域原则 有的话 直接插入 没有则新增拣货区域
                            var pickAreaGroup = pickOrderAreaDetailList.FindAll(a => a.IsNeedSplit == false).GroupBy(a => new { a.PickOrderCode, a.AreaId });
                            //只生成可以直接拣货 不需要拆盘的区域。。。
                            var ExistAreaList = this.WmsPickOrderAreaRepository.Query().Where(a => PickOrderCodeList.Contains(a.PickOrderCode)).ToList(); //this.WmsPickOrderAreaRepository.Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA WHERE PICKORDERCODE IN @CodeList", new { CodeList = PickOrderCodeList });
                            List<Entitys.SMT.WmsPickOrderArea> pickAreaList = new List<Entitys.SMT.WmsPickOrderArea>();
                            foreach (var item in pickAreaGroup)
                            {
                                if (ExistAreaList.Count > 0)
                                {
                                    if (ExistAreaList.Find(a => a.AreaId == item.FirstOrDefault().AreaId && a.PickOrderCode == item.FirstOrDefault().PickOrderCode) == null)
                                    {
                                        Entitys.SMT.WmsPickOrderArea pickArea = new Entitys.SMT.WmsPickOrderArea();
                                        pickArea.AreaId = item.FirstOrDefault().AreaId;
                                        pickArea.PickOrderCode = item.FirstOrDefault().PickOrderCode;
                                        //   pickArea.Issue_No = item.FirstOrDefault().Issue_No;
                                        pickArea.WareHouseCode = item.FirstOrDefault().WareHouseCode;
                                        pickArea.Status = 0;
                                        //   pickArea.ProofId = Guid.NewGuid().ToString();
                                        pickAreaList.Add(pickArea);
                                    }
                                }
                                else
                                {
                                    Entitys.SMT.WmsPickOrderArea pickArea = new Entitys.SMT.WmsPickOrderArea();
                                    pickArea.AreaId = item.FirstOrDefault().AreaId;
                                    pickArea.PickOrderCode = item.FirstOrDefault().PickOrderCode;
                                    //   pickArea.Issue_No = item.FirstOrDefault().Issue_No;
                                    pickArea.WareHouseCode = item.FirstOrDefault().WareHouseCode;
                                    pickArea.Status = 0;
                                    // pickArea.ProofId = Guid.NewGuid().ToString();
                                    pickAreaList.Add(pickArea);
                                }
                            }
                            //插入拣选任务
                            //   pickOrderServer.WmsPickOrderMainRepository.Update(pickOrderMain);
                            if (pickAreaList.Count > 0)
                            {
                                foreach (var item in pickAreaList)
                                {
                                    WmsPickOrderAreaRepository.Insert(item);
                                }
                            }
                            foreach (var item in pickOrderDetailList)
                            {
                                WmsPickOrderDetailRepository.Update(item);
                            }
                            foreach (var item in pickOrderAreaDetailList)
                            {
                                WmsPickOrderAreaDetailRepository.Insert(item);
                            }
                            foreach (var item in pickOrderMainList)
                            {
                                WmsPickOrderMainRepository.Update(item);
                            }
                            //   pickOrderServer.WmsPickOrderIssueRepository.Insert(wmsPickOrderIssueList);
                            //stockServer.StockRepository.Execute("UPDATE TB_WMS_REELID_STOCK SET ISLOCKED = 1 WHERE ID IN @IdList", new { IdList = stockIdList });
                            foreach (var item in stockIdList)
                            {
                                item.IsLocked = true;
                                // StockRepository.SqlQuery("UPDATE TB_WMS_REELID_STOCK SET ISLOCKED = 1 WHERE ID =" + item);
                                StockRepository.Update(a => new Bussiness.Entitys.Stock() { IsLocked = true, LockedQuantity = item.LockedQuantity }, p => p.Id == item.Id);
                            }
                            //stockServer.StockRepository.Update("");
                            //  UnitOfWork.Commit();
                            this.WmsSplitMainRepository.UnitOfWork.Commit();

                        }
                    }
                    else //不需要拆盘
                    {

                        //using (this.UnitOfWork)
                        {
                            this.WmsPickOrderMainRepository.UnitOfWork.TransactionEnabled = true;
                            // pickOrderMain.Status = 1;
                            foreach (var pickOrderMain in pickOrderMainList)
                            {
                                pickOrderMain.Status = 1;
                            }
                            ////2更改领料单状态;
                            //List<Entitys.SMT.WmsPickOrderArea> pickAreaList = new List<Entitys.SMT.WmsPickOrderArea>();
                            pickOrderAreaDetailList.ForEach(a => a.IsNeedSplit = false);
                            //生成拣货区域原则 有的话 直接插入 没有则新增拣货区域
                            var pickAreaGroup = pickOrderAreaDetailList.FindAll(a => a.IsNeedSplit == false).GroupBy(a => new { a.PickOrderCode, a.AreaId });
                            //只生成可以直接拣货 不需要拆盘的区域。。。
                            var ExistAreaList = this.WmsPickOrderAreaRepository.Query().Where(a => PickOrderCodeList.Contains(a.PickOrderCode)).ToList();/*.Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA WHERE PICKORDERCODE IN @CodeList", new { CodeList = PickOrderCodeList });*/
                            List<Entitys.SMT.WmsPickOrderArea> pickAreaList = new List<Entitys.SMT.WmsPickOrderArea>();
                            foreach (var item in pickAreaGroup)
                            {
                                if (ExistAreaList.Count > 0)
                                {
                                    if (ExistAreaList.Find(a => a.AreaId == item.FirstOrDefault().AreaId && a.PickOrderCode == item.FirstOrDefault().PickOrderCode) == null)
                                    {
                                        Entitys.SMT.WmsPickOrderArea pickArea = new Entitys.SMT.WmsPickOrderArea();
                                        pickArea.AreaId = item.FirstOrDefault().AreaId;
                                        pickArea.PickOrderCode = item.FirstOrDefault().PickOrderCode;
                                        //   pickArea.Issue_No = item.FirstOrDefault().Issue_No;
                                        pickArea.WareHouseCode = item.FirstOrDefault().WareHouseCode;
                                        pickArea.Status = 0;
                                        //   pickArea.ProofId = Guid.NewGuid().ToString();
                                        pickAreaList.Add(pickArea);
                                    }
                                }
                                else
                                {
                                    Entitys.SMT.WmsPickOrderArea pickArea = new Entitys.SMT.WmsPickOrderArea();
                                    pickArea.AreaId = item.FirstOrDefault().AreaId;
                                    pickArea.PickOrderCode = item.FirstOrDefault().PickOrderCode;
                                    //   pickArea.Issue_No = item.FirstOrDefault().Issue_No;
                                    pickArea.WareHouseCode = item.FirstOrDefault().WareHouseCode;
                                    pickArea.Status = 0;
                                    //      pickArea.ProofId = Guid.NewGuid().ToString();
                                    pickAreaList.Add(pickArea);
                                }
                            }

                            //插入拣选任务
                            // pickOrderServer.WmsPickOrderMainRepository.Update(pickOrderMain);
                            foreach (var item in pickOrderMainList)
                            {
                                WmsPickOrderMainRepository.Update(item);
                            }
                            if (pickAreaList.Count > 0)
                            {
                                foreach (var item in pickAreaList)
                                {
                                    WmsPickOrderAreaRepository.Insert(item);
                                }
                            }
                            foreach (var item in pickOrderDetailList)
                            {
                                WmsPickOrderDetailRepository.Update(item);
                            }
                            foreach (var item in pickOrderAreaDetailList)
                            {
                                WmsPickOrderAreaDetailRepository.Insert(item);
                            }
                            //   pickOrderServer.WmsPickOrderIssueRepository.Insert(wmsPickOrderIssueList);
                            //foreach (var item in stockIdList)
                            //{
                            //    stockServer.StockRepository.Execute("UPDATE TB_WMS_REELID_STOCK SET ISLOCKED = 1 WHERE ID =" + item);
                            //}
                            foreach (var item in stockIdList)
                            {
                                item.IsLocked = true;
                                // StockRepository.SqlQuery("UPDATE TB_WMS_REELID_STOCK SET ISLOCKED = 1 WHERE ID =" + item);
                                StockRepository.Update(a => new Bussiness.Entitys.Stock() { IsLocked = true, LockedQuantity = item.LockedQuantity }, p => p.Id == item.Id);
                            }
                            this.WmsPickOrderMainRepository.UnitOfWork.Commit();

                        }
                    }
                }
                else
                {
                    return DataProcess.Failure("物料已全部分配完毕");
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }

            return DataProcess.Success();
        }

        /// <summary>
        /// 获取可用库存
        /// </summary>
        /// <param name="MaterialCode"></param>
        /// <param name="WareHouseCode"></param>
        /// <returns></returns>
        public List<Entitys.StockVM> GetCurrentAvailableStock(string MaterialCode, string WareHouseCode)
        {
            //Entitys.SMT.WmsPickStation entity = new Entitys.SMT.WmsPickStation();
            //var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(entity);//序列化
            //Entitys.SMT.WmsPickStation after = Newtonsoft.Json.JsonConvert.DeserializeObject<Entitys.SMT.WmsPickStation>(jsonStr);//反序列化

            //1获取库存出库原则
            //  

            var codeList = DictionaryContract.Dictionaries.Where(a => a.TypeCode == "OutStockRule" && a.Enabled == true).OrderBy(a => a.Sort).ToList();

            string VirtualLocation = WareHouseCode + "00000000";
            //return StockVMRepository.SqlQuery("SELECT * FROM VIEW_WMS_STOCK WHERE MATERIALCODE='" + MaterialCode + "' AND WAREHOUSECODE='" + WareHouseCode + "' AND ISLOCKED = 0 AND LOCATIONCODE!='" + VirtualLocation + "' ORDER BY MATERIALCOUNT,CreatedTime").ToList();
            //var list = StockVMRepository.Query().Where(a => a.MaterialCode == MaterialCode && a.WareHouseCode == WareHouseCode && a.LocationCode != VirtualLocation).OrderBy(a => a.Quantity).ThenBy(a => a.CreatedTime).ToList();

            var query = StockVMRepository.Query().Where(a => a.MaterialCode == MaterialCode && a.WareHouseCode == WareHouseCode && a.LocationCode != VirtualLocation);

            //query.OrderBy(a => Bussiness.Common.Property.GetPropertyValue(a, "Age")).ThenByDesc(p => Bussiness.Common.Property.GetPropertyValue(p, "Age")).ToList();

            //var tt = (from p in query orderby Bussiness.Common.Property.GetPropertyValue(p, "Age") ascending, Bussiness.Common.Property.GetPropertyValue(p, "") descending select p).ToList();
            string prop = "";
            string order = "";
            foreach (var item in codeList)
            {
                if (item.Code == "FIFO")
                {
                    prop += item.Value + ",";
                    order += "asc" + ",";
                }
                if (item.Code == "LIFO")
                {
                    prop += item.Value + ",";
                    order += "desc" + ",";
                }
                if (item.Code == "TLFO")
                {
                    prop += item.Value + ",";
                    order += "asc" + ",";
                }
                if (item.Code == "TLLO")
                {
                    prop += item.Value + ",";
                    order += "DESC" + ",";
                }
            }
            if (!string.IsNullOrEmpty(prop))
            {
                prop = prop.Substring(0, prop.Length - 1);
                order = order.Substring(0, order.Length - 1);
                string[] fields = prop.Split(new[] { ',' }, true ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
                string[] orders = order.Split(new[] { ',' }, true ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
                StringBuilder sbOrderString = new StringBuilder();
                for (int i = 0; i < fields.Length; i++)
                {
                    sbOrderString.Append("{0} {1},".FormatWith(fields[i], orders[i]));
                }
                if (sbOrderString.Length > 0)
                {
                    sbOrderString = sbOrderString.Remove(sbOrderString.Length - 1, 1);
                }
                query = query.OrderBy(sbOrderString.ToString());
            }
            var list = query.ToList();
            return list;
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="PickOrderCode"></param>
        /// <returns></returns>
        public DataResult DoCancel(string PickOrderCode)
        {
            var pickOrderIssueList = this.WmsPickOrderIssueRepository.Query().Where(a => a.PickOrderCode == PickOrderCode).ToList();/* this.WmsPickOrderIssueRepository.SqlQuery("SELECT * FROM TB_WMS_PICK_ORDER_ISSUE WHERE PICKORDERCODE='" + PickOrderCode + "'");*/
            var mainEntity = this.WmsPickOrderMainRepository.Query().FirstOrDefault(a => a.PickOrderCode == PickOrderCode);//<Bussiness.Entitys.SMT.WmsPickOrderMain>("SELECT * FROM TB_WMS_PICK_ORDER_MAIN WHERE PICKORDERCODE='" + PickOrderCode + "'");
                                                                                                                           // Bussiness.Services.StockServer stockServer = new Services.StockServer(this.UnitOfWork);
            if (mainEntity.Status != 0)
            {
                return DataProcess.Failure("该拣货单任务已启动,此时不允许作废");
            }
            var areaDetailList = this.WmsPickOrderAreaDetailRepository.SqlQuery("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE PICKORDERCODE='" + PickOrderCode + "'");
            foreach (var item in areaDetailList)
            {
                if (item.Status > 0)
                {
                    return DataProcess.Failure("该拣货单正在拣选或有拆盘计划此时不允许作废");
                }
            }
            List<int> IssueHIdList = new List<int>();
            foreach (var item in pickOrderIssueList)
            {
                IssueHIdList.Add(item.Issue_HId.GetValueOrDefault(0));
            }
            // Bussiness.ShiYiTongServices.WmsPickServer pickServer = new WmsPickServer(this.UnitOfWork);
            // using (this.UnitOfWork)
            WmsPickOrderMainRepository.UnitOfWork.TransactionEnabled = true;
            {
                // this.UnitOfWork.TransactionEnabled = true;
                mainEntity.Status = 5;
                var pickMainList = WmsPickMainRepository.Query().Where(a => IssueHIdList.Contains(a.Id)).ToList();
                var pickDetaiList = WmsPickDetailRepository.Query().Where(a => IssueHIdList.Contains(a.Issue_HId)).ToList();
                //var pickOrderIssueList = WmsPickOrderIssueRepository.Query().Where(a => a.PickOrderCode == mainEntity.PickOrderCode).ToList();
                foreach (var item in pickOrderIssueList)
                {
                    item.Status = 5;
                    WmsPickOrderIssueRepository.Update(item);
                }
                this.WmsPickOrderMainRepository.Update(mainEntity);
                foreach (var item in pickMainList)
                {
                    item.Status = 0;
                    WmsPickMainRepository.Update(item);
                }
                foreach (var item in pickDetaiList)
                {
                    item.Status = 0;
                    WmsPickDetailRepository.Update(item);
                }
                //this.WmsPickOrderIssueRepository.Execute("UPDATE TB_WMS_PICK_ORDER_ISSUE SET STATUS = 5 WHERE PICKORDERCODE='" + mainEntity.PickOrderCode + "'");
                //pickServer.WmsPickMainRepository.Execute("UPDATE TB_WMS_PICK_MAIN SET STATUS = 0 WHERE ISSUE_HID IN @HIdList", new { HIdList = IssueHIdList });
                //pickServer.WmsPickDetailRepository.Execute("UPDATE TB_WMS_PICK_DETAIL SET STATUS = 0 WHERE ISSUE_HID IN @HIdList", new { HIdList = IssueHIdList });
                //this.WmsPickOrderMainRepository.Update(mainEntity);
                //foreach (var item in areaDetailList)
                //{
                //    stockServer.StockRepository.Execute("UPDATE TB_WMS_REELID_STOCK SET ISLOCKED=0 WHERE REELID='" + item.ReelId + "'");
                //}
                //this.UnitOfWork.Commit();
            }
            WmsPickOrderMainRepository.UnitOfWork.Commit();
            return DataProcess.Success();
        }

        /// <summary>
        /// 获取可以拣选的
        /// </summary>
        /// <param name="PickOrderCode"></param>
        /// <returns></returns>
        public List<Bussiness.Entitys.SMT.WmsPickOrderArea> GetAllAvailablePickArea(string PickOrderCode)
        {
            try
            {
                List<Bussiness.Entitys.SMT.WmsPickOrderArea> list = this.WmsPickOrderAreaRepository.Query().Where(a => a.PickOrderCode == PickOrderCode).ToList();
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        bool isExist = this.WmsPickOrderAreaDetailRepository.Query().FirstOrDefault(a => a.AreaId == item.AreaId && a.PickOrderCode == PickOrderCode && (a.Status == 1 || a.Status == 0) && a.IsNeedSplit == false) == null ? false : true;
                        //.GetEntity<int>("select COUNT(*) FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE AREAID='" + item.AreaId + "' AND PICKORDERCODE='" + PickOrderCode + "' AND STATUS in( 0,1) AND ISNEEDSPLIT = 0") > 0;
                        if (isExist)
                        {
                            // item.Status = 0;
                            //  newList.Add(item);
                            item.IsCanStart = true;
                        }
                        else
                        {
                            item.IsCanStart = false;
                        }
                    }

                }
                else
                {
                    return new List<WmsPickOrderArea>();
                }

                if (list.Count > 0)
                {
                    list = list.OrderBy(a => a.AreaId).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                return new List<WmsPickOrderArea>();
            }
            return new List<WmsPickOrderArea>();

        }
        /// <summary>
        /// 批量启动任务区域
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public DataResult PickTaskDoStart(List<Bussiness.Entitys.SMT.WmsPickOrderArea> list)
        {
            List<string> AreaCodeList = new List<string>();
            foreach (var item in list)
            {
                AreaCodeList.Add(item.AreaId);
            }


            try
            {
                var PickOrderCode = list.FirstOrDefault().PickOrderCode;
                this.WmsPickOrderMainRepository.UnitOfWork.TransactionEnabled = true;
                var pickOrderMain = this.WmsPickOrderMainRepository.Query().FirstOrDefault(a => a.PickOrderCode == PickOrderCode);
                list = this.WmsPickOrderAreaRepository.Query().Where(a => a.PickOrderCode == PickOrderCode && AreaCodeList.Contains(a.AreaId)).ToList();
                foreach (var item in list)
                {

                    //1 判断当前区域是否存在拣货或者补货任务
                    bool IsExistReplenishTask = true;
                    IsExistReplenishTask = StockLightContract.IsCurrentAreaShelfTasking(pickOrderMain.WareHouseCode, item.AreaId);
                    if (IsExistReplenishTask)
                    {
                        return DataProcess.Failure("区域" + item.AreaId + "尚有上架任务正在进行");
                    }
                    bool IsExistPickOrder = StockLightContract.IsCurrentAreaPickTasking(pickOrderMain.WareHouseCode, item.AreaId);
                    if (IsExistPickOrder)
                    {
                        return DataProcess.Failure("区域:" + item.AreaId + "尚有捡货任务正在进行");
                    }
                    bool IsExistSplitTask = StockLightContract.IsCurrentAreaSplitTasking(pickOrderMain.WareHouseCode, item.AreaId);
                    if (IsExistSplitTask)
                    {
                        return DataProcess.Failure("区域:" + item.AreaId + "尚有拆盘任务正在进行");
                    }

                    bool IsExistCheckTask = StockLightContract.IsCurrentAreaCheckTasking(pickOrderMain.WareHouseCode, item.AreaId);
                    if (IsExistCheckTask)
                    {
                        return DataProcess.Failure("区域:" + item.AreaId + "尚有盘点任务正在进行");
                    }
                    ////2通知PTL点亮该区域的拣货任务
                    //  Bussiness.PTLServices.PTLServer ptlServer = new Bussiness.PTLServices.PTLServer(UnitOfWork);
                    var pickAreaDetailList = this.WmsPickOrderAreaDetailRepository.Query().Where(a => a.PickOrderCode == item.PickOrderCode && a.AreaId == item.AreaId && a.IsNeedSplit == false && a.Status == 0).ToList();/*.Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE PICKORDERCODE='" + PickOrderCode + "' AND AREAID='" + AreaId + "' AND ISNEEDSPLIT = 0 AND STATUS =0");*/
                    if (pickAreaDetailList != null)
                    {
                        if (pickAreaDetailList.Count() == 0)
                        {
                            return DataProcess.Failure("区域:" + item.AreaId + "无可拣货的条码");
                        }
                    }
                    else
                    {
                        return DataProcess.Failure("区域:" + item.AreaId + "无可拣货的条码");
                    }
                    //var pickArea = this.WmsPickOrderAreaRepository.GetEntity<Bussiness.Entitys.SMT.WmsPickOrderArea>("SELECT * FROM TB_WMS_PICK_ORDER_AREA WHERE PICKORDERCODE='" + PickOrderCode + "' AND AREAID='" + AreaId + "'");
                    //if (pickArea.Status!=0)
                    //{
                    //    result.Message = "当前区域状态不对";
                    //    result.Success = false;
                    //    return RT.Utility.JsonHelper.SerializeObject(result);
                    //}
                    List<Bussiness.Entitys.PTL.DpsInterface> dpsDetailList = new List<Bussiness.Entitys.PTL.DpsInterface>();
                    Bussiness.Entitys.PTL.DpsInterfaceMain dpsMain = new Bussiness.Entitys.PTL.DpsInterfaceMain();
                    bool IsExistPTLOrder = false;
                    if (!string.IsNullOrEmpty(item.ProofId))//证明已发布过任务 此时追加任务
                    {
                        IsExistPTLOrder = true;
                    }
                    else
                    {
                        string PtlProofid = Guid.NewGuid().ToString();
                        item.ProofId = PtlProofid;
                    }
                    item.Status = 1;
                    pickOrderMain.Status = 2;
                    CreateDpsInterfaceForPick(ref dpsMain, ref dpsDetailList, pickAreaDetailList, item.ProofId);
                    Entitys.PTL.DpsInterfaceMain entityMain = new Entitys.PTL.DpsInterfaceMain();
                    if (IsExistPTLOrder == true)
                    {
                        entityMain = DpsInterfaceMainRepository.Query().FirstOrDefault(a => a.ProofId == item.ProofId);
                        if (entityMain != null && entityMain.Id != 0)
                        {
                            dpsMain = entityMain;
                            dpsMain.Status = 0;
                            dpsMain.OrderType = 0;
                        }
                    }
                    ////3更改拣货区域的任务状态
                    // using (this.UnitOfWork)
                    {
                        // UnitOfWork.TransactionEnabled = true;
                        //pickServer.WmsPickMainRepository.Execute("UPDATE TB_WMS_PICK_MAIN SET STATUS =5 WHERE  ISSUE_ID=" +  pickMain.Issue_HId);
                        pickAreaDetailList.ForEach(a => a.Status = 1);
                        //pickServer.WmsPickAreaRepository.Execute("UPDATE TB_WMS_PICK_AREA SET STATUS=5 WHERE  ISSUE_ID=" +  pickMain.Issue_HId+" AND AREAID='" + AreaId + "'");
                        //pickServer.WmsPickAreaDetailRepository.Execute("UPDATE TB_WMS_PICK_AREA_DETAIL SET STATUS=5 WHERE ISSUE_ID=" + pickMain.Issue_HId + " AND AREAID='" + AreaId + "'");
                        this.WmsPickOrderMainRepository.Update(pickOrderMain);
                        this.WmsPickOrderAreaRepository.Update(item);
                        foreach (var areaDetail in pickAreaDetailList)
                        {
                            this.WmsPickOrderAreaDetailRepository.Update(areaDetail);
                        }
                        if (IsExistPTLOrder == true && entityMain != null && entityMain.Id != 0)
                        {
                            DpsInterfaceMainRepository.Update(dpsMain);
                        }
                        else
                        {
                            DpsInterfaceMainRepository.Insert(dpsMain);
                        }
                        foreach (var dpsInterface in dpsDetailList)
                        {
                            this.DpsInterfaceRepository.Insert(dpsInterface);
                        }
                        // UnitOfWork.Commit();
                    }

                }
                this.WmsPickOrderMainRepository.UnitOfWork.Commit();
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                foreach (var item in list)
                {
                    var reuslt = ptlServer.StartOrder(item.AreaId, item.ProofId,false);
                }
                return DataProcess.Success("启动区域成功");
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 创建并发布PTL任务 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="detailList"></param>
        /// <param name="SplitAreaReelList"></param>
        /// <param name="splitMain"></param>
        /// <returns></returns>
        public bool CreateDpsInterfaceForPick(ref Bussiness.Entitys.PTL.DpsInterfaceMain main, ref List<Bussiness.Entitys.PTL.DpsInterface> detailList, List<Bussiness.Entitys.SMT.WmsPickOrderAreaDetail> PickAreaReelList, string ProofId)
        {
            main.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            main.OrderType = 0;
            main.ProofId = ProofId;
            main.Status = 0;
            main.OrderCode = PickAreaReelList.FirstOrDefault().PickOrderCode;
            main.WareHouseCode = PickAreaReelList.FirstOrDefault().WareHouseCode;
            main.AreaCode = PickAreaReelList.FirstOrDefault().AreaId;
            foreach (var item in PickAreaReelList)
            {
                Bussiness.Entitys.PTL.DpsInterface face = new Bussiness.Entitys.PTL.DpsInterface();
                face.BatchNO = item.BatchCode;
                face.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                face.GoodsName = item.MaterialCode;
                face.LocationId = item.LocationCode;
                face.MakerName = "";
                face.ProofId = ProofId;
                face.Quantity = item.OrgQuantity;
                face.Spec = "";
                face.Status = 0;
                face.UserId = "";
                face.ToteId = "";
                face.OperateId = Guid.NewGuid().ToString();
                face.RelationId = item.Id;
                face.MaterialLabel = item.ReelId;
                detailList.Add(face);
            }
            return true;
        }
        /// <summary>
        /// 批量结束任务区域
        /// </summary>
        /// <param name="PickOrderCode"></param>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        public DataResult PickTaskDoFinish(List<Bussiness.Entitys.SMT.WmsPickOrderArea> list)
        {
            List<string> AreaCodeList = new List<string>();
            foreach (var item in list)
            {
                AreaCodeList.Add(item.AreaId);
            }

            
            try
            {
                var PickOrderCode = list.FirstOrDefault().PickOrderCode;
                WmsPickOrderAreaRepository.UnitOfWork.TransactionEnabled = true;
                list = this.WmsPickOrderAreaRepository.Query().Where(a => a.PickOrderCode == PickOrderCode && AreaCodeList.Contains(a.AreaId)).ToList();
                foreach (var pickOrderArea in list)
                {
                    if (pickOrderArea.Status == 0)
                    {
                        return DataProcess.Failure("该捡货任务尚未启动，请先启动后再执行熄灭操作");
                    }
                    var CurrentArea = this.WmsPickOrderAreaRepository.Query().FirstOrDefault(a => a.AreaId == pickOrderArea.AreaId && a.PickOrderCode == pickOrderArea.PickOrderCode);
                    if (CurrentArea.Status == 2)
                    {
                        pickOrderArea.Status = 2;
                        pickOrderArea.IsNeedOffLight = false;
                        continue;
                    }
                    else
                    {
                        pickOrderArea.IsNeedOffLight = true;
                    }
                    //1通知PTL灭灯操作
                    bool IsPTLExecuteSuccess = false;
                    //2 更改区域下架完成状态
                    //3更新库存操作 将已下架的库存置为虚拟库位
                    var labelList = this.WmsPickOrderAreaDetailRepository.Query().Where(a => a.AreaId == pickOrderArea.AreaId && a.Status == 1 && a.PickOrderCode == pickOrderArea.PickOrderCode).ToList();//.Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL  WHERE STATUS=1 AND PICKORDERCODE='" + PickOrderCode + "' AND AREAID='" + AreaId + "'");
                    //bool IsExistSplitReel = this.WmsPickOrderAreaDetailRepository.Query().FirstOrDefault("SELECT COUNT(*) FROM TB_WMS_PICK_ORDER_AREA_DETAIL  WHERE PICKORDERCODE='" + PickOrderCode + "' AND ISNEEDSPLIT = 1") > 0;
                    List<string> ReelIdList = new List<string>();
                    foreach (var item in labelList)
                    {
                        ReelIdList.Add(item.ReelId);
                    }

                    var stockList = this.StockVMRepository.Query().Where(a => ReelIdList.Contains(a.MaterialLabel)).ToList();


                    var pickIssueList = this.WmsPickOrderIssueRepository.Query().Where(a => a.PickOrderCode == pickOrderArea.PickOrderCode).ToList();//Query("SELECT * FROM TB_WMS_PICK_ORDER_ISSUE WHERE PICKORDERCODE='" + PickOrderCode + "'");
                    List<int> IssueHIdList = new List<int>();
                    foreach (var item in pickIssueList)
                    {
                        IssueHIdList.Add(item.Issue_HId.GetValueOrDefault(0));
                    }
                    //  using (this.UnitOfWork)
                    {
                        //   UnitOfWork.TransactionEnabled = true;
                        //pickServer.WmsPickAreaRepository.Execute("UPDATE TB_WMS_SPLIT_AREA SET STATUS=6 WHERE STATUS=5 AND ISSUE_HID=" + Issue_HId + " AND AREAID='" + AreaId + "'");
                        CurrentArea.Status = 2;
                        this.WmsPickOrderAreaRepository.Update(CurrentArea);
                        if (ReelIdList.Count > 0)
                        {
                            string VirtualLocation = labelList.FirstOrDefault().WareHouseCode + "00000000";
                            labelList.ForEach(A =>
                            {
                                A.Status = 3;
                                A.RealPickedQuantity = A.NeedQuantity.GetValueOrDefault(0);
                            });

                            //stockServer.StockRepository.Execute("UPDATE TB_WMS_REELID_STOCK SET LOCATIONCODE ='" + VirtualLocation + "' WHERE REELID IN @ReelIdList", new { ReelIdList = ReelIdList });
                            foreach (var item in stockList)
                            {
                                if (item.IsElectronicMateria)
                                {
                                    item.LocationCode = VirtualLocation;
                                    StockRepository.Update(a => new Bussiness.Entitys.Stock() { LocationCode = VirtualLocation }, p => p.Id == item.Id);
                                }
                            }
                            foreach (var item in labelList)
                            {
                                this.WmsPickOrderAreaDetailRepository.Update(item);
                            }
                        }
                        //if (AreaCount - FinishedAreaCount == 1)
                        //{
                        //    pickServer.WmsPickMainRepository.Execute("UPDATE TB_WMS_PICK_MAIN SET STATUS =6 WHERE STATUS=5 AND ISSUE_HID=" + Issue_HId); 
                        //    pickServer.WmsPickDetailRepository.Execute("UPDATE TB_WMS_PICK_DETAIL SET STATUS =6 WHERE   ISSUE_HID=" + Issue_HId);
                        //    pickServer.WmsPickStationRepository.Execute("UPDATE TB_WMS_PICK_STATION SET STATUS =6 WHERE   ISSUE_HID=" + Issue_HId);
                        //}

                        //if (!IsExistSplitReel)
                        //{
                        //    var FinishedReel = pickOrderServer.WmsPickOrderAreaDetailRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE STATUS = 2 AND PICKORDERCODE='" + PickOrderCode + "'");
                        //    var AllReelCount = pickOrderServer.WmsPickOrderAreaDetailRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE PICKORDERCODE='" + PickOrderCode + "'");
                        //    if (AllReelCount - FinishedReel  ==ReelList.Count())
                        //    {
                        //        // 1更新所有拣货单操作
                        //        pickServer.WmsPickMainRepository.Execute("UPDATE TB_WMS_PICK_MAIN SET STATUS = 2 WHERE ISSUE_HID IN @HIdList", new { HIdList = IssueHIdList });
                        //        pickServer.WmsPickDetailRepository.Execute("UPDATE TB_WMS_PICK_DETAIL SET STATUS = 2 WHERE ISSUE_HID IN @HIdList", new { HIdList = IssueHIdList });
                        //        pickServer.WmsPickStationRepository.Execute("UPDATE TB_WMS_PICK_STATION SET STATUS = 2 WHERE ISSUE_HID IN @HIdList", new { HIdList = IssueHIdList });
                        //        //2库存释放

                        //        pickOrderServer.WmsPickOrderMainRepository.Execute("UPDATE TB_WMS_PICK_ORDER_MAIN SET STATUS = 2 WHERE PICKORDERCODE='" + PickOrderCode + "'");
                        //        pickOrderServer.WmsPickOrderIssueRepository.Execute("UPDATE TB_WMS_PICK_ORDER_ISSUE SET STATUS = 2 WHERE PICKORDERCODE='" + PickOrderCode + "'");
                        //        pickOrderServer.WmsPickOrderAreaRepository.Execute("UPDATE TB_WMS_PICK_ORDER_AREA SET STATUS = 2 WHERE PICKORDERCODE='" + PickOrderCode + "'");
                        //        //pickOrderServer.WmsPickOrderDetailRepository.Execute("UPDATE TB_WMS_PICK_ORDER_DETAIL SET STATUS = 2 WHERE PICKORDERCODE='" + PickOrderCode + "'");
                        //        //pickOrderServer.WmsPickOrderAreaDetailRepository.Execute("UPDATE TB_WMS_PICK_ORDER_AREA_DETAIL WHERE STATUS = 3 WHERE PICKORDERCODE='" + PickOrderCode + "'");
                        //    }
                        //}
                        //UnitOfWork.Commit();
                    }


                }
                WmsPickOrderAreaRepository.UnitOfWork.Commit();
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                foreach (var item in list)
                {
                    if (item.IsNeedOffLight == false)
                    {
                        continue;
                    }
                    //灭队
                    //WmsToPTLServer.ForWmsService forPtlServer = new WmsToPTLServer.ForWmsService();
                    //forPtlServer.Finish(item.WareHouseCode+item.AreaId, item.ProofId);
                    var result = ptlServer.FinishOrder(item.AreaId, item.ProofId,false);
                    //if (!result.Success)
                    //{
                    //    return DataProcess.Failure(result.Message);
                    //}


                }

                return DataProcess.Success("操作成功");
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
            return DataProcess.Success();
        }


        /// <summary>
        /// 确认此ReelId发料
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="Issue_HId"></param>
        /// <returns></returns>
        public DataResult ConfirmReelToSend(string ReelId, string PickOrderCode)
        {
            try
            {
                var entity = this.WmsPickOrderAreaDetailRepository.Query().FirstOrDefault(a => a.PickOrderCode == PickOrderCode && a.ReelId == ReelId);//.GetEntity<Business.ShiYiTongEntitys.WmsPickOrderAreaDetail>("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE REELID='" + ReelId + "'  AND PICKORDERCODE='" + PickOrderCode + "'");

                var pickOrderMain = this.WmsPickOrderMainRepository.Query().FirstOrDefault(a => a.PickOrderCode == PickOrderCode);
                if (entity != null)
                {
                    var pickOrderDetail = this.WmsPickOrderDetailRepository.Query().FirstOrDefault(a => a.PickOrderCode == PickOrderCode && a.DetailId == entity.PickOrderDetailId);//.GetEntity("SELECT * FROM TB_WMS_PICK_ORDER_DETAIL WHERE PICKORDERCODE='" + PickOrderCode + "' AND DETAILID='" + entity.PickOrderDetailId + "'");
                    if (entity.Status == 5)
                    {
                        return DataProcess.Failure("此条码已作废,无法重新确认");
                    }
                    else if (entity.Status < 3)
                    {
                        return DataProcess.Failure("此条码尚未拣选");
                    }
                    else
                    {
                        if (entity.Status == 3)
                        {
                            entity.Status = 4;
                            // pickOrderDetail.ConfirmQuantity = pickOrderDetail.ConfirmQuantity + entity.NeedQuantity;
                            pickOrderDetail.ConfirmQuantity = pickOrderDetail.ConfirmQuantity.GetValueOrDefault(0) + entity.RealPickedQuantity;
                            entity.ConfirmDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                            if (pickOrderDetail.ConfirmQuantity.GetValueOrDefault(0) >= pickOrderDetail.CurQuantity)
                            {
                                pickOrderDetail.Status = 4;
                            }
                            if (entity.NeedQuantity != entity.RealPickedQuantity)//拣货数量与实际分配数量不匹配
                            {
                                pickOrderDetail.CurQuantity = pickOrderDetail.CurQuantity + entity.RealPickedQuantity - entity.NeedQuantity;
                                if (pickOrderDetail.CurQuantity >= pickOrderDetail.Quantity)
                                {
                                    // pickOrderDetail.CurQuantity = ReadyQauntiy;
                                    pickOrderDetail.Status = 3;
                                }
                                else if (pickOrderDetail.CurQuantity > 0 && pickOrderDetail.CurQuantity < pickOrderDetail.Quantity)
                                {
                                    // item.CurQuantity = ReadyQauntiy;
                                    pickOrderDetail.Status = 2;
                                }
                                else
                                {
                                    pickOrderDetail.Status = 1;

                                }
                            }
                            //  using (pickServer.UnitOfWork)
                            int finishedCount = this.WmsPickOrderAreaDetailRepository.GetCount(a => a.PickOrderCode == PickOrderCode && a.Status >= 4);
                            int totalCount = this.WmsPickOrderAreaDetailRepository.GetCount(a => a.PickOrderCode == PickOrderCode);
                            {
                                this.WmsPickOrderAreaDetailRepository.UnitOfWork.TransactionEnabled = true;
                                this.WmsPickOrderDetailRepository.Update(pickOrderDetail);
                                this.WmsPickOrderAreaDetailRepository.Update(entity);
                                //删除库存
                                // StockRepository.Delete(a => a.MaterialLabel == entity.ReelId);
                                var stock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == entity.ReelId);
                                if (stock != null) // 理論 電子料和盤料分開處理 
                                {
                                    stock.Quantity = stock.Quantity - entity.RealPickedQuantity;//entity.NeedQuantity.GetValueOrDefault(0);
                                    stock.LockedQuantity = stock.LockedQuantity - entity.NeedQuantity.GetValueOrDefault(0);//entity.NeedQuantity.GetValueOrDefault(0);

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

                                bool NotReady = this.WmsPickOrderDetailRepository.GetCount(a => (a.Status == 0 || a.Status == 1 || a.Status == 2) && a.PickOrderCode == PickOrderCode) > 0;
                                if (totalCount - finishedCount == 1 && NotReady == false)
                                {
                                    this.WmsPickOrderMainRepository.Update(a => new Bussiness.Entitys.SMT.WmsPickOrderMain() { Status = 3 }, p => p.PickOrderCode == PickOrderCode);

                                    var IssueList = this.WmsPickOrderIssueRepository.Query().Where(a => a.PickOrderCode == PickOrderCode).ToList();
                                    foreach (var item in IssueList)
                                    {
                                        item.Status = (int)Bussiness.Enums.SMT.PickOrderStatusEnum.Ready;
                                        this.WmsPickOrderIssueRepository.Update(item);
                                        if (item.OrderType==0)
                                        {
                                            this.WmsPickMainRepository.Update(a => new Bussiness.Entitys.SMT.WmsPickMain()
                                            {
                                                Status = (int)Bussiness.Enums.SMT.PickStatusEnum.Finished
                                            }, p => p.Id == item.Issue_HId);

                                            this.WmsPickDetailRepository.Update(a => new Bussiness.Entitys.SMT.WmsPickDetail()
                                            {
                                                Status = (int)Bussiness.Enums.SMT.PickStatusEnum.Finished
                                            }, p => p.Issue_HId == item.Issue_HId);
                                        }
                                        else
                                        {
                                            this.WmsPickMainRepository.Update(a => new Bussiness.Entitys.SMT.WmsPickMain()
                                            {
                                                Status = (int)Bussiness.Enums.SMT.PickStatusEnum.WaitingForShelf
                                            }, p => p.Id == item.Issue_HId);

                                            this.WmsPickDetailRepository.Update(a => new Bussiness.Entitys.SMT.WmsPickDetail()
                                            {
                                                Status = (int)Bussiness.Enums.SMT.PickStatusEnum.WaitingForShelf
                                            }, p => p.Issue_HId == item.Issue_HId);

                                        }
                                    }
                                    #region
                                    if (pickOrderMain.OrderType==1)//调拨单 此时生成出库单
                                    {
                                        Bussiness.Entitys.In inEntity = new Entitys.In();
                                        inEntity.BillCode = "";
                                        inEntity.InDate = DateTime.Now.ToString("yyyy-MM-dd");
                                        inEntity.WareHouseCode = pickOrderMain.InWareHouseCode;
                                        inEntity.InDict = IssueList.First().IssueType;
                                        inEntity.OrderType = pickOrderMain.OrderType;
                                        inEntity.PickOrderCode = pickOrderMain.PickOrderCode;
                                        inEntity.Remark = "";
                                        inEntity.Status = (int)Bussiness.Enums.InStatusCaption.WaitingForShelf;
                                        inEntity.AddMaterial = new List<Entitys.InMaterial>();
                                        var pickedOrderDetailList = this.WmsPickOrderAreaDetailRepository.Query().Where(a => a.PickOrderCode == PickOrderCode && a.Status == (int)Bussiness.Enums.SMT.PickOrderAreaDetailStatusEnum.Finished).ToList();
                                        foreach (var item in pickedOrderDetailList)
                                        {
                                            Entitys.InMaterial inMaterial = new Entitys.InMaterial();
                                            inMaterial.BatchCode = item.BatchCode;
                                            inMaterial.BillCode = inEntity.BillCode;
                                            inMaterial.CustomCode = "";
                                            inMaterial.ItemNo = "";
                                            inMaterial.LocationCode = "";
                                            inMaterial.ManufactrueDate =DateTime.Parse(item.ReelCreateCode) ;
                                            inMaterial.MaterialCode = item.MaterialCode;
                                            inMaterial.MaterialLabel = item.ReelId;
                                            inMaterial.Quantity = item.RealPickedQuantity;
                                            inMaterial.Status = (int)Bussiness.Enums.InStatusCaption.WaitingForShelf;
                                            inMaterial.SupplierCode = item.SupplierCode;
                                            inEntity.AddMaterial.Add(inMaterial);
                                        }
                                        var inEntityResult = this.InContract.CreateInEntity(inEntity);
                                        if (!inEntityResult.Success)
                                        {
                                            return DataProcess.Failure(inEntityResult.Message);
                                        }
                                    }
                                    #endregion

                                }
                                WmsPickOrderAreaDetailRepository.UnitOfWork.Commit();
                            }
                        }
                        return DataProcess.Success("复核成功");
                    }
                }
                else
                {
                    return DataProcess.Failure("此条码不在此拣货单上");
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
            return DataProcess.Success();
        }


        /// <summary>
        /// 检查此REELID是否在此拣货任务中
        /// </summary>
        /// <returns></returns>
        public DataResult IsTheReelIdInPickOrder(string ReelId, string PickOrderCode)
        {
            try
            {
                var entity = this.WmsPickOrderAreaDetailRepository.Query().FirstOrDefault(a => a.PickOrderCode == PickOrderCode && a.ReelId == ReelId);
                if (entity != null)
                {
                    //if (entity.Status!=3)
                    //{
                    // result.Success = false;
                    // result.Message = "此ReelId已确认";
                    // return RT.Utility.JsonHelper.SerializeObject(result);
                    //}
                    return DataProcess.Success("获取成功", entity);
                }
                else
                {
                    return DataProcess.Failure("该条码不存在此拣货单上");
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message); ;
            }
        }

        /// <summary>
        /// 根据库位获取拣货任务
        /// </summary>
        /// <returns></returns>
        public DataResult GetReelIdByLocationCodeForPick(string PickOrderCode, string ReelId, string LocationCode)
        {
            try
            {
                var query = this.WmsPickOrderAreaDetailRepository.Query().Where(a => a.PickOrderCode == PickOrderCode);
                if (string.IsNullOrEmpty(ReelId) && string.IsNullOrEmpty(LocationCode))
                {
                    return DataProcess.Failure("物料条码或者库位码未输入");
                }
                if (!string.IsNullOrEmpty(ReelId))
                {
                    query = query.Where(a => a.ReelId == ReelId);
                }
                if (!string.IsNullOrEmpty(LocationCode))
                {
                    query = query.Where(a => a.LocationCode == LocationCode);
                }

                var reel = query.FirstOrDefault();
                if (reel == null)
                {
                    return DataProcess.Failure("未找到该拣货单单条码信息");
                }
                return DataProcess.Success("获取成功", reel);
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
        }
        #endregion

        #region 拆盘管理
        /// <summary>
        /// 作废拆盘单
        /// </summary>
        /// <param name="SplirtNo"></param>
        /// <returns></returns>
        public DataResult CancelSplitOrder(string SplitNo)
        {
            var mainEnitty = this.WmsSplitMainRepository.Query().FirstOrDefault(a => a.SplitNo == SplitNo);
            if (mainEnitty == null)
            {
                return DataProcess.Failure("未找到该拆盘单");
            }
            if (mainEnitty.Status != 0)
            {
                return DataProcess.Failure("拆盘任务已开始,此时不能作废");
            }
            //Bussiness.ShiYiTongServices.WmsPickOrderServer pickOrderServer = new WmsPickOrderServer(this.UnitOfWork);
            //Bussiness.Services.StockServer stockServer = new Services.StockServer(this.UnitOfWork);
            List<Bussiness.Entitys.SMT.WmsSplitIssue> orderIssueList = this.WmsSplitIssueRepository.Query().Where(a => a.SplitNo == SplitNo).ToList();
            List<string> PickOrderCodeList = new List<string>();
            List<string> PickDetailIdList = new List<string>();
            List<string> splitReelLabelList = new List<string>();
            foreach (var item in orderIssueList)
            {
                PickOrderCodeList.Add(item.PickOrderCode);
            }
            List<Bussiness.Entitys.SMT.WmsSplitAreaReel> splitReelList = this.WmsSplitAreaReelRepository.Query().Where(a => a.SplitNo == SplitNo).ToList();
            foreach (var item in splitReelList)
            {
                splitReelLabelList.Add(item.ReelId);
            }
            var PickOrderAreaDetailList = this.WmsPickOrderAreaDetailRepository.Query().Where(a => PickOrderCodeList.Contains(a.PickOrderCode) && splitReelLabelList.Contains(a.ReelId)).ToList();/*("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE REELID IN(SELECT REELID FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + mainEnitty.SplitNo + "') AND PICKORDERCODE IN @CodeList", new { CodeList = PickOrderCodeList });*/

            foreach (var item in PickOrderAreaDetailList)
            {
                PickDetailIdList.Add(item.PickOrderDetailId);
            }
            var PickOrderDetailList = this.WmsPickOrderDetailRepository.Query().Where(a => PickDetailIdList.Contains(a.DetailId) && PickOrderCodeList.Contains(a.PickOrderCode)).ToList();//("SELECT * FROM TB_WMS_PICK_ORDER_DETAIL WHERE DETAILID IN @DetailIdList AND PICKORDERCODE IN @CodeList", new { DetailIdList = PickDetailIdList, CodeList = PickOrderCodeList });
            foreach (var item in PickOrderDetailList)
            {
                var list = PickOrderAreaDetailList.FindAll(a => a.PickOrderDetailId == item.DetailId);
                if (list.Count > 0)
                {
                    var ReadyQauntiy = 0;
                    ReadyQauntiy = item.CurQuantity.GetValueOrDefault(0) - list.Sum(a => a.NeedQuantity.GetValueOrDefault(0));
                    if (ReadyQauntiy >= item.Quantity)
                    {
                        item.CurQuantity = ReadyQauntiy;
                        item.Status = 3;
                    }
                    else if (ReadyQauntiy > 0 && ReadyQauntiy < item.Quantity)
                    {
                        item.CurQuantity = ReadyQauntiy;
                        item.Status = 2;
                    }
                    else
                    {
                        item.CurQuantity = 0;
                        item.Status = 1;
                    }

                }
            }

            //  using (this.UnitOfWork)
            this.WmsSplitMainRepository.UnitOfWork.TransactionEnabled = true;
            {
                //1更新拆盘单状态
                //   this.UnitOfWork.TransactionEnabled = true;
                mainEnitty.Status = 5;
                //SplitAreaRepository.Execute("UPDATE TB_WMS_SPLIT_AREA SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");
                WmsSplitAreaRepository.Update(p => new WmsSplitArea() { Status = 5 }, a => a.SplitNo == SplitNo);
                //   SplitIssueRepository.Execute("UPDATE TB_WMS_SPLIT_ISSUE SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");
                WmsSplitAreaReelRepository.Update(p => new WmsSplitAreaReel() { Status = 5 }, a => a.SplitNo == SplitNo);//Execute("UPDATE TB_WMS_SPLIT_AREA_REEL SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");
                WmsSplitAreaReelDetailRepository.Update(p => new WmsSplitAreaReelDetail() { Status = 5 }, a => a.SplitNo == SplitNo);//Execute("UPDATE TB_WMS_SPLIT_AREA_REEL_DETAIL SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");

                //stockServer.StockRepository.Execute("UPDATE TB_WMS_REELID_STOCK SET ISLOCKED = 0 WHERE REELID IN(SELECT REELID FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + mainEnitty.SplitNo + "')");
                StockRepository.Update(a => new Bussiness.Entitys.Stock() { IsLocked = false }, p => splitReelLabelList.Contains(p.MaterialLabel));
                foreach (var item in PickOrderDetailList)
                {
                    this.WmsPickOrderDetailRepository.Update(item);
                }
                foreach (var item in PickOrderAreaDetailList)
                {
                    this.WmsPickOrderAreaDetailRepository.Delete(item);
                }
                WmsSplitMainRepository.Update(mainEnitty);
                WmsSplitMainRepository.UnitOfWork.Commit();
                //this.UnitOfWork.Commit();
            }
            return DataProcess.Success();
        }


        /// <summary>
        /// 拆分单区域任务启动
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public DataResult SplitTaskDoStart(List<Bussiness.Entitys.SMT.WmsSplitArea> list)
        {
            try
            {
                if (list == null || list.Count() == 0)
                {
                    return DataProcess.Failure("没有可启动的区域");
                }
                List<string> AreaCodeList = new List<string>();
                foreach (var item in list)
                {
                    AreaCodeList.Add(item.AreaId);
                }

                var SplitNo = list.FirstOrDefault().SplitNo;
                list = this.WmsSplitAreaRepository.Query().Where(a => a.SplitNo == SplitNo && AreaCodeList.Contains(a.AreaId)).ToList();
                var splitMain = this.WmsSplitMainRepository.Query().FirstOrDefault(a => a.SplitNo == SplitNo);
                this.WmsSplitMainRepository.UnitOfWork.TransactionEnabled = true;
                //1 判断当前区域是否存在拣货或者补货任务
                foreach (var wmsSplitArea in list)
                {
                    bool IsExistReplenishTask = false;
                    IsExistReplenishTask = this.StockLightContract.IsCurrentAreaShelfTasking(wmsSplitArea.WareHouseCode, wmsSplitArea.AreaId);
                    if (wmsSplitArea.Status != 0)
                    {
                        return DataProcess.Failure("区域:" + wmsSplitArea.AreaId + "状态不对");
                    }
                    if (IsExistReplenishTask)
                    {
                        return DataProcess.Failure("区域:" + wmsSplitArea.AreaId + "存在上架任务");
                    }
                    bool IsExistPickOrder = true;

                    IsExistPickOrder = StockLightContract.IsCurrentAreaPickTasking(wmsSplitArea.WareHouseCode, wmsSplitArea.AreaId);
                    if (IsExistPickOrder)
                    {
                        return DataProcess.Failure("区域:" + wmsSplitArea.AreaId + "存在拣货任务");
                    }
                    bool IsExistSplitOrder = true; //当前区域是否存在拆分任务
                    IsExistSplitOrder = this.StockLightContract.IsCurrentAreaSplitTasking(wmsSplitArea.WareHouseCode, wmsSplitArea.AreaId);
                    if (IsExistSplitOrder)
                    {
                        return DataProcess.Failure("区域:" + wmsSplitArea.AreaId + "存在拆盘任务");
                    }

                    bool IsExistCheckOrder = true; //当前区域是否存在拆分任务
                    IsExistSplitOrder = this.StockLightContract.IsCurrentAreaSplitTasking(wmsSplitArea.WareHouseCode, wmsSplitArea.AreaId);
                    if (IsExistSplitOrder)
                    {
                        return DataProcess.Failure("区域:" + wmsSplitArea.AreaId + "存在拆盘任务");
                    }
                    //2通知PTL点亮该区域的拆分任务

                    var splitAreaReelList = this.WmsSplitAreaReelRepository.Query().Where(a => a.SplitNo == wmsSplitArea.SplitNo && a.AreaId == wmsSplitArea.AreaId).ToList();//splitServer.SplitAreaReelRepository.Query("SELECT * FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + SplitNo + "' AND AREAID='" + AreaId + "'");
                    var splitArea = this.WmsSplitAreaRepository.GetEntity(wmsSplitArea.Id);
                    if (splitArea.Status != 0)
                    {
                        return DataProcess.Failure("区域状态不对,请勿重复启动");
                    }
                    //插入PTL中间表数据
                    List<Bussiness.Entitys.PTL.DpsInterface> dpsDetailList = new List<Bussiness.Entitys.PTL.DpsInterface>();
                    Bussiness.Entitys.PTL.DpsInterfaceMain dpsMain = new Bussiness.Entitys.PTL.DpsInterfaceMain();

                    bool IsExistPTLOrder = false;
                    if (!string.IsNullOrEmpty(splitArea.ProofId))//证明已发布过任务 此时追加任务
                    {
                        IsExistPTLOrder = true;
                    }
                    else
                    {
                        string PtlProofid = Guid.NewGuid().ToString();
                        splitArea.ProofId = PtlProofid;
                        wmsSplitArea.ProofId = PtlProofid;
                    }
                    CreateDpsInterfaceForSplit(ref dpsMain, ref dpsDetailList, splitAreaReelList, splitArea.ProofId);
                    if (IsExistPTLOrder == true)
                    {
                        dpsMain = DpsInterfaceMainRepository.Query().FirstOrDefault(a => a.ProofId == splitArea.ProofId);
                        dpsMain.Status = 0;
                        dpsMain.OrderType = 1;
                    }



                    //wmsToPtlServer.Finish(
                    // wmsToPtlServer.Finish(shelfMain.AreaId, LastProofId);
                    //  wmsToPtlServer.Start(AreaId, splitMain.ProofId);
                    //3更改次拆盘区域的任务状态

                    //  using (this.UnitOfWork)
                    {
                        //     UnitOfWork.TransactionEnabled = true;
                        //splitServer.SplitMainRepository.Execute("UPDATE TB_WMS_SPLIT_MAIN SET STATUS =1 WHERE STATUS=0 AND SPLITNO='" + SplitNo + "'");

                        //splitServer.SplitAreaRepository.Execute("UPDATE TB_WMS_SPLIT_AREA SET STATUS=1 ,PROOFID='" + PtlProofid + "' WHERE STATUS=0 AND SPLITNO='" + SplitNo + "' AND AREAID='" + AreaId + "'");
                        wmsSplitArea.Status = 1;
                        WmsSplitAreaRepository.Update(wmsSplitArea);
                        this.WmsSplitAreaReelRepository.Update(a => new Bussiness.Entitys.SMT.WmsSplitAreaReel() { Status = 1 }, p => p.Status == 0 && p.SplitNo == wmsSplitArea.SplitNo && p.AreaId == wmsSplitArea.AreaId);
                        //splitServer.SplitAreaRepository.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL SET STATUS=1 WHERE STATUS=0 AND SPLITNO='" + SplitNo + "' AND AREAID='" + AreaId + "'");
                        if (IsExistPTLOrder)
                        {
                            DpsInterfaceMainRepository.Update(dpsMain);
                        }
                        else
                        {
                            DpsInterfaceMainRepository.Insert(dpsMain);
                        }
                        foreach (var item in dpsDetailList)
                        {
                            DpsInterfaceRepository.Insert(item);
                        }
                        //   UnitOfWork.Commit();
                    }

                }
                if (splitMain.Status == 0)
                {
                    splitMain.Status = 1;
                    WmsSplitMainRepository.Update(splitMain);
                }
                WmsSplitMainRepository.UnitOfWork.Commit();
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                foreach (var item in list)
                {
                    //WmsToPTLServer.ForWmsService wmsToPtlServer = new WmsToPTLServer.ForWmsService();
                    //wmsToPtlServer.Start(item.WareHouseCode+item.AreaId, item.ProofId);

                    var result = ptlServer.StartOrder(item.AreaId, item.ProofId,false);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }


                return DataProcess.Success("操作成功");

            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
        }
        /// <summary>
        /// 拆分单区域任务熄灭
        /// </summary>
        /// <param name="SplitNo"></param>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        public DataResult SplitTaskDoFinish(List<Bussiness.Entitys.SMT.WmsSplitArea> list)
        {
            if (list == null || list.Count() == 0)
            {
                return DataProcess.Failure("未选择任何区域");
            }
            try
            {
                List<string> AreaCodeList = new List<string>();
                foreach (var item in list)
                {
                    AreaCodeList.Add(item.AreaId);
                }

                var SplitNo = list.FirstOrDefault().SplitNo;
                list = this.WmsSplitAreaRepository.Query().Where(a => a.SplitNo == SplitNo && AreaCodeList.Contains(a.AreaId)).ToList();
                WmsSplitMainRepository.UnitOfWork.TransactionEnabled = true;
                //1通知PTL亮灯操作
                foreach (var wmsSplitArea in list)
                {
                    if (wmsSplitArea.Status != 1)
                    {
                        return DataProcess.Failure("区域:" + wmsSplitArea.AreaId + "状态不对");
                    }
                    bool IsPTLExecuteSuccess = false;
                    //2 更改区域下架完成状态
                    //3更新库存操作 将已下架的库存置为虚拟库位
                    //Bussiness.ShiYiTongServices.WmsSplitServer splitServer = new Bussiness.ShiYiTongServices.WmsSplitServer(this.UnitOfWork);
                    //Bussiness.Services.StockServer stockServer = new StockServer(this.UnitOfWork);



                    //int AreaCount = splitServer.SplitAreaRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA WHERE SPLITNO='" + SplitNo + "'");
                    //int FinishedAreaCount = splitServer.SplitAreaRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA WHERE SPLITNO='" + SplitNo + "' AND STATUS=2");
                    //var ReelList = splitServer.SplitAreaReelRepository.Query("SELECT * FROM TB_WMS_SPLIT_AREA_REEL  WHERE STATUS=1 AND SPLITNO='" + SplitNo + "' AND AREAID='" + AreaId + "'");
                    var ReelList = WmsSplitAreaReelRepository.Query().Where(a => a.Status == 1 && a.SplitNo == wmsSplitArea.SplitNo && a.AreaId == wmsSplitArea.AreaId).ToList();
                    List<string> ReelIdList = new List<string>();
                    foreach (var item in ReelList)
                    {
                        ReelIdList.Add(item.ReelId);
                    }
                    string VirtualLocation = ReelList.FirstOrDefault().WareHouseCode + "00000000";
                    //通知PTL灭灯
                    //2通知PTL点亮该区域的拆分任务
                    //var splitMain = splitServer.SplitMainRepository.GetEntity<Bussiness.Entitys.SMT.WmsSplitMain>("SELECT * FROM TB_WMS_SPLIT_MAIN WHERE SPLITNO='" + SplitNo + "'");
                    var splitArea = WmsSplitAreaRepository.GetEntity(wmsSplitArea.Id);/*splitServer.SplitAreaRepository.GetEntity<Bussiness.Entitys.SMT.WmsSplitArea>("SELECT * FROM TB_WMS_SPLIT_AREA WHERE SPLITNO='" + SplitNo + "' AND AREAID='" + AreaId + "'");*/

                    //wmsToPtlServer.Finish(
                    // wmsToPtlServer.Finish(shelfMain.AreaId, LastProofId);

                    //  using (this.UnitOfWork)
                    {
                        //  UnitOfWork.TransactionEnabled = true;
                        wmsSplitArea.Status = 2;
                        splitArea.Status = 2;
                        WmsSplitAreaRepository.Update(splitArea);
                        //splitServer.SplitAreaRepository.Execute("UPDATE TB_WMS_SPLIT_AREA SET STATUS=2 WHERE STATUS=1 AND SPLITNO='" + SplitNo + "' AND AREAID='" + AreaId + "'");
                        //splitServer.SplitAreaRepository.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL SET STATUS=2 WHERE STATUS=1 AND SPLITNO='" + SplitNo + "' AND AREAID='" + AreaId + "'");
                        WmsSplitAreaReelRepository.Update(a => new Bussiness.Entitys.SMT.WmsSplitAreaReel() { Status = 2 }, p => p.Status == 1 && p.SplitNo == splitArea.SplitNo && p.AreaId == splitArea.AreaId);
                        foreach (var item in ReelIdList)
                        {
                            StockRepository.Update(a => new Bussiness.Entitys.Stock() { LocationCode = VirtualLocation }, p => p.MaterialLabel == item);//.Execute("UPDATE TB_WMS_REELID_STOCK SET LOCATIONCODE ='" + VirtualLocation + "' WHERE REELID ='" + item + "'");
                        }
                        //if (AreaCount - FinishedAreaCount == 1)
                        //{
                        //    splitServer.SplitMainRepository.Execute("UPDATE TB_WMS_SPLIT_MAIN SET STATUS =2 WHERE STATUS=1 AND SPLITNO='" + SplitNo + "'");
                        //}
                        // UnitOfWork.Commit();
                    }
                }

                var allSplitAreaList = this.WmsSplitAreaRepository.Query().Where(a => a.SplitNo == SplitNo).ToList();
                int finishCout = allSplitAreaList.FindAll(a => a.Status >= 2) == null ? 0 : allSplitAreaList.FindAll(a => a.Status >= 2).Count();
                if (finishCout == allSplitAreaList.Count())
                {
                    WmsSplitMainRepository.Update(a => new Bussiness.Entitys.SMT.WmsSplitMain() { Status = 2 }, p => p.SplitNo == SplitNo && p.Status == 1);
                }

                WmsSplitMainRepository.UnitOfWork.Commit();
                PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                foreach (var wmsSplitArea in list)
                {
                    //WmsToPTLServer.ForWmsService wmsToPtlServer = new WmsToPTLServer.ForWmsService();
                    //wmsToPtlServer.Finish(wmsSplitArea.AreaId, wmsSplitArea.ProofId);

                    var result = ptlServer.FinishOrder(wmsSplitArea.AreaId, wmsSplitArea.ProofId,false);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }

                return DataProcess.Success("操作成功");
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 创建并发布PTL任务 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="detailList"></param>
        /// <param name="SplitAreaReelList"></param>
        /// <param name="splitMain"></param>
        /// <returns></returns>
        public bool CreateDpsInterfaceForSplit(ref Bussiness.Entitys.PTL.DpsInterfaceMain main, ref List<Bussiness.Entitys.PTL.DpsInterface> detailList, List<Bussiness.Entitys.SMT.WmsSplitAreaReel> SplitAreaReelList, string ProofId)
        {
            main.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            main.OrderType = 1;
            main.ProofId = ProofId;
            main.Status = 0;
            main.OrderCode = SplitAreaReelList.FirstOrDefault().SplitNo;
            main.WareHouseCode = SplitAreaReelList.FirstOrDefault().WareHouseCode;
            main.AreaCode = SplitAreaReelList.FirstOrDefault().AreaId;
            foreach (var item in SplitAreaReelList)
            {
                Bussiness.Entitys.PTL.DpsInterface face = new Bussiness.Entitys.PTL.DpsInterface();
                face.BatchNO = "";
                face.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                face.GoodsName = item.MaterialCode;
                face.LocationId = item.LocationCode;
                face.MakerName = "";
                face.ProofId = ProofId;
                face.Quantity = item.OrgQuantity;
                face.Spec = "";
                face.Status = 0;
                face.UserId = "";
                face.ToteId = "";
                face.OperateId = Guid.NewGuid().ToString();
                face.RelationId = item.Id;
                face.MaterialLabel = item.ReelId;
                detailList.Add(face);
            }
            return true;
        }

        /// <summary>
        /// 作废拆盘单拣货条码
        /// </summary>
        /// <param name="SplitReel"></param>
        /// <param name="SplitNo"></param>
        /// <returns></returns>
        public DataResult CancelSplitReel(string SplitReel, string SplitNo)
        {
            var mainEnitty = this.WmsSplitMainRepository.Query().FirstOrDefault(a => a.SplitNo == SplitNo);
            if (mainEnitty == null)
            {
                return DataProcess.Failure("未找到该拆盘单");
            }
            var SplitReelEntity = this.WmsSplitAreaReelRepository.Query().FirstOrDefault(a => a.ReelId == SplitReel && a.SplitNo == SplitNo);
            if (SplitReelEntity == null)
            {
                return DataProcess.Failure("未找到该拆盘条码");
            }
            if (SplitReelEntity.Status == 5)
            {
                return DataProcess.Failure("该条码已作废");
            }
            if (SplitReelEntity.Status == 3 || SplitReelEntity.Status == 4)
            {
                return DataProcess.Failure("该条码已完成拆盘");
            }
            List<string> PickOrderCodeList = new List<string>();//拣货单编号集合
            List<Bussiness.Entitys.SMT.WmsSplitIssue> orderIssueList = this.WmsSplitIssueRepository.Query().Where(a => a.SplitNo == SplitNo).ToList();
            List<string> PickDetailIdList = new List<string>();//拣货明细ID集合
            foreach (var item in orderIssueList)
            {
                PickOrderCodeList.Add(item.PickOrderCode);
            }
            var PickOrderAreaDetailList = this.WmsPickOrderAreaDetailRepository.Query().Where(a => a.ReelId == SplitReel && PickOrderCodeList.Contains(a.PickOrderCode)).ToList();/*.Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE REELID ='" + SplitReel + "' AND PICKORDERCODE IN @CodeList", new { CodeList = PickOrderCodeList });*/
            foreach (var item in PickOrderAreaDetailList)
            {
                PickDetailIdList.Add(item.PickOrderDetailId);
            }
            var PickOrderDetailList = this.WmsPickOrderDetailRepository.Query().Where(a => PickOrderCodeList.Contains(a.PickOrderCode) && PickDetailIdList.Contains(a.DetailId)).ToList();/*("SELECT * FROM TB_WMS_PICK_ORDER_DETAIL WHERE DETAILID IN @DetailIdList AND PICKORDERCODE IN @CodeList", new { DetailIdList = PickDetailIdList, CodeList = PickOrderCodeList });*/
            foreach (var item in PickOrderDetailList)
            {
                var list = PickOrderAreaDetailList.FindAll(a => a.PickOrderDetailId == item.DetailId);
                if (list.Count > 0)
                {
                    var ReadyQauntiy = 0;
                    ReadyQauntiy = item.CurQuantity.GetValueOrDefault(0) - list.Sum(a => a.NeedQuantity.GetValueOrDefault(0));
                    if (ReadyQauntiy >= item.Quantity)
                    {
                        item.CurQuantity = ReadyQauntiy;
                        item.Status = 3;
                    }
                    else if (ReadyQauntiy > 0 && ReadyQauntiy < item.Quantity)
                    {
                        item.CurQuantity = ReadyQauntiy;
                        item.Status = 2;
                    }
                    else
                    {
                        item.CurQuantity = 0;
                        item.Status = 1;
                    }

                }
            }
            var stockEntity = this.StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == SplitReel);
            var count = this.WmsSplitAreaReelRepository.GetCount(a => a.SplitNo == SplitNo);/*.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + SplitNo + "'");*/
            var CancelCount = this.WmsSplitAreaReelRepository.GetCount(a => a.SplitNo == SplitNo && a.Status == 5);/*.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + SplitNo + "' and status =5");*/
            var ConfirmCount = this.WmsSplitAreaReelRepository.GetCount(a => (a.Status == 3 || a.Status == 4) && a.SplitNo == SplitNo);/*.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + SplitNo + "' and status =3");*/
            //var ReadyShelfCount = SplitAreaReelRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + SplitNo + "' and status =4");
            var AreaCount = this.WmsSplitAreaReelRepository.GetCount(a => a.SplitNo == SplitNo && a.AreaId == SplitReelEntity.AreaId);/*.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + SplitNo + "' and AREAID='" + SplitReelEntity.AreaId + "'");*/
            var AreaCancelCount = this.WmsSplitAreaReelRepository.GetCount(a => a.SplitNo == SplitNo && a.Status == 5 && a.AreaId == SplitReelEntity.AreaId);/*.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + SplitNo + "' and status =5 and AREAID='" + SplitReelEntity.AreaId + "'");
*/
            //   using (this.UnitOfWork)
            {
                //1更新拆盘单状态
                this.WmsSplitAreaReelRepository.UnitOfWork.TransactionEnabled = true;
                this.WmsSplitAreaReelRepository.Update(a => new Bussiness.Entitys.SMT.WmsSplitAreaReel() { Status = 5 }, p => p.SplitNo == SplitNo && p.ReelId == SplitReel);/*.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "' and REELID='" + SplitReel + "'");*/
                this.WmsSplitAreaReelDetailRepository.Update(a => new Bussiness.Entitys.SMT.WmsSplitAreaReelDetail() { Status = 5 }, p => p.SplitNo == SplitNo && p.OrgReelId == SplitReel);/*.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL_DETAIL SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "' AND  ORGREELID='" + SplitReel + "'");*/
                //if (stockEntity.LocationCode!="038A00000000")
                //  {
                //stockServer.StockRepository.Execute("UPDATE TB_WMS_REELID_STOCK SET ISLOCKED = 0 WHERE REELID ='" + SplitReel + "'");
                // }
                // else
                //   {
                if (stockEntity != null)
                {
                    this.StockRepository.Delete(stockEntity);
                }
                // }
                foreach (var item in PickOrderDetailList)
                {
                    this.WmsPickOrderDetailRepository.Update(item);
                }
                foreach (var item in PickOrderAreaDetailList)
                {
                    this.WmsPickOrderAreaDetailRepository.Delete(item);
                }
                //如果作废的是最后一个 并且点亮了  直接熄灭
                if (AreaCount - AreaCancelCount == 1)
                {
                    var SplitArea = this.WmsSplitAreaRepository.Query().FirstOrDefault(a => a.SplitNo == SplitNo && a.AreaId == SplitReelEntity.AreaId);/*.GetEntity<Bussiness.Entitys.SMT.WmsSplitArea>("SELECT * FROM TB_WMS_SPLIT_AREA WHERE SPLITNO='" + mainEnitty.SplitNo + "' AND AREAID='" + SplitReelEntity.AreaId + "'");*/
                    if (SplitArea.Status == 1)
                    {
                        //WmsToPTLServer.ForWmsService ptlServer = new WmsToPTLServer.ForWmsService();
                        //ptlServer.Finish(SplitArea.AreaId, SplitArea.ProofId);
                        PtlServer.PtlServer ptlServer = new PtlServer.PtlServer();
                        ptlServer.FinishOrder(SplitArea.AreaId, SplitArea.ProofId,false);
                    }
                }

                if (ConfirmCount > 0)
                {
                    if (count - (CancelCount + ConfirmCount) == 1)
                    {

                        this.WmsSplitAreaRepository.Update(a => new Entitys.SMT.WmsSplitArea() { Status = 3 }, p => p.SplitNo == SplitNo);//Execute("UPDATE TB_WMS_SPLIT_AREA SET STATUS =3 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");
                        //      SplitIssueRepository.Execute("UPDATE TB_WMS_SPLIT_ISSUE SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");
                        mainEnitty.Status = 3;
                        this.WmsSplitMainRepository.Update(mainEnitty);
                        //SplitAreaReelRepository.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");
                        //    SplitAreaReelDetailRepository.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL_DETAIL SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");

                    }
                }
                else
                {
                    if (count - CancelCount == 1)
                    {

                        this.WmsSplitAreaRepository.Update(a => new Entitys.SMT.WmsSplitArea() { Status = 5 }, p => p.SplitNo == SplitNo);//Execute("UPDATE TB_WMS_SPLIT_AREA SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");
                        //      SplitIssueRepository.Execute("UPDATE TB_WMS_SPLIT_ISSUE SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");
                        mainEnitty.Status = 5;
                        WmsSplitMainRepository.Update(mainEnitty);
                        //SplitAreaReelRepository.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");
                        //    SplitAreaReelDetailRepository.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL_DETAIL SET STATUS =5 WHERE SPLITNO='" + mainEnitty.SplitNo + "'");

                    }
                }

                this.WmsSplitAreaReelRepository.UnitOfWork.Commit();
            }
            return DataProcess.Success();
        }


        /// <summary>
        /// 确认拆盘
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="SplitNo"></param>
        /// <returns></returns>
        public DataResult ConfirmSplitReel(string ReelId, string SplitNo)
        {
            var result = CheckReelIdIsInSplitTask(ReelId, SplitNo);
            if (!result.Success)
            {
                return DataProcess.Failure(result.Message);
            }
            //var Connection = new System.Data.OracleClient.OracleConnection(); ;//Oracle.DataAccess.Client.OracleConnection();
            //Connection.ConnectionString = ConnectionString;
            ////Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand();
            //var cmd = new System.Data.OracleClient.OracleCommand();
            //cmd.Connection = Connection;
            //Connection.Open();
            // cmd.CommandText = "SELECT * FROM MES.IVMAT_REEL@MES WHERE REEL_ID='" + ReelId + "'";
            // cmd.CommandText = "return_reelid";

            //   cmd.Parameters.AddRange(parameter);
            //  cmd.ExecuteNonQuery();
            // string text = cmd.Parameters["current_reelid"].Value.ToString();

            //  return RunProcedure("pkg_tabletype.sp_cpzd", parameter, "ds");

            var SplitReelStock = this.StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == ReelId);
            List<Bussiness.Entitys.Label> labelList = new List<Entitys.Label>();
            if (result.Success)//result.Success
            {
                //1执行拆盘计划
                var SplitReelDetailList = this.WmsSplitAreaReelDetailRepository.Query().Where(a => a.SplitNo == SplitNo && a.OrgReelId == ReelId).ToList();//.Query("SELECT * FROM TB_WMS_SPLIT_AREA_REEL_DETAIL WHERE SPLITNO='" + SplitNo + "' AND ORGREELID='" + ReelId + "'");

                foreach (var item in SplitReelDetailList)
                {
                    if (!string.IsNullOrEmpty(item.SplitReelId))
                    {
                        //           // 更新
                        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        //            cmd.CommandText = "MES.SP_WMS_UPDATEREELIDQTY@MES";//MES.MES_IQC_REELID_QTY_UPDATE@MES
                        //            System.Data.OracleClient.OracleParameter[] parameter = {
                        //    new  System.Data.OracleClient.OracleParameter("PREELID",System.Data.OracleClient.OracleType.VarChar,100),
                        //    new  System.Data.OracleClient.OracleParameter("PREELQTY",System.Data.OracleClient.OracleType.Number),
                        //    new  System.Data.OracleClient.OracleParameter("PRESULT", System.Data.OracleClient.OracleType.VarChar,200),
                        //new  System.Data.OracleClient.OracleParameter("PRET", System.Data.OracleClient.OracleType.Number)
                        //};
                        //parameter[0].Value = item.SplitReelId;
                        //parameter[1].Value = item.SplitQuantity;//item.SplitQuantity.GetValueOrDefault(0);
                        //parameter[2].Direction = System.Data.ParameterDirection.Output;
                        //parameter[3].Direction = System.Data.ParameterDirection.Output;
                        //cmd.Parameters.Clear();
                        //cmd.Parameters.AddRange(parameter);
                        //cmd.ExecuteNonQuery();
                        //   string isOk = cmd.Parameters["PRET"].Value.ToString();
                        //   string returnStr = cmd.Parameters["PRESULT"].Value.ToString();
                        // if (isOk!="0")
                        //  {
                        //      return DataProcess.Failure(returnStr);
                        //  }
                        item.Status = 3;
                    }
                    else
                    {
                        // 生成
                        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        //            cmd.CommandText = "MES.MES_ISTOCK_REELID_LABEL@MES";
                        //            System.Data.OracleClient.OracleParameter[] parameter = {
                        //        new  System.Data.OracleClient.OracleParameter("PITEM",System.Data.OracleClient.OracleType.VarChar,100),
                        //        new  System.Data.OracleClient.OracleParameter("PDATECODE",System.Data.OracleClient.OracleType.VarChar,100),
                        //                 new  System.Data.OracleClient.OracleParameter("PLOTCODE",System.Data.OracleClient.OracleType.VarChar,100),
                        //    new  System.Data.OracleClient.OracleParameter("PQTY",System.Data.OracleClient.OracleType.Number),
                        //                 new  System.Data.OracleClient.OracleParameter("PVENDOR",System.Data.OracleClient.OracleType.VarChar,100),
                        //    new  System.Data.OracleClient.OracleParameter("PRESULT", System.Data.OracleClient.OracleType.VarChar,200),
                        //new  System.Data.OracleClient.OracleParameter("PRET", System.Data.OracleClient.OracleType.Number)};
                        //            parameter[0].Value = SplitReelStock.MaterialCode;
                        //            parameter[1].Value = NullToString(SplitReelStock.ReelCreateCode);
                        //            parameter[2].Value = NullToString(SplitReelStock.BatchCode);
                        //            parameter[3].Value = item.SplitQuantity.GetValueOrDefault(0);
                        //            parameter[4].Value = NullToString(SplitReelStock.SupplierCode);
                        //            parameter[5].Direction = System.Data.ParameterDirection.Output;
                        //            parameter[6].Direction = System.Data.ParameterDirection.Output;
                        //            cmd.Parameters.Clear();
                        //            cmd.Parameters.AddRange(parameter);
                        //            cmd.ExecuteNonQuery();
                        //            string isOk = cmd.Parameters["PRET"].Value.ToString();
                        //            string returnStr = cmd.Parameters["PRESULT"].Value.ToString();
                        //            if (isOk != "0")
                        //            {
                        //                return DataProcess.Failure(returnStr);
                        //            }
                        Bussiness.Entitys.Label newLabel = new Entitys.Label();
                        item.SplitReelId = SequenceContract.Create("Label");
                        newLabel.BatchCode = SplitReelStock.BatchCode;
                        newLabel.Code = item.SplitReelId;
                        newLabel.IsDeleted = false;
                        newLabel.MaterialCode = SplitReelStock.MaterialCode;
                    //    newLabel.ProductionDate = DateTime.Parse(SplitReelStock.ManufactureDate);
                        newLabel.Quantity = item.SplitQuantity.GetValueOrDefault(0);
                        newLabel.SupplierCode = SplitReelStock.SupplierCode;
                        // item.SplitReelId = DateTime.Now.ToString("yyyyMMddHHmmss");
                        item.Status = 3;
                        labelList.Add(newLabel);

                    }
                }
                //更新执定的reelId拣货详情
                //Bussiness.ShiYiTongServices.WmsPickOrderServer pickOrderServer = new Bussiness.ShiYiTongServices.WmsPickOrderServer(this.UnitOfWork);
                List<Bussiness.Entitys.SMT.WmsSplitIssue> orderIssueList = this.WmsSplitIssueRepository.Query().Where(a => a.SplitNo == SplitNo).ToList();//("SELECT * FROM TB_WMS_SPLIT_ISSUE WHERE SPLITNO='" + SplitNo + "'");
                List<string> PickOrderCodeList = new List<string>();
                List<string> PickDetailIdList = new List<string>();
                foreach (var item in SplitReelDetailList)
                {
                    PickDetailIdList.Add(item.PickDetailId);
                }



                List<Bussiness.Entitys.Stock> newStockList = new List<Bussiness.Entitys.Stock>();
                var splitMain = WmsSplitMainRepository.Query().FirstOrDefault(a => a.SplitNo == SplitNo);//.GetEntity<Bussiness.Entitys.SMT.WmsSplitMain>("SELECT * FROM TB_WMS_SPLIT_MAIN WHERE SPLITNO='" + SplitNo + "'");

                var SplitReel = this.WmsSplitAreaReelRepository.Query().FirstOrDefault(a => a.SplitNo == SplitNo && a.ReelId == ReelId);//GetEntity<Bussiness.Entitys.SMT.WmsSplitAreaReel>("SELECT * FROM TB_WMS_SPLIT_AREA_REEL WHERE ReelId='" + ReelId + "' AND SPLITNO='" + SplitNo + "'");

                List<Bussiness.Entitys.SMT.WmsPickOrderAreaDetail> pickAreaDetailList = this.WmsPickOrderAreaDetailRepository.Query().Where(a => PickDetailIdList.Contains(a.PickOrderDetailId) && a.ReelId == SplitReel.ReelId).ToList();//Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE PICKORDERDETAILID IN @CodeList AND REELID='" + SplitReel.ReelId + "'", new { CodeList = PickDetailIdList });
                foreach (var item in SplitReelDetailList)
                {
                    Bussiness.Entitys.Stock stock = new Bussiness.Entitys.Stock();
                    stock = HP.Utility.JsonHelper.DeserializeObject<Bussiness.Entitys.Stock>(HP.Utility.JsonHelper.SerializeObject(SplitReelStock));
                    stock.Id = 0;
                    stock.MaterialLabel = item.SplitReelId;
                    stock.Quantity = Convert.ToDecimal(item.SplitQuantity);
                    if (string.IsNullOrEmpty(item.PickDetailId) || string.IsNullOrEmpty(item.PickOrderCode))
                    {
                        stock.IsLocked = false;
                        stock.LockedQuantity = 0;
                    }
                    else
                    {
                        stock.IsLocked = true;
                        stock.LockedQuantity = item.SplitQuantity.GetValueOrDefault(0);
                    }
                    //if (item.PickDetailId != "" && item.PickOrderCode != null)
                    //{
                    newStockList.Add(stock);
                    //}

                }
                foreach (var item in SplitReelDetailList)
                {
                    foreach (var piclAreaReel in pickAreaDetailList)
                    {
                        if (item.PickOrderCode == piclAreaReel.PickOrderCode && item.PickDetailId == piclAreaReel.PickOrderDetailId)
                        {
                            piclAreaReel.ReelId = item.SplitReelId;
                            piclAreaReel.OrgQuantity = item.SplitQuantity;
                            piclAreaReel.NeedQuantity = item.SplitQuantity;
                            piclAreaReel.IsNeedSplit = false;
                            piclAreaReel.Status = 2;
                            break;
                        }
                    }
                }
                var count = this.WmsSplitAreaReelRepository.GetCount(a => a.SplitNo == SplitNo);//.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + SplitReel.SplitNo + "'");
                var SplitedCount = WmsSplitAreaReelRepository.GetCount(a => a.SplitNo == SplitNo && a.Status > 2);//GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL WHERE SPLITNO='" + SplitReel.SplitNo + "' and status >2");
                SplitReel.Status = 3;
                //MesBussiness.MesServices.SyncReelSplitInfoServer mesServer = new MesBussiness.MesServices.SyncReelSplitInfoServer(UnitOfWork);
                //  using (UnitOfWork)
                WmsSplitAreaReelRepository.UnitOfWork.TransactionEnabled = true;
                {
                    //删除原先库存
                    this.StockRepository.Delete(SplitReelStock);
                    foreach (var item in newStockList)
                    {
                        this.StockRepository.Insert(item);
                    }
                    //splitServer.SplitAreaReelRepository.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL SET STATUS=3 WHERE REELID ='" + SplitReel.ReelId + "'");
                    WmsSplitAreaReelRepository.Update(SplitReel);
                    foreach (var item in SplitReelDetailList)
                    {
                        WmsSplitAreaReelDetailRepository.Update(item);
                    }
                    foreach (var item in pickAreaDetailList)
                    {
                        WmsPickOrderAreaDetailRepository.Update(item);
                    }
                    if (count - SplitedCount == 1)
                    {
                        splitMain.Status = 3;
                        WmsSplitMainRepository.Update(splitMain);//Execute("UPDATE TB_WMS_SPLIT_MAIN SET STATUS=3 WHERE SPLITNO ='" + SplitReel.SplitNo + "'");

                        WmsSplitAreaRepository.Update(a => new Bussiness.Entitys.SMT.WmsSplitArea() { Status = 3 }, p => p.SplitNo == SplitNo);//Execute("UPDATE TB_WMS_SPLIT_AREA SET STATUS=3 WHERE SPLITNO ='" + SplitReel.SplitNo + "'");

                    }
                    foreach (var item in labelList)
                    {
                        LabelContract.Insert(item);
                    }
                    // this.UnitOfWork.TransactionEnabled = true;
                    WmsSplitAreaReelRepository.UnitOfWork.Commit();
                    #region 
                    //插入MES中间表

                    //var id = mesServer.SplitHeadRepository.GetEntity<int?>("SELECT MAX(HEAD_ID) FROM WMS_SPLIT_REEL_LINE");
                    //if (id == null)
                    //{
                    //    id = 0;
                    //}
                    //MesBussiness.MesEntitys.SplitReelHead head = new MesBussiness.MesEntitys.SplitReelHead();
                    //List<MesBussiness.MesEntitys.SplitReelDetail> detalist = new List<MesBussiness.MesEntitys.SplitReelDetail>();
                    //head.Head_Id = id + 2;
                    //head.Cur_Quantity = SplitReel.OrgQuantity;
                    //head.Old_Reel_Id = SplitReel.ReelId;
                    //head.User_Id = "WMS";
                    //head.User_Name = "WMS";
                    //head.Wms_Flag = 0;
                    //head.Create_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //foreach (var item in SplitReelDetailList)
                    //{
                    //    MesBussiness.MesEntitys.SplitReelDetail detail = new MesBussiness.MesEntitys.SplitReelDetail();
                    //    detail.Cur_Quantity = item.SplitQuantity;
                    //    detail.Date_Code = SplitReelStock.ReelCreateCode;
                    //    detail.Head_Id = head.Head_Id;
                    //    detail.Lot_Number = SplitReelStock.BatchCode;
                    //    detail.Material_Id = SplitReelStock.MaterialCode;
                    //    detail.New_Reel_Id = item.SplitReelId;
                    //    detail.User_Id = "WMS";
                    //    detail.Vendor_Id = SplitReelStock.SupplierCode;
                    //    detail.WmsFlag = 0;
                    //    detail.Create_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //    detalist.Add(detail);
                    //    cmd.Parameters.Clear();
                    //    cmd.CommandType = System.Data.CommandType.Text;
                    //    cmd.CommandText = "INSERT INTO WMS_SPLIT_REEL_LINE VALUES (" + detail.Head_Id + ",'" + detail.New_Reel_Id + "','" + detail.Material_Id + "','" + detail.Vendor_Id + "','" + detail.Date_Code + "','" + detail.Lot_Number + "'," + detail.Cur_Quantity + ",'" + detail.User_Id + "','" + detail.Create_Date + "'," + detail.WmsFlag + ")";
                    //    cmd.ExecuteNonQuery();
                    //    //mesServer.SplitDetailRepository.Execute("INSERT INTO WMS_SPLIT_REEL_LINE(HEAD_ID,NEW_REEL_ID,MATERIAL_ID,VENDOR_ID,DATE_CODE,LOT_NUMBER,CUR_QUANTITY,USER_ID,CREATE_DATE,WMS_FLAG) VALUES(@HeadId,@NewReelId,@MaterialId,@VendorId,@DateCode,@LotNumer,@CurQuantity,@UserId,,@WmsFlag)", new { HeadId = detail.Head_Id, NewReelId = detail.New_Reel_Id, MaterialId = detail.Material_Id, VendorId = detail.Vendor_Id, DateCode = detail.Date_Code, LotNumer = detail.Lot_Number, CurQuantity = detail.Cur_Quantity, UserId = detail.User_Id, WmsFlag = detail.WmsFlag });

                    //}
                    //// using (mesServer.UnitOfWork)
                    //// {
                    ////   mesServer.UnitOfWork.TransactionEnabled = true;
                    ////mesServer.SplitDetailRepository.Insert(detalist);
                    ////  mesServer.SplitHeadRepository.Insert(head);
                    ////   mesServer.UnitOfWork.Commit();
                    //// }
                    //mesServer.SplitHeadRepository.Execute("INSERT INTO WMS_SPLIT_REEL_HEAD VALUES(@Head_Id,@OldReelId,@Create_Date,@UserId,@CurQuantity,@WmsGetDate,@WmsFlag)", new { Head_Id = head.Head_Id, OldReelId = head.Old_Reel_Id, Create_Date = head.Create_Date, UserId = head.User_Id, CurQuantity = head.Cur_Quantity, WmsGetDate = head.Wms_Get_Date, WmsFlag = head.Wms_Flag });
                    #endregion

                }
            }
            else
            {

                return DataProcess.Failure(result.Message);
            }
            var splitMainAfter = this.WmsSplitMainRepository.Query().FirstOrDefault(a => a.SplitNo == SplitNo);
            if (splitMainAfter.Status == 3)
            {
                return DataProcess.Success("此拆盘单拆盘完毕");
            }

            return DataProcess.Success();
        }

        /// <summary>
        /// 检查ReelId是否在此拆盘单下
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="SplitNo"></param>
        /// <returns></returns>
        public DataResult CheckReelIdIsInSplitTask(string ReelId, string SplitNo)
        {
            var entity = this.WmsSplitAreaReelRepository.Query().FirstOrDefault(a => a.SplitNo == SplitNo && a.ReelId == ReelId); //("SELECT * FROM TB_WMS_SPLIT_AREA_REEL WHERE REELID='" + ReelId + "' AND SPLITNO='" + SplitNo + "'");
            if (entity == null)
            {
                return DataProcess.Failure("此条码不在此拆盘计划中");
            }
            if (entity.Status != 2)
            {
                return DataProcess.Failure("该拆盘单下的此条码还未下架或者已拆盘");
            }
            return DataProcess.Success();
        }
        /// <summary>
        /// WEB端手动确认上架拆盘条码
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="SplitNo"></param>
        /// <returns></returns>
        public DataResult WebConfirmShelfSplitReel(string ReelId, string SplitNo, string LocationCode)
        {
            #region 1库位码的合法性
            bool IsRightLocation = true;
            var LocationEntity = WareHouseContract.LocationRepository.Query().FirstOrDefault(a => a.Code == LocationCode);
            if (LocationEntity == null)
            {
                IsRightLocation = false;
            }
            if (!IsRightLocation)
            {
                return DataProcess.Failure("库位码不存在");
            }
            bool IsStockExist = StockRepository.GetCount(a => a.LocationCode == LocationCode) > 0;
            if (IsStockExist)
            {
                return DataProcess.Failure("库位上已存在条码");
            }

            #endregion

            var splitReelDetail = this.WmsSplitAreaReelDetailRepository.Query().FirstOrDefault(a => a.SplitReelId == ReelId && a.SplitNo == SplitNo);
            if (splitReelDetail == null)
            {
                return DataProcess.Failure("不存在此拆盘条码");
            }
            if (splitReelDetail.Status != 3)
            {
                return DataProcess.Failure("拆盘条码状态不对");
            }

            var label = this.LabelContract.Query().FirstOrDefault(a => a.Code == ReelId);
            //bool isReelIdExist = this.StockRepository.GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_REELID_STOCK WHERE REELID='" + ReelId + "' AND LOCATIONCODE='" + LocationEntity.WarehouseId + "00000000" + "'") > 0;
            //if (!isReelIdExist)
            //{
            //    result.Success = false;
            //    result.Message = "该ReelId不在缓存区";
            //    return RT.Utility.JsonHelper.SerializeObject(result);
            //}
            Entitys.SMT.WmsShelfMain main = new Entitys.SMT.WmsShelfMain();
            //var rule = new RT.BaseService.Sequence.Sequence("LabelRule");
            // main.CreateUserName = RT.BaseService.Identity.Account.UserName; ;
            main.ReplenishCode = Guid.NewGuid().ToString();
            //main.ShelfCode = LocationEntity.ShelfCode;
            main.Status = 3;
            main.AreaId = LocationEntity.ContainerCode;
            main.WareHouseCode = LocationEntity.WareHouseCode;
            main.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //从mes视图中获取到ReelId信息
            Entitys.SMT.WmsShelfDetail detail = new Entitys.SMT.WmsShelfDetail();
            detail.ReelId = splitReelDetail.SplitReelId;
            detail.BatchCode = label.BatchCode;
            detail.LocationCode = label.LocationCode;
            detail.ReelCreateCode = label.ManufactrueDate.GetValueOrDefault(DateTime.Now);
            detail.MaterialCode = label.MaterialCode;
            detail.OrgQuantity = splitReelDetail.OrgQuantity;
            detail.Quantity = splitReelDetail.SplitQuantity;
            detail.ReplenishCode = main.ReplenishCode;
            detail.SupplierCode = label.SupplierCode;
            detail.ShelfSortNo = 1;
            detail.Status = 3;
            detail.Status = (int)Bussiness.Enums.SMT.ReplenishStatusEnum.Finished;
            detail.FinishedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            //找到拆盘对应reelId
            splitReelDetail.Status = 4;
            var SplitMain = this.WmsSplitMainRepository.Query().FirstOrDefault(a => a.SplitNo == SplitNo);
            var splitStock = StockRepository.Query().FirstOrDefault(a => a.MaterialLabel == ReelId);
            if (splitStock != null)
            {
                splitStock.LocationCode = LocationCode;
                if (string.IsNullOrEmpty(splitReelDetail.PickOrderCode) || string.IsNullOrEmpty(splitReelDetail.PickDetailId))
                {
                    splitStock.IsLocked = false;
                }
            }



            var TotalCount = this.WmsSplitAreaReelDetailRepository.GetCount(a => a.SplitNo == SplitNo);//GetEntity<int>("SELECT COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL_DETAIL WHERE SPLITNO='" + SplitNo + "'");
            var ShelfieldReelCount = this.WmsSplitAreaReelDetailRepository.GetCount(a => a.SplitNo == SplitNo && a.Status >= 4);//.GetEntity<int>("SELECT  COUNT(*) FROM TB_WMS_SPLIT_AREA_REEL_DETAIL WHERE SPLITNO='" + SplitNo + "' AND STATUS >= 4");

            // using (work)
            {
                // work.TransactionEnabled = true;
                WmsSplitAreaReelDetailRepository.UnitOfWork.TransactionEnabled = true;
                this.StockRepository.Update(splitStock);
                this.WmsSplitAreaReelDetailRepository.Update(splitReelDetail);
                //shelfDetail.Status = (int)Business.Enum.ReplenishStatusEnum.Finished;
                //shelfDetail.FinishedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //shelfServer.ShelfDetailRepository.Update(shelfDetail);
                ShelfContract.WmsShelfMainRepository.Insert(main);
                ShelfContract.WmsShelfDetailRepository.Insert(detail);
                //是否更新区域状态
                if (!string.IsNullOrEmpty(splitReelDetail.PickDetailId) && !string.IsNullOrEmpty(splitReelDetail.PickOrderCode))//SplitReel.PickDetailId != "" && SplitReel.PickOrderCode != ""
                {
                    //更新单据对应的拣货任务详情

                    var pickAreaList = this.WmsPickOrderAreaRepository.Query().Where(a => a.PickOrderCode == splitReelDetail.PickOrderCode).ToList();//Query("SELECT * FROM TB_WMS_PICK_ORDER_AREA WHERE PICKORDERCODE='" + SplitReel.PickOrderCode + "'");
                    if (pickAreaList.Find(a => a.AreaId == LocationEntity.ContainerCode) == null)
                    {
                        var newPickArea = new Bussiness.Entitys.SMT.WmsPickOrderArea();
                        newPickArea.AreaId = LocationEntity.ContainerCode;
                        newPickArea.PickOrderCode = splitReelDetail.PickOrderCode;
                        newPickArea.ProofId = Guid.NewGuid().ToString();
                        newPickArea.Status = 0;//待启动
                        newPickArea.WareHouseCode = LocationEntity.WareHouseCode;
                        this.WmsPickOrderAreaRepository.Insert(newPickArea);
                    }
                    else
                    {
                        foreach (var item in pickAreaList)
                        {
                            if (item.AreaId == LocationEntity.ContainerCode)
                            {
                                item.Status = 0;
                                this.WmsPickOrderAreaRepository.Update(item);
                                break;
                            }
                        }
                    }
                    var pickAreaReel = this.WmsPickOrderAreaDetailRepository.Query().FirstOrDefault(a => a.ReelId == splitReelDetail.SplitReelId && a.PickOrderCode == splitReelDetail.PickOrderCode && a.PickOrderDetailId == splitReelDetail.PickDetailId);//GetEntity<Bussiness.Entitys.SMT.WmsPickOrderAreaDetail>("SELECT * FROM TB_WMS_PICK_ORDER_AREA_DETAIL WHERE REELID='" + SplitReel.SplitReelId + "'AND PICKORDERCODE='" + SplitReel.PickOrderCode + "' AND PICKORDERDETAILID='" + SplitReel.PickDetailId + "'");
                    pickAreaReel.LocationCode = LocationCode;
                    pickAreaReel.Status = 0;
                    pickAreaReel.AreaId = LocationEntity.ContainerCode;
                    pickAreaReel.WareHouseCode = LocationEntity.WareHouseCode;
                    pickAreaReel.IsNeedSplit = false;
                    this.WmsPickOrderAreaDetailRepository.Update(pickAreaReel);

                }
                if (TotalCount - ShelfieldReelCount == 1)
                {
                    //splitServer.SplitMainRepository.Execute("UPDATE TB_WMS_SPLIT_MAIN SET STATUS =4 WHERE SPLITNO='" + SplitNo + "'");
                    SplitMain.Status = 4;
                    this.WmsSplitMainRepository.Update(SplitMain);
                    //splitServer.SplitAreaRepository.Execute("UPDATE TB_WMS_SPLIT_AREA SET STATUS =4 WHERE SPLITNO='" + SplitNo + "'");
                    this.WmsSplitAreaRepository.Update(a => new Entitys.SMT.WmsSplitArea() { Status = 4 }, p => p.SplitNo == SplitNo);
                    this.WmsSplitAreaReelRepository.Update(a => new Entitys.SMT.WmsSplitAreaReel() { Status = 4 }, p => p.SplitNo == SplitNo);
                    //splitServer.SplitAreaReelRepository.Execute("UPDATE TB_WMS_SPLIT_AREA_REEL SET STATUS =4 WHERE SPLITNO='" + SplitNo + "'");

                }
                WmsSplitAreaReelDetailRepository.UnitOfWork.Commit();
            }

            return DataProcess.Success();

        }

        /// <summary>
        /// 根据库位获取拆分任务
        /// </summary>
        /// <returns></returns>
        public DataResult GetReelIdByLocationCodeForSplit(string SplitNo, string ReelId, string LocationCode)
        {
            try
            {
                var entity = new Bussiness.Entitys.SMT.WmsSplitAreaReel();
                var query = this.WmsSplitAreaReelRepository.Query().Where(a => a.SplitNo == SplitNo);
                if (string.IsNullOrEmpty(ReelId) && string.IsNullOrEmpty(LocationCode))
                {
                    return DataProcess.Failure("物料条码或者库位码未输入");
                }
                if (!string.IsNullOrEmpty(ReelId))
                {
                    query = query.Where(a => a.ReelId == ReelId);
                }
                if (!string.IsNullOrEmpty(LocationCode))
                {
                    query = query.Where(a => a.LocationCode == LocationCode);
                }

                var reel = query.FirstOrDefault();
                if (reel == null)
                {
                    return DataProcess.Failure("未找到该拆盘单条码信息");
                }
                return DataProcess.Success("获取成功", reel);




            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        #endregion
    }
}
