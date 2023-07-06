using Bussiness.Dtos;

namespace wms.Client.Model.RequestModel
{

    /// <summary>
    /// 设备通讯状态
    /// </summary>
    public class GetPlcDeivceStatusRequest : BaseRequest
    {
        public override string route { get => DeviceIP + "GetPlcDeivceStatus"; }
    }
    /// <summary>
    /// 设备报警信息
    /// </summary>
    public class GetAlarmInformationRequest : BaseRequest
    {
        public override string route { get => DeviceIP + "GetAlarmInformation"; }
    }

    /// <summary>
    /// 设备报警信息
    /// </summary>
    public class ResetAlarmRequest : BaseRequest
    {
        public override string route { get => DeviceIP + "ResetAlarm"; }
    }

    /// <summary>
    /// 扫描按钮
    /// </summary>
    public class StartScanBarcodeKeyDownRequest : BaseRequest
    {
        public override string route { get => DeviceIP + "StartScanBarcodeKeyDown"; }
    }
    /// <summary>
    /// 返回当前称重物料数量
    /// </summary>
    public class GetBackWeighingQuantityRequest : BaseRequest
    {
        public override string route { get => DeviceIP + "GetBackWeighingQuantity"; }
    }
    /// <summary>
    /// 返回M684状态
    /// </summary>
    public class GetBackDoubleTrayMethodRequest : BaseRequest
    {
        public override string route { get => DeviceIP + "GetBackDoubleTrayMethod"; }


        /// <summary>
        /// 再次控制货柜运转
        /// </summary>
        public class StartRunningContainerAgainRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "StartRunningContainerAgain"; }
        }

        /// <summary>
        /// 双托盘再次控制货柜运转
        /// </summary>
        public class DoubleTrayStartRunningContainerAgainRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "DoubleTrayStartRunningContainerAgain"; }
        }

        /// <summary>
        /// 双托盘控制货柜运转
        /// </summary>
        public class DoubleTrayStartRunningContainerRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "DoubleTrayStartRunningContainer"; }
        }

        /// <summary>
        /// 控制货柜运转
        /// </summary>
        public class StartRunningContainerRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "StartRunningContainer"; }
        }
        public class StartRestoreContainerRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "StartRestoreContainer"; }
        }

        public class OffXLightRequest : BaseRequest
        {
            public override string route { get => DeviceIP + " OffXLight"; }
        }

        /// <summary>
        /// 料斗行程设定
        /// </summary>
        public class HopperSettingRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "HopperSetting"; }
        }

        /// <summary>
        /// 安全门行程设定
        /// </summary>
        public class EmergencyDoorSettingRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "EmergencyDoorSetting"; }
        }


        #region 垂直学习
        /// <summary>
        /// 开启垂直学习
        /// </summary>
        public class StartM300Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM300"; }
        }
        /// <summary>
        /// 监视学习状态M340  true时学习结束
        /// </summary>
        public class GetM340Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM340"; }
        }
        /// <summary>
        /// 结束垂直学习
        /// </summary>
        public class FinishM341Request : BaseRequest
        {
            public override string route { get => DeviceIP + "FinishM341"; }
        }

        #endregion

        #region 水平学习
        /// <summary>
        /// 开启水平学习
        /// </summary>
        public class StartM400Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM400"; }
        }
        /// <summary>
        /// 监视学习状态M440  true时学习结束
        /// </summary>
        public class GetM440Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM440"; }
        }
        /// <summary>
        /// 结束水平学习
        /// </summary>
        public class FinishM441Request : BaseRequest
        {
            public override string route { get => DeviceIP + "FinishM441"; }
        }
        #endregion

        #region 自动门学习
        /// <summary>
        /// 开启自动门学习
        /// </summary>
        public class StartM450Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM450"; }
        }
        /// <summary>
        /// 监视学习状态M490 true时学习结束
        /// </summary>
        public class GetM490Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM490"; }
        }
        /// <summary>
        /// 结束自动门学习
        /// </summary>
        public class FinishM491Request : BaseRequest
        {
            public override string route { get => DeviceIP + "FinishM491"; }
        }

        /// <summary>
        /// 结束自动门学习
        /// </summary>
        public class SetM8TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM8True"; }
        }
        /// <summary>
        /// 结束自动门学习
        /// </summary>
        public class SetM9TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM9True"; }
        }
        /// <summary>
        /// 结束自动门学习
        /// </summary>
        public class SetM10TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM10True"; }
        }


        #endregion

        #region 托盘扫描
        /// <summary>
        /// 开启托盘扫描
        /// </summary>
        public class StartM350Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM350"; }
        }
        /// <summary>
        ///  设置M390为ON 表示进入托盘自定义状态
        /// </summary>
        public class SetM390TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM390True"; }
        }
        /// <summary>
        /// 开启检索未定义托盘
        /// </summary>
        public class StartM391Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM391"; }
        }
        /// <summary>
        /// 在M390为ON时 将M370置为true 表示取出未定义托盘
        /// </summary>
        public class SetM370TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM370True"; }
        }
        /// <summary>
        /// 获取托盘扫描状态
        /// </summary>
        public class GetM390Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM390"; }
        }
        /// <summary>
        /// 写入托盘号
        /// </summary>
        public class WriteD392Request : BaseRequest
        {
            public override string route { get => DeviceIP + "WriteD392"; }
        }
        /// <summary>
        /// 在M390为ON时 将M371置为true 表示存入自定义托盘
        /// </summary>
        public class SetM371TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM371True"; }
        }
        /// <summary>
        /// 确认完毕后 检索下一个未定义托盘
        /// </summary>
        public class ConfirmM394Request : BaseRequest
        {
            public override string route { get => DeviceIP + "ConfirmM394"; }
        }
        /// <summary>
        /// 获取下一个 状态 OFF表示还设有未定义托盘 ON全部定义完成
        /// </summary>
        public class GetM395Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM395"; }
        }
        /// <summary>
        /// 结束托盘扫描
        /// </summary>
        public class ConfirmM396Request : BaseRequest
        {
            public override string route { get => DeviceIP + "ConfirmM396"; }
        }
        #endregion

        #region 自动存取托盘
        /// <summary>
        /// 写入托盘号
        /// </summary>
        public class WriteD650Request : BaseRequest
        {
            public override string route { get => DeviceIP + "WriteD650"; }
        }
        /// <summary>
        /// 获取M650状态
        /// </summary>
        public class GetM650Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM650"; }
        }
        public class StartM650Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM650"; }
        }
        public class WriteD650_InRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "WriteD650_In"; }
        }
        public class SetM651TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM651True"; }
        }
        public class SetM654TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM654True"; }
        }

        public class GetM654TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM654"; }
        }
        /// <summary>
        /// 获取D651 托盘所在托架号
        /// </summary>
        public class GetD651Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetD651"; }
        }
        /// <summary>
        /// 启动M651
        /// </summary>
        public class StartM651Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM651"; }
        }
        /// <summary>
        /// 获取物料高度
        /// </summary>
        public class GetD652Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetD652"; }
        }
        #endregion

        #region 添加托盘
        /// <summary>
        /// 写入托盘号
        /// </summary>
        public class WriteD700Request : BaseRequest
        {
            public override string route { get => DeviceIP + "WriteD700"; }
        }
        /// <summary>
        /// 启动M700
        /// </summary>
        public class StartM700Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM700"; }
        }
        /// <summary>
        /// 获取M701状态 true 空间足够 
        /// </summary>
        public class GetM701Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM701"; }
        }
        /// <summary>
        /// 确认存入
        /// </summary>
        public class ConfirmM702Request : BaseRequest
        {
            public override string route { get => DeviceIP + "ConfirmM702"; }
        }
        #endregion

        #region 删除托盘
        /// <summary>
        /// 写入需要删除的托盘号
        /// </summary>
        public class WriteD750Request : BaseRequest
        {
            public override string route { get => DeviceIP + "WriteD750"; }
        }
        /// <summary>
        /// 启动M750
        /// </summary>
        public class StartM750Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM750"; }
        }
        /// <summary>
        /// 获取托盘所在货架号
        /// </summary>
        public class GetD751Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetD751"; }
        }
        /// <summary>
        /// 启动M751
        /// </summary>
        public class StartM751Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM751"; }
        }
        /// <summary>
        /// 监视托盘
        /// </summary>
        public class GetM752Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM752"; }
        }
        /// <summary>
        /// 确认删除
        /// </summary>
        public class ConfirmM753Request : BaseRequest
        {
            public override string route { get => DeviceIP + "ConfirmM753"; }
        }
        /// <summary>
        /// 取消删除
        /// </summary>
        public class ConfirmM754Request : BaseRequest
        {
            public override string route { get => DeviceIP + "ConfirmM754"; }
        }
        #endregion

        #region 整理存储空间
        /// <summary>
        /// 开始整理
        /// </summary>
        public class StartM800Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM800"; }
        }
        public class StartM810Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM810"; }
        }
        /// <summary>
        /// 监视托盘
        /// </summary>
        public class GetM801Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetM801"; }
        }
        /// <summary>
        /// 获取空间利用率
        /// </summary>
        public class GetD800Request : BaseRequest
        {
            public override string route { get => DeviceIP + "GetD800"; }
        }
        /// <summary>
        /// 确认整理完毕
        /// </summary>
        public class ConfirmM802Request : BaseRequest
        {
            public override string route { get => DeviceIP + "ConfirmM802"; }
        }
        #endregion

        #region 手动垂直运行
        /// <summary>
        /// 写入托架号
        /// </summary>
        public class WriteD500Request : BaseRequest
        {
            public override string route { get => DeviceIP + "WriteD500"; }
        }
        /// <summary>
        /// 检索托架位置
        /// </summary>
        public class StartM500Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM500"; }
        }
        /// <summary>
        /// 驱动升降机运行
        /// </summary>
        public class StartM501Request : BaseRequest
        {
            public override string route { get => DeviceIP + "StartM501"; }
        }
        #endregion

        #region 手动水平运行
        /// <summary>
        /// 在M410为ON时 将M410置为true 表示钩子向前运行到上部
        /// </summary>
        public class SetM410TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM410True"; }
        }
        /// <summary>
        /// 在M420为ON时 将M420置为true 表示钩子向前运行到下部
        /// </summary>
        public class SetM420TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM420True"; }
        }
        /// <summary>
        /// 在M460为ON时 将M460置为true 表示钩子向后运行到下部
        /// </summary>
        public class SetM460TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM460True"; }
        }
        /// <summary>
        /// 在M430为ON时 将M430置为true 表示钩子向后运行到上部
        /// </summary>
        public class SetM430TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM430True"; }
        }
        /// <summary>
        /// 在M450为ON时 将M450置为true 表示钩子向后运行到下部
        /// </summary>
        public class SetM450TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM450True"; }
        }
        /// <summary>
        /// 在M470为ON时 将M470置为true 表示钩子向前运行到下部
        /// </summary>
        public class SetM470TrueRequest : BaseRequest
        {
            public override string route { get => DeviceIP + "SetM470True"; }
        }
        #endregion
    }
}
