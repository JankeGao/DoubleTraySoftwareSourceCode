using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Enums;
using wms.Client.LogicCore.Interface;
using HomeAbout = wms.Client.UiCore.Template.DemoCharts.HomeAbout;
using wms.Client.Service;
using System.Windows;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 首页
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        //public GlobalData GlobalData;
        public MainViewModel()
        {
            
            //CommandParameter = "{Binding Path=DataContext,RelativeSource={RelativeSource Mode=FindAncestor,AncestorType={x:Type TabItem}}}"
        }
        #region 模块系统

        private ModuleManager _ModuleManager;

        public ObservableCollection<PageInfo> OpenPageCollection { get; set; } = GlobalData.OpenPageCollection;
       // public object CurrentPage { get; set; } = GlobalData.CurrentPage.;

        // public ObservableCollection<PageInfo> OpenPageCollection { get; set; } = new ObservableCollection<PageInfo>();
        /// <summary>
        /// 模块管理器
        /// </summary>
        public ModuleManager ModuleManager
        {
            get { return _ModuleManager; }
        }

        #endregion

        #region 工具栏

        private PopBoxViewModel _PopBoxView;

        /// <summary>
        /// 辅助窗口
        /// </summary>
        public PopBoxViewModel PopBoxView
        {
            get { return _PopBoxView; }
        }

        private NoticeViewModel _NoticeView;

        /// <summary>
        /// 通知模块
        /// </summary>
        public NoticeViewModel NoticeView
        {
            get { return _NoticeView; }
        }

        #endregion

        #region 命令(Binding Command)

        private bool isOpen;

        /// <summary>
        /// 窗口是否显示
        /// </summary>
        public bool DialogIsOpen
        {
            get { return isOpen; }
            set { isOpen = value; RaisePropertyChanged(); }
        }


        // private object _CurrentPage;

        ///// <summary>
        ///// 当前选择页
        ///// </summary>
        //public object CurrentPage
        //{
        //    get { return _CurrentPage; }
        //    set { _CurrentPage = value; RaisePropertyChanged(); }
        //}

        //    public object GlobalData.CurrentPage { get; set; } = GlobalData.CurrentPage;

        private RelayCommand<Module> _ExcuteCommand;
        private RelayCommand<PageInfo> _ExitCommand;

        /// <summary>
        /// 打开模块
        /// </summary>
        public RelayCommand<Module> ExcuteCommand
        {
            get
            {
                if (_ExcuteCommand == null)
                {
                    _ExcuteCommand = new RelayCommand<Module>(t => Excute(t));
                }
                return _ExcuteCommand;
            }
            set { _ExcuteCommand = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 关闭页
        /// </summary>
        public RelayCommand<PageInfo> ExitCommand
        {
            get
            {
                if (_ExitCommand == null)
                {
                    _ExitCommand = new RelayCommand<PageInfo>(t => ExitPage(t));
                }
                return _ExitCommand;
            }
            set { _ExitCommand = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 初始化/页面相关

        /// <summary>
        /// 初始化首页
        /// </summary>
        public async void InitDefaultView()
        {
            //初始化工具栏,通知窗口
            _PopBoxView = new PopBoxViewModel();
            _NoticeView = new NoticeViewModel();
            //加载窗体模块
            _ModuleManager = new ModuleManager();
            await _ModuleManager.LoadModules();
            //设置系统默认首页
            var page = GlobalData.OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals("系统状态"));
            if (page == null)
            {
                //演示Demo加载默认首页,较消耗性能。 实际开发务移除患者更新开发部件。
                HomeAbout about = new HomeAbout();
                GlobalData.OpenPageCollection.Add(new PageInfo() { HeaderName = "系统状态", Body = about });
                GlobalData.CurrentPage = GlobalData.OpenPageCollection[GlobalData.OpenPageCollection.Count - 1];
            }
        }

        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="module"></param>
        private async void Excute(Module module)
        {
            try
            {
                var page = GlobalData.OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals(module.Name));
                if (page != null) { GlobalData.CurrentPage = page; return; }
                if (string.IsNullOrWhiteSpace(module.Code))
                {
                    //404页面
                    //DefaultViewPage defaultViewPage = new DefaultViewPage();
                    //OpenPageCollection.Add(new PageInfo() { HeaderName = module.Name, Body = defaultViewPage });
                    //GlobalData.CurrentPage = defaultViewPage;
                }
                else
                {
                    await Task.Factory.StartNew(() =>
                    {
                        var dialog = ServiceProvider.Instance.Get<IModel>(module.Code);
                        dialog.BindDefaultModel();
                        GlobalData.OpenPageCollection.Add(new PageInfo() { HeaderName = module.Name, Body = dialog.GetView() });
                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                }
                GlobalData.CurrentPage = GlobalData.OpenPageCollection[GlobalData.OpenPageCollection.Count - 1];
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
            finally
            {
                Messenger.Default.Send(false, "PackUp");
                GC.Collect();
            }
        }

        /// <summary>
        /// 关闭页面
        /// </summary>
        /// <param name="module"></param>
        private void ExitPage(PageInfo module)
        {
            try
            {
                var tab = GlobalData.OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals(module.HeaderName));
                if (tab.HeaderName != "系统状态") GlobalData.OpenPageCollection.Remove(tab);
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        public void ExitPage(MenuBehaviorType behaviorType, string pageName)
        {
            switch (behaviorType)
            {
                case MenuBehaviorType.ExitCurrentPage:
                    var page = GlobalData.OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals(pageName));
                    if (page.HeaderName != "系统状态") GlobalData.OpenPageCollection.Remove(page);
                    break;
                case MenuBehaviorType.ExitAllPage:
                    var pageList = GlobalData.OpenPageCollection.Where(t => t.HeaderName != "系统状态").ToList();
                    if (pageList != null)
                    {
                        pageList.ForEach(t =>
                        {
                            GlobalData.OpenPageCollection.Remove(t);
                        });
                    }
                    GlobalData.CurrentPage = GlobalData.OpenPageCollection[GlobalData.OpenPageCollection.Count - 1];
                    break;
                case MenuBehaviorType.ExitAllExcept:
                    var pageListExcept = GlobalData.OpenPageCollection.Where(t => t.HeaderName != pageName && t.HeaderName != "系统状态").ToList();
                    if (pageListExcept != null)
                    {
                        pageListExcept.ForEach(t =>
                        {
                            GlobalData.OpenPageCollection.Remove(t);
                        });
                    }
                    break;
            }
        }

        /// <summary>
        /// 刷新界面-先关闭再打开
        /// </summary>
        /// <param name="module"></param>
        public async void UpdatePage(string moduleName ,string moduleCode)
        {
            var page = GlobalData.OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals(moduleName));
            GlobalData.OpenPageCollection.Remove(page);

            await Task.Factory.StartNew(() =>
            {
                var dialog = ServiceProvider.Instance.Get<IModel>(moduleCode);
                dialog.BindDefaultModel();
                GlobalData.OpenPageCollection.Add(new PageInfo() { HeaderName = moduleName, Body = dialog.GetView() });
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            GlobalData.CurrentPage = GlobalData.OpenPageCollection[GlobalData.OpenPageCollection.Count - 1];
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="module"></param>
        public async void ClosePage(string moduleName)
        {
            var page = GlobalData.OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals(moduleName));
            if (page.HeaderName != "系统状态") GlobalData.OpenPageCollection.Remove(page);
            GlobalData.CurrentPage = GlobalData.OpenPageCollection[GlobalData.OpenPageCollection.Count - 1];
        }

        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="module"></param>
        public async void Excute(string moduleName, string moduleCode)
        {
            try
            {
                var page = GlobalData.OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals(moduleName));
                if (page != null) { GlobalData.CurrentPage = page; return; }
                if (string.IsNullOrWhiteSpace(moduleCode))
                {
                    //404页面
                    //DefaultViewPage defaultViewPage = new DefaultViewPage();
                    //OpenPageCollection.Add(new PageInfo() { HeaderName = module.Name, Body = defaultViewPage });
                    //GlobalData.CurrentPage = defaultViewPage;
                }
                else
                {
                    await Task.Factory.StartNew(() =>
                    {
                        var dialog = ServiceProvider.Instance.Get<IModel>(moduleCode);
                        dialog.BindDefaultModel();
                        GlobalData.OpenPageCollection.Add(new PageInfo() { HeaderName = moduleName, Body = dialog.GetView() });
                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                }
                GlobalData.CurrentPage = GlobalData.OpenPageCollection[GlobalData.OpenPageCollection.Count - 1];
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
            finally
            {
                Messenger.Default.Send(false, "PackUp");
                GC.Collect();
            }
        }
        #endregion

    }
}