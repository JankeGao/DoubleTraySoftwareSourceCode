using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Interface;
using wms.Client.View;
using wms.Client.ViewModel;
using wms.Client.ViewModel.Base;

namespace wms.Client.ViewDlg
{
    /// <summary>
    /// 字典
    /// </summary>
    [Autofac(true)]
    public class UserDlg : BaseView<UserView, UserViewModel>, IModel
    {

    }
}
