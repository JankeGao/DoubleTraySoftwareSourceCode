using System;
using System.Collections.Generic;
using System.IO;
using Bussiness.Contracts;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HP.Core.Dependency;
using HP.Utility.Data;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Helpers;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.LogicCore.Interface;
using wms.Client.Model.ResponseModel;
using System.Runtime.InteropServices;


namespace wms.Client.ViewModel
{
    /// <summary>
    /// 登录逻辑处理
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
       
        /// <summary>
        /// 仓库契约
        /// </summary>
        private readonly IWareHouseContract WareHouseContract;

        #region 用户名/密码

        private string _Report;
        private string userName = string.Empty;
        private string passWord = string.Empty;
        private string orguserName = string.Empty;
        private string orgpassWord = string.Empty;
        private bool _IsCancel = true;
        private bool _UserChecked;

        public LoginViewModel()
        {
            WareHouseContract = IocResolver.Resolve<IWareHouseContract>();
            this.ReadConfigInfo();
            orguserName = userName;
            orgpassWord = passWord;
        }

        /// <summary>
        /// 数据库访问类型
        /// </summary>
        public string ServerType { get; set; }

        /// <summary>
        /// 数据库访问类型
        /// </summary>
        public bool UIDCheck { get; set; }

        /// <summary>
        /// 皮肤样式
        /// </summary>
        public string SkinName { get; set; }

        /// <summary>
        /// 进度报告
        /// </summary>
        public string Report
        {
            get { return _Report; }
            set { _Report = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 记住密码
        /// </summary>
        public bool UserChecked
        {
            get { return _UserChecked; }
            set { _UserChecked = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return passWord; }
            set { passWord = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 禁用按钮
        /// </summary>
        public bool IsCancel
        {
            get { return _IsCancel; }
            set { _IsCancel = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 命令(Binding Command)

        private RelayCommand _signCommand;

        public RelayCommand SignCommand
        {
            get
            {
                if (_signCommand == null)
                {
                    _signCommand = new RelayCommand(() => Login());
                }
                return _signCommand;
            }
        }

        private RelayCommand _exitCommand;

        public RelayCommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(() => ApplicationShutdown());
                }
                return _exitCommand;
            }
        }

        #endregion

        #region Login/Exit

        /// <summary>
        /// 登陆系统
        /// </summary>
        public async void Login()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
                {
                    this.Report = "请输入货柜编号及序列号";
                    return;
                }

      
                this.IsCancel = false;


                this.Report = "正在验证客户端序列号 . . .";

                if (orgpassWord!= passWord || orguserName!=userName) {
                    UIDCheck = false;
                }

                // 如果未经过验证
                if (!UIDCheck)
                {
                    DataResult loginResult = WareHouseContract.CheckUID(UserName, Password);

                    if (loginResult.Success)
                    {
                        SaveLoginInfo();

                        #region 设置客户端权限

                        // 先默认是Admin
                        Loginer.LoginerUser.IsAdmin = true;

                        var auth = new AuthorityEntity()
                        {
                            account = "admin",
                            groupName = "aaa",
                            menuName = "bbb",
                            parentName = "aaa",
                            menuCaption = "测试",
                            menuNameSpace = "aa",
                        };

                        var list = new List<AuthorityEntity>();
                        list.Add(auth);


                        Loginer.LoginerUser.authorityEntity = list;

                        this.Report = "加载客户端信息 . . .";

                        var dialog = ServiceProvider.Instance.Get<IModelDialog>("MainViewDlg");
                        dialog.BindDefaultViewModel();
                        Messenger.Default.Send(string.Empty, "ApplicationHiding");
                        bool taskResult = await dialog.ShowDialog();
                        this.ApplicationShutdown();

                        #endregion
                    }
                    else
                        this.Report = loginResult.Message;

                }
                else
                {
                    // 先默认是Admin
                    Loginer.LoginerUser.IsAdmin = true;

                    var auth = new AuthorityEntity()
                    {
                        account = "admin",
                        groupName = "aaa",
                        menuName = "bbb",
                        parentName = "aaa",
                        menuCaption = "测试",
                        menuNameSpace = "aa",
                    };

                    var list = new List<AuthorityEntity>();
                    list.Add(auth);


                    Loginer.LoginerUser.authorityEntity = list;

                    this.Report = "加载客户端信息 . . .";

                    //FaceInfoViewModel faceInfoViewModel = new FaceInfoViewModel();
                    //faceInfoViewModel.DeviceLogin();
                    //faceInfoViewModel.SendInfo();
                    //faceInfoViewModel.btn_SetAlarm_Click();
                    var dialog = ServiceProvider.Instance.Get<IModelDialog>("MainViewDlg");
                    dialog.BindDefaultViewModel();
                    Messenger.Default.Send(string.Empty, "ApplicationHiding");
                    bool taskResult = await dialog.ShowDialog();
                    this.ApplicationShutdown();
                }


                #endregion
            }
            catch (Exception ex)
            {
                this.Report = ex.Message;
            }
            finally
            {
                this.IsCancel = true;
            }
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        public void ApplicationShutdown()
        {
            Messenger.Default.Send("", "ApplicationShutdown");
        }


        #region 记住密码

        /// <summary>
        /// 读取本地配置信息
        /// </summary>
        public void ReadConfigInfo()
        {
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
            if (File.Exists(cfgINI))
            {
                IniFile ini = new IniFile(cfgINI);
                UserName = ini.IniReadValue("ClientInfo", "code");
                Password = CEncoder.Decode(ini.IniReadValue("ClientInfo", "uid"));
                UserChecked = ini.IniReadValue("ClientInfo", "SaveInfo") == "Y";
                SkinName = ini.IniReadValue("Skin", "Skin");
                UIDCheck = ini.IniReadValue("ClientInfo", "Checked") == "Y";
            }
        }

        /// <summary>
        /// 保存货柜信息
        /// </summary>
        private void SaveLoginInfo()
        {
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
            IniFile ini = new IniFile(cfgINI);
            ini.IniWriteValue("ClientInfo", "code", UserName); // 货柜编码
            ini.IniWriteValue("ClientInfo", "uid", CEncoder.Encode(Password)); // 序列号
            ini.IniWriteValue("ClientInfo", "SaveInfo", "Y"); // 保存序列号
            ini.IniWriteValue("ClientInfo", "Checked", "Y"); // 保存序列号
        }

        #endregion
    }
}
