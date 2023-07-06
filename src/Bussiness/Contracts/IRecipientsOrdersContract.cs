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
   public interface IRecipientsOrdersContract : IScopeDependency
    {
        IRepository<Entitys.RecipientsOrders, int> RecipientsOrdersRepository { get; }

        /// <summary>
        /// 领用单信息
        /// </summary>
        IQuery<Entitys.RecipientsOrders> RecipientsOrderss { get; }            

        /// <summary>
        /// 领用模具信息
        /// </summary>
        IQuery<RecipientsOrdersDto> RecipientsOrdersDtoList { get; }

        /// <summary>
        /// 删除领用清单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveRecipientsOrdersMaterial(int id);


        /// <summary>
        /// 删除领用单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveRecipientsOrders(int id);
    }
}
