using System.Collections.Generic;
using HP.Core.Data;
using HP.Core.Functions;

namespace HPC.BaseService.Dtos
{
    public class ModuleTreeOutputDto : IOutputDto
    {
        public int Id { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 父亲模块编码
        /// </summary>
        public string ParentCode { set; get; }

        /// <summary>
        /// 父亲模块编码
        /// </summary>
        public string Address { set; get; }

        /// <summary>
        /// 模块图标
        /// </summary>
        public string Icon { set; get; }

        //   [JsonProperty("iconCls")]
        public string IconCls
        {
            get { return Icon; }
        }

        // [JsonProperty("checked")]
        public bool Checked { set; get; }

        /// <summary>
        /// 模块排序
        /// </summary>
        public int Sort { set; get; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { set; get; }

        /// <summary>
        /// 模块排序
        /// </summary>
        public bool Enabled { set; get; }

        /// <summary>
        /// 目标
        /// </summary>
        public string Target { set; get; }

        //  [JsonProperty("state")]
        public string State { set; get; }

        /// <summary>
        /// 子模块
        /// </summary>
        //  [JsonProperty("children")]
        public List<ModuleTreeOutputDto> children { set; get; }

        /// <summary>
        /// 功能码
        /// </summary>
        public List<Function> Functions { set; get; }
    }
}
