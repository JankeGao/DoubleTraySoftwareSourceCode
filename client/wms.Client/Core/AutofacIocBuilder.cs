using System;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using HP.Core.Dependency;

namespace wms.Client.Core
{
    public class AutofacIocBuilder
    {
        /// <summary>
        /// Ioc容器初始化
        /// </summary>
        /// <param name="type">类型</param>
        public static void Build(Type type)
        {
            var builder = new ContainerBuilder();

            builder.RegisterServices();

           var container = IocBuilder.Build(builder);

           ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
        }

        /// <summary>
        /// Ioc容器初始化
        /// </summary>
        public static void Build<T>()
        {
            Build(typeof(T));
        }
    }
}
