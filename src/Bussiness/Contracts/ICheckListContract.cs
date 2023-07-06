using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    /// <summary>
    /// 盘点单CheckList
    /// </summary>
   public interface ICheckListContract: IScopeDependency
    {
        IRepository<Entitys.CheckList, int> CheckListRepository { get; }

     
        /// <summary>
        /// 获取盘点单信息
        /// </summary>
        IQuery<Dtos.CheckListDto> CheckListDtos { get; }

        /// <summary>
        /// 领用盘点单明细信息
        /// </summary>
        IQuery<Dtos.CheckListDetailDto> CheckListDetailDtos { get; }

        /// <summary>
        /// 添加盘点单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateCheckListEntity(Entitys.CheckList entity);

        IQuery<Entitys.CheckList> CheckLists { get; }

        IQuery<Entitys.CheckListDetail> CheckListDetails { get; }

        /// <summary>
        /// 作废盘点单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CancellationeCheckList(Entitys.CheckList entity);






    }
}
