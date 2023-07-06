
using HP.Core.Data;

namespace HPC.BaseService.Dtos
{
    /// <summary>
    /// 登录信息
    /// </summary>
    public class LabelMapInputDto : IInputDto<int>
    {
        public int Id { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 标签列表
        /// </summary>
        public string Labels { get; set; }

    }
}