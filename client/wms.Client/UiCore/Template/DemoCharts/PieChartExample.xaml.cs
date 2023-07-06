using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using HP.Utility;
using LiveCharts;
using LiveCharts.Wpf;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.Service;

namespace wms.Client.UiCore.Template.DemoCharts
{
    /// <summary>
    /// PieChartExample.xaml 的交互逻辑
    /// </summary>
    public partial class PieChartExample : UserControl
    {

        SeriesCollection seriesPie = new SeriesCollection();

        DispatcherTimer _mainTimer;
        public SeriesCollection SeriesPie
        {
            get
            {
                return seriesPie;
            }
            set
            {
                seriesPie = value;
                OnPropertyChanged("SeriesPie");
            }
        }
        public PieChartExample()
        {
            InitializeComponent();
         //   GetMyPageData();
            IsReading = false;
            DataContext = this;
            _mainTimer = new DispatcherTimer();
            _mainTimer.Interval = TimeSpan.FromSeconds(60);
            _mainTimer.Tick += new EventHandler(_mainTimer_Tick);
            _mainTimer.IsEnabled = true;
            // this.InjectStopOnClick();
        }

        void _mainTimer_Tick(object sender, EventArgs e)
        {
          //  Dispatcher.BeginInvoke(new Action(() => { GetMyPageData(); }));
        }
        public bool IsReading { get; set; }
        public void InjectStopOnClick()
        {
            IsReading = !IsReading;
            if (IsReading)
            {
                Task.Factory.StartNew(GetMyPageData);
            }
        }
        public class TopSales
        {
            public string Name { get; set; }
            public decimal? Value { get; set; }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        List<string> labels = new List<string>();
        public List<string> Labels
        {
            get
            {
                return labels;
            }
            set
            {
                labels = value;
                OnPropertyChanged("Labels");
            }
        }


        public async void GetMyPageData()
        {

            try
            {
                if (GlobalData.IsOnLine)
                {
                    // 读取PLC 状态信息
                    var alarmService = ServiceProvider.Instance.Get<IDashboardService>();

                    // 物料实体映射
                    var inTask = alarmService.GetTopOutMaterials();
                    // 设备在线
                    if (inTask.Result.Success)
                    {
                        List<TopSales> list = JsonHelper.DeserializeObject<List<TopSales>>(inTask.Result.Data.ToString());
                        if (list.Count > 0)
                        {
                            SeriesPie.Clear();
                            foreach (var n in list)
                            {
                                double value = Convert.ToDouble((decimal)n.Value);
                                ChartValues<double> chartvalue = new ChartValues<double>();
                                chartvalue.Add(value);
                                PieSeries series = new PieSeries();
                                series.Title = n.Name;
                                series.Values = chartvalue;
                                SeriesPie.Add(series);
                            }
                        }
                    }
                    else
                    {
                        Msg.Error(inTask.Result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalData.IsOnLine = false;
                Msg.Error("pieCharExampleError:"+ex.Message);
            }
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {

            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
            {
                series.PushOut = 0;
               // series.Values=
            }
                
           

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }


    }
}
