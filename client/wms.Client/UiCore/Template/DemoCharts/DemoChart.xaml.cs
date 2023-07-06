using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Bussiness.Contracts;
using Bussiness.Enums;
using HP.Core.Dependency;
using HP.Utility;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.Service;

namespace wms.Client.UiCore.Template.DemoCharts
{
    /// <summary>
    /// DemoChart.xaml 的交互逻辑
    /// </summary>
    public partial class DemoChart : UserControl, INotifyPropertyChanged
    {
        SeriesCollection seriesPie2 = new SeriesCollection();
        public SeriesCollection SeriesPie2
        {
            get
            {
                return seriesPie2;
            }

            set
            {
                seriesPie2 = value;
                OnPropertyChanged("SeriesPie2");
            }
        }
        DispatcherTimer _mainTimer;

        /// <summary>
        /// 入库任务契约
        /// </summary>
        private readonly INumAlarmContract NumAlarmContract;

        public DemoChart()
        {
            InitializeComponent();
            NumAlarmContract = IocResolver.Resolve<INumAlarmContract>();
            IsReading = false;

            DataContext = this;
            //GetMyPageData();
            _mainTimer = new DispatcherTimer();
            _mainTimer.Interval = TimeSpan.FromSeconds(60);
            _mainTimer.Tick += new EventHandler(_mainTimer_Tick);
            _mainTimer.IsEnabled = true;
            // this.InjectStopOnClick();
        }

        void _mainTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => { GetMyPageData(); }));
        }

        private double _axisMax;
        private double _axisMin;
        private string _curvalue;

        /// <summary>
        /// 播放数据
        /// </summary>
        public List<int> _doubles = new List<int>();

        public ChartValues<MeasureModel> ChartValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }


        private ObservableCollection<StockAlarmItem> _ModuleGroups = new ObservableCollection<StockAlarmItem>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<StockAlarmItem> ModuleGroups
        {
            get { return _ModuleGroups; }
            set { _ModuleGroups = value; OnPropertyChanged("ModuleGroups"); }
        }
        public string Curvalue
        {
            get { return _curvalue; }
            set
            {
                _curvalue = value;
                OnPropertyChanged("Curvalue");
            }
        }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }

        public bool IsReading { get; set; }

        public void Read()
        {
            while (IsReading)
            {
                Thread.Sleep(400);
                var now = DateTime.Now;
                int value = new Random().Next(10, 15);
                ChartValues.Add(new MeasureModel
                {
                    DateTime = now,
                    Value = value
                });
                SetAxisLimits(now);
                Curvalue = value + "%";
                //lets only use the last 150 values
                if (ChartValues.Count > 160) ChartValues.RemoveAt(0);
            }
            IsReading = false;
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(8).Ticks; // and 8 seconds behind
        }

        public void InjectStopOnClick()
        {
            IsReading = !IsReading;
            if (IsReading)
            {
                Task.Factory.StartNew(Read);
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
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
                // 是否在线
                if (GlobalData.IsOnLine)
                {
                    var query = NumAlarmContract.NumAlarmDtos.Where(a => a.Status == (int)MaterialNumStatusCaption.OverMin || a.Status == (int)MaterialNumStatusCaption.ReachedMin);

                    // 查询全部任务
                    var inTaskContainerList = query.OrderBy(a => a.CreatedTime).ToList();
                    if (inTaskContainerList.Count == 0)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            var inTaskItem = new StockAlarmItem()
                            {
                                Code = "",
                                Name = "暂无预警信息"
                            };
                            _ModuleGroups.Add(inTaskItem);
                        }
                    }

                    _ModuleGroups.Clear();
                    foreach (var intask in inTaskContainerList)
                    {
                        if (_ModuleGroups.Count < 4)
                        {
                            var inTaskItem = new StockAlarmItem()
                            {
                                Code = intask.MaterialCode,
                                Name = intask.MaterialName,
                                InCode = intask.Quantity,
                                ICON= intask.MinNum
                            };
                            _ModuleGroups.Add(inTaskItem);
                        }
                    }

                    // 原来图标显示
                    //// 读取PLC 状态信息
                    //var alarmService = ServiceProvider.Instance.Get<IDashboardService>();

                    //// 物料实体映射
                    //var inTask = alarmService.GetTopInMaterials();
                    //// 设备在线
                    //if (inTask.Result.Success)
                    //{
                    //    List<PieChartExample.TopSales> list = JsonHelper.DeserializeObject<List<PieChartExample.TopSales>>(inTask.Result.Data.ToString());
                    //    if (list.Count > 0)
                    //    {
                    //        SeriesPie2.Clear();
                    //        foreach (var n in list)
                    //        {
                    //            double value = Convert.ToDouble((decimal)n.Value);
                    //            ChartValues<double> chartvalue = new ChartValues<double>();
                    //            chartvalue.Add(value);
                    //            PieSeries series = new PieSeries();
                    //            series.Title = n.Name;
                    //            series.Values = chartvalue;
                    //            SeriesPie2.Add(series);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    GlobalData.IsOnLine = false;
                    //    Msg.Error(inTask.Result.Message);
                    //}
                }
            }
            catch (Exception ex)
            {
                GlobalData.IsOnLine = false;
                Msg.Error("首页查看数据库报错:"+ex.Message);
            }
        }

    }




    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
    /// <summary>
    /// 模块类
    /// </summary>
    public class StockAlarmItem
    {

        public string Code { get; set; }

        public decimal InCode { get; set; }
        /// <summary>
        /// 图标-IconFont
        /// </summary>
        public decimal? ICON { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 货柜编码
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 货柜品牌
        /// </summary>
        public string Brand { get; set; }


        /// <summary>
        /// 权限值
        /// </summary>
        public int? Authorities { get; set; }


    }
}
