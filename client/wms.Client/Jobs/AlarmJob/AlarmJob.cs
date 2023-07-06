using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Bussiness.Contracts;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Dependency;
using Quartz;
using SqlSugar;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.Service;
using wms.Client.ViewModel.other.Common;
using wms.Client.Template;
using wms.Client.ViewModel;

namespace wms.Client.Jobs.AlarmJob
{
    /**+++++++++++++++++++++++++++
     * 说明：设备状态运行监测
     *+++++++++++++++++++++++++++*/
    internal class AlarmJob : BaseJob
    {
        // 调用本地数据库
        public static string connStr = System.Configuration.ConfigurationSettings.AppSettings["connectionString"].ToString();
        //public static string connStr = ConfigurationManager.ConnectionStrings["Default"].ToString();

        private bool flag = false;

        SqlSugarClient db = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = connStr,
                DbType = DbType.SqlServer,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });

        public AlarmJob()
        {
            m_jobName = this.GetType().Name;
            ReadConfigInfo();
        }

        public override async Task Do(params object[] param)
        {
            Thread.Sleep(100);
            TaskBegin();
        }

        public override void Init(JobConfig config)
        {
            // define the job and tie it to our HelloJob class
            m_job = JobBuilder.Create<AlarmJob>()
                .WithIdentity(m_jobName)
                .WithDescription(config.Desc)
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            m_trigger = TriggerBuilder.Create()
                .WithIdentity(m_jobName)
                .WithCronSchedule(config.CronExpression)
                //.StartNow()
                .Build();

            // 获取货柜实体
            // 查询货柜下可存放的物料
       
           // ContainerEntity = WareHouseContract.Containers.FirstOrDefault(a => a.Code == ContainerCode);
        }


        private async void TaskBegin()
        {
            try
            {
                var ContainerEntity = new Container();
                // 读取货柜状态
                ContainerEntity = db.Queryable<Container>().Where(it => it.Code == ContainerCode).First();
                GlobalData.IsOnLine = true;

                var runingEntity = new wms.Client.Model.Entity. RunningContainer()
                {
                    ContainerCode = "",
                    TrayCode = 1,
                    XLight = 0
                };
                if (ContainerEntity != null)
                {
                    runingEntity.ContainerType = ContainerEntity.ContainerType;
                    runingEntity.IpAddress = ContainerEntity.Ip;
                    runingEntity.Port = int.Parse(ContainerEntity.Port);
                }
                // 读取PLC 状态信息
                var alarmService = ServiceProvider.Instance.Get<IBaseControlService>();

                // 物料实体映射
                var inTask = alarmService.GetPlcDeivceStatus(runingEntity);

                //设备在线
                if (inTask.Result.Success)
                {
                    ContainerEntity.Status = (int)DeviceStatusEnum.Running;
                }
                else
                {
                    // ContainerEntity.Status = (int)DeviceStatusEnum.Running;
                    ContainerEntity.Status = (int)DeviceStatusEnum.Fault;
                }

                // 设备状态
                GlobalData.DeviceStatus = ContainerEntity.Status;

                ContainerEntity.AlarmStatus = (int)DeviceAlarmStateEnum.Reach;
                var alarm = alarmService.GetAlarmInformation();
                var currentAlarm = new DeviceAlarm();
                // 设备发生报警
                if (alarm.Result.Success)
                {
                    flag = true;
                    string[] sArray = ((String)alarm.Result.Data).Split(';');
                    foreach (var item in sArray)
                    {
                        // 当前货柜没有此类别的报警正在发生
                        if (db.Queryable<DeviceAlarm>().Where(it => it.ContainerCode == ContainerCode && it.Status == (int)DeviceAlarmStateEnum.Urgencye && it.AlarmStatus == Convert.ToInt32(item)).ToList().Count <= 0)
                        {
                            var alarmEntity = new DeviceAlarm()
                            {
                                Code = Guid.NewGuid().ToString(),
                                ContainerCode = ContainerEntity.Code,
                                WarehouseCode = ContainerEntity.WareHouseCode,
                                Status = (int)DeviceAlarmStateEnum.Urgencye,
                                AlarmStatus = Convert.ToInt32(item),
                                CreatedTime = DateTime.Now
                            };
                            currentAlarm = alarmEntity;
                            if (db.Insertable(alarmEntity).IgnoreColumns(it => new { it.Id }).ExecuteCommand() < 0)
                            {
                                Print("新增报警信息失败");
                            }
                        }
                        else
                        {
                            currentAlarm = db.Queryable<DeviceAlarm>().Where(it =>
                                it.ContainerCode == ContainerCode && it.Status == (int) DeviceAlarmStateEnum.Urgencye &&
                                it.AlarmStatus == Convert.ToInt32(item)).First();
                        }
                    }

                    ContainerEntity.AlarmStatus = (int)DeviceAlarmStateEnum.Urgencye;
                }
                // 更新设备通讯状态
                if (db.Updateable(ContainerEntity).UpdateColumns(it => new { it.Status, it.AlarmStatus }).Where(it => it.Code == ContainerCode).ExecuteCommand() < 0)
                {
                    Print("更新货柜状态信息失败");
                }

                if (flag)
                {
                    if (!GlobalData.Comfirm)
                    {
                        GlobalData.Comfirm = true;
                        var message = HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.DeviceAlarmEnum), currentAlarm.AlarmStatus.Value);
                        //if (await Msg.Question("是否需要开始货柜?") == true)
                        //{
                        //    RunningContainer();
                        //}
                        //MessageBox.Show("设备发生报警:" + message + ",请复位！", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes
                        //  await Msg.Question("设备发生报警:" + message + ",请复位！") == true

                        MessageResult dr = ShowMessage("设备发生报警:" + message + ",请复位！");
                        if (dr.IsYes)
                        {
                            try
                            {

                                var deviceEnity = new DeviceAlarm()
                                {
                                    ContainerCode = ContainerCode
                                };
                                // 获取服务端复位方法
                                var alarmReService = ServiceProvider.Instance.Get<IAlarmService>();
                                var serverRest = alarmReService.PostRestAllAlarmServer(deviceEnity);

                                if (serverRest.Result.Success)
                                {
                                    // 获取当前设备下的所有报警信息
                                    // 物料实体映射-复位PLC
                                    var restTask = alarmService.PostRestAllAlarm();

                                    // 复位成功
                                    if (restTask.Result.Success)
                                    {
                                        flag = false;
                                        GlobalData.Comfirm = false;
                                        ContainerEntity.AlarmStatus = (int)DeviceAlarmStateEnum.Reach;
                                        // 更新设备通讯状态
                                        if (db.Updateable(ContainerEntity).UpdateColumns(it => new { it.Status, it.AlarmStatus }).Where(it => it.Code == ContainerCode).ExecuteCommand() < 0)
                                        {
                                            Msg.Warning("报警复位失败!");
                                           // MessageBox.Show("报警复位失败！");
                                        }
                                    }
                                    else
                                    {
                                        Msg.Warning("报警复位失败!");
                                        // MessageBox.Show("报警复位失败！");
                                    }
                                    return;
                                }
                                else
                                {
                                    Msg.Warning("报警复位失败!");
                                    // MessageBox.Show("报警复位失败！");
                                    return;
                                }


                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                        }
                        else
                        {
                            flag = false;
                            GlobalData.Comfirm = false;
                        }
                    }
                    else
                    {
                      //  flag = false;
                       // GlobalData.Comfirm = false;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalData.IsOnLine = false;
                Msg.Info("查询货柜实体数据报错" + ex.Message);
            }

        }
        public override async void End()
        {
            base.End();
            Print("Complated");
        }


        public delegate MessageResult MessageBoxShow(string msg);

        public MessageResult ShowMessage(string msg)
        {
           // (MessageBoxResult)App.Current.Dispatcher.BeginInvoke(new MessageBoxShow(MessageBoxShow_F), new object[] { msg }).;
           return (MessageResult) App.Current.Dispatcher .Invoke(new MessageBoxShow(MessageBoxShow_F), new object[] { msg });

            // return (MessageBoxResult)
            //App.Current.MainWindow.Dispatcher.Invoke(new Action(delegate
            //{
            //    MessageBox.Show(msg, "提示信息", MessageBoxButton.YesNo);
            //}));


            // MainWindow.
            //   Dispatcher.Invoke(new MessageBoxShow(MessageBoxShow_F), new object[] { msg });
        }
        public MessageResult MessageBoxShow_F(string msg)
        {
            //MsgView msgView = new MsgView(msg);
            //return msgView;
            //return MessageBox.Show(msg, "报警信息，请点确认！", MessageBoxButton.OK);
            return AlarmConfirmationBox.ShowDialog(msg);
        }

        /// <summary>
        /// 客户端货柜编码
        /// </summary>
        private string ContainerCode = string.Empty;

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
