using System.Collections.ObjectModel;
using wms.Client.Core.share.Common;

namespace wms.Client.Core.share.DataInterfaces
{
    /// <summary>
    /// 权限接口
    /// </summary>
    public interface IAuthority
    {
        /// <summary>
        /// 初始化权限
        /// </summary>
        /// <param name="authValue"></param>
        /// <returns></returns>
        void InitPermissions(int AuthValue);

        ObservableCollection<CommandStruct> ToolBarCommandList { get; set; }
    }
}
