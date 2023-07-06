using System;

namespace wms.Client.LogicCore.Common
{
    /// <summary>
    /// autofac 注册特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutofacAttribute : Attribute
    {
        public AutofacAttribute(bool allow)
        {
            _allow = allow;
        }

        private bool _allow;

        public bool Allow { get { return _allow; } }
    }
}
