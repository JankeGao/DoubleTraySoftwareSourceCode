using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Core.Mapping;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;
using HPC.BaseService.Contracts;

namespace Bussiness.Services
{
    public class ReceiveTaskServer : Contracts.IReceiveTaskContract
    {

        /// <summary>
        /// 自动生成编码
        /// </summary>
        public ISequenceContract SequenceContract { set; get; }

        /// <summary>
        /// 领用单
        /// </summary>
        public Bussiness.Contracts.IWareHouseContract WareHouseContract { get; set; }

        /// <summary>
        /// 领用单
        /// </summary>
        public Bussiness.Contracts.IReceiveContract ReceiveContract { get; set; }

        /// <summary>
        /// 领用单
        /// </summary>
        public Bussiness.Contracts.IMaterialContract MaterialContract { get; set; }

        /// <summary>
        /// 模具信息
        /// </summary>
        public Bussiness.Contracts.IMouldInformationContract MouldInformationContract { get; set; }

        /// <summary>
        /// 领用任务
        /// </summary>
        public IRepository<ReceiveTask, int> ReceiveTaskRepository { get; set; }
        /// <summary>
        /// 任务明细
        /// </summary>
        public IRepository<ReceiveTaskDetail, int> ReceiveTaskDetailRepository { get; set; }

        /// <summary>
        /// 历史领用信息
        /// </summary>
        public IRepository<ReceiveTaskHistory, int> ReceiveTaskHistoryRepository { get; set; }
        public IQuery<ReceiveTaskDetail> ReceiveTaskDetails => ReceiveTaskDetailRepository.Query();

        public IRepository<Entitys.StockVM, int> StockVMRepository { get; set; }
        public IQuery<StockVM> StockVMs => StockVMRepository.Query();
        public IMapper Mapper { set; get; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public IIdentityContract IdentityContract { get; set; }

        public IQuery<ReceiveTask> ReceiveTasks
        {
            get
            {
                return ReceiveTaskRepository.Query();
            }
        }

        /// <summary>
        /// 领用任务单信息
        /// </summary>
        public IQuery<ReceiveTaskDto> ReceiveTaskDtos
        {
            get
            {
                return ReceiveTasks.LeftJoin(IdentityContract.Users, (receiveTasks, user) => receiveTasks.LastTimeReceiveName == user.Code)                  
                    .Select((receiveTasks, user) => new ReceiveTaskDto()
                    {
                        Id = receiveTasks.Id,
                        Code = receiveTasks.Code,
                        ReceiveCode = receiveTasks.ReceiveCode,
                        Status = receiveTasks.Status,
                        ReceiveType = receiveTasks.ReceiveType,
                        ReceiveTime = receiveTasks.ReceiveTime,
                        ContainerCode = receiveTasks.ContainerCode,
                        LastTimeReceiveDatetime = receiveTasks.LastTimeReceiveDatetime,
                        LastTimeReceiveName = receiveTasks.LastTimeReceiveName,
                        LastTimeReturnDatetime = receiveTasks.LastTimeReturnDatetime,
                        LastTimeReturnName = receiveTasks.LastTimeReturnName,
                        PredictReturnTime = receiveTasks.PredictReturnTime,
                        WareHouseCode = receiveTasks.WareHouseCode,
                        Remarks = receiveTasks.Remarks,
                        CreatedTime = receiveTasks.CreatedTime,
                        IsDeleted = receiveTasks.IsDeleted,
                        ReceiveUserName = user.Name,
                    });
            }
        }

        /// <summary>
        /// 联合查询数据库数据任务明细
        /// </summary>
        public IQuery<ReceiveTaskDetailDto> ReceiveTaskDetailDtos
        {
            get
            {  
                // 领用明细和模具信息
                return ReceiveTaskDetails.LeftJoin(StockVMs, (receiveTaskDetails, stockVM) => receiveTaskDetails.MaterialLabel == stockVM.MaterialLabel)
                    .InnerJoin(WareHouseContract.LocationVMs, (receiveTaskDetails, stockVM, location) => stockVM.LocationCode == location.Code)
                    .InnerJoin(MaterialContract.Materials, (receiveTaskDetails, stockVM, location,material) => stockVM.MaterialCode == material.Code)
                    .LeftJoin(IdentityContract.Users, (receiveTaskDetails, stockVM, location,material,user) => receiveTaskDetails.LastTimeReturnName == user.Code)
                    .Select((receiveTaskDetails, stockVM, location, material,user) => new ReceiveTaskDetailDto()
                    {
                        Id = receiveTaskDetails.Id,
                        TaskCode = receiveTaskDetails.TaskCode,
                        ReceiveCode = receiveTaskDetails.ReceiveCode,
                        Status = receiveTaskDetails.Status,
                        MaterialName = stockVM.MaterialName,
                        MaterialCode = stockVM.MaterialCode,
                        LocationCode = stockVM.LocationCode,
                        MaterialType = stockVM.MaterialType,
                        MaterialLabel = stockVM.MaterialLabel,
                        ReceiveType = receiveTaskDetails.ReceiveType,
                        LastTimeReceiveDatetime = receiveTaskDetails.LastTimeReceiveDatetime,
                        ReceiveTime = receiveTaskDetails.ReceiveTime,
                        LastTimeReceiveName = receiveTaskDetails.LastTimeReceiveName,
                        LastTimeReturnDatetime = receiveTaskDetails.LastTimeReturnDatetime,
                        LastTimeReturnName = receiveTaskDetails.LastTimeReturnName,
                        PredictReturnTime = receiveTaskDetails.PredictReturnTime,
                        ContainerCode = stockVM.ContainerCode,
                        WareHouseCode = receiveTaskDetails.WareHouseCode,
                        MouldRemarks = stockVM.MaterialName,
                        ReturnUserName = user.Name,
                        Quantity = stockVM.Quantity,
                        CreatedTime = receiveTaskDetails.CreatedTime,
                        XLight = location.XLight,
                        YLight = location.YLight,
                        TrayCode=location.TrayCode,
                        BoxName= location.BoxName,
                        BoxUrl=location.BoxUrl,
                        MaterialUrl= material.PictureUrl,
                        TrayId= location.TrayId
                    });
            }
        }


        /// <summary>
        /// 联合查询数据库数据任务明细
        /// </summary>
        public IQuery<ReceiveTaskDetailDto> ReceiveHistoryDetailDtos
        {
            get
            {
                // 领用明细和模具信息
                return ReceiveTaskHistoryRepository.Query().LeftJoin(StockVMs, (receiveTaskDetails, stockVM) => receiveTaskDetails.MaterialLabel == stockVM.MaterialLabel)
                    .InnerJoin(WareHouseContract.LocationVMs, (receiveTaskDetails, stockVM, location) => stockVM.LocationCode == location.Code)
                    .InnerJoin(MaterialContract.Materials, (receiveTaskDetails, stockVM, location, material) => stockVM.MaterialCode == material.Code)
                    .LeftJoin(IdentityContract.Users, (receiveTaskDetails, stockVM, location, material, user) => receiveTaskDetails.LastTimeReturnName == user.Code)
                    .Select((receiveTaskDetails, stockVM, location, material, user) => new ReceiveTaskDetailDto()
                    {
                        Id = receiveTaskDetails.Id,
                        TaskCode = receiveTaskDetails.TaskCode,
                        ReceiveCode = receiveTaskDetails.ReceiveCode,
                        Status = receiveTaskDetails.Status,
                        MaterialName = stockVM.MaterialName,
                        MaterialCode = stockVM.MaterialCode,
                        LocationCode = stockVM.LocationCode,
                        MaterialType = stockVM.MaterialType,
                        MaterialLabel = stockVM.MaterialLabel,
                        ReceiveType = receiveTaskDetails.ReceiveType,
                        LastTimeReceiveDatetime = receiveTaskDetails.LastTimeReceiveDatetime,
                        ReceiveTime = receiveTaskDetails.ReceiveTime,
                        LastTimeReceiveName = receiveTaskDetails.LastTimeReceiveName,
                        LastTimeReturnDatetime = receiveTaskDetails.LastTimeReturnDatetime,
                        LastTimeReturnName = receiveTaskDetails.LastTimeReturnName,
                        PredictReturnTime = receiveTaskDetails.PredictReturnTime,
                        ContainerCode = stockVM.ContainerCode,
                        WareHouseCode = receiveTaskDetails.WareHouseCode,
                        MouldRemarks = stockVM.MaterialName,
                        ReturnUserName = user.Name,
                        Quantity = stockVM.Quantity,
                        CreatedTime = receiveTaskDetails.CreatedTime,
                        XLight = location.XLight,
                        YLight = location.YLight,
                        TrayCode = location.TrayCode,
                        BoxName = location.BoxName,
                        BoxUrl = location.BoxUrl,
                        MaterialUrl = material.PictureUrl,
                        TrayId = location.TrayId
                    });
            }
        }

        /// <summary>
        ///  添加领用任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateReceiveTaskEntity(Receive entity)
        {
            // 事务
            ReceiveTaskRepository.UnitOfWork.TransactionEnabled = true;
            {
                // 获取领用单
                var ReceiveEntity = ReceiveContract.Receives.FirstOrDefault(a => a.Code == entity.Code);
                // 获取领用单明细
                var ReceiveDetailList = ReceiveContract.ReceiveDetailDtos.Where(a => a.ReceiveCode == ReceiveEntity.Code).ToList();
                // 领用单任务明细表
                var ReceiveTaskMaterialList = new List<ReceiveTaskDetail>();

                foreach (var item in ReceiveDetailList)
                {
                    var ReceiveTaskMaterialItem = new ReceiveTaskDetail()
                    {
                        WareHouseCode = item.WareHouseCode,
                        Remarks = entity.Remarks,
                        ReceiveCode = item.ReceiveCode,
                        Status = item.Status,
                        ReceiveType = item.ReceiveType,
                        ContainerCode = item.ContainerCode,
                        MaterialLabel = item.MaterialLabel,
                        LocationCode= item.LocationCode,
                        PredictReturnTime = item.PredictReturnTime,
                        ReceiveTime = item.ReceiveTime,
                    };
                    ReceiveTaskMaterialList.Add(ReceiveTaskMaterialItem);
                }

                if (ReceiveTaskMaterialList.Count > 0)
                {
                    var groupList = ReceiveTaskMaterialList.GroupBy(a => new { a.WareHouseCode, a.ContainerCode });

                    foreach (var item in groupList)
                    {
                        ReceiveTaskDetail temp = item.FirstOrDefault();

                        var inTask = new ReceiveTask()
                        {
                            ReceiveCode = temp.ReceiveCode,
                            Remarks = temp.Remarks,
                            WareHouseCode = temp.WareHouseCode,
                            ContainerCode = temp.ContainerCode,
                            Status = temp.Status,
                            IsDeleted = temp.IsDeleted,
                            ReceiveType = temp.ReceiveType,
                            PredictReturnTime = ReceiveEntity.PredictReturnTime,
                            LastTimeReceiveName = ReceiveEntity.LastTimeReceiveName,
                            LastTimeReceiveDatetime = ReceiveEntity.LastTimeReceiveDatetime
                        };
                        // 生成编码
                        inTask.Code = SequenceContract.Create(inTask.GetType());

                        // 增加任务明细
                        foreach (ReceiveTaskDetail inTaskMaterial in item)
                        {
                            inTaskMaterial.TaskCode = inTask.Code;
                            if (!ReceiveTaskDetailRepository.Insert(inTaskMaterial))
                            {
                                return DataProcess.Failure(string.Format("领用任务明细{0}新增失败", entity.Code));
                            }
                        }

                        if (!ReceiveTaskRepository.Insert(inTask))
                        {
                            return DataProcess.Failure(string.Format("领用任务单{0}下发失败", entity.Code));
                        }
                    }
                    // 下发完成更改领用单状态                 
                    ReceiveEntity.Status = (int)ReceiveTaskEnum.Proceed;
                    if (ReceiveContract.ReceiveRepository.Update(ReceiveEntity) < 0)
                    {
                        return DataProcess.Failure(string.Format("领用单{0}编辑失败", ReceiveEntity.Id));
                    }
                    foreach (var item in ReceiveDetailList)
                    {
                        item.Status = (int)ReceiveTaskEnum.Proceed;
                        var updateItem = Mapper.MapTo<ReceiveDetail>(item);
                        if (ReceiveContract.ReceiveDetailRepository.Update(updateItem) < 0)
                        {
                            return DataProcess.Failure(string.Format("领用明细{0}编辑失败", item.ReceiveCode));
                        }
                    }
                }
            }
            ReceiveTaskRepository.UnitOfWork.Commit();

            return DataProcess.Success(string.Format("领用任务单{0}下发成功", entity.Code));

        }


        /// <summary>
        /// 删除领用任务单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveReceiveTask(int id)
        {
            ReceiveTask entity = ReceiveTaskRepository.GetEntity(id);
            if (entity.Status != (int)Enums.ReceiveTaskEnum.Wait)
            {
                return DataProcess.Failure("该领用任务单执行中或已完成");
            }

            ReceiveTaskRepository.UnitOfWork.TransactionEnabled = true;
            if (ReceiveTaskRepository.Delete(id) <= 0)
            {
                return DataProcess.Failure(string.Format("领用任务单{0}删除失败", entity.Code));
            }
            List<ReceiveTaskDetail> list = ReceiveTaskDetails.Where(a => a.ReceiveCode == entity.Code).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (ReceiveTaskDetail item in list)
                {
                    DataResult result = RemoveReceiveDetail(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }
            ReceiveTaskRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }

        /// <summary>
        /// 移除领用单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveReceiveDetail(int id)
        {
            ReceiveTaskDetail entity = ReceiveTaskDetailRepository.GetEntity(id);
            if (entity.Status != (int)Enums.ReceiveTaskEnum.Wait)
            {
                return DataProcess.Failure("该领用任务单执行中或已完成");
            }
            if (ReceiveTaskDetailRepository.Delete(id) > 0)
            {
                return DataProcess.Success(string.Format("领用任务明细{0}删除成功", entity.MaterialLabel));
            }
            return DataProcess.Failure("操作失败");
        }

        /// <summary>
        /// 领用单任务执行
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult HandShelfReceiveTask(ReceiveTaskDetail entity)
        {
            ReceiveTaskDetailRepository.UnitOfWork.TransactionEnabled = true;

            // 领用任务明细实体
            var receiveTaskDetailEntity = entity;
            if(receiveTaskDetailEntity.Status != (int)ReceiveTaskEnum.Wait)
            {
                receiveTaskDetailEntity.UpdatedTime = DateTime.Now;
                return DataProcess.Failure("该领用单任务明细单状态不为待执行");
            }

            receiveTaskDetailEntity.Status = (int)ReceiveTaskEnum.Proceed;
            receiveTaskDetailEntity.LastTimeReceiveDatetime = DateTime.Now;
            receiveTaskDetailEntity.LastTimeReceiveName= HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
            // 更新领用任务明细
            ReceiveTaskDetailRepository.Update(receiveTaskDetailEntity);

            //#region 领用历史

            //var history = Mapper.MapTo<ReceiveTaskHistory>(receiveTaskDetailEntity);
            //// 更新领用明细
            //ReceiveTaskHistoryRepository.Insert(history);

            //#endregion

            // 领用任务实体
            ReceiveTask receiveTaskEntity = ReceiveTasks.FirstOrDefault(a => a.Code == entity.TaskCode);
            if(receiveTaskEntity.Status == (int)ReceiveTaskEnum.Finish || receiveTaskEntity.Status == (int)ReceiveTaskEnum.Cancellation)
            {
                receiveTaskEntity.UpdatedTime = DateTime.Now;
                return DataProcess.Failure("该领用单任务已完成");
            }
            // 如果是注销，则执行后是已完成状态
            if(receiveTaskEntity.ReceiveType == 2)
            {
                receiveTaskEntity.Status = (int)ReceiveTaskEnum.Finish;
            }
            else
            {
                receiveTaskEntity.Status = (int)ReceiveTaskEnum.Proceed;
            }
           
            // 更新领用任务
            ReceiveTaskRepository.Update(receiveTaskEntity);

            // 领用单明细实体
            ReceiveDetailDto RrceiveDetailDto = ReceiveContract.ReceiveDetailDtos.FirstOrDefault(a => a.ReceiveCode == receiveTaskDetailEntity.ReceiveCode);
            ReceiveDetail receiveDetailEntity = Mapper.MapTo<ReceiveDetail>(RrceiveDetailDto);
            //if (receiveDetailEntity.Status == (int)ReceiveTaskEnum.Finish || receiveTaskEntity.Status == (int)ReceiveTaskEnum.Cancellation)
            //{
            //    return DataProcess.Failure("该领用单已完成");
            //}
            receiveDetailEntity.Status = (int)ReceiveTaskEnum.Proceed;


            // 更新领用明细
            ReceiveContract.ReceiveDetailRepository.Update(receiveDetailEntity);


            // 领用单实体
            Receive receiveEntity = ReceiveContract.Receives.FirstOrDefault(a => a.Code == receiveTaskEntity.ReceiveCode);
            //if (receiveEntity.Status == (int)ReceiveTaskEnum.Finish || receiveTaskEntity.Status == (int)ReceiveTaskEnum.Cancellation)
            //{
            //    return DataProcess.Failure("该领用单已完成");
            //}
            receiveEntity.Status = (int)ReceiveTaskEnum.Proceed;
            // 如果为三方系统同步创建
            if (receiveEntity.OrderType == (int)OrderTypeEnum.Other)
            {
                // 查询该中间表实体
                //var outMaterialIFEntity = OutContract.OutMaterialIFRepository.Query().FirstOrDefault(a => a.BillCode == outEntity.BillCode);
                //if (outMaterialIFEntity.Status == (int)OrderEnum.Wait)
                //{
                //    outMaterialIFEntity.Status = (int)OrderEnum.Finish;
                //    outMaterialIFEntity.RealPickedQuantity = item.PickedQuantity.GetValueOrDefault(0);
                //    OutContract.OutMaterialIFRepository.Update(outMaterialIFEntity);
                //}
            }
            // 更新领用单
            ReceiveContract.ReceiveRepository.Update(receiveEntity);


            ReceiveTaskDetailRepository.UnitOfWork.Commit();
            return DataProcess.Success("执行成功");
        }

        /// <summary>
        /// 归还
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult ReturnReceiveTask(ReceiveTaskDetail entity)
        {
            ReceiveTaskDetailRepository.UnitOfWork.TransactionEnabled = true;

            if(entity.LastTimeReturnDatetime == null )
            {
                entity.LastTimeReturnDatetime = DateTime.Now;
            }
            if(string.IsNullOrEmpty(entity.LastTimeReturnName))
            {
                entity.LastTimeReturnName =  HP.Core.Security.Permissions.IdentityManager.Identity.UserData.Code;
            }

            // 领用任务明细实体
            if (entity.Status != (int)ReceiveTaskEnum.Proceed)
            {
                return DataProcess.Failure(string.Format("领用任务明细状态不符合，不可归还"));
            }


            #region 任务单明细

            // 更新时间
            entity.UpdatedTime = DateTime.Now;
            // 领用任务明细状态
            entity.Status = (int)ReceiveTaskEnum.Finish;

            // 更新领用任务明细        
            if (entity.LastTimeReturnDatetime != null && entity.LastTimeReceiveDatetime != null)
            {
                TimeSpan ts = ((DateTime)entity.LastTimeReturnDatetime) - ((DateTime)entity.LastTimeReceiveDatetime);
                var Dates = ts.Days;
                var hour = ts.Hours;
                var minute = ts.Minutes;
                entity.ReceiveTime = (((Dates * 24 + hour) * 60) + minute);
            }

            ReceiveTaskDetailRepository.Update(entity);

            try
            {

                var history = Mapper.MapTo<ReceiveTaskHistory>(entity);
                // 更新领用明细
                ReceiveTaskHistoryRepository.Insert(history);
            }
            catch (Exception ex)
            {

            }
            #region 领用历史


            #endregion

            #endregion


            #region 任务单

            // 领用任务实体
            ReceiveTask receiveTaskEntity = ReceiveTasks.FirstOrDefault(a => a.Code == entity.TaskCode);

            // 根据任务查询任务明细是否都是已完成，更新领用任务信息
            if (ReceiveTaskDetailDtos.Where(a => a.TaskCode == receiveTaskEntity.Code).Any(a => a.Status != (int)ReceiveTaskEnum.Finish))
            {
                receiveTaskEntity.Status = receiveTaskEntity.Status;
            }
            else
            {
                receiveTaskEntity.Status = (int)ReceiveTaskEnum.Finish;
                if (receiveTaskEntity.LastTimeReceiveDatetime != null)
                {
                    TimeSpan ts = DateTime.Now - ((DateTime)receiveTaskEntity.LastTimeReceiveDatetime);
                    var Dates = ts.Days;
                    var hour = ts.Hours;
                    var minute = ts.Minutes;
                    receiveTaskEntity.ReceiveTime = (((Dates * 24 + hour) * 60) + minute);
                }
            }
            // 更新领用任务
            receiveTaskEntity.LastTimeReturnName = entity.LastTimeReturnName;
            receiveTaskEntity.LastTimeReturnDatetime = entity.LastTimeReturnDatetime;
            ReceiveTaskRepository.Update(receiveTaskEntity);
            #endregion


            #region 领用单明细

            // 领用单明细实体
            ReceiveDetailDto RrceiveDetailDto = ReceiveContract.ReceiveDetailDtos.FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel && a.ReceiveCode == receiveTaskEntity.ReceiveCode);
            ReceiveDetail receiveDetailEntity = Mapper.MapTo<ReceiveDetail>(RrceiveDetailDto);

            // 领用单明细状态
            receiveDetailEntity.Status = (int)ReceiveEnum.Finish;

            // 更新领用单明细
            receiveDetailEntity.LastTimeReturnName = entity.LastTimeReturnName;
            receiveDetailEntity.LastTimeReturnDatetime = entity.LastTimeReturnDatetime;
            if (receiveDetailEntity.LastTimeReturnDatetime != null && receiveDetailEntity.LastTimeReceiveDatetime != null)
            {
                TimeSpan ts = ((DateTime)receiveDetailEntity.LastTimeReturnDatetime) - ((DateTime)receiveDetailEntity.LastTimeReceiveDatetime);
                var Dates = ts.Days;
                var hour = ts.Hours;
                var minute = ts.Minutes;
                receiveDetailEntity.ReceiveTime = (((Dates * 24 + hour) * 60) + minute);

            }
            ReceiveContract.ReceiveDetailRepository.Update(receiveDetailEntity);

            #endregion


            #region 领用单

            // 领用单实体
            Receive receiveEntity = ReceiveContract.Receives.FirstOrDefault(a => a.Code == receiveTaskEntity.ReceiveCode);

            // 根据领用任务是否都是已完成，更改领用单状态释放模具。
            if (ReceiveTasks.Where(a => a.ReceiveCode == receiveEntity.Code).Any(a => a.Status != (int)ReceiveTaskEnum.Finish))
            {
                receiveEntity.Status = (int)ReceiveEnum.Proceed;
            }
            else
            {
                receiveEntity.Status = (int)ReceiveEnum.Finish;
                if (receiveEntity.LastTimeReceiveDatetime != null)
                {
                    TimeSpan ts = DateTime.Now - ((DateTime)receiveEntity.LastTimeReceiveDatetime);
                    var Dates = ts.Days;
                    var hour = ts.Hours;
                    var minute = ts.Minutes;
                    receiveEntity.ReceiveTime = (((Dates * 24 + hour) * 60) + minute);
                }            
            }
            // 更新领用单                 
            ReceiveContract.ReceiveRepository.Update(receiveEntity);

            #endregion


            #region 模具信息

            // 模具信息实体          
            MouldInformation mouldInformationEntity = MouldInformationContract.MouldInformations.FirstOrDefault(a => a.MaterialLabel == entity.MaterialLabel);
            if(receiveTaskEntity.ReceiveType == (int)ReceiveTypeEnum.Logout)
            {
                mouldInformationEntity.MouldState = (int)MouldInformationEnum.WriteOff;
                mouldInformationEntity.LastTimeReturnName = IdentityContract.Users.FirstOrDefault(a => a.Code == entity.LastTimeReturnName).Name;
                mouldInformationEntity.LastTimeReturnDatetime = entity.LastTimeReturnDatetime;
                if (MouldInformationContract.MouldInformationRepository.Update(mouldInformationEntity) <= 0)
                {
                    return DataProcess.Failure("模具状态编辑失败。");
                }
            }
            else
            {
                mouldInformationEntity.MouldState = (int)MouldInformationEnum.InWarehouse;
                mouldInformationEntity.ReceiveType = entity.ReceiveType;
                mouldInformationEntity.LastTimeReturnName = IdentityContract.Users.FirstOrDefault(a => a.Code == entity.LastTimeReturnName).Name;
                mouldInformationEntity.LastTimeReturnDatetime = entity.LastTimeReturnDatetime;
                mouldInformationEntity.Remarks = receiveEntity.Remarks;
                mouldInformationEntity.ReceiveTime = receiveEntity.ReceiveTime;
                if (MouldInformationContract.MouldInformationRepository.Update(mouldInformationEntity) <= 0)
                {
                    return DataProcess.Failure("模具状态编辑失败。");
                }
            }
            #endregion

            ReceiveTaskDetailRepository.UnitOfWork.Commit();
            return DataProcess.Success("执行成功");
        }
    }
}
