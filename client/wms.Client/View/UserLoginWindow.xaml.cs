using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Enums;
using wms.Client.ViewModel;

namespace wms.Client.View
{
    /// <summary>
    /// SkinWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserLoginWindow : UserControl
    {
        public delegate void GetTextHandler(string value1); //声明委托
        public GetTextHandler getTextHandler;

        public UserLoginWindow()
        {
            InitializeComponent();
        }
        public UserLoginWindow(MainWindow mainWindow) : this()
        {
            // 设置本窗口的DataContext。为TextBox的绑定提供源
            this.DataContext = mainWindow.DataContext;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var obj = new MainViewModel();
            if (obj == null) return;
            obj.ExitPage(MenuBehaviorType.ExitAllPage, "");
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }


}
