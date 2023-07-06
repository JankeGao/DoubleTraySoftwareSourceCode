using HP.Core.Data;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public class RegionService : IRegionContract
    {
        /// <summary>
        /// 行政区域仓储
        /// </summary>
        public IRepository<Region, int> RegionRepository { set; get; }

        public IQuery<Region> Regions
        {
            get { return RegionRepository.Query(); }
        }

        /// <summary>
        /// 创建行政区域
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateRegion(Region entity)
        {
            DataResult result = ValidateRegion(entity);
            if (!result.Success) return result;

            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("行政区域编码不能为空！");
            }

            if (Regions.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure("行政区域编码({0})已经存在！".FormatWith(entity.Code));
            }

            //插入行政区域
            if (!RegionRepository.Insert(entity))
            {
                return DataProcess.Failure("行政区域({0})创建失败！".FormatWith(entity.Code));
            }

            return DataProcess.Success("行政区域({0})创建成功！".FormatWith(entity.Code));
        }

        /// <summary>
        /// 编辑行政区域
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditRegion(Region entity)
        {
            DataResult result = ValidateRegion(entity);
            if (!result.Success) return result;

            var oriEntity = Regions.FirstOrDefault(a => a.Id == entity.Id);
            oriEntity.CheckNotNull("oriEntity");

            //编辑行政区域
            if (RegionRepository.Update(a=>new Region
            {
                ParentCode=entity.ParentCode,
                Name=entity.Name,
                ShortName=entity.ShortName,
                Longitude=entity.Longitude,
                Latitude=entity.Latitude,
                Lvl=entity.Lvl,
                Sort=entity.Sort,
                Enabled=entity.Enabled,
                Remark=entity.Remark
            },a=>a.Id==entity.Id)==0)
            {
                return DataProcess.Failure("行政区域({0})编辑失败！".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("行政区域({0})编辑成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 移除行政区域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveRegion(int id)
        {
            id.CheckGreaterThan("id",0);
            if (RegionRepository.Delete(id) == 0)
            {
                return DataProcess.Failure("行政区域({0})移除失败".FormatWith(id));
            }

            return DataProcess.Success("行政区域({0})移除成功！".FormatWith(id));
        }

        /// <summary>
        /// 验证数据合法性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private DataResult ValidateRegion(Region entity)
        {
            if (entity.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("行政区域名称不能为空！");
            }
            if (entity.ShortName.IsNullOrEmpty())
            {
                return DataProcess.Failure("行政区域简称不能为空！");
            }

            //验证上级编码是否存在
            if (!entity.ParentCode.IsNullOrEmpty())
            {
                var oriParentEntity = Regions.FirstOrDefault(a => a.Code == entity.ParentCode);
                oriParentEntity.CheckNotNull("oriParentEntity");

                if (oriParentEntity == null)
                {
                    return DataProcess.Failure("行政区域上级({0})不存在！".FormatWith(entity.ParentCode));
                }

                entity.Lvl = oriParentEntity.Lvl + 1;
            }
            else
            {
                entity.Lvl = 1;
            }

            return DataProcess.Success();
        }
    }
}
