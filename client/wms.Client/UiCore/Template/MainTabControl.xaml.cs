using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Enums;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.Service;
using wms.Client.ViewModel;

namespace wms.Client.UiCore.Template
{
    /// <summary>
    /// MainTabControl.xaml 的交互逻辑
    /// </summary>
    public partial class MainTabControl : UserControl
    {
        DispatcherTimer dTimer;
        public MainTabControl()
        {
            InitializeComponent();

        }

        private void ExitCommand(MenuBehaviorType type, string pageName)
        {
            var obj = this.DataContext as MainViewModel;
            if (obj == null) return;
            switch (type)
            {
                case MenuBehaviorType.ExitCurrentPage:
                    obj.ExitPage(MenuBehaviorType.ExitCurrentPage, pageName);
                    break;
                case MenuBehaviorType.ExitAllPage:
                    obj.ExitPage(MenuBehaviorType.ExitAllPage, pageName);
                    break;
                case MenuBehaviorType.ExitAllExcept:
                    obj.ExitPage(MenuBehaviorType.ExitAllExcept, pageName);
                    break;
            }
        }

        private void ExitCurrentPage_Click(object sender, RoutedEventArgs e)
        {
            var pageInfo = (sender as MenuItem).DataContext as PageInfo;
            ExitCommand(MenuBehaviorType.ExitCurrentPage, pageInfo.HeaderName);
        }

        /// <summary>
        /// 菜单关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitCurrent_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var HeaderName = button.CommandParameter.ToString();
            ExitCommand(MenuBehaviorType.ExitCurrentPage, HeaderName);
        }
        private void ExitAllPage_Click(object sender, RoutedEventArgs e)
        {
            var pageInfo = (sender as MenuItem).DataContext as PageInfo;
            ExitCommand(MenuBehaviorType.ExitAllPage, pageInfo.HeaderName);
        }

        private void ExitAllExcept_Click(object sender, RoutedEventArgs e)
        {
            var pageInfo = (sender as MenuItem).DataContext as PageInfo;
            ExitCommand(MenuBehaviorType.ExitAllExcept, pageInfo.HeaderName);
        }


    }
}
