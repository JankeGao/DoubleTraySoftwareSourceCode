using System;
using HP.Core.Dependency;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    //是一个虚类
    public interface IEquipmentTypeContract : IScopeDependency
    {
        IRepository<Entitys.EquipmentType, int> EquipmentTypeRepository { get; }

        IQuery<Entitys.EquipmentType> EquipmentType { get; }            //查找

        DataResult CreateEquipmentType(Entitys.EquipmentType entity);    //添加

        DataResult EditEquipmentType(Entitys.EquipmentType entity);      //修改

        DataResult DeleteEquipmentType(int id);                //删除


    }
}
