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
    public interface IMaterialContract : IScopeDependency
    {
        IRepository<Entitys.Material, int> MaterialRepository { get; }

        IRepository<MaterialBoxMap, int> MaterialBoxMapRepository { get; }
        IQuery<MaterialBoxMap> MaterialBoxMaps { get; }
        IQuery<Entitys.Material> Materials { get; }

        /// <summary>
        /// 物料DTO
        /// </summary>
        IQuery<MaterialDto> MaterialDtos { get; }

        DataResult CreateMaterial(MaterialDto entity);

        DataResult EditMaterial(MaterialDto entity);

        DataResult DeleteMaterial(int id);

        /// <summary>
        /// 导入维护物料
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditMateraiInfo(MaterialDto entity);
    }
}
