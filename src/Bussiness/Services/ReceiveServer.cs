using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;
using NPOI.SS.Util;

namespace Bussiness.Services
{
    public class ReceiveServer : Contracts.IReceiveContract
    {
        public IRepository<ReceiveIF, int> ReceiveIFRepository { get; set; }
        public IRepository<ReceiveDetailIF, int> ReceiveDetailIFRepository { get; set; }
        public IRepository<Receive, int> ReceiveRepository { get; set; }

        /// <summary>
        /// 领用清单
        /// </summary>
        public IRepository<ReceiveDetail, int> ReceiveDetailRepository { get; set; }
        public IQuery<ReceiveDetail> ReceiveDetails => ReceiveDetailRepository.Query();

        /// <summary>
        /// 领用任务
        /// </summary>
        public IReceiveTaskContract ReceiveTaskContract { set; get; }
        public IRepository<ReceiveTask, int> ReceiveTaskRepository { get; set; }

        /// <summary>
        /// 任务明细
        /// </summary>
        public IRepository<ReceiveTaskDetail, int> ReceiveTaskDetailRepository { get; set; }

        /// <summary>
        ///视图
        /// </summary>
        public IRepository<Entitys.StockVM, int> StockVMRepository { get; set; }
        public IQuery<StockVM> StockVMs => StockVMRepository.Query();
        /// <summary>
        /// 用户信息
        /// </summary>
        public IIdentityContract IdentityContract { get; set; }

        public ISequenceContract SequenceContract { set; get; }

        public IWareHouseContract WareHouseContract { set; get; }

        public IMaterialContract MaterialContract { set; get; }

        public IMapper Mapper { set; get; }

       

        /// <summary>
        /// 模具
        /// </summary>
        public IMouldInformationContract MouldInformationContract { set; get; }

        public IQuery<Receive> Receives
        {
            get
            {
                return ReceiveRepository.Query();
            }
        }

        /// <summary>
        /// 联合查询数据库数据领用单信息
        /// </summary>
        public IQuery<ReceiveDto> ReceiveDtos
        {
            get
            {
                return Receives.LeftJoin(WareHouseContract.WareHouses, (receive, warehouse) => receive.WareHouseCode == warehouse.Code)
                    .LeftJoin(IdentityContract.Users, (receive, warehouse,user) => receive.LastTimeReceiveName == user.Code)
                    .Select((receive, warehouse, user) => new ReceiveDto()
                    {
                        Id = receive.Id,
                        Code = receive.Code,
                        ReceiveTime = receive.ReceiveTime,
                        Remarks = receive.Remarks,
                        Status = receive.Status,
                        ReceiveType = receive.ReceiveType,
                        IsDeleted = receive.IsDeleted,
                        LastTimeReceiveDatetime = receive.LastTimeReceiveDatetime,
                        LastTimeReceiveName = receive.LastTimeReceiveName,
                        ReceiveUserName = user.Name,
                        PredictReturnTime = receive.PredictReturnTime,
                        WareHouseCode = receive.WareHouseCode,
                        WarehouseName = warehouse.Name,
                        CreatedTime = receive.CreatedTime,
                        CreatedUserName = receive.CreatedUserName,
                        BillCode= receive.BillCode
                    });


            }
        }

        /// <summary>
        /// 联合查询数据库数据领用明细
        /// </summary>
        public IQuery<ReceiveDetailDto> ReceiveDetailDtos
        {
            get
            {
                // 领用明细和模具信息
                return ReceiveDetails
                    .InnerJoin(StockVMs, (receiveDetail, stockVM) => receiveDetail.MaterialLabel == stockVM.MaterialLabel)
                    .InnerJoin(MaterialContract.Materials, (receiveDetail, stockVM,materials) => stockVM.MaterialCode == materials.Code)
                    .InnerJoin(WareHouseContract.LocationVMs, (receiveDetail, stockVM, materials, location) => stockVM.LocationCode == location.Code)
                    .LeftJoin(IdentityContract.Users, (receiveDetail, stockVM, materials, location, user) => receiveDetail.LastTimeReturnName == user.Code)
                    .Select((receiveDetail, stockVM, materials, location, user) => new ReceiveDetailDto()
                    {
                        Id = receiveDetail.Id,
                        ReceiveCode = receiveDetail.ReceiveCode,
                        Status= receiveDetail.Status,
                        MaterialName = stockVM.MaterialName,
                        MaterialCode = stockVM.MaterialCode,
                        LocationCode = stockVM.LocationCode,
                        MaterialType = stockVM.MaterialType,
                        MaterialLabel = stockVM.MaterialLabel,
                        ReceiveType = receiveDetail.ReceiveType,
                        ContainerCode= stockVM.ContainerCode,
                        ReceiveTime = receiveDetail.ReceiveTime,
                        LastTimeReceiveDatetime = receiveDetail.LastTimeReceiveDatetime,
                        LastTimeReceiveName = receiveDetail.LastTimeReceiveName,
                        LastTimeReturnDatetime = receiveDetail.LastTimeReturnDatetime,
                        LastTimeReturnName = receiveDetail.LastTimeReturnName,
                        PredictReturnTime = receiveDetail.PredictReturnTime,
                        WareHouseCode = stockVM.WareHouseCode,
                        Remarks = materials.Name,
                        Quantity = receiveDetail.Quantity,
                        CreatedTime = receiveDetail.CreatedTime,
                        ReturnUserName = user.Name,
                        XLight = location.XLight,
                        YLight = location.YLight
                    });
            }
        }
        /// <summary>
        /// 轮训接口--创建WMS出库单
        /// </summary>
        public DataResult CreateReceiveInterFace()
        {
            try
            {
                ReceiveIFRepository.UnitOfWork.TransactionEnabled = true;
                var list = ReceiveIFRepository.Query().Where(a => a.Status == (int)InterFaceBCaption.Waiting).ToList();
                int count = 0;
                foreach (var item in list)
                {
                    // 判断该来源单据号是否已存在出库单
                    if (Receives.Any(a => a.BillCode == item.BillCode))
                    {
                        item.Status = (int)OrderEnum.Error;
                        item.Remark = "来源单据号" + item.BillCode + "已存在";
                        ReceiveIFRepository.Update(item);
                        break;
                    }
                    int errorflag = 0;
                    var receiveEnity = new Receive()
                    {
                        BillCode = item.BillCode,
                        WareHouseCode = item.WareHouseCode,
                        ReceiveType = item.ReceiveType,
                        Status = (int)ReceiveEnum.Wait,
                        LastTimeReceiveDatetime = item.LastTimeReceiveDatetime,
                        LastTimeReceiveName = item.LastTimeReceiveName,
                        PredictReturnTime = item.PredictReturnTime,
                        AddMaterial = new List<Bussiness.Entitys.ReceiveDetail>(),
                        OrderType = (int)OrderTypeEnum.Other,
                    };
                    var materialList = ReceiveDetailIFRepository.Query().Where(a => a.BillCode == item.BillCode).ToList();
                    foreach (var outMaterial in materialList)
                    {
                        outMaterial.Status = (int)OrderEnum.Wait;
                        if (MouldInformationContract.MouldInformations.FirstOrDefault(a => a.MaterialLabel == outMaterial.MaterialLabel) == null)
                        {
                            item.Status = (int)OrderEnum.Error;
                            errorflag = 1;
                            outMaterial.Status = (int)OrderEnum.Error;
                            outMaterial.Remark = "模具编码" + outMaterial.MaterialLabel + "不存在!";
                            ReceiveDetailIFRepository.Update(outMaterial);
                            break;
                        }

                        var inMaterialEntity = new ReceiveDetail()
                        {
                            BillCode = outMaterial.BillCode,
                            Status = 0,
                            ReceiveType = item.ReceiveType,
                            MaterialLabel = outMaterial.MaterialLabel,
                            Quantity = outMaterial.Quantity,
                            LastTimeReceiveDatetime= outMaterial.LastTimeReceiveDatetime,
                            LastTimeReceiveName= outMaterial.LastTimeReceiveName,
                            PredictReturnTime= outMaterial.PredictReturnTime,
                        };
                        receiveEnity.AddMaterial.Add(inMaterialEntity);
                        ReceiveDetailIFRepository.Update(outMaterial);
                    }

                    if (errorflag == 1)
                    {
                        ReceiveIFRepository.Update(item);
                    }
                    else
                    {
                        var result = CreateReceive(receiveEnity);
                        if (!result.Success)
                        {
                            return DataProcess.Failure(result.Message);
                        }
                        item.Status = (int)OrderEnum.Wait;
                        ReceiveIFRepository.Update(item);
                        count = count + 1;
                    }
                }
                ReceiveIFRepository.UnitOfWork.Commit();
                return DataProcess.Success(string.Format("领用单同步成功,共有{0}条增加", count));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        ///  添加领用单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateReceive(Receive entity)
        {
            ReceiveRepository.UnitOfWork.TransactionEnabled = true;
            // 判断是否有领用单号
            if (String.IsNullOrEmpty(entity.Code))
            {
                entity.Code = SequenceContract.Create(entity.GetType());
            }

            if (Receives.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure("该领用单号已存在");
            }
            if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
            {
                foreach (ReceiveDetail item in entity.AddMaterial)
                {
                    item.ReceiveCode = entity.Code; // 领用单编码赋值给清单编码
                    item.Status = entity.Status;
                    item.IsDeleted = entity.IsDeleted;
                    item.ReceiveType = entity.ReceiveType;
                    item.LastTimeReceiveName = entity.LastTimeReceiveName;
                    item.LastTimeReceiveDatetime = entity.LastTimeReceiveDatetime;
                    item.PredictReturnTime = entity.PredictReturnTime;
                    DataResult result = CreateInMaterialEntity(item);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                    var moudleInfo = MouldInformationContract.MouldInformations.FirstOrDefault(a => a.MaterialLabel == item.MaterialLabel);
                    if (moudleInfo.MouldState != (int)MouldInformationEnum.InWarehouse)
                    {
                        return DataProcess.Failure(string.Format("模具{0}状态不是在库中，无法创建领用单", moudleInfo.MaterialLabel));
                    }
                    moudleInfo.MouldState = (int)MouldInformationEnum.MouldLock;
                    moudleInfo.LastTimeReceiveName = IdentityContract.Users.FirstOrDefault(a => a.Code == entity.LastTimeReceiveName).Name;
                    moudleInfo.LastTimeReceiveDatetime = item.LastTimeReceiveDatetime;
                    moudleInfo.ReceiveType = entity.ReceiveType;
                    if (MouldInformationContract.MouldInformationRepository.Update(moudleInfo) < 0)
                    {
                        return DataProcess.Failure(string.Format("模具{0}编辑失败", entity.Id));
                    }
                }
            }         
            if (ReceiveRepository.Insert(entity))
            {
               
                ReceiveRepository.UnitOfWork.Commit();
                return DataProcess.Success(string.Format("领用单创建成功", entity.Code));
                
            }           
            return DataProcess.Failure();
        }

        /// <summary>
        /// 添加领用清单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateInMaterialEntity(ReceiveDetail entity)
        {           
            if (ReceiveDetailRepository.Insert(entity))
            {               
                return DataProcess.Success(string.Format("领用清单{0}新增成功", entity.MaterialLabel));
            }
            return DataProcess.Failure("操作失败");
        }

        /// <summary>
        ///  作废领用单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CancellationeReceive(Receive entity)
        {
          
            ReceiveRepository.UnitOfWork.TransactionEnabled = true;
            // 领用单
            //var ReceiveEntity = Receives.FirstOrDefault(a => a.Code == entity.Code);
            // 获取领用明细
            var ReceiveDetailList = ReceiveDetails.Where(a => a.ReceiveCode == entity.Code).ToList();
                     
            // 循环判断领用单明细状态，作废领用单明细
            foreach (var item in ReceiveDetailList)
            {
                if(item.Status == (int)ReceiveEnum.Finish)
                {
                    return DataProcess.Failure(string.Format("领用明细{0}作废失败", item.ReceiveCode));
                }         
                item.Status = (int)ReceiveEnum.Cancellation;                
                // 更改模具信息状态
                var moudleInfo = MouldInformationContract.MouldInformations.FirstOrDefault(a => a.MaterialLabel == item.MaterialLabel);
                if (moudleInfo.MouldState != (int)MouldInformationEnum.MouldLock)
                {
                    return DataProcess.Failure(string.Format("模具{0}状态不是在领用锁定，无法作废领用单", moudleInfo.MaterialLabel));
                }
                moudleInfo.MouldState = (int)MouldInformationEnum.InWarehouse;
                if (MouldInformationContract.MouldInformationRepository.Update(moudleInfo) < 0)
                {
                    return DataProcess.Failure(string.Format("模具{0}编辑失败", moudleInfo.Id));
                }
                if (ReceiveDetailRepository.Update(item) <= 0)
                {
                    return DataProcess.Failure(string.Format("领用明细{0}作废失败", item.ReceiveCode));
                }
            }


            // 更改领用任务状态        
            // 获取领用任务
            var ReceiveTasklList = ReceiveTaskContract.ReceiveTasks.Where(a => a.ReceiveCode == entity.Code).ToList();
            // 循环获取任务判断更改状态，作废领用任务
            foreach (var item in ReceiveTasklList)
            {
                if (item.Status == (int)ReceiveEnum.Finish)
                {
                    return DataProcess.Failure(string.Format("作废失败，领用任务{0}已完成", item.Code));
                }
                else if (item.Status == (int)ReceiveEnum.Proceed)
                {
                    return DataProcess.Failure(string.Format("作废失败，领用任务{0}进行中", item.Code));
                }
                else
                {
                    item.Status = (int?)ReceiveEnum.Cancellation;
                    if(ReceiveTaskRepository.Update(item) <= 0)
                    {
                        return DataProcess.Failure(string.Format("领用任务{0}作废失败", item.Code));
                    }
                }
            }

            // 作废领用任务明细
            var ReceiveTaskDetailList = ReceiveTaskDetailRepository.Query().Where(a => a.ReceiveCode == entity.Code).ToList();
            foreach(var item in ReceiveTaskDetailList)
            {
                if(item.Status != (int?)ReceiveEnum.Wait)
                {
                    return DataProcess.Failure(string.Format("领用任务{0}作废失败", item.ReceiveCode));
                }
                item.Status = (int?)ReceiveEnum.Cancellation;
                if (ReceiveTaskDetailRepository.Update(item) <= 0)
                {
                    return DataProcess.Failure(string.Format("领用任务{0}作废失败", item.ReceiveCode));
                }
            }
            // 改变领用单的状态，作废领用单
            entity.Status = (int)ReceiveEnum.Cancellation;
            if (ReceiveRepository.Update(entity) <= 0)
            {
                return DataProcess.Failure(string.Format("领用单{0}作废失败", entity.Code));
            }
            ReceiveRepository.UnitOfWork.Commit();
            return DataProcess.Success("作废成功");

        }

        /// <summary>
        /// 编辑领用单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditReceive(Receive entity)
        {
            ReceiveRepository.UnitOfWork.TransactionEnabled = true;
            if (ReceiveRepository.Update(entity) <= 0)
            {
                return DataProcess.Failure(string.Format("领用单{0}编辑失败", entity.Code));
            }
            List<ReceiveDetail> list = ReceiveDetails.Where(a => a.ReceiveCode == entity.Code).ToList();

            // 删除原有的明细
            if (list != null && list.Count > 0)
            {
                foreach (ReceiveDetail item in list)
                {
                    DataResult result = RemoveReceiveDetail(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }

            // 添加新的明细
            if (entity.AddMaterial != null && entity.AddMaterial.Count() > 0)
            {
                foreach (ReceiveDetail item in entity.AddMaterial)
                {
                    item.ReceiveCode = entity.Code; // 领用单编码赋值给清单编码
                    item.Status = entity.Status;
                    item.IsDeleted = entity.IsDeleted;
                    DataResult result = CreateInMaterialEntity(item);
                    if (!result.Success)
                    {

                        return DataProcess.Failure(result.Message);
                    }
                    else
                    {
                        // 更新成功则更改模具信息的状态
                        var moudleInfo = MouldInformationContract.MouldInformations.FirstOrDefault(a => a.MaterialLabel == item.MaterialLabel);
                        if (moudleInfo.MouldState == (int)MouldInformationEnum.Employ || moudleInfo.MouldState == (int)MouldInformationEnum.MoldRepair || moudleInfo.MouldState == (int)MouldInformationEnum.WriteOff)
                        {
                            return DataProcess.Failure(string.Format("模具{0}编辑失败", moudleInfo.MaterialLabel));
                        }
                        moudleInfo.MouldState = (int)MouldInformationEnum.MouldLock;
                        if (MouldInformationContract.MouldInformationRepository.Update(moudleInfo) < 0)
                        {
                            return DataProcess.Failure(string.Format("模具{0}编辑失败", moudleInfo.MaterialLabel));
                        }                                               
                    }
                }
            }
            ReceiveRepository.UnitOfWork.Commit();
            return DataProcess.Success("编辑成功");
        }
      
        /// <summary>
        /// 删除领用单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveReceive(int id)
        {
            Receive entity = ReceiveRepository.GetEntity(id);
            if (entity.Status != (int)Enums.ReceiveEnum.Wait)
            {
                return DataProcess.Failure("该领用单执行中或已完成或者已作废");
            }

            ReceiveRepository.UnitOfWork.TransactionEnabled = true;
            if (ReceiveRepository.Delete(id) <= 0)
            {
                return DataProcess.Failure(string.Format("领用单{0}删除失败", entity.Code));
            }
            List<ReceiveDetail> list = ReceiveDetails.Where(a => a.ReceiveCode == entity.Code).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (ReceiveDetail item in list)
                {
                    DataResult result = RemoveReceiveDetail(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }         
            ReceiveRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }

        public DataResult RemoveReceiveDetail(int id)
        {
            ReceiveDetail entity = ReceiveDetailRepository.GetEntity(id);
            if (entity.Status == (int)Enums.ReceiveEnum.Finish || entity.Status == (int)Enums.ReceiveEnum.Proceed)
            {
                return DataProcess.Failure("该领用单执行中或已完成");
            }
            // 更改模具信息状态
            var moudleInfo = MouldInformationContract.MouldInformations.FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel);
            moudleInfo.MouldState = (int)MouldInformationEnum.InWarehouse;
            if (MouldInformationContract.MouldInformationRepository.Update(moudleInfo) < 0)
            {
                return DataProcess.Failure(string.Format("模具{0}编辑失败", moudleInfo.Id));
            }
            if (ReceiveDetailRepository.Delete(id) > 0)
            {              
                return DataProcess.Success(string.Format("领用明细{0}删除成功", entity.MaterialLabel));
            }           
            return DataProcess.Failure("操作失败");
        }
    }
}
