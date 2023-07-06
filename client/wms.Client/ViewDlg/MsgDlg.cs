using System.Threading.Tasks;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using wms.Client.LogicCore.Interface;

namespace wms.Client.ViewDlg
{
    /// <summary>
    /// 弹窗
    /// </summary>
    public class MsgDlg : IShowContent
    {
        private UserControl view;

        public void BindDataContext<T, V>(T control, V viewModel)
            where T : UserControl
            where V : class, new()
        {
            view = control;
            view.DataContext = viewModel;
        }

        public void BindDataContext<V>(V viewModel) where V : class, new()
        {
            view.DataContext = viewModel;
        }

        public async Task<bool> Show()
        {
            if (view == null) return false;
            object taskResult = await DialogHost.Show(view, "RootDialog"); //位于顶级窗口
            return (bool)taskResult;
        }


        //public async Task<bool> Close()
        //{
        //    if (view == null) return false;
        //    object taskResult = await DialogHost.Close(view, "RootDialog"); //位于顶级窗口
        //    return (bool)taskResult;
        //}
    }
}
