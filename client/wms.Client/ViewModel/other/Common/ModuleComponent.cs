

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using wms.Client.Core.share.Common.Attributes;

namespace wms.Client.ViewModel.other.Common
{
    /// <summary>
    /// 模块组件
    /// </summary>
    public class ModuleComponent
    {
        /// <summary>
        /// 获取程序集下的所有具备模块特性的集合
        /// </summary>
        /// <returns>模块特性集合</returns>
        public async Task<List<ModuleAttribute>> GetAssemblyModules()
        {
            try
            {
                List<ModuleAttribute> list = new List<ModuleAttribute>();
                await Task.Run(() =>
                {
                    Assembly asm = Assembly.GetEntryAssembly();
                    var types = asm.GetTypes();
                    foreach (var t in types)
                    {
                        var attr = (ModuleAttribute)t.GetCustomAttribute(typeof(ModuleAttribute), false);
                        if (attr != null)
                            list.Add(attr);
                    }
                });
                return list;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 模块验证
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool ModuleVerify(ModuleAttribute module)
        {
            //if (Contract.IsAdmin)
            //    return true;
            //else
            //{
            //    if (Contract.Menus
            //        .FirstOrDefault(t => t.MenuName.Equals(module.Name)) != null)
            //        return true;
            //}
            return false;
        }
    }
}
