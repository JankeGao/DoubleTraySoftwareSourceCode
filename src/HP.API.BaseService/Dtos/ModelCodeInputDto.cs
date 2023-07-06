namespace HPC.BaseService.Dtos
{
    public class ModelCodeInputDto
    {
        /// <summary>
        /// 实体模型命名空间
        /// </summary>
        public string Namespace { set; get; }
        /// <summary>
        /// 模型保存目录
        /// </summary>
        public string Catelog { set; get; }
        /// <summary>
        /// 模型表数据JSON信息
        /// </summary>
        public string ModelTableJson { set; get; }
    }
}
