using HP.Core.Data;
using HP.Core.Dependency;
using HP.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Contracts.SMT
{
   public interface IStockLightContract: IScopeDependency
    {
        /// <summary>
        /// 库存亮灯主表
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsReelLightMain, int> WmsReelLightMainRepository { get; }
        /// <summary>
        /// 库存亮灯区域
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsReelLightArea, int> WmsReelLightAreaRepository { get; }
        /// <summary>
        /// 库存亮灯区域明细
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsReelLightAreaDetail, int> WmsReelLightAreaDetailRepository { get; }
        /// <summary>
        /// 点亮库存
        /// </summary>
        /// <param name="StockIdList"></param>
        /// <returns></returns>
        DataResult LightReelArray(List<Bussiness.Entitys.Stock> stockList);
        /// <summary>
        /// 熄灭
        /// </summary>
        /// <returns></returns>
        DataResult OffLight();

        bool IsCurrentAreaShelfTasking(string WareHouseCode, string AreaId);

        bool IsCurrentAreaPickTasking(string WareHouseCode, string AreaId);
        bool IsCurrentAreaSplitTasking(string WareHouseCode, string AreaId);


        bool IsCurrentAreaCheckTasking(string WareHouseCode, string AreaId);

        DataResult OffAreaLight(string LightCode, string AreaCode);
    }
}
