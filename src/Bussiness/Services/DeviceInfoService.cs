using System;
using System.Collections.Generic;
using System.Linq;
using HP.Core.Data;
using Bussiness.Entitys;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using Bussiness.Dtos;
using Bussiness.Contracts;

namespace Bussiness.Services
{
    class DeviceInfoServer : Contracts.IDeviceInfoContract
    {
        public IRepository<DeviceInfo, int> DeviceInfo { get; set; }
        public IRepository<Container,int> Containers { get; set; }

        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }

        public IRepository<Entitys.StockVM, int> StockVMRepository { get; set; }
        public IQuery<StockVM> StockVMs => StockVMRepository.Query();

        public IWareHouseContract WareHouseContract { set; get; }

        public IRepository<Tray, int> TrayRepository { get; set; }

        public IQuery<DeviceInfo> DeviceInfos
        {
            get
            {
                return DeviceInfo.Query();
            }
        }



        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateDeviceInfo(Entitys.DeviceInfo entity)
        {
            entity.CheckNotNull("entity");
            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("设备编号不能为空！");
            }
            if (entity.AreaId == 0)
            {
                return DataProcess.Failure("设备所属区域编号不能为空！");
            }

            //验证设备编码是否存在
            if (DeviceInfos.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure("设备编码({0})已经存在！".FormatWith(entity.Code));
            }
            try
            {
                //插入区域
                if (!DeviceInfo.Insert(entity))
                {
                    return DataProcess.Failure("设备({0})创建失败！".FormatWith(entity.Code));
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure("设备{0}创建失败:" + ex.Message + "".FormatWith(entity.Code));
            }


            return DataProcess.Success("区域({0})创建成功！".FormatWith(entity.Code));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveDeviceInfo(int id)
        {
            var oriEntity = DeviceInfos.Where(a => a.Id == id).Select(a => new { a.Code }).FirstOrDefault();

            if (DeviceInfo.Delete(id) == 0)
            {
                return DataProcess.Failure("设备({0})移除失败".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("设备({0})移除成功！".FormatWith(oriEntity.Code));
        }


        public IQuery<MaterialDto> MaterialDtoList => MaterialContract.Materials.InnerJoin(StockVMs, (material, stockVMs) => material.Code == stockVMs.MaterialCode)
            //.InnerJoin(TrayRepository.Query(), (material, stockVMs, tray) => stockVMs.ContainerCode == tray.ContainerCode)
            .Select((material, stockVMs) => new MaterialDto
           {
               Code = material.Code,
               Name = material.Name,
               ShortName = material.ShortName,
               FileID = material.FileID,
               MaterialType = material.MaterialType,
               MinNum = material.MinNum,
               MaxNum =material.MaxNum,
               Unit = material.Unit,
               PackageUnit =material.PackageUnit,
               Remark1 =material.Remark1,
               Remark2 = material.Remark2,
               Remark3 = material.Remark3,
               Remark4 = material.Remark4,
               Remark5 = material.Remark5,
               UnitWeight = material.UnitWeight,
               PictureUrl  = material.PictureUrl,
               IsDeleted = material.IsDeleted,
               //ContainerCode = stockVMs.ContainerCode,
               //WareHouseCode =stockVMs.WareHouseCode,
               //LocationCode = stockVMs.LocationCode,
               //MaterialCode =stockVMs.MaterialCode,
               //Quantity = stockVMs.Quantity,
               //LockedQuantity = stockVMs.LockedQuantity,
               //MaterialLabel =stockVMs.MaterialLabel,
               //TrayCode = tray.Code
            });

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditDeviceInfo(Entitys.Container entity)
        {
            try
            {
                if (Containers.Update(entity) > 0)
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

                return DataProcess.Failure("设备{0}修改失败" + ex.Message + "".FormatWith(entity.Code));
            }
        }
    }
}
