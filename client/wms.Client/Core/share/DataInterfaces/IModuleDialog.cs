

using System.Threading.Tasks;

namespace wms.Client.Core.share.DataInterfaces
{
    /// <summary>
    /// 弹出式窗口接口
    /// </summary>
    public interface IModuleDialog
    {
        /// <summary>
        /// 弹出窗口
        /// </summary>
        Task<bool> ShowDialog();

        /// <summary>
        /// 注册模块事件
        /// </summary>
        void SubscribeEvent();

        /// <summary>
        /// 訂閱消息
        /// </summary>
        void SubscribeMessenger();

        /// <summary>
        /// 取消订阅消息
        /// </summary>
        void UnsubscribeMessenger();

    }
}
