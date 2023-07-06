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
   public interface IInContract : IScopeDependency
    {
        IRepository<Entitys.In, int> InRepository { get; }
        IRepository<Entitys.InMaterial, int> InMaterialRepository { get; }
        IRepository<InMaterialLabel, int> InMaterialLabelRepository { get; }

        IRepository<InIF, int> InIFRepository { get; }
        IRepository<InMaterialIF, int> InMaterialIFRepository { get; }

        IQuery<Entitys.In> Ins { get; }
        IQuery<Dtos.InDto> InDtos { get; }
  
        IQuery<Entitys.InMaterial> InMaterials{ get; }

        IQuery<Dtos.InMaterialDto> InMaterialDtos { get; }
        ///<summary>
        /// 创建入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateInEntity(Entitys.In entity);

        /// <summary>
        /// 删除入库单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveIn(int id);

        /// <summary>
        /// 作废入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CancelIn(In entity);

        ///<summary>
        /// 创建入库物料
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateInMaterialEntity(Entitys.InMaterial entity);

        /// <summary>
        /// 删除入库物料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveInMaterial(int id);
        /// <summary>
        /// 编辑入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditIn(Bussiness.Entitys.In entity);


        DataResult HandShelf(Bussiness.Entitys.InMaterial entity);


        /// <summary>
        /// 发送亮灯任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult SendOrderToPTL(In entity);

        /// <summary>
        /// 接口同步入库单
        /// </summary>
        /// <returns></returns>
        DataResult CreateInEntityInterFace();
        /// <summary>
        /// restful接口同步入库单
        /// </summary>
        /// <returns></returns>
        //object CreateInEntityInterFacedes();
    }
}
