namespace HPC.BaseService.Dtos
{
    public class ServiceCodeInputDto
    {
        /// <summary>
        /// 主仓储
        /// </summary>
        public string MasterEntity { set; get; }
        /// <summary>
        /// 辅仓储
        /// </summary>
        public string AssistEntitys { set; get; }
        /// <summary>
        /// 服务命名空间
        /// </summary>
        public string ServiceNamespace { set; get; }
        /// <summary>
        /// 服务类名
        /// </summary>
        public string ServiceClassName { set; get; }
        /// <summary>
        /// 服务保存目录
        /// </summary>
        public string ServiceCatelog { set; get; }
        /// <summary>
        /// 接口命名空间
        /// </summary>
        public string InterfaceNamespace { set; get; }
        /// <summary>
        /// 接口类名
        /// </summary>
        public string InterfaceClassName { set; get; }
        /// <summary>
        /// 接口保存目录
        /// </summary>
        public string InterfaceCatelog { set; get; }
    }
}
