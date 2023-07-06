using System.ComponentModel;

namespace Bussiness.Enums
{
  public  enum DeviceStatusEnum
    {
        [Description("初始")]
        None = 0,
        [Description("在线")]
        Running = 1,
        [Description("离线")]
        Fault = 2,
    }
}
