using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Services
{
    public class NumAlarmServer : Contracts.INumAlarmContract
    {
        /// <summary>
        /// 库存信息
        /// </summary>
        //public IStockContract StockContract { set; get; }
        
        /// <summary>
        /// 仓库信息
        /// </summary>
        public IWareHouseContract WareHouseContract { set; get; }

        public IInContract InContract { set; get; }

        /// <summary>
        /// 物料信息
        /// </summary>
        public IMaterialContract MaterialContract { set; get; }
        public IRepository<NumAlarm, int> NumAlarmRepository { set; get; }
        /// <summary>
        /// 仓库信息
        /// </summary>
        public Bussiness.Contracts.IStockContract StockContract { set; get; }

        public IQuery<NumAlarm> NumAlarms
        {
            get { return NumAlarmRepository.Query(); }
            
        }

        public IQuery<NumAlarmDto> NumAlarmDtos => NumAlarms
            .InnerJoin(MaterialContract.Materials, (numAlarm, material) => numAlarm.MaterialCode == material.Code)
            .LeftJoin(WareHouseContract.LocationVMs, (numAlarm, material, location) => numAlarm.MaterialCode == location.SuggestMaterialCode)
            .LeftJoin(StockContract.StockDtos, (numAlarm, material, location, stock) => numAlarm.MaterialCode == stock.MaterialCode)
            .Select((numAlarm, material, location, stock) => new NumAlarmDto()
            {
                Id = numAlarm.Id,
                MinNum = material.MinNum,
                MaxNum = material.MaxNum,
                Quantityes = stock.Quantity.ToString() ?? "0",
                MaterialCode = material.Code,
                MaterialName = material.Name,
                Status = numAlarm.Status,
                CreatedTime = numAlarm.CreatedTime,
                CreatedUserCode = numAlarm.CreatedUserCode,
                CreatedUserName = numAlarm.CreatedUserName,
                UpdatedTime = numAlarm.UpdatedTime,
                UpdatedUserCode = numAlarm.UpdatedUserCode,
                UpdatedUserName = numAlarm.UpdatedUserName,
                ContainerCode = location.ContainerCode,
                LocationCode = location.Code
            });

        /// <summary>
        /// 更新/插入库存物料上下限预警信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult UpdateNumAlarm(NumAlarm entity)
        {
            try
            {
                if (NumAlarms.Any(a => a.MaterialCode == entity.MaterialCode))
                {
                    if (NumAlarmRepository.Update(a =>new NumAlarm(){ Status = entity.Status}, a => a.MaterialCode == entity.MaterialCode ) == 0)
                    {
                        return DataProcess.Failure(string.Format("更新物料{0}(编码)库存预警状态失败！", entity.MaterialCode));
                    }
                }
                else
                {
                    if (NumAlarmRepository.Insert(entity))
                    {
                        return DataProcess.Success(string.Format("更新物料{0}(编码)库存预警状态成功！", entity.MaterialCode));
                    }
                }
            }
            catch (Exception e)
            {
                return DataProcess.Failure(e.Message);
            }

            return DataProcess.Failure("更新失败！");
        }

        /// <summary>
        /// 核查物料库存上下限
        /// </summary>
        /// <returns></returns>
        public DataResult CheckNumAlarm()
        {
            //获取未被删除的物料信息
            string sql = "select c.MaterialCode,case when c.quantity = c.MinNum then 0 when c.quantity = c.MaxNum then 1 when c.quantity < c.MinNum then 2  when c.quantity > c.MinNum then 3 end as Status from (SELECT * FROM(SELECT a.Code as MaterialCode, isnull(b.Quantity, 0) Quantity, a.MaxNum, a.MinNum FROM TB_WMS_MATERIAL A LEFT JOIN(SELECT  MATERIALCODE, SUM(Quantity) Quantity FROM TB_WMS_STOCK  group by MaterialCode) B ON A.Code = B.MaterialCode)  D where(D.Quantity >= D.MaxNum or D.Quantity <= D.MinNum) and D.MINNUM > 0) C";
            var list = NumAlarmRepository.SqlQuery(sql).ToList();
            NumAlarmRepository.UnitOfWork.TransactionEnabled = true;
            NumAlarmRepository.Delete(a => 1 == 1);
            if (list != null || list.Count > 0)
            {
                foreach (var item in list)
                {
                    NumAlarmRepository.Insert(item);
                }
            }
            NumAlarmRepository.UnitOfWork.Commit();

            //List<Material> materials = MaterialContract.Materials.Where(a => a.IsDeleted == false).ToList();
            //foreach (var material in materials)
            //{
            //    //判断物料是否设置了库存上下限
            //    if (material.MaxNum != 0 || material.MinNum != 0)
            //    {
            //        decimal num = StockContract.Stocks.Where(d => d.MaterialCode == material.Code).Sum(a => a.Quantity);
            //        // var stocks = 
            //        if (num > 0)
            //        {
            //            //获取库存上限
            //            var max = material.MaxNum;

            //            //获取库存下限
            //            var min = material.MinNum;

            //            //判断库存数量是否达到上限
            //            if (num == max)
            //            {
            //                var entity = new NumAlarm()
            //                {
            //                    MaterialCode = material.Code,
            //                    Status = (int)MaterialNumStatusCaption.ReachedMax
            //                };
            //                UpdateNumAlarm(entity);
            //            }

            //            //判断库存数量是否超过上限
            //            if (num > max)
            //            {
            //                var entity = new NumAlarm()
            //                {
            //                    MaterialCode = material.Code,
            //                    Status = (int)MaterialNumStatusCaption.OverMax
            //                };
            //                UpdateNumAlarm(entity);
            //            }

            //            //判断库存数量是否达到下限
            //            if (num == min)
            //            {
            //                var entity = new NumAlarm()
            //                {
            //                    MaterialCode = material.Code,
            //                    Status = (int)MaterialNumStatusCaption.ReachedMin
            //                };
            //                UpdateNumAlarm(entity);
            //            }

            //            //判断库存数量是否低于下限
            //            if (num < min)
            //            {
            //                var entity = new NumAlarm()
            //                {
            //                    MaterialCode = material.Code,
            //                    Status = (int)MaterialNumStatusCaption.OverMin
            //                };
            //                UpdateNumAlarm(entity);
            //            }

            //            //判断库存数量是否在上限与下限之间，主要针对修改库存上下限后造成的脏数据，将其清除
            //            if (num > min && num < max)
            //            {
            //                var entity = NumAlarms.FirstOrDefault(a => a.MaterialCode == material.Code);
            //                if (entity != null)
            //                {
            //                    NumAlarmRepository.Delete(entity);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            var entity = new NumAlarm()
            //            {
            //                MaterialCode = material.Code,
            //                Status = (int)MaterialNumStatusCaption.OverMin
            //            };
            //            UpdateNumAlarm(entity);
            //        }
            //    }
            //    else
            //    {
            //        //移除预警信息
            //        var entity = NumAlarms.FirstOrDefault(a =>
            //            a.MaterialCode == StockContract.StockDtos.FirstOrDefault(b => b.MaterialCode == material.Code)
            //                .MaterialCode);
            //        if (entity != null)
            //        {
            //            NumAlarmRepository.Delete(entity);
            //        }
            //    }
            //}
            return DataProcess.Success();
        }
    }
}