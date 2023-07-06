
using HP.Utility.Data;
using HslCommunication.Profinet.Melsec;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using PLCServer;
using PLCServer.Interceptor;
using ThreadState = System.Threading.ThreadState;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PLCServer
{
    // 日志拦截器
    [ThrottleServiceBehavior]
    public partial class PlcServer : IPlcServer
    {

        public static bool InWorking = false;

        public static bool OutWorking = false;

        public static bool IsWorking = false;

        public static bool IsRetry = false;

        //  System.Threading.WaitCallback waitCallback = new WaitCallback(ExecuteChangeQuantity);

        public static MelsecA1ENet melsec_net = null;

        public static bool isConnected = false;

        Thread retry_th = new Thread(WorkCallingBack);

        //public int falseTime=0;


        // private CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 停止服务
        /// </summary>
        public DataResult Stop()
        {
            #region 原来代码
            try
            {
                IsWorking = false;
                isConnected = false;
                if (melsec_net!=null)
                {
                    melsec_net.ConnectClose();
                }
                return DataProcess.Success();
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
            #endregion
        }


        #region PLC服务

        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <param name="PLCAddress"></param>
        /// <param name="PLCPort"></param>
        /// <returns></returns>
        public DataResult StartPLC(string PLCAddress,int PLCPort)
        {
            melsec_net = new MelsecA1ENet();
            //string PLCAddress = ConfigurationManager.AppSettings["PLCAddress"].ToString();
            //int PLCPort =Convert.ToInt32(ConfigurationManager.AppSettings["PLCPort"].ToString());
            var connResult = ConnectPLC(PLCAddress, PLCPort, melsec_net);
            if (connResult.Success)
            {
                isConnected = true;
            }

            // 防止多次启动线程
            if (!IsRetry)
            {
                // 启动线程
                retry_th.Start();
                IsRetry = true;
            }
    
            //else
            //{
            //    // 启动线程
            //    retry_th.Start();
            //    IsRetry = true;
            //}
            // ThreadPool.QueueUserWorkItem(new WaitCallback(WorkCallingBack), this);
            return connResult;
        }

        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <param name="PLCAddress"></param>
        /// <param name="PLCPort"></param>
        /// <returns></returns>
        public void StopPLC(string PLCAddress, int PLCPort)
        {
            retry_th.Suspend();//挂起线程
        }

        /// <summary>
        /// 点击扫描按钮
        /// </summary>
        /// <param name = "runningContainer" >货柜信息</ param >
        /// < returns ></ returns >
        public DataResult StartScanBarcodeKeyDown(Command.RunningContainer runningContainer)
        {
            if (isConnected || runningContainer.ContainerType == 1 || runningContainer.ContainerType == 2)
            {
                int SignalDelay = 200;
                if (runningContainer.ContainerType == 3)
                {
                    try
                    {
                        var M65111Result = melsec_net.ReadBool("M651");
                        if (M65111Result.IsSuccess)
                        {
                            if (M65111Result.Content == true)
                            {
                                return DataProcess.Failure("取出动作尚未结束");
                            }
                        }
                        var M654Result = melsec_net.ReadBool("M654");
                        if (M654Result.IsSuccess)
                        {
                            if (M654Result.Content == true)
                            {
                                return DataProcess.Failure("存入动作尚未结束");
                            }
                        }
                        var M900Result = melsec_net.ReadBool("M900");
                        if (M900Result.IsSuccess)
                        {
                            if (M900Result.Content == true)
                            {
                                return DataProcess.Failure("更换托盘动作尚未结束");
                            }
                        }
                        var UnitWeightResult = melsec_net.Write("D192", (float)runningContainer.UnitWeight);
                        if (!UnitWeightResult.IsSuccess)
                        {
                            return DataProcess.Failure("物料单重传值未成功");
                        }

                        Thread.Sleep(SignalDelay);

                    }
                    catch (Exception EX)
                    {
                        return DataProcess.Failure(EX.Message);
                    }
                }
                else if (runningContainer.ContainerType == 1)
                {
                    return StartRunningC3000Container(runningContainer);
                }
                else if (runningContainer.ContainerType == 2)
                {
                    return StartRunningHanelContainer(runningContainer);
                }
            }
            else
            {
                return DataProcess.Failure("PLC未连接");
            }

            return DataProcess.Success();
        }

        /// <summary>
        /// 返回当前称重物料数量
        /// </summary>
        /// <returns></returns>
        public DataResult GetBackWeighingQuantity()
        {
            if (isConnected)
            {
                var ReturnQuantityResult = melsec_net.ReadUInt16("D196");
                if (!ReturnQuantityResult.IsSuccess)
                {
                    return DataProcess.Failure("获取物料称重数量未成功");
                }
                return DataProcess.Success(ReturnQuantityResult.Content);
            }
            // 如果未连接，则返回失败状态的DataResult对象
            return DataProcess.Failure("PLC未连接");

        }


        public DataResult GetBackDoubleTrayMethod()
        {
            if(isConnected)
            {
                var ReturnM684Result = melsec_net.ReadBool("M684");
                if(!ReturnM684Result.IsSuccess)
                {
                    return DataProcess.Failure("获取M684状态未成功");
                }
                return DataProcess.Success(ReturnM684Result.Content);
            }
            // 如果未连接，则返回失败状态的DataResult对象
            return DataProcess.Failure("PLC未连接");
        }

        /// <summary>
        ///启动货柜逻辑-自动运行
        /// </summary>
        /// <param name="runningContainer">货柜信息</param>
        /// <returns></returns>
        public DataResult StartRunningContainer(Command.RunningContainer runningContainer)
        {
            try
            {
                if (runningContainer.TrayCode<=0)
                {
                    return DataProcess.Failure("托盘序号不可为0，请核查数据");
                }
              //  runningContainer.ContainerType = 3;//正泰
                if (isConnected || runningContainer.ContainerType==1 || runningContainer.ContainerType==2)
                {
                    int SignalDelay = 500;//Convert.ToInt32(ConfigurationManager.AppSettings["SignalDelay"].ToString());
                    //runningContainer.ContainerType = 3;
                    if (runningContainer.ContainerType==3)
                    {
                        var M65111Result = melsec_net.ReadBool("M651");
                        if (M65111Result.IsSuccess)
                        {
                            if (M65111Result.Content == true)
                            {
                                return DataProcess.Failure("取出动作尚未结束");
                            }
                        }
                        var M654Result = melsec_net.ReadBool("M654");
                        if (M654Result.IsSuccess)
                        {
                            if (M654Result.Content == true)
                            {
                                return DataProcess.Failure("存入动作尚未结束");
                            }
                        }
                        var M900Result = melsec_net.ReadBool("M900");
                        if (M900Result.IsSuccess)
                        {
                            if (M900Result.Content == true)
                            {
                                return DataProcess.Failure("更换托盘动作尚未结束");
                            }
                        }
                        var TrayResult = melsec_net.Write("D650", runningContainer.TrayCode);
                        if (TrayResult.IsSuccess)
                        {
                            //2.	写入D218 十进制 X轴灯号 
                            //var XResult = melsec_net.Write("D219", runningContainer.XLight);
                            var XResult = melsec_net.Write("D251", runningContainer.XLight);
                            Thread.Sleep(SignalDelay);
                            var XLenghtResult = melsec_net.Write("D250",runningContainer.XLenght);
                            if (XResult.IsSuccess)
                            {
                                //发送 3.	M651 bit  置为 ON
                                var M650Result = melsec_net.Write("M650", true);
                                if (M650Result.IsSuccess)
                                {
                                    Thread.Sleep(SignalDelay);
                                    M650Result = melsec_net.Write("M650", false);
                                    if (!M650Result.IsSuccess)
                                    {
                                        return DataProcess.Failure("M650置为OFF失败" + M650Result.Message);
                                    }

                                    var M651Result = melsec_net.Write("M651", true);
                                    if (!M651Result.IsSuccess)
                                    {
                                        return DataProcess.Failure("M651置为ON失败" + M651Result.Message);
                                    }
                                    //Thread.Sleep(SignalDelay);

                                    //M651Result = melsec_net.Write("M651", false);
                                    //if (!M651Result.IsSuccess)
                                    //{
                                    //    return DataProcess.Failure("M651置为OFF失败" + M651Result.Message);
                                    //}
                                }
                                else
                                {
                                    return DataProcess.Failure("M650置为ON失败" + M650Result.Message);
                                }
                            }
                            else
                            {
                                return DataProcess.Failure("写入X轴灯号失败:" + XResult.Message);
                            }
                        }
                        else
                        {
                            return DataProcess.Failure("写入托盘序号失败:" + TrayResult.Message);
                        }
                    }
                    else if (runningContainer.ContainerType==1)
                    {
                        return StartRunningC3000Container(runningContainer);
                    }
                    else if (runningContainer.ContainerType==2)
                    {
                        return StartRunningHanelContainer(runningContainer);
                    }
                    else
                    {
                        //1.	写入D490  十进制 运行货柜的托盘序号
                        var TrayResult = melsec_net.Write("D490", runningContainer.TrayCode);
                        if (TrayResult.IsSuccess)
                        {
                            //2.	写入R218 十进制 X轴灯号 
                            //var XResult = melsec_net.Write("R218", runningContainer.XLight);
                            var XResult = melsec_net.Write("D251", runningContainer.XLight);
                            Thread.Sleep(SignalDelay);
                            var XLenghtResult = melsec_net.Write("D250", runningContainer.XLenght);
                            if (XResult.IsSuccess)
                            {
                                //发送 3.	M50  bit  置为 ON
                                var M5OResult = melsec_net.Write("M50", true);
                                if (M5OResult.IsSuccess)
                                {
                                    Thread.Sleep(SignalDelay);
                                    M5OResult = melsec_net.Write("M50", false);
                                    if (!M5OResult.IsSuccess)
                                    {
                                        return DataProcess.Failure("M50置为OFF失败" + M5OResult.Message);
                                    }

                                    var M30Result = melsec_net.Write("M30", true);
                                    if (!M30Result.IsSuccess)
                                    {
                                        return DataProcess.Failure("M30置为ON失败" + M5OResult.Message);
                                    }
                                    Thread.Sleep(SignalDelay);

                                    M30Result = melsec_net.Write("M30", false);
                                    if (!M30Result.IsSuccess)
                                    {
                                        return DataProcess.Failure("M30置为OFF失败" + M5OResult.Message);
                                    }
                                }
                                else
                                {
                                    return DataProcess.Failure("M50置为ON失败" + M5OResult.Message);
                                }
                            }
                            else
                            {
                                return DataProcess.Failure("写入X轴灯号失败:" + XResult.Message);
                            }
                        }
                        else
                        {
                            return DataProcess.Failure("写入托盘序号失败:" + TrayResult.Message);
                        }
                    }

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception EX)
            {

                return DataProcess.Failure(EX.Message);
            }
            return DataProcess.Success();
        }

        /// <summary>
        /// 双托盘启动货柜逻辑-自动运行
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult DoubleTrayStartRunningContainer(Command.RunningContainer runningContainer)
        {
            try
            {
                if (runningContainer.TrayCode <= 0)
                {
                    return DataProcess.Failure("托盘序号不可为0，请核查数据");
                }
                if (isConnected || runningContainer.ContainerType == 1 || runningContainer.ContainerType == 2)
                {
                    int SignalDelay = 500;
                    if (runningContainer.ContainerType == 3)
                    {
                        var M65111Result = melsec_net.ReadBool("M651");
                        if (M65111Result.IsSuccess)
                        {
                            if (M65111Result.Content == true)
                            {
                                return DataProcess.Failure("取出动作尚未结束");
                            }
                        }
                        var M654Result = melsec_net.ReadBool("M654");
                        if (M654Result.IsSuccess)
                        {
                            if (M654Result.Content == true)
                            {
                                return DataProcess.Failure("存入动作尚未结束");
                            }
                        }
                        var M900Result = melsec_net.ReadBool("M900");
                        if (M900Result.IsSuccess)
                        {
                            if (M900Result.Content == true)
                            {
                                return DataProcess.Failure("更换托盘动作尚未结束");
                            }
                        }
                        //2.	写入D218 十进制 X轴灯号 
                        //var XResult = melsec_net.Write("D219", runningContainer.XLight);
                        var XResult = melsec_net.Write("D251", runningContainer.XLight);
                        Thread.Sleep(SignalDelay);
                        var XLenghtResult = melsec_net.Write("D250", runningContainer.XLenght);
                        if (XResult.IsSuccess)
                        {
                            var M672Result = melsec_net.Write("M672", true);
                            if (M672Result.IsSuccess)
                            {
                                Thread.Sleep(200);
                                M672Result = melsec_net.Write("M672", false);
                                if (!M672Result.IsSuccess)
                                {
                                    return DataProcess.Failure("M672置为OFF失败" + M672Result.Message);
                                }
                            }
                        }
                        else
                        {
                            return DataProcess.Failure("写入X轴灯号失败:" + XResult.Message);
                        }
                    }
                    
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception EX)
            {

                return DataProcess.Failure(EX.Message);
            }
            return DataProcess.Success();
        }

        /// <summary>
        ///双托盘启动货柜逻辑-再次自动运行
        /// </summary>
        /// <param name="runningContainer">货柜信息</param>
        /// <returns></returns>
        public DataResult DoubleTrayStartRunningContainerAgain(Command.RunningContainer runningContainer)
        {
            try
            {
                if (runningContainer.TrayCode <= 0)
                {
                    return DataProcess.Failure("托盘序号不可为0，请核查数据");
                }
                //  runningContainer.ContainerType = 3;//正泰
                if (isConnected || runningContainer.ContainerType == 1 || runningContainer.ContainerType == 2)
                {
                    int SignalDelay = 500;//Convert.ToInt32(ConfigurationManager.AppSettings["SignalDelay"].ToString());
                    //runningContainer.ContainerType = 3;
                    if (runningContainer.ContainerType == 3)
                    {
                        var M65111Result = melsec_net.ReadBool("M651");
                        if (M65111Result.IsSuccess)
                        {
                            if (M65111Result.Content == true)
                            {
                                return DataProcess.Failure("取出动作尚未结束");
                            }
                        }
                        var M654Result = melsec_net.ReadBool("M654");
                        if (M654Result.IsSuccess)
                        {
                            if (M654Result.Content == true)
                            {
                                return DataProcess.Failure("存入动作尚未结束");
                            }
                        }
                        var M900Result = melsec_net.ReadBool("M900");
                        if (M900Result.IsSuccess)
                        {
                            if (M900Result.Content == true)
                            {
                                return DataProcess.Failure("更换托盘动作尚未结束");
                            }
                        }
                        //2.	写入D218 十进制 X轴灯号 
                        //var XResult = melsec_net.Write("D219", runningContainer.XLight);
                        var XResult = melsec_net.Write("D251", runningContainer.XLight);
                        Thread.Sleep(SignalDelay);
                        var XLenghtResult = melsec_net.Write("D250", runningContainer.XLenght);
                        if (XResult.IsSuccess)
                        {
                            var M280Result = melsec_net.Write("M280", true);
                            if (M280Result.IsSuccess)
                            {
                                Thread.Sleep(100);
                                M280Result = melsec_net.Write("M280", false);
                                if (!M280Result.IsSuccess)
                                {
                                    return DataProcess.Failure("M280置为OFF失败" + M280Result.Message);
                                }
                                Thread.Sleep(300);
                                var M281Result = melsec_net.Write("M281", true);
                                if (M281Result.IsSuccess)
                                {
                                    Thread.Sleep(100);
                                    M281Result = melsec_net.Write("M281", false);
                                    if (!M281Result.IsSuccess)
                                    {
                                        return DataProcess.Failure("M281置为OFF失败" + M280Result.Message);
                                    }
                                }
                                else
                                {
                                    return DataProcess.Failure("M281置为ON失败" + M280Result.Message);

                                }
                            }
                            else
                            {
                                return DataProcess.Failure("M280置为ON失败" + M280Result.Message);
                            }

                            var M672Result = melsec_net.Write("M672", true);
                            if (M672Result.IsSuccess)
                            {
                                Thread.Sleep(200);
                                M672Result = melsec_net.Write("M672", false);
                                if (!M672Result.IsSuccess)
                                {
                                    return DataProcess.Failure("M672置为OFF失败" + M672Result.Message);
                                }
                            }
                        }
                        else
                        {
                            return DataProcess.Failure("写入X轴灯号失败:" + XResult.Message);
                        }

                    }

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception EX)
            {

                return DataProcess.Failure(EX.Message);
            }
            return DataProcess.Success();
        }

        /// <summary>
        ///启动货柜逻辑-再次自动运行
        /// </summary>
        /// <param name="runningContainer">货柜信息</param>
        /// <returns></returns>
        public DataResult StartRunningContainerAgain(Command.RunningContainer runningContainer)
        {
            try
            {
                if (runningContainer.DoubleTrayCode <= 0)
                {
                    return DataProcess.Failure("托盘序号不可为0，请核查数据");
                }
                //  runningContainer.ContainerType = 3;//正泰
                if (isConnected || runningContainer.ContainerType == 1 || runningContainer.ContainerType == 2)
                {
                    int SignalDelay = 500;//Convert.ToInt32(ConfigurationManager.AppSettings["SignalDelay"].ToString());
                    //runningContainer.ContainerType = 3;
                    if (runningContainer.ContainerType == 3)
                    {
                        var M65111Result = melsec_net.ReadBool("M651");
                        if (M65111Result.IsSuccess)
                        {
                            if (M65111Result.Content == true)
                            {
                                return DataProcess.Failure("取出动作尚未结束");
                            }
                        }
                        var M654Result = melsec_net.ReadBool("M654");
                        if (M654Result.IsSuccess)
                        {
                            if (M654Result.Content == true)
                            {
                                return DataProcess.Failure("存入动作尚未结束");
                            }
                        }
                        var M900Result = melsec_net.ReadBool("M900");
                        if (M900Result.IsSuccess)
                        {
                            if (M900Result.Content == true)
                            {
                                return DataProcess.Failure("更换托盘动作尚未结束");
                            }
                        }
                        var TrayResult = melsec_net.Write("D650", runningContainer.DoubleTrayCode);
                        if (TrayResult.IsSuccess)
                        {
                            //2.	写入D218 十进制 X轴灯号 
                            //var XResult = melsec_net.Write("D219", runningContainer.XLight);
                            var XResult = melsec_net.Write("D251", runningContainer.XLight);
                            Thread.Sleep(SignalDelay);
                            var XLenghtResult = melsec_net.Write("D250", runningContainer.XLenght);
                            if (XResult.IsSuccess)
                            {
                                //var M280Result = melsec_net.Write("M280",true);
                                //if (M280Result.IsSuccess)
                                //{
                                //    Thread.Sleep(100);
                                //    M280Result = melsec_net.Write("M280", false);
                                //    if (!M280Result.IsSuccess)
                                //    {
                                //        return DataProcess.Failure("M280置为OFF失败" + M280Result.Message);
                                //    }
                                //    Thread.Sleep(500);
                                //    var M281Result = melsec_net.Write("M281", true);
                                //    if (M281Result.IsSuccess)
                                //    {
                                //        Thread.Sleep(100);
                                //        M281Result = melsec_net.Write("M281", false);
                                //        if (!M281Result.IsSuccess)
                                //        {
                                //            return DataProcess.Failure("M281置为OFF失败" + M280Result.Message);
                                //        }
                                //    }
                                //    else
                                //    {
                                //        return DataProcess.Failure("M281置为ON失败" + M280Result.Message);

                                //    }
                                //}
                                //else
                                //{
                                //    return DataProcess.Failure("M280置为ON失败" + M280Result.Message);
                                //}

                                //发送 3.	M651 bit  置为 ON
                                var M650Result = melsec_net.Write("M650", true);
                                if (M650Result.IsSuccess)
                                {
                                    Thread.Sleep(SignalDelay);
                                    M650Result = melsec_net.Write("M650", false);
                                    if (!M650Result.IsSuccess)
                                    {
                                        return DataProcess.Failure("M650置为OFF失败" + M650Result.Message);
                                    }

                                    var M651Result = melsec_net.Write("M651", true);
                                    if (!M651Result.IsSuccess)
                                    {
                                        return DataProcess.Failure("M651置为ON失败" + M651Result.Message);
                                    }
                                    //Thread.Sleep(SignalDelay);

                                    //M651Result = melsec_net.Write("M651", false);
                                    //if (!M651Result.IsSuccess)
                                    //{
                                    //    return DataProcess.Failure("M651置为OFF失败" + M651Result.Message);
                                    //}
                                }
                                else
                                {
                                    return DataProcess.Failure("M650置为ON失败" + M650Result.Message);
                                }
                            }
                            else
                            {
                                return DataProcess.Failure("写入X轴灯号失败:" + XResult.Message);
                            }
                        }
                        else
                        {
                            return DataProcess.Failure("写入托盘序号失败:" + TrayResult.Message);
                        }
                    }
                    else if (runningContainer.ContainerType == 1)
                    {
                        return StartRunningC3000Container(runningContainer);
                    }
                    else if (runningContainer.ContainerType == 2)
                    {
                        return StartRunningHanelContainer(runningContainer);
                    }
                    else
                    {
                        //1.	写入D490  十进制 运行货柜的托盘序号
                        var TrayResult = melsec_net.Write("D490", runningContainer.TrayCode);
                        if (TrayResult.IsSuccess)
                        {
                            //2.	写入R218 十进制 X轴灯号 
                            //var XResult = melsec_net.Write("R218", runningContainer.XLight);
                            var XResult = melsec_net.Write("D251", runningContainer.XLight);
                            Thread.Sleep(SignalDelay);
                            var XLenghtResult = melsec_net.Write("D250", runningContainer.XLenght);
                            if (XResult.IsSuccess)
                            {
                                //发送 3.	M50  bit  置为 ON
                                var M5OResult = melsec_net.Write("M50", true);
                                if (M5OResult.IsSuccess)
                                {
                                    Thread.Sleep(SignalDelay);
                                    M5OResult = melsec_net.Write("M50", false);
                                    if (!M5OResult.IsSuccess)
                                    {
                                        return DataProcess.Failure("M50置为OFF失败" + M5OResult.Message);
                                    }

                                    var M30Result = melsec_net.Write("M30", true);
                                    if (!M30Result.IsSuccess)
                                    {
                                        return DataProcess.Failure("M30置为ON失败" + M5OResult.Message);
                                    }
                                    Thread.Sleep(SignalDelay);

                                    M30Result = melsec_net.Write("M30", false);
                                    if (!M30Result.IsSuccess)
                                    {
                                        return DataProcess.Failure("M30置为OFF失败" + M5OResult.Message);
                                    }
                                }
                                else
                                {
                                    return DataProcess.Failure("M50置为ON失败" + M5OResult.Message);
                                }
                            }
                            else
                            {
                                return DataProcess.Failure("写入X轴灯号失败:" + XResult.Message);
                            }
                        }
                        else
                        {
                            return DataProcess.Failure("写入托盘序号失败:" + TrayResult.Message);
                        }
                    }

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception EX)
            {

                return DataProcess.Failure(EX.Message);
            }
            return DataProcess.Success();
        }

        /// <summary>
        /// 启动货柜逻辑-自动存入
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult StartRestoreContainer(Command.RunningContainer runningContainer)
        {
            if (isConnected || runningContainer.ContainerType == 1 || runningContainer.ContainerType == 2)
            {

                if (runningContainer.TrayCode <= 0)
                {
                    return DataProcess.Failure("托盘序号不可为0，请核查数据");
                }
              //  runningContainer.ContainerType = 3;//正泰
                if (isConnected || runningContainer.ContainerType == 1 || runningContainer.ContainerType == 2)
                {
                    int SignalDelay = 500;//Convert.ToInt32(ConfigurationManager.AppSettings["SignalDelay"].ToString());
                    //runningContainer.ContainerType = 3;
                    if (runningContainer.ContainerType == 3)
                    {
                        var M651Result = melsec_net.ReadBool("M651");
                        if (M651Result.IsSuccess)
                        {
                            if (M651Result.Content == true)
                            {
                                return DataProcess.Failure("取出动作尚未结束");
                            }
                        }
                        var M654Result = melsec_net.ReadBool("M654");
                        if (M654Result.IsSuccess)
                        {
                            if (M654Result.Content == true)
                            {
                                return DataProcess.Failure("存入动作尚未结束");
                            }
                        }
                        var M900Result = melsec_net.ReadBool("M900");
                        if (M900Result.IsSuccess)
                        {
                            if (M900Result.Content == true)
                            {
                                return DataProcess.Failure("更换托盘动作尚未结束");
                            }
                        }
                        var result = WriteD650_In(runningContainer);
                        if (result.Success)
                        {
                            result = SetM654True();
                            if (!result.Success)
                            {
                                return DataProcess.Failure(result.Message);
                            }
                        }
                        else
                        {
                            return DataProcess.Failure(result.Message);
                        }
                        return DataProcess.Success();
                    }
                    else if (runningContainer.ContainerType == 1)
                    {
                        return StartRestoreC3000Container(runningContainer);
                    }
                    else if (runningContainer.ContainerType == 2)
                    {
                        return FinishHanellContainer(runningContainer);
                    }
                    else
                    {

                    }

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            return DataProcess.Success();
        }

        /// <summary>
        /// 熄灯
        /// </summary>
        /// <returns></returns>
        public DataResult OffXLight(Command.RunningContainer runningContainer)
        {
            if (isConnected)
            {
                int SignalDelay = Convert.ToInt32(ConfigurationManager.AppSettings["SignalDelay"].ToString());
                if (runningContainer.ContainerType == 3)
                {
                    var XResult = melsec_net.Write("D219", 0);
                    if (XResult.IsSuccess)
                    {
                        return DataProcess.Success();
                    }
                    else
                    {
                        return DataProcess.Failure("X轴灯号赋值0失败");
                    }
                }
                else
                {
                    var XResult = melsec_net.Write("R218", 0);
                    if (XResult.IsSuccess)
                    {
                        return DataProcess.Success();
                    }
                    else
                    {
                        return DataProcess.Failure("X轴灯号赋值0失败");
                    }
                }
            }
            else
            {
                return DataProcess.Failure("PLC未连接");
            }

        }
        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <param name="PlcIp"></param>
        /// <param name="PlcPort"></param>
        /// <param name="melsec_net"></param>
        /// <returns></returns>
        private DataResult ConnectPLC(string PlcIp,int PlcPort, MelsecA1ENet melsec_net)
        {
            // 连接
            System.Net.IPAddress address;
            if (!System.Net.IPAddress.TryParse(PlcIp, out address))
            {
                return DataProcess.Failure("Ip地址输入不正确");
            }
            melsec_net.IpAddress = PlcIp;
            int port = PlcPort;

            melsec_net.Port = port;

            melsec_net.ConnectClose();

            try
            {
                var connect = melsec_net.ConnectServer();
                if (connect.IsSuccess)
                {
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("连接PLC失败:"+connect.Message);
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 料斗行程设定
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult HopperSetting()
        {
            try
            {
                if (isConnected)
                {
                    int SignalDelay = Convert.ToInt32(ConfigurationManager.AppSettings["SignalDelay"].ToString());
                    int SignalSettingTime = Convert.ToInt32(ConfigurationManager.AppSettings["SignalSettingTime"].ToString());
                    int SignalTime = Convert.ToInt32(ConfigurationManager.AppSettings["SignalTime"].ToString());
                    {
                        //1.	M600  bit  置为 ON
                        var M600Result = melsec_net.Write("M600", true);
                        if (!M600Result.IsSuccess)
                        {
                            return DataProcess.Failure("M600置为ON失败" + M600Result.Message);
                        }
                        Thread.Sleep(SignalDelay);
                        //2.	M600  bit  置为 OFF
                        M600Result = melsec_net.Write("M600", false);
                        if (!M600Result.IsSuccess)
                        {
                            return DataProcess.Failure("M600置为OFF失败" + M600Result.Message);
                        }
                        //3.判断 当 M613 为ON时
                        var i = 0;
                        var M613Result = melsec_net.ReadBool("M613");
                        if (!M613Result.IsSuccess)
                        {
                            return DataProcess.Failure("读取M613状态失败:" + M613Result.Message);
                        }

                        // 循环判断M613的值，如果不为On，则循环读取，且小于读取次数
                        while ((!M613Result.Content) && i< SignalTime) 
                        {
                            Thread.Sleep(SignalSettingTime);
                            M613Result = melsec_net.ReadBool("M613");
                            if (!M613Result.IsSuccess)
                            {
                                return DataProcess.Failure("读取M613状态失败:" + M613Result.Message);
                            }
                            i++;
                        }

                        // 如果超过读取的次数
                        if (i>= SignalTime)
                        {
                            return DataProcess.Failure("料斗行程设定超时！");
                        }

                        //4.	M614  bit  置为ON
                        var M614Result = melsec_net.Write("M614", true);
                        if (!M614Result.IsSuccess)
                        {
                            return DataProcess.Failure("M614置为ON失败" + M614Result.Message);
                        }
                        Thread.Sleep(SignalDelay);
                        //	5.	M614  bit  置为 OFF
                        M614Result = melsec_net.Write("M614", false);
                        if (!M614Result.IsSuccess)
                        {
                            return DataProcess.Failure("M614置为OFF失败" + M614Result.Message);
                        }

                    }
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
            return DataProcess.Success();
        }



        /// <summary>
        /// 安全门行程设定
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult EmergencyDoorSetting()
        {
            try
            {
                if (isConnected)
                {
                    int SignalDelay = Convert.ToInt32(ConfigurationManager.AppSettings["SignalDelay"].ToString());
                    int SignalSettingTime = Convert.ToInt32(ConfigurationManager.AppSettings["SignalSettingTime"].ToString());
                    int SignalTime = Convert.ToInt32(ConfigurationManager.AppSettings["SignalTime"].ToString());
                    {
                        //1.	M601  bit  置为 ON
                        var M601Result = melsec_net.Write("M601", true);
                        if (!M601Result.IsSuccess)
                        {
                            return DataProcess.Failure("M601置为ON失败" + M601Result.Message);
                        }
                        Thread.Sleep(SignalDelay);
                        //2.	M601  bit  置为 OFF
                        M601Result = melsec_net.Write("M601", false);
                        if (!M601Result.IsSuccess)
                        {
                            return DataProcess.Failure("M601置为OFF失败" + M601Result.Message);
                        }
                        //3.判断 当 M626为ON时
                        var i = 0;
                        var M626Result = melsec_net.ReadBool("M626");
                        if (!M626Result.IsSuccess)
                        {
                            return DataProcess.Failure("读取M626状态失败:" + M626Result.Message);
                        }

                        // 循环判断M626的值，如果不为On，则循环读取，且小于读取次数
                        while ((!M626Result.Content) && i < SignalTime)
                        {
                            Thread.Sleep(SignalSettingTime);
                            M626Result = melsec_net.ReadBool("M626");
                            if (!M626Result.IsSuccess)
                            {
                                return DataProcess.Failure("读取M613状态失败:" + M626Result.Message);
                            }
                            i++;
                        }

                        // 如果超过读取的次数
                        if (i >= SignalTime)
                        {
                            return DataProcess.Failure("安全门行程设定超时！");
                        }

                        //4.	M627  bit  置为ON
                        var M627Result = melsec_net.Write("M627", true);
                        if (!M627Result.IsSuccess)
                        {
                            return DataProcess.Failure("M627置为ON失败" + M627Result.Message);
                        }
                        Thread.Sleep(SignalDelay);
                        //	5.	M627  bit  置为 OFF
                        M627Result = melsec_net.Write("M627", false);
                        if (!M627Result.IsSuccess)
                        {
                            return DataProcess.Failure("M627置为OFF失败" + M627Result.Message);
                        }
                    }
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }

            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
            return DataProcess.Success();
        }

        /// <summary>
        /// 获取PLC状态
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public DataResult HandFinishEmergencyDoorSetting()
        {
            try
            {
                if (isConnected)
                {
                    var M614Result = melsec_net.Write("M626", true);
                    if (!M614Result.IsSuccess)
                    {
                        return DataProcess.Failure("M626置为ON失败" + M614Result.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 获取PLC状态
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public DataResult GetPlcDeivceStatus(Command.RunningContainer runningContainer)
        {
            try
            {
               // runningContainer.ContainerType = 3;
                if (isConnected || runningContainer.ContainerType==1 || runningContainer.ContainerType==2)
                {
                    if (runningContainer.ContainerType==0 || runningContainer.ContainerType==3)
                    {
                        //01 PLC 接收远程控制指令——在线
                        //00 PLC 不接受远程控制指令——离线

                        var onLineResult = melsec_net.ReadInt32("D206");
                        if (!onLineResult.IsSuccess)
                        {
                            return DataProcess.Failure("读取PLC状态失败:" + onLineResult.Message);
                        }
                        if (onLineResult.Content == 0)
                        {
                            return DataProcess.Success();
                        }
                        else
                        {
                            return DataProcess.Failure("PLC不在线");
                        }
                    }
                    else if (runningContainer.ContainerType==1)
                    {
                        return GetC3000DeivceStatus(runningContainer);
                    }
                    else
                    {
                        return GetHanelStatus(runningContainer);
                    }
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <returns></returns>
        public DataResult GetAlarmInformation()
        {
            try
            {
                if (isConnected)
                {
                    //1.	M212  为ON 则发生报警
                    //00 PLC 不接受远程控制指令——离线
                    // 水平柜212
                    var onAlarmResult = melsec_net.ReadBool("M229");
                    if (!onAlarmResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取PLC报警状态失败:" + onAlarmResult.Message);
                    }
                    // 如果发生了报警
                    if (onAlarmResult.Content)
                    {
                        string returnStr = "";
                        var M200Result = melsec_net.ReadBool("M200");
                        if (M200Result.IsSuccess)
                        {
                            if (M200Result.Content)
                            {
                                returnStr += "200;";
                            }
                        }
                        var M201Result = melsec_net.ReadBool("M201");
                        if (M201Result.IsSuccess)
                        {
                            if (M201Result.Content)
                            {
                                returnStr += "201;";
                            }
                        }
                        var M202Result = melsec_net.ReadBool("M202");
                        if (M202Result.IsSuccess)
                        {
                            if (M202Result.Content)
                            {
                                returnStr += "202;";
                            }
                        }
                        var M203Result = melsec_net.ReadBool("M203");
                        if (M203Result.IsSuccess)
                        {
                            if (M203Result.Content)
                            {
                                returnStr += "203;";
                            }
                        }
                        var M204Result = melsec_net.ReadBool("M204");
                        if (M204Result.IsSuccess)
                        {
                            if (M204Result.Content)
                            {
                                returnStr += "204;";
                            }
                        }
                        var M205Result = melsec_net.ReadBool("M205");
                        if (M205Result.IsSuccess)
                        {
                            if (M205Result.Content)
                            {
                                returnStr += "205;";
                            }
                        }
                        var M206Result = melsec_net.ReadBool("M206");
                        if (M206Result.IsSuccess)
                        {
                            if (M206Result.Content)
                            {
                                returnStr += "206;";
                            }
                        }
                        var M207Result = melsec_net.ReadBool("M207");
                        if (M207Result.IsSuccess)
                        {
                            if (M207Result.Content)
                            {
                                returnStr += "207;";
                            }
                        }
                        var M208Result = melsec_net.ReadBool("M208");
                        if (M208Result.IsSuccess)
                        {
                            if (M208Result.Content)
                            {
                                returnStr += "208;";
                            }
                        }
                        var M209Result = melsec_net.ReadBool("M209");
                        if (M209Result.IsSuccess)
                        {
                            if (M209Result.Content)
                            {
                                returnStr += "209;";
                            }
                        }
                        var M210Result = melsec_net.ReadBool("M210");
                        if (M210Result.IsSuccess)
                        {
                            if (M210Result.Content)
                            {
                                returnStr += "210;";
                            }
                        }
                        var M211Result = melsec_net.ReadBool("M211");
                        if (M211Result.IsSuccess)
                        {
                            if (M211Result.Content)
                            {
                                returnStr += "211;";
                            }
                        }

                        //垂直回转柜 新加
                        var M212Result = melsec_net.ReadBool("M212");
                        if (M212Result.IsSuccess)
                        {
                            if (M212Result.Content)
                            {
                                returnStr += "212;";
                            }
                        }
                        var M213Result = melsec_net.ReadBool("M213");
                        if (M213Result.IsSuccess)
                        {
                            if (M213Result.Content)
                            {
                                returnStr += "213;";
                            }
                        }
                        var M214Result = melsec_net.ReadBool("M214");
                        if (M214Result.IsSuccess)
                        {
                            if (M214Result.Content)
                            {
                                returnStr += "214;";
                            }
                        }
                        var M215Result = melsec_net.ReadBool("M241");
                        if (M215Result.IsSuccess)
                        {
                            if (M215Result.Content)
                            {
                                returnStr += "241;";
                            }
                        }
                        var M216Result = melsec_net.ReadBool("M216");
                        if (M216Result.IsSuccess)
                        {
                            if (M216Result.Content)
                            {
                                returnStr += "216;";
                            }
                        }
                        var M217Result = melsec_net.ReadBool("M217");
                        if (M217Result.IsSuccess)
                        {
                            if (M217Result.Content)
                            {
                                returnStr += "217;";
                            }
                        }
                        var M218Result = melsec_net.ReadBool("M218");
                        if (M218Result.IsSuccess)
                        {
                            if (M218Result.Content)
                            {
                                returnStr += "206;";
                            }
                        }
                        var M219Result = melsec_net.ReadBool("M219");
                        if (M219Result.IsSuccess)
                        {
                            if (M219Result.Content)
                            {
                                returnStr += "219;";
                            }
                        }
                        var M220Result = melsec_net.ReadBool("M220");
                        if (M220Result.IsSuccess)
                        {
                            if (M220Result.Content)
                            {
                                returnStr += "220;";
                            }
                        }
                        var M221Result = melsec_net.ReadBool("M221");
                        if (M221Result.IsSuccess)
                        {
                            if (M221Result.Content)
                            {
                                returnStr += "221;";
                            }
                        }
                        var M222Result = melsec_net.ReadBool("M222");
                        if (M222Result.IsSuccess)
                        {
                            if (M222Result.Content)
                            {
                                returnStr += "222;";
                            }
                        }
                        var M1200Result = melsec_net.ReadBool("M1200");
                        if (M1200Result.IsSuccess)
                        {
                            if (M1200Result.Content)
                            {
                                returnStr += "1200;";
                            }
                        }
                        var M1201Result = melsec_net.ReadBool("M1201");
                        if (M1201Result.IsSuccess)
                        {
                            if (M1201Result.Content)
                            {
                                returnStr += "1201;";
                            }
                        }
                        var M1202Result = melsec_net.ReadBool("M1202");
                        if (M1202Result.IsSuccess)
                        {
                            if (M1202Result.Content)
                            {
                                returnStr += "1202;";
                            }
                        }
                        var M1203Result = melsec_net.ReadBool("M1203");
                        if (M1203Result.IsSuccess)
                        {
                            if (M1203Result.Content)
                            {
                                returnStr += "1203;";
                            }
                        }
                        var M1204Result = melsec_net.ReadBool("M1204");
                        if (M1204Result.IsSuccess)
                        {
                            if (M1204Result.Content)
                            {
                                returnStr += "1204;";
                            }
                        }
                        var M1205Result = melsec_net.ReadBool("M1205");
                        if (M1205Result.IsSuccess)
                        {
                            if (M1205Result.Content)
                            {
                                returnStr += "1205;";
                            }
                        }
                        var M1206Result = melsec_net.ReadBool("M1206");
                        if (M1206Result.IsSuccess)
                        {
                            if (M1206Result.Content)
                            {
                                returnStr += "1206;";
                            }
                        }
                        var M1207Result = melsec_net.ReadBool("M1207");
                        if (M1207Result.IsSuccess)
                        {
                            if (M1207Result.Content)
                            {
                                returnStr += "1207;";
                            }
                        }
                        var M1208Result = melsec_net.ReadBool("M1208");
                        if (M1208Result.IsSuccess)
                        {
                            if (M1208Result.Content)
                            {
                                returnStr += "1208;";
                            }
                        }
                        var M1209Result = melsec_net.ReadBool("M1209");
                        if (M1209Result.IsSuccess)
                        {
                            if (M1209Result.Content)
                            {
                                returnStr += "1209;";
                            }
                        }
                        var M1210Result = melsec_net.ReadBool("M1210");
                        if (M1210Result.IsSuccess)
                        {
                            if (M1210Result.Content)
                            {
                                returnStr += "1210;";
                            }
                        }
                        var M1211Result = melsec_net.ReadBool("M1211");
                        if (M1211Result.IsSuccess)
                        {
                            if (M1211Result.Content)
                            {
                                returnStr += "1211;";
                            }
                        }
                        var M1212Result = melsec_net.ReadBool("M1212");
                        if (M1212Result.IsSuccess)
                        {
                            if (M1212Result.Content)
                            {
                                returnStr += "1212;";
                            }
                        }
                        var M1213Result = melsec_net.ReadBool("M1213");
                        if (M1213Result.IsSuccess)
                        {
                            if (M1213Result.Content)
                            {
                                returnStr += "1213;";
                            }
                        }
                        var M1214Result = melsec_net.ReadBool("M1214");
                        if (M1214Result.IsSuccess)
                        {
                            if (M1214Result.Content)
                            {
                                returnStr += "1214;";
                            }
                        }
                        var M1215Result = melsec_net.ReadBool("M1215");
                        if (M1215Result.IsSuccess)
                        {
                            if (M1215Result.Content)
                            {
                                returnStr += "1215;";
                            }
                        }
                        var M1216Result = melsec_net.ReadBool("M1216");
                        if (M1216Result.IsSuccess)
                        {
                            if (M1216Result.Content)
                            {
                                returnStr += "1216;";
                            }
                        }
                        var M1217Result = melsec_net.ReadBool("M1217");
                        if (M1217Result.IsSuccess)
                        {
                            if (M1217Result.Content)
                            {
                                returnStr += "1217;";
                            }
                        }
                        var M1218Result = melsec_net.ReadBool("M1218");
                        if (M1218Result.IsSuccess)
                        {
                            if (M1218Result.Content)
                            {
                                returnStr += "1218;";
                            }
                        }
                        var M1219Result = melsec_net.ReadBool("M1219");
                        if (M1219Result.IsSuccess)
                        {
                            if (M1219Result.Content)
                            {
                                returnStr += "1219;";
                            }
                        }
                        var M1220Result = melsec_net.ReadBool("M1220");
                        if (M1220Result.IsSuccess)
                        {
                            if (M1220Result.Content)
                            {
                                returnStr += "1220;";
                            }
                        }
                        var M1221Result = melsec_net.ReadBool("M1221");
                        if (M1221Result.IsSuccess)
                        {
                            if (M1221Result.Content)
                            {
                                returnStr += "1221;";
                            }
                        }
                        var M1222Result = melsec_net.ReadBool("M1222");
                        if (M1222Result.IsSuccess)
                        {
                            if (M1222Result.Content)
                            {
                                returnStr += "1222;";
                            }
                        }
                        var M1223Result = melsec_net.ReadBool("M1223");
                        if (M1223Result.IsSuccess)
                        {
                            if (M1223Result.Content)
                            {
                                returnStr += "1223;";
                            }
                        }
                        var M1224Result = melsec_net.ReadBool("M1224");
                        if (M1224Result.IsSuccess)
                        {
                            if (M1224Result.Content)
                            {
                                returnStr += "1224;";
                            }
                        }
                        var M1225Result = melsec_net.ReadBool("M1225");
                        if (M1225Result.IsSuccess)
                        {
                            if (M1225Result.Content)
                            {
                                returnStr += "1225;";
                            }
                        }

                        if (!string.IsNullOrEmpty(returnStr))
                        {
                            returnStr = returnStr.Substring(0, returnStr.Length - 1);
                        }
                        return DataProcess.Success("查询成功",returnStr);
                    }
                    else
                    {
                        return DataProcess.Failure("PLC无报警");
                    }
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        /// <summary>
        /// 复位报警信息
        /// </summary>
        /// <returns></returns>
        public DataResult ResetAlarm()
        {
            try
            {
                if (isConnected)
                {
                    int SignalDelay = Convert.ToInt32(ConfigurationManager.AppSettings["SignalDelay"].ToString());
                    {
                        //1.	M601  bit  置为 ON

                        //水平柜 M215  //回转柜M230
                        var M215Result = melsec_net.Write("M230", true);
                        if (!M215Result.IsSuccess)
                        {
                            return DataProcess.Failure("M230置为ON失败" + M215Result.Message);
                        }
                        Thread.Sleep(SignalDelay);
                        //2.	M600  bit  置为 OFF
                        M215Result = melsec_net.Write("M230", false);
                        if (!M215Result.IsSuccess)
                        {
                            return DataProcess.Failure("M230置为OFF失败" + M215Result.Message);
                        }
                    }
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
            return DataProcess.Success();
        }

        #endregion
    }


    public partial class PlcServer
    {
        #region 重试机制

        private void inWorkCallBack(object obj)
        {
            try
            {
                while (InWorking)
                {

                }

            }
            catch (Exception)
            {

               
            }
        }

        /// <summary>
        /// 重连
        /// </summary>
        /// <param name="obj"></param>
        public static void WorkCallingBack(object obj)
        {
            while (true)
            {
                try
                {
                    // 1秒钟尝试重新连接一次
                    Thread.Sleep(1000);
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var read = melsec_net.ReadInt32("D206", 10);
                    if (read.IsSuccess)
                    {
                        //成功读取，委托显示
                        isConnected = true;
                       // falseTime = 0;
                    }
                    else
                    {
                        //失败读取，应该对失败信息进行日志记录，不应该显示，测试访问时才适合显示错误信息
                        //重新链接
                        melsec_net.ConnectClose();

                        var connect = melsec_net.ConnectServer();
                        if (connect.IsSuccess)
                        {
                            var test = melsec_net.ReadInt32("D206", 10);
                            if (test.IsSuccess)
                            {
                                //成功读取，委托显示
                                isConnected = true;
                           //     falseTime = 0;
                                setlog(connect.Message);
                            }
                            else
                            {
                                isConnected = false;
                           //     falseTime++;
                                setlog(connect.Message);
                            }

                            //    isConnected = true;
                        }
                        else
                        {
                            isConnected = false;
                         //   falseTime++;
                            setlog(connect.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    melsec_net.ConnectClose();

                    var connect = melsec_net.ConnectServer();
                    if (connect.IsSuccess)
                    {
                        isConnected = true;
                     //   falseTime = 0;
                    }
                    else
                    {
                        isConnected = false;
                   //     falseTime++;
                    }
                }
                System.Threading.Thread.Sleep(2000);//决定了访问的频率
            }
        }

        #endregion


        private static void setlog(string message)
        {
            string logPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\log\\";
            if (!Directory.Exists(logPath))//没有则创建
            {
                Directory.CreateDirectory(logPath);
            }
            using (FileStream stream = new FileStream(logPath + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append))
            using (StreamWriter Writer = new StreamWriter(stream))
            {
                Writer.WriteLine($"{DateTime.Now}:{message}");
            }
        }



        #region 卡迪斯
        /// <summary>
        /// 启动托盘
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult StartRunningC3000Container(Command.RunningContainer runningContainer)
        {
            try
            {
                {
                    string AddressIp = runningContainer.IpAddress + ":" + runningContainer.Port.ToString();
                    string TrayNumber = "800000" + runningContainer.TrayCode.ToString().PadLeft(2, '0');
                    string trans = "http://" + AddressIp + "/cgi-bin/setValues.exe?PDP,,DB904.DBW100,x=8001&PDP,,DB904.DBW130,x=8001&PDP,,DB904.DBW132,x=8001&PDP,,DB904.DBW134,x=8002&PDP,,DB904.DBW136,x=8002&PDP,,DB904.DBD126,x=" + TrayNumber + "& PDP,,DB904.DBD122,x=0&PDP,,DB904.DBD518,x=0&PDP,,DB904.DBD522,x=0&PDP,,DB904.DBD526,x=0&PDP,,DB904.DBD530,x=0&PDP,,DB904.DBW2,x=3";
                    var result = Common.HttpApiHelper.InvokeWebapiApi(trans, "", "get", "");
                    if (result.Code == "200" && result.Content == "Done")
                    {
                        return DataProcess.Success("发送命令成功");
                    }
                    else
                    {
                        return DataProcess.Failure("发送命令失败:" + result.Content);
                    }
                }
            }
            catch (Exception EX)
            {

                return DataProcess.Failure(EX.Message);
            }
            return DataProcess.Success();
        }

        /// <summary>
        /// 存入托盘
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult StartRestoreC3000Container(Command.RunningContainer runningContainer)
        {
            try
            {
                {
                    string AddressIp = runningContainer.IpAddress + ":" + runningContainer.Port.ToString();
                    string TrayNumber = "800000" + runningContainer.TrayCode.ToString().PadLeft(2, '0');
                    string trans = "http://" + AddressIp + "/cgi-bin/setValues.exe?PDP,,DB904.DBW100,x=8001&PDP,,DB904.DBD126,x=" + TrayNumber + "& PDP,,DB904.DBD122,x=0&PDP,,DB904.DBW2,x=4";
                    var result = Common.HttpApiHelper.InvokeWebapiApi(trans, "", "get", "");
                    if (result.Code == "200" && result.Content == "Done")
                    {
                        return DataProcess.Success("发送命令成功");
                    }
                    else
                    {
                        return DataProcess.Failure("发送命令失败:" + result.Content);
                    }
                }
            }
            catch (Exception EX)
            {

                return DataProcess.Failure(EX.Message);
            }
            return DataProcess.Success();
        }

        /// <summary>
        /// 获取货柜状态
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult GetC3000DeivceStatus(Command.RunningContainer runningContainer)
        {
            string AddressIp = runningContainer.IpAddress + ":" + runningContainer.Port.ToString();
            string command = "http://" + AddressIp + "/cgi-bin/setValues.exe?PDP,,DB904.DBW100,x=8001&PDP,,DB904.DBD122,x=0&PDP,,DB904.DBD522,x=0&PDP,,DB904.DBW2,x=2";
            var result = Common.HttpApiHelper.InvokeWebapiApi(command, "", "get", "");
            if (result.Code == "200" && result.Content == "Done")
            {
                //1 获取状态
                string getDoorStatus = "http://" + AddressIp + "/cgi-bin/OrderValues.exe?DoorStatus+dummy+1000+PDP,,DB904.DBW2,x+PDP,,DB904.DBD4,x";
                var result1 = Common.HttpApiHelper.InvokeWebapiApi(getDoorStatus, "", "get", "");
                if (result1.Code == "200")
                {

                    string ccc = "http://" + AddressIp + "/cgi-bin/ReadFile.exe?DoorStatus";
                    var doorStatusContent = Common.HttpApiHelper.InvokeWebapiApi(ccc, "", "get", "");
                    if (doorStatusContent.Code == "200")
                    {
                        //1解析字符
                        var tagsWithValue = GetTagsWithValue(doorStatusContent.Content);

                        var ControlIsBusy = Convert.ToInt32(tagsWithValue["DBW2"]) != 0;
                        if (ControlIsBusy)
                        {
                            return DataProcess.Failure("Control is busy");
                        }
                        var ResponseCode = Convert.ToInt32(tagsWithValue["DBD4"]);
                        if (ResponseCode == 20000)
                        {
                            return DataProcess.Success("货柜正常");
                        }
                        else
                        {
                            if (ResponseCode == 20001)
                            {
                                return DataProcess.Failure(" HI invalid ");
                            }
                            if (ResponseCode == 20007)
                            {
                                return DataProcess.Failure(" LevelLower invalid  ");
                            }
                            if (ResponseCode == 20008)
                            {
                                return DataProcess.Failure(" LevelUpper invalid  ");
                            }
                            else
                            {
                                return DataProcess.Failure("货柜暂时异常");
                            }
                        }
                    }
                    else
                    {
                        return DataProcess.Failure("获取状态失败:" + doorStatusContent.Content);
                    }
                }
                else
                {
                    return DataProcess.Failure("获取状态失败:" + result1.Content);
                }
            }
            else
            {
                return DataProcess.Failure("发送命令失败:" + result.Content);
            }
            return DataProcess.Success();
        }


        /// <summary>
        /// 从 ReadFile 命令的响应文本中解析出 TAG 及其值。
        /// </summary>
        /// <param name="fullResponse">响应文本。</param>
        /// <returns>TAG 及其值 的集合。</returns>
        public static Dictionary<string, string> GetTagsWithValue(string fullResponse)
        {
            string PDP__DB904_ = "PDP,,DB904.";
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(fullResponse))
            {
                string[] lines = fullResponse.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].Trim();
                    //PDP,,DB904.DBD4,x=15ffff
                    if (lines[i].StartsWith(PDP__DB904_, StringComparison.OrdinalIgnoreCase))
                    {
                        int indexOfComma = lines[i].LastIndexOf(',');
                        int indexOfEqual = lines[i].IndexOf('=');

                        if (PDP__DB904_.Length < indexOfComma && indexOfComma < indexOfEqual)
                        {
                            string key = lines[i].Substring(PDP__DB904_.Length, indexOfComma - PDP__DB904_.Length);
                            string value = lines[i].Substring(indexOfEqual + 1);

                            result[key] = value;
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region 亨乃尔
        /// <summary>
        /// 获取链接
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public Socket StartHanel(Command.RunningContainer runningContainer)
        {
            try
            {
                int port = runningContainer.Port;
                string host = runningContainer.IpAddress;
                ///创建终结点EndPoint
                IPAddress ip = IPAddress.Parse(host);
                //IPAddress ipp = new IPAddress("127.0.0.1");
                IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndpoint实例

                ///创建socket并连接到服务器
                ///
                //TcpClient tcpClient = new TcpClient();
                //tcpClient.InitSocket();
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket
                socket.Connect(ipe);//连接到服务器
                                    //connectThread = new Thread(new ThreadStart(SocketReceive));
                                    //connectThread.Start();

                // GetHanelStatus(socket);
                return socket;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        //public void SocketReceive()
        //{
        //    // SocketConnet();
        //    //不断接收服务器发来的数据  
        //    while (true)
        //    {
        //        /////接受从服务器返回的信息
        //        string recvStr = "";
        //        byte[] recvBytes = new byte[1024];
        //        int bytes;
        //        bytes = socket.Receive(recvBytes, recvBytes.Length, 0);//从服务器端接受返回信息
        //        if (bytes == 0)
        //        {
        //            continue;
        //        }
        //        recvStr = Encoding.ASCII.GetString(recvBytes, 0, bytes);
        //        string str = Encoding.ASCII.GetString(recvBytes, 0, bytes);


        //    }
        //}
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult GetHanelStatus(Command.RunningContainer runningContainer)
        {
            var socket = StartHanel(runningContainer);
            try
            {
           
                if (socket != null)
                {
                    string containerCode = runningContainer.ContainerCode.ToString();
                    string status = " * G0" + containerCode + "1:2301$U XR$006$macro=read_status$\r\n";
                    try
                    {
                        byte[] bs = Encoding.ASCII.GetBytes(status);//把字符串编码为字节

                        socket.Send(bs, bs.Length, 0);//发送信息


                        return GetReturnResult(socket);


                    }
                    catch (ArgumentNullException ex)
                    {
                        string aa = ex.Message;
                    }
                    catch (SocketException ex)
                    {
                        string aa = ex.Message;
                    }

                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("未连接到亨乃尔货柜");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
            finally
            {
                if (socket!=null)
                {
                    socket.Close();
                    //socket.Dispose();
                }
            }
        }


        public DataResult GetReturnResult(Socket socket)
        {
            //接收返回信息
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;
            bytes = socket.Receive(recvBytes, recvBytes.Length, 0);//从服务器端接受返回信息
            recvStr = Encoding.ASCII.GetString(recvBytes, 0, bytes);
            if (!string.IsNullOrEmpty(recvStr))
            {
                //*G2301:0011$V XS$1$m$E01$
                var array = recvStr.Split('$');
                if (array[4] == "E00")
                {
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Success();
                }
            }
            else
            {
                return DataProcess.Failure();
            }
        }
        /// <summary>
        /// 驱动亨乃尔托盘
        /// </summary>
        /// <returns></returns>
        public DataResult StartRunningHanelContainer(Command.RunningContainer runningContainer)
        {
            //string transfer = "*G0011:2301$U XR$"+ number+"$macro =get_shelf$PM1=1$PM2=<pm2>$PM3=<pm3> $PM4=<pm4>$PM5=<pm5>$PM6=<pm6>$PM7=<pm7>$PM8=<pm8>$PM9=<pm9> $PM10=<pm10>$PM11=<pm11>$PM12=<pm12>$PM13=<pm13>$PM14=<pm14> $PM15=<pm15>$PM16=<pm16>$\r\n";
            string number = runningContainer.TrayCode.ToString();
            string containerCode = runningContainer.ContainerCode.ToString();
            string transfer = "*G0" + containerCode + "1:2301$U XR$009$macro=get_shelf$PM1=" + number + "$\r\n";
            var socket = StartHanel(runningContainer);
            if (socket==null)
            {
                return DataProcess.Failure("链接失败");
            }
            try
            {
                byte[] bs = Encoding.ASCII.GetBytes(transfer);//把字符串编码为字节

                socket.Send(bs, bs.Length, 0);//发送信息

                return GetReturnResult(socket);
            }
            catch (ArgumentNullException ex)
            {
                string aa = ex.Message;
                return DataProcess.Failure(ex.Message);
            }
            catch (SocketException ex)
            {
                return DataProcess.Failure(ex.Message);
            }
            finally
            {
                if (socket != null)
                {
                    socket.Close();
                    //socket.Dispose();
                }
            }
            return DataProcess.Success();
        }
        /// <summary>
        /// 存储托盘
        /// </summary>
        /// <returns></returns>
        public DataResult FinishHanellContainer(Command.RunningContainer runningContainer)
        {
            string number = "009";
            string containerCode = runningContainer.ContainerCode.ToString();
            string transfer = "*G0" + containerCode + "1:2301$U XR$" + number + "$macro=store_shelf$PM14=0$\r\n";
            var socket = StartHanel(runningContainer);
            if (socket == null)
            {
                return DataProcess.Failure("链接失败");
            }
            try
            {
                byte[] bs = Encoding.ASCII.GetBytes(transfer);//把字符串编码为字节

                socket.Send(bs, bs.Length, 0);//发送信息
                return GetReturnResult(socket);

            }
            catch (ArgumentNullException ex)
            {
                return DataProcess.Failure(ex.Message);
            }
            catch (SocketException ex)
            {
                return DataProcess.Failure(ex.Message);
            }
            finally
            {
                if (socket != null)
                {
                    socket.Close();
                    //socket.Dispose();
                }
            }
            return DataProcess.Success();
        }

        #endregion


        #region 垂直货柜逻辑

        #region 垂直学习
        /// <summary>
        /// 开启垂直学习
        /// </summary>
        /// <returns></returns>
        public DataResult StartM300()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M300", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动垂直学习时报:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M300", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动垂直学习时报:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }




        /// <summary>
        /// 监视学习状态M340  true时学习结束
        /// </summary>
        /// <returns></returns>
        public DataResult GetM340()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M340");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取监视状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        ///读取前部托盘数
        /// </summary>
        /// <returns></returns>
        public DataResult GetD300()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadInt32("D300");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取前部托盘数失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        ///读取后部托盘数
        /// </summary>
        /// <returns></returns>
        public DataResult GetD301()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadInt32("D301");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取后部托盘数失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        ///结束垂直学习
        /// </summary>
        /// <returns></returns>
        public DataResult FinishM341()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M341", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("结束垂直学习时报:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M341", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("结束垂直学习时报:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        #endregion

        #region 水平学习
        /// <summary>
        /// 开启水平学习
        /// </summary>
        /// <returns></returns>
        public DataResult StartM400()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M400", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动水平学习时:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M400", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动水平学习时:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 监视学习状态M440  true时学习结束
        /// </summary>
        /// <returns></returns>
        public DataResult GetM440()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M440");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取水平学习监视状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        ///结束水平学习
        /// </summary>
        /// <returns></returns>
        public DataResult FinishM441()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M441", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("结束水平学习时报:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M441", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("结束水平学习时报:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        #endregion

        #region 自动门学习
        /// <summary>
        /// 开启自动门学习
        /// </summary>
        /// <returns></returns>
        public DataResult StartM450()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M450", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动自动门学习时:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M450", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动自动门学习时:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 监视自动门学习状态  true时学习结束
        /// </summary>
        /// <returns></returns>
        public DataResult GetM490()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M490");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取自动门学习监视状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        ///结束自动水平学习
        /// </summary>
        /// <returns></returns>
        public DataResult FinishM491()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M491", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("结束自动门学习时报:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M491", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("结束自动门学习时报:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }



        /// <summary>
        /// 确定存入
        /// </summary>
        /// <returns></returns>
        public DataResult SetM8True()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M8", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("存入失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        #endregion

        #region 托盘扫描





        /// <summary>
        ///读取托盘扫描前部托盘数
        /// </summary>
        /// <returns></returns>
        public DataResult GetD390()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadInt32("D390");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取前部托盘数失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        ///读取托盘扫描后部托盘数
        /// </summary>
        /// <returns></returns>
        public DataResult GetD391()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadInt32("D391");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取后部托盘数失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }







        /// <summary>
        /// 获取开始定义 后状态
        /// </summary>
        /// <returns></returns>
        public DataResult GetM392()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M392");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取开始定义状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }



        /// <summary>
        /// 确认 写入托盘号完毕
        /// </summary>
        /// <returns></returns>
        public DataResult ConfirmM393()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M393", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("确认失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M393", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("确认失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }









        #endregion

        #region 自动存取托盘
        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <returns></returns>
        public DataResult WriteD650(Command.RunningContainer runningContainer)
        {
            try
            {
                if (isConnected)
                {
                    int SignalDelay = 500;
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("D650", runningContainer.TrayCode);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("写入取出托盘时:" + onLineResult.Message);
                    }
                    //var XResult = melsec_net.Write("D219", runningContainer.XLight);
                    var XResult = melsec_net.Write("D251", runningContainer.XLight);
                    Thread.Sleep(SignalDelay);
                    var XLenghtResult = melsec_net.Write("D250",runningContainer.XLenght);
                    if (XResult.IsSuccess)
                    {
                        return DataProcess.Success();
                    }
                    else
                    {
                        return DataProcess.Failure("写入取出托盘灯号时:" + onLineResult.Message);
                    }
                      
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 获取M650状态
        /// </summary>
        /// <returns></returns>
        public DataResult StartM650()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M650", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(500);
                    onLineResult = melsec_net.Write("M650", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }

                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        /// <summary>
        /// 启动M651
        /// </summary>
        /// <returns></returns>
        public DataResult StartM651()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M651", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    //System.Threading.Thread.Sleep(200);
                    //onLineResult = melsec_net.Write("M651", false);
                    //if (!onLineResult.IsSuccess)
                    //{
                    //    return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    //}
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 获取M650状态
        /// </summary>
        /// <returns></returns>
        public DataResult GetM650()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M650");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("获取状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 获取D651 托盘所在托架号
        /// </summary>
        /// <returns></returns>
        public DataResult GetD651()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadInt32("D651");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("获取失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
 

        /// <summary>
        /// 获取物料高度
        /// </summary>
        /// <returns></returns>
        public DataResult GetD652()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadInt32("D652");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("获取失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }







        public DataResult WriteD650_In(Command.RunningContainer runningContainer)
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("D650", runningContainer.TrayCode);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("写入存入托盘时:" + onLineResult.Message);
                    }
                    var XResult = melsec_net.Write("D219", 0);
                    if (XResult.IsSuccess)
                    {
                        return DataProcess.Success();
                    }
                    else
                    {
                        return DataProcess.Failure("写入托盘灯号时:" + onLineResult.Message);
                    }
                    return DataProcess.Success();

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        /// <summary>
        /// 确定取出
        /// </summary>
        /// <returns></returns>
        public DataResult SetM651True()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var M654Result = melsec_net.ReadBool("M654");
                    if (M654Result.IsSuccess)
                    {
                        if (M654Result.Content == true)
                        {
                            return DataProcess.Failure("存入动作尚未结束");
                        }
                    }
                    else
                    {
                        return DataProcess.Failure("获取M654状态失败");
                    }

                    var onLineResult = melsec_net.Write("M651", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("存入失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 确定存入
        /// </summary>
        /// <returns></returns>
        public DataResult SetM654True()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var M651Result = melsec_net.ReadBool("M651");
                    if (M651Result.IsSuccess)
                    {
                        if (M651Result.Content==true)
                        {
                            return DataProcess.Failure("取出动作尚未结束");
                        }
                    }
                    else
                    {
                        return DataProcess.Failure("获取M651状态失败");
                    }

                    var onLineResult = melsec_net.Write("M654", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("存入失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        #endregion

        #region 添加托盘
        /// <summary>
        /// 添加托盘
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult WriteD700(Command.RunningContainer runningContainer)
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("D700", runningContainer.TrayCode);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("写入托盘失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 启动M700
        /// </summary>
        /// <returns></returns>
        public DataResult StartM700()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M700", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M700", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 获取M701状态 true 空间足够 
        /// </summary>
        /// <returns></returns>
        public DataResult GetM701()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M701");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("获取状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 确认存入
        /// </summary>
        /// <returns></returns>
        public DataResult ConfirmM702()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M702", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("存入失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M702", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("存入失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        #endregion

        #region 删除托盘
        /// <summary>
        /// 写入需要删除的托盘
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        public DataResult WriteD750(Command.RunningContainer runningContainer)
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("D750", runningContainer.TrayCode);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("写入托盘失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 启动M750
        /// </summary>
        /// <returns></returns>
        public DataResult StartM750()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M750", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M750", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 获取托盘所在货架号
        /// </summary>
        /// <returns></returns>
        public DataResult GetD751()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadInt32("D751");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("获取状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 启动M751
        /// </summary>
        /// <returns></returns>
        public DataResult StartM751()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M751", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M751", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }



        /// <summary>
        /// 监视托盘
        /// </summary>
        /// <returns></returns>
        public DataResult GetM752()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M752");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("获取状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 确认删除
        /// </summary>
        /// <returns></returns>
        public DataResult ConfirmM753()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M753", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("存入失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M753", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("存入失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        /// <summary>
        /// 取消删除
        /// </summary>
        /// <returns></returns>
        public DataResult ConfirmM754()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M754", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("存入失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M754", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("存入失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        #endregion

        #region 整理存储空间

        /// <summary>
        /// 开始整理
        /// </summary>
        /// <returns></returns>
        public DataResult StartM800()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M800", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M800", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 监视托盘
        /// </summary>
        /// <returns></returns>
        public DataResult GetM801()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M801");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("获取状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 获取空间利用率
        /// </summary>
        /// <returns></returns>
        public DataResult GetD800()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadInt32("D800");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("获取空间利用率:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 确认整理完毕
        /// </summary>
        /// <returns></returns>
        public DataResult ConfirmM802()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M802", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("确认失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M802", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("确认失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        #endregion


        #region 手动垂直运行

        /// <summary>
        /// 写入托架号
        /// </summary>
        /// <returns></returns>
        public DataResult WriteD500(Command.RunningContainer runningContainer)
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("D500", runningContainer.TrayCode);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("写入托架号失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        /// <summary>
        /// 检索托架位置
        /// </summary>
        /// <returns></returns>
        public DataResult StartM500()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M500", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("检索托架位置:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M500", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("检索托架位置:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 驱动升降机运行
        /// </summary>
        /// <returns></returns>
        public DataResult StartM501()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M501", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("驱动升降机运行:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M501", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("驱动升降机运行:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        #endregion

        #region 手动水平运行
        /// <summary>
        /// 在M410为ON时 将M410置为true 表示钩子向前运行到上部
        /// </summary>
        /// <returns></returns>
        public DataResult SetM410True()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M410", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("钩子向前运行到上部:" + onLineResult.Message);
                    }
                    return DataProcess.Success();

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 在M420为ON时 将M420置为true 表示钩子向前运行到下部
        /// </summary>
        /// <returns></returns>
        public DataResult SetM420True()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M420", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("钩子向前运行到下部:" + onLineResult.Message);
                    }
                    return DataProcess.Success();

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        /// <summary>
        /// 在M460为ON时 将M460置为true 表示钩子向后运行到下部
        /// </summary>
        /// <returns></returns>
        public DataResult SetM460True()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M460", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("钩子向后运行到下部:" + onLineResult.Message);
                    }
                    return DataProcess.Success();

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 在M430为ON时 将M430置为true 表示钩子向后运行到上部
        /// </summary>
        /// <returns></returns>
        public DataResult SetM430True()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M430", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("钩子向后运行到上部:" + onLineResult.Message);
                    }
                    return DataProcess.Success();

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 在M450为ON时 将M450置为true 表示钩子向后运行到下部
        /// </summary>
        /// <returns></returns>
        public DataResult SetM450True()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M450", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("钩子向后运行到下部:" + onLineResult.Message);
                    }
                    return DataProcess.Success();

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 在M470为ON时 将M470置为true 表示钩子向前运行到下部
        /// <returns></returns>
        public DataResult SetM470True()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M470", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("钩子向前运行到下部:" + onLineResult.Message);
                    }
                    return DataProcess.Success();

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        #endregion

        #region 托盘扫描逻辑变更
        /// <summary>
        /// 开启托盘扫描
        /// </summary>
        /// <returns></returns>
        public DataResult StartM350()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M350", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动托盘扫描时报:" + onLineResult.Message);
                    }
                    //System.Threading.Thread.Sleep(200);
                    //onLineResult = melsec_net.Write("M350", false);
                    //if (!onLineResult.IsSuccess)
                    //{
                    //    return DataProcess.Failure("启动托盘扫描时报:" + onLineResult.Message);
                    //}
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        /// <summary>
        /// 设置M390为ON 表示进入托盘自定义状态
        /// </summary>
        /// <returns></returns>
        public DataResult SetM390True()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M390", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("进入托盘自定义状态报:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 开启检索未定义托盘
        /// </summary>
        /// <returns></returns>
        public DataResult StartM391()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M391", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动开始定义失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M391", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("启动开始定义失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        /// <summary>
        /// 在M390为ON时 将M370置为true 表示取出未定义托盘
        /// </summary>
        /// <returns></returns>
        public DataResult SetM370True()
        {
            try
            {
                if (isConnected)
                {
                    var result = GetM390();
                    if (result.Success && (bool)result.Data==true)
                    {
                        var onLineResult = melsec_net.Write("M370", true);
                        if (!onLineResult.IsSuccess)
                        {
                            return DataProcess.Failure("取出未定义托盘:" + onLineResult.Message);
                        }
                        return DataProcess.Success();
                    }
                    else
                    {
                        return DataProcess.Failure("M390状态为OFF");
                    }
                 
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 获取托盘扫描状态
        /// </summary>
        /// <returns></returns>
        public DataResult GetM390()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M390");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取监视状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <returns></returns>
        public DataResult WriteD392(Command.RunningContainer runningContainer)
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("D392", runningContainer.TrayCode);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("写入托盘号失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 在M390为ON时 将M371置为true 表示存入自定义托盘
        /// </summary>
        /// <returns></returns>
        public DataResult SetM371True()
        {
            try
            {
                if (isConnected)
                {
                    var result = GetM390();
                    if (result.Success && (bool)result.Data == true)
                    {
                        var onLineResult = melsec_net.Write("M371", true);
                        if (!onLineResult.IsSuccess)
                        {
                            return DataProcess.Failure("取出未定义托盘:" + onLineResult.Message);
                        }
                        return DataProcess.Success();
                    }
                    else
                    {
                        return DataProcess.Failure("M390状态为OFF");
                    }

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }



        /// <summary>
        /// 确认完毕后 检索下一个未定义托盘
        /// </summary>
        /// <returns></returns>
        public DataResult ConfirmM394()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M394", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("确认失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M394", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("确认失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        /// 获取下一个 状态 OFF表示还设有未定义托盘 ON全部定义完成
        /// </summary>
        /// <returns></returns>
        public DataResult GetM395()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.ReadBool("M395");
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("读取监视状态失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success(onLineResult.Content);
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }

        /// <summary>
        ///结束托盘扫描
        /// </summary>
        /// <returns></returns>
        public DataResult ConfirmM396()
        {
            try
            {
                if (isConnected)
                {
                    //01 PLC 接收远程控制指令——在线
                    //00 PLC 不接受远程控制指令——离线
                    var onLineResult = melsec_net.Write("M396", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("结束托盘扫描失败:" + onLineResult.Message);
                    }
                    System.Threading.Thread.Sleep(200);
                    onLineResult = melsec_net.Write("M396", false);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("结束托盘扫描失败:" + onLineResult.Message);
                    }
                    return DataProcess.Success();
                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        #endregion

        #region 自动门手动操作
        /// <summary>
        /// 手动开门
        /// </summary>
        /// <returns></returns>
        public DataResult SetM9True()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M9", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("手动开门:" + onLineResult.Message);
                    }
                    else
                    {
                        return DataProcess.Success();
                    }

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }


        /// <summary>
        /// 手动关门
        /// </summary>
        /// <returns></returns>
        public DataResult SetM10True()
        {
            try
            {
                if (isConnected)
                {
                    var onLineResult = melsec_net.Write("M10", true);
                    if (!onLineResult.IsSuccess)
                    {
                        return DataProcess.Failure("手动关门:" + onLineResult.Message);
                    }
                    else
                    {
                        return DataProcess.Success();
                    }

                }
                else
                {
                    return DataProcess.Failure("PLC未连接");
                }
            }
            catch (Exception ex)
            {

                return DataProcess.Failure(ex.Message);
            }
        }
        #endregion
        #endregion

    }
}