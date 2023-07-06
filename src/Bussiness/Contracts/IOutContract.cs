using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Entitys;
using Bussiness.Entitys.InterFace;
using HP.Core.Data;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
   public interface IOutContract : IScopeDependency
    {
        IRepository<Entitys.Out, int> OutRepository { get; }
        IRepository<Entitys.OutMaterial, int> OutMaterialRepository { get; }
        //IRepository<Entitys.OutMaterialLabel, int> OutMaterialLabelRepository { get; }
        IRepository<OutIF, int> OutIFRepository { get; }
        IRepository<OutMaterialIF, int> OutMaterialIFRepository { get;  }
        IQuery<Entitys.Out> Outs { get; }
        IQuery<Dtos.OutDto> OutDtos { get; }

        IQuery<Entitys.OutMaterial> OutMaterials{ get; }

        IQuery<Dtos.OutMaterialDto> OutMaterialDtos { get; }

        IQuery<Entitys.OutMaterialLabel> OutMaterialLabels { get; }
        IQuery<Dtos.OutMaterialLabelDto> OutMaterialLabelDtos { get; }

        ///<summary>
        /// 创建出库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateOutEntity(Entitys.Out entity);

        /// <summary>
        /// 删除出库单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveOut(int id);


        DataResult CancelOut(Out entity);
        ///<summary>
        /// 删除出库物料
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateOutMaterialEntity(Entitys.OutMaterial entity);

        /// <summary>
        /// 删除出库物料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveOutMaterial(int id);
        /// <summary>
        /// 编辑出库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditOut(Bussiness.Entitys.Out entity);



        /// <summary>
        /// 计算可以库存
        /// </summary>
        /// <param name="MaterialCode"></param>
        /// <param name="WareHouseCode"></param>
        /// <returns></returns>
        DataResult CheckAvailableStock(string MaterialCode, string WareHouseCode);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DataResult CreateOutEntityInterFace();
    }
}
