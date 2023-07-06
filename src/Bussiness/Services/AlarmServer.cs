using System;
using System.Collections.Generic;
using Bussiness.Contracts;
using Bussiness.Dtos;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Models;

namespace Bussiness.Services
{
    public class AlarmServer : Contracts.IAlarmContract
    {
        /// <summary>
        /// 库存信息
        /// </summary>
        public IStockContract StockContract { set; get; }
        public IWareHouseContract WareHouseContract { set; get; }
        public IMaterialContract MaterialContract { set; get; }
        public IRepository<Alarm, int> AlarmRepository { get; set; }
        public IRepository<Dictionary,int> DictionaryRepository { get; set; }

        public IQuery<Alarm> Alarms {
            get
            {
                return AlarmRepository.Query();
            }
        }

        public IQuery<AlarmDto> AlarmDtos => Alarms.InnerJoin(StockContract.StockDtos, (alarm, stock)=> alarm.MaterialLabel == stock.MaterialLabel)
            .InnerJoin(WareHouseContract.WareHouses, (alarm, stock, warehouse) => stock.WareHouseCode == warehouse.Code)
            .InnerJoin(MaterialContract.Materials, (alarm, stock, warehouse,material) => stock.MaterialCode == material.Code)
            .Select((alarm, stock, warehouse, material) => new AlarmDto
            {

            Id = alarm.Id,
            MaterialLabel = alarm.MaterialLabel,
            LocationCode=stock.LocationCode,
            ManufactureDate =stock.ManufactureDate,
            MaterialCode=stock.MaterialCode,
            MaterialName= material.Name,
            Quantity =stock.Quantity,
            TrayCode = stock.TrayCode,
            ContainerCode = stock.ContainerCode,
            BatchCode = stock.BatchCode,
            WareHouseCode = stock.WareHouseCode,
            WareHouseName = warehouse.Name,
            CreatedTime = alarm.CreatedTime,
            CreatedUserCode = alarm.CreatedUserCode,
            CreatedUserName = alarm.CreatedUserName,
            UpdatedTime = alarm.UpdatedTime,
            UpdatedUserCode = alarm.UpdatedUserCode,
            UpdatedUserName = alarm.UpdatedUserName,
            Status = alarm.Status,
            ValidityPeriod=material.ValidityPeriod
            });


        public DataResult CreateAlarm(Alarm entity)
        {
            if (Alarms.Any(a=>a.MaterialLabel==entity.MaterialLabel))
            {
                return DataProcess.Failure(string.Format("库存预警编码{0}已存在", entity.MaterialLabel));
            }
            if (AlarmRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("库存预警编码{0}创建成功", entity.MaterialLabel));
            }
            return DataProcess.Failure();
        }

        public DataResult DeleteAlarm(int id)
        {
            if (AlarmRepository.LogicDelete(id)>0)
            {
                return DataProcess.Success("删除成功");
            }
            return DataProcess.Failure();
        }

        public DataResult EditAlarm(Alarm entity)
        {
            if (AlarmRepository.Update(entity)>0)
            {
                return DataProcess.Success(string.Format("库存预警{0}编辑成功", entity.MaterialLabel));
            }
            return DataProcess.Failure();
        }

        // 核查预警信息
        public DataResult UpdateAlarm(Alarm entity)
        {
            try
            {
                if (Alarms.Any(a => a.MaterialLabel == entity.MaterialLabel))
                {
                    if (AlarmRepository.Update(a => new Alarm()
                    {
                        Status = entity.Status

                    }, a => a.MaterialLabel == entity.MaterialLabel) == 0)
                    {
                        return DataProcess.Failure("更新订单信息({0})失败！".FormatWith(entity.MaterialLabel));
                    }
                }
                else
                {

                    if (AlarmRepository.Insert(entity))
                    {
                        return DataProcess.Success(string.Format("库存预警编码{0}创建成功", entity.MaterialLabel));
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return DataProcess.Failure();
        }


        /// <summary>
        /// 核查库存预警--定时任务
        /// </summary>
        /// <returns></returns>
        public DataResult CheckAlarm()
        {
            // 获取提前预警天数
            List<Dictionary> dic = DictionaryRepository.Query().Where(a => a.Code == "WarningDays").ToList();
            int overDay =Convert.ToInt32(dic[0].Value);

            // 当前日期
            DateTime date = DateTime.Now;

            var list = StockContract.StockDtos.ToList();
            foreach (var item in list)
            {
                if (item.ManufactureDate != null)
                {
                    double days = date.Subtract(Convert.ToDateTime(item.ManufactureDate)).TotalDays;

                    // 如果设置了库存有效期，则进行核查
                    if (item.ValidityPeriod > 0)
                    {
                        // 查询的时间（调用当前方法的时刻与生产日期的差值）加提前预警天数大于库存有效期天数进行“即将过期”报警
                        if (days + overDay > item.ValidityPeriod)
                        {
                            var entity = new Alarm()
                            {
                                MaterialLabel = item.MaterialLabel,
                                Status = (int)MaterialStatusCaption.Normal
                            };
                            UpdateAlarm(entity);
                        }

                        /**
                         *  当查询的时间（调用当前方法的时刻与生产日期的差值）加提前预警天数小于库存有效期天数时，
                         *  删除库存预警表中关于此物料的预警信息（此方法是针对修改物料有效期后造成的脏数据）
                         */
                        if (days + overDay < item.ValidityPeriod)
                        {
                            var entity = AlarmRepository.Query().FirstOrDefault(a => a.MaterialLabel == item.MaterialLabel);
                            if (entity != null)
                            {
                                AlarmRepository.Delete(entity);
                            }
                        }
                        // 查询的时间（调用当前方法的时刻与生产日期的差值）大于库存有效期进行“已过期”报警
                        if (days > item.ValidityPeriod)
                        {
                            var entity = new Alarm()
                            {
                                MaterialLabel = item.MaterialLabel,
                                Status = (int)MaterialStatusCaption.Alam
                            };
                            UpdateAlarm(entity);
                        }
                    }
                    else
                    {
                        //当物料的有效期设置为0时，删除库存预警表中关于此物料的预警信息（此方法是针对修改物料有效期后造成的脏数据）
                        var entity = AlarmRepository.Query().FirstOrDefault(a =>a.MaterialLabel == item.MaterialLabel);
                        if (entity != null)
                        {
                            AlarmRepository.Delete(entity);
                        }
                        
                    }
                }
            }
            return DataProcess.Success("库存预警成核查成功");
        }

    }
}
