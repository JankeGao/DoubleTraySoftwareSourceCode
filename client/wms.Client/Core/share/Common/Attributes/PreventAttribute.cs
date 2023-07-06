

using System;

namespace wms.Client.Core.share.Common.Attributes
{
    /// <summary>
    /// 禁止序列化特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PreventAttribute : Attribute
    {
    }
}
