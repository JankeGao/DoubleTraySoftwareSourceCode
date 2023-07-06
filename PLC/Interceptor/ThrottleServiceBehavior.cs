using System;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace PLCServer.Interceptor
{
    // 应用自定义服务行为的2中方式：
    // 1. 继承Attribute作为特性 服务上打上标示
    // 2. 继承BehaviorExtensionElement, 然后修改配置文件
    public class ThrottleServiceBehaviorAttribute : Attribute, IServiceBehavior
    {
        #region implement IServiceBehavior
        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispather in serviceHostBase.ChannelDispatchers)
            {
                foreach (var endpoint in channelDispather.Endpoints)
                {
                    // holyshit DispatchRuntime 
                    endpoint.DispatchRuntime.MessageInspectors.Add(new ThrottleDispatchMessageInspector());
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {

        }
        #endregion

        #region override BehaviorExtensionElement
        //public override Type BehaviorType
        //{
        //    get { return typeof(ThrottleServiceBehavior); }
        //}

        //protected override object CreateBehavior()
        //{
        //    return new ThrottleServiceBehavior();
        //}
        #endregion
    }
}
