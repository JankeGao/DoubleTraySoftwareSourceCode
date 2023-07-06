using System;
using System.Reflection;
using wms.Client.LogicCore.Enums;

namespace wms.Client.LogicCore.UserAttribute
{
    public class GetEnumAttrbute
    {
        /// <summary>
        /// 获取模块名称
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static DescriptionAttribute GetDescription(Enum en)
        {
            Type type = en.GetType();   //获取类型  
            MemberInfo[] memberInfos = type.GetMember(en.ToString());   //获取成员  
            if (memberInfos != null && memberInfos.Length > 0)
            {
                //获取描述特性  
                DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];   //获取描述特性  

                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0];    //返回当前描述  
                }
            }
            return null;
        }
    }
}
