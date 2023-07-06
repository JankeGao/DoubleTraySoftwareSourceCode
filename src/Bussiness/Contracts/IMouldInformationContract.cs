using System;
using HP.Core.Dependency;
using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;
using Bussiness.Dtos;

namespace Bussiness.Contracts
{
    //是一个虚类
    public interface IMouldInformationContract : IScopeDependency
    {
        IRepository<Entitys.MouldInformation, int> MouldInformationRepository { get; }

        IQuery<Entitys.MouldInformation> MouldInformations { get; }            //查找

        DataResult CreateMouldInformation(Entitys.MouldInformation entity);    //添加

        DataResult EditMouldInformation(Entitys.MouldInformation entity);      //修改

        DataResult DeleteMouldInformation(int id);                //删除

        IQuery<MouldInformationDto> MouldInformationDtos { get; }


    }
}
