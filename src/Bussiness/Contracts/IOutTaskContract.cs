using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IOutTaskContract : IScopeDependency
   {
       /// <summary>
       /// 下发入库任务
       /// </summary>
       /// <param name="entity"></param>
       /// <returns></returns>
       DataResult CreateOutTaskEntity(Out entity);

        IRepository<Entitys.OutTask, int> OutTaskRepository { get; }

        IRepository<OutTaskMaterial, int> OutTaskMaterialRepository { get; }
        IRepository<Entitys.OutTaskMaterialLabel, int> OutTaskMaterialLabelRepository { get; }
        IQuery<Entitys.OutTask> OutTasks { get; }
        IQuery<OutTaskDto> OutTaskDtos { get; }
        IQuery<OutTaskMaterial> OutTaskMaterials { get; }
        IQuery<Entitys.OutTaskMaterialLabel> OutTaskMaterialLabels{ get; }


        IQuery<OutTaskMaterialLabelDto> OutTaskMaterialLabelDtos { get; }

        IQuery<OutTaskMaterialDto> OutTaskMaterialDtos { get; }
        /// <summary>
        /// 删除入库任务单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveOutTask(int id);

        /// <summary>
        /// 删除入库任务单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //  DataResult CheckAvailableLocation(OutTaskMaterialLabel entity);



        /// <summary>
        /// 编辑入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //DataResult EditOut(Bussiness.Entitys.OutTask entity);


        DataResult ConfirmHandPicked(OutTaskMaterialDto entity);


        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult SendOrderToPTL(OutTask entity);

        /// <summary>
        /// 手动出库-查询可用库存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CheckAvailableStock(OutTaskMaterialLabel entity);

        /// <summary>
        /// 手动出库-客户端
        /// </summary>
        /// <param name="outTaskEntityDto"></param>
        /// <returns></returns>
        DataResult HandPickClient(OutTaskDto outTaskEntityDto);
        /// <summary>
        /// 启动货柜
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        DataResult HandStartContainer(OutTaskMaterialDto entityDto);

        DataResult HandRestoreContainer(OutTaskMaterialDto entityDto);
    }
}
