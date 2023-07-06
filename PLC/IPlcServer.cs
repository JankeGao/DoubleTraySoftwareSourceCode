
using HP.Utility.Data;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PLCServer
{
    //XmlSerializerFormat(Style = OperationFormatStyle.Document)
    [ServiceContract]
    public interface IPlcServer
    {
        /// <summary>
        /// 扫描传递物料单重
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartScanBarcodeKeyDown", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartScanBarcodeKeyDown(Command.RunningContainer runningContainer);

        /// <summary>
        /// 返回当前称重物料数量
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetBackWeighingQuantity", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetBackWeighingQuantity();

        /// <summary>
        /// 返回M684状态
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetBackDoubleTrayMethod", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetBackDoubleTrayMethod();

        /// <summary>
        /// 货柜自动运行
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartRunningContainer", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartRunningContainer(Command.RunningContainer runningContainer);

        /// <summary>
        /// 货柜再次自动运行
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartRunningContainerAgain", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartRunningContainerAgain(Command.RunningContainer runningContainer);

        /// <summary>
        /// 双托盘货柜自动运行
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "DoubleTrayStartRunningContainer", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult DoubleTrayStartRunningContainer(Command.RunningContainer runningContainer);

        /// <summary>
        /// 双托盘货柜再次自动运行
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "DoubleTrayStartRunningContainerAgain", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult DoubleTrayStartRunningContainerAgain(Command.RunningContainer runningContainer);

        /// <summary>
        /// 货柜存入托盘
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartRestoreContainer", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartRestoreContainer(Command.RunningContainer runningContainer);
        /// <summary>
        /// 熄灭X轴灯号
        /// </summary>
        /// <param name="runningContainer"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "OffXLight", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult OffXLight(Command.RunningContainer runningContainer);

        /// <summary>
        /// 路径行程设定
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "HopperSetting", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult HopperSetting();
        /// <summary>
        /// 安全门行程设定
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "EmergencyDoorSetting", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult EmergencyDoorSetting();

        /// <summary>
        /// 获取PLC通讯状态
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetPlcDeivceStatus", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetPlcDeivceStatus(Command.RunningContainer runningContainer);
        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetAlarmInformation", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetAlarmInformation();
        /// <summary>
        /// 复位报警
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ResetAlarm", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult ResetAlarm();


        #region 卡迪斯
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartRunningC3000Container", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartRunningC3000Container(Command.RunningContainer runningContainer);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetC3000DeivceStatus", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetC3000DeivceStatus(Command.RunningContainer runningContainer);
        #endregion

        #region 亨乃尔
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartRunningHanelContainer", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartRunningHanelContainer(Command.RunningContainer runningContainer);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetHanelStatus", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetHanelStatus(Command.RunningContainer runningContainer);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "FinishHanellContainer", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult FinishHanellContainer(Command.RunningContainer runningContainer);
        #endregion

        #region 朗杰升降柜
        #region 垂直学习
        /// <summary>
        /// 开启垂直学习
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM300", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM300();

        /// <summary>
        /// 监视学习状态M340  true时学习结束
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetM340", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetM340();

        /// <summary>
        /// 读取前部托盘数
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetD300", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetD300();

        /// <summary>
        /// 读取后部托盘数
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetD301", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetD301();
        /// <summary>
        /// 结束垂直学习
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "FinishM341", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult FinishM341();
        #endregion

        #region 水平学习
        /// <summary>
        /// 开启水平学习
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM400", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM400();

        /// <summary>
        /// 监视学习状态M440  true时学习结束
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetM440", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetM440();
        /// <summary>
        /// 结束水平学习
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "FinishM441", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult FinishM441();
        #endregion

        #region 自动门学习

        /// <summary>
        /// 开启自动门学习
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM450", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM450();
        /// <summary>
        /// 监视自动门学习状态  true时学习结束
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetM490", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetM490();
        /// <summary>
        /// 结束自动水平学习
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "FinishM491", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult FinishM491();
        #endregion

        #region 托盘扫描
        /// <summary>
        /// 开启托盘扫描
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM350", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM350();


        /// <summary>
        /// 设置M390为ON 表示进入托盘自定义状态
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM390True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM390True();
        /// <summary>
        /// 开启托盘扫描  开始定义
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM391", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM391();

        /// <summary>
        /// 获取托盘扫描状态
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetM390", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetM390();

        /// <summary>
        /// 在M390为ON时 将M370置为true 表示取出未定义托盘
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM370True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM370True();


        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "WriteD392", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult WriteD392(Command.RunningContainer runningContainer);


        /// <summary>
        /// 在M390为ON时 将M371置为true 表示存入自定义托盘
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM371True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM371True();

        /// <summary>
        /// 确认完毕后 下一个
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ConfirmM394", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult ConfirmM394();
        /// <summary>
        /// 获取下一个 状态 OFF表示还设有未定义托盘 ON全部定义完成
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetM395", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetM395();
        /// <summary>
        /// 结束托盘扫描
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ConfirmM396", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult ConfirmM396();

        ///// <summary>
        ///// 读取托盘扫描前部托盘数
        ///// </summary>
        ///// <returns></returns>
        //[OperationContract]
        //[WebInvoke(Method = "GET", UriTemplate = "GetD390", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //DataResult GetD390();
        ///// <summary>
        ///// 读取托盘扫描后部托盘数
        ///// </summary>
        ///// <returns></returns>
        //[OperationContract]
        //[WebInvoke(Method = "GET", UriTemplate = "GetD391", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //DataResult GetD391();
  
        ///// <summary>
        ///// 获取开始定义 后状态
        ///// </summary>
        ///// <returns></returns>
        //[OperationContract]
        //[WebInvoke(Method = "GET", UriTemplate = "GetM392", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //DataResult GetM392();
  
        ///// <summary>
        ///// 确认 写入托盘号完毕
        ///// </summary>
        ///// <returns></returns>
        //[OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "ConfirmM393", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //DataResult ConfirmM393();


        #endregion

        #region 自动存取托盘
        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "WriteD650", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult WriteD650(Command.RunningContainer runningContainer);

        /// <summary>
        /// 开始取出
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM650", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM650();
        /// <summary>
        /// 获取M650状态
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetM650", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetM650();
        /// <summary>
        /// 获取D651 托盘所在托架号
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetD651", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetD651();
        /// <summary>
        /// 启动M651
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM651", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM651();
        /// <summary>
        /// 获取物料高度
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetD652", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetD652();

        /// <summary>
        /// 存入托盘
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "WriteD650_In", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult WriteD650_In(Command.RunningContainer runningContainer);
        /// <summary>
        /// 取出托盘
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM651True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM651True();

        /// <summary>
        /// 存入托盘
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM654True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM654True();
        #endregion

        #region 添加托盘
        /// <summary>
        /// 添加托盘
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "WriteD700", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult WriteD700(Command.RunningContainer runningContainer);
        /// <summary>
        /// 启动M700
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM700", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM700();
        /// <summary>
        ///  获取M701状态 true 空间足够 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetM701", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetM701();
        /// <summary>
        /// 确认存入
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ConfirmM702", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult ConfirmM702();
        #endregion

        #region 删除托盘
        /// <summary>
        /// 写入需要删除的托盘
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "WriteD750", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult WriteD750(Command.RunningContainer runningContainer);
        /// <summary>
        /// 启动M750
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM750", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM750();
        /// <summary>
        /// 获取托盘所在货架号
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetD751", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetD751();
        /// <summary>
        /// 启动M751
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM751", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM751();
        /// <summary>
        /// 监视托盘
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetM752", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetM752();
        /// <summary>
        /// 确认删除
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ConfirmM753", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult ConfirmM753();

        /// <summary>
        /// 取消删除
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ConfirmM754", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult ConfirmM754();
        #endregion

        #region 整理存储空间
        /// <summary>
        /// 开始整理
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM800", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM800();
        /// <summary>
        /// 监视整理是否完成
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetM801", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetM801();
        /// <summary>
        /// 获取空间利用率
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetD800", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult GetD800();
        /// <summary>
        /// 确认整理完毕
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ConfirmM802", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult ConfirmM802();
        #endregion

        #region 手动垂直运行
        /// <summary>
        /// 写入托架号
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "WriteD500", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult WriteD500(Command.RunningContainer runningContainer);

        /// <summary>
        /// 检索托架位置
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM500", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM500();

        /// <summary>
        /// 驱动升降机运行
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StartM501", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult StartM501();
        #endregion

        #region 手动垂直运行

        /// <summary>
        ///  在M410为ON时 将M410置为true 表示钩子向前运行到上部
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM410True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM410True();

        /// <summary>
        /// 在M420为ON时 将M420置为true 表示钩子向前运行到下部
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM420True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM420True();

        /// <summary>
        ///  在M460为ON时 将M460置为true 表示钩子向后运行到下部
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM460True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM460True();

        /// <summary>
        ///  在M430为ON时 将M430置为true 表示钩子向后运行到上部
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM430True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM430True();

        /// <summary>
        /// 在M450为ON时 将M450置为true 表示钩子向后运行到下部
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM450True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM450True();

        /// <summary>
        /// 在M470为ON时 将M470置为true 表示钩子向前运行到下部
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM470True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM470True();
        #endregion

        #region 手动开关自动门
        /// <summary>
        /// 手动开门
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM9True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM9True();
        /// <summary>
        ///  手动关门
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM10True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM10True();

        /// <summary>
        /// 自动门学习
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetM8True", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataResult SetM8True();
        #endregion
        #endregion

    }


    //public interface IPlcServer
    //{
    //    /// <summary>
    //    /// 货柜自动运行
    //    /// </summary>
    //    /// <param name="runningContainer"></param>
    //    /// <returns></returns>
    //    [OperationContract]
    //    DataResult StartRunningContainer(Command.RunningContainer runningContainer);
    //    /// <summary>
    //    /// 路径行程设定
    //    /// </summary>
    //    /// <returns></returns>
    //    [OperationContract]
    //    DataResult HopperSetting();
    //    /// <summary>
    //    /// 安全门行程设定
    //    /// </summary>
    //    /// <returns></returns>
    //    [OperationContract]
    //    DataResult EmergencyDoorSetting();

    //    /// <summary>
    //    /// 获取PLC通讯状态
    //    /// </summary>
    //    /// <returns></returns>
    //    [OperationContract]
    //    DataResult GetPlcDeivceStatus();
    //    /// <summary>
    //    /// 获取报警信息
    //    /// </summary>
    //    /// <returns></returns>
    //    [OperationContract]
    //    DataResult GetAlarmInformation();
    //    /// <summary>
    //    /// 复位报警
    //    /// </summary>
    //    /// <returns></returns>
    //    [OperationContract]

    //    DataResult ResetAlarm();


    //    #region 卡迪斯
    //    [OperationContract]
    //    DataResult StartRunningC3000Container(Command.RunningContainer runningContainer);

    //    [OperationContract]
    //    DataResult GetC3000DeivceStatus(Command.RunningContainer runningContainer);
    //    #endregion

    //    #region 亨乃尔
    //    [OperationContract]
    //    DataResult StartRunningHanelContainer(Command.RunningContainer runningContainer);


    //    [OperationContract]
    //    DataResult GetHanelStatus(Command.RunningContainer runningContainer);


    //    [OperationContract]
    //    DataResult FinishHanellContainer(Command.RunningContainer runningContainer);
    //    #endregion

    //    #region 
    //    #endregion

    //}
}
