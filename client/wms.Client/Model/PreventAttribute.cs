using System;

namespace wms.Client.Model
{
    /// <summary>
    /// 标记不序列化
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PreventAttribute : Attribute
    {
    }
}
