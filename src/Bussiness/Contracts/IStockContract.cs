using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
   public interface IStockContract : IScopeDependency
    {
        IRepository<Entitys.StockVM, int> StockVMRepository { get;  }

        IRepository<Entitys.InactiveStockVM,int> InactiveStockVMRepository { get; }

        IQuery<InactiveStockVM> InactiveStockVMs { get; }

        IRepository<Entitys.MaterialStatusVM, int> MaterialStatusRepository { get; }

        IQuery<MaterialStatusVM> MaterialStatusS { get; }

        IRepository<Entitys.InventoryStatusVM, int> InventoryStatusRepository { get; }

        IQuery<InventoryStatusVM> InventoryStatus { get; }

        IRepository<Entitys.Stock, int> StockRepository { get; }
        IQuery<Entitys.Stock> Stocks { get; }
        IQuery<Dtos.StockDto> StockDtos { get; }
        ///<summary>
        /// 新增库存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateStockEntity(Entitys.Stock entity);

        /// <summary>
        /// 删除库存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveStock(int id);

        /// <summary>
        /// 根据库位码或者物料条码获取库存
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        DataResult GetStockByMaterialLabel(string MaterialLabel, string LocationCode);

        /// <summary>
        /// 移库
        /// </summary>
        /// <param name="MaterialLabel"></param>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        DataResult StockMoveLocationCode(string MaterialLabel, string LocationCode);

        DataResult CheckOldStock();
    }
}
