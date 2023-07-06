using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Bussiness.Contracts;
using Bussiness.Enums;
using HP.Core.Dependency;
using LiveCharts;
using LiveCharts.Defaults;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.Service;
using wms.Client.ViewModel;

namespace wms.Client.UiCore.Template.DemoCharts
{
    /// <summary>
    /// MaterialCards.xaml 的交互逻辑
    /// </summary>
    public partial class MaterialCards : UserControl, INotifyPropertyChanged
    {

        private double _lastLecture;
        private double _trend;
        /// <summary>
        /// 入库任务契约
        /// </summary>
        private readonly IInTaskContract InTaskContract;
        /// <summary>
        /// 入库任务契约
        /// </summary>
        private readonly IOutTaskContract OutTaskContract;

        DispatcherTimer _mainTimer;

        public MaterialCards()
        {
            InitializeComponent();
            InTaskContract = IocResolver.Resolve<IInTaskContract>();
            OutTaskContract = IocResolver.Resolve<IOutTaskContract>();
            ReadConfigInfo();
           // GetMyPageData();
            _mainTimer = new DispatcherTimer();
            _mainTimer.Interval = TimeSpan.FromSeconds(60);
            _mainTimer.Tick += new EventHandler(_mainTimer_Tick);
            _mainTimer.IsEnabled = true;
            IsReading = false;
            DataContext = this;
            //this.InjectStopOnClick();
        }

        void _mainTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => { GetMyPageData(); }));
        }

        public bool IsReading { get; set; }

        public SeriesCollection LastHourSeries { get; set; }

        public double LastLecture
        {
            get { return _lastLecture; }
            set
            {
                _lastLecture = value;
                OnPropertyChanged("LastLecture");
            }
        }


        private void SetLecture()
        {
            var target = ((ChartValues<ObservableValue>)LastHourSeries[0].Values).Last().Value;
            var step = (target - _lastLecture) / 4;
            Task.Run(() =>
            {
                for (var i = 0; i < 4; i++)
                {
                    Thread.Sleep(100);
                    LastLecture += step;
                }
                LastLecture = target;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateOnclick(object sender, RoutedEventArgs e)
        {
        //    TimePowerChart.Update(true);
        }

        // 入库单点击
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 刷新界面
            var obj = new MainViewModel();
            if (obj == null) return;

            obj.Excute("入库任务", "InTaskDlg");
        }



        // 出库单点击
        private void Button_ClickOut(object sender, RoutedEventArgs e)
        {
            // 刷新界面
            var obj = new MainViewModel();
            if (obj == null) return;

            obj.Excute("出库任务", "OutTaskDlg");
        }

        private ObservableCollection<InTaskItem> _ModuleGroups = new ObservableCollection<InTaskItem>();
        private ObservableCollection<OutTaskItem> _ModuleOutGroups = new ObservableCollection<OutTaskItem>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<InTaskItem> ModuleGroups
        {
            get { return _ModuleGroups; }
            set { _ModuleGroups = value; OnPropertyChanged("ModuleGroups"); }
        }
        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<OutTaskItem> ModuleOutGroups
        {
            get { return _ModuleOutGroups; }
            set { _ModuleOutGroups = value; OnPropertyChanged("ModuleOutGroups"); }
        }

        public void InjectStopOnClick()
        {
            IsReading = !IsReading;
            if (IsReading)
            {
                Task.Factory.StartNew(GetMyPageData);
            }
        }

        /// <summary>
        /// 获取本机货柜任务
        /// </summary>
        public async void GetMyPageData()
        {
            try
            {
                if (GlobalData.IsOnLine)
                {
                    var query = InTaskContract.InTaskDtos.Where(a => a.Status != (int)InTaskStatusCaption.Finished && a.Status != (int)InTaskStatusCaption.Cancel && a.ContainerCode == ContainerCode);

                    // 查询全部任务
                    var inTaskContainerList = query.OrderBy(a => a.CreatedTime).ToList();
                    if (inTaskContainerList.Count == 0)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            var inTaskItem = new InTaskItem()
                            {
                                Code = "",
                                Name = "暂无入库任务"
                            };
                            _ModuleGroups.Add(inTaskItem);
                        }
                    }

                    _ModuleGroups.Clear();
                    foreach (var intask in inTaskContainerList)
                    {
                        if (_ModuleGroups.Count < 4)
                        {
                            var inTaskItem = new InTaskItem()
                            {
                                Code = intask.Code,
                                Name = intask.InDictDescription,
                                InCode = intask.InCode,
                            };
                            _ModuleGroups.Add(inTaskItem);
                        }
                    }

                    var query1 = OutTaskContract.OutTaskDtos.Where(a => a.Status != (int)OutTaskStatusCaption.Finished && a.Status != (int)OutTaskStatusCaption.Cancel && a.ContainerCode == ContainerCode);
                    // 查询全部任务
                    var outTaskContainerList = query1.OrderBy(a => a.CreatedTime).ToList();
                    if (outTaskContainerList.Count == 0)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            var inTaskItem = new OutTaskItem()
                            {
                                Code = "",
                                Name = "暂无出库任务"
                            };
                            _ModuleOutGroups.Add(inTaskItem);
                        }
                    }
                    _ModuleOutGroups.Clear();
                    foreach (var intask in outTaskContainerList)
                    {
                        if (_ModuleOutGroups.Count < 4)
                        {
                            var inTaskItem = new OutTaskItem()
                            {
                                Code = intask.Code,
                                Name = intask.OutDictDescription,
                                InCode = intask.OutCode,
                            };
                            _ModuleOutGroups.Add(inTaskItem);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                GlobalData.IsOnLine = false;
                Msg.Error("MaterialCardsError:"+ex.Message);
            }
        }

        /// <summary>
        /// 客户端货柜编码
        /// </summary>
        private string ContainerCode = string.Empty;

        /// <summary>
        /// 读取本地配置信息--人员登录时间
        /// </summary>
        public void ReadConfigInfo()
        {
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
            if (File.Exists(cfgINI))
            {
                IniFile ini = new IniFile(cfgINI);
                ContainerCode = ini.IniReadValue("ClientInfo", "code");
            }
        }
    }
}
