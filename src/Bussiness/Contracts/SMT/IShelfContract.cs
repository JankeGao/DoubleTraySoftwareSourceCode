using HP.Core.Data;
using HP.Core.Dependency;
using HP.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Entitys.SMT;
using HP.Data.Orm;

namespace Bussiness.Contracts.SMT
{
   public interface IShelfContract: IScopeDependency
    {
        /// <summary>
        /// 上架主表服务
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsShelfMain, int> WmsShelfMainRepository { get; }

        IRepository<Bussiness.Entitys.SMT.WmsShelfMainVM, int> WmsShelfMainVMRepository { get; }
        /// <summary>
        /// 上架明细服务
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsShelfDetail, int> WmsShelfDetailRepository { get; }

        IRepository<Bussiness.Entitys.SMT.WmsShelfDetailVM, int> WmsShelfDetailVMRepository { get; }
        /// <summary>
        /// 上架排序服务
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsShelfSort, int> WmsShelfSortRepository { get; }
        /// <summary>
        /// WEB端确认上架
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        DataResult WebConfirmShelf(Bussiness.Entitys.Label label);
        /// <summary>
        /// 强制完成上架任务
        /// </summary>
        /// <param name="ReplenishCode"></param>
        /// <returns></returns>
        DataResult CompelFinishedReplenishOrder(string ReplenishCode);

        /// <summary>
        /// 第一次扫描
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        DataResult FirstShelf(Bussiness.Entitys.Label label);
        /// <summary>
        /// PDA扫描确认
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        DataResult PdaConfirmShelf(Bussiness.Entitys.Label label);
        /// <summary>
        /// 下一次扫描
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        DataResult NextShelf(Bussiness.Entitys.Label label);

        /// <summary>
        /// 结束当前任务
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        DataResult FinishCurrentReplenish(Entitys.Label label);
        /// <summary>
        /// 跳过当前亮灯位置
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        DataResult SkipCurrentLightLocation(Entitys.Label label);
    }
}
