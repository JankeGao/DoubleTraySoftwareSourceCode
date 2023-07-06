using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using wms.Client.Service;
using wms.Client.ViewModel;

namespace wms.Client.UiCore.Template.DemoCharts
{
    /// <summary>
    /// HomeAbout.xaml 的交互逻辑
    /// </summary>
    public partial class HomeAbout : UserControl
    {
        DispatcherTimer timer;
        public HomeAbout()
        {
            InitializeComponent();
            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(5000);
            //timer.Tick += timer1_Tick;
            //timer.Start();


          
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
          //  inChart.Child = CreateMediaElementOnWorkerThread();
        }

        private HostVisual CreateMediaElementOnWorkerThread()
        {
            var notify = new AutoResetEvent(false);
            var hostVisual = new HostVisual();

            var thread = new Thread(_ => MediaWorkerThread(hostVisual, notify));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            // Wait for the worker thread to spin up and create the VisualTarget.
            notify.WaitOne();

            return hostVisual;
        }

        private void MediaWorkerThread(HostVisual hostVisual, AutoResetEvent notify)
        {
            var visualTargetPS = new VisualTargetPresentationSource(hostVisual);
            visualTargetPS.RootVisual = createMediaElement();

            notify.Set();

            System.Windows.Threading.Dispatcher.Run();
        }
        private FrameworkElement createMediaElement()
        {
            //DemoCharts.DemoChart demoChart = new DemoChart();
            //demoChart.ChartValues = new LiveCharts.ChartValues<MeasureModel>();
            //mediaElement.Source = new Uri(@"f:\1.mp4");

            //mediaElement.Width = 200;
            //mediaElement.Height = 100;
            UserControl userControl =new  DemoChart();
           // userControl.ChartValues = new LiveCharts.ChartValues<MeasureModel>();
            return userControl;
        }
        #region 异步,刷新页面
        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //dispatcherTimer.Tick += DispatcherTimer_Tick;//绑定计时器事件
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 5);//设置时间间隔：：每隔1秒刷新页面
            //dispatcherTimer.Start();//开启运行
        }

        /// <summary>
        /// 计时器事件：异步执行[刷新当前页面]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Task.Factory.StartNew(AsynchronousRefresh);
        }

        /// <summary>
        /// 刷新当前页面方法
        /// </summary>
        public void AsynchronousRefresh()
        {
            //Task task = new Task(() => QueryCondition(index, page, true));
            //task.Start();
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            // 刷新界面
            //var obj = new MainViewModel();
            //if (obj == null) return;
            //obj.UpdatePage(GlobalData.LoginPageName, GlobalData.LoginPageCode);
        }
    }
}
