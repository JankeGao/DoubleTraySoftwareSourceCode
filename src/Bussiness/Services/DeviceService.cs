using System;
using Bussiness.Contracts;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;

namespace Bussiness.Services
{
    public class DeviceService : IDeviceContract
    {
        public IRepository<Device, int> DeviceRepository { set; get; }

        public IQuery<Device> Devices
        {
           get
            {
                return DeviceRepository.Query();
            }
        }

        //public IQuery<DeviceVM> DeviceVMs
        //{
        //    get
        //    {
        //        return Devices.InnerJoin(AreaContract.AreasInfos, (video, area) => video.AreaId == area.Id).Select((video, area) => new DeviceVM
        //        {
        //            Id =video.Id,
        //            Name=video.Name,
        //            Code = video.Code,
        //            Address =video.Address,
        //            Port = video.Port,
        //            AreaId = video.AreaId,
        //            ServerAddress = video.ServerAddress,
        //            Channel = video.Channel,
        //            Description = video.Description,
        //            Status =video.Status,
        //            AreaName = area.Name,
        //            ParentAreaCode = area.ParentCode,
        //            CreatedTime = video.CreatedTime,
        //            CreatedUserCode = video.CreatedUserCode,
        //            CreatedUserName = video.CreatedUserName,
        //            UserName = video.UserName,
        //            PassWord = video.PassWord
        //        });
        //    }
        //}

        /// <summary>
        /// 创建设备
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateDevice(Device entity)
        {
            entity.CheckNotNull("entity");
            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("设备编号不能为空！");
            }
            if (entity.AreaCode==0)
            {
                return DataProcess.Failure("设备所属区域编号不能为空！");
            }

            //验证设备编码是否存在
            if (Devices.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure("设备编码({0})已经存在！".FormatWith(entity.Code));
            }
            try
            {
                //插入区域
                if (!DeviceRepository.Insert(entity))
                {
                    return DataProcess.Failure("设备({0})创建失败！".FormatWith(entity.Code));
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure("设备{0}创建失败:"+ex.Message+"".FormatWith(entity.Code));
            }


            return DataProcess.Success("区域({0})创建成功！".FormatWith(entity.Code));
        }

        public DataResult EditDevice(Device entity)
        {
            try
            {
                if (DeviceRepository.Update(entity)>0)
                {
                    return DataProcess.Success("设备{0}修改成功".FormatWith(entity.Code));
                }
                else
                {
                    return DataProcess.Failure("设备{0}修改失败".FormatWith(entity.Code));
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure("设备{0}修改失败"+ex.Message+"".FormatWith(entity.Code));
            }
        }

        public DataResult RemoveDevice(int id)
        {
            var oriEntity = Devices.Where(a => a.Id == id).Select(a => new { a.Code }).FirstOrDefault();

            if (DeviceRepository.Delete(id) == 0)
            {
                return DataProcess.Failure("设备({0})移除失败".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("设备({0})移除成功！".FormatWith(oriEntity.Code));
        }
    }
}
