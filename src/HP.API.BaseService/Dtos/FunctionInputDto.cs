using System.Collections.Generic;

namespace HPC.BaseService.Dtos
{
    public class FunctionInputDto
    {
        /// <summary>
        /// 模块主键
        /// </summary>
        public string ModuleCode { set; get; }
        /// <summary>
        /// 功能码
        /// </summary>
        public List<string> FunctionCodes { set; get; }
    }
}