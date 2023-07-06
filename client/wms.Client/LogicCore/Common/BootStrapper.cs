using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Interface;

namespace wms.Client.LogicCore.Common
{
    public class BootStrapper
    {
        /// <summary>
        /// 注册方法
        /// </summary>
        public static void Initialize(IAutoFacLocator autoFacLocator)
        {
            ServiceProvider.RegisterServiceLocator(autoFacLocator);
            ServiceProvider.Instance.Register();
        }
    }
}
