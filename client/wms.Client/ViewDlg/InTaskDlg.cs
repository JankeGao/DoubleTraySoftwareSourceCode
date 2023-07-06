using System;
using System.Windows;
using System.Windows.Threading;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Interface;
using wms.Client.View;
using wms.Client.ViewModel;
using wms.Client.ViewModel.Base;

namespace wms.Client.ViewDlg
{
    [Autofac(true)]
    public class InTaskDlg : BaseView<InTaskView, InTaskViewModel>, IModel
    {

        /// <summary>
        /// 刷新界面
        /// </summary>
        public void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);
            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {
            }
        }
        private object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }
}
