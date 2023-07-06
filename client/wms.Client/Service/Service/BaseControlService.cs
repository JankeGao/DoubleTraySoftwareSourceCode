using System;
using System.Threading;
using System.Threading.Tasks;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Utility.Data;
using wms.Client.Core.Interfaces;
using wms.Client.Model.Entity;
using wms.Client.Model.RequestModel;
using static wms.Client.Model.RequestModel.GetBackDoubleTrayMethodRequest;
using RunningContainer = wms.Client.Model.Entity.RunningContainer;

namespace wms.Client.Service.Service
{
    public class BaseControlService : IBaseControlService
    {
        /// <summary>
        /// 设备通讯状态
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> GetPlcDeivceStatus(RunningContainer model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetPlcDeivceStatusRequest() ,model,RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 设备报警信息
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> GetAlarmInformation()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetAlarmInformationRequest(), RestSharp.Method.GET);
            return r;
        }

        /// <summary>
        /// 料斗行程设定
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> GetHopperSetting()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new HopperSettingRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 安全门行程设定
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> GetEmergencyDoorSetting()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new EmergencyDoorSettingRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 复位报警信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResult> PostRestAllAlarm()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new ResetAlarmRequest(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 点击扫描按钮
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResult> PostStartScanBarcodeKeyDown(RunningContainer model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartScanBarcodeKeyDownRequest(), model, RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 返回当前称重物料数量
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> GetBackWeighingQuantity()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetBackWeighingQuantityRequest(), RestSharp.Method.GET);
            return r;
        }

        /// <summary>
        /// 返回M684的状态
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> GetBackDoubleTrayMethod()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetBackDoubleTrayMethodRequest(), RestSharp.Method.GET);
            return r;
        }

        /// <summary>
        /// 再次控制货柜运转
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResult> PostStartRunningContainerAgain(RunningContainer model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            string CurrentRunningTray = "";
            bool IsTakeIn = false;
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + wms.Client.LogicCore.Configuration.SerivceFiguration.INI_CFG;
            if (System.IO.File.Exists(cfgINI))
            {
                wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                CurrentRunningTray = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                IsTakeIn = Convert.ToBoolean(ini.IniReadValue("ClientInfo", "IsTakeIn").ToString());
            }
            if (!string.IsNullOrEmpty(CurrentRunningTray))
            {
                if (int.TryParse(CurrentRunningTray, out int trayNumber) == true)
                {
                    if (trayNumber != model.TrayCode)
                    {
                        model.LastTrayCode = CurrentRunningTray;
                    }
                }
            }

            var r = await baseService.GetRequest<DataResult>(new StartRunningContainerAgainRequest(), model, RestSharp.Method.POST);
            if (r.Success)
            {
                if (System.IO.File.Exists(cfgINI))
                {
                    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                    ini.IniWriteValue("ClientInfo", "CurrentRunningTray", model.TrayCode.ToString());
                    ini.IniWriteValue("ClientInfo", "IsTakeIn", "False");
                }
            }
            return r;
        }

        /// <summary>
        /// 双托盘再次控制货柜运转
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResult> DoubleTrayPostStartRunningContainerAgain(RunningContainer model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            string CurrentRunningTray = "";
            bool IsTakeIn = false;
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + wms.Client.LogicCore.Configuration.SerivceFiguration.INI_CFG;
            if (System.IO.File.Exists(cfgINI))
            {
                wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                CurrentRunningTray = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                IsTakeIn = Convert.ToBoolean(ini.IniReadValue("ClientInfo", "IsTakeIn").ToString());
            }
            if (!string.IsNullOrEmpty(CurrentRunningTray))
            {
                if (int.TryParse(CurrentRunningTray, out int trayNumber) == true)
                {
                    if (trayNumber != model.TrayCode)
                    {
                        model.LastTrayCode = CurrentRunningTray;
                    }
                }
            }

            var r = await baseService.GetRequest<DataResult>(new DoubleTrayStartRunningContainerAgainRequest(), model, RestSharp.Method.POST);
            if (r.Success)
            {
                if (System.IO.File.Exists(cfgINI))
                {
                    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                    ini.IniWriteValue("ClientInfo", "CurrentRunningTray", model.TrayCode.ToString());
                    ini.IniWriteValue("ClientInfo", "IsTakeIn", "False");
                }
            }
            return r;
        }

        /// <summary>
        /// 双托盘控制货柜运转
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResult> DoubleTrayPostStartRunningContainer(RunningContainer model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            string CurrentRunningTray = "";
            bool IsTakeIn = false;
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + wms.Client.LogicCore.Configuration.SerivceFiguration.INI_CFG;
            if (System.IO.File.Exists(cfgINI))
            {
                wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                CurrentRunningTray = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                IsTakeIn = Convert.ToBoolean(ini.IniReadValue("ClientInfo", "IsTakeIn").ToString());
            }
            if (!string.IsNullOrEmpty(CurrentRunningTray))
            {
                if (int.TryParse(CurrentRunningTray, out int trayNumber) == true)
                {
                    if (trayNumber != model.TrayCode)
                    {
                        model.LastTrayCode = CurrentRunningTray;
                    }
                }
            }

            var r = await baseService.GetRequest<DataResult>(new DoubleTrayStartRunningContainerRequest(), model, RestSharp.Method.POST);
            if (r.Success)
            {
                if (System.IO.File.Exists(cfgINI))
                {
                    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                    ini.IniWriteValue("ClientInfo", "CurrentRunningTray", model.TrayCode.ToString());
                    ini.IniWriteValue("ClientInfo", "IsTakeIn", "False");
                }
            }
            return r;
        }

        /// <summary>
        /// 控制货柜运转
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResult> PostStartRunningContainer(RunningContainer model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            string CurrentRunningTray = "";
            bool IsTakeIn = false;
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + wms.Client.LogicCore.Configuration.SerivceFiguration.INI_CFG;
            if (System.IO.File.Exists(cfgINI))
            {
                wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                CurrentRunningTray = ini.IniReadValue("ClientInfo", "CurrentRunningTray");
                IsTakeIn = Convert.ToBoolean(ini.IniReadValue("ClientInfo", "IsTakeIn").ToString());
            }
            if (!string.IsNullOrEmpty(CurrentRunningTray))
            {
                if (int.TryParse(CurrentRunningTray, out int trayNumber) == true)
                {
                    if (trayNumber != model.TrayCode)
                    {
                        model.LastTrayCode = CurrentRunningTray;
                    }
                }
            }

            var r = await baseService.GetRequest<DataResult>(new StartRunningContainerRequest(), model, RestSharp.Method.POST);
            if (r.Success)
            {
                if (System.IO.File.Exists(cfgINI))
                {
                    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                    ini.IniWriteValue("ClientInfo", "CurrentRunningTray",model.TrayCode.ToString());
                    ini.IniWriteValue("ClientInfo", "IsTakeIn", "False");
                }
            }
            return r;
        }
        public async Task<DataResult> PostStartRestoreContainer(RunningContainer model)
        {
            bool IsTakeIn = false;
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + wms.Client.LogicCore.Configuration.SerivceFiguration.INI_CFG;
            //if (System.IO.File.Exists(cfgINI))
            //{
            //    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
            //    IsTakeIn = Convert.ToBoolean(ini.IniReadValue("ClientInfo", "IsTakeIn").ToString());
            //}
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();

            var r = await baseService.GetRequest<DataResult>(new StartRestoreContainerRequest(), model, RestSharp.Method.POST);
            if (r.Success)
            {
                if (System.IO.File.Exists(cfgINI))
                {
                    wms.Client.LogicCore.Helpers.Files.IniFile ini = new wms.Client.LogicCore.Helpers.Files.IniFile(cfgINI);
                    ini.IniWriteValue("ClientInfo", "IsTakeIn", "True");
                }
            }
            return r;
        }

        public async Task<DataResult> OffXLight(RunningContainer model)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new OffXLightRequest(), model, RestSharp.Method.POST);
            return r;
        }

        #region 朗杰升降柜
        #region 垂直学习
        /// <summary>
        /// 开启垂直学习
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM300()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM300Request(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 监视学习状态M340  true时学习结束
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetM340()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM340Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 结束垂直学习
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> FinishM341()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new FinishM341Request(), RestSharp.Method.POST);
            return r;
        }
        #endregion

        #region 水平学习
        /// <summary>
        /// 开启水平学习
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM400()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM400Request(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 监视学习状态M440  true时学习结束
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetM440()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM440Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 结束水平学习
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> FinishM441()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new FinishM441Request(), RestSharp.Method.POST);
            return r;
        }
        #endregion

        #region 自动门学习

        /// <summary>
        /// 开启自动门学习
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM450()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM450Request(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 监视自动门学习状态  true时学习结束
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetM490()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM490Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 结束自动水平学习
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> FinishM491()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new FinishM491Request(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        ///M8设置为true 自动门学习
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> SetM8True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM8TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// M9设置为true 手动开门
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> SetM9True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM9TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// M9设置为true 手动关门
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> SetM10True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM10TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        #endregion

        #region 托盘扫描
        /// <summary>
        /// 开启托盘扫描
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> StartM350()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM350Request(), RestSharp.Method.POST);
            return r;
        }


        /// <summary>
        /// 设置M390为ON 表示进入托盘自定义状态
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> SetM390True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM390TrueRequest(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 开启托盘扫描  开始定义
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM391()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM391Request(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 获取托盘扫描状态
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetM390()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM390Request(), RestSharp.Method.GET);
            return r;
        }

        /// <summary>
        /// 在M390为ON时 将M370置为true 表示取出未定义托盘
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> SetM370True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM370TrueRequest(), RestSharp.Method.POST);
            return r;
        }


        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> WriteD392(RunningContainer runningContainer)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new WriteD392Request(), runningContainer, RestSharp.Method.POST);
            return r;
        }


        /// <summary>
        /// 在M390为ON时 将M371置为true 表示存入自定义托盘
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> SetM371True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM371TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 确认完毕后 下一个
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> ConfirmM394()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new ConfirmM394Request(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 获取下一个 状态 OFF表示还设有未定义托盘 ON全部定义完成
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetM395()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM395Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 结束托盘扫描
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> ConfirmM396()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new ConfirmM396Request(), RestSharp.Method.POST);
            return r;
        }


        #endregion

        #region 自动存取托盘
        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> WriteD650(RunningContainer runningContainer)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new WriteD650Request(), runningContainer,RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 获取M650状态
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> StartM650()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM650Request(), RestSharp.Method.POST  );
            return r;
        }
        /// <summary>
        /// 获取M650状态
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> GetM650()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM650Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 获取D651 托盘所在托架号
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetD651()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetD651Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 启动M651
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM651()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM651Request(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 获取物料高度
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetD652()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetD652Request(), RestSharp.Method.GET);
            return r;
        }

        /// <summary>
        /// 获取M650状态
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> WriteD650_In(RunningContainer runningContainer)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new WriteD650_InRequest(), runningContainer, RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 获取M651状态
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> SetM651True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM651TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 获取M654状态
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> SetM654True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM654TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        public async Task<DataResult> GetM654()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM654TrueRequest(), RestSharp.Method.GET);
            return r;
        }
        #endregion

        #region 添加托盘
        /// <summary>
        /// 添加托盘
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> WriteD700(RunningContainer runningContainer)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new WriteD700Request(), runningContainer, RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 启动M700
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM700()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM700Request(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        ///  获取M701状态 true 空间足够 
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetM701()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM701Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 确认存入
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> ConfirmM702()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new ConfirmM702Request(), RestSharp.Method.POST);
            return r;
        }
        #endregion

        #region 删除托盘
        /// <summary>
        /// 写入需要删除的托盘
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> WriteD750(RunningContainer runningContainer)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new WriteD750Request(), runningContainer ,RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 启动M750
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM750()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM750Request(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 获取托盘所在货架号
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetD751()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetD751Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 启动M751
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM751()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM751Request(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 监视托盘
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetM752()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM752Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 确认删除
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> ConfirmM753()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new ConfirmM753Request(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 取消删除
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> ConfirmM754()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new ConfirmM754Request(), RestSharp.Method.POST);
            return r;
        }
        #endregion

        #region 整理存储空间
        /// <summary>
        /// 开始后侧空间
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM800()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM800Request(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 开始后侧空间
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> StartM810()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM810Request(), RestSharp.Method.POST);
            return r;
        }
        /// <summary>
        /// 监视整理是否完成
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult> GetM801()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetM801Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 获取空间利用率
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> GetD800()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new GetD800Request(), RestSharp.Method.GET);
            return r;
        }
        /// <summary>
        /// 确认整理完毕
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> ConfirmM802()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new ConfirmM802Request(), RestSharp.Method.POST);
            return r;
        }
        #endregion

        #region 手动垂直运行
        /// <summary>
        /// 写入托架号
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> WriteD500(RunningContainer runningContainer)
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new WriteD500Request(), runningContainer,RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 检索托架位置
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM500()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM500Request(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 驱动升降机运行
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> StartM501()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new StartM501Request(), RestSharp.Method.POST);
            return r;
        }
        #endregion

        #region 手动垂直运行

        /// <summary>
        ///  在M410为ON时 将M410置为true 表示钩子向前运行到上部
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> SetM410True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM410TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 在M420为ON时 将M420置为true 表示钩子向前运行到下部
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> SetM420True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM420TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        ///  在M460为ON时 将M460置为true 表示钩子向后运行到下部
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> SetM460True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM460TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        ///  在M430为ON时 将M430置为true 表示钩子向后运行到上部
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> SetM430True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM430TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 在M450为ON时 将M450置为true 表示钩子向后运行到下部
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> SetM450True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM450TrueRequest(), RestSharp.Method.POST);
            return r;
        }

        /// <summary>
        /// 在M470为ON时 将M470置为true 表示钩子向前运行到下部
        /// </summary>
        /// <returns></returns>
         public async Task<DataResult> SetM470True()
        {
            BaseServiceRequest<DataResult> baseService = new BaseServiceRequest<DataResult>();
            var r = await baseService.GetRequest<DataResult>(new SetM470TrueRequest(), RestSharp.Method.POST);
            return r;
        }
        #endregion

        #endregion


    }
}
