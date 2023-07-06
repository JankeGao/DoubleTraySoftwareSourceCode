using wms.Client.LogicCore.Interface;

namespace wms.Client.LogicCore.Configuration
{
    public class ServiceProvider
    {
        public static IAutoFacLocator Instance { get; private set; }

        public static void RegisterServiceLocator(IAutoFacLocator s)
        {
            Instance = s;
        }

    }
}
