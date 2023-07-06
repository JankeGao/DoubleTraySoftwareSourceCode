using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Dtos;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    /// <summary>
    /// 盘点任务
    /// </summary>
   public interface ICheckContract: IScopeDependency
    {
        IRepository<Entitys.CheckMain, int> CheckRepository { get; }

        IRepository<Entitys.CheckDetail, int> CheckDetailRepository { get; }
        IRepository<Entitys.CheckArea, int> CheckAreaRepository { get; }
        IQuery<Entitys.CheckMain> Checks { get; }
        IQuery<Dtos.CheckDto> CheckDtos { get; }

        IQuery<Entitys.CheckDetail> CheckDetails { get; }

        IQuery<Dtos.CheckDetailDto> CheckDetailDtos { get; }


        ///<summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateCheckEntity(Entitys.CheckMain entity);

        ///<summary>
        /// 创建盘点单任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateCheckListEntity(Entitys.CheckList entity);

        /// <summary>
        /// 删除盘点单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult CancelCheck(int id);


        ///<summary>
        /// 创建盘点物料条码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateCheckDetailEntity(Entitys.CheckDetail entity);

        /// <summary>
        /// 编辑盘点单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditCheck(Bussiness.Entitys.CheckMain entity);


        /// <summary>
        /// 手动盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult HandCheckDetail(Bussiness.Entitys.CheckDetail entity);
        /// <summary>
        /// 提交盘点结果
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult SubmitCheckResult(Bussiness.Entitys.CheckMain entity);
        /// <summary>
        /// 盘点完成后 重新盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CheckAgain(Bussiness.Entitys.CheckMain entity);

        /// <summary>
        /// 手动添加盘点条码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateCheckDetailForHand(Entitys.CheckDetail entity);


        /// <summary>
        /// 客户端手动执行盘点-成功
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        DataResult HandCheckDetailClient(CheckDto entityDto);


        /// <summary>
        /// 客户端手动执行盘点-完成
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        DataResult ConfirmCheck(CheckDto entityDto);
    }
}
