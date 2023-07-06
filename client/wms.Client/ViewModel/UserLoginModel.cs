using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HP.Core.Dependency;
using HP.Core.Mapping;
using HP.Core.Security;
using HP.Utility;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Helpers;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.Model.ResponseModel;
using wms.Client.Service;
using wms.Client.ViewModel;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 操作人员登录
    /// </summary>
    public class UserLoginModel : ViewModelBase
    {
        public IIdentityContract IdentityContract = IocResolver.Resolve<IIdentityContract>();

        public IAuthorizationContract AuthorizationContract = IocResolver.Resolve<IAuthorizationContract>();
        public IMapper Mapper = IocResolver.Resolve<IMapper>();

        private readonly string _basePath = ConfigurationManager.AppSettings["ServerIP"];


        private readonly string _faceIP = ConfigurationManager.AppSettings["FaceIP"];
        private readonly string _facePort = ConfigurationManager.AppSettings["FacePort"];
        private readonly string _faceUser = ConfigurationManager.AppSettings["FaceUser"];
        private readonly string _facePass = ConfigurationManager.AppSettings["FacePassword"];
        //private HCNetSDK.MSGCallBack m_falarmData = null; //报警的

        //private bool m_bInitSDK = false;

        //public UserLoginModel()
        //{
        //    // 人员权限信息
        //     m_bInitSDK = HCNetSDK.NET_DVR_Init();
        //   //设置报警回调函数
        //     m_falarmData = new HCNetSDK.MSGCallBack(MsgCallback);
        //     HCNetSDK.NET_DVR_SetDVRMessageCallBack_V30(m_falarmData, IntPtr.Zero);
        //     DeviceLogin();
        //}





        #region 用户名/密码

        private string _Report;
        private string userName = string.Empty;
        private string name = string.Empty;
        private string passWord = string.Empty;
        private string _pictureUrl = string.Empty;
        private bool _IsCancel = true;
        private bool _UserChecked;

        /// <summary>
        /// 设备IP
        /// </summary>
        private string deviceIP = string.Empty;
        public string DeviceIP
        {
            get { return deviceIP; }
            set { deviceIP = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 设备端口号
        /// </summary>
        private string devicePort = string.Empty;
        public string DevicePort
        {
            get { return devicePort; }
            set { devicePort = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 设备用户名
        /// </summary>
        private string deviceName = string.Empty;
        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 设备密码
        /// </summary>
        private string devicePassword = string.Empty;
        public string DevicePassword
        {
            get { return devicePassword; }
            set { devicePassword = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 数据库访问类型
        /// </summary>
        public string ServerType { get; set; }

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
        /// 用户名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
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


        /// <summary>
        /// 禁用按钮
        /// </summary>
        public string PictureUrl
        {
            get { return _pictureUrl; }
            set { _pictureUrl = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 命令(Binding Command)

        private RelayCommand _userLoginCommand;

        public RelayCommand UserLoginCommand
        {
            get
            {
                if (_userLoginCommand == null)
                {
                    _userLoginCommand = new RelayCommand(() => UserLogin());
                }
                return _userLoginCommand;
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




        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使
            // 用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                pwd = pwd + s[i].ToString("x2");
            }
            return pwd;
        }

        public async void DeviceLogin()
        {
            //DeviceIP = _faceIP;
            //DevicePort = _facePort;
            //DeviceName = _faceUser;
            //DevicePassword = _facePass;

            //if (String.IsNullOrEmpty(DeviceIP))
            //{
            //    Msg.Warning("请输入人脸识别设备IP地址！");
            //    return;
            //}
            //if (String.IsNullOrEmpty(DevicePort))
            //{
            //    Msg.Warning("请输入人脸识别设备端口号！");
            //    return;
            //}
            //if (String.IsNullOrEmpty(DeviceName))
            //{
            //    Msg.Warning("请输入人脸识别设备登录用户名！");
            //    return;
            //}
            //if (String.IsNullOrEmpty(DevicePassword))
            //{
            //    Msg.Warning("请输入人脸识别设备登录密码！");
            //    return;
            //}
            //if (!Login(true))
            //{
            //    Msg.Warning("人脸设备登录失败！请检查配置信息！");
            //    return;
            //}
            //else
            //{
            //    btn_SetAlarm_Click();
            //}
        }


        //private Int32[] m_lAlarmHandle = new Int32[200];
        //private void btn_SetAlarm_Click()
        //{
        //    HCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam = new HCNetSDK.NET_DVR_SETUPALARM_PARAM();
        //    struAlarmParam.dwSize = (uint)Marshal.SizeOf(struAlarmParam);
        //    struAlarmParam.byLevel = 0; //0- 一级布防,1- 二级布防
        //    m_lAlarmHandle[m_lUserID] = HCNetSDK.NET_DVR_SetupAlarmChan_V41(m_lUserID, ref struAlarmParam);
        //    if (m_lAlarmHandle[m_lUserID] < 0)
        //    {
        //        uint iLastErr = HCNetSDK.NET_DVR_GetLastError();
        //        //label2.Text = "布防失败，错误号：" + iLastErr; ;
        //    }
        //    else
        //    {
        //        //label2.Text = "布防成功";
        //        //btn_SetAlarm.Enabled = false;
        //        //btnCloseAlarm.Enabled = true;
        //    }
        //}

        /// <summary>
        /// 登陆系统
        /// </summary>
        public async void UserLogin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
                {
                    this.Report = "请输入用户名密码";
                    return;
                }

                if (UserName == "admin" && Password == "admin@123")
                {
                    GlobalData.OutLineUse = true;
                    GlobalData.loginTime = DateTime.Now.ToString();
                    GlobalData.UserCode = "admin";
                    GlobalData.UserName = "超级管理员";
                    // 刷新界面
                    var obj = new MainViewModel();
                    if (obj == null) return;
                    obj.UpdatePage(GlobalData.LoginPageName, GlobalData.LoginPageCode);

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

                    DialogHost.CloseDialogCommand.Execute(null, null);
                    return;
                }
                else
                {
                    if (!GlobalData.IsOnLine)
                    {
                        this.Report = "无法连接至远程服务器，请输入离线操作密码 ";
                        return;
                    }
                }
                this.Report = "正在验证登录 . . .";
                var user = ServiceProvider.Instance.Get<IUserService>();

                //  var IdentityContract = ServiceProvider.Instance.Get<IIdentityContract>();
                //  public IIdentityContract IdentityContract { set; get; }

                // 系统登录
                var inputDto = new LoginInfo() { Code = UserName, Password = MD5Encrypt32(Password).ToUpper() };

                // DataResult loginResult = IdentityContract.Login(inputDto);
                MD5 md5 = new MD5CryptoServiceProvider();//创建MD5对象（MD5类为抽象类不能被实例化）
                var LoginTask = user.LoginAsync(inputDto);
                var timeouttask = Task.Delay(10000);
                var completedTask = await Task.WhenAny(LoginTask, timeouttask);
                if (completedTask == timeouttask)
                {
                    this.Report = "系统连接超时,请联系管理员!";
                }
                else
                {
                    var task = await LoginTask;
                    if (task.Success)
                    {
                        if (task.Data == null)
                        {
                            this.Report = task.Message;
                            return;
                        }
                        // IdentityTicket entity = (IdentityTicket)task.Data;

                        IdentityTicket entity = JsonHelper.DeserializeObject<IdentityTicket>(task.Data.ToString());
                        UserData req = (UserData)entity.UserData;

                        #region 设置用户基础信息
                   
                        Loginer.LoginerUser.Token = entity.Token;

                        // 核查用户是否有此模块操作权限
                        var authCheck = user.GetCheckAuth(GlobalData.LoginModule);
                        if (!authCheck.Result.Success)
                        {
                            this.Report = "抱歉，您无该模块操作权限";
                            return;
                        }

                        GlobalData.loginTime = DateTime.Now.ToString();
                        GlobalData.UserCode = req.Code;
                        GlobalData.UserName = req.Name;
                        GlobalData.PictureUrl= req.Header;

                        // 在线使用
                        GlobalData.OutLineUse = false;
                        this.Report = "加载用户信息 . . .";

                        // 刷新界面
                        var obj = new MainViewModel();
                        if (obj == null) return;
                        obj.UpdatePage(GlobalData.LoginPageName, GlobalData.LoginPageCode);


                        DialogHost.CloseDialogCommand.Execute(null, null);
                        #endregion
                    }
                    else
                        this.Report = task.Message;
                }


                #endregion
            }
            catch (Exception ex)
            {
                this.Report = ex.Message;
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
                UserName = ini.IniReadValue("Login", "User");
                Password = CEncoder.Decode(ini.IniReadValue("Login", "Password"));
                UserChecked = ini.IniReadValue("Login", "SaveInfo") == "Y";
                SkinName = ini.IniReadValue("Skin", "Skin");
                ServerType = ini.IniReadValue("Server", "Bridge");
            }
        }


        //public int m_iDeviceIndex = -1;
        //HCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo;
        //public int m_iUserID = -1;
        //private uint m_AysnLoginResult = 0;
        //private bool LoginCallBackFlag = false;
        //private bool AysnLoginFlag = false;
        //public Int32 m_lSetCardCfgHandle = 0;
        //public Int32 m_lSetFaceHandle = 0;
        //public int m_lUserID = -1;
        //private HCNetSDK.RemoteConfigCallback g_fSetFaceCallback = null;

        //private HCNetSDK.RemoteConfigCallback g_fSetGatewayCardCallback = null;

        //private HCNetSDK.NET_DVR_CARD_CFG_V50 m_struCardInfo = new HCNetSDK.NET_DVR_CARD_CFG_V50();

        //public bool Login(bool bStatus)//true said add node login, false for the existing node to log in 
        //{
        //    try
        //    {
        //        LoginCallBackFlag = false;
        //        m_struDeviceInfo = new HCNetSDK.NET_DVR_DEVICEINFO_V30();

        //        HCNetSDK.NET_DVR_DEVICEINFO_V30 struDeviceInfo = new HCNetSDK.NET_DVR_DEVICEINFO_V30();
        //        struDeviceInfo.sSerialNumber = new byte[HCNetSDK.SERIALNO_LEN];

        //        HCNetSDK.NET_DVR_NETCFG_V50 struNetCfg = new HCNetSDK.NET_DVR_NETCFG_V50();
        //        struNetCfg.Init();
        //        HCNetSDK.NET_DVR_DEVICECFG_V40 struDevCfg = new HCNetSDK.NET_DVR_DEVICECFG_V40();
        //        struDevCfg.sDVRName = new byte[HCNetSDK.NAME_LEN];
        //        struDevCfg.sSerialNumber = new byte[HCNetSDK.SERIALNO_LEN];
        //        struDevCfg.byDevTypeName = new byte[HCNetSDK.DEV_TYPE_NAME_LEN];
        //        HCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new HCNetSDK.NET_DVR_USER_LOGIN_INFO();
        //        HCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new HCNetSDK.NET_DVR_DEVICEINFO_V40();
        //        struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[HCNetSDK.SERIALNO_LEN];
        //        uint dwReturned = 0;
        //        int lUserID = -1;

        //        struLoginInfo.bUseAsynLogin = AysnLoginFlag;
        //        struLoginInfo.cbLoginResult = new HCNetSDK.LoginResultCallBack(AsynLoginMsgCallback);

        //        if (bStatus)
        //        {
        //            struLoginInfo.sDeviceAddress = DeviceIP; // 设备IP地址
        //            struLoginInfo.sUserName = DeviceName;
        //            struLoginInfo.sPassword = DevicePassword;
        //            ushort.TryParse(DevicePort, out struLoginInfo.wPort);
        //        }

        //        lUserID = HCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
        //        if (struLoginInfo.bUseAsynLogin)
        //        {
        //            for (int i = 0; i < 1000; i++)
        //            {
        //                if (!LoginCallBackFlag)
        //                {
        //                    Thread.Sleep(5);
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //            if (!LoginCallBackFlag)
        //            {
        //            }
        //            if (m_AysnLoginResult == 1)
        //            {
        //                lUserID = m_iUserID;
        //                struDeviceInfoV40.struDeviceV30 = m_struDeviceInfo;
        //            }
        //            else
        //            {
        //                return false;
        //            }

        //        }

        //        if (lUserID < 0)
        //        {
        //            uint nErr = HCNetSDK.NET_DVR_GetLastError();
        //            string strTemp = string.Format("NET_DVR_Login_V40 [{0}]", DeviceIP);
        //            if (nErr == HCNetSDK.NET_DVR_PASSWORD_ERROR)
        //            {
        //                MessageBox.Show("user name or password error!");
        //                if (1 == struDeviceInfoV40.bySupportLock)
        //                {
        //                    string strTemp1 = string.Format("Left {0} try opportunity", struDeviceInfoV40.byRetryLoginTime);
        //                    MessageBox.Show(strTemp1);
        //                }
        //            }
        //            else if (nErr == HCNetSDK.NET_DVR_USER_LOCKED)
        //            {
        //                if (1 == struDeviceInfoV40.bySupportLock)
        //                {
        //                    string strTemp1 = string.Format("user is locked, the remaining lock time is {0}", struDeviceInfoV40.dwSurplusLockTime);
        //                    MessageBox.Show(strTemp1);
        //                }
        //            }
        //            else
        //            {
        //                MessageBox.Show("net error or dvr is busy!");
        //            }
        //            return false;
        //        }
        //        else
        //        {
        //            if (1 == struDeviceInfoV40.byPasswordLevel)
        //            {
        //                MessageBox.Show("default password, please change the password");
        //            }
        //            else if (3 == struDeviceInfoV40.byPasswordLevel)
        //            {
        //                MessageBox.Show("risk password, please change the password");
        //            }
        //            struDeviceInfo = struDeviceInfoV40.struDeviceV30;
        //        }

        //        if (bStatus)
        //        {

        //        }

        //        if (1 == (struDeviceInfo.bySupport & 0x80))
        //        {
        //        }
        //        else
        //        {
        //        }

        //        uint dwSize = (uint)Marshal.SizeOf(struNetCfg);
        //        IntPtr ptrNetCfg = Marshal.AllocHGlobal((int)dwSize);
        //        Marshal.StructureToPtr(struNetCfg, ptrNetCfg, false);

        //        if (!HCNetSDK.NET_DVR_GetDVRConfig(lUserID, HCNetSDK.NET_DVR_GET_NETCFG_V50, 0, ptrNetCfg, dwSize, ref dwReturned))
        //        {

        //        }
        //        else
        //        {
        //            //IPv6 temporary unrealized 
        //            struNetCfg = (HCNetSDK.NET_DVR_NETCFG_V50)Marshal.PtrToStructure(ptrNetCfg, typeof(HCNetSDK.NET_DVR_NETCFG_V50));
        //            string strTemp = string.Format("multi-cast ipv4 {0}", struNetCfg.struMulticastIpAddr.sIpV4);
        //        }
        //        Marshal.FreeHGlobal(ptrNetCfg);

        //        dwReturned = 0;
        //        uint dwSize2 = (uint)Marshal.SizeOf(struDevCfg);
        //        IntPtr ptrDevCfg = Marshal.AllocHGlobal((int)dwSize2);
        //        Marshal.StructureToPtr(struDevCfg, ptrDevCfg, false);

        //        if (!HCNetSDK.NET_DVR_GetDVRConfig(lUserID, HCNetSDK.NET_DVR_GET_DEVICECFG_V40, 0, ptrDevCfg, dwSize2, ref dwReturned))
        //        {
        //        }
        //        else
        //        {
        //            struDevCfg = (HCNetSDK.NET_DVR_DEVICECFG_V40)Marshal.PtrToStructure(ptrDevCfg, typeof(HCNetSDK.NET_DVR_DEVICECFG_V40));
        //        }
        //        Marshal.FreeHGlobal(ptrDevCfg);

        //        //if (DoGetDeviceResoureCfg(m_iDeviceIndex))
        //        //{

        //        //}
        //        m_iUserID = lUserID;
        //        m_lUserID = lUserID;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (m_bInitSDK == true)
        //        {
        //            HCNetSDK.NET_DVR_Cleanup();
        //        }
        //        return false;
        //    }
        //}

        //public void AsynLoginMsgCallback(Int32 lUserID, UInt32 dwResult, ref HCNetSDK.NET_DVR_DEVICEINFO_V30 lpDeviceInfo, IntPtr pUser)
        //{
        //    if (dwResult == 1)
        //    {
        //        m_struDeviceInfo = lpDeviceInfo;
        //    }
        //    m_AysnLoginResult = dwResult;
        //    m_iUserID = lUserID;
        //    LoginCallBackFlag = true;
        //}

        //#endregion

        //#region 报警回调以及具体报警解析
        //public void MsgCallback(int lCommand, ref HCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        //{
        //    //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
        //    switch (lCommand)
        //    {
        //        case HCNetSDK.COMM_ALARM_ACS://门禁主机报警信息
        //            ProcessCommAlarm_ACS(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
        //            break;
        //        default:
        //            break;
        //    }
        //}
        //public void UpdateClientList(string strAlarmMsg)
        //{
        //    //列表新增报警信息
        //    // listBox1.Items.Add(strAlarmMsg);
        //}
        //private void ProcessCommAlarm_ACS(ref HCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        //{
        //    try
        //    {
        //        string strAlarmMsg = "";
        //        HCNetSDK.NET_DVR_ACS_ALARM_INFO acsInfo = new HCNetSDK.NET_DVR_ACS_ALARM_INFO();
        //        uint dwSize = (uint)Marshal.SizeOf(acsInfo);
        //        acsInfo = (HCNetSDK.NET_DVR_ACS_ALARM_INFO)Marshal.PtrToStructure(pAlarmInfo, typeof(HCNetSDK.NET_DVR_ACS_ALARM_INFO));


        //        //报警设备IP地址
        //        strAlarmMsg += "ip:【" + pAlarmer.sDeviceIP + "】";
        //        //设备序列号
        //        if (pAlarmer.bySerialValid == 1)
        //        {
        //            string SN = Encoding.ASCII.GetString(pAlarmer.sSerialNumber).Replace("\0", "");
        //            string name = pAlarmer.sDeviceName;
        //            SN = SN.Substring(SN.Length - 9);
        //            strAlarmMsg += "name:【" + name + "】";
        //            strAlarmMsg += "SN:【" + SN + "】";
        //        }
        //        //主类型
        //        strAlarmMsg += "主类型:【" + acsInfo.dwMajor + "】";
        //        //次类型
        //        strAlarmMsg += "次类型:【" + acsInfo.dwMinor + "】"; //0x4b 人脸认证通过
        //        //报警时间
        //        string datetime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", acsInfo.struTime.dwYear.ToString(), acsInfo.struTime.dwMonth.ToString(), acsInfo.struTime.dwDay.ToString(), acsInfo.struTime.dwHour.ToString(), acsInfo.struTime.dwMinute.ToString(), acsInfo.struTime.dwSecond.ToString());
        //        strAlarmMsg += "时间:【" + DateTime.Parse(datetime) + "】";

        //        var valtime = DateTime.Now.AddMinutes(-2);

        //        // 判断是否为今天的布防时间
        //        if (DateTime.Parse(datetime) > valtime)
        //        {
        //            var rfid = Encoding.ASCII.GetString(acsInfo.struAcsEventInfo.byCardNo).Replace("\0", "");
        //            // 是1分钟内发生的
        //            if (!String.IsNullOrEmpty(rfid))
        //            {
        //                // 有人刷卡登录了
        //                MessageBox.Show("卡号登录了！" + rfid);
        //            }
        //        }

        //        //网络操作用户名
        //        //acb.sNetUser = Encoding.ASCII.GetString(acsInfo.sNetUser) + "】";
        //        //主机地址
        //        strAlarmMsg += "IP:【" + acsInfo.struRemoteHostAddr.sIpV4 + "】";
        //        //图片数据大小
        //        //acb.dwPicDataLen = acsInfo.dwPicDataLen + "】";

        //        //报警信息详细参数
        //        //卡号
        //        strAlarmMsg += "卡号:【" + Encoding.ASCII.GetString(acsInfo.struAcsEventInfo.byCardNo).Replace("\0", "") + "】";
        //        /*//卡类型
        //        acb.AcsEventInfo_obj.byCardType = acsInfo.struAcsEventInfo.byCardType;
        //        //白名单单号
        //        acb.AcsEventInfo_obj.byWhiteListNo = acsInfo.struAcsEventInfo.byWhiteListNo;
        //        //报告上传通道
        //        acb.AcsEventInfo_obj.byReportChannel = acsInfo.struAcsEventInfo.byReportChannel;
        //        //读卡器类型
        //        acb.AcsEventInfo_obj.byCardReaderKind = acsInfo.struAcsEventInfo.byCardReaderKind;
        //        //读卡器编号
        //        acb.AcsEventInfo_obj.dwCardReaderNo = acsInfo.struAcsEventInfo.dwCardReaderNo;
        //        //门编号
        //        acb.AcsEventInfo_obj.dwDoorNo = acsInfo.struAcsEventInfo.dwDoorNo;
        //        //多重卡认证序号
        //        acb.AcsEventInfo_obj.dwVerifyNo = acsInfo.struAcsEventInfo.dwVerifyNo;
        //        //报警输入号
        //        acb.AcsEventInfo_obj.dwAlarmInNo = acsInfo.struAcsEventInfo.dwAlarmInNo;
        //        //报警输出号
        //        acb.AcsEventInfo_obj.dwAlarmOutNo = acsInfo.struAcsEventInfo.dwAlarmOutNo;
        //        //事件触发器编号
        //        acb.AcsEventInfo_obj.dwCaseSensorNo = acsInfo.struAcsEventInfo.dwCaseSensorNo;
        //        //RS485通道号
        //        acb.AcsEventInfo_obj.dwRs485No = acsInfo.struAcsEventInfo.dwRs485No;
        //        //群组编号
        //        acb.AcsEventInfo_obj.dwMultiCardGroupNo = acsInfo.struAcsEventInfo.dwMultiCardGroupNo;
        //        //人员通道号
        //        acb.AcsEventInfo_obj.wAccessChannel = acsInfo.struAcsEventInfo.wAccessChannel;
        //        //设备编号
        //        acb.AcsEventInfo_obj.byDeviceNo = acsInfo.struAcsEventInfo.byDeviceNo;
        //        //分控器编号
        //        acb.AcsEventInfo_obj.byDistractControlNo = acsInfo.struAcsEventInfo.byDistractControlNo;
        //        //工号
        //        acb.AcsEventInfo_obj.dwEmployeeNo = acsInfo.struAcsEventInfo.dwEmployeeNo;
        //        //就地控制器编号
        //        acb.AcsEventInfo_obj.wLocalControllerID = acsInfo.struAcsEventInfo.wLocalControllerID;
        //        //网口ID
        //        acb.AcsEventInfo_obj.byInternetAccess = acsInfo.struAcsEventInfo.byInternetAccess;
        //        //防区类型
        //        acb.AcsEventInfo_obj.byType = acsInfo.struAcsEventInfo.byType;
        //        //物理地址
        //        acb.AcsEventInfo_obj.byMACAddr = acsInfo.struAcsEventInfo.byMACAddr;
        //        //刷卡类型
        //        acb.AcsEventInfo_obj.bySwipeCardType = acsInfo.struAcsEventInfo.bySwipeCardType;
        //        //保留
        //        acb.AcsEventInfo_obj.byRes2 = acsInfo.struAcsEventInfo.byRes2;
        //        //事件流水号
        //        acb.AcsEventInfo_obj.dwSerialNo = acsInfo.struAcsEventInfo.dwSerialNo;*/
        //        //string result = JsonConvert.SerializeObject(acb);
        //        //ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + @"\Alarm", "报警", result);
        //        UpdateClientList(strAlarmMsg);
        //    }
        //    catch (Exception ex)
        //    {
        //        //todo 异常记录
        //    }
        //}
        #endregion

    }
}
