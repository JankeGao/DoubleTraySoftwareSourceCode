using System;
using Bussiness.Dtos;
using HP.Core.Dependency;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    //是一个虚类
    public interface  IBoxContract: IScopeDependency
    {
        IRepository<Entitys.Box, int> BoxRepository { get; }

        IQuery<Entitys.Box> Box { get; }            //查找

        DataResult CreateBox(Entitys.Box entity);    //添加

        DataResult EditBox(Entitys.Box entity);      //修改

        DataResult DeleteBox(int id);                //删除


    }
}
