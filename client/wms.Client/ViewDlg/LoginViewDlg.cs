using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Interface;
using wms.Client.View;
using wms.Client.ViewModel;
using wms.Client.ViewModel.Base;

namespace wms.Client.ViewDlg
{
    /// <summary>
    /// 登录窗口
    /// </summary>
    [Autofac(true)]
    public class LoginViewDlg : BaseViewDialog<Login>, IModelDialog
    {
        public override void BindDefaultViewModel()
        {
            LoginViewModel viewModel = new LoginViewModel();
            viewModel.ReadConfigInfo();
            GetDialogWindow().DataContext = viewModel;
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
            Messenger.Default.Register<string>(GetDialogWindow(), "ApplicationHiding", new Action<string>((msg) => { GetDialogWindow().Hide(); }));
            Messenger.Default.Register<string>(GetDialogWindow(), "ApplicationShutdown", new Action<string>((arg) => { Application.Current.Shutdown(); }));
        }

        private Window GetDialogWindow()
        {
            return GetDialog() as Window;
        }
    }
}
