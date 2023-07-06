using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wms.Client.View
{
    /// <summary>
    /// InTaskView.xaml 的交互逻辑
    /// </summary>
    public partial class InTaskView : UserControl
    {
        public InTaskView()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => BarCode.Focus()));
        }

        /// <summary>
        /// 选择入库任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickIntask(object sender, RoutedEventArgs e)
        {
           Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => BarCode.Focus()));
        }
        
        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void returnInTask(object sender, RoutedEventArgs e)
        {
           // Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => InOrder.Focus()));
        }

        private void Updateclick(object sender, RoutedEventArgs e)
        {

        }
    }
}
