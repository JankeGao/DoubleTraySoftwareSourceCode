using System;
using HP.Core.Data;
using Bussiness.Entitys;
using HP.Data.Orm;
using HP.Utility.Data;
using Bussiness.Dtos;
using Bussiness.Contracts;
using HP.Core.Mapping;

namespace Bussiness.Services
{
    class MouldInformationServer : IMouldInformationContract
    {
        public IRepository<MouldInformation, int> MouldInformationRepository { get; set; }

        public IMapper Mapper { set; get; }
        public IQuery<MouldInformation> MouldInformations
        {
            get
            {
                return MouldInformationRepository.Query();
            }
        }

        public IStockContract StockContract { set; get; }

        /// <summary>
        /// 联合查询数据库数据
        /// </summary>
        public IQuery<MouldInformationDto> MouldInformationDtos
        {
            get
            {
                return MouldInformations.InnerJoin(StockContract.StockDtos, (mouldInformation, stockVMs) => mouldInformation.MaterialLabel == stockVMs.MaterialLabel)
                    .Select((mouldInformation, stockVMs) => new MouldInformationDto()
                    {
                        Id = mouldInformation.Id,
                        MaterialCode = stockVMs.MaterialCode,       // 物料编码
                        MaterialName = stockVMs.MaterialName,       // 模具名称
                        MaterialLabel = mouldInformation.MaterialLabel, //模具编码                       
                        MaterialType =stockVMs.MaterialType,       //  物料种类
                        MouldState = mouldInformation.MouldState,
                        ReceiveType= mouldInformation.ReceiveType,
                        RecipientsCode= mouldInformation.RecipientsCode,
                        LocationCode = stockVMs.LocationCode,
                        LastTimeReceiveName = mouldInformation.LastTimeReceiveName,         
                        LastTimeReceiveDatetime = mouldInformation.LastTimeReceiveDatetime,
                        ReceiveTime = mouldInformation.ReceiveTime,
                        LastTimeReturnName = mouldInformation.LastTimeReturnName,
                        LastTimeReturnDatetime = mouldInformation.LastTimeReturnDatetime,
                        Remarks = mouldInformation.Remarks,
                        Quantity= stockVMs.Quantity,
                        WareHouseCode = stockVMs.WareHouseCode,
                        ContainerCode= stockVMs.ContainerCode,
                        TrayCode= stockVMs.TrayCode,
                        IsDeleted = mouldInformation.IsDeleted,
                    });
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateMouldInformation(MouldInformation entity)
        {
            if (MouldInformations.Any(a => a.MaterialLabel == entity.MaterialLabel))
            {
                return DataProcess.Failure(string.Format("模具信息的编码{0}已存在", entity.MaterialLabel));
            }
            if (MouldInformationRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("模具信息0}创建成功", entity.MaterialLabel));
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult DeleteMouldInformation(int id)
        {
            if (MouldInformationRepository.LogicDelete(id) > 0)
            {
                return DataProcess.Success("删除成功");
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditMouldInformation(MouldInformation OneEntity)
        {
            MouldInformation entity = Mapper.MapTo<MouldInformation>(OneEntity);
            entity.CreatedTime = DateTime.Now;
           
            if (MouldInformationRepository.Update(entity) > 0)
            {
                return DataProcess.Success(string.Format("模具{0}编辑成功", entity.Id));
            }
            return DataProcess.Failure();
        }
    }
}
