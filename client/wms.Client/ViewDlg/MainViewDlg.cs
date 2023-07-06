using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Interface;
using wms.Client.ViewModel.Base;
using wms.Client.ViewModel;

namespace wms.Client.ViewDlg
{
    /// <summary>
    /// 首页
    /// </summary>
    [Autofac(true)]
    public class MainViewDlg : BaseViewDialog<MainWindow>, IModelDialog
    {
        public override void BindDefaultViewModel()
        {
            MainViewModel model = new MainViewModel();
            model.InitDefaultView();
            GetDialogWindow().DataContext = model;
        }

        public override void BindViewModel<TViewModel>(TViewModel viewModel)
        {
            GetDialogWindow().DataContext = viewModel;
        }

        public override void Close()
        {
            GetDialogWindow().Close();
        }

        public override Task<bool> ShowDialog(DialogOpenedEventHandler openedEventHandler = null, DialogClosingEventHandler closingEventHandler = null)
        {
            GetDialogWindow().ShowDialog();
            return Task.FromResult(true);
        }

        public override void RegisterDefaultEvent()
        {
            GetDialogWindow().MouseDown += (sender, e) => { if (e.LeftButton == MouseButtonState.Pressed) { GetDialogWindow().DragMove(); } };
            Messenger.Default.Register<string>(GetDialogWindow(), "MainExit", new Action<string>(async (msg) =>
            {
                if (await Msg.Question(msg))
                {
                  //  IUser user = ServiceProvider.Instance.Get<IBridgeManager>
                  //(Loginer.LoginerUser.ServerBridgeType).GetUserManager();
                  //  var r = await user.Logout(Loginer.LoginerUser.Account);
                  //  if (r.Success)
                  //      this.Close();
                  //  else
                  //      Msg.Error(r.ErrorCode);
                    this.Close();
                }
            }));
            Messenger.Default.Register<string>(GetDialogWindow(), "MinWindow", new Action<string>((msg) => { GetDialogWindow().WindowState = WindowState.Minimized; }));
            Messenger.Default.Register<bool>(GetDialogWindow(), "MaxWindow", new Action<bool>((arg) =>
            {
                var win = GetDialogWindow();
                if (win.WindowState == WindowState.Maximized)
                    win.WindowState = WindowState.Normal;
                else
                    win.WindowState = WindowState.Maximized;
            }));
        }

        private Window GetDialogWindow()
        {
            return GetDialog() as Window;
        }
    }
}
