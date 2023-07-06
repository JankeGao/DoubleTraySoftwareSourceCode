using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
   public interface IWareHouseContract : IScopeDependency
    {
        IQuery<WareHouse> WareHouseAuthDtos { get; }

        IQuery<Area> Areas { get; }
        /// <summary>
        /// 仓储
        /// </summary>
        IRepository<Entitys.WareHouse, int> WareHouseRepository { get; }

        IRepository<Entitys.Tray, int> TrayRepository { get; }

        IRepository<TrayWeightMap, int> TrayWeightMapRepository { get; }

        IRepository<TrayUserMap, int> TrayUserMapRepository { get; }

        IRepository<Entitys.Location, int> LocationRepository { get; }

        IRepository<Entitys.LocationVIEW, int> LocationVIEWRepository { get; }

        IRepository<Entitys.Channel, int> ChannelRepository { get;  }
        /// <summary>
        /// 查询
        /// </summary>
        IQuery<Entitys.WareHouse> WareHouses { get; }
        IQuery<Entitys.Container> Containers { get; }
        IQuery<ContainerDto> ContainerDtos { get; }
        IQuery<Tray> Trays { get; }
        IQuery<TrayUserDto> TrayUserDtos { get; }
        IQuery<Entitys.Location> Locations { get; }
        IQuery<Entitys.LocationVM> LocationVMs { get; }
        IQuery<LocationVIEW> LocationVIEWs { get; }


        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateWareHouse(Entitys.WareHouse entity);
        /// <summary>
        /// 编辑仓库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditWareHouse(Entitys.WareHouse entity);

        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveWareHouse(int id);



        /// <summary>
        /// 创建货柜
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        DataResult CreateContainer(ContainerDto entityDto);
        /// <summary>
        /// 编辑货柜
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditContainer(Container entity);
        /// <summary>
        /// 移除货柜
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveContainer(int id);



        /// <summary>
        /// 创建托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateTray(Tray entity);

        /// <summary>
        /// 编辑托盘
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditTray(Tray entity);
        /// <summary>
        /// 移除托盘
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveTray(int id);

        /// <summary>
        /// 用户批量维护权限
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult AddBatchTrayUserMap(TrayUserMap entity);


        /// <summary>
        /// 维护托盘储位信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditTrayLocation(Tray entity);

        ///<summary>
        /// 创建库位
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateLocation(Entitys.Location entity);

        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveLocation(int id);

        /// <summary>
        /// /根据LayoutId删除储位
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        DataResult RemoveLocationByLayoutId(string layoutId);

        /// <summary>
        /// /根据TrayId删除储位
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        DataResult RemoveLocationByTrayId(Tray entity);

        /// <summary>
        /// 验证客户端UID
        /// </summary>
        /// <param name="code"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        DataResult CheckUID(string code, string uid);

    }
}
