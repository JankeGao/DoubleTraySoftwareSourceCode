using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Services
{
    public class RecipientsOrdersnServer : Contracts.IRecipientsOrdersContract
    {
        public IRepository<RecipientsOrders, int> RecipientsOrdersRepository { get; set; }

        /// <summary>
        /// 领用清单
        /// </summary>
        public IRepository<ReceiveDetailed, int> ReceiveDetailedRepository { get; set; }
        public IQuery<ReceiveDetailed> ReceiveDetaileds => ReceiveDetailedRepository.Query();
        /// <summary>
        ///视图
        /// </summary>
        public IRepository<Entitys.StockVM, int> StockVMRepository { get; set; }
        public IQuery<StockVM> StockVMs => StockVMRepository.Query();

        /// <summary>
        /// 模具
        /// </summary>
        public IMouldInformationContract MouldInformationContract { set; get; }

        public IQuery<RecipientsOrders> RecipientsOrderss
        {
            get
            {
                return RecipientsOrdersRepository.Query();
            }
        }


        public DataResult RemoveRecipientsOrdersMaterial(int id)
        {
            RecipientsOrders entity = RecipientsOrdersRepository.GetEntity(id);
            if (entity.RecipientsOrdersState != (int)Enums.RecipientsOrdersEnum.Accomplish)
            {
                return DataProcess.Failure("该领用单进行中或已完成");
            }
            if (RecipientsOrdersRepository.Delete(id) > 0)
            {
                return DataProcess.Success(string.Format("领用单{0}删除成功", entity.InCode));
            }
            return DataProcess.Failure("操作失败");
        }
        /// <summary>
        /// 删除领用单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveRecipientsOrders(int id)
        {
            RecipientsOrders entity = RecipientsOrdersRepository.GetEntity(id);
            if (entity.RecipientsOrdersState != (int)Enums.RecipientsOrdersEnum.Accomplish)
            {
                return DataProcess.Failure("该领用单执行中或已完成");
            }

            RecipientsOrdersRepository.UnitOfWork.TransactionEnabled = true;
            if (entity.RecipientsOrdersState != (int)Enums.RecipientsOrdersEnum.Proceed)
            {
                return DataProcess.Failure("该领用单执行中或已完成");
            }
            if (RecipientsOrdersRepository.Delete(id) <= 0)
            {
                return DataProcess.Failure(string.Format("领用单{0}删除失败", entity.Code));
            }
            List<RecipientsOrders> list = RecipientsOrderss.Where(a => a.InCode == entity.Code).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (RecipientsOrders item in list)
                {
                    DataResult result = RemoveRecipientsOrdersMaterial(item.Id);
                    if (!result.Success)
                    {
                        return DataProcess.Failure(result.Message);
                    }
                }
            }
            RecipientsOrdersRepository.UnitOfWork.Commit();
            return DataProcess.Success("操作成功");
        }

        /// <summary>
        /// 联合查询数据库数据
        /// </summary>
        public IQuery<RecipientsOrdersDto> RecipientsOrdersDtoList
        {
            get
            {

                // 领用明细和模具信息
                return ReceiveDetaileds.LeftJoin(MouldInformationContract.MouldInformations, (receiveDetaileds, mouldInformations) => receiveDetaileds.MouldCode == mouldInformations.Code)
                    .LeftJoin(StockVMs, (receiveDetaileds, mouldInformations, stockVMs) => mouldInformations.MaterialLabel == stockVMs.MaterialLabel)
                    .LeftJoin(RecipientsOrderss,(receiveDetaileds, mouldInformations, stockVMs,recipientsOrders)=> receiveDetaileds.InCode == recipientsOrders.InCode)
                    .Select((receiveDetaileds, mouldInformations, stockVMs, recipientsOrders) => new RecipientsOrdersDto()
                    {
                        Id = receiveDetaileds.Id,       
                        InCode = receiveDetaileds.InCode,
                        RecipientsOrdersState = recipientsOrders.RecipientsOrdersState,
                        MaterialLabel = mouldInformations.MaterialLabel,
                        Code = mouldInformations.Code,
                        Remarks = mouldInformations.Remarks,
                        RecipientsOrdersQuantity = recipientsOrders.RecipientsOrdersQuantity,
                        LocationCode = stockVMs.LocationCode,
                        IsDeleted = mouldInformations.IsDeleted,

                    });
            }
        }
    }
}
