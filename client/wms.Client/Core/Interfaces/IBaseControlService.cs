using System.Threading.Tasks;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Utility.Data;
using wms.Client.Model.Entity;
using RunningContainer = wms.Client.Model.Entity.RunningContainer;

namespace wms.Client.Core.Interfaces
{
    public interface IBaseControlService
    {
        /// <summary>
        /// 获取设备状态接口
        /// </summary>
        /// <returns></returns>
       Task<DataResult> GetPlcDeivceStatus(RunningContainer model);

        /// <summary>
        /// 读取设备报警信息
        /// </summary>
        /// <returns></returns>
       Task<DataResult> GetAlarmInformation();

        /// <summary>
        /// 复位全部报警
        /// </summary>
        /// <returns></returns>
       Task<DataResult> PostRestAllAlarm();
        /// <summary>
        /// 点击扫描按钮
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DataResult> PostStartScanBarcodeKeyDown(RunningContainer model);

        /// <summary>
        /// 返回当前称重物料数量
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetBackWeighingQuantity();

        /// <summary>
        /// 返回M684的状态
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetBackDoubleTrayMethod();

        /// <summary>
        /// 控制货柜运转(取出货柜)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DataResult> PostStartRunningContainer(RunningContainer model);

        /// <summary>
        /// 再次控制货柜运转(取出货柜)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DataResult> PostStartRunningContainerAgain(RunningContainer model);

        /// <summary>
        /// 双托盘再次控制货柜运转(取出货柜)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DataResult> DoubleTrayPostStartRunningContainerAgain(RunningContainer model);

        /// <summary>
        /// 双托盘控制货柜运转(取出货柜)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DataResult> DoubleTrayPostStartRunningContainer(RunningContainer model);

        /// <summary>
        /// 存入货柜
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DataResult> PostStartRestoreContainer(RunningContainer model);
        /// <summary>
        /// 熄灭X轴灯号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DataResult> OffXLight(RunningContainer model);

        /// <summary>
        /// 安全门行程设定
        /// </summary>
        /// <returns></returns>
       Task<DataResult> GetEmergencyDoorSetting();

        /// <summary>
        /// 料斗行程设定
        /// </summary>
        /// <returns></returns>
       Task<DataResult> GetHopperSetting();

        #region 朗杰升降柜
        #region 垂直学习
        /// <summary>
        /// 开启垂直学习
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM300();

        /// <summary>
        /// 监视学习状态M340  true时学习结束
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetM340();
        /// <summary>
        /// 结束垂直学习
        /// </summary>
        /// <returns></returns>
        Task<DataResult> FinishM341();
        #endregion

        #region 水平学习
        /// <summary>
        /// 开启水平学习
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM400();

        /// <summary>
        /// 监视学习状态M440  true时学习结束
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetM440();
        /// <summary>
        /// 结束水平学习
        /// </summary>
        /// <returns></returns>
        Task<DataResult> FinishM441();
        #endregion

        #region 自动门学习

        /// <summary>
        /// 开启自动门学习
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM450();
        /// <summary>
        /// 监视自动门学习状态  true时学习结束
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetM490();
        /// <summary>
        /// 结束自动水平学习
        /// </summary>
        /// <returns></returns>
        Task<DataResult> FinishM491();

        /// <summary>
        /// 自动门学习
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM8True();

        /// <summary>
        /// 手动开启自动门
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM9True();

        /// <summary>
        /// 手动关闭自动门
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM10True();
        #endregion

        #region 托盘扫描
        /// <summary>
        /// 开启托盘扫描
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM350();


        /// <summary>
        /// 设置M390为ON 表示进入托盘自定义状态
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM390True();
        /// <summary>
        /// 开启托盘扫描  开始定义
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM391();

        /// <summary>
        /// 获取托盘扫描状态
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetM390();

        /// <summary>
        /// 在M390为ON时 将M370置为true 表示取出未定义托盘
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM370True();


        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <returns></returns>
        Task<DataResult> WriteD392(RunningContainer runningContainer);


        /// <summary>
        /// 在M390为ON时 将M371置为true 表示存入自定义托盘
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM371True();

        /// <summary>
        /// 确认完毕后 下一个
        /// </summary>
        /// <returns></returns>
        Task<DataResult> ConfirmM394();
        /// <summary>
        /// 获取下一个 状态 OFF表示还设有未定义托盘 ON全部定义完成
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetM395();
        /// <summary>
        /// 结束托盘扫描
        /// </summary>
        /// <returns></returns>
        Task<DataResult> ConfirmM396();


        #endregion

        #region 自动存取托盘
        /// <summary>
        /// 写入托盘号
        /// </summary>
        /// <returns></returns>
        Task<DataResult> WriteD650(RunningContainer runningContainer);

        /// <summary>
        /// 开始取出
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM650();
        /// <summary>
        /// 获取M650状态
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetM650();
        /// <summary>
        /// 获取D651 托盘所在托架号
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetD651();
        /// <summary>
        /// 启动M651
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM651();
        /// <summary>
        /// 获取物料高度
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetD652();
        /// <summary>
        /// 存入写进托盘号
        /// </summary>
        /// <returns></returns>
        Task<DataResult> WriteD650_In(Model.Entity.RunningContainer runningContainer);
        /// <summary>
        /// 开始取出
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM651True();

        /// <summary>
        /// 开始存入
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM654True();

        Task<DataResult> GetM654();
        #endregion

        #region 添加托盘
        /// <summary>
        /// 添加托盘
        /// </summary>
        /// <returns></returns>
        Task<DataResult> WriteD700(RunningContainer runningContainer);
        /// <summary>
        /// 启动M700
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM700();
        /// <summary>
        ///  获取M701状态 true 空间足够 
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetM701();
        /// <summary>
        /// 确认存入
        /// </summary>
        /// <returns></returns>
        Task<DataResult> ConfirmM702();
        #endregion

        #region 删除托盘
        /// <summary>
        /// 写入需要删除的托盘
        /// </summary>
        /// <returns></returns>
        Task<DataResult> WriteD750(RunningContainer runningContainer);
        /// <summary>
        /// 启动M750
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM750();
        /// <summary>
        /// 获取托盘所在货架号
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetD751();
        /// <summary>
        /// 启动M751
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM751();
        /// <summary>
        /// 监视托盘
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetM752();
        /// <summary>
        /// 确认删除
        /// </summary>
        /// <returns></returns>
        Task<DataResult> ConfirmM753();

        /// <summary>
        /// 取消删除
        /// </summary>
        /// <returns></returns>
        Task<DataResult> ConfirmM754();
        #endregion

        #region 整理存储空间
        Task<DataResult> StartM810();
        /// <summary>
        /// 开始整理
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM800();
        /// <summary>
        /// 监视整理是否完成
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetM801();
        /// <summary>
        /// 获取空间利用率
        /// </summary>
        /// <returns></returns>
        Task<DataResult> GetD800();
        /// <summary>
        /// 确认整理完毕
        /// </summary>
        /// <returns></returns>
        Task<DataResult> ConfirmM802();
        #endregion

        #region 手动垂直运行
        /// <summary>
        /// 写入托架号
        /// </summary>
        /// <returns></returns>
        Task<DataResult> WriteD500(RunningContainer runningContainer);

        /// <summary>
        /// 检索托架位置
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM500();

        /// <summary>
        /// 驱动升降机运行
        /// </summary>
        /// <returns></returns>
        Task<DataResult> StartM501();
        #endregion

        #region 手动垂直运行

        /// <summary>
        ///  在M410为ON时 将M410置为true 表示钩子向前运行到上部
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM410True();

        /// <summary>
        /// 在M420为ON时 将M420置为true 表示钩子向前运行到下部
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM420True();

        /// <summary>
        ///  在M460为ON时 将M460置为true 表示钩子向后运行到下部
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM460True();

        /// <summary>
        ///  在M430为ON时 将M430置为true 表示钩子向后运行到上部
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM430True();

        /// <summary>
        /// 在M450为ON时 将M450置为true 表示钩子向后运行到下部
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM450True();

        /// <summary>
        /// 在M470为ON时 将M470置为true 表示钩子向前运行到下部
        /// </summary>
        /// <returns></returns>
        Task<DataResult> SetM470True();
        #endregion
        #endregion
    }
}
