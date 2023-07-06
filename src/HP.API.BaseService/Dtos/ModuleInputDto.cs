using System.Collections.Generic;
using HP.Core.Data;

namespace HPC.BaseService.Dtos
{
    public class ModuleInputDto:IInputDto<int>
    {
        public int Id { set; get; }
        /// <summary>
        /// 主键
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
        /// 模块Address地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 模块图标
        /// </summary>
        public string Icon { set; get; }
        /// <summary>
        /// 模块排序
        /// </summary>
        public int Sort { set; get; }
        /// <summary>
        /// 模块备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 模块是否启用
        /// </summary>
        public bool Enabled { set; get; }
        /// <summary>
        /// 模块是否显示
        /// </summary>
        public bool Visible { set; get; }
        /// <summary>
        /// 目标
        /// </summary>
        public string Target { set; get; }
        /// <summary>
        /// 是否公共菜单
        /// </summary>
        public bool IsPublic { set; get; }
        /// <summary>
        /// 是否菜单
        /// </summary>
        public bool IsMenu { set; get; }
        /// <summary>
        /// 工具箱
        /// </summary>
        public bool IsToolbox { set; get; }
        /// <summary>
        /// 操作码列表
        /// </summary>
        public List<string> FunctionCodes { set; get; }
    }
}