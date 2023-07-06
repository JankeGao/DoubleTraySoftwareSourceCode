using HPC.BaseService.Contracts;
using HP.Core.Data;
using HP.Core.Initialize;
using HP.Core.Security;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;

namespace HPC.BaseService.Services
{
    public class EntityInfoService : IEntityInfoContract
    {
        /// <summary>
        /// 实体信息仓储
        /// </summary>
        public IRepository<EntityInfo, string> EntityInfoRepository { set; get; }
        public IEntityInitializer EntityInitializer { set; get; }

        public IQuery<EntityInfo> EntityInfos
        {
            get { return EntityInfoRepository.Query(); }
        }

        /// <summary>
        /// 实体配置编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditEntityInfo(EntityInfo entity)
        {
            if (EntityInfoRepository.Update(a => new EntityInfo
            {
                DataLogEnabled = entity.DataLogEnabled
            }, a => a.Id == entity.Id) == 0)
            {
                return DataProcess.Failure("数据实体({0})更新失败！".FormatWith(entity.Id));
            }

            //更新实体信息
            EntityInitializer.UpdateEntity(entity);

            return DataProcess.Success("数据实体更新成功！");
        }
    }
}
