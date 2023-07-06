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
   public interface IInTaskContract : IScopeDependency
   {
       /// <summary>
       /// 下发入库任务
       /// </summary>
       /// <param name="entity"></param>
       /// <returns></returns>
       DataResult CreateInTaskEntity(In entity);

        IRepository<Entitys.InTask, int> InTaskRepository { get; }
        IRepository<Entitys.InTaskMaterial, int> InTaskMaterialRepository { get; }
        IQuery<Entitys.InTask> InTasks { get; }
        IQuery<InTaskDto> InTaskDtos { get; }

        IQuery<Entitys.InTaskMaterial> InTaskMaterials{ get; }

        IQuery<Dtos.InTaskMaterialDto> InTaskMaterialDtos { get; }


        /// <summary>
        /// 删除入库任务单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveInTask(int id);

        /// <summary>
        /// 删除入库任务单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult CheckAvailableLocation(InTaskMaterial entity);



        /// <summary>
        /// 编辑入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //DataResult EditIn(Bussiness.Entitys.InTask entity);



        DataResult HandShelf(InTaskMaterialDto entity);


        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult SendOrderToPTL(InTask entity);

        /// <summary>
        /// 查看客户端可存放储位
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CheckClientLocation(InTaskMaterialDto entity);

        /// <summary>
        /// 客户端执行手动入库
        /// </summary>
        /// <param name="inTaskEntityDto"></param>
        /// <returns></returns>
        DataResult HandShelfClient(InTaskDto inTaskEntityDto);
        /// <summary>
        /// 启动货柜
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        DataResult HandStartContainer(InTaskMaterialDto entityDto);

        DataResult HandRestoreContainer(InTaskMaterialDto entityDto);
    }
}
