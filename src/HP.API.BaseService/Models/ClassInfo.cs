namespace HPC.BaseService.Models
{
    public class ClassInfo
    {
        /// <summary>
        /// 实体对象
        /// </summary>
        public object Object { set; get; }
        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { set; get; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace { set; get; }
        /// <summary>
        /// 类名称
        /// </summary>
        public string ClassFullName { set; get; }
        /// <summary>
        /// 类名称
        /// </summary>
        public string ClassName { set; get; }
        /// <summary>
        /// 属性类型名称
        /// </summary>
        public string PkTypeName { set; get; }
        /// <summary>
        /// 实体描述
        /// </summary>
        public string Description { set; get; }
    }
}
