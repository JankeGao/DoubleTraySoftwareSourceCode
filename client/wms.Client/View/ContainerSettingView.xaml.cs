using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wms.Client.Template;

namespace wms.Client.View
{
    /// <summary>
    /// ContainerSetting.xaml 的交互逻辑
    /// </summary>
    public partial class ContainerSettingView : UserControl
    {
        private int _Step;
        public int Step
        {
            get
            {
                // int temp = this.stepBar1.Progress;
                // return ++temp;
                return 1;
            }
        }
        ObservableCollection<string> list = new ObservableCollection<string>();

        public ContainerSettingView()
        {
            InitializeComponent();
        }
        private void FlatButton_Click(object sender, RoutedEventArgs e)
        {
            //this.stepBar1.Progress++;
        }
        private void FlatButton_Click1(object sender, RoutedEventArgs e)
        {
           // this.stepBar1.Progress--;
        }
        private void btn_AddItem(object sender, RoutedEventArgs e)
        {
            list.Add("进行中");
        }
        private void btn_RemoveItem(object sender, RoutedEventArgs e)
        {
            list.RemoveAt(0);
        }

        private void btn_Verification_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.OnTimerElapsed(sender, null);
        }
    }
}
