using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Bussiness.Contracts;
using HP.Core.Data;
using HP.Data.Orm.Entity;
using Bussiness.Entitys;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Services
{
    class EquipmentTypeServer : Contracts.IEquipmentTypeContract
    {
        public IRepository<EquipmentType, int> EquipmentTypeRepository { get; set; }

        /// <summary>
        /// 仓库契约
        /// </summary>
        public IWareHouseContract WareHouseContract { set; get; }

        public IQuery<EquipmentType> EquipmentType
        {
            get
            {
                return EquipmentTypeRepository.Query();
            }
        }

        //判断该设备型号是否存在并创建
        public DataResult CreateEquipmentType(EquipmentType entity)
        {
            if (EquipmentType.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure(string.Format("设备型号{0}已存在", entity.Code));
            }
            if (EquipmentTypeRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("设备型号{0}创建成功", entity.Code));
            }
            return DataProcess.Failure();
        }

        //删除设备型号的操作
        public DataResult DeleteEquipmentType(int id)
        {
            // 验证id 是否存在
            var entity = EquipmentTypeRepository.GetEntity(id);
            if (entity==null)
            {
                return DataProcess.Failure(string.Format("设备型号{0}在系统中不存在", entity.Code));
            }
            // 验证在仓库中是否存在该型号设备
            if (WareHouseContract.Containers.Any(a => a.EquipmentCode == entity.Code))
            {
                return DataProcess.Failure(string.Format("设备型号{0}在仓库中仍存在对应设备，无法删除", entity.Code));
            }
            if (EquipmentTypeRepository.LogicDelete(id) > 0)
            {
                return DataProcess.Success("删除成功");
            }
            return DataProcess.Failure();
        }

        //对设备型号的信息进行编辑
        public DataResult EditEquipmentType(EquipmentType entity)
        {
            Console.WriteLine(entity);
            if (EquipmentTypeRepository.Update(entity) > 0)
            {
                return DataProcess.Success(string.Format("设备型号{0}编辑成功", entity.Code));
            }
            return DataProcess.Failure();
        }
    }
}
