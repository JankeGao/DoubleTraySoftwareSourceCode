using System;
using System.Windows;
using System.Windows.Controls;



namespace wms.Client.View
{


    /// <summary>
    /// SkinWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GuideWindow : UserControl
    {
        public delegate void SxDelegate();//声明委托  
        public event SxDelegate SxEvent;//声明事件

        public GuideWindow()
        {
            InitializeComponent();
        }


        private void UpdateOnclick(object sender, RoutedEventArgs e)
        {

            SxEvent?.Invoke();
        }


    }
}
