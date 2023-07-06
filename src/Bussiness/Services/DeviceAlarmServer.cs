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
    public class DeviceAlarmServer : Contracts.IDeviceAlarmContract
    {
        /// <summary>
        /// 仓库
        /// </summary>
        public IWareHouseContract WareHouseContract { set; get; }

        public IRepository<DeviceAlarm, int> DeviceAlarmRepository { get; set; }


        public IQuery<DeviceAlarm> DeviceAlarms
        {
            get
            {
                return DeviceAlarmRepository.Query();
            }
        }

        /// <summary>
        /// 联合查询数据库数据
        /// </summary>
        public IQuery<DeviceAlarmDto> DeviceAlarmDtos
        {
            get
            {
                return DeviceAlarms.LeftJoin(WareHouseContract.Containers, (deviceAlarm,container) => container.Code == deviceAlarm.ContainerCode)
                    .LeftJoin(WareHouseContract.WareHouses, (deviceAlarm, container, warehouse) => container.WareHouseCode == warehouse.Code)
                    .Select((deviceAlarm, container, warehouse) => new DeviceAlarmDto()
                    {
                        Id = deviceAlarm.Id,
                        Code = deviceAlarm.Code,
                        ContainerCode = container.Code,
                        CreatedTime = deviceAlarm.CreatedTime,
                        CreatedUserCode = deviceAlarm.CreatedUserCode,
                        ContinueTime = deviceAlarm.ContinueTime,
                        CreatedUserName = deviceAlarm.CreatedUserName,
                        UpdatedTime = deviceAlarm.UpdatedTime,
                        UpdatedUserCode = deviceAlarm.UpdatedUserCode,
                        UpdatedUserName = deviceAlarm.UpdatedUserName,
                        Status = deviceAlarm.Status,
                        WarehouseName = warehouse.Name,
                        AlarmStatus = deviceAlarm.AlarmStatus,
                    });
            }
        }


        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateDeviceAlarm(DeviceAlarm entity)
        {
            if (DeviceAlarms.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure(string.Format("该设备报警的编码{0}已存在", entity.Code));
            }
            if (DeviceAlarmRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("设备报警编码{0}创建成功", entity.Code));
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult DeleteDeviceAlarm(int id)
        {
            if (DeviceAlarmRepository.LogicDelete(id) > 0)
            {
                return DataProcess.Success("删除成功");
            }
            return DataProcess.Failure();
        }

        /// <summary>
        /// 复位
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditDeviceAlarm(DeviceAlarm entity)
        {
            var alarmList = DeviceAlarms.Where(a => a.ContainerCode == entity.ContainerCode && a.Status == (int) DeviceAlarmStateEnum.Urgencye).ToList();
            foreach (var item in alarmList)
            {
                var Data = DateTime.Now;
                TimeSpan ts = Data - item.CreatedTime;
                int Dates = ts.Days;
                int hour = ts.Hours;
                int minute = ts.Minutes;
                item.ContinueTime = (((Dates * 24 + hour) * 60) + minute);
                item.UpdatedTime = Data;
                item.Status = (int) DeviceAlarmStateEnum.Reach;

                if (DeviceAlarmRepository.Update(item) < 0)
                {
                    return DataProcess.Failure("复位失败！");
                }
            }

            return DataProcess.Success(string.Format("全部报警复位成功！"));
        }

 
    }
}
