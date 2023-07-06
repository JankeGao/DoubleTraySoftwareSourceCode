using System.ComponentModel;

namespace Bussiness.Enums
{
    public enum DeviceAlarmEnum
    {


        [Description("紧急停止")]
        Urgencye = 200,

        [Description("冲顶保护")]
        Reach = 201,

        [Description("托底保护")]
        Fault = 202,

        [Description("维修门开启")]
        Maintain = 203,

        [Description("电机1过热")]
        DiaphragmKeepOut = 204,

        [Description("电机2过热")]
        ElectricalMachineryOverheating = 205,

        [Description("伺服驱动器故障")]
        UuivertorOverload = 206,

        [Description("变频器故障")]
        UuivertorOverheating = 207,

        [Description("安全光栅遮拦")]
        InputDefaultPhase = 208,

        [Description("物料高度超限")]
        OutputDefaultPhase = 209,

        [Description("垂直定位偏差超限")]
        UuivertorCPU = 210,

        [Description("水平定位偏差超限")]
        UuivertorPowerFailure = 211,

        [Description("取料口托盘位置不正确")]
        AlarmHappen = 212,

        [Description("勾爪位置不正确")]
        AlarmReset = 213,

        [Description("托架被占用")]
        StartingUpReset = 214,

        [Description("货柜空间不足")]
        Affirm = 241,
        [Description("物料遮挡高度光栅")]
        MaterialHigher = 216,
        [Description("物料超重")]
        MaterialWeighter = 217,
        [Description("安全门超限")]
        SafeDoor = 218,
        [Description("安全回路未激活")]
        SafeNotLive = 219,
        [Description("托盘号不存在")]
        TrayNotFound = 220,
        [Description("托架号不存在")]
        TrayShelfNotFound = 221,
        [Description("托盘号重复")]
        TrayAreadyExist = 222,
        [Description("托盘取出-初始状态，传感器信号错误，" + "\r\n" + 
            "请检查取料口托盘感应器是否输出正常")]
        MresultA = 1200,
        [Description("托盘取出-初始状态，感应器信号错误，" + "\r\n" +
            "请检查升降机托盘感应器是否输出正常")]
        MresultB = 1201,
        [Description("托盘取出-初始状态，感应器信号错误，" + "\r\n" +
            "请检查升降机钩爪原点感应器是否输出正常")]
        MresultC = 1202,
        [Description("托盘取出-关闭安全门，感应器信号错误，" + "\r\n" +
            "请检查安全门原点感应器是否输出正常")]
        MresultD = 1203,
        [Description("托盘取出-垂直运行，感应器信号错误，" + "\r\n" +
            "请检查升降机托盘感应器是否输出正常")]
        MresultE = 1204,
        [Description("托盘取出-垂直运行，感应器信号错误，" + "\r\n" +
            "请检查升降机钩爪原点感应器是否输出正常")]
        MresultF = 1205,
        [Description("托盘取出-垂直运行，感应器信号错误，" + "\r\n" +
            "请检查升降机托架感应器是否输出正常")]
        MresultG = 1206,
        [Description("托盘取出-垂直运行，标志位信号错误，" + "\r\n" +
            "请检查M506是否输出正常")]
        MresultH = 1207,
        [Description("托盘取出-钩取托盘，传感器信号错误，" + "\r\n" +
            "请检查升降机托盘感应器是否输出正常")]
        MresultI = 1208,
        [Description("托盘取出-钩取托盘，传感器信号错误，" + "\r\n" +
            "请检查升降机钩爪到位感应器是否输出正常")]
        MresultJ = 1209,
        [Description("托盘取出-垂直运行取料口，标志位信号错误，" + "\r\n" +
            "请检查M506是否输出正常")]
        MresultK = 1210,
        [Description("托盘取出-垂直运行取料口，传感器信号错误，" + "\r\n" +
            "请检查升降机托架感应器是否输出正常")]
        MresultL = 1211,
        [Description("托盘取出-打开安全门，传感器信号错误，" + "\r\n" +
            "请检查安全门到位感应器是否输出正常")]
        MresultM = 1212,
        [Description("托盘存入-初始状态，感应器信号错误，" + "\r\n" +
            "请检查取料口托盘感应器是否有输出正常")]
        MresultN = 1213,
        [Description("托盘存入-初始状态，感应器信号错误，" + "\r\n" +
            "请检查升降机托盘感应器是否有输出正常")]
        MresultO = 1214,
        [Description("托盘存入-初始状态，感应器信号错误，" + "\r\n" +
            "请检查升降机钩爪原点感应器是否有输出正常")]
        MresultP = 1215,
        [Description("托盘存入-对位运行，标志位信号错误，" + "\r\n" +
            "请检查M506是否输出正常")]
        MresultQ = 1216,
        [Description("托盘存入-对位运行，感应器信号错误，" + "\r\n" +
            "请检查升降机托架检测感应器是否有输出正常")]
        MresultR = 1217,
        [Description("托盘存入-打开安全门，感应器信号错误，" + "\r\n" +
            "请检查安全门到位检测感应器是否有输出正常")]
        MresultS = 1218,
        [Description("托盘存入-钩取托盘，感应器信号错误，" + "\r\n" +
            "请检查升降机托盘感应器是否有输出正常")]
        MresultT = 1219,
        [Description("托盘存入-钩取托盘，感应器信号错误，" + "\r\n" +
            "请检查升降机钩爪到位感应器是否有输出正常")]
        MresultU = 1220,
        [Description("托盘存入-关闭安全门，感应器信号错误，" + "\r\n" +
            "请检查安全门原点感应器是否有输出正常")]
        MresultV = 1221,
        [Description("托盘存入-垂直运行，标志位信号错误，" + "\r\n" +
            "M506是否有输出正常")]
        MresultW = 1222,
        [Description("托盘存入-垂直运行，感应器信号错误，" + "\r\n" +
            "请检查升降机托架检测感应器是否有输出正常")]
        MresultX = 1223,
        [Description("托盘存入-退回托盘，感应器信号错误，" + "\r\n" +
            "请检查升降机钩爪原点感应器是否有输出正常")]
        MresultY = 1224,
        [Description("托盘存入-退回托盘，感应器信号错误，" + "\r\n" +
            "请检查升降机托盘感应器是否有输出正常")]
        MresultZ = 1225,

    }

    public enum DeviceAlarmEnum1
    {
        [Description("紧急停止")]
        Urgencye = 200,

        [Description("安全触边动作")]
        Reach = 201,

        [Description("偏载报警")]
        Fault = 202,

        [Description("维修面板开启")]
        Maintain = 203,

        [Description("安全光栏遮挡")]
        DiaphragmKeepOut = 204,

        [Description("电机过热")]
        ElectricalMachineryOverheating = 205,

        [Description("变频器过载")]
        UuivertorOverload = 206,

        [Description("变频器过热")]
        UuivertorOverheating = 207,

        [Description("输入缺相")]
        InputDefaultPhase = 208,

        [Description("输出缺相")]
        OutputDefaultPhase = 209,

        [Description("变频器CPU故障")]
        UuivertorCPU = 210,

        [Description("变频器内部电路故障")]
        UuivertorPowerFailure = 211,

        [Description("报警发生")]
        AlarmHappen = 212,

        [Description("报警重置")]
        AlarmReset = 213,

        [Description("开机报警重置")]
        StartingUpReset = 214,

        [Description("触摸屏确认按钮")]
        Affirm = 241,

    }
}
