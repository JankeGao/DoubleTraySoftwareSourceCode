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
   public interface IReceiveTaskContract : IScopeDependency
    {
        IRepository<Entitys.ReceiveTask, int> ReceiveTaskRepository { get; }

        IRepository<ReceiveTaskHistory, int> ReceiveTaskHistoryRepository { get; set; }


        IQuery<ReceiveTaskDetailDto> ReceiveHistoryDetailDtos { get; }

        /// <summary>
        /// 领用任务信息
        /// </summary>
        IQuery<ReceiveTaskDto> ReceiveTaskDtos { get; }
        IQuery<ReceiveTaskDetailDto> ReceiveTaskDetailDtos { get; }

        /// <summary>
        /// 领用任务明细信息
        /// </summary>
        IQuery<ReceiveTask> ReceiveTasks { get; }
      
        /// <summary>
        /// 添加领用任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult CreateReceiveTaskEntity(Receive entity);

        /// <summary>
        /// 删除领用任务单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveReceiveTask(int id);

        /// <summary>
        ///执行领用任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult HandShelfReceiveTask(ReceiveTaskDetail entity);

        /// <summary>
        ///归还
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult ReturnReceiveTask(ReceiveTaskDetail entity);
    }
}
