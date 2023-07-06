using System.Linq;
using System.Reflection;
using Autofac;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Interface;
using wms.Client.Service.Service;
using wms.Client.ViewDlg;
using DictionaryService = wms.Client.Service.Service.DictionaryService;

namespace wms.Client.LogicCore.Common
{
    public class AutofacLocator : IAutoFacLocator
    {
        IContainer container;

        public TInterface Get<TInterface>(string typeName)
        {
            return container.ResolveNamed<TInterface>(typeName);
        }

        public TInterface Get<TInterface>()
        {
            return container.Resolve<TInterface>();
        }

        public void Register()
        {
            var Container = new ContainerBuilder();
            Assembly asm = Assembly.GetExecutingAssembly();
            RegisterByAssembly(asm, ref Container);  //Auto Service
            Container.RegisterType<UserService>().As<IUserService>();
            Container.RegisterType<BaseControlService>().As<IBaseControlService>();
            Container.RegisterType<DashboardService>().As<IDashboardService>();
            Container.RegisterType<InTaskService>().As<IInTaskService>();
            Container.RegisterType<OutTaskService>().As<IOutTaskService>();
            Container.RegisterType<AlarmService>().As<IAlarmService>();
            Container.RegisterType<CheckMainService>().As<ICheckMainService>();
            Container.RegisterType<ReceiveTaskService>().As<IReceiveTaskService>();
            Container.RegisterType<LabelService>().As<ILabelService>();
            Container.RegisterType<GroupService>().As<IGroupService>();
            Container.RegisterType<MenuService>().As<IMenuService>();
            Container.RegisterType<DictionaryService>().As<IDictionariesService>();
            Container.RegisterType<MsgDlg>().As<IShowContent>();

            container = Container.Build();
        }

        private void RegisterByAssembly(Assembly asm, ref ContainerBuilder Container)
        {
            var types = asm.GetTypes();
            foreach (var t in types)
            {
                var attr = (AutofacAttribute)t.GetCustomAttribute(typeof(AutofacAttribute), false);
                if (attr != null && attr.Allow)
                {
                    var interfaceDefault = t.GetInterfaces().FirstOrDefault();
                    if (interfaceDefault != null)
                    {
                        Container.RegisterType(t).Named(t.Name, interfaceDefault);
                    }
                }
            }
        }

    }
}
