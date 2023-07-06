namespace HPC.BaseService.Models
{
    public class ModelTableMapping
    {
        /// <summary>
        /// 数据库配置名称
        /// </summary>
        public string DbContextConfigName { set; get; }
        /// <summary>
        /// 模型类名
        /// </summary>
        public string[] ClassNames { set; get; }
        /// <summary>
        /// 表名称
        /// </summary>
        public string[] TableNames { set; get; }
    }
}
