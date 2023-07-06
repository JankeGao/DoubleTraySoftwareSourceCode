using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IReceiveContract : IScopeDependency
    {
        IRepository<Entitys.Receive, int> ReceiveRepository { get; }

        IRepository<Entitys.ReceiveDetail, int> ReceiveDetailRepository { get; }
        

        /// <summary>
        /// 领用单信息
        /// </summary>
        IQuery<Entitys.Receive> Receives { get; }            

        /// <summary>
        /// 领用单信息
        /// </summary>
        IQuery<ReceiveDto> ReceiveDtos { get; }


        /// <summary>
        /// 领用单明细信息
        /// </summary>
        IQuery<ReceiveDetailDto> ReceiveDetailDtos { get; }

        /// <summary>
        /// 添加领用单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult CreateReceive(Receive entity);

        /// <summary>
        /// 删除领用清单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveReceive(int id);

        /// <summary>
        /// 编辑领用单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditReceive(Bussiness.Entitys.Receive entity);

        /// <summary>
        /// 作废领用单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CancellationeReceive(Bussiness.Entitys.Receive entity);

        DataResult CreateReceiveInterFace();

    }
}
