using MaterialDesignThemes.Wpf;
using System.Windows.Controls;

namespace wms.Client.View
{
    /// <summary>
    /// SkinWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BarCodeView : UserControl
    {
        public BarCodeView()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
