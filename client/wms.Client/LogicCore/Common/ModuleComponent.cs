using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Enums;
using wms.Client.LogicCore.UserAttribute;
using wms.Client.Model.ResponseModel;

namespace wms.Client.LogicCore.Common
{
    public class ModuleComponent
    {

        private ObservableCollection<ModuleGroup> GetModuleGroups()
        {
            ObservableCollection<ModuleGroup> mgGroups = new ObservableCollection<ModuleGroup>();
            var array = Enum.GetValues(typeof(ModuleType));

            foreach (var r in array)
            {
                var m = (ModuleType)r;
                var descattr = GetEnumAttrbute.GetDescription(m);
                if (descattr != null)
                    mgGroups.Add(new ModuleGroup() { GroupIcon = descattr.Remark, GroupName = descattr.Caption });
            }

            return mgGroups;
        }

        /// <summary>
        /// 获取当前程序集下模块
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IList<ModuleAttribute>> GetModules()
        {
            try
            {
                IList<ModuleAttribute> list = new List<ModuleAttribute>();

                await Task.Run(() =>
                {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    var types = asm.GetTypes();
                    foreach (var t in types)
                    {
                        var attr = (ModuleAttribute)t.GetCustomAttribute(typeof(ModuleAttribute), false);
                        if (attr != null)
                        {
                            list.Add(attr);

                        }
                    }
                });
                return list;
            }
            catch
            {
                return null;
            }
        }

        public AuthorityEntity Authority;

        /// <summary>
        /// 模块验证
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool ModuleVerify(ModuleAttribute module)
        {
            bool result = false;
            if (Loginer.LoginerUser.IsAdmin)
                result = true;
            else
            {
                Authority = Loginer.LoginerUser.authorityEntity.FirstOrDefault(t => t.menuName.Equals(module.Name));
                if (Authority != null) result = true;
            }
            return result;
        }

    }
}
