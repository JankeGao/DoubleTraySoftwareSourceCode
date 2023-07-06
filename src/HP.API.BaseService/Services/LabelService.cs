using HP.Core.Data;
using HP.Core.Sequence;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public class LabelService : ILabelContract
    {
        /// <summary>
        /// 标签仓储
        /// </summary>
        public IRepository<Label, int> LabelRepository { set; get; }

        /// <summary>
        /// 标签映射
        /// </summary>
        public IRepository<LabelMap, int> LabelMapRepository { set; get; }

        /// <summary>
        /// 编码序列契约
        /// </summary>
        public ISequenceContract SequenceContract { set; get; }

        public IQuery<Label> Labels
        {
            get { return LabelRepository.Query(); }
        }

        public IQuery<LabelMap> LabelMaps
        {
            get { return LabelMapRepository.Query(); }
        }

        public void Test()
        {

        }

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateLabel(Label entity)
        {
            DataResult result = ValidateLabel(entity);
            if (!result.Success) return result;
            entity.Code = SequenceContract.Create(entity.GetType());
            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("标签编码不能为空！");
            }

            if (Labels.Any(a => a.Name == entity.Name))
            {
                return DataProcess.Failure("标签名称({0})已经存在！".FormatWith(entity.Code));
            }

            //插入标签
            if (!LabelRepository.Insert(entity))
            {
                return DataProcess.Failure("标签({0})创建失败！".FormatWith(entity.Code));
            }

            return DataProcess.Success("标签({0})创建成功！".FormatWith(entity.Code));
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult AddLabel(LabelMapInputDto entityDto)
        {
            if (entityDto.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("打标编码不能为空！");
            }
            //如果有数据
            if (LabelMaps.Any(p => p.BCode == entityDto.Code))
            {
                if (LabelMapRepository.Delete(p => p.BCode == entityDto.Code) == 0)
                {
                    return DataProcess.Failure("原始标签绑定数据删除失败！");
                }
            }
            //添加标签
            foreach (string label in entityDto.Labels.FromJsonString<string[]>())
            {

                if (!LabelMapRepository.Insert(new LabelMap()
                {
                    BCode = entityDto.Code,
                    Code = label,
                    Name= Labels.FirstOrDefault(a=>a.Code== label).Name
                }))
                {
                    return DataProcess.Failure("添加标签失败！");
                }
            }
            return DataProcess.Success("添加标签成功！");
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveLabel(int id)
        {
            // 标签实体
            var entity=Labels.FirstOrDefault(a => a.Id == id);
            id.CheckGreaterThan("id",0);
            if (LabelRepository.Delete(id) == 0)
            {
                return DataProcess.Failure("标签({0})移除失败".FormatWith(id));
            }
            // 删除所有标签的绑定关系
            if (LabelMaps.Any(p => p.Code == entity.Code))
            {
                if (LabelMapRepository.Delete(p => p.Code == entity.Code) == 0)
                {
                    return DataProcess.Failure("原始标签绑定数据删除失败！");
                }
            }
            return DataProcess.Success("标签({0})移除成功！".FormatWith(id));
        }

        /// <summary>
        /// 验证数据合法性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private DataResult ValidateLabel(Label entity)
        {
            if (entity.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("标签名称不能为空！");
            }
            return DataProcess.Success();
        }
    }
}
