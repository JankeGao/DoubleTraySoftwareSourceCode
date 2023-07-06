using HP.Core.Data;
using HP.Core.Dependency;
using HP.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Data.Orm;

namespace Bussiness.Contracts.SMT
{
   public interface IPickContract: IScopeDependency
    {
        /// <summary>
        /// 初始单据 明细数据服务
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsPickDetail, int> WmsPickDetailRepository { get; }
        /// <summary>
        /// 初始单据明细视图
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsPickDetailVM, int> WmsPickDetailVMRepository { get; }
        
        /// <summary>
        /// 初始单据 数据服务
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsPickMain, int> WmsPickMainRepository { get; }
        /// <summary>
        /// 拣货单明细数据服务
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsPickOrderDetail, int> WmsPickOrderDetailRepository { get; }
        /// <summary>
        /// 拣货单关联  初始单据服务
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsPickOrderIssue, int> WmsPickOrderIssueRepository { get; }
        /// <summary>
        /// 拣货单主表服务
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsPickOrderMain, int> WmsPickOrderMainRepository { get; }
        /// <summary>
        /// 初始单据 站位数据服务
        /// </summary>
        IRepository<Bussiness.Entitys.SMT.WmsPickOrderArea, int> WmsPickOrderAreaRepository { get; }

        IRepository<Bussiness.Entitys.SMT.WmsPickOrderAreaDetail, int> WmsPickOrderAreaDetailRepository { get; }
        IRepository<Bussiness.Entitys.SMT.WmsPickOrderAreaDetailVM, int> WmsPickOrderAreaDetailVMRepository { get; }
        IRepository<Bussiness.Entitys.SMT.WmsSplitArea, int> WmsSplitAreaRepository { get; }
        IRepository<Bussiness.Entitys.SMT.WmsSplitAreaReel, int> WmsSplitAreaReelRepository { get; }
        IRepository<Bussiness.Entitys.SMT.WmsSplitAreaReelDetail, int> WmsSplitAreaReelDetailRepository { get; }
        IRepository<Bussiness.Entitys.SMT.WmsSplitIssue, int> WmsSplitIssueRepository { get; }
        IRepository<Bussiness.Entitys.SMT.WmsSplitIssueVM, int> WmsSplitIssueVMRepository { get; }
        IRepository<Bussiness.Entitys.SMT.WmsSplitMain, int> WmsSplitMainRepository { get; }

        IQuery<Dtos.PickDto> PickDtos { get; }



        /// <summary>
        /// 合并领料单
        /// </summary>
        /// <param name="Issue_HIdList"></param>
        /// <param name="IsWorkorder"></param>
        /// <returns></returns>
        DataResult CombinePickOrder(List<int> Issue_HIdList, bool IsWorkorder = true);


        ///<summary>
        /// 创建备料单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreatePickEntity(Entitys.SMT.WmsPickMain entity);

        /// <summary>
        /// 删除出库单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemovePickMain(int id);


        ///<summary>
        /// 创建备料物料
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreatePickMaterialEntity(Entitys.SMT.WmsPickDetail entity);

        /// <summary>
        /// 删除出库物料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemovePickMaterial(int id);
        /// <summary>
        /// 编辑备料单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditPickMain(Bussiness.Entitys.SMT.WmsPickMain entity);

        /// <summary>
        /// 分配拣货单任务
        /// </summary>
        /// <param name="PickOrderCodeList"></param>
        /// <returns></returns>
        DataResult CheckPickOrder(List<string> PickOrderCodeList);

        /// <summary>
        /// 作废拣货单
        /// </summary>
        /// <param name="PickOrderCode"></param>
        /// <returns></returns>
        DataResult DoCancel(string PickOrderCode);

        /// <summary>
        /// 获取可启动任务区域
        /// </summary>
        /// <param name="PickOrderCode"></param>
        /// <returns></returns>
        List<Bussiness.Entitys.SMT.WmsPickOrderArea> GetAllAvailablePickArea(string PickOrderCode);

        /// <summary>
        /// 启动区域任务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        DataResult PickTaskDoStart(List<Bussiness.Entitys.SMT.WmsPickOrderArea> list);
        /// <summary>
        /// 熄灭区域任务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        DataResult PickTaskDoFinish(List<Bussiness.Entitys.SMT.WmsPickOrderArea> list);
        /// <summary>
        /// 拆盘单作废
        /// </summary>
        /// <param name="SplitNo"></param>
        /// <returns></returns>
        DataResult CancelSplitOrder(string SplitNo);
        /// <summary>
        /// 作废拣货条码
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="PickOrderCode"></param>
        /// <returns></returns>
        DataResult CancelPcikAreaDetailReel(string ReelId, string PickOrderCode);

        /// <summary>
        /// 拆盘区域亮灯
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        DataResult SplitTaskDoStart(List<Bussiness.Entitys.SMT.WmsSplitArea> list);

        /// <summary>
        /// 拆盘区域灭灯
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        DataResult SplitTaskDoFinish(List<Bussiness.Entitys.SMT.WmsSplitArea> list);
        /// <summary>
        /// 作废拆盘条码
        /// </summary>
        /// <param name="SplitReel">拆盘条码</param>
        /// <param name="SplitNo">拆盘单号</param>
        /// <returns></returns>
        DataResult CancelSplitReel(string SplitReel, string SplitNo);
        /// <summary>
        /// 确定拆盘
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="SplitNo"></param>
        /// <returns></returns>
        DataResult ConfirmSplitReel(string ReelId, string SplitNo);
        /// <summary>
        /// 检查拆盘条码是否在此拆盘单上
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="SplitNo"></param>
        /// <returns></returns>
        DataResult CheckReelIdIsInSplitTask(string ReelId, string SplitNo);

        /// <summary>
        /// WEB端上架拆盘条码
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="SplitNo"></param>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        DataResult WebConfirmShelfSplitReel(string ReelId, string SplitNo, string LocationCode);
        /// <summary>
        /// 复核
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="PickOrdercode"></param>
        /// <returns></returns>
        DataResult ConfirmReelToSend(string ReelId, string PickOrdercode);
        /// <summary>
        /// 根据库位 🐎 获取拣货信息  
        /// </summary>
        /// <param name="PickOrderCode"></param>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        DataResult GetReelIdByLocationCodeForPick(string PickOrderCode, string ReelId,string LocationCode);
        /// <summary>
        /// 查找条码是否在此拣货单上
        /// </summary>
        /// <param name="ReelId"></param>
        /// <param name="PickOrderCode"></param>
        /// <returns></returns>
        DataResult IsTheReelIdInPickOrder(string ReelId, string PickOrderCode);
        /// <summary>
        /// 根据库位码或者条码找到拆盘条码信息
        /// </summary>
        /// <param name="SplitNo"></param>
        /// <param name="ReelId"></param>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        DataResult GetReelIdByLocationCodeForSplit(string SplitNo, string ReelId, string LocationCode);
    }
}
