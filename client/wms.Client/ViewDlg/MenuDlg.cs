using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Interface;
using wms.Client.View;
using wms.Client.ViewModel;
using wms.Client.ViewModel.Base;

namespace wms.Client.ViewDlg
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Autofac(true)]
    public class MenuDlg : BaseView<MenuView, MenuViewModel>, IModel
    {

    }
}
