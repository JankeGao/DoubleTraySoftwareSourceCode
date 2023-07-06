using HP.Core.Data;

namespace HPC.BaseService.Dtos
{
    public class ModuleEntityMapInputDto:IInputDto<int>
    {
        public int Id { get; set; }
        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleId { set; get; }

        public string EntityIdJson { set; get; }
    }
}
