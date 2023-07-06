using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Bussiness.Dtos;
using Bussiness.Entitys;
using GalaSoft.MvvmLight.Command;
using HP.Core.Dependency;
using HPC.BaseService.Contracts;
using wms.Client.Common;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Enums;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.LogicCore.Interface;
using wms.Client.View;
using wms.Client.ViewModel.Base;
using System.Windows.Interop;
using HP.Core.Security;
using HP.Utility;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;
using wms.Client.Service;
using wms.Client.ViewModel;
using MaterialDesignThemes.Wpf;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 人脸信息下发
    /// </summary>
    [Module(ModuleType.SystemSettings, "FaceInfoDlg", "权限下发")]
    public class FaceInfoViewModel : DataProcess<DeviceAlarm>
    {
        private readonly string _faceIP = ConfigurationManager.AppSettings["FaceIP"];
        private readonly string _facePort = ConfigurationManager.AppSettings["FacePort"];
        private readonly string _faceUser = ConfigurationManager.AppSettings["FaceUser"];
        private readonly string _facePass = ConfigurationManager.AppSettings["FacePassword"];
        public static HCNetSDK.MSGCallBack m_falarmData = null; //报警的


        /// <summary>
        /// 人员权限信息
        /// </summary>
        private readonly IIdentityContract IdentityContract;

        private bool m_bInitSDK = false;
        public FaceInfoViewModel()
        {
            // 人员权限信息
            IdentityContract = IocResolver.Resolve<IIdentityContract>();
            LoginOutCommand = new RelayCommand<string>(LoginOut);
            m_bInitSDK = HCNetSDK.NET_DVR_Init();
            this.ReadConfigInfo();
            //this.ShowLogin();
            //设置报警回调函数
            m_falarmData = new HCNetSDK.MSGCallBack(MsgCallback);
            IntPtr intPtr = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);

            HCNetSDK.NET_DVR_SetDVRMessageCallBack_V30(m_falarmData, intPtr);// IntPtr.Zero
        }

        private bool _IsCancel = true;

        /// <summary>
        /// 禁用按钮
        /// </summary>
        public bool IsCancel
        {
            get { return _IsCancel; }
            set { _IsCancel = value; RaisePropertyChanged(); }
        }



        #region 任务组

        private ObservableCollection<User> _UserList = new ObservableCollection<User>();

        /// <summary>
        /// 已加载模块<含分组>
        /// </summary>
        public ObservableCollection<User> UserList
        {
            get { return _UserList; }
            set { _UserList = value; RaisePropertyChanged(); }
        }


        #endregion

        #region 命令(Binding Command)

        private RelayCommand _deviceLoginCommand;

        /// <summary>
        /// 门禁设备登录
        /// </summary>
        public RelayCommand DeviceLoginCommand
        {
            get
            {
                if (_deviceLoginCommand == null)
                {
                    _deviceLoginCommand = new RelayCommand(() => DeviceLogin());
                }
                return _deviceLoginCommand;
            }
        }


        private RelayCommand _SetAlarmCommand;

        /// <summary>
        /// 门禁设备登录
        /// </summary>
        public RelayCommand SetAlarmCommand
        {
            get
            {
                if (_SetAlarmCommand == null)
                {
                    _SetAlarmCommand = new RelayCommand(() => btn_SetAlarm_Click());
                }
                return _SetAlarmCommand;
            }
        }

        

        private RelayCommand _scanBarcodeCommand;

        /// <summary>
        /// 扫描入库条码
        /// </summary>
        public RelayCommand<string> ScanBarcodeCommand { get; private set; }



        private RelayCommand _inTaskCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> InTaskCommand { get; private set; }



        private RelayCommand _selectItemCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<InTaskMaterialDto> SelectItemCommand { get; private set; }


        /// <summary>
        /// 确认存入指令
        /// </summary>
        private RelayCommand _handShelfCommand;

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand  HandShelfCommand { get; private set; }



        /// <summary>
        /// 完成提交
        /// </summary>
        private RelayCommand _submitCommand;
        public RelayCommand SubmitCommand { get; private set; }



        private RelayCommand _SendInfoCommand;

        public RelayCommand SendInfoCommand
        {
            get
            {
                if (_SendInfoCommand == null)
                {
                    _SendInfoCommand = new RelayCommand(() => SendInfo());
                }
                return _SendInfoCommand;
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        private RelayCommand _LoginOutCommand;

        public RelayCommand<string> LoginOutCommand { get; private set; }
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
        /// 客户端货柜编码
        /// </summary>
        private string ContainerCode = string.Empty;





        #endregion
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="code"></param>
        public async void LoginOut(string code)
        {
            // 重新读取登录信息
            if (await Msg.Question("是否退出登录？"))
            {
                // 清除登录信息
                GlobalData.loginTime = "";
                GlobalData.UserCode = "";
                GlobalData.UserName = "";
                GlobalData.PictureUrl = "";
                var obj = new MainViewModel();
                if (obj == null) return;
                obj.ExitPage(MenuBehaviorType.ExitAllPage, "");
            }
        }

        /// <summary>
        /// 核验登录
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckLogin()
        {
            // 系统用户注销时间
            var checkTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["LoginOutTime"].ToString());
            // 如果未登录
            if (string.IsNullOrWhiteSpace(GlobalData.loginTime))
            {
                return false;
            }
            else
            {
                var login = Convert.ToDateTime(GlobalData.loginTime).AddMinutes(checkTime);
                var now = DateTime.Now;

                // 如果时间已过期
                if (DateTime.Compare(now, login) > 0)
                {
                    // 系统登录
                    return false;
                }
                else
                {
                    // 核查用户是否有此模块操作权限
                    var user = ServiceProvider.Instance.Get<IUserService>();
                    var authCheck = user.GetCheckAuth(GlobalData.LoginModule);
                    if (!authCheck.Result.Success)
                    {
                        if (await Msg.Question("抱歉，您无该模块操作权限！"))
                        {
                            return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }


        /// <summary>
        /// 核验登录人员
        /// </summary>
        /// <param name="code"></param>
        public async void ShowLogin()
        {
            GlobalData.LoginModule = "Role";
            GlobalData.LoginPageCode = "FaceInfoDlg";
            GlobalData.LoginPageName = "权限下发";
            //如果登录
            if (await CheckLogin())
            {
                DeviceIP = _faceIP;
                DevicePort = _facePort;
                DeviceName = _faceUser;
                DevicePassword = _facePass;
                DeviceLogin();
                // 获取当前任务的明细
                //var inTaskMaterialList = InTaskContract.InTaskMaterialDtos.Where(a => a.InTaskCode == code).ToList();
                //InTaskMaterial.Clear();
                //inTaskMaterialList.ForEach((arg) => InTaskMaterial.Add(arg));
                //TabPageIndex = 1;
                //GlobalData.IsFocus = true;
            }
            else //如果未登录
            {
                var dialog = ServiceProvider.Instance.Get<IShowContent>();
                dialog.BindDataContext(new UserLoginWindow(), new UserLoginModel());
                dialog.Show();
            }
        }

        private Int32[] m_lAlarmHandle = new Int32[200];

        public void btn_SetAlarm_Click()
        {
            if (m_iUserID==-1)
            {
                Msg.Warning("请先登录设备！");
                return;
            }
            HCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam = new HCNetSDK.NET_DVR_SETUPALARM_PARAM();
            struAlarmParam.dwSize = (uint)Marshal.SizeOf(struAlarmParam);
            struAlarmParam.byLevel = 0; //0- 一级布防,1- 二级布防
            m_lAlarmHandle[m_lUserID] = HCNetSDK.NET_DVR_SetupAlarmChan_V41(m_lUserID, ref struAlarmParam);
            if (m_lAlarmHandle[m_lUserID] < 0)
            {
                uint iLastErr = HCNetSDK.NET_DVR_GetLastError();
                //label2.Text = "布防失败，错误号：" + iLastErr; ;
            }
            else
            {
                Msg.Warning("开启成功！");
                return;
                //label2.Text = "布防成功";
                //btn_SetAlarm.Enabled = false;
                //btnCloseAlarm.Enabled = true;
            }
        }

        /// <summary>
        /// 登录至门禁设备
        /// </summary>
        public async void DeviceLogin()
        {
            if (String.IsNullOrEmpty(DeviceIP))
            {
                DeviceIP = System.Configuration.ConfigurationManager.AppSettings["FaceIP"].ToString();
                //Msg.Warning("请输入人脸识别设备IP地址！");
                //return;
            }
            if (String.IsNullOrEmpty(DevicePort))
            {
                DevicePort = System.Configuration.ConfigurationManager.AppSettings["FacePort"].ToString();
                //Msg.Warning("请输入人脸识别设备端口号！");
                //return;
            }
            if (String.IsNullOrEmpty(DeviceName))
            {
                //Msg.Warning("请输入人脸识别设备登录用户名！");
                //return;
                DeviceName = System.Configuration.ConfigurationManager.AppSettings["FaceUser"].ToString();
            }
            if (String.IsNullOrEmpty(DevicePassword))
            {
                 DevicePassword = System.Configuration.ConfigurationManager.AppSettings["FacePassword"].ToString();
                //Msg.Warning("请输入人脸识别设备登录密码！");
                //return;
            }
            if (!Login(true))
            {
                Msg.Warning("登录失败！请检查配置信息！");
                return;
            }
            else
            {
                Msg.Warning("登录成功！");
                return;
            }
        }

        public int m_iDeviceIndex = -1;
        HCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo;
        public int m_iUserID = -1;
        private uint m_AysnLoginResult = 0;
        private bool LoginCallBackFlag = false;
        private bool AysnLoginFlag = false;
        public Int32 m_lSetCardCfgHandle = 0;
        public Int32 m_lSetFaceHandle = 0;
        public int m_lUserID = -1;
        private HCNetSDK.RemoteConfigCallback g_fSetFaceCallback = null;

        private HCNetSDK.RemoteConfigCallback g_fSetGatewayCardCallback = null;

        private HCNetSDK.NET_DVR_CARD_CFG_V50 m_struCardInfo = new HCNetSDK.NET_DVR_CARD_CFG_V50();

        public bool Login(bool bStatus)//true said add node login, false for the existing node to log in 
        {
            try
            {
                LoginCallBackFlag = false;
                m_struDeviceInfo = new HCNetSDK.NET_DVR_DEVICEINFO_V30();

                HCNetSDK.NET_DVR_DEVICEINFO_V30 struDeviceInfo = new HCNetSDK.NET_DVR_DEVICEINFO_V30();
                struDeviceInfo.sSerialNumber = new byte[HCNetSDK.SERIALNO_LEN];

                HCNetSDK.NET_DVR_NETCFG_V50 struNetCfg = new HCNetSDK.NET_DVR_NETCFG_V50();
                struNetCfg.Init();
                HCNetSDK.NET_DVR_DEVICECFG_V40 struDevCfg = new HCNetSDK.NET_DVR_DEVICECFG_V40();
                struDevCfg.sDVRName = new byte[HCNetSDK.NAME_LEN];
                struDevCfg.sSerialNumber = new byte[HCNetSDK.SERIALNO_LEN];
                struDevCfg.byDevTypeName = new byte[HCNetSDK.DEV_TYPE_NAME_LEN];
                HCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new HCNetSDK.NET_DVR_USER_LOGIN_INFO();
                HCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new HCNetSDK.NET_DVR_DEVICEINFO_V40();
                struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[HCNetSDK.SERIALNO_LEN];
                uint dwReturned = 0;
                int lUserID = -1;

                struLoginInfo.bUseAsynLogin = AysnLoginFlag;
                struLoginInfo.cbLoginResult = new HCNetSDK.LoginResultCallBack(AsynLoginMsgCallback);

                if (bStatus)
                {
                    struLoginInfo.sDeviceAddress = DeviceIP; // 设备IP地址
                    struLoginInfo.sUserName = DeviceName;
                    struLoginInfo.sPassword = DevicePassword;
                    ushort.TryParse(DevicePort, out struLoginInfo.wPort);
                }

                lUserID = HCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
                if (struLoginInfo.bUseAsynLogin)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        if (!LoginCallBackFlag)
                        {
                            Thread.Sleep(5);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!LoginCallBackFlag)
                    {
                    }
                    if (m_AysnLoginResult == 1)
                    {
                        lUserID = m_iUserID;
                        struDeviceInfoV40.struDeviceV30 = m_struDeviceInfo;
                    }
                    else
                    {
                        return false;
                    }

                }

                if (lUserID < 0)
                {
                    uint nErr = HCNetSDK.NET_DVR_GetLastError();
                    string strTemp = string.Format("NET_DVR_Login_V40 [{0}]", DeviceIP);
                    if (nErr == HCNetSDK.NET_DVR_PASSWORD_ERROR)
                    {
                        MessageBox.Show("user name or password error!");
                        if (1 == struDeviceInfoV40.bySupportLock)
                        {
                            string strTemp1 = string.Format("Left {0} try opportunity", struDeviceInfoV40.byRetryLoginTime);
                            MessageBox.Show(strTemp1);
                        }
                    }
                    else if (nErr == HCNetSDK.NET_DVR_USER_LOCKED)
                    {
                        if (1 == struDeviceInfoV40.bySupportLock)
                        {
                            string strTemp1 = string.Format("user is locked, the remaining lock time is {0}", struDeviceInfoV40.dwSurplusLockTime);
                            MessageBox.Show(strTemp1);
                        }
                    }
                    else
                    {
                       // MessageBox.Show("net error or dvr is busy!");
                    }
                    return false;
                }
                else
                {
                    if (1 == struDeviceInfoV40.byPasswordLevel)
                    {
                        MessageBox.Show("default password, please change the password");
                    }
                    else if (3 == struDeviceInfoV40.byPasswordLevel)
                    {
                        MessageBox.Show("risk password, please change the password");
                    }
                    struDeviceInfo = struDeviceInfoV40.struDeviceV30;
                }

                if (bStatus)
                {

                }

                if (1 == (struDeviceInfo.bySupport & 0x80))
                {
                }
                else
                {
                }

                uint dwSize = (uint)Marshal.SizeOf(struNetCfg);
                IntPtr ptrNetCfg = Marshal.AllocHGlobal((int)dwSize);
                Marshal.StructureToPtr(struNetCfg, ptrNetCfg, false);

                if (!HCNetSDK.NET_DVR_GetDVRConfig(lUserID, HCNetSDK.NET_DVR_GET_NETCFG_V50, 0, ptrNetCfg, dwSize, ref dwReturned))
                {

                }
                else
                {
                    //IPv6 temporary unrealized 
                    struNetCfg = (HCNetSDK.NET_DVR_NETCFG_V50)Marshal.PtrToStructure(ptrNetCfg, typeof(HCNetSDK.NET_DVR_NETCFG_V50));
                    string strTemp = string.Format("multi-cast ipv4 {0}", struNetCfg.struMulticastIpAddr.sIpV4);
                }
                Marshal.FreeHGlobal(ptrNetCfg);

                dwReturned = 0;
                uint dwSize2 = (uint)Marshal.SizeOf(struDevCfg);
                IntPtr ptrDevCfg = Marshal.AllocHGlobal((int)dwSize2);
                Marshal.StructureToPtr(struDevCfg, ptrDevCfg, false);

                if (!HCNetSDK.NET_DVR_GetDVRConfig(lUserID, HCNetSDK.NET_DVR_GET_DEVICECFG_V40, 0, ptrDevCfg, dwSize2, ref dwReturned))
                {
                }
                else
                {
                    struDevCfg = (HCNetSDK.NET_DVR_DEVICECFG_V40)Marshal.PtrToStructure(ptrDevCfg, typeof(HCNetSDK.NET_DVR_DEVICECFG_V40));
                }
                Marshal.FreeHGlobal(ptrDevCfg);

                //if (DoGetDeviceResoureCfg(m_iDeviceIndex))
                //{

                //}
                m_iUserID = lUserID;
                m_lUserID= lUserID;
                return true;
            }
            catch (Exception ex)
            {
                if (m_bInitSDK == true)
                {
                    HCNetSDK.NET_DVR_Cleanup();
                }
                return false;
            }
        }

        // Asynchronous callback function
        public void AsynLoginMsgCallback(Int32 lUserID, UInt32 dwResult, ref HCNetSDK.NET_DVR_DEVICEINFO_V30 lpDeviceInfo, IntPtr pUser)
        {
            if (dwResult == 1)
            {
                m_struDeviceInfo = lpDeviceInfo;
            }
            m_AysnLoginResult = dwResult;
            m_iUserID = lUserID;
            LoginCallBackFlag = true;
        }


        private string RFIDCode= string.Empty;
        private string FaceUrl = string.Empty;
        private bool RFIDSetting = true;

        IntPtr pUser = new IntPtr();//用户数据


        /// <summary>
        /// 下发人脸信息
        /// </summary>
        public async void SendInfo()
        {
            try
            {
                // 获取全部人员信息
                var userList = IdentityContract.Users.Where(a => a.Enabled).ToList();

                foreach (var item in userList)
                {
                    if (!String.IsNullOrEmpty(item.RFIDCode))
                    {
                        RFIDCode = item.RFIDCode;
                        //FaceUrl = _basePath + item.PictureUrl;
                        FaceUrl = item.Remark;// @"C:\Users\Administrator\Desktop\西安智能货柜\人脸.jpg";
                        string result = "";
                        result = DownCardMsg(m_lUserID, RFIDCode, "");
                        if (result != "")
                        {
                            Msg.Warning(result);
                        }
                        // 下发其他人脸信息
                        if (RFIDSetting)
                        {
                            if (string.IsNullOrEmpty(FaceUrl))
                            {
                                Msg.Warning("该用户"+item.Code+"未上传人脸照片");
                            }
                            else
                            {
                                result = DowmFaceMsg(m_lUserID, RFIDCode);
                                if (result != "")
                                {
                                    Msg.Warning(result);
                                }
                            }
                            UserList.Add(item);
                        }
                        else
                        {
                            RFIDSetting = true;
                            Msg.Warning(string.Format("员工{0}人脸信息下发失败，请核验。", item.Name));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        #region 下载卡号和人脸
        /// <summary>
        /// 启用长链接待发送卡片数据
        /// </summary>
        /// <param name="UserID">登录设备的ID</param>
        /// <param name="cardNo">卡号10位十进制，不够前面补0</param>
        /// <param name="password">卡密码</param>
        /// <returns></returns>
        public string DownCardMsg(int UserID, string cardNo, string password)
        {
            if (-1 != m_lSetCardCfgHandle)
            {
                if (HCNetSDK.NET_DVR_StopRemoteConfig(m_lSetCardCfgHandle))
                {
                    m_lSetCardCfgHandle = -1;
                }
            }
            HCNetSDK.NET_DVR_CARD_CFG_COND struCond = new HCNetSDK.NET_DVR_CARD_CFG_COND();
            struCond.dwSize = (uint)Marshal.SizeOf(struCond);
            struCond.dwCardNum = 1;

            int dwSize = Marshal.SizeOf(struCond);
            IntPtr ptrStruCond = Marshal.AllocHGlobal(dwSize);
            Marshal.StructureToPtr(struCond, ptrStruCond, false);

            g_fSetGatewayCardCallback = new HCNetSDK.RemoteConfigCallback(ProcessSetGatewayCardCallback);
            //IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(this.RFIDCode)).Handle;
            //IntPtr hwnd = new WindowInteropHelper(this.RFIDCode).Handle; 
            m_lSetCardCfgHandle = HCNetSDK.NET_DVR_StartRemoteConfig(UserID, HCNetSDK.NET_DVR_SET_CARD_CFG_V50, ptrStruCond, dwSize, g_fSetGatewayCardCallback, pUser);
            if (-1 == m_lSetCardCfgHandle)
            {
                Marshal.FreeHGlobal(ptrStruCond);
                return "";
            }
            Marshal.FreeHGlobal(ptrStruCond);
            m_struCardInfo = GetCurCardInfo(RFIDCode, "");
            UpdateMoudleCfg();
            if (!SendCardData(m_struCardInfo))
            {
                HCNetSDK.NET_DVR_StopRemoteConfig(m_lSetCardCfgHandle);
                m_lSetCardCfgHandle = -1;
            }
            return "";
        }

        /// <summary>
        /// 发送卡片数据
        /// </summary>
        /// <param name="struCardCfg"></param>
        /// <param name="dwDiffTime"></param>
        /// <returns></returns>
        private bool SendCardData(HCNetSDK.NET_DVR_CARD_CFG_V50 struCardCfg, uint dwDiffTime = 0)
        {
            if (-1 == m_lSetCardCfgHandle)
            {
                return false;
            }
            uint dwSize = (uint)Marshal.SizeOf(struCardCfg);
            struCardCfg.dwSize = dwSize;
            IntPtr ptrStruCard = Marshal.AllocHGlobal((int)dwSize);
            Marshal.StructureToPtr(struCardCfg, ptrStruCard, false);
            if (!HCNetSDK.NET_DVR_SendRemoteConfig(m_lSetCardCfgHandle, (int)HCNetSDK.LONG_CFG_SEND_DATA_TYPE_ENUM.ENUM_ACS_SEND_DATA, ptrStruCard, dwSize))
            {
                Marshal.FreeHGlobal(ptrStruCard);
                return false;
            }
            Marshal.FreeHGlobal(ptrStruCard);
            return true;
        }

        /// <summary>
        /// 启用长链接待发送人脸数据
        /// </summary>
        /// <param name="UserID">登录设备的ID</param>
        /// <param name="cardNo">卡号10位十进制，不够前面补0</param>
        /// <returns></returns>
        public string DowmFaceMsg(int UserID, string cardNo)
        {
            string imgphta = FaceUrl;
            if (-1 != m_lSetFaceHandle)
            {
                if (HCNetSDK.NET_DVR_StopRemoteConfig(m_lSetFaceHandle))
                {
                    m_lSetFaceHandle = -1;
                }
            }
            HCNetSDK.NET_DVR_FACE_PARAM_COND lpInBuffer = new HCNetSDK.NET_DVR_FACE_PARAM_COND();
            lpInBuffer.Init();
            byte[] sCardNo = System.Text.Encoding.Default.GetBytes(cardNo);
            sCardNo.CopyTo(lpInBuffer.byCardNo, 0);
            lpInBuffer.byEnableCardReader[0] = 1;
            lpInBuffer.dwFaceNum = 1;
            lpInBuffer.byFaceID = (byte)1;
            lpInBuffer.byFaceDataType = (byte)1;
            lpInBuffer.dwSize = (uint)Marshal.SizeOf(lpInBuffer);
            int dwSize = Marshal.SizeOf(lpInBuffer);
            IntPtr ptrStruCond = Marshal.AllocHGlobal(dwSize);
            Marshal.StructureToPtr(lpInBuffer, ptrStruCond, false);
            g_fSetFaceCallback = new HCNetSDK.RemoteConfigCallback(ProcessSetFaceCfgCallbackData);
            // 启动远程配置。
            m_lSetFaceHandle = HCNetSDK.NET_DVR_StartRemoteConfig(UserID, HCNetSDK.NET_DVR_SET_FACE_PARAM_CFG, ptrStruCond, dwSize, g_fSetFaceCallback, pUser);
            if (-1 == m_lSetFaceHandle)
            {
                uint error = HCNetSDK.NET_DVR_GetLastError();
                Marshal.FreeHGlobal(ptrStruCond);
                return "发送人脸启用远程配置错误：" + error.ToString();
            }
            Marshal.FreeHGlobal(ptrStruCond);
            if (!SendFaceData(cardNo, imgphta))
            {
                HCNetSDK.NET_DVR_StopRemoteConfig(m_lSetFaceHandle);
                m_lSetFaceHandle = -1;
            }
            return "";
        }

        /// <summary>
        /// 发送人脸数据
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="imgphta">人脸图片路径</param>
        /// <returns></returns>
        private bool SendFaceData(string cardNo, string imgphta)
        {
            HCNetSDK.NET_DVR_FACE_PARAM_CFG m_struFingerPrintOne = new HCNetSDK.NET_DVR_FACE_PARAM_CFG();
            //byte[] faceData = SaveImage(imgphta);
            m_struFingerPrintOne.Init();
            byte[] sCardNo = System.Text.Encoding.Default.GetBytes(cardNo);
            sCardNo.CopyTo(m_struFingerPrintOne.byCardNo, 0);

            //if (!File.Exists(imgphta))
            //{
            //    MessageBox.Show("The face picture does not exist!");
            //    return false;
            //}

            byte[] fs = Convert.FromBase64String(imgphta);
            //FileStream fs = new FileStream(imgphta, FileMode.OpenOrCreate);
            //if (0 == fs.Length)
            //{
            //    MessageBox.Show("The face picture is 0k,please input another picture!");
            //    return false;
            //}
            if (200 * 1024 < fs.Length)
            {
                MessageBox.Show("The face picture is larger than 200k,please input another picture!");
                return false;
            }
            m_struFingerPrintOne.dwFaceLen = (uint)fs.Length;
            int iLen = (int)m_struFingerPrintOne.dwFaceLen;
           // byte[] by = new byte[iLen];
            m_struFingerPrintOne.pFaceBuffer = Marshal.AllocHGlobal(iLen);
            //  fs.Read(by, 0, iLen);
            Marshal.Copy(fs, 0, m_struFingerPrintOne.pFaceBuffer, iLen);
            //  Marshal.Copy(by, 0, m_struFingerPrintOne.pFaceBuffer, iLen);
            //  fs.Close();
            /*
            //需要注意的是pFaceBuffer字段，c++中是char  *，这个不能转成String
            //先定义一个类，名字随便取，例如：这个类是存放图片的byte[]信息的
            HCNetSDK.NET_DVR_FACE_PARAM_CFG_FACEDATA facedata = new HCNetSDK.NET_DVR_FACE_PARAM_CFG_FACEDATA();
            facedata.faceData = faceData;
            //facedata.write();
            Console.Write(facedata);
            ptrfacedata = Marshal.AllocHGlobal(Marshal.SizeOf(facedata));
            m_struFingerPrintOne.pFaceBuffer = ptrfacedata;//这里的facedata必须是类
            m_struFingerPrintOne.dwFaceLen = (uint)faceData.Length;
            */
            m_struFingerPrintOne.byEnableCardReader = new byte[HCNetSDK.MAX_CARD_READER_NUM_512];
            m_struFingerPrintOne.byEnableCardReader[0] = 1;
            m_struFingerPrintOne.byFaceID = (byte)1;
            m_struFingerPrintOne.byFaceDataType = (byte)1;
            uint dwSize = (uint)Marshal.SizeOf(m_struFingerPrintOne);
            m_struFingerPrintOne.dwSize = dwSize;
            Console.Write(m_struFingerPrintOne);
            IntPtr ptrStruCard = Marshal.AllocHGlobal((int)dwSize);
            Marshal.StructureToPtr(m_struFingerPrintOne, ptrStruCard, false);
            if (!HCNetSDK.NET_DVR_SendRemoteConfig(m_lSetFaceHandle, (int)HCNetSDK.ENUM_ACS_INTELLIGENT_IDENTITY_DATA, ptrStruCard, dwSize))
            {
                uint error = HCNetSDK.NET_DVR_GetLastError();
                Marshal.FreeHGlobal(ptrStruCard);
                Marshal.FreeHGlobal(m_struFingerPrintOne.pFaceBuffer);
                MessageBox.Show("发送人脸数据错误：" + error.ToString());
                return false;
            }

            Marshal.FreeHGlobal(ptrStruCard);
            Marshal.FreeHGlobal(m_struFingerPrintOne.pFaceBuffer);
            return true;
        }

        #endregion

        #region 删除卡片和人脸

        /// <summary>
        /// 启用长链接待删除卡片信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="cardNo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private string DelCardMsg(int UserID, string cardNo, string password)
        {
            if (-1 != m_lSetCardCfgHandle)
            {
                if (HCNetSDK.NET_DVR_StopRemoteConfig(m_lSetCardCfgHandle))
                {
                    m_lSetCardCfgHandle = -1;
                }
            }
            HCNetSDK.NET_DVR_CARD_CFG_COND struCond = new HCNetSDK.NET_DVR_CARD_CFG_COND();
            struCond.dwSize = (uint)Marshal.SizeOf(struCond);
            struCond.dwCardNum = 1;
            ushort.TryParse("0", out struCond.wLocalControllerID);
            struCond.byCheckCardNo = 1;
            int dwSize = Marshal.SizeOf(struCond);
            IntPtr ptrStruCond = Marshal.AllocHGlobal(dwSize);
            Marshal.StructureToPtr(struCond, ptrStruCond, false);
            g_fSetGatewayCardCallback = new HCNetSDK.RemoteConfigCallback(ProcessSetGatewayCardCallback);
            m_lSetCardCfgHandle = HCNetSDK.NET_DVR_StartRemoteConfig(m_lUserID, HCNetSDK.NET_DVR_SET_CARD_CFG_V50, ptrStruCond, dwSize, g_fSetGatewayCardCallback, pUser);
            if (-1 == m_lSetCardCfgHandle)
            {
                uint error = HCNetSDK.NET_DVR_GetLastError();
                Marshal.FreeHGlobal(ptrStruCond);
                return "删除卡片启用远程配置错误：" + error.ToString();
            }
            Marshal.FreeHGlobal(ptrStruCond);

            #region 创建NET_DVR_CARD_CFG_V50
            HCNetSDK.NET_DVR_CARD_CFG_V50 m_struCardCfg = new HCNetSDK.NET_DVR_CARD_CFG_V50();
            m_struCardCfg.Init();
            m_struCardCfg.dwSize = (uint)Marshal.SizeOf(m_struCardCfg);
            byte[] byTempCardNo = new byte[HCNetSDK.ACS_CARD_NO_LEN];
            byTempCardNo = System.Text.Encoding.UTF8.GetBytes(RFIDCode);
            for (int i = 0; i < byTempCardNo.Length; i++)
            {
                m_struCardCfg.byCardNo[i] = byTempCardNo[i];
            }
            m_struCardCfg.dwModifyParamType = (uint)4095;
            m_struCardCfg.byCardValid = (byte)0;
            m_struCardCfg.byCardType = (byte)1;
            uint dwSize2 = (uint)Marshal.SizeOf(m_struCardCfg);
            m_struCardCfg.dwSize = dwSize2;
            #endregion

            if (!SendCardData(m_struCardCfg))
            {
                HCNetSDK.NET_DVR_StopRemoteConfig(m_lSetCardCfgHandle);
                m_lSetCardCfgHandle = -1;
            }
            return "ok";
        }

        /// <summary>
        /// 启用长链接待删除人脸信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        private string DelFaceMsg(int UserID, string cardNo)
        {
            HCNetSDK.NET_DVR_FACE_PARAM_CTRL struFaceDelCtrl = new HCNetSDK.NET_DVR_FACE_PARAM_CTRL();
            struFaceDelCtrl.dwSize = (uint)Marshal.SizeOf(struFaceDelCtrl);
            struFaceDelCtrl.byMode = 0;
            HCNetSDK.NET_DVR_FACE_PARAM_BYCARD struDelByCard = new HCNetSDK.NET_DVR_FACE_PARAM_BYCARD();
            struDelByCard.Init();

            String strCardNo = cardNo;
            byte[] byCardNo = System.Text.Encoding.Default.GetBytes(strCardNo);
            byCardNo.CopyTo(struDelByCard.byCardNo, 0);

            struDelByCard.byEnableCardReader[0] = 1;
            struDelByCard.byFaceID[0] = 1;

            int dwByCardSize = Marshal.SizeOf(struDelByCard);
            IntPtr ptrFaceByCard = Marshal.AllocHGlobal(dwByCardSize);
            Marshal.StructureToPtr(struDelByCard, ptrFaceByCard, false);

            //结构体数据拷贝到联合体里面
            struFaceDelCtrl.struProcessMode = (HCNetSDK.NET_DVR_DEL_FACE_PARAM_MODE)Marshal.PtrToStructure(ptrFaceByCard, typeof(HCNetSDK.NET_DVR_DEL_FACE_PARAM_MODE));
            Marshal.FreeHGlobal(ptrFaceByCard);
            int dwSize = Marshal.SizeOf(struFaceDelCtrl);
            IntPtr ptrFaceDelCtrl = Marshal.AllocHGlobal(dwSize);
            Marshal.StructureToPtr(struFaceDelCtrl, ptrFaceDelCtrl, false);

            bool bFaceDel = HCNetSDK.NET_DVR_RemoteControl(UserID, HCNetSDK.NET_DVR_DEL_FACE_PARAM_CFG, ptrFaceDelCtrl, (uint)dwSize);
            if (!bFaceDel)
            {
                uint error = HCNetSDK.NET_DVR_GetLastError();
                Marshal.FreeHGlobal(ptrFaceDelCtrl);
             //   return "删除人脸图片错误：" + hikError.GetError_Code((int)error);
                return "删除人脸图片错误：";
            }

            Marshal.FreeHGlobal(ptrFaceDelCtrl);
            return "";
        }


        #endregion


        #region 返回函数
        private void ProcessSetGatewayCardCallback(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData)
        {
            if (pUserData == null)
            {
                return;
            }

            if (dwType != (uint)HCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_STATUS)
            {
                return;
            }
            uint dwStatus = (uint)Marshal.ReadInt32(lpBuffer);

            if (dwStatus == (uint)HCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_PROCESSING)
            {
                string strTemp = null;
                //strTemp = string.Format("Send SUCC,CardNO:{0}", System.Text.Encoding.UTF8.GetString(m_struCardInfo.byCardNo).TrimEnd('\0'));
                //g_formList.AddLog(m_lDeviceIndex, AcsDemoPublic.OPERATION_SUCC_T, strTemp);
            }
            else if (dwStatus == (uint)HCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_FAILED)
            {
                RFIDSetting = false;
              //  MessageBox.Show("RFID 员工卡号下发失败！");
                return;
            }
            else if (dwStatus == (uint)HCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_SUCCESS)
            {

             // MessageBox.Show("RFID 员工卡号下发完成！");
             // MessageBox.Show("NET_DVR_SET_CARD_CFG_V50 Set finish");
            }
            else if (dwStatus == (uint)HCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_EXCEPTION)
            {
                RFIDSetting = false;
             //   MessageBox.Show("RFID 员工卡号下发异常！");
                return;
            }
        }

        private void ProcessSetFaceCfgCallbackData(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData)
        {
            if (pUserData == null)
            {
                return;
            }

            if (dwType == (uint)HCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_STATUS)
            {
                uint dwStatus = (uint)Marshal.ReadInt32(lpBuffer);

                if (dwStatus == (uint)HCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_PROCESSING)
                {
                 //   MessageBox.Show("SetFaceParam Processing");
                }
                else if (dwStatus == (uint)HCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_FAILED)
                {
                 //   MessageBox.Show("SetFaceParam Failed");
                    HCNetSDK.PostMessage(pUserData, 1001, 0, 0);
                }
                else if (dwStatus == (uint)HCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_SUCCESS)
                {
                 //   MessageBox.Show("SetFaceParam Success");
                    HCNetSDK.PostMessage(pUserData, 1001, 0, 0);
                }
                else if (dwStatus == (uint)HCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_EXCEPTION)
                {
                 //   MessageBox.Show("SetFaceParam Exception");
                    HCNetSDK.PostMessage(pUserData, 1001, 0, 0);
                }
                else
                {
                 //   MessageBox.Show("Unknown Status");
                    HCNetSDK.PostMessage(pUserData, 1001, 0, 0);
                }
            }
            else if (dwType == (uint)HCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_DATA)
            {
                var result = Marshal.PtrToStructure(lpBuffer, typeof(HCNetSDK.NET_DVR_FACE_PARAM_STATUS));
                var struFaceParamStatus = (HCNetSDK.NET_DVR_FACE_PARAM_STATUS)result;
                //       string strTemp = "SetFaceParam Return Failed,byCardReaderRecvStatus is：" + struFaceParamStatus.byCardReaderRecvStatus[0];
                //        strTemp = string.Format("SetFaceParam Return Failed,byCardReaderRecvStatus is %d", struFaceParamStatus.byCardReaderRecvStatus[0]);
                if (struFaceParamStatus.byCardReaderRecvStatus[0] != 1)
                {
                    string strTemp = "SetFaceParam Return Failed,byCardReaderRecvStatus is：" + struFaceParamStatus.byCardReaderRecvStatus[0];
                    //    MessageBox.Show(strTemp);
                  //  MessageBox.Show("人脸图片信息下发失败!"+strTemp);
                    return;
                }
                else
                {
                    string strTemp = "SetFaceParam Return success ,byCardReaderRecvStatus is：" + struFaceParamStatus.byCardReaderRecvStatus[0];
                 //   MessageBox.Show(strTemp);
                }

            }
            return;
        }
        #endregion

        #region 其他
        public HCNetSDK.NET_DVR_CARD_CFG_V50 GetCurCardInfo(string cardNo, string password)
        {
            HCNetSDK.NET_DVR_CARD_CFG_V50 struCardCfg = new HCNetSDK.NET_DVR_CARD_CFG_V50();
            struCardCfg.Init();

            //see the NET_DVR_CARD_CFG_V50 definition
            struCardCfg.dwModifyParamType = 0x00000087;
            struCardCfg.byCardValid = 1;
            struCardCfg.byCardType = (byte)1;
            //struCardCfg.byCardModelType = (byte)2;

            struCardCfg.struValid.byEnable = 1;
            struCardCfg.struValid.struBeginTime.wYear = (ushort)(DateTime.Now.Year - 1);
            struCardCfg.struValid.struBeginTime.byMonth = (byte)DateTime.Now.Month;
            struCardCfg.struValid.struBeginTime.byDay = (byte)DateTime.Now.Day;
            struCardCfg.struValid.struBeginTime.byHour = (byte)DateTime.Now.Hour;
            struCardCfg.struValid.struBeginTime.byMinute = (byte)DateTime.Now.Minute;
            struCardCfg.struValid.struBeginTime.bySecond = (byte)DateTime.Now.Second;

            struCardCfg.struValid.struEndTime.wYear = (ushort)(DateTime.Now.Year + 3);
            struCardCfg.struValid.struEndTime.byMonth = (byte)DateTime.Now.Month;
            struCardCfg.struValid.struEndTime.byDay = (byte)DateTime.Now.Day;
            struCardCfg.struValid.struEndTime.byHour = (byte)DateTime.Now.Hour;
            struCardCfg.struValid.struEndTime.byMinute = (byte)DateTime.Now.Minute;
            struCardCfg.struValid.struEndTime.bySecond = (byte)DateTime.Now.Second;

            byte[] sCardNo = System.Text.Encoding.Default.GetBytes(cardNo);
            sCardNo.CopyTo(struCardCfg.byCardNo, 0);

            byte[] sCardPassword = System.Text.Encoding.Default.GetBytes(password);
            sCardPassword.CopyTo(struCardCfg.byCardPassword, 0);

            return struCardCfg;
        }

        private void UpdateMoudleCfg()
        {
            int i;
            int j;
            m_struCardInfo.dwModifyParamType |= 0x8;
            System.Array.Clear(m_struCardInfo.byDoorRight, 0, m_struCardInfo.byDoorRight.Length);
            for (i = 0; i < 1; i++)
            {
                if (true)
                {
                    m_struCardInfo.byDoorRight[i] = 1;
                }
            }
            m_struCardInfo.dwModifyParamType |= 0x100;
            for (i = 0; i < 1; i++)
            {
                for (j = 0; j < HCNetSDK.MAX_CARD_RIGHT_PLAN_NUM; j++)
                {
                   ushort.TryParse("1", out m_struCardInfo.wCardRightPlan[i * HCNetSDK.MAX_CARD_RIGHT_PLAN_NUM + j]);
                }
            }
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public string SetDeviceTime(int UserID)
        {
            HCNetSDK.NET_DVR_TIME dvr_time = new HCNetSDK.NET_DVR_TIME();
            dvr_time.dwYear = DateTime.Now.Year;
            dvr_time.dwMonth = DateTime.Now.Month;
            dvr_time.dwDay = DateTime.Now.Day;
            dvr_time.dwHour = DateTime.Now.Hour;
            dvr_time.dwMinute = DateTime.Now.Minute;
            dvr_time.dwSecond = DateTime.Now.Second;
            bool flag = HCNetSDK.NET_DVR_SetDVRConfig(UserID, HCNetSDK.NET_DVR_SET_TIMECFG, 0, dvr_time, Marshal.SizeOf(dvr_time));
            if (!flag)
            {
                uint ecode = HCNetSDK.NET_DVR_GetLastError();
                return "设置时间失败！" + ecode;
            }
            return "";
        }
        #endregion

        #region 报警回调以及具体报警解析
        public void MsgCallback(int lCommand, ref HCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            switch (lCommand)
            {
                case HCNetSDK.COMM_ALARM_ACS://门禁主机报警信息
                    ProcessCommAlarm_ACS(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                default:
                    break;
            }
        }
        public void UpdateClientList(string strAlarmMsg)
        {
            //列表新增报警信息
           // listBox1.Items.Add(strAlarmMsg);
        }
        private void ProcessCommAlarm_ACS(ref HCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            try
            {
                string strAlarmMsg = "";
                HCNetSDK.NET_DVR_ACS_ALARM_INFO acsInfo = new HCNetSDK.NET_DVR_ACS_ALARM_INFO();
                uint dwSize = (uint)Marshal.SizeOf(acsInfo);
                acsInfo = (HCNetSDK.NET_DVR_ACS_ALARM_INFO)Marshal.PtrToStructure(pAlarmInfo, typeof(HCNetSDK.NET_DVR_ACS_ALARM_INFO));

                //报警设备IP地址
                strAlarmMsg += "ip:【" + pAlarmer.sDeviceIP + "】";
                //设备序列号
                if (pAlarmer.bySerialValid == 1)
                {
                    string SN = Encoding.ASCII.GetString(pAlarmer.sSerialNumber).Replace("\0", "");
                    string name = pAlarmer.sDeviceName;
                    SN = SN.Substring(SN.Length - 9);
                    strAlarmMsg += "name:【" + name + "】";
                    strAlarmMsg += "SN:【" + SN + "】";
                }
                //主类型
                strAlarmMsg += "主类型:【" + acsInfo.dwMajor + "】";
                //次类型
                strAlarmMsg += "次类型:【" + acsInfo.dwMinor + "】"; //0x4b 人脸认证通过
                //报警时间
                string datetime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", acsInfo.struTime.dwYear.ToString(), acsInfo.struTime.dwMonth.ToString(), acsInfo.struTime.dwDay.ToString(), acsInfo.struTime.dwHour.ToString(), acsInfo.struTime.dwMinute.ToString(), acsInfo.struTime.dwSecond.ToString());
                strAlarmMsg += "时间:【" + DateTime.Parse(datetime) + "】";

                var valtime = DateTime.Now.AddMinutes(-2);

                // 判断是否为今天的布防时间
                if (DateTime.Parse(datetime) > valtime)
                {
                    var rfid = Encoding.ASCII.GetString(acsInfo.struAcsEventInfo.byCardNo).Replace("\0", "");
                    // 是1分钟内发生的
                    if (!String.IsNullOrEmpty(rfid) && (acsInfo.dwMinor == 1 || acsInfo.dwMinor==75))//验证通过 1 卡  75 人脸
                    {
                        // 有人刷卡登录了
                        // MessageBox.Show("卡号登录了！" + rfid);
                       // 系统登录
               
                        //  Dispatcher.BeginInvoke(new Action(() => { GetMyPageData(); }));


                        App.Current.Dispatcher.Invoke(new Action(() => {

                            var user = ServiceProvider.Instance.Get<IUserService>();
                            var inputDto = new LoginInfo() { VerifyCode = rfid };
                            var LoginTask = user.LoginAsync(inputDto);
                            var timeouttask = Task.Delay(10000);
                            var completedTask = Task.WhenAny(LoginTask, timeouttask).Result;
                            if (completedTask == timeouttask)
                            {

                            }
                            else
                            {
                                var task = LoginTask.Result;
                                if (task.Success)
                                {
                                    if (task.Data == null)
                                    {
                                        return;
                                    }
                                    // IdentityTicket entity = (IdentityTicket)task.Data;

                                    IdentityTicket entity =
                                        JsonHelper.DeserializeObject<IdentityTicket>(task.Data.ToString());
                                    UserData req = (UserData)entity.UserData;

                                    #region 设置用户基础信息

                                    Loginer.LoginerUser.Token = entity.Token;
                                    GlobalData.loginTime = DateTime.Now.ToString();
                                    GlobalData.UserCode = req.Code;
                                    GlobalData.UserName = req.Name;
                                    GlobalData.PictureUrl = req.Header;
                                    // 刷新界面
                                    var obj = new MainViewModel();
                                    if (obj == null) return;
                                    obj.UpdatePage(GlobalData.LoginPageName, GlobalData.LoginPageCode);
                                    DialogHost.CloseDialogCommand.Execute(null, null);

                                    #endregion
                                }
                                else
                                {
                                    //  MessageBox.Show(task.Message);
                                }
                            }

                   

                        }));
                
                    }

                    //网络操作用户名
                    //acb.sNetUser = Encoding.ASCII.GetString(acsInfo.sNetUser) + "】";
                    //主机地址
                    strAlarmMsg += "IP:【" + acsInfo.struRemoteHostAddr.sIpV4 + "】";
                    //图片数据大小
                    //acb.dwPicDataLen = acsInfo.dwPicDataLen + "】";

                    //报警信息详细参数
                    //卡号
                    strAlarmMsg +=
                        "卡号:【" + Encoding.ASCII.GetString(acsInfo.struAcsEventInfo.byCardNo).Replace("\0", "") + "】";
                    /*//卡类型
                    acb.AcsEventInfo_obj.byCardType = acsInfo.struAcsEventInfo.byCardType;
                    //白名单单号
                    acb.AcsEventInfo_obj.byWhiteListNo = acsInfo.struAcsEventInfo.byWhiteListNo;
                    //报告上传通道
                    acb.AcsEventInfo_obj.byReportChannel = acsInfo.struAcsEventInfo.byReportChannel;
                    //读卡器类型
                    acb.AcsEventInfo_obj.byCardReaderKind = acsInfo.struAcsEventInfo.byCardReaderKind;
                    //读卡器编号
                    acb.AcsEventInfo_obj.dwCardReaderNo = acsInfo.struAcsEventInfo.dwCardReaderNo;
                    //门编号
                    acb.AcsEventInfo_obj.dwDoorNo = acsInfo.struAcsEventInfo.dwDoorNo;
                    //多重卡认证序号
                    acb.AcsEventInfo_obj.dwVerifyNo = acsInfo.struAcsEventInfo.dwVerifyNo;
                    //报警输入号
                    acb.AcsEventInfo_obj.dwAlarmInNo = acsInfo.struAcsEventInfo.dwAlarmInNo;
                    //报警输出号
                    acb.AcsEventInfo_obj.dwAlarmOutNo = acsInfo.struAcsEventInfo.dwAlarmOutNo;
                    //事件触发器编号
                    acb.AcsEventInfo_obj.dwCaseSensorNo = acsInfo.struAcsEventInfo.dwCaseSensorNo;
                    //RS485通道号
                    acb.AcsEventInfo_obj.dwRs485No = acsInfo.struAcsEventInfo.dwRs485No;
                    //群组编号
                    acb.AcsEventInfo_obj.dwMultiCardGroupNo = acsInfo.struAcsEventInfo.dwMultiCardGroupNo;
                    //人员通道号
                    acb.AcsEventInfo_obj.wAccessChannel = acsInfo.struAcsEventInfo.wAccessChannel;
                    //设备编号
                    acb.AcsEventInfo_obj.byDeviceNo = acsInfo.struAcsEventInfo.byDeviceNo;
                    //分控器编号
                    acb.AcsEventInfo_obj.byDistractControlNo = acsInfo.struAcsEventInfo.byDistractControlNo;
                    //工号
                    acb.AcsEventInfo_obj.dwEmployeeNo = acsInfo.struAcsEventInfo.dwEmployeeNo;
                    //就地控制器编号
                    acb.AcsEventInfo_obj.wLocalControllerID = acsInfo.struAcsEventInfo.wLocalControllerID;
                    //网口ID
                    acb.AcsEventInfo_obj.byInternetAccess = acsInfo.struAcsEventInfo.byInternetAccess;
                    //防区类型
                    acb.AcsEventInfo_obj.byType = acsInfo.struAcsEventInfo.byType;
                    //物理地址
                    acb.AcsEventInfo_obj.byMACAddr = acsInfo.struAcsEventInfo.byMACAddr;
                    //刷卡类型
                    acb.AcsEventInfo_obj.bySwipeCardType = acsInfo.struAcsEventInfo.bySwipeCardType;
                    //保留
                    acb.AcsEventInfo_obj.byRes2 = acsInfo.struAcsEventInfo.byRes2;
                    //事件流水号
                    acb.AcsEventInfo_obj.dwSerialNo = acsInfo.struAcsEventInfo.dwSerialNo;*/
                    //string result = JsonConvert.SerializeObject(acb);
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + @"\Alarm", "报警", result);
                    UpdateClientList(strAlarmMsg);
                }
            }
            catch (Exception ex)
            {
                //todo 异常记录
            }
        }
        #endregion

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
