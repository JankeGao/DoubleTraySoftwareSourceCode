using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using wms.Client.LogicCore.Common;
namespace wms.Client.Service
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public class GlobalData : ViewModelBase
    {
        /// <summary>
        /// 远程服务器是否在线
        /// </summary>

        private static bool _OutLineUse = false;
        public static bool OutLineUse
        {
            get
            {
                return _OutLineUse;
            }
            set
            {
                _OutLineUse = value;
            }
        }


        /// <summary>
        /// UserName
        /// </summary>
        private static bool _IsOnLine = true;
        public static bool IsOnLine
        {
            get
            {
                return _IsOnLine;
            }
            set
            {
                _IsOnLine = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsOnLine)));
            }
        }

        /// <summary>
        /// 设备报警状态
        /// </summary>
        public static int AlarmStatus { get; set; }


        /// <summary>
        /// 报警提示框
        /// </summary>

        private static bool _IsComfirm = false;
        public static bool Comfirm
        {
            get
            {
                return _IsComfirm;
            }
            set
            {
                _IsComfirm = value;
            }
        }

        /// <summary>
        /// 设备状态
        /// </summary>
        public static int DeviceStatus { get; set; }

        /// <summary>
        /// 当前登录模块
        /// </summary>
        public static string LoginModule { get; set; }

        public static string LoginPageCode { get; set; }
        public static string LoginPageName { get; set; }

        public static string Token { get; set; }

        /// <summary>
        /// 指引单据
        /// </summary>
        public static string GuideCode { get; set; }
        /// <summary>
        /// 指引类别
        /// </summary>
        public static int GuideType { get; set; }
        public static int TabPageIndex { get; set; }
        public static ObservableCollection<PageInfo> OpenPageCollection { get; set; } = new ObservableCollection<PageInfo>();


        /// <summary>
        /// 新建静态属性变更通知
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        private static object _testValue = new object();
        public static object CurrentPage
        {
            get
            {
                return _testValue;
            }
            set
            {
                _testValue = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentPage)));
            }
        }

        /// <summary>
        /// UserCode
        /// </summary>
        private static string _UserCode = string.Empty;
        public static string UserCode
        {
            get
            {
                return _UserCode;
            }
            set
            {
                _UserCode = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(UserCode)));
            }
        }


        /// <summary>
        /// UserName
        /// </summary>
        private static string _UserName = string.Empty;
        public static string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(UserName)));
            }
        }

        /// <summary>
        /// UserName
        /// </summary>
        private static string _PictureUrl = string.Empty;
        public static string PictureUrl
        {
            get
            {
                return _PictureUrl;
            }
            set
            {
                _PictureUrl = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(PictureUrl)));
            }
        }

        /// <summary>
        /// UserName
        /// </summary>
        private static string _loginTime = string.Empty;
        public static string loginTime
        {
            get
            {
                return _loginTime;
            }
            set
            {
                _loginTime = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(loginTime)));
            }
        }

        /// <summary>
        /// UserName
        /// </summary>
        private static bool _IsFocus = true;
        public static bool IsFocus
        {
            get
            {
                return _IsFocus;
            }
            set
            {
                _IsFocus = value;
                //调用通知
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsFocus)));
            }
        }

    }
}
